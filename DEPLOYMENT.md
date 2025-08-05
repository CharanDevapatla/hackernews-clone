# Deploying Hacker News App to Azure

## Backend Deployment

```bash
cd backend
dotnet publish HackerNewsAPI/HackerNewsAPI.csproj -c Release -o ./publish
cd publish
zip -r ../backend.zip .
cd ..

az group create --name hacker-news-rg --location eastus

az appservice plan create \
    --name hacker-news-plan \
    --resource-group hacker-news-rg \
    --sku B1 \
    --is-linux

az webapp create \
    --resource-group hacker-news-rg \
    --plan hacker-news-plan \
    --name hacker-news-api-20240805 \
    --runtime "DOTNETCORE:8.0"

az webapp deploy \
    --resource-group hacker-news-rg \
    --name hacker-news-api-20240805 \
    --src-path backend.zip \
    --type zip

az webapp cors add \
    --resource-group hacker-news-rg \
    --name hacker-news-api-20240805 \
    --allowed-origins "*"
```

## Frontend Deployment

```bash
cd frontend
npm install
npm run build -- --configuration production
```

Create `dist/frontend/browser/server.js`:
```javascript
const express = require('express');
const path = require('path');
const app = express();

const API_URL = process.env.API_URL || 'https://hacker-news-api-20240805.azurewebsites.net';

app.use(express.static(__dirname));

app.get('/index.html', (req, res) => {
  const fs = require('fs');
  let html = fs.readFileSync(path.join(__dirname, 'index.html'), 'utf8');
  html = html.replace('</head>', `<script>window.API_URL = '${API_URL}';</script></head>`);
  res.send(html);
});

app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, 'index.html'));
});

const port = process.env.PORT || 8080;
app.listen(port);
```

Create `dist/frontend/browser/package.json`:
```json
{
  "name": "hacker-news-frontend",
  "version": "1.0.0",
  "scripts": {
    "start": "node server.js"
  },
  "dependencies": {
    "express": "^4.18.2"
  }
}
```

```bash
cd dist/frontend/browser
npm install --production
zip -r ../../../frontend.zip .
cd ../../..

az webapp create \
    --resource-group hacker-news-rg \
    --plan hacker-news-plan \
    --name hacker-news-web-20240805 \
    --runtime "NODE:20-lts"

az webapp deploy \
    --resource-group hacker-news-rg \
    --name hacker-news-web-20240805 \
    --src-path frontend.zip \
    --type zip

az webapp config appsettings set \
    --resource-group hacker-news-rg \
    --name hacker-news-web-20240805 \
    --settings API_URL=https://hacker-news-api-20240805.azurewebsites.net
```

## Cleanup

```bash
az group delete --name hacker-news-rg --yes --no-wait
```
