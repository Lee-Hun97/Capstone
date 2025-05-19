
# RealityCapture CLI를 실행하여 3D 모델을 생성하는 API
@app.route('/process_rc', methods=['POST'])
def process_rc():
x    timestamp = request.form.get('timestamp')

    if not user_id or not timestamp:
        return jsonify({'status': 'fail', 'message': 'user_id 또는 timestamp 누락'}), 400

    # 입력 및 출력 경로 설정
    input_folder = os.path.abspath(os.path.join('uploads', f'user_{user_id}', timestamp))
    output_folder = os.path.abspath(os.path.join('outputs', f'user_{user_id}', timestamp))
    project_path = os.path.join(output_folder, 'my_project.rcproj')
    output_model_path = os.path.join(output_folder, 'model.obj')
    os.makedirs(output_folder, exist_ok=True)

    # RealityCapture CLI 실행 명령 구성
    command = [
        "RealityCapture.exe",
        "-newScene",
        f"-addFolder \"{input_folder}\"",
        "-align",
        "-calculateNormalModel",
        "-selectComponent \"Component 0\"",
        "-selectModel \"Model 1\"",
        "-simplify",
        f"-exportModel \"Model 1\" \"{output_model_path}\"",
        f"-save \"{project_path}\"",
        "-quit"
    ]


    try:
        print("RC 입력 경로:", input_folder)
        process = subprocess.Popen(" ".join(command), shell=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)

        for line in process.stdout:
            print("[RC] " + line.strip())

        process.wait()
        if process.returncode != 0:
            return jsonify({'status': 'fail', 'message': 'RealityCapture 실행 실패'}), 500

        # 모델 생성 성공 → DB에 저장
        conn = sqlite3.connect('database.db')
        cur = conn.cursor()

        # 새 모델 저장
        cur.execute(
            "INSERT INTO models (user_id, timestamp, model_path) VALUES (?, ?, ?)",
            (user_id, timestamp, output_model_path)
        )

        # 최신 3개 초과 시 오래된 모델 삭제
        cur.execute(
            "SELECT id FROM models WHERE user_id = ? ORDER BY created_at DESC",
            (user_id,)
        )
        rows = cur.fetchall()
        if len(rows) > 3:
            for row in rows[3:]:
                cur.execute("DELETE FROM models WHERE id = ?", (row[0],))

        conn.commit()
        conn.close()

        return jsonify({
            'status': 'ok',
            'message': '3D 모델 생성 완료',
            'model_path': output_model_path
        })

    except Exception as e:
        return jsonify({
            'status': 'fail',
            'message': '서버 오류',
            'error': str(e)
        }), 500
    
############
    from flask import Flask, request, jsonify
import sqlite3
import os
import time
import re
import subprocess
from subprocess import Popen, PIPE, STDOUT

app = Flask(__name__)

#로그인 구현 
@app.route('/login', methods=['POST'])
def login():
    data = request.get_json()  # JSON 데이터 받기
    email = data.get('email')
    password = data.get('password')

    # DB 연결
    conn = sqlite3.connect('database.db')
    cursor = conn.cursor()

    # 사용자 조회
    cursor.execute('SELECT id FROM users WHERE email = ? AND password = ?', (email, password))
    user = cursor.fetchone()

    conn.close()

    if user:
        return jsonify({'status': 'ok', 'user_id': user[0]})
    else:
        return jsonify({'status': 'fail', 'message': '로그인 실패'}), 401


# 이미지 업로드 처리 (여러 장 처리 버전)
@app.route('/upload', methods=['POST'])
def upload():
    user_id = request.form.get('user_id')
    files = request.files.getlist('file') 

    if not files or not user_id:
        return jsonify({'status': 'fail', 'message': '파일 또는 user_id 누락'}), 400

    timestamp = time.strftime("%Y%m%d_%H%M%S")
    folder_path = os.path.join('uploads', f'user_{user_id}', timestamp)
    os.makedirs(folder_path, exist_ok=True)

    saved_files = []
    for file in files:
        filename = file.filename
        save_path = os.path.join(folder_path, filename)
        file.save(save_path)
        saved_files.append(save_path)

    return jsonify({
        'status': 'ok',
        'message': f'{len(saved_files)}개 파일 저장 완료',
        'upload_paths': saved_files
    })

# RealityCapture CLI를 실행하여 3D 모델을 생성하는 API
@app.route('/process_rc', methods=['POST'])
def process_rc():
    user_id = request.form.get('user_id')
    timestamp = request.form.get('timestamp')

    if not user_id or not timestamp:
        return jsonify({'status': 'fail', 'message': 'user_id 또는 timestamp 누락'}), 400

    input_folder = os.path.abspath(os.path.join('uploads', f'user_{user_id}', timestamp))
    output_folder = os.path.abspath(os.path.join('outputs', f'user_{user_id}', timestamp))
    project_path = os.path.join(output_folder, 'my_project.rcproj')
    output_model_path = os.path.join(output_folder, 'model.obj')
    os.makedirs(output_folder, exist_ok=True)

    command = [
        "RealityCapture.exe",
        "-newScene",
        "-addFolder", input_folder,
        "-align",
        "-calculateNormalModel",
        "-selectComponent", "Component 0",
        "-selectModel", "Model 1",
        "-simplify",
        "-exportModel", "Model 1", output_model_path,
        "-save", project_path,
        "-quit"
    ]

    try:
        print("RC 입력 경로:", input_folder)
        process = subprocess.Popen(command, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)

        for line in process.stdout:
            print("[RC] " + line.strip())

        process.wait()
        if process.returncode != 0:
            return jsonify({'status': 'fail', 'message': 'RealityCapture 실행 실패'}), 500

        return jsonify({
            'status': 'ok',
            'message': '3D 모델 생성 완료',
            'model_path': output_model_path
        })

    except Exception as e:
        return jsonify({
            'status': 'fail',
            'message': '서버 오류',
            'error': str(e)
        }), 500


