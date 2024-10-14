using Azure.Storage.Sas;
using SportSpot.V1.Exceptions;
using SportSpot.V1.Storage;
using SportSpot.V1.User;

namespace SportSpot.V1.Media
{
    public class MediaService(IBlobClient _blobClient, IBlurHashFactory _blurHashFactory, IMediaRepository _mediaRepository) : IMediaService
    {
        public async Task<MediaEntity> GetMedia(Guid id)
        {
            return await _mediaRepository.Get(id) ?? throw new MediaNotFoundException();
        }

        public async Task BlockMedia(MediaEntity media)
        {
            media.Blocked = true;
            await _mediaRepository.Update(media);
        }

        public async Task UnblockMedia(MediaEntity media)
        {
            media.Blocked = false;
            await _mediaRepository.Update(media);
        }

        public async Task<MediaEntity> CreateMedia(string filename, byte[] data, AuthUserEntity creator)
        {
            MediaEntity mediaEntity = new()
            {
                Id = await GetFreeId(),
                FileName = filename,
                CreatedBy = creator.Id,
                CreatedAt = DateTime.UtcNow,
                Blocked = false
            };
            mediaEntity.BlurHash = await GenerateBlurHash(mediaEntity, data);

            await _blobClient.UploadData(mediaEntity.Id.ToString(), data, true);
            await _mediaRepository.Add(mediaEntity);
            return mediaEntity;
        }

        public async Task<MediaEntity> UpdateMedia(MediaEntity media, string filename, byte[] data, AuthUserEntity editor)
        {
            media.FileName = filename;
            media.UpdatedAt = DateTime.UtcNow;
            media.UpdatedBy = editor.Id;
            media.BlurHash = await GenerateBlurHash(media, data);

            await _blobClient.UploadData(media.Id.ToString(), data, true);
            await _mediaRepository.Update(media);
            return media;
        }

        public async Task<MediaEntity> CreateOrUpdateMedia(Guid id, string filename, byte[] data, AuthUserEntity user)
        {
            MediaEntity? mediaEntity = await _mediaRepository.Get(id);
            if (mediaEntity is null)
                return await CreateMedia(filename, data, user);
            return await UpdateMedia(mediaEntity, filename, data, user);
        }

        public async Task DeleteMedia(MediaEntity media)
        {
            await _mediaRepository.Delete(media);
        }

        public async Task DeleteMedia(Guid id)
        {
            await DeleteMedia(await GetMedia(id));
        }


        private async Task<Guid> GetFreeId()
        {
            Guid id;
            do
            {
                id = Guid.NewGuid();
            } while (await _mediaRepository.Get(id) is not null);
            return id;
        }

        private async Task<string?> GenerateBlurHash(MediaEntity mediaEntity, byte[] data)
        {
            try
            {
                return await _blurHashFactory.GetBlurHashGenerator(mediaEntity).GenerateBlurHash(data);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<byte[]> GetMediaAsBytes(MediaEntity media)
        {
            if (media.Blocked)
                return []; //TODO: return placeholder image
            return await _blobClient.DownloadData(media.Id.ToString());
        }

        public string GenerateSaSUri(MediaEntity media)
        {
            if (media.Blocked)
                return string.Empty; //TODO: return placeholder image
            return _blobClient.GenerateSaSUri(media.Id.ToString(), BlobContainerSasPermissions.Read).ToString();
        }
    }
}
