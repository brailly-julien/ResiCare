# syntax=docker/dockerfile:1
#
# Image de DÉMO auto-suffisante : l'API .NET sert le front Angular (même origine → aucun CORS)
# avec une base SQLite semée au démarrage (rien à héberger, données de démo prêtes).
#   docker build -t resicare .
#   docker run --rm -p 8080:8080 resicare   ->  http://localhost:8080

# ---- 1) Build du front Angular ----
FROM node:22 AS frontend
WORKDIR /app/web
COPY resicare-web/package.json resicare-web/package-lock.json ./
RUN npm ci
COPY resicare-web/ ./
RUN npm run build

# ---- 2) Build + publish du back .NET (embarque le front dans wwwroot) ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend
WORKDIR /src
COPY Directory.Build.props ./
COPY src/ ./src/
RUN dotnet restore src/ResiCare.Api/ResiCare.Api.csproj
# Le build Angular devient le contenu statique servi par l'API.
COPY --from=frontend /app/web/dist/resicare-web/browser/ ./src/ResiCare.Api/wwwroot/
RUN dotnet publish src/ResiCare.Api/ResiCare.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- 3) Image d'exécution ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=backend /app/publish ./

# Config NON sensible de la démo (base SQLite éphémère, re-semée à chaque démarrage ; comptes
# de démo déjà affichés sur la page de login). La clé de signature JWT (Jwt__Key) est
# volontairement ABSENTE : on ne committe aucun secret, même de démo. Elle doit être fournie au
# lancement — variable d'environnement de l'hébergeur, ou `docker run -e Jwt__Key="..."` (>= 32 octets).
ENV ASPNETCORE_ENVIRONMENT=Production \
    Database__Provider=Sqlite \
    ConnectionStrings__Default="Data Source=/tmp/resicare.db" \
    SeedOnStartup=true \
    EnableHttpsRedirection=false \
    RateLimiting__LoginPermitLimit=100 \
    Jwt__Issuer=ResiCare \
    Jwt__Audience=ResiCareClient \
    Jwt__ExpiryMinutes=120

EXPOSE 8080
# Écoute sur $PORT si l'hébergeur en fournit un (Render, Railway...), sinon 8080.
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} exec dotnet ResiCare.Api.dll"]
