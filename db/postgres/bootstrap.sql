-- Bootstrap script for GTA Admin Reports App (PostgreSQL)
-- Creates schema and inserts initial demo data.

BEGIN;

CREATE TABLE IF NOT EXISTS users (
    "Id" SERIAL PRIMARY KEY,
    "Username" VARCHAR(64) NOT NULL,
    "AdminClass" INTEGER NOT NULL DEFAULT 1,
    "IsAdmin" BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS reports (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "PlayerName" VARCHAR(64) NOT NULL,
    "ViolationType" VARCHAR(64) NOT NULL,
    "Description" VARCHAR(1024) NOT NULL,
    "Priority" INTEGER NOT NULL DEFAULT 1,
    "Status" VARCHAR(32) NOT NULL DEFAULT 'Open',
    "TargetAdminClass" INTEGER NOT NULL DEFAULT 0,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "ProcessedAt" TIMESTAMP NULL,
    CONSTRAINT "FK_reports_users" FOREIGN KEY ("UserId") REFERENCES users("Id")
);

CREATE TABLE IF NOT EXISTS app_logs (
    "Id" BIGSERIAL PRIMARY KEY,
    "Level" VARCHAR(16) NOT NULL,
    "Action" VARCHAR(64) NOT NULL,
    "Message" VARCHAR(512) NOT NULL,
    "Details" VARCHAR(2048) NULL,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Optional helper indexes for reporting/sorting performance.
CREATE INDEX IF NOT EXISTS idx_reports_violation_type ON reports ("ViolationType");
CREATE INDEX IF NOT EXISTS idx_reports_target_admin_class ON reports ("TargetAdminClass");
CREATE INDEX IF NOT EXISTS idx_reports_status ON reports ("Status");

-- Seed users (3 admins + 2 players).
INSERT INTO users ("Username", "AdminClass", "IsAdmin")
SELECT * FROM (
    VALUES
        ('AdminJunior_1', 1, TRUE),
        ('AdminSenior_1', 2, TRUE),
        ('AdminHead_1', 3, TRUE),
        ('Player_One', 1, FALSE),
        ('Player_Two', 1, FALSE)
) AS seed("Username", "AdminClass", "IsAdmin")
WHERE NOT EXISTS (SELECT 1 FROM users);

-- Seed reports based on existing player IDs.
WITH p1 AS (
    SELECT "Id", "Username" FROM users WHERE "Username" = 'Player_One' LIMIT 1
),
p2 AS (
    SELECT "Id", "Username" FROM users WHERE "Username" = 'Player_Two' LIMIT 1
)
INSERT INTO reports (
    "UserId",
    "PlayerName",
    "ViolationType",
    "Description",
    "Priority",
    "Status",
    "TargetAdminClass",
    "CreatedAt"
)
SELECT * FROM (
    SELECT
        p1."Id",
        p1."Username",
        'flood',
        'Многократные однотипные сообщения в чат.',
        3,
        'Open',
        1,
        NOW() - INTERVAL '30 minutes'
    FROM p1
    UNION ALL
    SELECT
        p2."Id",
        p2."Username",
        'cheat',
        'Подозрение на использование стороннего ПО.',
        7,
        'Open',
        2,
        NOW() - INTERVAL '20 minutes'
    FROM p2
    UNION ALL
    SELECT
        p1."Id",
        p1."Username",
        'exploit',
        'Обнаружено использование критического эксплойта.',
        9,
        'Open',
        3,
        NOW() - INTERVAL '10 minutes'
    FROM p1
) AS seed_reports
WHERE NOT EXISTS (SELECT 1 FROM reports);

INSERT INTO app_logs ("Level", "Action", "Message", "Details")
SELECT 'Info', 'Bootstrap', 'Инициализация базы данных выполнена.', 'Созданы таблицы и добавлены стартовые данные.'
WHERE NOT EXISTS (SELECT 1 FROM app_logs);

COMMIT;
