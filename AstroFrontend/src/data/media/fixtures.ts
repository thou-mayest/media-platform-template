import type { RawActor, RawAlbum, RawPost } from './types';

// Dates are anchored so that roughly a quarter of each creator's library falls
// inside NEW_BADGE_DAYS, matching the badge density in the design. An album's
// updatedAt equals its publishedAt — the mock has no edit history — and posts
// always sit on or after their album's date.
export const rawActors: RawActor[] = [
  ['Mara Solano', 'Photographer & Filmmaker', 'Saturated, sun-drunk images of coastal life — analog portraits, night markets and long-exposure seascapes shot across the Mediterranean and West Africa.', 38200, '2026-07-28'],
  ['Ivo Prieto', 'Photographer', 'Documents harbour towns and the people who work them, almost entirely on expired film stock.', 12400, '2026-07-02'],
  ['Lena Aoki', 'Filmmaker', 'Short-form documentary work about repetition, craft and the hands that make things.', 9100, '2026-06-21'],
  ['Tomas Réti', 'Photographer', 'Architectural studies of postwar concrete, photographed only in flat winter light.', 15600, '2026-07-13'],
  ['Nadia Belkacem', 'Visual artist', 'Builds collages from found photographs and textile fragments, then rephotographs them.', 7300, '2026-06-09'],
  ['Kwame Osei', 'Photographer', 'Portrait work rooted in Accra — studio backdrops, tailored cloth, direct gaze.', 41800, '2026-08-03'],
  ['Sol Marchetti', 'Filmmaker', 'Slow, static-camera studies of landscapes at the moment weather turns.', 5200, '2026-07-05'],
];

