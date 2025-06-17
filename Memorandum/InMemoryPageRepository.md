# BookVisionWeb.Infrastructure/InMemoryPageRepository.cs の役割

## 概要

**InMemoryPageRepository.cs** は、
アプリ内で「Page」エンティティを **メモリ上だけで管理するリポジトリ実装クラス** です。

---

## 主な役割

### 1. IPageRepository のインメモリ実装

- `IPageRepository` インターフェースの実装
- データベース等を使わず、アプリ実行中だけ `ConcurrentDictionary<PageId, Page>` でデータを保持

### 2. 開発・テスト用の簡易ストレージ

- 永続化（DB 保存）が不要な場面（ローカル動作確認・自動テスト等）向け
- アプリ終了とともに全データは消える
- 本番運用では通常、永続ストレージ実装（例：RDBMS, ファイル DB 等）と差し替える

### 3. スレッドセーフな実装

- `ConcurrentDictionary` を使うことでマルチスレッドでも安全に動作

---

## 実装ポイント

- `FindAsync(PageId id)`
  指定 ID の Page を非同期で検索・返却（見つからなければ null）
- `SaveAsync(Page page)`
  Page エンティティを非同期で保存（既存 ID なら上書き）

---

## まとめ

- **InMemoryPageRepository** は
  「Page エンティティの一時的なメモリ保存庫」
- 開発・テスト環境でのストレージ差し替え・疎結合な設計を実現
- 本番では永続リポジトリ実装に差し替える前提の「仮置き」的クラス

---

> 要するに
> **「Page の保存・取得を DB 不要で簡易に実現する開発・検証用の倉庫」**
> です。
