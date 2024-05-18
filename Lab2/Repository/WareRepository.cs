using Lab2.Data;
using Lab2.Models;
using Lab2.Interfaces;
using Azure.Core;

namespace Lab2.Repository
{
    public class WareRepository : IWareInterface
    {
        private readonly DataContext _context;
        public WareRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateWare(int ownerId, int categoryId, Ware ware)
        {
            var owner = _context.Owners.Where(a => a.Id == ownerId).FirstOrDefault();
            var category = _context.Categories.Where(a => a.Id == categoryId).FirstOrDefault();

            var wareOwner = new WareOwner()
            {
                Owner = owner,
                Ware = ware,
            };

            _context.Add(wareOwner);

            var wareCategory = new WareCategory()
            {
                Category = category,
                Ware = ware,
            };

            _context.Add(wareCategory);

            _context.Add(ware);

            return Save();
        }

        public bool DeleteWare(Ware ware)
        {
            _context.Remove(ware);

            return Save();
        }

        public bool DeleteWares(List<Ware> wares)
        {
            _context.RemoveRange(wares);

            return Save();
        }

        public Ware GetWare(int wareId)
        {
            return _context.Wares.Where(p => p.Id == wareId).FirstOrDefault();
        }

        public Ware GetWare(string name)
        {
            return _context.Wares.Where(p => p.Name == name).FirstOrDefault();
        }

        public decimal GetWareRating(int wareId)
        {
            var reviews = _context.Reviews.Where(p => p.Ware.Id == wareId);

            if (reviews.Count() <= 0) 
            {
                return 0;
            }
            return (decimal)reviews.Sum(r => r.Rating) / reviews.Count();
        }

        public ICollection<Ware> GetWares()
        {
            return _context.Wares.OrderBy(p =>  p.Id).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateWare(int ownerId, int categoryId, Ware ware)
        {
            _context.Update(ware);

            return Save();
        }

        public bool WareExists(int wareId)
        {
            return _context.Wares.Any(p => p.Id == wareId);
        }
    }
}
