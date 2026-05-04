from deepface import DeepFace
import numpy as np
from numpy.linalg import norm

def get_embedding(image_path):
    """
    Extract face embedding using Facenet model.
    """
    try:
        result = DeepFace.represent(
            img_path=image_path,
            model_name="Facenet",
            enforce_detection=False
        )
        return np.array(result[0]["embedding"])
    except Exception as e:
        print(f"Error extracting embedding: {e}")
        return None

def compare_faces(emb1, emb2, threshold=0.68):
    """
    Compare two embeddings using Euclidean distance (norm).
    """
    try:
        distance = norm(emb1 - emb2)
        return distance < threshold
    except Exception as e:
        print(f"Error during comparison: {e}")
        return False

