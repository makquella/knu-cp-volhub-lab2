# VolHub

VolHub — навчальний багатокомпонентний проєкт із курсу **«Кросплатформне програмування»**. Репозиторій демонструє еволюцію від консольного застосунку й модульних тестів (ЛР1), створення та публікації NuGet‑пакету з інфраструктурою Vagrant (ЛР2), до вебзастосунку ASP.NET Core MVC з OAuth2/Auth0 (ЛР3) та підтримки кількох СУБД із довідниками й розширеним пошуком (ЛР4).

---

## Структура рішення

| Шлях | Опис |
|------|------|
| `src/VolHub.Domain` | Доменні моделі та бізнес-логіка (ЛР1). |
| `src/VolHub.Api` | Minimal API / консольні сценарії (ЛР1–ЛР2). |
| `src/VolHub.Mvc` | ASP.NET Core MVC застосунок (ЛР3–ЛР4). |
| `tests/VolHub.Tests` | Модульні тести (xUnit). |
| `Vagrantfile`, `scripts/` | Інфраструктура для ЛР2 (Vagrant). |

Нижче наведено інструкції для запуску кожної лабораторної роботи.

---

## Лабораторна робота 1 — «C# застосунок і тести»

**Мета:** реалізувати базову доменну логіку, підняти Minimal API та додати модульні тести.

### Кроки
1. **Відновити залежності**
   ```powershell
   dotnet restore
   ```
2. **Запустити Minimal API**
   ```powershell
   dotnet run --project src/VolHub.Api
   ```
   - Базова адреса: `http://localhost:5000`
   - Swagger UI: `http://localhost:5000/swagger`
3. **Запустити тести**
   ```powershell
   dotnet test
   ```

---

## Лабораторна робота 2 — «NuGet + Vagrant»

**Мета:** зібрати NuGet‑пакет, розгорнути локальний реєстр та показати роботу застосунку на різних ОС через Vagrant.

### Передумови
- Встановлені Vagrant + VirtualBox/Hyper-V
- Локальний реєстр NuGet (BaGet контейнер або інсталяція)

### Кроки
1. **Побудувати пакунок**
   ```powershell
   dotnet pack src/VolHub.Api -c Release
   ```
2. **Опублікувати у BaGet**
   ```powershell
   dotnet nuget push src/VolHub.Api/bin/Release/VolHub.Tool.*.nupkg `
     --source http://localhost:5555/v3/index.json `
     --api-key local-api-key
   ```
3. **Підняти віртуальні машини**
   ```powershell
   vagrant up baget      # BaGet (Ubuntu + Docker)
   vagrant up ubuntu     # Ubuntu 22.04 + .NET 8
   vagrant up fedora     # Fedora 41 + .NET 8
   ```
4. **Зупинити / прибрати машини**
   ```powershell
   vagrant halt
   vagrant destroy -f
   ```

---

## Лабораторна робота 3 — «ASP.NET Core MVC + Auth0»

**Мета:** побудувати вебзастосунок із реєстрацією, авторизацією через Auth0 (OIDC) та трьома підпрограмами BMI/TextTool/Geo.

### Передумови
- Обліковий запис Auth0 (або інший OIDC-провайдер)
- .NET SDK 8.0

### Кроки
1. **Перейти до MVC-проєкту**
   ```powershell
   cd src/VolHub.Mvc
   ```
2. **Відновити та зібрати**
   ```powershell
   dotnet restore
   dotnet build
   ```
3. **Налаштувати секрети Auth0**
   ```powershell
   dotnet user-secrets set "Auth0:Domain" "<your-domain>"
   dotnet user-secrets set "Auth0:ClientId" "<client-id>"
   dotnet user-secrets set "Auth0:ClientSecret" "<client-secret>"
   ```
4. **Підготувати базу (SQLite за замовчуванням)**
   ```powershell
   dotnet ef database update
   ```
5. **Запустити застосунок**
   ```powershell
   dotnet run --launch-profile "VolHub.Mvc"
   ```
   - URL: `https://localhost:5032`
   - Доступні сторінки: реєстрація/вхід (Auth0), профіль, підпрограми BMI/TextTool/Geo (після авторизації).

---

## Лабораторна робота 4 — «Кілька СУБД, довідники та пошук»

**Мета:** додати підтримку чотирьох СУБД (Sqlite, SqlServer, Postgres, InMemory), розширити інтерфейс довідниками та центральною таблицею подій, реалізувати сторінку розширеного пошуку.

### Передумови
- .NET SDK 8.0
- Готова інстанція потрібної СУБД (SQLite входить до .NET; для SQL Server/PostgreSQL потрібні локальні інсталяції або контейнери)

### Кроки
1. **Налаштувати провайдера БД**
   - У `src/VolHub.Mvc/appsettings.json` встановити `Database:Provider` на `Sqlite`, `SqlServer`, `Postgres` або `InMemory`.
   - Оновити відповідний рядок у розділі `ConnectionStrings`.
2. **Застосувати міграції**
   ```powershell
   dotnet ef database update --project src/VolHub.Mvc --startup-project src/VolHub.Mvc
   ```
3. **(За потреби) Налаштувати секрети Auth0** — аналогічно до ЛР3.
4. **Запустити застосунок**
   ```powershell
   dotnet run --project src/VolHub.Mvc --launch-profile "VolHub.Mvc"
   ```
5. **Перевірити нові сторінки**
   - «Довідники» → «Категорії подій» та «Локації проведення» (список + деталі з пов’язаними подіями).
   - «Події» → центральна таблиця волонтерських заходів (список + деталі).
   - «Пошук» → фільтри за датами, множинними категоріями, початком назви, закінченням організатора (з JOIN категорій і локацій).

---

## Загальні корисні команди

```powershell
# Збірка всього рішення
dotnet build

# Тести
dotnet test

# Додати нову міграцію (MVC)
dotnet ef migrations add <Name> --project src/VolHub.Mvc --startup-project src/VolHub.Mvc

# Оновити базу для обраного провайдера
dotnet ef database update --project src/VolHub.Mvc --startup-project src/VolHub.Mvc
```

---

## Ліцензія

Проєкт опублікований під ліцензією MIT. Повний текст — у файлі [`LICENSE`](LICENSE).
