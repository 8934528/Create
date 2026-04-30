import cv2
import numpy as np
from deepface import DeepFace
import os

class FaceEngine:
    def __init__(self, model_name='VGG-Face', detector_backend='opencv'):
        self.model_name = model_name
        self.detector_backend = detector_backend

    def get_embedding(self, frame):
        """
        Extract embedding from the first face found in the frame.
        """
        try:
            # DeepFace.represent returns a list of embeddings (one for each face)
            results = DeepFace.represent(
                img_path=frame,
                model_name=self.model_name,
                detector_backend=self.detector_backend,
                enforce_detection=True
            )
            if results:
                return results[0]['embedding']
        except Exception as e:
            print(f"Error extracting embedding: {e}")
        return None

    def verify(self, img1, img2):
        """
        Verify if two images belong to the same person.
        """
        try:
            result = DeepFace.verify(
                img1_path=img1,
                img2_path=img2,
                model_name=self.model_name,
                detector_backend=self.detector_backend
            )
            return result['verified'], result['distance']
        except Exception as e:
            print(f"Error during verification: {e}")
            return False, 1.0

    def find_match(self, img, db_path):
        """
        Find a match for an image in a directory of images.
        """
        try:
            results = DeepFace.find(
                img_path=img,
                db_path=db_path,
                model_name=self.model_name,
                detector_backend=self.detector_backend
            )
            return results
        except Exception as e:
            print(f"Error during search: {e}")
            return []
