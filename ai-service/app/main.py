from fastapi import FastAPI
from routes import register, verify

app = FastAPI(title="Face Recognition API")

@app.get("/")
def read_root():
    return {"message": "Face Recognition AI Service is running"}

app.include_router(register.router, prefix="/register")
app.include_router(verify.router, prefix="/verify")
