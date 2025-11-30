"""
Collaborative Filtering Model Training
Sử dụng LightFM với WARP loss function
Train trên Google Colab, sau đó download model về local
"""
import pandas as pd
import numpy as np
from lightfm import LightFM
from lightfm.data import Dataset
from lightfm.cross_validation import random_train_test_split
from lightfm.evaluation import precision_at_k, recall_at_k, auc_score
import pickle
import os

def prepare_lightfm_data(interactions_csv="interactions.csv", items_csv="items.csv"):
    """
    Chuẩn bị dữ liệu cho LightFM
    
    Args:
        interactions_csv: File interactions với columns: user_id, item_id, rating, timestamp
        items_csv: File items với columns: item_id, name, description, category, brand, price
    
    Returns:
        interactions, item_features, user_features
    """
    print("📂 Loading data...")
    
    # Load interactions
    interactions_df = pd.read_csv(interactions_csv)
    print(f"✅ Loaded {len(interactions_df)} interactions")
    
    # Load items for features
    items_df = pd.read_csv(items_csv)
    print(f"✅ Loaded {len(items_df)} items")
    
    # Tạo Dataset
    dataset = Dataset()
    dataset.fit(
        users=interactions_df['user_id'].unique(),
        items=interactions_df['item_id'].unique()
    )
    
    # Build interactions matrix (implicit feedback)
    # Nếu có rating, chuyển thành binary (rating > 0)
    if 'rating' in interactions_df.columns:
        interactions_df['interaction'] = (interactions_df['rating'] > 0).astype(int)
    else:
        # Nếu không có rating, coi như tất cả interactions đều positive
        interactions_df['interaction'] = 1
    
    # Build item features
    print("\n🔤 Building item features...")
    item_features_list = []
    for _, item in items_df.iterrows():
        features = []
        if pd.notna(item.get('category')):
            features.append(f"category:{item['category']}")
        if pd.notna(item.get('brand')):
            features.append(f"brand:{item['brand']}")
        if pd.notna(item.get('name')):
            # Lấy keywords từ name (ví dụ: "Royal Canin Adult" -> "royal", "canin", "adult")
            keywords = item['name'].lower().split()[:3]  # Lấy 3 từ đầu
            features.extend([f"keyword:{kw}" for kw in keywords])
        item_features_list.append(features)
    
    # Fit item features
    dataset.fit_partial(items=items_df['item_id'].unique())
    for feature in set([f for features in item_features_list for f in features]):
        dataset.fit_partial(item_features=[feature])
    
    # Build interactions
    print("\n📊 Building interactions matrix...")
    (interactions, weights) = dataset.build_interactions(
        [(row['user_id'], row['item_id'], row['interaction']) 
         for _, row in interactions_df.iterrows()]
    )
    print(f"✅ Interactions matrix shape: {interactions.shape}")
    
    # Build item features
    item_features = dataset.build_item_features(
        [(item_id, features) 
         for item_id, features in zip(items_df['item_id'], item_features_list)]
    )
    print(f"✅ Item features shape: {item_features.shape}")
    
    # User features (optional - có thể thêm sau)
    user_features = None
    
    return interactions, item_features, user_features, dataset


