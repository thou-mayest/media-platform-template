import { defineConfig } from "astro/config";

const isProductionBuild = process.argv.some((argument) => argument === "build");
const siteUrl = process.env.SITE_URL?.trim();
const inquiryEmail = process.env.PUBLIC_INQUIRY_EMAIL?.trim();

if (isProductionBuild && !siteUrl) {
  throw new Error("SITE_URL is required for production builds.");
}

if (isProductionBuild && !inquiryEmail) {
  throw new Error("PUBLIC_INQUIRY_EMAIL is required for production builds.");
}

if (siteUrl) {
  const parsedSite = new URL(siteUrl);
  if (
    parsedSite.protocol !== "https:" ||
    parsedSite.username ||
    parsedSite.password ||
    parsedSite.pathname !== "/" ||
    parsedSite.search ||
    parsedSite.hash ||
    parsedSite.hostname === "localhost" ||
    parsedSite.hostname === "127.0.0.1"
  ) {
    throw new Error("SITE_URL must be a public HTTPS root origin.");
  }
}

if (inquiryEmail && !/^[^\s@?&#%]+@[^\s@?&#%]+\.[^\s@?&#%]+$/.test(inquiryEmail)) {
  throw new Error("PUBLIC_INQUIRY_EMAIL must be a valid email address.");
}

export default defineConfig({
  site: siteUrl || "http://localhost:4321",
  compressHTML: true,
});
