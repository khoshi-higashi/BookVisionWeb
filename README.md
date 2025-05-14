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

---

## 🗒 開発メモ（OCR 動作確認備忘録）

> macOS / zsh の例です。Windows PowerShell ではコマンドを読み替えてください。

### ✅ 前提環境

| 項目                  | 確認コマンド             | 期待結果               |
| --------------------- | ------------------------ | ---------------------- |
| .NET SDK 9            | `dotnet --version`       | `9.x.xxx`              |
| Tesseract  ランタイム | `tesseract --version`    | バージョンが表示される |
| 英語辞書              | `tesseract --list-langs` | `eng` が含まれる       |
| プロジェクト依存      | `dotnet build`           | `Build succeeded`      |

### ▶️ Web API 起動

```bash
cd ~/Projects/BookVisionWeb
dotnet run --project BookVisionWeb.Web &
APP_PID=$!
sleep 5
```

### 🖼️ 画像アップロード → OCR 実行 → 結果保存

```bash
PAGE_ID=$(curl -s -F "file=@sample.jpg" http://localhost:5040/api/pages | jq -r .pageId)
curl -X POST http://localhost:5040/api/pages/$PAGE_ID/ocr | jq -r .text | tr -d ' ' > ocr_result.txt
```

- `ocr_result.txt` に OCR 結果を保存（半角スペースなし）

### 🧪 期待レスポンス例

```json
{
  "pageId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "text": "Thequickbrownfoxjumpsoverthelazydog1234567890"
}
```

### ⚠️ よくあるエラーと対処法

| 症状                                             | 対策                                                      |
| ------------------------------------------------ | --------------------------------------------------------- |
| `text` が空                                      | 画像 DPI 不足、Tesseract 辞書不足（300DPI 以上 & `-jpn`） |
| `curl: (7) Failed to connect`                    | API 起動待ち時間不足。`sleep 5` を調整                    |
| `500` エラー                                     | Tesseract DLL や`TESSDATA_PREFIX`の設定漏れ               |
| `DllNotFoundException: liblept…`                 | `brew install leptonica`                                  |
| `System.InvalidOperationException (Antiforgery)` | `.DisableAntiforgery()` 忘れに注意                        |

### 🧹 クリーンアップ

```bash
kill $APP_PID
rm /private/tmp/*.jpg
```

---
