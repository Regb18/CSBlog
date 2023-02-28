using CSBlog.Services.Interfaces;
using System.Drawing.Text;

namespace CSBlog.Services
{
    public class ImageService : IImageService
    {
        // default blog, category, and user
        private readonly string _defualtUserImage = "/img/DefaultUserImage.png";
        private readonly string _defualtBlogImage = "/img/DefaultBlogImage.jpg";
        private readonly string _defualtCategoryImage = "/img/DefaultCategoryImage.png";
        
        
        public string ConvertByteArrayToFile(byte[] fileData, string extension, int defaultImage)
        {
            if (fileData == null || fileData.Length == 0)
            {
                switch (defaultImage)
                {
                    case 1: return _defualtUserImage;
                    case 2: return _defualtBlogImage;
                    case 3: return _defualtCategoryImage;
                }
            }


            try
            {
                string imageBase64Data = Convert.ToBase64String(fileData!);
                imageBase64Data = string.Format($"data:{extension};base64,{imageBase64Data}");

                return imageBase64Data;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                using MemoryStream memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();
                memoryStream.Close();

                return byteFile;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
