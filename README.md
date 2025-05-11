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
