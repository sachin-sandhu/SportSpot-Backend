using SportSpot.V1.Media.Entities;

namespace SportSpot.V1.Media.BlurHash
{
    public class DefaultBlurHashFactory : IBlurHashFactory
    {
        public IBlurHashGenerator GetBlurHashGenerator(MediaEntity mediaEntity)
        {
            return new ImageBlurHashGenerator();
        }
    }
}
