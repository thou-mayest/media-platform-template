import { defineConfig } from "astro/config";
import node from "@astrojs/node";

export default defineConfig({
  site: "http://192.168.100.45:4321",
  output: "hybrid",
  adapter: node({ mode: "standalone" }),
});
