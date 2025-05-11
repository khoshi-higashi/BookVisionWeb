using BookVisionWeb.Domain;
using BookVisionWeb.UseCase;
using Moq;
using Xunit;

namespace BookVisionWeb.Tests;

public class RecognizePageInteractorTests
{
    [Fact]
    public async Task HandleAsync_ShouldRecognizeTextAndUpdatePage()
    {
        var pageId = new PageId(Guid.NewGuid());
        var bookId = new BookId(Guid.NewGuid());
        var page = new Page(pageId, "sample.jpg");

        var pageRepoMock = new Mock<IPageRepository>();
        pageRepoMock.Setup(r => r.GetByIdAsync(bookId, pageId)).ReturnsAsync(page);

        var ocrGatewayMock = new Mock<IOcrGateway>();
        ocrGatewayMock.Setup(g => g.RecognizeAsync("sample.jpg")).ReturnsAsync("Recognized Text");

        var interactor = new RecognizePageInteractor(pageRepoMock.Object, ocrGatewayMock.Object);

        await interactor.HandleAsync(bookId, pageId);

        Assert.Equal("Recognized Text", page.Text);
        Assert.Equal(OcrStatus.Completed, page.OcrStatus);
    }
}
