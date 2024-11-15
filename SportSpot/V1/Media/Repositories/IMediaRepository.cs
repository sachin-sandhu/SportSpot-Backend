using SportSpot.V1.Media.Entities;

namespace SportSpot.V1.Media.Repositories
{
    public interface IMediaRepository
    {
        Task Add(MediaEntity medium);
        Task Delete(MediaEntity medium);
        Task Update(MediaEntity medium);
        Task<MediaEntity?> Get(Guid id);
        Task<List<MediaEntity>> GetAll();
    }
}
