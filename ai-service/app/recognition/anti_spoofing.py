import cv2
import numpy as np

class AntiSpoofing:
    def __init__(self):
        self.face_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + 'haarcascade_frontalface_default.xml')
        self.eye_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + 'haarcascade_eye.xml')

    def check_spoof(self, frame):
        """
        Perform a simple anti-spoofing check.
        Returns (is_real, message)
        """
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        faces = self.face_cascade.detectMultiScale(gray, 1.3, 5)

        if len(faces) == 0:
            return False, "No face detected"

        for (x, y, w, h) in faces:
            roi_gray = gray[y:y+h, x:x+w]
            eyes = self.eye_cascade.detectMultiScale(roi_gray)
            
            if len(eyes) < 2:
                return False, "Eyes not detected - possible spoof or poor lighting"

        # Movement detection would require multiple frames.
        # For a single frame API, we can at least check for texture or use more advanced models.
        # Here we'll return True if face and eyes are found.
        return True, "Face detected"

    def detect_blink(self, current_frame, previous_eyes_state):
        """
        Blink detection requires tracking state over frames.
        This is a helper to check if eyes are closed in the current frame.
        """
        gray = cv2.cvtColor(current_frame, cv2.COLOR_BGR2GRAY)
        eyes = self.eye_cascade.detectMultiScale(gray, 1.1, 10)
        
        # If no eyes detected but face is present, they might be blinking
        eyes_closed = len(eyes) == 0
        return eyes_closed
