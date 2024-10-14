using Blurhash.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SportSpot.V1.Media
{
    public class ImageBlurHashGenerator : IBlurHashGenerator
    {
        public Task<string> GenerateBlurHash(byte[] data)
        {
            return Task.FromResult(Blurhasher.Encode(Image.Load<Rgba32>(data), 4, 4));
        }
    }
}
