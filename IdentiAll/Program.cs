using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace IdentiAll
{
    class Program
    {
        // Add your Computer Vision subscription key and endpoint
        static string subscriptionKey = "37668eee7bed44ba92d330ea1307188c";
        static string endpoint = "https://jamalvision.cognitiveservices.azure.com/";

        #region Images Test
        // URL image used for analyzing an image (turn can caramel)
        //private const string ANALYZE_URL_IMAGE = "https://tribunadejundiai.com.br/wp-content/uploads/2020/07/Screenshot_20200729-201823_Twitter-1200x675.jpg";
        // URL image used for analyzing an image (Pepsi)
        //private const string ANALYZE_URL_IMAGE = "https://1.bp.blogspot.com/-aczoJe0YxS0/U8BUXLiL5II/AAAAAAAAmSs/bEIDWfQy3BE/s1600/pepsi+twist+propaganda.jpg";
        // URL image used for analyzing an image (Hot Dog Towner)
        //private const string ANALYZE_URL_IMAGE = "https://static.vakinha.com.br/uploads/vakinha/image/97431/images.jpg?ims=700x410";
        // URL image used for analyzing an image (Faustão eletrical barbecue grill)
        //private const string ANALYZE_URL_IMAGE = "https://tvhistoria.com.br/wp-content/uploads/2020/04/churrasqueira-600x400.jpg";
        // URL image used for analyzing an image (supermarket cart grill)
        //private const string ANALYZE_URL_IMAGE = "https://images.virgula.com.br/2016/03/12670483_1115178645190819_1611457482391407274_n.jpg";
        // URL image used for analyzing an image (old tv)
        private const string ANALYZE_URL_IMAGE = "https://i.ytimg.com/vi/DyKOz2Sm2IY/maxresdefault.jpg";
        #endregion

        static void Main(string[] args)
        {
            Console.WriteLine("Jamal Computer Vision!");
            Console.WriteLine("----------------------");

            // Create a client
            ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);

            // Analyze an image to get features and other properties.
            ImageAnalysis imageAnalysis = AnalyzeImageUrl(client, ANALYZE_URL_IMAGE).Result;

            // Sunmarizes the image content.
            Console.WriteLine("Summary:");
            foreach (var caption in imageAnalysis.Description.Captions)
            {
                Console.WriteLine($"{caption.Text} with confidence {caption.Confidence}");
            }

            Console.WriteLine();

            // Display categories the image is divided into.
            Console.WriteLine("Categories:");
            foreach (var category in imageAnalysis.Categories)
            {
                Console.WriteLine($"{category.Name} with confidence {category.Score}");
            }
            Console.WriteLine();

            // Image tags and their confidence score
            Console.WriteLine("Tags:");
            foreach (var tag in imageAnalysis.Tags)
            {
                Console.WriteLine($"{tag.Name} {tag.Confidence}");
            }
            Console.WriteLine();

            // Objects
            Console.WriteLine("Objects:");
            foreach (var obj in imageAnalysis.Objects)
            {
                Console.WriteLine($"{obj.ObjectProperty} with confidence {obj.Confidence} at location {obj.Rectangle.X}, " +
                  $"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
            }
            Console.WriteLine();

            // Well-known (or custom, if set) brands.
            Console.WriteLine("Brands:");
            foreach (var brand in imageAnalysis.Brands)
            {
                Console.WriteLine($"Logo of {brand.Name} with confidence {brand.Confidence} at location {brand.Rectangle.X}, " +
                  $"{brand.Rectangle.X + brand.Rectangle.W}, {brand.Rectangle.Y}, {brand.Rectangle.Y + brand.Rectangle.H}");
            }
            Console.WriteLine();

            // Faces
            Console.WriteLine("Faces:");
            foreach (var face in imageAnalysis.Faces)
            {
                Console.WriteLine($"A {face.Gender} of age {face.Age} at location {face.FaceRectangle.Left}, " +
                  $"{face.FaceRectangle.Left}, {face.FaceRectangle.Top + face.FaceRectangle.Width}, " +
                  $"{face.FaceRectangle.Top + face.FaceRectangle.Height}");
            }
            Console.WriteLine();

            // Adult or racy content, if any.
            Console.WriteLine("Adult:");
            Console.WriteLine($"Has adult content: {imageAnalysis.Adult.IsAdultContent} with confidence {imageAnalysis.Adult.AdultScore}");
            Console.WriteLine($"Has racy content: {imageAnalysis.Adult.IsRacyContent} with confidence {imageAnalysis.Adult.RacyScore}");
            Console.WriteLine();

            // Identifies the color scheme.
            Console.WriteLine("Color Scheme:");
            Console.WriteLine("Is black and white?: " + imageAnalysis.Color.IsBWImg);
            Console.WriteLine("Accent color: " + imageAnalysis.Color.AccentColor);
            Console.WriteLine("Dominant background color: " + imageAnalysis.Color.DominantColorBackground);
            Console.WriteLine("Dominant foreground color: " + imageAnalysis.Color.DominantColorForeground);
            Console.WriteLine("Dominant colors: " + string.Join(",", imageAnalysis.Color.DominantColors));
            Console.WriteLine();

            // Celebrities in image, if any.
            Console.WriteLine("Celebrities:");
            foreach (var category in imageAnalysis.Categories)
            {
                if (category.Detail?.Celebrities != null)
                {
                    foreach (var celeb in category.Detail.Celebrities)
                    {
                        Console.WriteLine($"{celeb.Name} with confidence {celeb.Confidence} at location {celeb.FaceRectangle.Left}, " +
                          $"{celeb.FaceRectangle.Top}, {celeb.FaceRectangle.Height}, {celeb.FaceRectangle.Width}");
                    }
                }
            }
            Console.WriteLine();

            // Popular landmarks in image, if any.
            Console.WriteLine("Landmarks:");
            foreach (var category in imageAnalysis.Categories)
            {
                if (category.Detail?.Landmarks != null)
                {
                    foreach (var landmark in category.Detail.Landmarks)
                    {
                        Console.WriteLine($"{landmark.Name} with confidence {landmark.Confidence}");
                    }
                }
            }
            Console.WriteLine();

            // Detects the image types.
            Console.WriteLine("Image Type:");
            Console.WriteLine("Clip Art Type: " + imageAnalysis.ImageType.ClipArtType);
            Console.WriteLine("Line Drawing Type: " + imageAnalysis.ImageType.LineDrawingType);
            Console.WriteLine();
        }

        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        public static async Task<ImageAnalysis> AnalyzeImageUrl(ComputerVisionClient client, string imageUrl)
        {
            // Creating a list that defines the features to be extracted from the image. 
            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
    {
        VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
        VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
        VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
        VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
        VisualFeatureTypes.Objects
    };
            // Analyze the URL image 
            Task<ImageAnalysis> task = Task.Run(() => client.AnalyzeImageAsync(imageUrl, features));
            task.Wait();            

            ImageAnalysis results = task.Result;

            return results;
        }
    }
}
