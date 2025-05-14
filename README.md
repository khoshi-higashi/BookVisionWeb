# BookVisionWeb

BookVisionWeb is a Clean-Architecture–driven web service that turns photos / scans of books into searchable text and lets you browse the results in your browser.

---

## Features

| Layer              | Highlights                                                                                               |
| ------------------ | -------------------------------------------------------------------------------------------------------- |
| **Domain**         | `Book`, `Page`, value-object IDs. No framework code, 100 % unit-testable.                                |
| **UseCase**        | One interactor now: **RecognizePage** (OCR). Pure C#.                                                    |
| **Interface**      | Minimal-API endpoint `POST /api/books/{bookId}/pages/{pageId}/ocr` plus JSON presenter.                  |
| **Infrastructure** | `TesseractGateway` for local OCR, `EfCorePageRepository` for DB (SQLite by default). Swap either via DI. |

---

## Project Map

```

BookVisionWeb.sln
├─ BookVisionWeb.Domain/ <- Entities / value objects
├─ BookVisionWeb.UseCase/ <- DTOs & interactors
├─ BookVisionWeb.Interface/ <- Web API, presenter, ports
├─ BookVisionWeb.Infrastructure/ <- EF Core, OCR gateway
└─ BookVisionWeb.Web/ <- ASP.NET host (Minimal API)

```

---

## Tests

```bash
dotnet test
```

---

## 🗒 開発メモ（縦書き OCR 備忘録）

- Tesseract は `jpn_vert` モデルを使うことで縦書きにも対応可能
- 画像を左に 90 度回転させると精度が上がる（sips -r -90 など）
- 出力時に半角スペースを除くには：

```bash
curl -X POST http://localhost:5040/api/pages/$PAGE_ID/ocr | jq -r .text | tr -d ' ' > ocr_result.txt
```

- jpn_vert.traineddata は /opt/homebrew/share/tessdata に配置（macOS）
