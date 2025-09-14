using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.DTO;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{


    private readonly ILogger<ProductController> _logger;
    private readonly IProductService _service;
    private const int MaxPageSize = 100;

    public ProductController(ILogger<ProductController> logger, IProductService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> Get([FromQuery] int pageStart = 0, [FromQuery] int pageSize = 5, CancellationToken cancellationToken = default)
    {
        if (pageStart < 0) return BadRequest("pageStart must be >= 0");
        if (pageSize <= 0 || pageSize > MaxPageSize) return BadRequest($"pageSize must be between 1 and {MaxPageSize}");

        var items = await _service.GetProductsAsync(pageStart, pageSize, cancellationToken);
        return Ok(items);
    }

    [HttpGet("euros")]
    public async Task<ActionResult<IEnumerable<ProductEuroDto>>> GetInEuros([FromQuery] int pageStart = 0, [FromQuery] int pageSize = 5, CancellationToken cancellationToken = default)
    {
        if (pageStart < 0) return BadRequest("pageStart must be >= 0");
        if (pageSize <= 0 || pageSize > MaxPageSize) return BadRequest($"pageSize must be between 1 and {MaxPageSize}");

        var items = await _service.GetProductsInEurosAsync(pageStart, pageSize, cancellationToken);
        return Ok(items);
    }
}