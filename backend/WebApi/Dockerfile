# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore WebApi/WebApi.csproj

# publish webapi
RUN dotnet publish WebApi/WebApi.csproj -c Release -o /app/publish

# ---- build EF migrations bundle -> /app/migrate ----
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# IMPORTANT:
# --project: project chứa DbContext + Migrations (thường Infrastructure)
# --startup-project: project startup (WebApi)
RUN dotnet ef migrations bundle \
  --project Infrastructure/Infrastructure.csproj \
  --startup-project WebApi/WebApi.csproj \
  --output /app/migrate \
  --force

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
COPY --from=build /app/migrate /app/migrate

ENTRYPOINT ["dotnet", "WebApi.dll"]
