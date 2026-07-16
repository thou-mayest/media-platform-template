import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  reporter: 'list',
  use: {
    baseURL: 'http://127.0.0.1:4321',
    trace: 'retain-on-failure',
  },
  webServer: [
    {
      command: 'npm run test:mock-api',
      url: 'http://127.0.0.1:5199/health',
      reuseExistingServer: true,
      timeout: 30_000,
    },
    {
      command: 'npm run preview -- --host 127.0.0.1',
      url: 'http://127.0.0.1:4321',
      env: { POSTS_API_URL: 'http://127.0.0.1:5199' },
      reuseExistingServer: true,
      timeout: 30_000,
    },
  ],
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'mobile-chromium',
      use: { ...devices['Pixel 5'] },
    },
  ],
});
