# VolHub — волонтерський хаб (ЛР2, Crossplatform Programming)

Навчальний сервіс для координації волонтерських запитів: створення заявок, реєстрація волонтерів, призначення виконавців і базова статистика.  
Стек: **.NET 8**, ASP.NET Core Minimal API, **Swagger**, тести **xUnit**, деплой через **Vagrant + VirtualBox**, приватний NuGet (**BaGet**).

**Репозиторій:** https://github.com/makquella/knu-cp-volhub-lab2

---

## Вимоги

- **.NET 8 SDK** — https://dotnet.microsoft.com/
- **Git** — https://git-scm.com/
- _(для демонстрації ЛР2)_ **Vagrant** — https://www.vagrantup.com/ та **VirtualBox** — https://www.virtualbox.org/

Перевір:

```bash
dotnet --info
vagrant --version
VBoxManage --version

Клонування
git clone https://github.com/makquella/knu-cp-volhub-lab2.git
cd knu-cp-volhub-lab2

Локальний запуск API (без Vagrant)
dotnet restore
dotnet build
dotnet run --project src/VolHub.Api
Відкрити Swagger: http://localhost:5000/swagger

Тести
dotnet test

Основні HTTP-ендпоїнти
GET /api/requests?status=New&location=Kyiv&priority=1 — фільтрація заявок
POST /api/requests — тіло: {"title":"Допомога","location":"Kyiv","priority":1}
POST /api/volunteers — тіло: {"name":"Іван","phone":"+380..."}
POST /api/assign — тіло: {"requestId":"<GUID>","volunteerId":"<GUID>"}
POST /api/requests/{id}/start → POST /api/requests/{id}/complete
GET /api/stats — агрегована статистика

ЛР2: деплой через Vagrant (BaGet + Ubuntu)

Піднімаються дві ВМ:
baget — приватний NuGet (BaGet) з API-ключем local-api-key, індекс: http://localhost:5555/v3/index.json
ubuntu — приклад хоста застосунку; ставить .NET 8, додає джерело NuGet і запускає volhub як .NET глобальний tool
fedora - запасний варiант

1. Підняти інфраструктуру:
vagrant up baget
vagrant up ubuntu

2. Перевірити:
BaGet індекс: http://localhost:5555/v3/index.json
Swagger сервісу (в Ubuntu ВМ): http://localhost:5001/swagger

3. Зупинка / видалення:
vagrant halt
vagrant destroy -f

Примітка: у Vagrantfile є профіль Fedora. За потреби можна додати його аналогічно до Ubuntu.

(Опційно) Публікація NuGet-пакета в BaGet
У репозиторії не зберігаються *.nupkg. За потреби згенеруй локально та опублікуй у BaGet.

1. Збірка пакета:
dotnet restore
dotnet build -c Release
dotnet pack src/VolHub.Api -c Release

Очікуваний шлях до артефакту:
src/VolHub.Api/bin/Release/VolHub.Tool.1.0.0.nupkg

2. Пуш у BaGet (коли baget ВМ запущена і слухає http://localhost:5555):
dotnet nuget push src/VolHub.Api/bin/Release/VolHub.Tool.1.0.0.nupkg \
  --source http://localhost:5555/v3/index.json \
  --api-key local-api-key

Перевірити індекс:
http://localhost:5555/v3/index.json

Структура проєкту
VolHub.sln
src/
  VolHub.Api/        — HTTP API + Swagger
  VolHub.Domain/     — доменні моделі/сервіси
tests/
  VolHub.Tests/      — xUnit
Vagrantfile          — BaGet + Ubuntu (Vagrant/VirtualBox)

.gitignore та артефакти:
   У Git не комітяться: bin/, obj/, .vagrant/, *.nupkg, *.snupkg, логи тощо.
   Пакет VolHub.Tool.*.nupkg збирається локально і, за потреби, заливається в BaGet.

Типові проблеми та рішення:
   Порт 5555/5001 зайнятий → змінити у Vagrantfile параметр forwarded_port або закрити процес, що слухає порт.
   dotnet nuget push → connection refused → переконайся, що ВМ baget працює (vagrant status) і контейнер BaGet слухає порт 80 (vagrant ssh baget && sudo docker ps).
   Не монтуються шари у VirtualBox → оновити VirtualBox, перезавантажити ВМ (vagrant reload).
```
