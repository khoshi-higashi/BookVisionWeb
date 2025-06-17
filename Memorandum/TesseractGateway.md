# BookVisionWeb.Infrastructure/TesseractGateway.cs の役割

## 概要

**TesseractGateway.cs** は、
OCR エンジン「Tesseract」を使って画像からテキストを抽出するための **ゲートウェイ（橋渡し）実装クラス** です。

---

## 主な役割

### 1. ITesseractGateway インターフェースの実装

- アプリ全体が `ITesseractGateway` を通じて OCR 機能を利用できるようにする
- 依存逆転（DI）により、Tesseract の具体実装をアプリの他部分から分離

### 2. 画像ファイル → テキスト変換の実体

- 画像ファイルパスを受け取り、Tesseract を呼び出してテキスト認識を実行
- 認識結果（テキスト）をアプリ内部へ返却

### 3. OCR エンジンの交換可能化

- このクラスを差し替えれば、他の OCR エンジンやモック実装とも容易に交換可能
- テストや将来的な OCR エンジンの入れ替えも柔軟に対応

---

## 実装ポイント

- `RecognizeTextAsync(string imagePath)`
  画像ファイルのパスを受け取り、Tesseract を使って OCR を実行し、テキストを返す非同期メソッド

---

## まとめ

- **TesseractGateway** は
  「Tesseract OCR を呼び出して画像からテキスト抽出するための**専用ゲートウェイ実装クラス**」
- インターフェースを通じた依存逆転により、OCR 処理の切り替えやテスト容易性を担保

---

> 要するに
> **「Tesseract で画像からテキスト抽出を担う橋渡し専用クラス」**
> です。
