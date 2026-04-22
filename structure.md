# Project Structure

Create/
│
├── global.json
├── .env
├── .gitignore
├── Create.sln
│
├── backend-dotnet/
│   ├── Create.API/              (ASP.NET Core Web API - Controllers)
│   ├── Create.Application/      (Business Logic / Services)
│   ├── Create.Domain/           (Entities: User, Attendance, Face)
│   ├── Create.Infrastructure/   (PostgreSQL + External services)
│   └── Create.Shared/           (DTOs, helpers, constants)
│
├── ai-service/                  (Python Face Recognition Engine)
│   ├── app/
│   │   ├── main.py
│   │   ├── recognition/
│   │   │   ├── face_engine.py
│   │   │   ├── anti_spoof.py
│   │   │   └── utils.py
│   │   ├── routes/
│   │   │   ├── register.py
│   │   │   └── verify.py
│   │   └── models/
│   ├── requirements.txt
│   └── .env
│
├── frontend-web/                (ASP.NET Razor Pages OR ASPX UI)
│   ├── Pages/
│   │   ├── Index.cshtml
│   │   ├── Register.cshtml
│   │   ├── Attendance.cshtml
│   ├── wwwroot/
│   │   ├── css/
│   │   ├── js/
│   └── Services/
│
└── database/
    ├── schema.sql
    └── seed.sql
