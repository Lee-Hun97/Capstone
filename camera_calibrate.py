import numpy as np
import cv2
import glob

CHECKERBOARD = (9, 6)

# 3D 공간 점과 이미지 평면 점
objpoints = []  # 3D
imgpoints = []  # 2D

# 실제 세계 좌표계의 체스보드 포인트 초기화
objp = np.zeros((CHECKERBOARD[0] * CHECKERBOARD[1], 3), np.float32)
objp[:, :2] = np.mgrid[0:CHECKERBOARD[0], 0:CHECKERBOARD[1]].T.reshape(-1, 2)

# 촬영한 이미지가 저장된 경로
images = glob.glob('cam_cal/*.jpg')

for fname in images:
    img = cv2.imread(fname)
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

    # 체스보드 코너 찾기
    ret, corners = cv2.findChessboardCorners(gray, CHECKERBOARD, None)

    if ret:
        objpoints.append(objp)
        imgpoints.append(corners)
        cv2.drawChessboardCorners(img, CHECKERBOARD, corners, ret)
        cv2.imshow('img', img)
        cv2.waitKey(200)

cv2.destroyAllWindows()

# 보정 실행
ret, camera_matrix, dist_coeffs, rvecs, tvecs = cv2.calibrateCamera(
    objpoints, imgpoints, gray.shape[::-1], None, None
)

# 결과 출력
print("카메라 행렬 (camera_matrix):\n", camera_matrix)
print("왜곡 계수 (dist_coeffs):\n", dist_coeffs)

# 결과 저장 (선택)
np.savez("camera_params.npz", camera_matrix=camera_matrix, dist_coeffs=dist_coeffs)
