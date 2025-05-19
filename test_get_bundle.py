import requests

# 서버 주소 설정
url = "http://localhost:5000/get_bundle"
params = {
    'user_id': 'test@example.com',
    'timestamp': '20250519_150000'
}

# 요청
response = requests.get(url, params=params)

# 결과 처리
if response.status_code == 200:
    with open("downloaded_modelbundle", "wb") as f:
        f.write(response.content)
    print("번들 다운로드 성공. 파일 저장됨: downloaded_modelbundle")
else:
    print("번들 다운로드 실패")
    print("상태코드:", response.status_code)
    print("응답 내용:", response.text)
