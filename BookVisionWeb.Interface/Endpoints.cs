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

    public static WebApplication MapUploadForm(this WebApplication app)
    {
        app.MapGet("/upload", () => Results.Text(@"
    <html>
      <head><meta charset=""UTF-8""></head>
      <body>
        <h1>画像アップロード</h1>
        <form method=""post"" enctype=""multipart/form-data"" action=""/upload"">
          <input type=""file"" name=""file"" accept=""image/*"" />
          <input type=""submit"" value=""アップロード"" />
        </form>
      </body>
    </html>
", "text/html; charset=utf-8"));

        app.MapPost("/upload", async (HttpRequest request, IPageRepository repo, IOcrGateway ocr) =>
        {
            var file = request.Form.Files["file"];
            if (file == null || file.Length == 0)
                return Results.BadRequest("ファイルが選択されていません");

            var pageId = new PageId(Guid.NewGuid());
            var ext = Path.GetExtension(file.FileName) ?? ".jpg";
            var tempPath = Path.Combine(Path.GetTempPath(), $"{pageId.Value}{ext}");

            await using (var stream = File.Create(tempPath))
            {
                await file.CopyToAsync(stream);
            }

            var page = new Page(pageId, tempPath);
            await repo.SaveAsync(page);

            var text = await ocr.RecognizeAsync(tempPath);
            page.AttachOcr(text);
            await repo.SaveAsync(page);

return Results.Text($@"
    <html>
      <head><meta charset =""UTF - 8""></head>
      <body>
        <h2>OCR結果</h2>
        <pre>{ System.Net.WebUtility.HtmlEncode(text) }</pre>
        <a href=""/upload"">戻る</a>
      </body>
    </html>
", "text/html; charset=utf-8");
        });

        return app;
    }
}
