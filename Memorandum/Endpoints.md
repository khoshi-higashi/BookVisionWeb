# BookVisionWeb.Interface/Endpoints.cs の役割

## 概要

**Endpoints.cs** は、
BookVisionWeb の Web API で「画像アップロード」「OCR 実行」などのエンドポイント（API ルーティング）を定義する **エンドポイント登録用クラス** です。

---

## 主な役割

### 1. API エンドポイント（ルート）の登録

- `MapOcr` 拡張メソッドとして、Web アプリに OCR 関連 API を登録
  - `/api/pages` ：ページ画像をアップロードして登録（POST）
  - `/api/pages/{pageId:guid}/ocr` ：登録済み画像で OCR を実行しテキストを返す（POST）

### 2. HTTP リクエストの処理フロー記述

- 画像ファイル（`IFormFile`）を一時フォルダへ保存
- 新規 `Page` を作成してリポジトリに保存
- `PageId` をクライアントに返却
- OCR リクエストでは、`IRecognizePageUseCase`（ユースケース）と `RecognizePageJsonPresenter`（レスポンス整形）を協調動作させて結果返却

### 3. Clean Architecture の「インターフェース層」

- ドメインやユースケース層とは独立し、
  **Web リクエストの受け口（Web フレームワーク依存の部分）だけを担当**
- アプリの「外部 I/O」をここで一元化し、内部ロジックと分離

---

## まとめ

- **Endpoints.cs** は、
  BookVisionWeb の Web API（画像アップロード／OCR 実行）の **エンドポイント定義クラス**
- API ルーティングの集約・入出力の橋渡し役
- Clean Architecture の「インターフェース層（外部 I/O）」を担う

---

> 要するに
> **「WebAPI の URL ルートと処理内容を記述する唯一の場所」**
> です。
