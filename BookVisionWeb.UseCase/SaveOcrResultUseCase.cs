using BookVisionWeb.Domain;
using System;

namespace BookVisionWeb.UseCase
{
    /// <summary>
    /// OCR結果保存ユースケース
    /// </summary>
    public class SaveOcrResultUseCase
    {
        private readonly IPageRepository _repository;

        public SaveOcrResultUseCase(IPageRepository repository)
        {
            _repository = repository;
        }

        public async Task SaveAsync(PageId pageId, string ocrText)
        {
            var page = await _repository.FindAsync(pageId);
            if (page == null)
                throw new InvalidOperationException("指定したページが存在しません");

            page.AttachOcr(ocrText);
            await _repository.SaveAsync(page);
        }
    }
}