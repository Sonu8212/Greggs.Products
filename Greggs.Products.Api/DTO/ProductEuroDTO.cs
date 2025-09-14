namespace Greggs.Products.Api.DTO
{
    public class ProductEuroDTO
    {
        public record ProductEuroDto(string Name, decimal PriceInPounds, decimal PriceInEuros);
    }
}
