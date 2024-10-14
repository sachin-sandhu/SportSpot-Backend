namespace SportSpot.V1.Media
{
    public class DefaultBlurHashFactory : IBlurHashFactory
    {
        public IBlurHashGenerator GetBlurHashGenerator(MediaEntity mediaEntity)
        {
            return new ImageBlurHashGenerator();
        }
    }
}
