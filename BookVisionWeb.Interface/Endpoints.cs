using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Drawing;
using BookVisionWeb.Domain;
using BookVisionWeb.UseCase;
using Microsoft.Extensions.Hosting;

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

        // --- List all pages and OCR results ---
        app.MapGet("/api/pages", async (IPageRepository repo) =>
        {
            var pages = await repo.FindAllAsync();
            var response = pages.Select(p => new
            {
                pageId = p.Id.Value,
                imagePath = p.ImagePath,
                ocrText = p.OcrText,
                ocrStatus = p.OcrStatus.ToString(),
                registeredAt = p.RegisteredAt
            });
            return Results.Ok(response);
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

    /// <summary>
    /// Resolve the template path. First looks in &lt;ContentRoot&gt;/templates, then one level up.
    /// </summary>
    private static string ResolveTemplatePath(IHostEnvironment env, string fileName)
    {
        var primary = Path.Combine(env.ContentRootPath, "templates", fileName);
        if (File.Exists(primary)) return primary;

        // Fallback: ../templates
        var fallback = Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "templates", fileName));
        if (File.Exists(fallback)) return fallback;

        throw new FileNotFoundException($"Template '{fileName}' was not found. Looked in '{primary}' and '{fallback}'.");
    }

    public static WebApplication MapUploadForm(this WebApplication app)
    {
        app.MapGet("/upload", (IHostEnvironment env) =>
        {
            var htmlPath = ResolveTemplatePath(env, "main.html");
            var html = File.ReadAllText(htmlPath);
            return Results.Text(html, "text/html; charset=utf-8");
        });

        app.MapGet("/pages", (IHostEnvironment env) =>
        {
            var htmlPath = ResolveTemplatePath(env, "pages_list.html");
            var html = File.ReadAllText(htmlPath);
            return Results.Text(html, "text/html; charset=utf-8");
        });

        app.MapPost("/upload", async (HttpRequest request, IHostEnvironment env, IPageRepository repo, IOcrGateway ocr) =>
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

            // Rotate image 90 degrees counter‑clockwise for vertical text
            using (var img = Image.FromFile(tempPath))
            {
                img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                img.Save(tempPath);
            }

            var page = new Page(pageId, tempPath);
            await repo.SaveAsync(page);

            var text = await ocr.RecognizeAsync(tempPath);
            page.AttachOcr(text);
            await repo.SaveAsync(page);

            // Load result HTML template and inject dynamic values
            var template = File.ReadAllText(ResolveTemplatePath(env, "ocr_result_2000.html"));
            var html = template
                .Replace("{{ocrText}}", System.Net.WebUtility.HtmlEncode(text))
                .Replace("{{pageId}}", pageId.Value.ToString());

            return Results.Text(html, "text/html; charset=utf-8");
        });

        // --- Easter‑egg: HTTP 418 “I'm a teapot” ---
        app.MapGet("/coffee", () =>
            Results.Text("I'm a teapot ☕🫖", "text/plain; charset=utf-8", statusCode: 418));

        return app;
    }
}
