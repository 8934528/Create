# Project Structure

The project is organized into a distributed micro-monolith architecture, separating concerns between the AI processing engine, the ASP.NET core backend, and the web frontend.

```text
Create/

├── 📂 ai-service/                # Python Face Recognition Microservice (FastAPI)
│   ├── 📂 app/                   # Core application logic
│   │   ├── 📂 models/            # AI model configurations and weights
│   │   ├── 📂 recognition/       # Face processing & Anti-spoofing engines
│   │   ├── 📂 routes/            # API endpoints (Register, Verify)
│   │   └── main.py               # Application entry point
│   ├── requirements.txt          # Python dependencies
│   └── .env                      # AI Service local configuration
│
├── 📂 backend-dotnet/            # ASP.NET Core Clean Architecture Backend
│   ├── 📂 Create.API/            # Presentation Layer (Controllers, Swagger)
│   ├── 📂 Create.Application/    # Business Logic Layer (Services, Interfaces)
│   ├── 📂 Create.Domain/         # Core Domain (Entities, Enums)
│   ├── 📂 Create.Infrastructure/ # Persistence & External Services (EF Core, AI Client)
│   └── 📂 Create.Shared/         # Shared Cross-cutting Concerns (DTOs, Constants)
│
├── 📂 frontend-web/              # Web Application (ASP.NET Razor Pages)
│   ├── 📂 Pages/                 # UI Pages (Attendance, Registration, Dashboard)
│   ├── 📂 wwwroot/               # Static assets (CSS, JS, Images)
│   └── 📂 Services/              # Backend API wrappers
│
├── 📂 database/                  # Database Schemas and Seed scripts
│   ├── schema.sql                # PostgreSQL table definitions
│   └── seed.sql                  # Initial data for development
│
├── 📄 .env                       # Global environment variables
├── 📄 Create.sln                 # .NET Solution file
└── 📄 README.md                  # Main project documentation
```

## Component Responsibilities

### AI Service

Responsible for converting images into high-dimensional facial embeddings (128d or 512d vectors) and performing anti-spoofing checks to ensure the face presented is "live".

### Backend API

The central orchestrator. It manages user authentication, stores embeddings in the `pgvector` enabled database, and provides endpoints for the frontend. It follows **Clean Architecture** to keep the core logic independent of external frameworks.

### Frontend Web

Provides a user-friendly interface for registration and a real-time face scanning module that uses the browser's camera to capture and send frames for verification.

### PostgreSQL (pgvector)

Used not just for standard relational data, but also as a **Vector Database**. It enables fast cosine similarity or Euclidean distance searches between stored face embeddings.
