# Create

## features

## SetUp Environment Variables

events, work, class smart register collection system using face id

create the `.env` file copy and pste the following:

        DB_HOST=localhost
        DB_PORT=5432
        DB_USER=postgres
        DB_PASSWORD=[put your postgres password]
        DB_NAME=create_db

        JWT_SECRET=super_secret_key
        AI_SERVICE_URL=http://localhost:8000

## run

1. Setup the Database (PostgreSQL)
Ensure PostgreSQL is running on your machine, then run these commands in a terminal or pgAdmin:

        sql
        -- 1. Create the database
        CREATE DATABASE create_db;
        -- 2. Connect to create_db and enable the vector extension
        -- (This must be done inside the create_db database)

        CREATE EXTENSION IF NOT EXISTS vector;

    If you get an error that "vector" is not available, you need to install pgvector for your PostgreSQL version.

2. Run the AI Service (Python)

    You already have this running. It provides the face embedding logic on `http://localhost:8000`.

3. Run the Backend API (.NET)

    Open a new terminal and run:

        bash
        cd backend-dotnet/Create.API
        dotnet run
        ```
        This will start the API at `http://localhost:5242`. You can view the Swagger documentation at `http://localhost:5242/swagger`.

4. Run the Frontend (Web)

        Open another terminal and run:

        bash
        cd frontend-web
        dotnet run
        

    This will start the website. Look for the `http://localhost:5200` link in the output.

## Access the App

Click this link once the frontend is running: `http://localhost:5200`

You can then navigate to:

- Register to add a new user with their face.
- Attendance to start the real-time face scanner.
- Dashboard to view attendance analytics.

## contribution
