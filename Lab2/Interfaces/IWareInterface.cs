using Lab2.Models;

namespace Lab2.Interfaces
{
    public interface IWareInterface
    {
        ICollection<Ware> GetWares();
        Ware GetWare(int wareId);
        Ware GetWare(string name);
        decimal GetWareRating(int wareId);
        bool WareExists(int wareId);
        bool CreateWare(int ownerId, int categoryId, Ware ware);
        bool UpdateWare(int ownerId, int categoryId, Ware ware);
        bool DeleteWare(Ware ware);
        bool DeleteWares(List<Ware> wares);
        bool Save();
    }
}
