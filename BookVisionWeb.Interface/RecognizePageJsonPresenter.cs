namespace BookVisionWeb.Interface;

using BookVisionWeb.UseCase;

public sealed class RecognizePageJsonPresenter : IRecognizePagePresenter
{
    public object? View { get; private set; }
    public void Complete(RecognizePageOutput o)
        => View = new { pageId = o.PageId.Value, text = o.Text };
}
