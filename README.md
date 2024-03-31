Запуск проекта:

1. Убедиться, что установлен .NET 8.0 SDK (https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
2. Убедиться, что установлен Docker
4. Склонировать репозиторий
5. Запустить контейнер Postgresql, запустив docker-compose.yml. Для этого открыть консоль (терминал) в директории с csproj файлом, например `cd CFPService/CFPService`
6. Выполнить команду `docker compose up'
7. После развёртывания бд выполнить команду `dotnet run`
9. После сборки и запуска приложения перейти на http://localhost:5108/swagger/index.html
