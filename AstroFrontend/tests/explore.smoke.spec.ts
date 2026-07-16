import { expect, test } from '@playwright/test';

test('home renders popular and newest posts from the API', async ({ page }) => {
  await page.goto('/');

  await expect(page.getByRole('heading', { name: 'Discover work worth returning to.' })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Most viewed' })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Newest uploads' })).toBeVisible();
  await expect(page.getByRole('link', { name: 'View Market at Noon' }).first()).toBeVisible();
});

test('Explore filters through normal server-rendered form submissions', async ({ page }) => {
  await page.goto('/explore');

  await expect(page.getByText('24 works')).toBeVisible();
  await page.getByRole('combobox', { name: 'Category', exact: true }).selectOption('Painting');
  await page.getByRole('combobox', { name: 'Tag', exact: true }).selectOption('abstract');
  await page.getByRole('combobox', { name: 'Sort', exact: true }).selectOption('popular');
  await page.getByRole('button', { name: 'Apply filters' }).click();

  await expect(page).toHaveURL(/category=Painting/);
  await expect(page).toHaveURL(/tag=abstract/);
  await expect(page).toHaveURL(/sort=popular/);
  await expect(page.getByText('6 works')).toBeVisible();
});

test('tag page renders a dedicated server-side result page', async ({ page }) => {
  await page.goto('/tag/abstract');

  await expect(page.getByRole('heading', { name: '#abstract' })).toBeVisible();
  await expect(page.getByText('6 published works')).toBeVisible();
});

test('search form redirects to a keyword URL and renders results', async ({ page }) => {
  await page.goto('/search');
  await page.getByLabel('Keyword').fill('market');
  await page.getByRole('button', { name: 'Search' }).click();

  await expect(page).toHaveURL(/\/search\/market$/);
  await expect(page.getByRole('heading', { name: '“market”' })).toBeVisible();
  await expect(page.getByText('1 result')).toBeVisible();
});

test('public SSR pages send cache headers and no browser scripts', async ({ page, request }) => {
  const publicPages = [
    { path: '/', maxAge: 60, sharedMaxAge: 300 },
    { path: '/explore', maxAge: 120, sharedMaxAge: 600 },
    { path: '/tag/abstract', maxAge: 120, sharedMaxAge: 600 },
    { path: '/search', maxAge: 120, sharedMaxAge: 600 },
    { path: '/search/market', maxAge: 120, sharedMaxAge: 600 },
  ];

  for (const { path, maxAge, sharedMaxAge } of publicPages) {
    const response = await request.get(path);
    const cacheControl = response.headers()['cache-control'];
    expect(cacheControl).toContain(`max-age=${maxAge}`);
    expect(cacheControl).toContain(`s-maxage=${sharedMaxAge}`);
    expect(cacheControl).toContain('stale-while-revalidate=3600');

    await page.goto(path);
    await expect(page.locator('script')).toHaveCount(0);
  }

  const detailResponse = await request.get(
    '/posts/10000000-0000-4000-8000-000000000001',
  );
  expect(detailResponse.headers()['cache-control']).toBe('no-store');

  const missingResponse = await request.get('/posts/not-a-guid');
  expect(missingResponse.status()).toBe(404);
  expect(missingResponse.headers()['cache-control']).toBe('no-store');
});

test('Explore remains usable on a mobile viewport', async ({ page }) => {
  await page.goto('/explore');

  await expect(page.getByRole('combobox', { name: 'Category', exact: true })).toBeVisible();
  await expect(page.getByRole('combobox', { name: 'Tag', exact: true })).toBeVisible();
  await expect(page.getByRole('combobox', { name: 'Sort', exact: true })).toBeVisible();

  const hasHorizontalOverflow = await page.evaluate(
    () => document.documentElement.scrollWidth > document.documentElement.clientWidth,
  );
  expect(hasHorizontalOverflow).toBe(false);
});
