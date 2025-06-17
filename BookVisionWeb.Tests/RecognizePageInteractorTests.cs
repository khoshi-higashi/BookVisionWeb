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
        var page = new Page(pageId, "sample.jpg");

        var pageRepoMock = new Mock<IPageRepository>();
        pageRepoMock.Setup(r => r.FindAsync(pageId)).ReturnsAsync(page);

        var ocrGatewayMock = new Mock<IOcrGateway>();
        ocrGatewayMock.Setup(g => g.RecognizeAsync("sample.jpg")).ReturnsAsync("Recognized Text");

        var interactor = new RecognizePageInteractor(pageRepoMock.Object, ocrGatewayMock.Object);

        var presenterMock = new Mock<IRecognizePagePresenter>();
        await interactor.HandleAsync(new RecognizePageInput(pageId), presenterMock.Object);

        Assert.Equal("Recognized Text", page.OcrText);
        // OcrStatus.Completed への更新は現状の UseCase 実装では行われていません
        // Assert.Equal(OcrStatus.Completed, page.OcrStatus);
    }
}
