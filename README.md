# VolHub

VolHub — навчальний проєкт з кросплатформного програмування. Репозиторій містить три послідовні лабораторні роботи:

- **Lab 1:** кросплатформний застосунок на .NET з модульними тестами;
- **Lab 2:** пакування застосунку у NuGet та розгортання через Vagrant на декількох ОС;
- **Lab 3:** ASP.NET Core MVC застосунок з інтеграцією OAuth2 (Auth0).

---

## Lab 1 — C# Application & Tests

### Тематика та структура
- Тема: управління волонтерськими запитами.
- Основні проєкти:
  - `src/VolHub.Domain` — доменна модель та сервіси.
  - `src/VolHub.Api` — Minimal API (можна запускати як глобальний .NET tool).
  - `tests/VolHub.Tests` — модульні тести на xUnit.

### Як запустити застосунок
```powershell
dotnet restore
dotnet run --project src/VolHub.Api
```
API відкриється на `http://localhost:5000`, Swagger доступний за `/swagger`.

### Тести
```powershell
dotnet test
```
Тести перевіряють бізнес-правила сервісу призначення заявок.

### Документація
- Опис теми, інструкції зі збірки та запуску наведені в цьому README.
- Коментарі у коді уточнюють призначення ключових класів та сценаріїв.

---

## Lab 2 — Vagrant & Multi-OS Deployment

### Крок 1. Пакування та публікація NuGet-пакету
```powershell
# Створення пакета (глобальний .NET tool)
dotnet pack src/VolHub.Api -c Release

# Публікація у BaGet (локальний NuGet-сервер)
dotnet nuget push src/VolHub.Api/bin/Release/VolHub.Tool.1.0.0.nupkg `
  --source http://localhost:5555/v3/index.json `
  --api-key local-api-key
```
> Публікація виконується після старту віртуальної машини `baget` (див. нижче) або будь-якого іншого сервера з підтримкою NuGet API.

### Крок 2. Розгортання через Vagrant
```powershell
vagrant up baget      # BaGet (Ubuntu, Docker)
vagrant up ubuntu     # Ubuntu 22.04 + .NET 8
vagrant up fedora     # Fedora 41 + .NET 8
```
- Ubuntu та Fedora машини встановлюють .NET SDK, підключають приватний NuGet (`VolHubBaGet`), інсталюють глобальний інструмент `volhub` та запускають API на `0.0.0.0:5000`.
- Порти проброшені на хост:
  - BaGet: `http://localhost:5555/v3/index.json`
  - Ubuntu API: `http://localhost:5001/swagger`
  - Fedora API: `http://localhost:5002/swagger`

### Очистка
```powershell
vagrant halt
vagrant destroy -f
```

### Чекліст для демонстрації
1. Показати пакування (`dotnet pack`) та публікацію в BaGet.
2. Запустити `vagrant up` та продемонструвати роботу API на кожній ОС.
3. Пояснити сценарій використання NuGet-джерела.

---

## Lab 3 — ASP.NET Core MVC + OAuth2

### Функціональність
- Головна сторінка з описом лабораторної та швидкими кнопками логіну / переходу до підпроцедур.
- Авторизація через Auth0 (OIDC). Анонімні користувачі при зверненні до підпроцедур отримують редирект на логін.
- Сторінка реєстрації з полями: логін (унікальний, ≤50), ПІБ (≤500), телефон у форматі `+380XXXXXXXXX`, пароль + підтвердження (8–16, цифра, спецсимвол, велика літера), e-mail (RFC 5322).
- Профіль користувача (редагування ПІБ/телефону) з валідацією та повідомленням про успішне збереження.
- Три підпроцедури (доступні лише після входу):
  1. **BMI** — розрахунок індексу, легенда категорій.
  2. **TextTool** — нормалізація ПІБ, рахунок символів і слів, валідація телефону/e-mail.
  3. **Geo** — відстань між координатами (Haversine) з підказками.

Кожна форма містить `@Html.AntiForgeryToken()`, валідаційні повідомлення та плейсхолдери.

### Підготовка Auth0
- Створіть застосунок типу **Regular Web App**.
- Налаштуйте:
  - Allowed Callback URLs: `https://localhost:5032/signin-oidc`
  - Allowed Logout URLs: `https://localhost:5032/signout-callback-oidc`
  - Allowed Web Origins: `https://localhost:5032`

### Локальний запуск
```powershell
cd src/VolHub.Mvc

dotnet restore
dotnet build

# User-secrets (заповніть власними значеннями)
dotnet user-secrets set "Auth0:Domain" "makquella.eu.auth0.com"
dotnet user-secrets set "Auth0:ClientId" "<YOUR_CLIENT_ID>"
dotnet user-secrets set "Auth0:ClientSecret" "<YOUR_CLIENT_SECRET>"

# Міграції та база (SQLite)
dotnet ef database update

# Запуск
dotnet run --launch-profile "VolHub.Mvc"
```

### База даних
- SQLite, файл `volhub.db` винесено у `.gitignore`.
- Міграція `AddUserProfile` створює таблицю профілів з унікальними індексами на логін та e-mail. Паролі зберігаються у вигляді хешу.

### Чекліст скріншотів / демонстрації
1. `dotnet run` + відкриття головної (`https://localhost:5032`).
2. Форма реєстрації: показати помилки (пароль/телефон) та успішну реєстрацію.
3. Логін через Auth0 (адреса `.../signin-oidc` у рядку браузера).
4. Сторінка профілю з редагуванням ПІБ/телефону.
5. Спроба доступу до підпроцедур до/після авторизації.
6. Робота BMI/TextTool/Geo (ввід даних → результат).
7. Сторінка 404 та приклад summary валідації.
8. Огляд структури `volhub.db` (наприклад, у DB Browser for SQLite).

---

## Корисні команди

- Відновлення та збірка всього рішення:
  ```powershell
  dotnet restore
  dotnet build
  ```
- Лінтинг (за потреби) — використовувати стандартні Visual Studio / Rider інструменти.
- Перезапис міграцій (якщо потрібно):
  ```powershell
  dotnet ef migrations add <Name> --project src/VolHub.Mvc --startup-project src/VolHub.Mvc
  dotnet ef database update --project src/VolHub.Mvc --startup-project src/VolHub.Mvc
  ```

---

## Ліцензія

Проєкт розповсюджується за ліцензією MIT. Див. файли ліцензій у директоріях залежностей для додаткових деталей.
