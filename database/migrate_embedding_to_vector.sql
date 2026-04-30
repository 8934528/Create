
CREATE EXTENSION IF NOT EXISTS vector;

ALTER TABLE faces
    ALTER COLUMN embedding TYPE vector(128)
    USING embedding::vector;


CREATE INDEX IF NOT EXISTS faces_embedding_ivfflat_idx
    ON faces
    USING ivfflat (embedding vector_l2_ops)
    WITH (lists = 100);

ANALYSE faces;

-- Verification query
SELECT column_name, data_type, udt_name
FROM information_schema.columns
WHERE table_name = 'faces'
  AND column_name = 'embedding';
