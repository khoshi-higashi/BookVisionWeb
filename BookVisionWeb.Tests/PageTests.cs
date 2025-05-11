using BookVisionWeb.Domain;
using Xunit;

namespace BookVisionWeb.Tests;

public class PageTests
{
    [Fact]
    public void SetOcrStatus_UpdatesStatusCorrectly()
    {
        var page = new Page(new PageId(Guid.NewGuid()), "sample.jpg");

        page.SetOcrStatus(OcrStatus.Completed);

        Assert.Equal(OcrStatus.Completed, page.OcrStatus);
    }

    [Fact]
    public void SetText_UpdatesTextCorrectly()
    {
        var page = new Page(new PageId(Guid.NewGuid()), "sample.jpg");

        page.SetText("example text");

        Assert.Equal("example text", page.Text);
    }
}
