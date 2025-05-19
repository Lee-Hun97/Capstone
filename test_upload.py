import requests
import os

url = 'http://127.0.0.1:5000/upload'
data = {'user_id': 'test@example.com'}

files = []

for filename in os.listdir('tests'):
    if filename.lower().endswith(('.jpg', '.jpeg', '.png')):
        filepath = os.path.join('tests', filename)
        f = open(filepath, 'rb')
        files.append(('file', f))

response = requests.post(url, files=files, data=data)

# 작업 끝난 후 파일 닫기
for _, f in files:
    f.close()

print("상태코드:", response.status_code)
print("응답:", response.json())
print("업로드 준비된 파일 수:", len(files))
print("보낼 유저 ID:", data['user_id'])
