namespace UmeåUppgiftAPI.Models
{
    public class CatImage
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class CatBreed
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class CatImagesViewModel
    {
        public List<CatImage> Images { get; set; } = new List<CatImage>();
        public int CurrentPage { get; set; } = 0;
        public int TotalPages { get; set; } = 0;
    }
} 