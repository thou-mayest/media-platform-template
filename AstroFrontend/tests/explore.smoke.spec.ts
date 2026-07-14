import { expect, test } from '@playwright/test';

test('home page links to the Explore experience', async ({ page }) => {
  await page.goto('/');

  await expect(page.getByRole('heading', { name: 'An index of contemporary visual artists.' })).toBeVisible();

  const primaryNavigation = page.getByRole('navigation', { name: 'Primary navigation' });
  await primaryNavigation.getByRole('link', { name: 'Explore' }).click();

  await expect(page).toHaveURL(/\/explore$/);
  await expect(page.getByRole('heading', { name: 'Explore works.' })).toBeVisible();
});

test('search filters works and persists its state in the URL', async ({ page }) => {
  await page.goto('/explore');

  await expect(page.getByText('36 works', { exact: true })).toBeVisible();
  await page.getByRole('searchbox', { name: 'Search works' }).fill('Mira Okafor');

  await expect(page.getByText('4 works', { exact: true })).toBeVisible();
  await expect(page).toHaveURL(/q=Mira(?:\+|%20)Okafor/);
});

test('category, tag and sort controls update the result set', async ({ page }) => {
  await page.goto('/explore');

  await page.getByRole('button', { name: 'Painting 11' }).click();
  await expect(page.getByText('11 works', { exact: true })).toBeVisible();
  await expect(page).toHaveURL(/category=painting/);

  await page.getByRole('button', { name: 'All works' }).click();
  await page.getByLabel('Tag').selectOption('portrait');
  await expect(page.getByText('2 works', { exact: true })).toBeVisible();

  await page.getByRole('button', { name: 'Clear filters' }).click();
  await page.getByLabel('Sort').selectOption('title');

  const firstVisibleWork = page.locator('[data-explore-item]:visible').first();
  await expect(firstVisibleWork.getByRole('link', { name: 'Balance no.7', exact: true }).first()).toBeVisible();
});

test('empty results can be reset', async ({ page }) => {
  await page.goto('/explore');

  await page.getByRole('searchbox', { name: 'Search works' }).fill('not-a-real-work');
  await expect(page.getByText('No works match those filters.')).toBeVisible();

  await page.getByRole('button', { name: 'Reset explore' }).click();
  await expect(page.getByText('36 works', { exact: true })).toBeVisible();
});

test('Explore remains usable on a mobile viewport', async ({ page }) => {
  await page.goto('/explore');

  await expect(page.getByRole('searchbox', { name: 'Search works' })).toBeVisible();
  await expect(page.getByLabel('Tag')).toBeVisible();
  await expect(page.getByLabel('Sort')).toBeVisible();

  const hasHorizontalOverflow = await page.evaluate(
    () => document.documentElement.scrollWidth > document.documentElement.clientWidth,
  );
  expect(hasHorizontalOverflow).toBe(false);
});
