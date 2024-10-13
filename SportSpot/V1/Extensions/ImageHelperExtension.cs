using SixLabors.ImageSharp;
using SportSpot.V1.Exceptions;

namespace SportSpot.V1.Extensions
{
    public static class ImageHelperExtension
    {

        public static byte[] FromBase64String(this string value)
        {
            string toConvert = value;
            if (toConvert.Contains(','))
                toConvert = toConvert.Split(',')[1];
            try
            {
                return Convert.FromBase64String(toConvert);
            }
            catch (Exception)
            {
                throw new BadRequestException("Invalid Base64 string");
            }
        }

        public static bool CheckImage(this byte[] bytes)
        {
            try
            {
                Image.Load(bytes);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static byte[] GetAsByteImage(this string base64)
        {
            byte[] bytes = FromBase64String(base64);
            if (!bytes.CheckImage())
                throw new BadRequestException("Invalid image format");
            return bytes;
        }
    }
}
