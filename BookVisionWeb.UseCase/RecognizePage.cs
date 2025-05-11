namespace BookVisionWeb.UseCase;

using BookVisionWeb.Domain;

public sealed record RecognizePageInput(PageId PageId);
public sealed record RecognizePageOutput(PageId PageId, string Text);

public interface IRecognizePagePresenter
{
    void Complete(RecognizePageOutput output);
}

public interface IRecognizePageUseCase
{
    Task HandleAsync(RecognizePageInput input, IRecognizePagePresenter presenter);
}

public interface IPageRepository
{
    Task<Page?> FindAsync(PageId id);
    Task SaveAsync(Page page);
}

public interface IOcrGateway
{
    Task<string> RecognizeAsync(string imagePath);
}

public sealed class RecognizePageInteractor : IRecognizePageUseCase
{
    private readonly IPageRepository _repo;
    private readonly IOcrGateway _ocr;
    public RecognizePageInteractor(IPageRepository repo, IOcrGateway ocr)
        => (_repo, _ocr) = (repo, ocr);

    public async Task HandleAsync(RecognizePageInput i, IRecognizePagePresenter p)
    {
        var page = await _repo.FindAsync(i.PageId)
                   ?? throw new InvalidOperationException("Page not found");

        var text = await _ocr.RecognizeAsync(page.ImagePath);
        page.AttachOcr(text);
        await _repo.SaveAsync(page);

        p.Complete(new(i.PageId, text));
    }
}
