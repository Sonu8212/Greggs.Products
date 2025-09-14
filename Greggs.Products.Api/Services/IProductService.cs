using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using static Greggs.Products.Api.DTO.ProductDTO;
using static Greggs.Products.Api.DTO.ProductEuroDTO;

namespace Greggs.Products.Api.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync(int pageStart, int pageSize, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductEuroDto>> GetProductsInEurosAsync(int pageStart, int pageSize, CancellationToken cancellationToken = default);
    }
}
