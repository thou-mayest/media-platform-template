using Catalog.Domain;

namespace CleanModular.UnitTests.Catalog;

public class CatalogDomainTests
{
    [Fact]
    public void Category_Should_Update_Name_And_Slug()
    {
        var category = new Category("Initial", "initial");
        Assert.Equal("Initial", category.Name);
        Assert.Equal("initial", category.Slug);

        category.Update("Updated Name", "updated-name");

        Assert.Equal("Updated Name", category.Name);
        Assert.Equal("updated-name", category.Slug);
        Assert.NotNull(category.UpdateDate);
    }

    [Fact]
    public void Tag_Should_Update_Name_And_Slug()
    {
        var tag = new Tag("Photography", "photography");
        Assert.Equal("Photography", tag.Name);
        Assert.Equal("photography", tag.Slug);

        tag.Update("Photo Art", "photo-art");

        Assert.Equal("Photo Art", tag.Name);
        Assert.Equal("photo-art", tag.Slug);
        Assert.NotNull(tag.UpdateDate);
    }

    [Fact]
    public void Video_Should_ChangeCategory_And_ManageTags()
    {
        var creatorId = Guid.NewGuid();
        var initialCategoryId = Guid.NewGuid();
        var newCategoryId = Guid.NewGuid();
        var tagId1 = Guid.NewGuid();
        var tagId2 = Guid.NewGuid();

        var video = new Video(
            creatorId,
            initialCategoryId,
            "Video 1",
            "video-1",
            2024,
            "Oil on Canvas",
            "100x100",
            1.0,
            "#000000",
            1);

        Assert.Equal(initialCategoryId, video.CategoryId);

        video.ChangeCategory(newCategoryId);
        Assert.Equal(newCategoryId, video.CategoryId);

        video.AddTag(tagId1);
        video.AddTag(tagId2);
        // Adding duplicate should be ignored
        video.AddTag(tagId1);

        Assert.Equal(2, video.VideoTags.Count);

        video.RemoveTag(tagId1);
        Assert.Single(video.VideoTags);
        Assert.Equal(tagId2, video.VideoTags.First().TagId);
    }
}

