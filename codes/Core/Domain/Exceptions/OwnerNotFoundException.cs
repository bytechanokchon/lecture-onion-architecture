namespace Domain.Exceptions;

public class OwnerNotFoundException : NotFoundException
{
    public OwnerNotFoundException(Guid ownerId) : base($"The owner with identifier {ownerId} was not found.")
    {
        
    }
}
