-- Enable Extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS vector;

-- USERS
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    full_name VARCHAR(150) NOT NULL,
    email VARCHAR(150) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    role VARCHAR(50) NOT NULL, -- student, employee, attendee
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- FACE DATA (Embeddings)
CREATE TABLE faces (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    embedding VECTOR(128), -- pgvector enabled
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- EVENTS / SESSIONS
CREATE TABLE events (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(150),
    location VARCHAR(150),
    start_time TIMESTAMP,
    end_time TIMESTAMP
);

-- ATTENDANCE
CREATE TABLE attendance (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id),
    event_id UUID REFERENCES events(id),
    check_in_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status VARCHAR(50) -- present / late / denied
);

