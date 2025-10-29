# VolHub

VolHub — навчальний багатокомпонентний проєкт із курсу **«Кросплатформне програмування»**. Репозиторій демонструє поступове ускладнення: від консольного застосунку й тестів (ЛР1), створення NuGet-пакету та інфраструктури (ЛР2), до ASP.NET Core MVC з OAuth2 (ЛР3) та підтримки кількох СУБД із довідниками й пошуком (ЛР4).

---

## Структура рішення

| Шлях | Опис |
|------|------|
| `src/VolHub.Domain` | Доменні моделі та бізнес-логіка (ЛР1). |
| `src/VolHub.Api` | Minimal API / консольні сценарії (ЛР1–ЛР2). |
| `src/VolHub.Mvc` | ASP.NET Core MVC застосунок (ЛР3–ЛР4). |
| `tests/VolHub.Tests` | Модульні тести (xUnit). |
| `Vagrantfile`, `scripts/` | Інфраструктура для ЛР2 (Vagrant). |
| `VolHub_Zvit_UA.docx` | Поточний шаблон звіту (оновлено для ЛР4). |

Далі — покрокові інструкції для запуску кожної лабораторної роботи.

---

## Лабораторна робота 1 — «C# застосунок і тести»

**Мета:** побудувати базову доменну логіку, запустити Minimal API та написати модульні тести.

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

**Основні артефакти:**  
`src/VolHub.Domain`, `src/VolHub.Api`, `tests/VolHub.Tests`

---

## Лабораторна робота 2 — «NuGet + Vagrant»

**Мета:** створити NuGet-пакет, підняти інфраструктуру на Vagrant та протестувати багатоплатформний запуск.

### Передумови
- Встановлений Vagrant + VirtualBox/Hyper-V
- Локальний реєстр NuGet (BaGet) або контейнер із ним

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
3. **Підняти інфраструктуру**
   ```powershell
   vagrant up baget      # Реєстр NuGet (Ubuntu + Docker)
   vagrant up ubuntu     # Ubuntu 22.04 + .NET 8
   vagrant up fedora     # Fedora 41 + .NET 8
   ```
4. **Зупинити/прибрати машини**
   ```powershell
   vagrant halt
   vagrant destroy -f
   ```

**Основні артефакти:**  
`Vagrantfile`, сценарії `dotnet pack / dotnet nuget push`, опис у `README`, гілка з конфігураціями машин.

---

## Лабораторна робота 3 — «ASP.NET Core MVC + Auth0»

**Мета:** створити вебзастосунок із реєстрацією, авторизацією через OAuth2 (Auth0) та трьома підпрограмами (BMI, TextTool, Geo).

### Передумови
- Обліковий запис Auth0 (або інший OIDC-провайдер)
- .NET SDK 8.0

### Кроки
1. **Перейти в MVC проєкт**
   ```powershell
   cd src/VolHub.Mvc
   ```
2. **Відновити та зібрати**
   ```powershell
   dotnet restore
   dotnet build
   ```
3. **Налаштувати secrets для Auth0**
   ```powershell
   dotnet user-secrets set "Auth0:Domain" "<your-domain>"
   dotnet user-secrets set "Auth0:ClientId" "<client-id>"
   dotnet user-secrets set "Auth0:ClientSecret" "<client-secret>"
   ```
4. **Оновити базу (SQLite за замовчуванням)**
   ```powershell
   dotnet ef database update
   ```
5. **Запустити застосунок**
   ```powershell
   dotnet run --launch-profile "VolHub.Mvc"
   ```
   - URL: `https://localhost:5032`
   - Доступні сторінки:
     - реєстрація/вхід (Auth0)
     - профіль користувача
     - підпрограми BMI, TextTool, Geo (після авторизації)

**Основні артефакти:**  
контролери `AccountController`, `SubroutinesController`, Razor-сторінки у `Views/Account`, `Views/Subroutines`, моделі `AppUserProfile`, валідації паролів.

---

## Лабораторна робота 4 — «Кілька СУБД, довідники та пошук»

**Мета:** розширити вебзастосунок для роботи з чотирма СУБД, додати довідники, центральну таблицю подій та сторінку пошуку із складними фільтрами.

### Передумови
- .NET SDK 8.0
- Одна або кілька СУБД:
  - SQLite (за замовчуванням)
  - SQL Server
  - PostgreSQL
  - InMemory (для тестових сценаріїв)

### Кроки
1. **Налаштувати провайдера БД**
   - У `src/VolHub.Mvc/appsettings.json` змінити `Database:Provider` на `Sqlite`, `SqlServer`, `Postgres` або `InMemory`.
   - Оновити відповідний рядок у `ConnectionStrings`.
2. **Застосувати міграції**
   ```powershell
   dotnet ef database update --project src/VolHub.Mvc --startup-project src/VolHub.Mvc
   ```
3. **(Опційно) Оновити Auth0 secrets** — якщо ще не налаштовано (див. ЛР3).
4. **Запустити застосунок**
   ```powershell
   dotnet run --project src/VolHub.Mvc --launch-profile "VolHub.Mvc"
   ```
5. **Перевірити новий функціонал**
   - меню «Довідники» → «Категорії подій», «Локації проведення»
   - «Події» → список волонтерських заходів
   - «Пошук» → фільтри (дата від/до, множинний вибір категорій, початок назви, закінчення організатора)

### Що нового в коді
- `Program.cs` перемикає `UseSqlite / UseSqlServer / UseNpgsql / UseInMemoryDatabase`.
- `Models/EventModels.cs`, `Data/AppDbContext.cs`, міграція `20251029215031_Lab4Events*` та сид-дані.
- Нові контролери й представлення: `EventCategoriesController`, `VenuesController`, `VolunteerEventsController`, `EventSearchController` (+ Razor-сторінки).
- Оновлений `_Layout.cshtml` із новою навігацією.

---

## Загальні команди (нагадування)

```powershell
# Збірка всього рішення
dotnet build

# Тести
dotnet test

# Створення нової міграції (MVC)
dotnet ef migrations add <Name> --project src/VolHub.Mvc --startup-project src/VolHub.Mvc

# Оновити базу для поточного провайдера
dotnet ef database update --project src/VolHub.Mvc --startup-project src/VolHub.Mvc
```

---

## Звіти

- `VolHub_Zvit_UA.docx` — актуальний шаблон звіту для ЛР4 (містить структуру, опис етапів і місця для скріншотів).
- Для попередніх лабораторних можна використовувати історію репозиторію або адаптувати поточний документ.

---

## Ліцензія

Проєкт опублікований під ліцензією MIT. Повний текст — у файлі [`LICENSE`](LICENSE).