// coverAspectRatio: 1.5 = 3:2 · 0.667 = 2:3 · 0.8 = 4:5 · 1.778 = 16:9 · 1 = square
export const rawAlbums: RawAlbum[] = [
  // ── Mara Solano — 14 albums ───────────────────────────────────
  [0, 'Coastal Studies', 'Sun-drenched shoreline studies shot on medium-format film.', 'Bleached wooden groyne standing in shallow turquoise water at midday', 1.5, 9, 0, false, 'Coast,Analog,Seascape', '2026-06-19', '2026-06-19'],
  [0, 'Night Market', 'Paper-lantern portraits and street scenes from Marrakech after dark.', 'Spice seller arranging cones of saffron under hanging paper lanterns', 1.5, 11, 3, true, 'Marrakech,Night Photography,Street,Portraits,Morocco', '2025-11-02', '2025-11-02'],
  [0, 'Analog Portraits', 'Grainy 35mm portraits in natural light only — no strobes.', 'Woman turned three-quarters to a window, grain visible across her cheek', 0.667, 18, 0, false, 'Portraits,Analog,Natural Light', '2025-06-19', '2025-06-19'],
  [0, 'Long-Exposure Sea', 'Ten-minute exposures of the Atlantic at first light.', 'Atlantic swell blurred to smooth grey fog against a black basalt shelf', 1.778, 2, 4, true, 'Seascape,Long Exposure,Atlantic', '2026-07-11', '2026-07-11'],
  [0, 'Golden Hour', 'The last twenty minutes of daylight, chased for a month.', 'Low sun raking across a dry hillside, long shadows from single trees', 1.5, 27, 0, false, 'Golden Hour,Landscape', '2024-08-08', '2024-08-08'],
  [0, 'Studio Sessions', 'Controlled-light figure and still-life work from the loft.', 'Single softbox lighting a draped figure against seamless grey paper', 0.8, 15, 0, false, 'Studio,Still Life,Figure', '2025-02-27', '2025-02-27'],
  [0, 'Street After Rain', 'Neon reflections and wet asphalt across three cities.', 'Red and green neon signage mirrored in a puddle on black asphalt', 1.5, 22, 0, false, 'Street,Night Photography,Neon', '2026-03-15', '2026-03-15'],
  [0, 'Desert Light', 'Hard shadows, dust and long horizons in the Sahara.', 'Sharp-edged dune shadow cutting diagonally across rippled orange sand', 1.778, 19, 0, false, 'Desert,Landscape,Sahara', '2024-05-21', '2024-05-21'],
  [0, 'Harbour Nights', 'Long-lens studies of working harbours after midnight.', 'Crane silhouetted against sodium floodlights above a container quay', 1.5, 13, 3, true, 'Harbour,Night Photography,Documentary', '2026-02-08', '2026-02-08'],
  [0, 'Salt & Film', 'Cyanotype and salt-print darkroom experiments.', 'Prussian-blue cyanotype of fern fronds with visible brush edges', 1, 20, 0, false, 'Darkroom,Cyanotype,Analog', '2026-05-30', '2026-05-30'],
  [0, 'Ferry Crossings', 'Deck-level frames from eleven Mediterranean ferry routes.', 'Wet steel deck rail with a coastline receding into haze beyond', 1.5, 16, 2, false, 'Ferry,Mediterranean,Travel', '2026-04-22', '2026-04-22'],
  [0, 'Market Hands', 'Close studies of hands at work across six market towns.', 'Weathered hands tying twine around a bundle of green herbs', 0.8, 21, 0, false, 'Documentary,Portraits,Market', '2024-10-17', '2024-10-17'],
  [0, 'Rain Season', 'Six weeks photographing the same street through monsoon.', 'Figure under a translucent umbrella crossing a flooded side street', 0.667, 14, 5, true, 'Rain,Street,Documentary', '2025-09-23', '2025-09-23'],
  [0, 'First Light', 'Everything shot in the ninety minutes after sunrise.', 'Mist lifting off still water as the first sun strikes a far ridge', 1.778, 25, 0, false, 'Sunrise,Landscape,Coast', '2026-07-28', '2026-07-28'],

  // ── Suggested actors — so their profiles are never empty ──────
  [1, 'Dockside', 'Working quays photographed on expired Portra.', 'Rust-streaked bollard with mooring rope, colours shifted magenta', 1.5, 18, 0, false, 'Harbour,Analog,Documentary', '2026-07-02', '2026-07-02'],
  [1, 'Trawler Crews', 'Portraits made between shifts on the north coast.', 'Deckhand in oilskins leaning on a winch, cigarette unlit', 0.667, 14, 2, false, 'Portraits,Documentary,Coast', '2025-03-30', '2025-03-30'],
  [1, 'Low Tide', 'What the water leaves behind, catalogued weekly.', 'Grid of shells and rope fragments arranged on wet grey sand', 1, 22, 0, false, 'Coast,Still Life', '2024-09-12', '2024-09-12'],

  [2, 'The Bindery', 'A bookbinder repeats one motion for eleven minutes.', 'Hands drawing waxed thread through folded signatures in a sewing frame', 1.778, 4, 9, true, 'Craft,Documentary,Hands', '2026-06-21', '2026-06-21'],
  [2, 'Kiln Days', 'Three firings, filmed end to end without a cut.', 'Orange glow through a kiln spyhole in an otherwise dark workshop', 1.778, 6, 7, true, 'Craft,Ceramics,Documentary', '2025-06-05', '2025-06-05'],
  [2, 'Loom', 'Static camera on a single weaver across one season.', 'Warp threads under tension on a floor loom, shuttle mid-pass', 1.5, 3, 5, false, 'Craft,Textile,Documentary', '2024-04-18', '2024-04-18'],

  [3, 'Panel Housing', 'Prefab facades in flat January light.', 'Repeating concrete panel facade with identical balconies, no sky', 0.8, 31, 0, false, 'Architecture,Brutalism,Monochrome', '2026-07-13', '2026-07-13'],
  [3, 'Stairwells', 'Forty concrete stairwells, photographed head-on.', 'Cast-concrete stair flight photographed square-on with no horizon', 0.667, 26, 0, true, 'Architecture,Geometric', '2025-12-07', '2025-12-07'],
  [3, 'Civic Concrete', 'Municipal buildings nobody photographs on purpose.', 'Board-marked concrete civic hall under an even white winter sky', 1.5, 19, 0, false, 'Architecture,Brutalism', '2024-02-25', '2024-02-25'],

  [4, 'Cut Paper I', 'Found photographs cut and rephotographed against cloth.', 'Torn photographic fragments overlapping on indigo woven cloth', 1, 17, 0, false, 'Collage,Mixed Media', '2026-06-09', '2026-06-09'],
  [4, 'Inherited Cloth', 'Family textiles scanned at very high resolution.', 'Close scan of worn embroidery showing broken stitches and fibre', 1, 12, 0, false, 'Textile,Archive', '2025-08-16', '2025-08-16'],
  [4, 'Second Print', 'Every image printed, damaged, then printed again.', 'Creased and abraded photographic print rephotographed flat', 0.8, 15, 0, false, 'Collage,Process', '2024-11-29', '2024-11-29'],

  [5, 'Accra Studio', 'Backdrop portraits made over two years in one room.', 'Seated portrait against a hand-painted blue studio backdrop', 0.8, 34, 0, true, 'Portraits,Studio,Accra', '2026-08-03', '2026-08-03'],
  [5, 'Sunday Best', 'Tailored cloth photographed the morning it was finished.', 'Freshly pressed patterned jacket on a tailor dummy in daylight', 0.667, 28, 0, false, 'Portraits,Textile,Accra', '2025-05-20', '2025-05-20'],
  [5, 'Direct Gaze', 'Fifty portraits, all eye-level, all looking back.', 'Eye-level head-and-shoulders portrait meeting the camera directly', 1, 22, 0, true, 'Portraits,Studio', '2024-07-14', '2024-07-14'],

  [6, 'Weather Turns', 'Static frames held until the light changes.', 'Flat moorland under a dark front advancing from the left edge', 1.778, 5, 8, true, 'Landscape,Weather,Slow Cinema', '2026-07-05', '2026-07-05'],
  [6, 'Valley Fog', 'One valley, filmed at dawn for thirty consecutive days.', 'Fog filling a valley floor with ridgelines emerging above it', 1.778, 4, 6, false, 'Landscape,Weather', '2025-10-06', '2025-10-06'],
  [6, 'Before Snow', 'The hour before the first snowfall, three winters running.', 'Bare birches against a heavy yellow-grey pre-snow sky', 1.5, 3, 4, false, 'Landscape,Winter,Slow Cinema', '2024-12-19', '2024-12-19'],
];

