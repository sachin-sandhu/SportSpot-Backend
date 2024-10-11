using SportSpot.V1.Media.Entities;

namespace SportSpot.V1.Media.BlurHash
{
    public interface IBlurHashFactory
    {
        IBlurHashGenerator GetBlurHashGenerator(MediaEntity mediaEntity);
    }
}
