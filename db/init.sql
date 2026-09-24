CREATE TABLE IF NOT EXISTS elements (
    id              BIGSERIAL PRIMARY KEY,
    attribute_value TEXT,
    html_code       TEXT NOT NULL,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_elements_attribute_value
    ON elements (attribute_value);