from fastapi import APIRouter, HTTPException
from pydantic import BaseModel
import base64
import cv2
import numpy as np
import os
from recognition.face_engine import get_embedding, compare_faces
from recognition.anti_spoofing import detect_blink, detect_head_movement, is_real_face

router = APIRouter()

class UserEmbedding(BaseModel):
    userId: str
    embedding: list

class VerifyRequest(BaseModel):
    image: str
    users: list 
    prev_image: str | None = None  # optional


def decode_image(b64_str: str):
    """Decode a base64 image string (with or without data-URI prefix)."""
    if "," in b64_str:
        b64_str = b64_str.split(",")[1]
    data = base64.b64decode(b64_str)
    arr = np.frombuffer(data, np.uint8)
    return cv2.imdecode(arr, cv2.IMREAD_COLOR)


@router.post("/")
def verify_face(req: VerifyRequest):
    try:
        # Decode current frame
        img = decode_image(req.image)
        if img is None:
            raise HTTPException(status_code=400, detail="Invalid image data")

        # Decode previous frame 
        prev_frame = None
        if req.prev_image:
            try:
                prev_frame = decode_image(req.prev_image)
                # Resize to match current frame if needed
                if prev_frame is not None and prev_frame.shape != img.shape:
                    prev_frame = cv2.resize(prev_frame, (img.shape[1], img.shape[0]))
            except Exception:
                prev_frame = None

        movement = detect_head_movement(prev_frame, img)

        # is_real_face now also runs texture analysis on the current frame
        if not is_real_face(blink=False, movement=movement, image=img):
            if req.prev_image is not None:
                # if we actually had a previous frame to compare, reject
                return {"match": False, "error": "Spoof detected: no liveness signal"}

        path = "temp_verify.jpg"
        cv2.imwrite(path, img)
        new_emb = get_embedding(path)

        if os.path.exists(path):
            os.remove(path)

        if new_emb is None:
            return {"match": False, "error": "No face detected in frame"}

        for u in req.users:
            try:
                emb = np.array(u["embedding"])
                if compare_faces(new_emb, emb):
                    return {
                        "match": True,
                        "userId": u["userId"]
                    }
            except Exception:
                continue

        return {"match": False}

    except Exception as e:
        return {"match": False, "error": str(e)}
