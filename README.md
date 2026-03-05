# GTA Admin Reports App

Проект для администрирования сервера GTA 5 (FiveM) с автоматической сортировкой репортов по классам администратора:
- `Junior`
- `Senior`
- `Head`

## Что делает приложение

- загружает открытые репорты;
- распределяет репорты по классам администраторов;
- фильтрует список по классу (`Junior`, `Senior`, `Head`);
- позволяет быстро подготовить демо-данные для защиты проекта.

## Технологии

- C# (.NET 8)
- WPF
- Entity Framework Core
- PostgreSQL

## Структура проекта

- `Data/AppDbContext.cs` — EF Core контекст.
- `Data/DatabaseInitializer.cs` — автосоздание локальной БД и тестовых данных.
- `Services/ReportClassifierService.cs` — правила сортировки.
- `Services/ReportService.cs` — загрузка и обновление репортов.
- `ViewModels/MainWindowViewModel.cs` — логика интерфейса.
- `MainWindow.xaml` — интерфейс.
- `appsettings.json` — конфигурация подключения к БД.
- `sql-init.sql` — SQL-скрипт создания таблиц в PostgreSQL.
- `Services/AppLoggerService.cs` — сохранение логов действий и ошибок в БД.

## Правила сортировки

- `chat`, `flood`, `spam`, `insult` -> `Junior`
- `cheat`, `hack`, `aimbot`, `wallhack` -> `Senior`
- `ban_evasion`, `ddos`, `exploit`, `dupe` -> `Head`
- если тип нарушения не определен, применяется правило по `Priority`:
  - `>= 8` -> `Head`
  - `>= 5` -> `Senior`
  - иначе -> `Junior`

---

## Установка и запуск с GitHub (Windows)

### 1) Предварительные требования

Перед установкой проверьте:

- установлен `Git`;
- установлен `.NET SDK 8.0` (рекомендуется 8.0.4xx и выше);
- ОС: Windows 10/11.

Проверка в терминале:

```powershell
git --version
dotnet --version
```

Если `dotnet` не находится:
- перезапустите терминал/IDE;
- либо используйте полный путь:  
  `C:\Program Files\dotnet\dotnet.exe`.

### 2) Клонирование проекта

```powershell
git clone <ССЫЛКА_НА_ВАШ_РЕПОЗИТОРИЙ>
cd GtaAdminReportsApp
```

Пример:

```powershell
git clone https://github.com/<username>/<repo>.git
cd <repo>/GtaAdminReportsApp
```

### 3) Восстановление пакетов и сборка

```powershell
dotnet restore
dotnet build -c Release
```

Если команда `dotnet` недоступна:

```powershell
& "C:\Program Files\dotnet\dotnet.exe" restore
& "C:\Program Files\dotnet\dotnet.exe" build -c Release
```

### 4) Первый запуск (PostgreSQL)

```powershell
dotnet run
```

Перед первым запуском:
- установите PostgreSQL;
- создайте базу данных (например, `gta_admin_db`);
- укажите параметры подключения в `appsettings.json` (`ConnectionStrings:PostgresDatabase`).

При запуске приложения автоматически:
- создаются таблицы (если отсутствуют);
- добавляются стартовые тестовые данные.

### 5) Базовый сценарий проверки

После старта приложения:

1. Нажмите `Сбросить демо-данные`;
2. Нажмите `Заполнить 50 репортов`;
3. Нажмите `Сортировать по классам`;
4. Проверьте колонку `Класс администратора`;
5. Используйте фильтр по классам.

---

## Настройка PostgreSQL

1. Откройте `appsettings.json`;
2. Заполните строку:
   - `ConnectionStrings:PostgresDatabase`
3. Создайте БД и выполните `sql-init.sql` (опционально, можно полагаться на автосоздание);
4. Запустите проект повторно.

Пример `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "PostgresDatabase": "Host=127.0.0.1;Port=5432;Database=gta_admin_db;Username=postgres;Password=postgres;"
  }
}
```

---

## Сборка готового исполняемого файла (.exe)

Команда публикации:

```powershell
dotnet publish -c Release -r win-x64 --self-contained false
```

Файлы будут в каталоге:

`bin/Release/net8.0-windows/win-x64/publish`

Запуск:
- `GtaAdminReportsApp.exe`

---

## Частые проблемы и решения

### 1) `dotnet` не распознан

- перезапустите Windows Terminal / PowerShell / Cursor;
- проверьте наличие файла  
  `C:\Program Files\dotnet\dotnet.exe`;
- запустите команды через полный путь.

### 2) Ошибка подключения к PostgreSQL

- проверьте, что служба PostgreSQL запущена;
- проверьте `Host/Port/Database/Username/Password` в `appsettings.json`;
- убедитесь, что у пользователя есть права на создание таблиц.

### 3) Не отображаются новые данные

- нажмите `Загрузить репорты`;
- убедитесь, что фильтр не ограничивает список (`Все`).

---

## Краткая инструкция для преподавателя

1. Склонировать проект из GitHub;
2. Выполнить `dotnet restore`;
3. Выполнить `dotnet run`;
4. В окне программы нажать:
   - `Сбросить демо-данные`
   - `Заполнить 50 репортов`
   - `Сортировать по классам`
5. Проверить распределение по `Junior/Senior/Head`.
