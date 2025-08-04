# Hacker News Feed Viewer

Shows latest Hacker News stories with search and pagination.

## Tech Stack
- Angular frontend
- .NET Core backend  
- Hacker News API

## Quick Start

**Backend:**
```bash
cd backend
dotnet run --project HackerNewsAPI
```

**Frontend:**
```bash
cd frontend  
npm install
ng serve
```

## Requirements
- .NET 8.0
- Node.js 16+
- Angular CLI

## Features
- Story list with pagination
- Search stories
- Click to read articles
- Caching for performance
- Full test coverage

## API
- `GET /api/stories/newest` - Get stories with pagination
- `GET /api/stories/{id}` - Get single story

## Tests
```bash
dotnet test           # Backend
ng test               # Frontend
```