# BookVisionWeb.Domain/Page.cs の役割

## 概要

**BookVisionWeb.Domain/Page.cs** は、
「本の 1 ページ」を表現し、画像・OCR 状態・テキストなどを管理する **ドメインモデルクラス** を定義するファイルです。

---

## 主な役割

### 1. ページ（Page）のドメインモデル

- 本の 1 ページを表すエンティティ
- 画像パス・OCR 結果・進行状況などを一元的に管理

### 2. 型安全な ID 管理

- `PageId`、`BookId` という record struct で ID の型安全性を保証
  - 「この UUID は本当に Page/Book 用なのか」をコンパイル時に保証

### 3. OCR 処理の状態と結果の管理

- `OcrStatus`（進捗状態: 未開始・進行中・完了・失敗）
- OCR テキスト (`OcrText`)
- 任意のテキスト (`Text`)

### 4. ドメイン操作用メソッド

- OCR 結果の付与: `AttachOcr(string text)`
- OCR 状態の更新: `SetOcrStatus(OcrStatus status)`
- テキストの設定: `SetText(string text)`

---

## まとめ

- **「1 ページ＝ 1 インスタンス」**でアプリ全体のページ情報を厳密に管理
- DDD（ドメイン駆動設計）的観点から、状態や振る舞いをまとめている
- 型安全と保守性・拡張性を高めるための土台

---

> ### 現場目線の補足
>
> 適切な型・ドメインモデルを設けることで、
> アプリの安全性・拡張性・バグ低減に直結します。