# 가장 최근 모델 경로 찾는 도우미 함수
def get_latest_model_path(user_id):
    base_dir = os.path.join('outputs', f'user_{user_id}')
    if not os.path.exists(base_dir):
        return None, None

    subfolders = sorted(
        [f.path for f in os.scandir(base_dir) if f.is_dir()],
        key=os.path.getmtime,
        reverse=True
    )

    if not subfolders:
        return None, None

    latest_folder = subfolders[0]
    timestamp = os.path.basename(latest_folder)
    model_path = os.path.join(latest_folder, 'model.obj')

    if os.path.exists(model_path):
        return model_path, timestamp
    return None, None


# 유저가 이름 붙여서 저장하는 API
@app.route('/save_model', methods=['POST'])
def save_model():
    name = request.form.get('name')
    user_id = request.form.get('user_id')

    if not name or not user_id:
        return jsonify({'status': 'fail', 'message': 'name 또는 user_id 누락'}), 400

    model_path, timestamp = get_latest_model_path(user_id)
    if not model_path:
        return jsonify({'status': 'fail', 'message': '모델 파일 없음'}), 404

    model_id = str(uuid.uuid4())

    conn = sqlite3.connect('database.db')
    cur = conn.cursor()
    cur.execute('''
        CREATE TABLE IF NOT EXISTS models (
            model_id TEXT PRIMARY KEY,
            name TEXT,
            user_email TEXT
        )
    ''')
    cur.execute('''
        INSERT INTO models (model_id, name, user_email)
        VALUES (?, ?, ?)
    ''', (model_id, name, user_id))

    conn.commit()
    conn.close()

    return jsonify({'status': 'ok', 'model_id': model_id, 'timestamp': timestamp})

# 저장된 모델 목록 조회 API
@app.route('/get_saved_models', methods=['GET'])
def get_saved_models():
    user_id = request.args.get('user_id')
    if not user_id:
        return jsonify({'status': 'fail', 'message': 'user_id 누락'}), 400

    conn = sqlite3.connect('database.db')
    cur = conn.cursor()

    # 테이블 없으면 생성 (안전하게)
    cur.execute('''
        CREATE TABLE IF NOT EXISTS models (
            model_id TEXT PRIMARY KEY,
            name TEXT,
            user_email TEXT,
            timestamp TEXT,
            created_at TEXT DEFAULT CURRENT_TIMESTAMP
        )
    ''')

    cur.execute('''
        SELECT name, timestamp, created_at FROM models
        WHERE user_email = ?
        ORDER BY created_at DESC
    ''', (user_id,))
    rows = cur.fetchall()
    conn.close()

    result = []
    for row in rows:
        result.append({
            'name': row[0],
            'timestamp': row[1],
            'created_at': row[2]
        })

    return jsonify({'status': 'ok', 'models': result})


#최근에 올린 폴더명 찾기
@app.route('/latest_upload', methods=['POST'])
def latest_upload():
    user_id = request.form.get('user_id')
    if not user_id:
        return jsonify({'status': 'fail', 'message': 'user_id 누락'}), 400

    base_path = os.path.join('uploads', f'user_{user_id}')
    if not os.path.exists(base_path):
        return jsonify({'status': 'fail', 'message': '업로드 없음'}), 404

    # 업로드된 timestamp 폴더 목록 가져오기
    timestamps = [
        folder for folder in os.listdir(base_path)
        if os.path.isdir(os.path.join(base_path, folder))
    ]
    if not timestamps:
        return jsonify({'status': 'fail', 'message': 'timestamp 폴더 없음'}), 404

    # 최신 timestamp (폴더명 기준으로 정렬)
    latest = sorted(timestamps, reverse=True)[0]

    return jsonify({
        'status': 'ok',
        'timestamp': latest
    })

if __name__ == '__main__':
    app.run(debug=True)

# 로그인
@app.route('/login', methods=['POST'])
def login():
    data = request.get_json(force=True)
    email = data.get('email')
    password = data.get('password')

    if not email or not password:
        return jsonify({'status': 'fail', 'message': '이메일 또는 비밀번호 누락'}), 400

    conn = get_db()
    cur = conn.cursor()
    cur.execute('SELECT user_id FROM users WHERE email = ? AND password = ?', (email, password))
    user = cur.fetchone()
    conn.close()

    if user:
        return jsonify({'status': 'ok', 'user_id': user[0]})
    return jsonify({'status': 'fail', 'message': '로그인 실패'}), 401


