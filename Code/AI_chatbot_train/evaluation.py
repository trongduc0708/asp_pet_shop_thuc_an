"""
Evaluation Script
Đánh giá models với Precision@10, Recall@10, và AUC Score
Target: Precision@10 > 0.15, Recall@10 > 0.10, AUC > 0.80
"""
import pandas as pd
import numpy as np
import pickle
from lightfm import LightFM
from lightfm.data import Dataset
from lightfm.cross_validation import random_train_test_split
from lightfm.evaluation import precision_at_k, recall_at_k, auc_score
from sklearn.metrics.pairwise import cosine_similarity
import os

def evaluate_collaborative_model(models_dir="./models", interactions_csv="interactions.csv", items_csv="items.csv"):
    """
    Đánh giá Collaborative Filtering model (LightFM)
    """
    print("=" * 60)
    print("EVALUATING COLLABORATIVE FILTERING MODEL")
    print("=" * 60)
    
    # Load model
    with open(os.path.join(models_dir, "lightfm_model.pkl"), 'rb') as f:
        model = pickle.load(f)
    
    with open(os.path.join(models_dir, "lightfm_dataset.pkl"), 'rb') as f:
        dataset = pickle.load(f)
    
    # Load interactions
    interactions_df = pd.read_csv(interactions_csv)
    interactions_df['interaction'] = (interactions_df.get('rating', 1) > 0).astype(int)
    
    # Rebuild interactions
    interactions, _ = dataset.build_interactions(
        [(row['user_id'], row['item_id'], row['interaction']) 
         for _, row in interactions_df.iterrows()]
    )
    
    # Split train/test
    train, test = random_train_test_split(interactions, test_percentage=0.2, random_state=42)
    
    # Build item features
    items_df = pd.read_csv(items_csv)
    item_features_list = []
    for _, item in items_df.iterrows():
        features = []
        if pd.notna(item.get('category')):
            features.append(f"category:{item['category']}")
        if pd.notna(item.get('brand')):
            features.append(f"brand:{item['brand']}")
        item_features_list.append(features)
    
    item_features = dataset.build_item_features(
        [(item_id, features) 
         for item_id, features in zip(items_df['item_id'], item_features_list)]
    )
    
    # Evaluate
    print("\n📊 Evaluating on test set...")
    test_precision = precision_at_k(model, test, item_features=item_features, k=10, num_threads=4).mean()
    test_recall = recall_at_k(model, test, item_features=item_features, k=10, num_threads=4).mean()
    test_auc = auc_score(model, test, item_features=item_features, num_threads=4).mean()
    
    print(f"\n✅ Results:")
    print(f"  Precision@10: {test_precision:.4f} {'✅' if test_precision > 0.15 else '❌'} (Target: > 0.15)")
    print(f"  Recall@10:    {test_recall:.4f} {'✅' if test_recall > 0.10 else '❌'} (Target: > 0.10)")
    print(f"  AUC Score:    {test_auc:.4f} {'✅' if test_auc > 0.80 else '❌'} (Target: > 0.80)")
    
    return {
        'precision@10': test_precision,
        'recall@10': test_recall,
        'auc': test_auc
    }


def evaluate_content_based_model(models_dir="./models", test_interactions_csv="interactions.csv"):
    """
    Đánh giá Content-Based model (Sentence-BERT)
    """
    print("\n" + "=" * 60)
    print("EVALUATING CONTENT-BASED MODEL")
    print("=" * 60)
    
    # Load embeddings và similarity matrix
    item_embeddings = np.load(os.path.join(models_dir, "item_embeddings.npy"))
    similarity_matrix = np.load(os.path.join(models_dir, "similarity_matrix.npy"))
    
    with open(os.path.join(models_dir, "item_mapping.pkl"), 'rb') as f:
        item_mapping = pickle.load(f)
    
    # Load test interactions
    test_df = pd.read_csv(test_interactions_csv)
    # Chỉ lấy positive interactions
    test_df = test_df[test_df.get('rating', 1) > 0]
    
    # Sample test set
    test_df = test_df.sample(min(1000, len(test_df)), random_state=42)
    
    # Evaluate
    print("\n📊 Evaluating on test set...")
    precisions = []
    recalls = []
    
    for _, row in test_df.iterrows():
        user_id = row['user_id']
        true_item_id = row['item_id']
        
        # Tìm items tương tự
        true_item_idx = item_mapping['item2idx'].get(true_item_id)
        if true_item_idx is None:
            continue
        
        # Lấy top 10 similar items
        similar_indices = np.argsort(similarity_matrix[true_item_idx])[::-1][1:11]  # Exclude itself
        recommended_items = [item_mapping['item_id'][idx] for idx in similar_indices]
        
        # Tính precision và recall
        if true_item_id in recommended_items:
            precisions.append(1.0)
            recalls.append(1.0)
        else:
            precisions.append(0.0)
            recalls.append(0.0)
    
    avg_precision = np.mean(precisions) if precisions else 0.0
    avg_recall = np.mean(recalls) if recalls else 0.0
    
    print(f"\n✅ Results:")
    print(f"  Precision@10: {avg_precision:.4f} {'✅' if avg_precision > 0.15 else '❌'} (Target: > 0.15)")
    print(f"  Recall@10:    {avg_recall:.4f} {'✅' if avg_recall > 0.10 else '❌'} (Target: > 0.10)")
    
    return {
        'precision@10': avg_precision,
        'recall@10': avg_recall
    }


