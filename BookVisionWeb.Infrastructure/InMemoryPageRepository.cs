using System.Collections.Concurrent;
using BookVisionWeb.Domain;
using BookVisionWeb.UseCase;

namespace BookVisionWeb.Infrastructure
{
    // 必ず public を付ける
    public sealed class InMemoryPageRepository : IPageRepository
    {
        private readonly ConcurrentDictionary<PageId, Page> _store = new();

        public Task<Page?> FindAsync(PageId id)
            => Task.FromResult(_store.TryGetValue(id, out var p) ? p : null);

        public Task SaveAsync(Page page)
        {
            _store[page.Id] = page;
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Page>> FindAllAsync()
        {
            var pages = _store.Values.ToArray();
            return Task.FromResult<IEnumerable<Page>>(pages);
        }
    }
}
