namespace Lab2.Models
{
    public class WareCategory
    {
        public int WareId { get; set; }
        public int CategoryId { get; set; }
        public Ware Ware { get; set; }
        public Category Category { get; set; }
    }
}
