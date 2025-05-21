import requests
from app import build_assetbundle

user_id = "test@example.com"
server_url = "http://127.0.0.1:5000"

# 최신 timestamp 받아오기
def get_latest_timestamp(user_id):
    response = requests.post(f"{server_url}/latest_upload", data={'user_id': user_id})
    data = response.json()
    if data['status'] != 'ok':
        print(f"[에러] 최신 timestamp 조회 실패: {data.get('message', '')}")
        return None
    return data['timestamp']

# 실행
timestamp = get_latest_timestamp(user_id)
if not timestamp:
    exit(1)

print(f"[INFO] 최신 timestamp: {timestamp}")

success, msg = build_assetbundle(user_id, timestamp)
if success:
    print(f"[성공] 번들 빌드 완료: user_id={user_id}, timestamp={timestamp}")
else:
    print(f"[실패] 번들 빌드 실패: {msg}")
