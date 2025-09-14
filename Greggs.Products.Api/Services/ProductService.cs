using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Helpers;
using Greggs.Products.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using static Greggs.Products.Api.DTO.ProductDTO;
using static Greggs.Products.Api.DTO.ProductEuroDTO;
using System.Linq;

namespace Greggs.Products.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly IDataAccess<Product> _dataAccess;
        private readonly ICurrencyConverter _currencyConverter;

        public ProductService(IDataAccess<Product> dataAccess, ICurrencyConverter currencyConverter)
        {
            _dataAccess = dataAccess;
            _currencyConverter = currencyConverter;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(int pageStart, int pageSize, CancellationToken cancellationToken = default)
        {
            var products = await _dataAccess.ListAsync(pageStart, pageSize, cancellationToken);
            return products.Select(p => new ProductDto(p.Name, p.PriceInPounds));
        }

        public async Task<IEnumerable<ProductEuroDto>> GetProductsInEurosAsync(int pageStart, int pageSize, CancellationToken cancellationToken = default)
        {
            var products = await _dataAccess.ListAsync(pageStart, pageSize, cancellationToken);

            return products.Select(p =>
            {
                var euros = _currencyConverter.ConvertToEur(p.PriceInPounds);
                return new ProductEuroDto(p.Name, p.PriceInPounds, euros);
            });
        }
    }
}
