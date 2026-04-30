import cv2
import numpy as np


def compute_ear(eye_landmarks):
    try:
        if len(eye_landmarks) < 6:
            return 0.25

        p = [np.array(pt) for pt in eye_landmarks]

        # Vertical distances
        v1 = np.linalg.norm(p[1] - p[5])
        v2 = np.linalg.norm(p[2] - p[4])
        # Horizontal distance
        h = np.linalg.norm(p[0] - p[3])

        ear = (v1 + v2) / (2.0 * h)
        return ear
    except Exception as e:
        print(f"Error computing EAR: {e}")
        return 0.25


def detect_blink(eye_landmarks):
    ear = compute_ear(eye_landmarks)
    return ear < 0.2


def detect_head_movement(prev_frame, current_frame):
    try:
        if prev_frame is None or current_frame is None:
            return False
        diff = np.sum(np.abs(current_frame.astype(np.float32) - prev_frame.astype(np.float32)))
        return diff > 5000
    except Exception as e:
        print(f"Error detecting movement: {e}")
        return False


def detect_texture(image):
    try:
        if image is None:
            return False
        gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
        laplacian = cv2.Laplacian(gray, cv2.CV_64F)
        variance = laplacian.var()
        print(f"[AntiSpoof] Laplacian variance: {variance:.2f}")
        return variance > 50
    except Exception as e:
        print(f"Error in texture detection: {e}")
        return False


def is_real_face(blink: bool, movement: bool, image=None) -> bool:
    texture = detect_texture(image) if image is not None else True

    if movement:
        return True

    if texture and blink:
        return True

    return False
