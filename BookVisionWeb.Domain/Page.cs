namespace BookVisionWeb.Domain;

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

    public Page(PageId id, string imagePath)
    {
        Id = id;
        ImagePath = imagePath;
        OcrStatus = OcrStatus.NotStarted;
    }

    public void AttachOcr(string text) => OcrText = text;

    public void SetOcrStatus(OcrStatus status)
    {
        OcrStatus = status;
    }

    public void SetText(string text)
    {
        Text = text;
    }
}
