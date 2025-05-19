import requests
import json

BASE_URL = "http://127.0.0.1:5000"
user_id = "test123"               # 실제 유저 ID
model_name = "테스트모델"         # 저장한 모델 이름

def print_pretty_json(res):
    try:
        parsed = res.json()
        print(json.dumps(parsed, indent=2, ensure_ascii=False))
    except Exception as e:
        print("응답 파싱 오류:", e)
        print(res.text)

# 저장된 모델 목록 조회
def get_saved_models(user_id):
    res = requests.get(f"{BASE_URL}/get_saved_models", params={"user_id": user_id})
    print("모델 목록 상태코드:", res.status_code)
    print("응답 본문:")
    print_pretty_json(res)

# 이름으로 모델 다운로드
def get_model_by_name(user_id, name):
    res = requests.get(f"{BASE_URL}/get_model_by_name", params={"user_id": user_id, "name": name})
    print("모델 다운로드 상태코드:", res.status_code)
    if res.status_code == 200:
        with open("downloaded_model.obj", "wb") as f:
            f.write(res.content)
        print("모델 파일 저장 완료: downloaded_model.obj")
    else:
        print("다운로드 실패:")
        print_pretty_json(res)

# 실행
get_saved_models(user_id)
get_model_by_name(user_id, model_name)
