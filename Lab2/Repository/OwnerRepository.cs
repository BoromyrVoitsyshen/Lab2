using Lab2.Data;
using Lab2.Interfaces;
using Lab2.Models;

namespace Lab2.Repository
{
    public class OwnerRepository : IOwnerInterface
    {
        private readonly DataContext _context;

        public OwnerRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateOwner(Owner owner)
        {
            _context.Add(owner);

            return Save();
        }

        public bool DeleteOwner(Owner owner)
        {
            _context.Remove(owner);

            return Save();
        }

        public bool DeleteOwners(List<Owner> owners)
        {
            _context.RemoveRange(owners);

            return Save();
        }

        public Owner GetOwner(int ownerId)
        {
            return _context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
        }

        public ICollection<Owner> GetOwnerOfAWare(int wareId)
        {
            return _context.WareOwners.Where(w => w.Ware.Id == wareId).Select(o => o.Owner).ToList();
        }

        public ICollection<Owner> GetOwners()
        {
            return _context.Owners.OrderBy(o => o.Id).ToList();
        }

        public ICollection<Ware> GetWareByOwner(int ownerId)
        {
            return _context.WareOwners.Where(w => w.Owner.Id == ownerId).Select(w => w.Ware).ToList();
        }

        public bool OwnerExists(int ownerId)
        {
            return _context.Owners.Any(o => o.Id == ownerId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateOwner(Owner owner)
        {
            _context.Update(owner);

            return Save();
        }
    }
}
