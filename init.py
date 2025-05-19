# SQLite DB 구조 정리 및 적용 (Flask)
# users, models 테이블 생성 및 외래키 설정

import sqlite3

def initialize_database():
    conn = sqlite3.connect('database.db')
    conn.execute('PRAGMA foreign_keys = ON')
    cur = conn.cursor()

    # 1. 사용자 테이블
    cur.execute('''
        CREATE TABLE IF NOT EXISTS users (
            user_id TEXT PRIMARY KEY,
            email TEXT UNIQUE,
            password TEXT
        )
    ''')

    # 2. 모델 테이블
    cur.execute('''
        CREATE TABLE IF NOT EXISTS models (
            model_id TEXT PRIMARY KEY,
            name TEXT,
            user_id TEXT,
            timestamp TEXT,
            created_at TEXT DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (user_id) REFERENCES users(user_id)
        )
    ''')

    conn.commit()
    conn.close()

# 실행 예시
if __name__ == '__main__':
    initialize_database()
    print("DB 초기화 완료")
