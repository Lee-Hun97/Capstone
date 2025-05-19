Python 3.13.3 (tags/v3.13.3:6280bb5, Apr  8 2025, 14:47:33) [MSC v.1943 64 bit (AMD64)] on win32
Enter "help" below or click "Help" above for more information.
>>> import cv2
... import cv2.aruco as aruco
... import numpy as np
... import glob
... 
... # 이미지 폴더 경로 및 마커 실제 크기
... image_folder = "C:/Users/7950X3D/Desktop/MARKER/series"  # 이미지들이 저장된 폴더
... marker_length_cm = 5.0
... 
... # 카메라 보정값
... camera_matrix = np.array([
...     [463.43603745, 0.0, 667.86311667],
...     [0.0, 479.29023226, 518.97120463],
...     [0.0, 0.0, 1.0]
... ], dtype=np.float32)
... 
... dist_coeffs = np.array([[ 0.01233968, -0.04555962, -0.0090304, -0.00100949,  0.02680105]], dtype=np.float32)
... 
... # 대비 보정 함수
... def enhance_contrast(img):
...     gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
...     clahe = cv2.createCLAHE(clipLimit=3.0, tileGridSize=(8, 8))
...     return clahe.apply(gray)
... 
... # 이미지 목록 가져오기
... image_paths = glob.glob(image_folder + "/*.jpg")
... scale_factors = []
... 
... for path in image_paths:
...     print(f"\n이미지 처리 중: {path}")
...     image = cv2.imread(path)
...     if image is None:
...         print("이미지를 불러올 수 없습니다.")
...         continue
... 
    input_img = enhance_contrast(image)
    aruco_dict = aruco.Dictionary_get(aruco.DICT_4X4_50)
    parameters = aruco.DetectorParameters_create()
    corners, ids, _ = aruco.detectMarkers(input_img, aruco_dict, parameters=parameters)

    if ids is None or len(ids) < 3:
        print(""마커 3개 이상 인식되지 않음")
        continue

    rvecs, tvecs, _ = aruco.estimatePoseSingleMarkers(corners, marker_length_cm, camera_matrix, dist_coeffs)

    required_ids = [0, 1, 2]
    marker_positions = {}
    for i in range(len(ids)):
        marker_id = ids[i][0]
        if marker_id in required_ids:
            marker_positions[marker_id] = tvecs[i][0]

    if len(marker_positions) < 3:
        print("마커가 모두 인식되지 않음")
        continue

    v0 = marker_positions[0]
    v1 = marker_positions[1]
    unity_distance = np.linalg.norm(v1 - v0)
    real_distance = 5.0  # 기준 거리 (cm)
    scale_factor = real_distance / unity_distance
    scale_factors.append(scale_factor)
    print(f"scale factor: {scale_factor:.5f}")

if scale_factors:
    average_scale = np.mean(scale_factors)
    print("\n평균 스케일 팩터:", round(average_scale, 5))
else:
