# BookVisionWeb.Interface/RecognizePageJsonPresenter.cs の役割

## 概要

**RecognizePageJsonPresenter.cs** は、
OCR 結果（RecognizePageOutput）をクライアントに返却するための **JSON 整形用 Presenter クラス** を定義しています。

---

## 主な役割

### 1. UseCase の出力を API レスポンス形式に変換

- ユースケース（`RecognizePageUseCase`など）の出力（`RecognizePageOutput`）を、API で返しやすい形（JSON）に加工する
- 具体的には、`pageId` と `text` のプロパティを持つ匿名オブジェクトを `View` に格納

### 2. Clean Architecture の Presenter 層

- Clean Architecture では「ユースケース層の出力 → プレゼンター層で整形 → コントローラ/フレームワーク層で返却」という流れ
- プレゼンター層（このクラス）が、ドメインやユースケースの「生の出力」を、Web API レスポンスに最適な形に変換する役割

### 3. フロントエンドや外部とデータ受け渡し

- この Presenter を通すことで、フロントエンドや外部システムへ一貫したレスポンス形式（`{ pageId, text }`）を保証

---

## まとめ

- **RecognizePageJsonPresenter** は
  OCR 処理の出力結果（ページ ID・認識テキスト）を、API で返すための「JSON 形式オブジェクト」に変換するクラス
- クリーンアーキテクチャの「Presenter 層」として、内部表現と外部レスポンスの橋渡しを担う

---

> クリーンアーキテクチャを知らない人向けには
> 「**API レスポンス用の整形担当クラス**」
> という説明が最も実用的です。
