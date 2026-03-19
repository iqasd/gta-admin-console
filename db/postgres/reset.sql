-- Reset script for GTA Admin Reports App (PostgreSQL)
-- Clears application tables and re-runs bootstrap seed.

BEGIN;

TRUNCATE TABLE app_logs RESTART IDENTITY CASCADE;
TRUNCATE TABLE reports RESTART IDENTITY CASCADE;
TRUNCATE TABLE users RESTART IDENTITY CASCADE;

COMMIT;

-- Recreate initial data
\i bootstrap.sql
