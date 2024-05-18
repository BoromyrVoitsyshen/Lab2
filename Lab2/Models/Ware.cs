namespace Lab2.Models
{
    public class Ware
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<WareCategory> WareCategories { get; set; }
        public ICollection<WareOwner> WareOwners { get; set; }
    }
}