def evaluate_hybrid_model(models_dir="./models", interactions_csv="interactions.csv"):
    """
    Đánh giá Hybrid model
    """
    print("\n" + "=" * 60)
    print("EVALUATING HYBRID MODEL")
    print("=" * 60)
    
    # Load hybrid recommender
    with open(os.path.join(models_dir, "hybrid_recommender.pkl"), 'rb') as f:
        recommender = pickle.load(f)
    
    # Load test interactions
    test_df = pd.read_csv(interactions_csv)
    test_df = test_df[test_df.get('rating', 1) > 0]
    test_df = test_df.sample(min(1000, len(test_df)), random_state=42)
    
    # Count interactions per user
    user_interactions = test_df.groupby('user_id').size().to_dict()
    
    # Evaluate
    print("\n📊 Evaluating on test set...")
    precisions = []
    recalls = []
    
    for _, row in test_df.iterrows():
        user_id = row['user_id']
        true_item_id = row['item_id']
        num_interactions = user_interactions.get(user_id, 0)
        
        # Get recommendations
        recommendations = recommender.recommend(
            user_id=user_id,
            num_interactions=num_interactions,
            k=10
        )
        recommended_items = [item_id for item_id, _ in recommendations]
        
        # Tính precision và recall
        if true_item_id in recommended_items:
            precisions.append(1.0)
            recalls.append(1.0)
        else:
            precisions.append(0.0)
            recalls.append(0.0)
    
    avg_precision = np.mean(precisions) if precisions else 0.0
    avg_recall = np.mean(recalls) if recalls else 0.0
    
    print(f"\n✅ Results:")
    print(f"  Precision@10: {avg_precision:.4f} {'✅' if avg_precision > 0.15 else '❌'} (Target: > 0.15)")
    print(f"  Recall@10:    {avg_recall:.4f} {'✅' if avg_recall > 0.10 else '❌'} (Target: > 0.10)")
    
    return {
        'precision@10': avg_precision,
        'recall@10': avg_recall
    }


def generate_evaluation_report(models_dir="./models", 
                               interactions_csv="interactions.csv",
                               items_csv="items.csv",
                               output_path="./evaluation_report.txt"):
    """
    Tạo báo cáo đánh giá tổng hợp
    """
    print("\n" + "=" * 60)
    print("GENERATING EVALUATION REPORT")
    print("=" * 60)
    
    results = {}
    
    # Evaluate Collaborative
    try:
        results['collaborative'] = evaluate_collaborative_model(models_dir, interactions_csv, items_csv)
    except Exception as e:
        print(f"❌ Error evaluating collaborative model: {e}")
        results['collaborative'] = None
    
    # Evaluate Content-Based
    try:
        results['content_based'] = evaluate_content_based_model(models_dir, interactions_csv)
    except Exception as e:
        print(f"❌ Error evaluating content-based model: {e}")
        results['content_based'] = None
    
    # Evaluate Hybrid
    try:
        results['hybrid'] = evaluate_hybrid_model(models_dir, interactions_csv)
    except Exception as e:
        print(f"❌ Error evaluating hybrid model: {e}")
        results['hybrid'] = None
    
    # Generate report
    report = []
    report.append("=" * 60)
    report.append("EVALUATION REPORT - RECOMMENDATION SYSTEM")
    report.append("=" * 60)
    report.append("")
    
    if results['collaborative']:
        report.append("COLLABORATIVE FILTERING (LightFM):")
        report.append(f"  Precision@10: {results['collaborative']['precision@10']:.4f}")
        report.append(f"  Recall@10:    {results['collaborative']['recall@10']:.4f}")
        report.append(f"  AUC Score:    {results['collaborative']['auc']:.4f}")
        report.append("")
    
    if results['content_based']:
        report.append("CONTENT-BASED (Sentence-BERT):")
        report.append(f"  Precision@10: {results['content_based']['precision@10']:.4f}")
        report.append(f"  Recall@10:    {results['content_based']['recall@10']:.4f}")
        report.append("")
    
    if results['hybrid']:
        report.append("HYBRID (LightFM + Sentence-BERT):")
        report.append(f"  Precision@10: {results['hybrid']['precision@10']:.4f}")
        report.append(f"  Recall@10:    {results['hybrid']['recall@10']:.4f}")
        report.append("")
    
    report.append("=" * 60)
    report.append("TARGET METRICS:")
    report.append("  Precision@10 > 0.15")
    report.append("  Recall@10    > 0.10")
    report.append("  AUC Score    > 0.80")
    report.append("=" * 60)
    
    # Save report
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write('\n'.join(report))
    
    print(f"\n✅ Evaluation report saved to {output_path}")
    
    # Print report
    print("\n" + '\n'.join(report))
    
    return results


if __name__ == "__main__":
    # Run evaluation
    results = generate_evaluation_report(
        models_dir="./models",
        interactions_csv="interactions.csv",
        items_csv="items.csv",
        output_path="./evaluation_report.txt"
    )

