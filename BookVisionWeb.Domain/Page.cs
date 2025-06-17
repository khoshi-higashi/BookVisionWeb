namespace BookVisionWeb.Domain;

// ページID型（変更なし）
public readonly record struct PageId(Guid Value);
public readonly record struct BookId(Guid Value);

public enum OcrStatus
{
    NotStarted,
    InProgress,
    Completed,
    Failed
}

public sealed class Page
{
    public PageId Id { get; }
    public string ImagePath { get; private set; }
    public string? OcrText { get; private set; }
    public OcrStatus OcrStatus { get; private set; }
    public string? Text { get; private set; }

    // OCR登録日時プロパティを追加
    // OCR結果が保存された日時（yyyy-MM-dd HH:mm:ss形式で永続化する想定）
    public DateTime? RegisteredAt { get; private set; }

    // コンストラクタは既存通り
    public Page(PageId id, string imagePath)
    {
        Id = id;
        ImagePath = imagePath;
        OcrStatus = OcrStatus.NotStarted;
        RegisteredAt = null;
    }

    // OCRテキストをセットし、同時に登録日時もセットするメソッド
    public void AttachOcr(string text)
    {
        OcrText = text;
        RegisteredAt = DateTime.Now;
    }

    // OCR登録日時のみを個別にセットする場合（不要なら省略可）
    public void SetRegisteredAt(DateTime dateTime)
    {
        RegisteredAt = dateTime;
    }

    public void SetOcrStatus(OcrStatus status)
    {
        OcrStatus = status;
    }

    public void SetText(string text)
    {
        Text = text;
    }
}
