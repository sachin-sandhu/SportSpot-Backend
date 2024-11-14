namespace SportSpot.V1.Media.BlurHash
{
    public interface IBlurHashGenerator
    {
        Task<string> GenerateBlurHash(byte[] data);
    }
}