def train_lightfm_model(interactions_csv="interactions.csv", 
                       items_csv="items.csv",
                       output_dir="./models",
                       latent_dim=64,
                       epochs=10,
                       num_threads=4):
    """
    Train LightFM model với WARP loss
    
    Args:
        interactions_csv: File interactions
        items_csv: File items
        output_dir: Thư mục lưu model
        latent_dim: Số latent factors (default: 64)
        epochs: Số epochs training
        num_threads: Số threads
    """
    print("=" * 60)
    print("COLLABORATIVE FILTERING - LightFM Training")
    print("=" * 60)
    
    # Tạo thư mục output
    os.makedirs(output_dir, exist_ok=True)
    
    # Prepare data
    interactions, item_features, user_features, dataset = prepare_lightfm_data(
        interactions_csv, items_csv
    )
    
    # Split train/test
    print("\n✂️ Splitting train/test (80/20)...")
    train, test = random_train_test_split(
        interactions, 
        test_percentage=0.2,
        random_state=42
    )
    print(f"✅ Train interactions: {train.nnz}")
    print(f"✅ Test interactions: {test.nnz}")
    
    # Initialize model
    print(f"\n🤖 Initializing LightFM model (latent_dim={latent_dim}, loss='warp')...")
    model = LightFM(
        loss='warp',
        no_components=latent_dim,
        learning_rate=0.05,
        item_alpha=1e-6,
        user_alpha=1e-6,
        random_state=42
    )
    
    # Train model
    print(f"\n🔄 Training model ({epochs} epochs)...")
    for epoch in range(epochs):
        model.fit_partial(
            train,
            item_features=item_features,
            user_features=user_features,
            epochs=1,
            num_threads=num_threads,
            verbose=True
        )
        
        # Evaluate
        train_precision = precision_at_k(model, train, item_features=item_features, 
                                        user_features=user_features, k=10, num_threads=num_threads).mean()
        test_precision = precision_at_k(model, test, item_features=item_features,
                                        user_features=user_features, k=10, num_threads=num_threads).mean()
        
        print(f"Epoch {epoch+1}/{epochs} - Train Precision@10: {train_precision:.4f}, "
              f"Test Precision@10: {test_precision:.4f}")
    
    # Final evaluation
    print("\n📊 Final Evaluation:")
    train_precision = precision_at_k(model, train, item_features=item_features,
                                    user_features=user_features, k=10, num_threads=num_threads).mean()
    train_recall = recall_at_k(model, train, item_features=item_features,
                              user_features=user_features, k=10, num_threads=num_threads).mean()
    train_auc = auc_score(model, train, item_features=item_features,
                         user_features=user_features, num_threads=num_threads).mean()
    
    test_precision = precision_at_k(model, test, item_features=item_features,
                                    user_features=user_features, k=10, num_threads=num_threads).mean()
    test_recall = recall_at_k(model, test, item_features=item_features,
                             user_features=user_features, k=10, num_threads=num_threads).mean()
    test_auc = auc_score(model, test, item_features=item_features,
                        user_features=user_features, num_threads=num_threads).mean()
    
    print(f"Train - Precision@10: {train_precision:.4f}, Recall@10: {train_recall:.4f}, AUC: {train_auc:.4f}")
    print(f"Test  - Precision@10: {test_precision:.4f}, Recall@10: {test_recall:.4f}, AUC: {test_auc:.4f}")
    
    # Save model
    model_path = os.path.join(output_dir, "lightfm_model.pkl")
    with open(model_path, 'wb') as f:
        pickle.dump(model, f)
    print(f"\n✅ Saved model to {model_path}")
    
    # Save dataset (cần để rebuild interactions sau này)
    dataset_path = os.path.join(output_dir, "lightfm_dataset.pkl")
    with open(dataset_path, 'wb') as f:
        pickle.dump(dataset, f)
    print(f"✅ Saved dataset to {dataset_path}")
    
    # Save metadata
    metadata = {
        'latent_dim': latent_dim,
        'loss': 'warp',
        'train_precision@10': float(train_precision),
        'train_recall@10': float(train_recall),
        'train_auc': float(train_auc),
        'test_precision@10': float(test_precision),
        'test_recall@10': float(test_recall),
        'test_auc': float(test_auc),
        'num_users': interactions.shape[0],
        'num_items': interactions.shape[1],
        'num_interactions': interactions.nnz
    }
    metadata_path = os.path.join(output_dir, "collaborative_metadata.pkl")
    with open(metadata_path, 'wb') as f:
        pickle.dump(metadata, f)
    print(f"✅ Saved metadata to {metadata_path}")
    
    print("\n" + "=" * 60)
    print("✅ COLLABORATIVE FILTERING MODEL TRAINING COMPLETED!")
    print("=" * 60)
    
    return model, dataset, metadata


if __name__ == "__main__":
    # Train model
    # Lưu ý: Chạy trên Google Colab với GPU để nhanh hơn
    model, dataset, metadata = train_lightfm_model(
        interactions_csv="interactions.csv",
        items_csv="items.csv",
        output_dir="./models",
        latent_dim=64,
        epochs=10
    )
    
    print("\n✅ Training completed! Download models from Colab to local machine.")

