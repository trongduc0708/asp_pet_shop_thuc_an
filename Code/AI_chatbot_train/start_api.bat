@echo off
echo Starting Recommendation API Server...
python -m uvicorn recommendation_api:app --host 0.0.0.0 --port 8081 --reload
pause

