import requests

BASE = "http://127.0.0.1:5000"
user_id = "test@example.com"
model_name = "테스트모델"

def save_model(user_id, name):
    res = requests.post(f"{BASE}/save_model", data={"user_id": user_id, "name": name})
    print("모델 저장 상태코드:", res.status_code)
    print("응답 본문:", res.text)  # ← JSON decode 전에 이걸 먼저 보기

save_model(user_id, model_name)
