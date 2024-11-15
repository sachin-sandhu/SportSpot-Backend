using SportSpot.V1.Media.Entities;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.Media.Services
{
    public interface IMediaService
    {
        Task<MediaEntity> GetMedia(Guid id);
        Task<MediaEntity> CreateMedia(string filename, byte[] data, AuthUserEntity creator);
        Task<MediaEntity> UpdateMedia(MediaEntity media, string filename, byte[] data, AuthUserEntity editor);
        Task<MediaEntity> CreateOrUpdateMedia(Guid id, string filename, byte[] data, AuthUserEntity user);
        Task<byte[]> GetMediaAsBytes(MediaEntity media);
        string GenerateSaSUri(MediaEntity media);
        Task BlockMedia(MediaEntity media);
        Task UnblockMedia(MediaEntity media);
        Task DeleteMedia(MediaEntity media);
        Task DeleteMedia(Guid id);
        Task DeleteAllMedia();
    }
}
