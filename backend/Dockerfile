# Multi-stage build for .NET 8 application

# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution file first
COPY *.sln ./

# Copy project files
COPY HackerNewsAPI/*.csproj ./HackerNewsAPI/
COPY HackerNewsAPI.Tests/*.csproj ./HackerNewsAPI.Tests/

RUN dotnet restore

# Copy everything else
COPY . ./
RUN dotnet publish HackerNewsAPI/HackerNewsAPI.csproj -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "HackerNewsAPI.dll"]
