namespace SportSpot.V1.Media
{
    public interface IBlurHashFactory
    {
        IBlurHashGenerator GetBlurHashGenerator(MediaEntity mediaEntity);
    }
}
