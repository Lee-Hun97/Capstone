import sqlite3

# DB 연결 (없으면 새로 생성됨)
conn = sqlite3.connect('database.db')

# 커서 만들기
cursor = conn.cursor()

# 사용자 테이블 생성 (이메일, 비번 저장)
cursor.execute('''
CREATE TABLE IF NOT EXISTS users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    email TEXT NOT NULL,
    password TEXT NOT NULL
)
''')

# 저장하고 종료
conn.commit()
conn.close()

print("✅ users 테이블이 생성되었습니다.")
