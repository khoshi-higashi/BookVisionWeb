namespace BookVisionWeb.Domain;

public readonly record struct PageId(Guid Value);

public sealed class Page
{
    public PageId Id { get; }
    public string ImagePath { get; private set; }
    public string? OcrText { get; private set; }

    public Page(PageId id, string imagePath)
    {
        Id = id;
        ImagePath = imagePath;
    }

    public void AttachOcr(string text) => OcrText = text;
}
