using Microsoft.Extensions.Caching.Distributed;

namespace SportSpot_Test.Location.Mocks
{
    internal class MockDistributedCache : IDistributedCache
    {
        private readonly Dictionary<string, byte[]> _cache = [];

        public int Counter { get; set; } = 0;

        public byte[]? Get(string key)
        {
            var element = _cache.TryGetValue(key, out var value) ? value : null;
            if (element != null)
                Counter++;
            return element;
        }

        public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            var element = _cache.TryGetValue(key, out var value) ? value : null;
            if (element != null)
                Counter++;
            return Task.FromResult(element);
        }

        public void Refresh(string key)
        {
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            _cache.Remove(key);
            _cache.Add(key, value);
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            _cache.Remove(key);
            _cache.Add(key, value);
            return Task.CompletedTask;
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public int Count() => _cache.Count;
    }
}
