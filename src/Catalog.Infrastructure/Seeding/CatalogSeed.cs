using System.Globalization;
using System.Text;
using Catalog.Domain;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Seeding;

public static class CatalogSeed
{
    private static readonly Dictionary<string, string> Palette = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ink"] = "#221F1A",
        ["char"] = "#2B2A28",
        ["oat"] = "#E7DEC9",
        ["clay"] = "#B5563A",
        ["ochre"] = "#C99A3F",
        ["sage"] = "#8C9A78",
        ["slate"] = "#586A72",
        ["plum"] = "#6E4B5E",
        ["sand"] = "#D9C7A6",
        ["teal"] = "#356A64",
        ["oxblood"] = "#7A2E2A",
        ["sky"] = "#A9C2CC",
        ["rose"] = "#C98A7E",
        ["forest"] = "#2F4538",
        ["mustard"] = "#D4A82C",
        ["stone"] = "#9B9384",
        ["cobalt"] = "#33527A",
        ["blush"] = "#E8D5CC",
        ["umber"] = "#4A3B2E",
        ["bone"] = "#F0E9DA",
        ["noir"] = "#1A1714"
    };

    private static readonly double[] AspectRatios = [0.8, 1.3, 1, 0.78, 1.5, 0.92];

    private static readonly RawCreator[] RawCreators =
    [
        new("Mira Okafor", "Lagos, Nigeria", "Painting · Collage", "Builds dense, layered surfaces where torn paper and pigment compete for the same ground.", "clay", "2014"),
        new("Johan Vesterberg", "Stockholm, Sweden", "Photography", "Photographs northern light and stone with a patience that borders on devotion.", "slate", "2009"),
        new("Renata Salgado", "São Paulo, Brazil", "Sculpture · Installation", "Makes weight look weightless - balancing bronze, concrete and colour against gravity.", "stone", "2012"),
        new("Aiko Tanaka", "Kyoto, Japan", "Printmaking · Drawing", "Cuts woodblocks of rain, tide and line - quiet repetitions that accumulate into weather.", "sky", "2016"),
        new("Daniel Mercer", "Glasgow, Scotland", "Painting", "Paints the city after dark: lit windows, wet streets, the interior life of buildings.", "umber", "2008"),
        new("Yara Haddad", "Beirut, Lebanon", "Textile · Mixed Media", "Dyes and stitches maps of memory and home into cotton, wool and indigo.", "oxblood", "2015"),
        new("Tomas Brandt", "Berlin, Germany", "Digital · Generative", "Writes systems that draw - fields and lattices where code behaves like pigment.", "cobalt", "2018"),
        new("Lucia Ferraro", "Naples, Italy", "Photography · Painting", "Moves between lens and brush, chasing the same Mediterranean light through both.", "rose", "2011"),
        new("Noah Adeyemi", "Brooklyn, USA", "Drawing · Painting", "Draws the figure with charcoal and acrylic - portraits of presence and congregation.", "char", "2017")
    ];

    private static readonly RawVideo[] RawVideos =
    [
        new(0, "Market at Noon", 2024, "Painting", "Oil and paper on linen", "Abstract,Color Field", "glow", "clay,ochre"),
        new(0, "Untitled (Red Ground)", 2023, "Painting", "Oil on canvas", "Abstract,Color Field,Minimal", "field", "oxblood"),
        new(0, "Crossings", 2024, "Collage", "Mixed media on board", "Geometric,Abstract", "bars", "clay,oat"),
        new(0, "Harmattan", 2022, "Painting", "Acrylic on canvas", "Landscape,Abstract", "horizon", "ochre,sand,clay"),
        new(1, "North Light I", 2023, "Photography", "Archival pigment print", "Landscape,Minimal,Monochrome", "horizon", "slate,sky,bone"),
        new(1, "Quarry", 2022, "Photography", "Silver gelatin print", "Monochrome,Minimal", "duo", "char,stone"),
        new(1, "Field Study", 2024, "Photography", "Archival pigment print", "Landscape,Minimal", "horizon", "sage,bone,stone"),
        new(1, "Ice Sheet", 2023, "Photography", "Archival pigment print", "Minimal,Monochrome", "field", "sky"),
        new(2, "Balance no.7", 2024, "Sculpture", "Bronze and steel", "Geometric,Abstract", "split", "char,sand"),
        new(2, "Soft Architecture", 2023, "Sculpture", "Cast concrete", "Minimal,Geometric", "field", "stone"),
        new(2, "Counterweight", 2022, "Sculpture", "Carrara marble", "Minimal,Organic", "glow", "bone,stone"),
        new(2, "Tropico", 2024, "Installation", "Steel and pigment", "Color Field,Geometric", "corner", "forest,mustard"),
        new(3, "Rainband", 2024, "Printmaking", "Woodblock print", "Minimal,Landscape", "vbars", "sky,bone"),
        new(3, "Hundred Lines", 2023, "Drawing", "Ink on paper", "Minimal,Monochrome,Geometric", "vbars", "char,bone"),
        new(3, "Plum Rain", 2022, "Printmaking", "Woodblock print", "Organic,Minimal", "glow", "plum,blush"),
        new(3, "Tide Table", 2024, "Printmaking", "Screenprint", "Geometric,Color Field", "bars", "teal,oat"),
        new(4, "Tenement Window", 2023, "Painting", "Oil on board", "Figurative,Still Life", "duo", "slate,umber"),
        new(4, "Clyde", 2024, "Painting", "Oil on canvas", "Landscape,Abstract", "horizon", "slate,stone,char"),
        new(4, "Interior with Lamp", 2022, "Painting", "Oil on canvas", "Figurative,Still Life", "glow", "umber,mustard"),
        new(4, "Nocturne", 2024, "Painting", "Oil on linen", "Abstract,Color Field,Monochrome", "field", "noir"),
        new(5, "Threshold", 2024, "Textile", "Hand-dyed cotton", "Geometric,Color Field", "bars", "oxblood,sand"),
        new(5, "Mending I", 2023, "Textile", "Wool and linen", "Organic,Abstract", "corner", "rose,oat"),
        new(5, "Cartography", 2022, "Mixed Media", "Thread on canvas", "Abstract,Geometric", "split", "teal,sand"),
        new(5, "Domestic Map", 2024, "Textile", "Cotton and indigo", "Geometric,Color Field,Monochrome", "vbars", "cobalt,bone"),
        new(6, "Field 0x9", 2024, "Digital", "Generative print", "Abstract,Geometric,Color Field", "corner", "cobalt,sky"),
        new(6, "Drift", 2023, "Digital", "Generative still", "Abstract,Minimal", "glow", "plum,cobalt"),
        new(6, "Lattice", 2024, "Digital", "Generative print", "Geometric,Minimal", "bars", "char,slate"),
        new(6, "Solar", 2023, "Digital", "Generative print", "Color Field,Abstract", "sun", "ink,mustard"),
        new(7, "Bay, Evening", 2024, "Photography", "Archival pigment print", "Landscape,Color Field", "horizon", "cobalt,rose,sand"),
        new(7, "Still Life with Lemons", 2023, "Painting", "Oil on panel", "Still Life,Figurative", "glow", "mustard,forest"),
        new(7, "Vesuvio", 2022, "Photography", "Archival pigment print", "Landscape,Minimal", "horizon", "plum,rose,stone"),
        new(7, "Window, Posillipo", 2024, "Painting", "Oil on canvas", "Figurative,Color Field", "split", "sky,blush"),
        new(8, "Sitter I", 2024, "Drawing", "Charcoal on paper", "Portrait,Figurative,Monochrome", "glow", "char,stone"),
        new(8, "Two Figures", 2023, "Painting", "Acrylic on canvas", "Figurative,Color Field", "split", "clay,teal"),
        new(8, "Self, Morning", 2022, "Drawing", "Graphite on paper", "Portrait,Monochrome,Minimal", "field", "stone"),
        new(8, "Congregation", 2024, "Painting", "Acrylic on canvas", "Figurative,Abstract", "corner", "oxblood,ochre")
    ];

    public static async Task SeedAsync(CatalogDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Creators.AnyAsync(cancellationToken))
        {
            var hasChanges = false;
            if (!await dbContext.Categories.AnyAsync(c => c.Slug == Category.DefaultSlug, cancellationToken))
            {
                dbContext.Categories.Add(new Category(Category.DefaultName, Category.DefaultSlug));
                hasChanges = true;
            }
            if (!await dbContext.Tags.AnyAsync(t => t.Slug == Tag.DefaultSlug, cancellationToken))
            {
                dbContext.Tags.Add(new Tag(Tag.DefaultName, Tag.DefaultSlug));
                hasChanges = true;
            }
            if (hasChanges)
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            return;
        }

        var defaultCategory = new Category(Category.DefaultName, Category.DefaultSlug);
        dbContext.Categories.Add(defaultCategory);

        var defaultTag = new Tag(Tag.DefaultName, Tag.DefaultSlug);
        dbContext.Tags.Add(defaultTag);

        var creators = RawCreators
            .Select(rawCreator => new Creator(
                rawCreator.Name,
                Slugify(rawCreator.Name),
                rawCreator.Location,
                rawCreator.Disciplines,
                rawCreator.Bio,
                Palette[rawCreator.AccentKey],
                string.Concat(rawCreator.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(part => part[0])),
                rawCreator.Since))
            .ToList();

        var categories = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);
        var tags = new Dictionary<string, Tag>(StringComparer.OrdinalIgnoreCase);
        var videos = new List<Video>();
        var links = new List<VideoTag>();

        for (var i = 0; i < RawVideos.Length; i++)
        {
            var rawVideo = RawVideos[i];
            var creator = creators[rawVideo.CreatorIndex];

            if (!categories.TryGetValue(rawVideo.Category, out var category))
            {
                category = new Category(rawVideo.Category, Slugify(rawVideo.Category));
                categories.Add(rawVideo.Category, category);
            }

            var colors = rawVideo.Colors
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(colorKey => Palette[colorKey])
                .ToArray();

            var aspectRatio = AspectRatios[i % AspectRatios.Length];
            var video = new Video(
                creator.Id,
                category.Id,
                rawVideo.Title,
                $"{Slugify(rawVideo.Title)}-{i}",
                rawVideo.Year,
                rawVideo.Medium,
                GenerateSize(rawVideo.Category, aspectRatio, i),
                aspectRatio,
                GenerateBackground(rawVideo.Pattern, colors),
                i);

            videos.Add(video);

            foreach (var tagName in rawVideo.Tags
                         .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (!tags.TryGetValue(tagName, out var tag))
                {
                    tag = new Tag(tagName, Slugify(tagName));
                    tags.Add(tagName, tag);
                }

                links.Add(new VideoTag(video.Id, tag.Id));
            }
        }

        await dbContext.Creators.AddRangeAsync(creators, cancellationToken);
        await dbContext.Categories.AddRangeAsync(categories.Values, cancellationToken);
        await dbContext.Tags.AddRangeAsync(tags.Values, cancellationToken);
        await dbContext.Videos.AddRangeAsync(videos, cancellationToken);
        await dbContext.VideoTags.AddRangeAsync(links, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string GenerateBackground(string pattern, IReadOnlyList<string> colors)
    {
        var c0 = colors.ElementAtOrDefault(0) ?? Palette["stone"];
        var c1 = colors.ElementAtOrDefault(1) ?? c0;
        var c2 = colors.ElementAtOrDefault(2) ?? c1;

        return pattern switch
        {
            "field" => c0,
            "duo" => $"linear-gradient(150deg, {c0}, {c1})",
            "split" => $"linear-gradient(100deg, {c0} 0 50%, {c1} 50% 100%)",
            "horizon" => $"linear-gradient({c0} 0 40%, {c1} 40% 64%, {c2} 64% 100%)",
            "sun" => $"radial-gradient(circle at 50% 62%, {c1} 0 19%, {c0} 19%)",
            "glow" => $"radial-gradient(125% 92% at 28% 22%, {c1}, {c0})",
            "bars" => $"repeating-linear-gradient(90deg, {c0} 0 12%, {c1} 12% 24%)",
            "vbars" => $"repeating-linear-gradient(0deg, {c0} 0 8%, {c1} 8% 16%)",
            "corner" => $"conic-gradient(from 210deg at 78% 22%, {c1}, {c0}, {c1})",
            _ => c0
        };
    }

    private static string GenerateSize(string category, double aspectRatio, int index)
    {
        if (string.Equals(category, "Sculpture", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(category, "Installation", StringComparison.OrdinalIgnoreCase))
        {
            var heights = new[] { "h. 120 cm", "h. 86 cm", "h. 175 cm", "h. 54 cm" };
            return heights[index % heights.Length];
        }

        var baseHeights = new[] { 150, 90, 120, 60 };
        var height = baseHeights[index % baseHeights.Length];
        var width = (int)Math.Round(height * aspectRatio);
        return $"{width} × {height} cm";
    }

    private static string Slugify(string value)
    {
        var normalized = value.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
            else if (builder.Length > 0 && builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        return builder.ToString().Trim('-');
    }

    private sealed record RawCreator(
        string Name,
        string Location,
        string Disciplines,
        string Bio,
        string AccentKey,
        string Since);

    private sealed record RawVideo(
        int CreatorIndex,
        string Title,
        int Year,
        string Category,
        string Medium,
        string Tags,
        string Pattern,
        string Colors);
}
