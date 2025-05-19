import requests

user_id = "test@example.com"

# STEP 1: 서버에 최신 업로드 timestamp 요청
latest_url = "http://127.0.0.1:5000/latest_upload"
latest_res = requests.post(latest_url, data={"user_id": user_id})

if latest_res.status_code != 200:
    print("최신 timestamp 요청 실패:", latest_res.json())
    exit()

timestamp = latest_res.json()['timestamp']
print(f"자동으로 받은 timestamp: {timestamp}")

# STEP 2: /process 요청으로 모델 생성 시작
process_url = "http://127.0.0.1:5000/process_rc"
process_res = requests.post(process_url, data={
    "user_id": user_id,
    "timestamp": timestamp
})

print("모델링 응답:", process_res.status_code)
print(process_res.json())
