from fastapi import FastAPI, HTTPException, UploadFile, File
from pydantic import BaseModel
import cv2
import numpy as np
import base64
from app.recognition.face_engine import FaceEngine
from app.recognition.anti_spoofing import AntiSpoofing
import io
from PIL import Image

app = FastAPI(title="Face Recognition AI Service")
face_engine = FaceEngine()
anti_spoof = AntiSpoofing()

class ImageData(BaseModel):
    image_base64: str

def decode_image(base64_str):
    try:
        if "base64," in base64_str:
            base64_str = base64_str.split("base64,")[1]
        img_data = base64.b64decode(base64_str)
        nparr = np.frombuffer(img_data, np.uint8)
        img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
        return img
    except Exception as e:
        print(f"Error decoding image: {e}")
        return None

@app.post("/extract-embedding")
async def extract_embedding(data: ImageData):
    img = decode_image(data.image_base64)
    if img is None:
        raise HTTPException(status_code=400, detail="Invalid image data")
    
    embedding = face_engine.get_embedding(img)
    if embedding is None:
        raise HTTPException(status_code=404, detail="No face detected or embedding could not be generated")
    
    return {"embedding": embedding}

@app.post("/verify")
async def verify(data1: ImageData, data2: ImageData):
    img1 = decode_image(data1.image_base64)
    img2 = decode_image(data2.image_base64)
    
    if img1 is None or img2 is None:
        raise HTTPException(status_code=400, detail="Invalid image data")
    
    verified, distance = face_engine.verify(img1, img2)
    return {"verified": verified, "distance": distance}

@app.post("/check-spoof")
async def check_spoof(data: ImageData):
    img = decode_image(data.image_base64)
    if img is None:
        raise HTTPException(status_code=400, detail="Invalid image data")
    
    is_real, message = anti_spoof.check_spoof(img)
    return {"is_real": is_real, "message": message}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
