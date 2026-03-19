# PostgreSQL ready-to-use database

Эта папка содержит готовые скрипты для быстрого поднятия БД на любом ПК.

## Файлы

- `bootstrap.sql` - создание таблиц + стартовые данные.
- `reset.sql` - очистка таблиц + повторное заполнение.

## Быстрый запуск через psql

1. Создать базу данных:

```sql
CREATE DATABASE gta_admin_db;
```

2. Выполнить инициализацию:

```powershell
psql -h 127.0.0.1 -p 5432 -U postgres -d gta_admin_db -f "db/postgres/bootstrap.sql"
```

3. При необходимости сбросить и заполнить заново:

```powershell
cd "db/postgres"
psql -h 127.0.0.1 -p 5432 -U postgres -d gta_admin_db -f "reset.sql"
```

## Проверка

```sql
SELECT COUNT(*) FROM users;
SELECT COUNT(*) FROM reports;
SELECT COUNT(*) FROM app_logs;
```
