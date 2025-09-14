namespace Greggs.Products.Api.DTO
{
    public class ProductDTO
    {
        public record ProductDto(string Name, decimal PriceInPounds);
    }
}
