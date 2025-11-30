"""
Hybrid Recommendation System Training
Kết hợp LightFM (Collaborative) + Sentence-BERT (Content-Based)
với Dynamic Weighting dựa trên số interactions của user
"""
import pandas as pd
import numpy as np
import pickle
import os
from lightfm import LightFM
from lightfm.data import Dataset
from sklearn.metrics.pairwise import cosine_similarity

def calculate_dynamic_alpha(num_interactions):
    """
    Tính alpha (weight cho Collaborative Filtering) dựa trên số interactions
    
    Args:
        num_interactions: Số lượng interactions của user
    
    Returns:
        alpha: Weight cho CF (0-1), (1-alpha) là weight cho Content-Based
    """
    if num_interactions == 0:
        # User mới: 100% Content-Based
        return 0.0
    elif num_interactions <= 5:
        # User ít tương tác: 20% CF, 80% Content
        return 0.2
    elif num_interactions <= 20:
        # User trung bình: 50% CF, 50% Content
        return 0.5
    else:
        # User active: 70% CF, 30% Content
        return 0.7


def load_models(models_dir="./models"):
    """
    Load các models đã train
    
    Returns:
        lightfm_model, lightfm_dataset, item_embeddings, similarity_matrix, item_mapping
    """
    print("📂 Loading trained models...")
    
    # Load LightFM model
    with open(os.path.join(models_dir, "lightfm_model.pkl"), 'rb') as f:
        lightfm_model = pickle.load(f)
    print("✅ Loaded LightFM model")
    
    # Load LightFM dataset
    with open(os.path.join(models_dir, "lightfm_dataset.pkl"), 'rb') as f:
        lightfm_dataset = pickle.load(f)
    print("✅ Loaded LightFM dataset")
    
    # Load item embeddings
    item_embeddings = np.load(os.path.join(models_dir, "item_embeddings.npy"))
    print(f"✅ Loaded item embeddings: {item_embeddings.shape}")
    
    # Load similarity matrix
    similarity_matrix = np.load(os.path.join(models_dir, "similarity_matrix.npy"))
    print(f"✅ Loaded similarity matrix: {similarity_matrix.shape}")
    
    # Load item mapping
    with open(os.path.join(models_dir, "item_mapping.pkl"), 'rb') as f:
        item_mapping = pickle.load(f)
    print("✅ Loaded item mapping")
    
    return lightfm_model, lightfm_dataset, item_embeddings, similarity_matrix, item_mapping


def get_popular_items(interactions_csv="interactions.csv", top_k=50):
    """
    Lấy danh sách sản phẩm phổ biến (cho Cold Start)
    
    Args:
        interactions_csv: File interactions
        top_k: Số lượng sản phẩm phổ biến
    
    Returns:
        popular_item_ids: List item_id của sản phẩm phổ biến
    """
    df = pd.read_csv(interactions_csv)
    popular = df.groupby('item_id').size().sort_values(ascending=False).head(top_k)
    return popular.index.tolist()


