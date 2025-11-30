"""
Python API Server để serve Recommendation Models
Sử dụng FastAPI để tạo REST API cho ASP.NET gọi

Models:
- LightFM (Collaborative Filtering)
- Sentence-BERT (Content-Based Filtering)
- Hybrid với Dynamic Weighting
"""
from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from typing import List, Optional
import pickle
import numpy as np
import pandas as pd
import os

app = FastAPI(title="Pet Shop Recommendation API")

# CORS middleware để cho phép ASP.NET gọi
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Trong production nên giới hạn domain
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# ========== Global Model Instances ==========
lightfm_model = None
lightfm_dataset = None
item_embeddings = None
similarity_matrix = None
item_mapping = None
popular_items = []
interactions_df = None


def calculate_dynamic_alpha(num_interactions):
    """
    Tính alpha (weight cho Collaborative Filtering) dựa trên số interactions
    
    Args:
        num_interactions: Số lượng interactions của user
    
    Returns:
        alpha: Weight cho CF (0-1), (1-alpha) là weight cho Content-Based
    """
    if num_interactions == 0:
        return 0.0  # User mới: 100% Content-Based
    elif num_interactions <= 5:
        return 0.2  # User ít tương tác: 20% CF, 80% Content
    elif num_interactions <= 20:
        return 0.5  # User trung bình: 50% CF, 50% Content
    else:
        return 0.7  # User active: 70% CF, 30% Content


def get_user_interaction_count(user_id):
    """
    Đếm số interactions của user
    
    Args:
        user_id: ID của user
    
    Returns:
        count: Số lượng interactions
    """
    if interactions_df is None:
        return 0
    return len(interactions_df[interactions_df['user_id'] == user_id])


def get_cf_scores(user_id, num_items):
    """
    Lấy scores từ Collaborative Filtering (LightFM)
    
    Args:
        user_id: ID của user
        num_items: Số lượng items
    
    Returns:
        scores: Array scores cho tất cả items
    """
    if lightfm_model is None or lightfm_dataset is None:
        return np.zeros(num_items)
    
    try:
        # Get user internal ID
        user_mapping = lightfm_dataset.mapping()[0]
        user_internal_id = user_mapping.get(user_id)
        
        if user_internal_id is None:
            # User mới, không có trong training data
            return np.zeros(num_items)
        
        # Predict scores
        scores = lightfm_model.predict(
            user_internal_id,
            np.arange(num_items),
            item_features=None
        )
        return scores
    except Exception as e:
        print(f"Warning: Error getting CF scores for user {user_id}: {e}")
        return np.zeros(num_items)


def get_content_scores(item_id, num_items):
    """
    Lấy scores từ Content-Based (Sentence-BERT similarity)
    
    Args:
        item_id: ID của item hiện tại (nếu có)
        num_items: Số lượng items
    
    Returns:
        scores: Array scores cho tất cả items
    """
    if similarity_matrix is None or item_mapping is None:
        return np.zeros(num_items)
    
    if item_id is None:
        return np.zeros(num_items)
    
    # Tìm index của item
    item_idx = item_mapping['item2idx'].get(item_id)
    if item_idx is None:
        return np.zeros(num_items)
    
    # Lấy similarity scores
    scores = similarity_matrix[item_idx]
    return scores


def hybrid_recommend(user_id, current_item_id=None, num_interactions=None, k=10):
    """
    Hybrid recommendation với dynamic weighting
    
    Args:
        user_id: ID của user
        current_item_id: ID của item hiện tại (optional)
        num_interactions: Số lượng interactions của user (nếu None sẽ tự động đếm)
        k: Số lượng recommendations
    
    Returns:
        recommendations: List of (item_id, score) tuples
    """
    if item_mapping is None:
        return []
    
    num_items = len(item_mapping['item_id'])
    
    # Đếm interactions nếu chưa có
    if num_interactions is None:
        num_interactions = get_user_interaction_count(user_id)
    
    # Tính dynamic alpha
    alpha = calculate_dynamic_alpha(num_interactions)
    
    # Cold Start: User mới (0 interactions)
    if num_interactions == 0:
        # 100% Content-Based + Popular Items
        if current_item_id:
            # Có item reference, dùng content similarity
            content_scores = get_content_scores(current_item_id, num_items)
            top_indices = np.argsort(content_scores)[::-1][:k]
            recommendations = [
                (item_mapping['item_id'][idx], float(content_scores[idx]))
                for idx in top_indices
            ]
        else:
            # Không có item reference, trả về popular items
            popular_indices = [
                item_mapping['item2idx'].get(item_id) 
                for item_id in popular_items 
                if item_mapping['item2idx'].get(item_id) is not None
            ][:k]
            recommendations = [
                (item_mapping['item_id'][idx], 1.0) 
                for idx in popular_indices
            ]
    else:
        # Có interactions, dùng hybrid
        cf_scores = get_cf_scores(user_id, num_items)
        content_scores = get_content_scores(current_item_id, num_items)
        
        # Normalize scores
        if cf_scores.max() > 0:
            cf_scores = (cf_scores - cf_scores.min()) / (cf_scores.max() - cf_scores.min() + 1e-8)
        if content_scores.max() > 0:
            content_scores = (content_scores - content_scores.min()) / (content_scores.max() - content_scores.min() + 1e-8)
        
        # Hybrid combination
        hybrid_scores = alpha * cf_scores + (1 - alpha) * content_scores
        top_indices = np.argsort(hybrid_scores)[::-1][:k]
        
        recommendations = [
            (item_mapping['item_id'][idx], float(hybrid_scores[idx]))
            for idx in top_indices
        ]
    
    return recommendations


