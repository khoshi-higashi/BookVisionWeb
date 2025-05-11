using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;
using BookVisionWeb.Domain;
using BookVisionWeb.UseCase;

namespace BookVisionWeb.Interface;

public static class Endpoints
{
    public static WebApplication MapOcr(this WebApplication app)
    {
        // --- Upload a page image and register it ---
        app.MapPost("/api/pages", async (IFormFile file, IPageRepository repo) =>
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest("File is missing.");

            var pageId = new PageId(Guid.NewGuid());
            var ext = Path.GetExtension(file.FileName) ?? ".jpg";
            var filePath = Path.Combine(Path.GetTempPath(), $"{pageId.Value}{ext}");

            await using (var stream = File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            await repo.SaveAsync(new Page(pageId, filePath));

            return Results.Ok(new { pageId = pageId.Value });
        }).DisableAntiforgery();

        // --- Run OCR on a registered page ---
        app.MapPost("/api/pages/{pageId:guid}/ocr",
            async (Guid pageId,
                   IRecognizePageUseCase uc,
                   RecognizePageJsonPresenter presenter) =>
            {
                await uc.HandleAsync(new(new PageId(pageId)), presenter);
                return Results.Ok(presenter.View);
            }).DisableAntiforgery();

        return app;
    }
}