class HybridRecommender:
    """
    Hybrid Recommender System
    Kết hợp Collaborative Filtering (LightFM) và Content-Based (Sentence-BERT)
    với Dynamic Weighting
    """
    
    def __init__(self, models_dir="./models", interactions_csv="interactions.csv"):
        print("=" * 60)
        print("INITIALIZING HYBRID RECOMMENDER SYSTEM")
        print("=" * 60)
        
        # Load models
        self.lightfm_model, self.lightfm_dataset, self.item_embeddings, \
        self.similarity_matrix, self.item_mapping = load_models(models_dir)
        
        # Load popular items
        self.popular_items = get_popular_items(interactions_csv)
        
        # Build item features for LightFM
        self._build_item_features()
        
        print("✅ Hybrid Recommender initialized successfully!")
    
    def _build_item_features(self):
        """Build item features từ dataset"""
        # Item features đã được build trong train_collaborative.py
        # Chỉ cần lưu reference
        pass
    
    def _get_cf_scores(self, user_id, num_items):
        """
        Lấy scores từ Collaborative Filtering (LightFM)
        
        Args:
            user_id: ID của user
            num_items: Số lượng items
        
        Returns:
            scores: Array scores cho tất cả items
        """
        if self.lightfm_model is None or self.lightfm_dataset is None:
            return np.zeros(num_items)
        
        try:
            # Get user internal ID
            user_mapping = self.lightfm_dataset.mapping()[0]
            user_internal_id = user_mapping.get(user_id)
            
            if user_internal_id is None:
                # User mới, không có trong training data
                return np.zeros(num_items)
            
            # Predict scores
            scores = self.lightfm_model.predict(
                user_internal_id,
                np.arange(num_items),
                item_features=None  # Có thể thêm item features nếu cần
            )
            return scores
        except Exception as e:
            print(f"Warning: Error getting CF scores for user {user_id}: {e}")
            return np.zeros(num_items)
    
    def _get_content_scores(self, item_id, num_items):
        """
        Lấy scores từ Content-Based (Sentence-BERT similarity)
        
        Args:
            item_id: ID của item hiện tại (nếu có)
            num_items: Số lượng items
        
        Returns:
            scores: Array scores cho tất cả items
        """
        if item_id is None:
            # Không có item reference, trả về zeros
            return np.zeros(num_items)
        
        # Tìm index của item
        item_idx = self.item_mapping['item2idx'].get(item_id)
        if item_idx is None:
            return np.zeros(num_items)
        
        # Lấy similarity scores
        scores = self.similarity_matrix[item_idx]
        return scores
    
    def recommend(self, user_id, current_item_id=None, num_interactions=0, k=10):
        """
        Hybrid recommendation với dynamic weighting
        
        Args:
            user_id: ID của user
            current_item_id: ID của item hiện tại (optional)
            num_interactions: Số lượng interactions của user (để tính alpha)
            k: Số lượng recommendations
        
        Returns:
            recommendations: List of (item_id, score) tuples
        """
        num_items = len(self.item_mapping['item_id'])
        
        # Tính dynamic alpha
        alpha = calculate_dynamic_alpha(num_interactions)
        
        # Cold Start: User mới (0 interactions)
        if num_interactions == 0:
            # 100% Content-Based + Popular Items
            if current_item_id:
                # Có item reference, dùng content similarity
                content_scores = self._get_content_scores(current_item_id, num_items)
                top_indices = np.argsort(content_scores)[::-1][:k]
                recommendations = [
                    (self.item_mapping['item_id'][idx], float(content_scores[idx]))
                    for idx in top_indices
                ]
            else:
                # Không có item reference, trả về popular items
                popular_indices = [
                    self.item_mapping['item2idx'].get(item_id) 
                    for item_id in self.popular_items 
                    if self.item_mapping['item2idx'].get(item_id) is not None
                ][:k]
                recommendations = [
                    (self.item_mapping['item_id'][idx], 1.0) 
                    for idx in popular_indices
                ]
        else:
            # Có interactions, dùng hybrid
            cf_scores = self._get_cf_scores(user_id, num_items)
            content_scores = self._get_content_scores(current_item_id, num_items)
            
            # Normalize scores
            if cf_scores.max() > 0:
                cf_scores = (cf_scores - cf_scores.min()) / (cf_scores.max() - cf_scores.min() + 1e-8)
            if content_scores.max() > 0:
                content_scores = (content_scores - content_scores.min()) / (content_scores.max() - content_scores.min() + 1e-8)
            
            # Hybrid combination
            hybrid_scores = alpha * cf_scores + (1 - alpha) * content_scores
            top_indices = np.argsort(hybrid_scores)[::-1][:k]
            
            # Convert to item_ids
            recommendations = [
                (self.item_mapping['item_id'][idx], float(hybrid_scores[idx]))
                for idx in top_indices
            ]
        
        return recommendations


def save_hybrid_recommender(models_dir="./models", interactions_csv="interactions.csv", output_path="./models/hybrid_recommender.pkl"):
    """
    Save hybrid recommender system
    
    Args:
        models_dir: Thư mục chứa models
        interactions_csv: File interactions để lấy popular items
        output_path: Đường dẫn lưu hybrid recommender
    """
    print("=" * 60)
    print("SAVING HYBRID RECOMMENDER SYSTEM")
    print("=" * 60)
    
    recommender = HybridRecommender(models_dir, interactions_csv)
    
    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    with open(output_path, 'wb') as f:
        pickle.dump(recommender, f)
    
    print(f"✅ Saved hybrid recommender to {output_path}")
    print("\n" + "=" * 60)
    print("✅ HYBRID RECOMMENDER SYSTEM READY!")
    print("=" * 60)


if __name__ == "__main__":
    # Save hybrid recommender
    save_hybrid_recommender(
        models_dir="./models",
        interactions_csv="interactions.csv",
        output_path="./models/hybrid_recommender.pkl"
    )
    
    # Test
    print("\n🧪 Testing hybrid recommender...")
    with open("./models/hybrid_recommender.pkl", 'rb') as f:
        recommender = pickle.load(f)
    
    # Test với user mới (cold start)
    print("\n1. Cold Start (user mới, 0 interactions):")
    recs = recommender.recommend(user_id=9999, num_interactions=0, k=5)
    print(f"   Recommendations: {recs}")
    
    # Test với user ít tương tác
    print("\n2. User ít tương tác (3 interactions):")
    recs = recommender.recommend(user_id=1, num_interactions=3, k=5)
    print(f"   Recommendations: {recs}")
    
    # Test với user active
    print("\n3. User active (25 interactions):")
    recs = recommender.recommend(user_id=1, num_interactions=25, k=5)
    print(f"   Recommendations: {recs}")

