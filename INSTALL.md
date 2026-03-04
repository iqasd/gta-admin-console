# INSTALL.md

Краткая инструкция по установке и запуску проекта `GtaAdminReportsApp` из GitHub.

## 1. Требования

- Windows 10/11
- Git
- .NET SDK 8.0

Проверка:

```powershell
git --version
dotnet --version
```

Если `dotnet` не найден, используйте:

```powershell
& "C:\Program Files\dotnet\dotnet.exe" --version
```

## 2. Клонирование репозитория

```powershell
git clone https://github.com/<username>/<repo>.git
cd <repo>/GtaAdminReportsApp
```

## 3. Установка зависимостей и сборка

```powershell
dotnet restore
dotnet build -c Release
```

## 4. Быстрый запуск (без MySQL)

По умолчанию проект работает на локальной SQLite базе.

```powershell
dotnet run
```

После первого запуска автоматически создаются:
- файл БД `fivem_local.db`;
- таблицы;
- стартовые демо-данные.

## 5. Проверка работы в интерфейсе

1. Нажать `Сбросить демо-данные`;
2. Нажать `Заполнить 50 репортов`;
3. Нажать `Сортировать по классам`;
4. Убедиться, что репорты распределены по `Junior/Senior/Head`.

## 6. (Опционально) Переключение на MySQL

В файле `appsettings.json`:
- установить `"DatabaseProvider": "MySql"`;
- заполнить строку `ConnectionStrings:FiveMDatabase`.

Запуск после настройки:

```powershell
dotnet run
```

## 7. Сборка .exe для передачи

```powershell
dotnet publish -c Release -r win-x64 --self-contained false
```

Результат:
- `bin/Release/net8.0-windows/win-x64/publish/GtaAdminReportsApp.exe`
