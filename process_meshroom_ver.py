#사진 불러와서 모델링
@app.route('/process_m', methods=['POST'])
def process_m():
    user_id = request.form.get('user_id')
    timestamp = request.form.get('timestamp')

    if not is_valid_user(user_id):
        return jsonify({'status': 'fail', 'message': '존재하지 않는 유저'}), 403
    if not user_id or not timestamp:
        return jsonify({'status': 'fail', 'message': 'user_id 또는 timestamp 누락'}), 400

    input_folder = os.path.join('uploads', f'user_{user_id}', timestamp)
    output_folder = os.path.join('outputs', f'user_{user_id}', timestamp)
    os.makedirs(output_folder, exist_ok=True)

    # 디버깅 출력
    print("[디버깅] 입력 경로:", input_folder)
    print("[디버깅] 폴더 존재?", os.path.exists(input_folder))
    print("[디버깅] 폴더 내용물:", os.listdir(input_folder) if os.path.exists(input_folder) else "❌ 없음")

    # 실시간 로그 출력용 Meshroom 실행
    command = [
        'meshroom_batch',
        '--input', input_folder,
        '--output', output_folder
    ]

    try:
        print("모델 생성 시작됨...")
        process = Popen(command, stdout=PIPE, stderr=STDOUT, text=True)

        for line in process.stdout:
            print("[Meshroom] " + line.strip())

        process.wait()
        if process.returncode != 0:
            return jsonify({'status': 'fail', 'message': '모델 생성 실패'}), 500

        return jsonify({
            'status': 'ok',
            'message': '3D 모델 생성 완료',
            'output_folder': output_folder
        })

    except Exception as e:
        return jsonify({
            'status': 'fail',
            'message': '예외 발생',
            'error': str(e)
        }), 500
