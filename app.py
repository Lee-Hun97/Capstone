from flask import Flask, request, jsonify, send_file
import sqlite3
import os
import uuid
import subprocess
import time
import shutil
from subprocess import Popen, PIPE, STDOUT
from get_ScaleFactor_final import get_scale_factor

camera_matrix = np.array([
    [0, 0, 0],
    [0, 0, 0],
    [0, 0, 1]
], dtype=np.float32)

dist_coeffs = np.array([0, 0, 0, 0, 0])

app = Flask(__name__)

# DB 연결 함수 (외래키 활성화 포함)
def get_db():
    conn = sqlite3.connect('database.db')
    conn.execute('PRAGMA foreign_keys = ON')
    return conn

# user_id 유효성 검사 함수
def is_valid_user(user_id):
    conn = get_db()
    cur = conn.cursor()
    cur.execute('SELECT 1 FROM users WHERE user_id = ?', (user_id,))
    exists = cur.fetchone()
    conn.close()
    return exists is not None

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

# 이미지 업로드
@app.route('/upload', methods=['POST'])
def upload():
    user_id = request.form.get('user_id')
    if not user_id:
        return jsonify({'status': 'fail', 'message': 'user_id 누락'}), 400
    if not is_valid_user(user_id):
        return jsonify({'status': 'fail', 'message': '존재하지 않는 유저'}), 403

    files = request.files.getlist('file')
    if not files:
        return jsonify({'status': 'fail', 'message': '파일 누락'}), 400

    timestamp = time.strftime("%Y%m%d_%H%M%S")
    folder_path = os.path.join('uploads', f'user_{user_id}', timestamp)
    os.makedirs(folder_path, exist_ok=True)

    for file in files:
        file.save(os.path.join(folder_path, file.filename))

    return jsonify({'status': 'ok', 'timestamp': timestamp})

