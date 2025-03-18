#docker build -f C:\Users\ivanov\source\repos\MatrixResponsibility\MatrixResponsibility.Client\Dockerfile C:\Users\ivanov\source\repos\MatrixResponsibility -t ivanovm/matrixresponsibility.client; docker push ivanovm/matrixresponsibility.client:latest

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app
COPY ["MatrixResponsibility.Client/MatrixResponsibility.Client.csproj", "MatrixResponsibility.Client/"]
RUN dotnet restore ./MatrixResponsibility.Client/MatrixResponsibility.Client.csproj
COPY . .
RUN dotnet publish ./MatrixResponsibility.Client/MatrixResponsibility.Client.csproj -c $BUILD_CONFIGURATION -o output

FROM nginx:alpine
WORKDIR /var/www/web
COPY --from=build /app/output/wwwroot .
COPY MatrixResponsibility.Client/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80