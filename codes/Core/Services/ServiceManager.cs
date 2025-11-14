using Service.Abstractions;

namespace Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IOwnerService> _lazyOwnerService;

    public ServiceManager(IRepositoryManager repositoryManager)
    {
        this._lazyOwnerService = new Lazy<IOwnerService>(() => new OwnerService(repositoryManager));
    }

    public IOwnerService OwnerService => this._lazyOwnerService.Value;
}
