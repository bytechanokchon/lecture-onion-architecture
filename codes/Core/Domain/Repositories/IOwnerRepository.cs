using System;
using Domain.Entities;

namespace Domain.Repositories;

public interface IOwnerRepository
{
    Task<IEnumerable<Owner>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Owner> GetByIdAsync(Guid guid, CancellationToken cancellationToken = default);
    void Insert(Owner owner);
    void Remove(Owner owner);
}