// Posts exist for three albums only. Authoring believable alt text for every
// item across 32 albums is not feasible, and generated alt text is exactly the
// filler the SEO brief rules out. The remaining albums render an empty state,
// which is a real production case anyway — a freshly created album.
export const rawPosts: RawPost[] = [
  // ── album 0 · Coastal Studies (9 photos) — published 2026-06-19
  [0, 'photo', 1.5, null, 'Bleached wooden groyne standing in shallow turquoise water at midday', 'Groyne no. 4, low tide', '2026-06-19'],
  [0, 'photo', 0.667, null, 'Swimmer mid-stride entering the sea, spray caught at knee height', null, '2026-06-19'],
  [0, 'photo', 1.5, null, 'Empty striped deckchairs facing an overexposed white horizon', null, '2026-06-20'],
  [0, 'photo', 0.8, null, 'Salt crust drying in cracked patterns on dark harbour stone', 'Salt, four days without rain', '2026-06-20'],
  [0, 'photo', 1.778, null, 'Long breakwater receding to a distant lighthouse under flat cloud', null, '2026-06-22'],
  [0, 'photo', 1, null, 'Coiled blue mooring rope on a sun-bleached wooden bollard', null, '2026-06-23'],
  [0, 'photo', 0.667, null, 'Child in a red swimsuit silhouetted against bright shallow water', null, '2026-06-24'],
  [0, 'photo', 1.5, null, 'Fishing skiff pulled up on shingle with nets folded across the bow', 'Borrowed for the afternoon', '2026-06-26'],
  [0, 'photo', 0.8, null, 'Sunburnt shoulder and a strap mark, shot close in hard midday light', null, '2026-06-27'],

  // ── album 1 · Night Market (11 photos, 3 videos) — published 2025-11-02
  [1, 'photo', 1.5, null, 'Spice seller arranging cones of saffron and paprika under paper lanterns', 'The stall that started the series', '2025-11-02'],
  [1, 'photo', 0.8, null, 'Portrait of a tea vendor lit only by the burner in front of him', null, '2025-11-02'],
  [1, 'video', 1.778, 47, 'Slow pan across a crowded food alley at dusk as the lanterns come on', 'Forty seconds, no cuts', '2025-11-03'],
  [1, 'photo', 0.667, null, 'Hands tearing flatbread over a steel counter, motion blur on the fingers', null, '2025-11-03'],
  [1, 'photo', 1.5, null, 'Neon reflections broken across a puddle after the evening rain', null, '2025-11-04'],
  [1, 'photo', 1, null, 'Stacked brass lamps catching light from a single overhead bulb', null, '2025-11-04'],
  [1, 'video', 1.778, 112, 'Handheld walk past the grilled-corn carts, ambient sound only', null, '2025-11-05'],
  [1, 'photo', 0.8, null, 'Woman counting coins at a fabric stall, face half in shadow', null, '2025-11-06'],
  [1, 'photo', 1.5, null, 'Overhead canopy of hanging lanterns receding down a narrow lane', 'Shot standing on a crate', '2025-11-07'],
  [1, 'photo', 0.667, null, 'Butcher’s scale swinging slightly, lit from below by a bare filament', null, '2025-11-08'],
  [1, 'video', 0.5625, 28, 'Vertical clip of a mint tea pour from arm’s height into a glass', null, '2025-11-09'],
  [1, 'photo', 1.778, null, 'Wide view of the square at full dark, smoke lit from beneath by stalls', null, '2025-11-10'],
  [1, 'photo', 1, null, 'Close crop of dried rose petals in a shallow woven basket', null, '2025-11-11'],
  [1, 'photo', 0.8, null, 'Boy asleep against a stack of folded rugs as the market packs up', 'Last frame of the trip', '2025-11-12'],

  // ── album 3 · Long-Exposure Sea (2 photos, 4 videos) — published 2026-07-11
  [3, 'photo', 1.778, null, 'Atlantic swell blurred to smooth grey fog against a black basalt shelf', 'Ten minutes at f/16', '2026-07-11'],
  [3, 'video', 1.778, 180, 'Static three-minute frame of the tide crossing a rock shelf at dawn', null, '2026-07-11'],
  [3, 'photo', 1.5, null, 'Half-submerged boulder softened by a long exposure at first light', null, '2026-07-12'],
  [3, 'video', 1.778, 95, 'Cloud shadow moving across open water, camera locked off', null, '2026-07-13'],
  [3, 'video', 1.778, 140, 'Incoming set breaking over the shelf, filmed from above the waterline', null, '2026-07-14'],
  [3, 'video', 0.5625, 62, 'Vertical study of water draining off rock between waves', null, '2026-07-15'],
];
