import type { RawActor, RawAlbum } from './types';

export const rawActors: RawActor[] = [
  ['Mara Solano', 'Photographer & Filmmaker', 'Saturated, sun-drunk images of coastal life — analog portraits, night markets and long-exposure seascapes shot across the Mediterranean and West Africa.', 38200, '2026-07-28'],
  ['Ivo Prieto', 'Photographer', 'Documents harbour towns and the people who work them, almost entirely on expired film stock.', 12400, '2026-07-14'],
  ['Lena Aoki', 'Filmmaker', 'Short-form documentary work about repetition, craft and the hands that make things.', 9100, '2026-06-30'],
  ['Tomas Réti', 'Photographer', 'Architectural studies of postwar concrete, photographed only in flat winter light.', 15600, '2026-07-21'],
  ['Nadia Belkacem', 'Visual artist', 'Builds collages from found photographs and textile fragments, then rephotographs them.', 7300, '2026-06-18'],
  ['Kwame Osei', 'Photographer', 'Portrait work rooted in Accra — studio backdrops, tailored cloth, direct gaze.', 41800, '2026-08-01'],
  ['Sol Marchetti', 'Filmmaker', 'Slow, static-camera studies of landscapes at the moment weather turns.', 5200, '2026-05-22'],
];

// coverAspectRatio: 1.5 = 3:2 · 0.667 = 2:3 · 0.8 = 4:5 · 1.778 = 16:9 · 1 = square
export const rawAlbums: RawAlbum[] = [
  // ── Mara Solano — 14 albums ───────────────────────────────────
  [0, 'Coastal Studies', 'Sun-drenched shoreline studies shot on medium-format film.', 'Bleached wooden groyne standing in shallow turquoise water at midday', 1.5, 24, 0, false, 'Coast,Analog,Seascape', '2024-06-14', '2026-07-28'],
  [0, 'Night Market', 'Paper-lantern portraits and street scenes from Marrakech after dark.', 'Spice seller arranging cones of saffron under hanging paper lanterns', 1.5, 27, 4, true, 'Marrakech,Night Photography,Street,Portraits,Morocco', '2023-11-02', '2026-07-12'],
  [0, 'Analog Portraits', 'Grainy 35mm portraits in natural light only — no strobes.', 'Woman turned three-quarters to a window, grain visible across her cheek', 0.667, 18, 0, false, 'Portraits,Analog,Natural Light', '2023-04-19', '2026-05-30'],
  [0, 'Long-Exposure Sea', 'Ten-minute exposures of the Atlantic at first light.', 'Atlantic swell blurred to smooth grey fog against a black basalt shelf', 1.778, 8, 4, true, 'Seascape,Long Exposure,Atlantic', '2024-09-30', '2026-07-25'],
  [0, 'Golden Hour', 'The last twenty minutes of daylight, chased for a month.', 'Low sun raking across a dry hillside, long shadows from single trees', 1.5, 27, 0, false, 'Golden Hour,Landscape', '2022-08-08', '2026-03-11'],
  [0, 'Studio Sessions', 'Controlled-light figure and still-life work from the loft.', 'Single softbox lighting a draped figure against seamless grey paper', 0.8, 15, 0, false, 'Studio,Still Life,Figure', '2023-02-27', '2026-04-02'],
  [0, 'Street After Rain', 'Neon reflections and wet asphalt across three cities.', 'Red and green neon signage mirrored in a puddle on black asphalt', 1.5, 22, 0, false, 'Street,Night Photography,Neon', '2024-03-15', '2026-06-08'],
  [0, 'Desert Light', 'Hard shadows, dust and long horizons in the Sahara.', 'Sharp-edged dune shadow cutting diagonally across rippled orange sand', 1.778, 19, 0, false, 'Desert,Landscape,Sahara', '2022-05-21', '2026-02-19'],
  [0, 'Harbour Nights', 'Long-lens studies of working harbours after midnight.', 'Crane silhouetted against sodium floodlights above a container quay', 1.5, 13, 3, true, 'Harbour,Night Photography,Documentary', '2024-01-26', '2026-07-03'],
  [0, 'Salt & Film', 'Cyanotype and salt-print darkroom experiments.', 'Prussian-blue cyanotype of fern fronds with visible brush edges', 1, 20, 0, false, 'Darkroom,Cyanotype,Analog', '2023-07-11', '2026-06-27'],
  [0, 'Ferry Crossings', 'Deck-level frames from eleven Mediterranean ferry routes.', 'Wet steel deck rail with a coastline receding into haze beyond', 1.5, 16, 2, false, 'Ferry,Mediterranean,Travel', '2024-05-04', '2026-05-14'],
  [0, 'Market Hands', 'Close studies of hands at work across six market towns.', 'Weathered hands tying twine around a bundle of green herbs', 0.8, 21, 0, false, 'Documentary,Portraits,Market', '2022-10-17', '2026-01-30'],
  [0, 'Rain Season', 'Six weeks photographing the same street through monsoon.', 'Figure under a translucent umbrella crossing a flooded side street', 0.667, 14, 5, true, 'Rain,Street,Documentary', '2023-09-23', '2026-04-21'],
  [0, 'First Light', 'Everything shot in the ninety minutes after sunrise.', 'Mist lifting off still water as the first sun strikes a far ridge', 1.778, 25, 0, false, 'Sunrise,Landscape,Coast', '2024-11-08', '2026-07-19'],

  // ── Suggested actors — so their profiles are never empty ──────
  [1, 'Dockside', 'Working quays photographed on expired Portra.', 'Rust-streaked bollard with mooring rope, colours shifted magenta', 1.5, 18, 0, false, 'Harbour,Analog,Documentary', '2024-07-02', '2026-07-14'],
  [1, 'Trawler Crews', 'Portraits made between shifts on the north coast.', 'Deckhand in oilskins leaning on a winch, cigarette unlit', 0.667, 14, 2, false, 'Portraits,Documentary,Coast', '2023-03-30', '2026-05-09'],
  [1, 'Low Tide', 'What the water leaves behind, catalogued weekly.', 'Grid of shells and rope fragments arranged on wet grey sand', 1, 22, 0, false, 'Coast,Still Life', '2022-09-12', '2026-02-02'],

  [2, 'The Bindery', 'A bookbinder repeats one motion for eleven minutes.', 'Hands drawing waxed thread through folded signatures in a sewing frame', 1.778, 4, 9, true, 'Craft,Documentary,Hands', '2024-10-21', '2026-06-30'],
  [2, 'Kiln Days', 'Three firings, filmed end to end without a cut.', 'Orange glow through a kiln spyhole in an otherwise dark workshop', 1.778, 6, 7, true, 'Craft,Ceramics,Documentary', '2023-06-05', '2026-04-16'],
  [2, 'Loom', 'Static camera on a single weaver across one season.', 'Warp threads under tension on a floor loom, shuttle mid-pass', 1.5, 3, 5, false, 'Craft,Textile,Documentary', '2022-04-18', '2026-01-12'],

  [3, 'Panel Housing', 'Prefab facades in flat January light.', 'Repeating concrete panel facade with identical balconies, no sky', 0.8, 31, 0, false, 'Architecture,Brutalism,Monochrome', '2024-08-13', '2026-07-21'],
  [3, 'Stairwells', 'Forty concrete stairwells, photographed head-on.', 'Cast-concrete stair flight photographed square-on with no horizon', 0.667, 26, 0, true, 'Architecture,Geometric', '2023-12-07', '2026-05-27'],
  [3, 'Civic Concrete', 'Municipal buildings nobody photographs on purpose.', 'Board-marked concrete civic hall under an even white winter sky', 1.5, 19, 0, false, 'Architecture,Brutalism', '2022-02-25', '2026-03-05'],

  [4, 'Cut Paper I', 'Found photographs cut and rephotographed against cloth.', 'Torn photographic fragments overlapping on indigo woven cloth', 1, 17, 0, false, 'Collage,Mixed Media', '2024-04-09', '2026-06-18'],
  [4, 'Inherited Cloth', 'Family textiles scanned at very high resolution.', 'Close scan of worn embroidery showing broken stitches and fibre', 1, 12, 0, false, 'Textile,Archive', '2023-08-16', '2026-04-09'],
  [4, 'Second Print', 'Every image printed, damaged, then printed again.', 'Creased and abraded photographic print rephotographed flat', 0.8, 15, 0, false, 'Collage,Process', '2022-11-29', '2026-02-24'],

  [5, 'Accra Studio', 'Backdrop portraits made over two years in one room.', 'Seated portrait against a hand-painted blue studio backdrop', 0.8, 34, 0, true, 'Portraits,Studio,Accra', '2024-12-03', '2026-08-01'],
  [5, 'Sunday Best', 'Tailored cloth photographed the morning it was finished.', 'Freshly pressed patterned jacket on a tailor dummy in daylight', 0.667, 28, 0, false, 'Portraits,Textile,Accra', '2023-05-20', '2026-06-11'],
  [5, 'Direct Gaze', 'Fifty portraits, all eye-level, all looking back.', 'Eye-level head-and-shoulders portrait meeting the camera directly', 1, 22, 0, true, 'Portraits,Studio', '2022-07-14', '2026-03-28'],

  [6, 'Weather Turns', 'Static frames held until the light changes.', 'Flat moorland under a dark front advancing from the left edge', 1.778, 5, 8, true, 'Landscape,Weather,Slow Cinema', '2024-02-11', '2026-05-22'],
  [6, 'Valley Fog', 'One valley, filmed at dawn for thirty consecutive days.', 'Fog filling a valley floor with ridgelines emerging above it', 1.778, 4, 6, false, 'Landscape,Weather', '2023-10-06', '2026-03-17'],
  [6, 'Before Snow', 'The hour before the first snowfall, three winters running.', 'Bare birches against a heavy yellow-grey pre-snow sky', 1.5, 3, 4, false, 'Landscape,Winter,Slow Cinema', '2022-12-19', '2026-01-08'],
];