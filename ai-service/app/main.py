"""
Face Recognition AI Service
---------------------------
A specialized microservice for handling facial embedding extraction,
face verification, and anti-spoofing logic using DeepFace and OpenCV.
"""

from fastapi import FastAPI
from routes import register, verify

app = FastAPI(
    title="Face Recognition API",
    description="Microservice for AI-driven face ID registration and verification.",
    version="1.0.0"
)

@app.get("/", tags=["Health Check"])
def read_root():
    """Returns a simple health check message."""
    return {"message": "Face Recognition AI Service is running"}

app.include_router(register.router, prefix="/register", tags=["Registration"])
app.include_router(verify.router, prefix="/verify", tags=["Verification"])

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
