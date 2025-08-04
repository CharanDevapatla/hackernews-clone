# Hacker News API Backend

.NET Core Web API that fetches and serves Hacker News stories.

## Run
```bash
dotnet run --project HackerNewsAPI
```

## Test
```bash
dotnet test
```

## Endpoints
- `GET /api/stories/newest` - Latest stories with pagination/search
- `GET /api/stories/{id}` - Single story by ID

## Features
- Memory caching (5 min)
- Dependency injection
- Error handling
- CORS enabled

API runs on https://localhost:5001