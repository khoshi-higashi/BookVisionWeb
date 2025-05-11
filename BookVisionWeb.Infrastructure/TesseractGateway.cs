namespace BookVisionWeb.Infrastructure;

using BookVisionWeb.UseCase;
using System.Diagnostics;

public sealed class TesseractGateway : IOcrGateway
{
    public async Task<string> RecognizeAsync(string imagePath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "tesseract",
                Arguments = $"\"{imagePath}\" stdout -l eng",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (!string.IsNullOrWhiteSpace(error))
        {
            Console.Error.WriteLine($"Tesseract error: {error}");
        }

        return output.Trim();
    }
}
