"""
Content-Based Filtering Model Training
Sử dụng Sentence-BERT (paraphrase-multilingual-MiniLM-L12-v2) để tạo embeddings
Train trên Google Colab, sau đó download models về local
"""
import pandas as pd
import numpy as np
from sentence_transformers import SentenceTransformer
from sklearn.metrics.pairwise import cosine_similarity
import os
import pickle

def create_item_embeddings(items_csv_path="items.csv", output_dir="./models"):
    """
    Tạo embeddings cho items sử dụng Sentence-BERT
    
    Args:
        items_csv_path: Đường dẫn đến file items.csv
        output_dir: Thư mục lưu output
    """
    print("=" * 60)
    print("CONTENT-BASED FILTERING - Sentence-BERT Training")
    print("=" * 60)
    
    # Tạo thư mục output
    os.makedirs(output_dir, exist_ok=True)
    
    # Load data
    print(f"\n📂 Loading items from {items_csv_path}...")
    df = pd.read_csv(items_csv_path)
    print(f"✅ Loaded {len(df)} items")
    
    # Kiểm tra columns cần thiết
    required_cols = ['item_id', 'name', 'description', 'category', 'brand']
    missing_cols = [col for col in required_cols if col not in df.columns]
    if missing_cols:
        raise ValueError(f"Missing required columns: {missing_cols}")
    
    # Tạo text features từ các đặc điểm sản phẩm
    print("\n🔤 Creating text features from product attributes...")
    df['text_features'] = (
        df['name'].fillna('') + ' ' +
        df['description'].fillna('') + ' ' +
        df['category'].fillna('') + ' ' +
        df['brand'].fillna('')
    )
    
    # Load Sentence-BERT model (multilingual)
    print("\n🤖 Loading Sentence-BERT model (paraphrase-multilingual-MiniLM-L12-v2)...")
    model = SentenceTransformer('paraphrase-multilingual-MiniLM-L12-v2')
    print("✅ Model loaded successfully")
    
    # Tạo embeddings
    print("\n🔄 Generating embeddings (this may take a while)...")
    texts = df['text_features'].tolist()
    embeddings = model.encode(
        texts,
        show_progress_bar=True,
        batch_size=32,
        convert_to_numpy=True
    )
    print(f"✅ Generated embeddings shape: {embeddings.shape}")
    
    # Tính similarity matrix
    print("\n📊 Computing similarity matrix...")
    similarity_matrix = cosine_similarity(embeddings)
    print(f"✅ Similarity matrix shape: {similarity_matrix.shape}")
    
    # Lưu embeddings
    embeddings_path = os.path.join(output_dir, "item_embeddings.npy")
    np.save(embeddings_path, embeddings)
    print(f"✅ Saved embeddings to {embeddings_path}")
    
    # Lưu similarity matrix
    similarity_path = os.path.join(output_dir, "similarity_matrix.npy")
    np.save(similarity_path, similarity_matrix)
    print(f"✅ Saved similarity matrix to {similarity_path}")
    
    # Lưu item mapping
    item_mapping = {
        'item_id': df['item_id'].tolist(),
        'item2idx': {item_id: idx for idx, item_id in enumerate(df['item_id'])},
        'idx2item': {idx: item_id for idx, item_id in enumerate(df['item_id'])}
    }
    mapping_path = os.path.join(output_dir, "item_mapping.pkl")
    with open(mapping_path, 'wb') as f:
        pickle.dump(item_mapping, f)
    print(f"✅ Saved item mapping to {mapping_path}")
    
    # Lưu metadata
    metadata = {
        'model_name': 'paraphrase-multilingual-MiniLM-L12-v2',
        'embedding_dim': embeddings.shape[1],
        'num_items': len(df),
        'items_csv': items_csv_path
    }
    metadata_path = os.path.join(output_dir, "content_metadata.pkl")
    with open(metadata_path, 'wb') as f:
        pickle.dump(metadata, f)
    print(f"✅ Saved metadata to {metadata_path}")
    
    print("\n" + "=" * 60)
    print("✅ CONTENT-BASED MODEL TRAINING COMPLETED!")
    print("=" * 60)
    print(f"\n📦 Output files:")
    print(f"  - {embeddings_path}")
    print(f"  - {similarity_path}")
    print(f"  - {mapping_path}")
    print(f"  - {metadata_path}")
    
    return embeddings, similarity_matrix, item_mapping


if __name__ == "__main__":
    # Train model
    # Lưu ý: Chạy trên Google Colab với GPU để nhanh hơn
    embeddings, similarity, mapping = create_item_embeddings(
        items_csv_path="items.csv",
        output_dir="./models"
    )
    
    # Test similarity
    print("\n🧪 Testing similarity...")
    test_item_idx = 0
    similar_items = np.argsort(similarity[test_item_idx])[::-1][1:6]  # Top 5 similar (exclude itself)
    print(f"Item {mapping['item_id'][test_item_idx]} is similar to:")
    for idx in similar_items:
        print(f"  - {mapping['item_id'][idx]} (similarity: {similarity[test_item_idx][idx]:.4f})")

