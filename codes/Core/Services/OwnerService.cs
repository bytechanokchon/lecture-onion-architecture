using Service.Abstractions;
using Shared;

namespace Services;

public class OwnerService : IOwnerService
{
    private readonly IRepositoryManager _repositoryManager;
    
    public OwnerService(IRepositoryManager repositoryManager)
    {
        this._repositoryManager = repositoryManager;
    }

    public Task<IEnumerable<OwnerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
