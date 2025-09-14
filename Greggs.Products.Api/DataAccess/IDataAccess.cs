using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Greggs.Products.Api.DataAccess;

public interface IDataAccess<T> // Removed 'out' to make T invariant
{
    Task<IEnumerable<T>> ListAsync(int pageStart, int pageSize, CancellationToken ct = default);
}