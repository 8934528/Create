# 🚀 Create: Smart Attendance System

> **Next-Gen Event, Work, and Class Registration powered by AI Face Recognition.**

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512bd4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/download)
[![Python 3.10](https://img.shields.io/badge/Python-3.10-3776ab?style=for-the-badge&logo=python)](https://www.python.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15+-336791?style=for-the-badge&logo=postgresql)](https://www.postgresql.org/)
[![FastAPI](https://img.shields.io/badge/FastAPI-0.100+-009688?style=for-the-badge&logo=fastapi)](https://fastapi.tiangolo.com/)

---

## Key Features

- **AI Face ID Registration**: Securely register users using advanced facial embedding technology.
- **Real-time Attendance**: High-speed, contactless check-in using live camera feeds.
- **Anti-Spoofing**: Built-in liveness detection to prevent photo/video spoofing attacks.
- **Analytics Dashboard**: Comprehensive insights into attendance trends and event participation.
- **High Performance**: Powered by `pgvector` for lightning-fast similarity searches in PostgreSQL.

---

## Technology Stack

| Component | Technology | Role |
| :--- | :--- | :--- |
| **Backend** | ASP.NET Core 8 | RESTful API, Business Logic, Security |
| **AI Engine** | Python + FastAPI | DeepFace, OpenCV, Face Recognition |
| **Database** | PostgreSQL + pgvector | Persistent Storage, Vector Embeddings |
| **Frontend** | Razor Pages + JS | User Interface & Real-time Scanner |
| **Auth** | JWT (JSON Web Tokens) | Secure Stateless Authentication |

---

## Getting Started

### Prerequisites

1. **.NET 8 SDK**
2. **Python 3.10+**
3. **PostgreSQL 15+** with the **pgvector** extension installed.

### Environment Setup

Create a `.env` file in the root directory and configure the following:

```env
DB_HOST=localhost
DB_PORT=5432
DB_USER=postgres
DB_PASSWORD=your_password
DB_NAME=create_db

JWT_SECRET=your_ultra_secure_long_secret_key
AI_SERVICE_URL=http://localhost:8000
```

### Installation & Execution

#### 1. Database Initialization

```sql
CREATE DATABASE create_db;
-- Connect to create_db
CREATE EXTENSION IF NOT EXISTS vector;
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
```

#### 2. Run the AI Service

```bash
cd ai-service
pip install -r requirements.txt
cd app
python main.py
```

#### 3. Run the Backend API

```bash
cd backend-dotnet/Create.API
dotnet run
```
*Swagger UI available at: `http://localhost:5242/swagger`*

#### 4. Run the Web Frontend

```bash
cd frontend-web
dotnet run
```

*Access the application at: `http://localhost:5200`*

---

## Architecture Overview

The system follows a **Clean Architecture** pattern, ensuring high maintainability and scalability:

- **Frontend**: Captures video frames and sends them to the Backend.
- **Backend**: Acts as an orchestrator, handling authentication, business rules, and database interactions.
- **AI Service**: Dedicated Python microservice for heavy-lift facial recognition and anti-spoofing logic.
- **Database**: Stores user data and high-dimensional face embeddings for similarity matching.

---

## Contribution

Contributions are welcome! Please see the [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

This project is licensed under the MIT License.
