import cv2
import cv2.aruco as aruco

aruco_dict = aruco.Dictionary_get(aruco.DICT_4X4_50)

for i in range(3):
    marker_img = aruco.drawMarker(aruco_dict, i, 300)
    filename = f"C:/Users/7950X3D/Desktop/MARKER/marker_{i}.png"
    cv2.imwrite(filename, marker_img)
    print(f"Saved: {filename}")
