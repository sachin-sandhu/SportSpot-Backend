using Azure.Storage.Sas;
using SportSpot.V1.Exceptions.Media;
using SportSpot.V1.Media.BlurHash;
using SportSpot.V1.Media.Entities;
using SportSpot.V1.Media.Repositories;
using SportSpot.V1.Storage;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.Media.Services
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
                FileName = filename,
                CreatedBy = creator.Id,
                CreatedAt = DateTime.UtcNow,
                Blocked = false
            };
            mediaEntity.BlurHash = await GenerateBlurHash(mediaEntity, data);
            await _mediaRepository.Add(mediaEntity);
            await _blobClient.UploadData(mediaEntity.Id.ToString(), data, true);
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

        public async Task<MediaEntity> CreateOrUpdateMedia(Guid? id, string filename, byte[] data, AuthUserEntity user)
        {
            if (!id.HasValue)
                return await CreateMedia(filename, data, user);
            MediaEntity? mediaEntity = await _mediaRepository.Get(id.Value);
            if (mediaEntity is null)
                return await CreateMedia(filename, data, user);
            return await UpdateMedia(mediaEntity, filename, data, user);
        }

        public async Task DeleteMedia(MediaEntity media)
        {
            try
            {
                await _blobClient.DeleteBlob(media.Id.ToString());
            }
            catch (Exception)
            {
                //Ignore
            }
            await _mediaRepository.Delete(media);
        }

        public async Task DeleteMedia(Guid id)
        {
            await DeleteMedia(await GetMedia(id));
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

        public async Task DeleteAllMedia()
        {
            foreach (MediaEntity media in await _mediaRepository.GetAll())
            {
                await DeleteMedia(media);
            }
        }
    }
}
