using Lab2.Models;

namespace Lab2.Interfaces
{
    public interface IOwnerInterface
    {
        ICollection<Owner> GetOwners();
        Owner GetOwner(int ownerId);
        ICollection<Owner> GetOwnerOfAWare(int wareId);
        ICollection<Ware> GetWareByOwner(int ownerId);
        bool OwnerExists(int ownerId);
        bool CreateOwner(Owner owner);
        bool UpdateOwner(Owner owner);
        bool DeleteOwner(Owner owner);
        bool DeleteOwners(List<Owner> owners);
        bool Save();
    }
}
