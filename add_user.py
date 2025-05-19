import sqlite3

def insert_test_users():
    conn = sqlite3.connect('database.db')
    conn.execute('PRAGMA foreign_keys = ON')
    cur = conn.cursor()

    cur.execute('''
        CREATE TABLE IF NOT EXISTS users (
            user_id TEXT PRIMARY KEY,
            email TEXT UNIQUE,
            password TEXT
        )
    ''')

    users = [
        ('test@example.com', 'test@example.com', '1234'),
        ('test456', 'test2@example.com', '5678'),
        ('test789', 'test3@example.com', 'abcd')
    ]

    cur.executemany('''
        INSERT OR IGNORE INTO users (user_id, email, password)
        VALUES (?, ?, ?)
    ''', users)

    conn.commit()
    conn.close()
    print("테스트 계정 3명 추가 완료")

insert_test_users()
