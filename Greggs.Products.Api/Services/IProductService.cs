using Greggs.Products.Api.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Greggs.Products.Api.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync(int pageStart, int pageSize, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductEuroDto>> GetProductsInEurosAsync(int pageStart, int pageSize, CancellationToken cancellationToken = default);
    }

}
