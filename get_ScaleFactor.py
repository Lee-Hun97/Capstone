import cv2
import cv2.aruco as aruco
import numpy as np
import glob

def get_scale_factor(image_folder, camera_matrix, dist_coeffs, marker_length_cm=5.0):
    image_paths = glob.glob(image_folder + "/*.jpg")
    scale_factors = []

    def enhance_contrast(img):
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        clahe = cv2.createCLAHE(clipLimit=3.0, tileGridSize=(8, 8))
        return clahe.apply(gray)

    for path in image_paths:
        image = cv2.imread(path)
        if image is None:
            continue

        input_img = enhance_contrast(image)
        aruco_dict = aruco.Dictionary_get(aruco.DICT_4X4_50)
        parameters = aruco.DetectorParameters_create()
        corners, ids, _ = aruco.detectMarkers(input_img, aruco_dict, parameters=parameters)

        if ids is None or len(ids) < 3:
            continue

        rvecs, tvecs, _ = aruco.estimatePoseSingleMarkers(corners, marker_length_cm, camera_matrix, dist_coeffs)

        required_ids = [0, 1, 2]
        marker_positions = {}
        for i in range(len(ids)):
            marker_id = ids[i][0]
            if marker_id in required_ids:
                marker_positions[marker_id] = tvecs[i][0]

        if len(marker_positions) < 3:
            continue

        v0 = marker_positions[0]
        v1 = marker_positions[1]
        unity_distance = np.linalg.norm(v1 - v0)
        real_distance = 5.0
        scale_factor = real_distance / unity_distance
        scale_factors.append(scale_factor)

    if scale_factors:
        return float(np.mean(scale_factors))
    else:
        raise ValueError("유효한 마커가 충분하지 않아 스케일 팩터 계산 실패")
