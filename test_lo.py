import requests

url = 'http://127.0.0.1:5000/login'
data = {
    'email': 'test@example.com',
    'password': '1234'
}

response = requests.post(url, json=data)
print("Status Code:", response.status_code)
print("Raw Response:", repr(response.text))

try:
    print("Parsed JSON:", response.json())
except Exception as e:
    print("JSON Decode Error:", e)
