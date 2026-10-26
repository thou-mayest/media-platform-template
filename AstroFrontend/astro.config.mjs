import { defineConfig } from "astro/config";

export default defineConfig({
  site: process.env.ASTRO_SITE_URL ?? "http://localhost",
  vite: {
    server: {
      proxy: {
        "/api": {
          target: process.env.API_BASE_URL ?? "http://127.0.0.1:5048",
          changeOrigin: true,
        },
      },
    },
  },
});
