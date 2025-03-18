# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
# Создаем директорию для данных
RUN mkdir -p /app/Data

# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MatrixResponsibility/MatrixResponsibility.csproj", "MatrixResponsibility/"]
RUN dotnet restore "./MatrixResponsibility/MatrixResponsibility.csproj"
COPY . .
WORKDIR "/src/MatrixResponsibility"
RUN dotnet build "./MatrixResponsibility.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MatrixResponsibility.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Копируем Excel-файлы из папки Data в контейнер
RUN ls -la Data/  # Проверяем, что файлы видны в контексте
COPY MatrixResponsibility/Data/*.xlsx /app/Data/
ENTRYPOINT ["dotnet", "MatrixResponsibility.dll"]