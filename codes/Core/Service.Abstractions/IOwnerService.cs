using Shared;

namespace Service.Abstractions;

public interface IOwnerService
{
    Task<IEnumerable<OwnerDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