def load_models():
    """Load models khi server khởi động"""
    global lightfm_model, lightfm_dataset, item_embeddings, similarity_matrix, item_mapping, popular_items, interactions_df
    
    try:
        base_path = os.path.dirname(os.path.abspath(__file__))
        models_dir = os.path.join(base_path, "models")
        
        # Load LightFM model
        lightfm_path = os.path.join(models_dir, "lightfm_model.pkl")
        if os.path.exists(lightfm_path):
            with open(lightfm_path, 'rb') as f:
                lightfm_model = pickle.load(f)
            print("✅ Loaded LightFM model")
        
        # Load LightFM dataset
        dataset_path = os.path.join(models_dir, "lightfm_dataset.pkl")
        if os.path.exists(dataset_path):
            with open(dataset_path, 'rb') as f:
                lightfm_dataset = pickle.load(f)
            print("✅ Loaded LightFM dataset")
        
        # Load item embeddings
        embeddings_path = os.path.join(models_dir, "item_embeddings.npy")
        if os.path.exists(embeddings_path):
            item_embeddings = np.load(embeddings_path)
            print(f"✅ Loaded item embeddings: {item_embeddings.shape}")
        
        # Load similarity matrix
        similarity_path = os.path.join(models_dir, "similarity_matrix.npy")
        if os.path.exists(similarity_path):
            similarity_matrix = np.load(similarity_path)
            print(f"✅ Loaded similarity matrix: {similarity_matrix.shape}")
        
        # Load item mapping
        mapping_path = os.path.join(models_dir, "item_mapping.pkl")
        if os.path.exists(mapping_path):
            with open(mapping_path, 'rb') as f:
                item_mapping = pickle.load(f)
            print("✅ Loaded item mapping")
        
        # Load popular items
        interactions_path = os.path.join(base_path, "interactions.csv")
        if os.path.exists(interactions_path):
            interactions_df = pd.read_csv(interactions_path)
            popular_items = interactions_df.groupby('item_id').size().sort_values(ascending=False).head(50).index.tolist()
            print(f"✅ Loaded {len(popular_items)} popular items")
        
        print("✅ All models loaded successfully!")
        return True
    except Exception as e:
        print(f"❌ Error loading models: {e}")
        import traceback
        traceback.print_exc()
        return False


@app.on_event("startup")
async def startup_event():
    load_models()


# ========== API Models ==========
class RecommendationRequest(BaseModel):
    user_id: int
    current_item_id: Optional[str] = None
    k: int = 10
    num_interactions: Optional[int] = None  # Optional: nếu không có sẽ tự động đếm


class RecommendationResponse(BaseModel):
    recommendations: List[dict]
    success: bool
    message: str
    alpha: Optional[float] = None  # Weight cho CF (để debug)


# ========== API Endpoints ==========
@app.get("/")
async def root():
    return {
        "message": "Pet Shop Recommendation API",
        "status": "running",
        "models_loaded": lightfm_model is not None and item_embeddings is not None
    }


@app.get("/health")
async def health():
    return {
        "status": "healthy",
        "models_loaded": lightfm_model is not None and item_embeddings is not None,
        "lightfm_loaded": lightfm_model is not None,
        "content_based_loaded": item_embeddings is not None
    }


@app.post("/recommend", response_model=RecommendationResponse)
async def get_recommendations(request: RecommendationRequest):
    """
    Lấy recommendations cho user với Hybrid approach
    """
    if lightfm_model is None and item_embeddings is None:
        raise HTTPException(status_code=503, detail="Models not loaded")
    
    try:
        # Get recommendations
        num_interactions = request.num_interactions
        if num_interactions is None:
            num_interactions = get_user_interaction_count(request.user_id)
        
        recommendations = hybrid_recommend(
            user_id=request.user_id,
            current_item_id=request.current_item_id,
            num_interactions=num_interactions,
            k=request.k
        )
        
        # Calculate alpha for response
        alpha = calculate_dynamic_alpha(num_interactions)
        
        # Format response
        result = [
            {
                "item_id": item_id,
                "score": score
            }
            for item_id, score in recommendations
        ]
        
        return RecommendationResponse(
            recommendations=result,
            success=True,
            message="Recommendations generated successfully",
            alpha=alpha
        )
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error generating recommendations: {str(e)}")


@app.post("/recommend/batch")
async def get_batch_recommendations(requests: List[RecommendationRequest]):
    """
    Lấy recommendations cho nhiều users cùng lúc
    """
    if lightfm_model is None and item_embeddings is None:
        raise HTTPException(status_code=503, detail="Models not loaded")
    
    results = []
    for req in requests:
        try:
            num_interactions = req.num_interactions
            if num_interactions is None:
                num_interactions = get_user_interaction_count(req.user_id)
            
            recommendations = hybrid_recommend(
                user_id=req.user_id,
                current_item_id=req.current_item_id,
                num_interactions=num_interactions,
                k=req.k
            )
            
            results.append({
                "user_id": req.user_id,
                "recommendations": [
                    {"item_id": item_id, "score": score}
                    for item_id, score in recommendations
                ],
                "success": True,
                "alpha": calculate_dynamic_alpha(num_interactions)
            })
        except Exception as e:
            results.append({
                "user_id": req.user_id,
                "recommendations": [],
                "success": False,
                "error": str(e)
            })
    
    return {"results": results}


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8081)
