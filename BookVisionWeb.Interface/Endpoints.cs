using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
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
                   IPageRepository repo,
                   IOcrGateway ocr) =>
            {
                // 1. DBからPage取得
                var page = await repo.FindAsync(new PageId(pageId)); if (page == null) return Results.NotFound();

                // 2. OCR実行
                var text = await ocr.RecognizeAsync(page.ImagePath);

                // 3. OCRテキストセット＋登録日時セット
                page.AttachOcr(text);

                // 4. DBへ保存（RegisteredAtも含めて保存）
                await repo.SaveAsync(page);

                return Results.Ok(new { pageId = page.Id.Value, ocrText = page.OcrText, registeredAt = page.RegisteredAt });
            }).DisableAntiforgery();
        return app;
    }

    public static WebApplication MapUploadForm(this WebApplication app)
    {
        app.MapGet("/upload", () =>
        {
            var html = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "templates", "upload_form.html"));
            return Results.Text(html, "text/html; charset=utf-8");
        });

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

            // Load result HTML template and inject dynamic values
            var template = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "templates", "ocr_result.html"));
            var html = template
                .Replace("{{ocrText}}", System.Net.WebUtility.HtmlEncode(text))
                .Replace("{{pageId}}", pageId.Value.ToString());

            return Results.Text(html, "text/html; charset=utf-8");
        });

        return app;
    }
}
