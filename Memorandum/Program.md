# BookVisionWeb.Web/Program.cs の役割

## 概要

**Program.cs** は、
BookVisionWeb アプリケーションのエントリーポイント（起動ファイル）であり、
**WebAPI アプリの起動・依存関係注入（DI）・エンドポイント登録をまとめるファイル** です。

---

## 主な役割

### 1. アプリのエントリーポイント

- .NET Web アプリケーションの「最初に実行されるファイル」
- アプリ全体の起動と終了をここから制御

### 2. DI（依存関係注入）の登録

- UseCase 層、Interface 層、Infrastructure 層の各サービスを DI コンテナに登録
  - `IRecognizePageUseCase` → `RecognizePageInteractor`
  - `RecognizePageJsonPresenter`
  - `IPageRepository` → `InMemoryPageRepository`
  - `IOcrGateway` → `TesseractGateway`
- 各層の依存をここで紐付けることで、疎結合な構成を実現

### 3. WebAPI エンドポイントのマッピング

- `app.MapOcr();` により、OCR 用 API ルーティング（Interface 層の拡張メソッドで定義）を登録
- WebAPI として外部からアクセス可能なルートを定義

### 4. アプリケーションの起動

- `app.Run("http://localhost:5040");` でサーバーを起動し、指定ポートで待機

---

## まとめ

- **Program.cs** は、
  BookVisionWeb WebAPI アプリの「起動」「依存性登録」「エンドポイント設定」を一括管理する**エントリーファイル**
- クリーンアーキテクチャ各層の依存関係を明示し、アプリ全体を組み上げて起動

---

> 要するに
> **「WebAPI アプリの“起動と構成”を一手に引き受ける最重要ファイル」**
> です。
