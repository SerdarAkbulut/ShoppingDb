namespace ShoppingApi.DTO
{
    public class SliderDTO
    {
        public List<ImageDTO> Images { get; set; }
    }
    public class ImageDTO
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
    }
}
