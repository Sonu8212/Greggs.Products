using Greggs.Products.Api.Configuration;
using Greggs.Products.Api.Models;
using System;

namespace Greggs.Products.Api.Helpers
{
    public interface ICurrencyConverter
    {
        decimal ConvertToEur(decimal amountInGbp);
    }

    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly CurrencyOptions _options;

        public CurrencyConverter(CurrencyOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public decimal ConvertToEur(decimal amountInGbp)
        {
            // Round away from zero to 2 decimals
            return Math.Round(amountInGbp * _options.GbpToEur, 2, MidpointRounding.AwayFromZero);
        }
    }
}
