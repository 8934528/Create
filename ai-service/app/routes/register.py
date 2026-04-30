from fastapi import APIRouter, HTTPException
from pydantic import BaseModel
import base64
import numpy as np
import cv2
import os
from recognition.face_engine import get_embedding

router = APIRouter()

class RegisterRequest(BaseModel):
    image: str  # base64

@router.post("/")
def register_face(req: RegisterRequest):
    try:
        # Decode base64 image
        if "," in req.image:
            img_data = base64.b64decode(req.image.split(",")[1])
        else:
            img_data = base64.b64decode(req.image)
            
        np_arr = np.frombuffer(img_data, np.uint8)
        img = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)

        if img is None:
            raise HTTPException(status_code=400, detail="Invalid image data")

        path = "temp_register.jpg"
        cv2.imwrite(path, img)

        embedding = get_embedding(path)
        
        # Cleanup
        if os.path.exists(path):
            os.remove(path)

        if embedding is None:
            return {"success": False, "error": "No face detected or failed to extract embedding"}

        return {
            "success": True,
            "embedding": embedding.tolist()
        }

    except Exception as e:
        return {"success": False, "error": str(e)}
