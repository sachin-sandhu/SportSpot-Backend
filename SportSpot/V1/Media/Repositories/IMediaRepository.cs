using SportSpot.V1.Media.Entities;

namespace SportSpot.V1.Media.Repositories
{
    public interface IMediaRepository
    {
        public Task Add(MediaEntity medium);
        public Task Delete(MediaEntity medium);
        public Task Update(MediaEntity medium);
        public Task<MediaEntity?> Get(Guid id);
    }
}