# 모델 생성
@app.route('/process_rc', methods=['POST'])
def process_rc():
    user_id = request.form.get('user_id')
    timestamp = request.form.get('timestamp')

    if not user_id or not timestamp:
        return jsonify({'status': 'fail', 'message': 'user_id 또는 timestamp 누락'}), 400
    if not is_valid_user(user_id):
        return jsonify({'status': 'fail', 'message': '존재하지 않는 유저'}), 403

    input_folder = os.path.abspath(os.path.join('uploads', f'user_{user_id}', timestamp))
    output_folder = os.path.abspath(os.path.join('outputs', f'user_{user_id}', timestamp))
    os.makedirs(output_folder, exist_ok=True)
    output_model_path = os.path.join(output_folder, 'model.fbx')
    scale_factor = get_scale_factor(input_folder, camera_matrix, dist_coeffs, marker_length_cm=5.0)
    print("스케일 팩터:", scale_factor)
    
    command = [
        "RealityCapture.exe",
        "-newScene",
        "-addFolder", input_folder,
        "-align",
        "-calculateNormalModel",
        "-selectComponent", "Component 0",
        "-selectModel", "Model 1",
        "-simplify",
        "-scaleSceneUnits", str(scale_factor),
        "-exportModel", "Model 1", output_model_path,
        "-quit"
    ]
    try:
        process = subprocess.Popen(command, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
        for line in process.stdout:
            print("[RC]", line.strip())
        process.wait()

        if process.returncode != 0:
            return jsonify({'status': 'fail', 'message': 'RealityCapture 실행 실패'}), 500

        #Unity 번들 빌드 자동 실행
        success, msg = build_assetbundle(user_id, timestamp)
        if not success:
            return jsonify({'status': 'fail', 'message': 'AssetBundle 빌드 실패', 'error': msg}), 500        

        return jsonify({
            'status': 'ok',
            'message': '모델 + 번들 생성 완료',
            'model_path': output_model_path
        })
    except Exception as e:
        return jsonify({
            'status': 'fail',
            'message': '서버 오류',
            'error': str(e)
        }), 500


# 최신 모델 경로 반환
def get_latest_model_path(user_id):
    base_dir = os.path.join('outputs', f'user_{user_id}')
    if not os.path.exists(base_dir):
        return None, None
    folders = sorted([f.path for f in os.scandir(base_dir) if f.is_dir()], key=os.path.getmtime, reverse=True)
    if not folders:
        return None, None
    latest = folders[0]
    timestamp = os.path.basename(latest)
    model_path = os.path.join(latest, 'model.fbx')
    return (model_path, timestamp) if os.path.exists(model_path) else (None, None)

# 모델 저장
@app.route('/save_model', methods=['POST'])
def save_model():
    name = request.form.get('name')
    user_id = request.form.get('user_id')

    if not name or not user_id:
        return jsonify({'status': 'fail', 'message': 'name 또는 user_id 누락'}), 400
    if not is_valid_user(user_id):
        return jsonify({'status': 'fail', 'message': '존재하지 않는 유저'}), 403

    model_path, timestamp = get_latest_model_path(user_id)
    if not model_path:
        return jsonify({'status': 'fail', 'message': '모델 파일 없음'}), 404

    model_id = str(uuid.uuid4())
    conn = get_db()
    cur = conn.cursor()
    cur.execute('''
        INSERT INTO models (model_id, name, user_id, timestamp)
        VALUES (?, ?, ?, ?)
    ''', (model_id, name, user_id, timestamp))
    conn.commit()
    conn.close()

    return jsonify({'status': 'ok', 'model_id': model_id, 'timestamp': timestamp})

# 모델 목록 조회
@app.route('/get_saved_models', methods=['GET'])
def get_saved_models():
    user_id = request.args.get('user_id')
    if not user_id:
        return jsonify({'status': 'fail', 'message': 'user_id 누락'}), 400
    if not is_valid_user(user_id):
        return jsonify({'status': 'fail', 'message': '존재하지 않는 유저'}), 403

    conn = get_db()
    cur = conn.cursor()
    cur.execute('''
        SELECT name, timestamp, created_at FROM models
        WHERE user_id = ?
        ORDER BY created_at DESC
    ''', (user_id,))
    rows = cur.fetchall()
    conn.close()

    result = [{'name': r[0], 'timestamp': r[1], 'created_at': r[2]} for r in rows]
    return jsonify({'status': 'ok', 'models': result})

# 모델 다운로드
@app.route('/get_model', methods=['GET'])
def get_model():
    user_id = request.args.get('user_id')
    timestamp = request.args.get('timestamp')

    if not user_id or not timestamp:
        return jsonify({'status': 'fail', 'message': 'user_id 또는 timestamp 누락'}), 400
    if not is_valid_user(user_id):
        return jsonify({'status': 'fail', 'message': '존재하지 않는 유저'}), 403

    model_path = os.path.abspath(os.path.join('outputs', f'user_{user_id}', timestamp, 'model.fbx'))

    if not os.path.exists(model_path):
        return jsonify({'status': 'fail', 'message': '모델 파일 없음'}), 404

    return send_file(model_path, as_attachment=False)


# 최신 업로드 timestamp 반환
@app.route('/latest_upload', methods=['POST'])
def latest_upload():
    user_id = request.form.get('user_id')
    if not user_id:
        return jsonify({'status': 'fail', 'message': 'user_id 누락'}), 400
    if not is_valid_user(user_id):
        return jsonify({'status': 'fail', 'message': '존재하지 않는 유저'}), 403

    base_path = os.path.join('uploads', f'user_{user_id}')
    if not os.path.exists(base_path):
        return jsonify({'status': 'fail', 'message': '업로드 없음'}), 404

    timestamps = sorted(os.listdir(base_path), reverse=True)
    if not timestamps:
        return jsonify({'status': 'fail', 'message': 'timestamp 폴더 없음'}), 404

    return jsonify({'status': 'ok', 'timestamp': timestamps[0]})

# 번들 파일 다운로드 API
@app.route('/get_bundle', methods=['GET'])
def get_bundle():
    user_id = request.args.get('user_id')
    timestamp = request.args.get('timestamp')

    if not user_id or not timestamp:
        return jsonify({'status': 'fail', 'message': 'user_id 또는 timestamp 누락'}), 400
    if not is_valid_user(user_id):
        return jsonify({'status': 'fail', 'message': '존재하지 않는 유저'}), 403

    bundle_path = os.path.join(r"E:\unity\My project\AssetBundles", "modelbundle")

    if not os.path.exists(bundle_path):
        return jsonify({'status': 'fail', 'message': 'AssetBundle 없음'}), 404

    return send_file(bundle_path, as_attachment=True, download_name="modelbundle")


def build_assetbundle(user_id, timestamp):
    UNITY_EXE = r"C:\Program Files\Unity\Hub\Editor\6000.0.46f1\Editor\Unity.exe"
    UNITY_PROJECT = r"E:\unity\My project"
    FBX_TARGET = os.path.join(UNITY_PROJECT, "Assets", "ImportedModels", "model.fbx")
    LOG_PATH = os.path.join(UNITY_PROJECT, "build_log.txt")

    fbx_source = os.path.abspath(os.path.join('outputs', f'user_{user_id}', timestamp, 'model.fbx'))

    if not os.path.exists(fbx_source):
        print(f"[Unity 번들 빌드 실패] FBX 파일 없음: {fbx_source}")
        return False, "FBX 파일 없음"

    # 복사
    os.makedirs(os.path.dirname(FBX_TARGET), exist_ok=True)
    shutil.copy2(fbx_source, FBX_TARGET)
    print(f"[Unity 빌드] FBX 복사 완료 → {FBX_TARGET}")

    # CLI 실행
    cmd = [
        UNITY_EXE,
        "-batchmode",
        "-quit",
        "-projectPath", UNITY_PROJECT,
        "-executeMethod", "AssetBundleBuilder.BuildModelAssetBundle",
        "-logFile", LOG_PATH
    ]

    result = subprocess.run(cmd, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    if result.returncode != 0:
        print(f"[Unity 빌드 실패]\n{result.stderr}")
        return False, result.stderr

    print("[Unity 빌드 성공]")
    return True, "빌드 성공"

#if __name__ == '__main__':
#    app.run(debug=True)
if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)

