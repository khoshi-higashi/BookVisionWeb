using BookVisionWeb.Infrastructure;
using Xunit;
using System.IO;

namespace BookVisionWeb.Tests;

public class OcrGatewayTests
{
    [Fact(Skip = "Integration test - requires actual OCR engine and image file")]
    public async Task RecognizeAsync_ReturnsRecognizedText()
    {
        var gateway = new TesseractGateway();
        var imagePath = "./sample.jpg"; // 画像ファイルを事前に配置しておく必要あり

        var result = await gateway.RecognizeAsync(imagePath);

        Assert.False(string.IsNullOrWhiteSpace(result));
    }
}
