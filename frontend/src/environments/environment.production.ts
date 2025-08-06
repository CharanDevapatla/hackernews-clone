export const environment = {
  production: true,
  apiUrl: (window as unknown as { API_URL?: string }).API_URL || 'https://hacker-news-api-20250805092617.azurewebsites.net'
};