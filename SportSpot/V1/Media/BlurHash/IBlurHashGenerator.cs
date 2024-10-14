namespace SportSpot.V1.Media
{
    public interface IBlurHashGenerator
    {
        Task<string> GenerateBlurHash(byte[] data);
    }
}
