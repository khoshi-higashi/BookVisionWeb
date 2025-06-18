using BookVisionWeb.Domain;
using BookVisionWeb.Infrastructure;
using Xunit;
using System.Linq;

namespace BookVisionWeb.Tests;

public class InMemoryPageRepositoryTests
{
    [Fact]
    public async Task FindAllAsync_ReturnsAllSavedPages()
    {
        var repo = new InMemoryPageRepository();
        var page1 = new Page(new PageId(Guid.NewGuid()), "a.jpg");
        var page2 = new Page(new PageId(Guid.NewGuid()), "b.jpg");
        await repo.SaveAsync(page1);
        await repo.SaveAsync(page2);

        var pages = await repo.FindAllAsync();

        Assert.Equal(2, pages.Count());
        Assert.Contains(page1, pages);
        Assert.Contains(page2, pages);
    }
}
