namespace Lab2.Models
{
    public class WareOwner
    {
        public int WareId { get; set; }
        public int OwnerId { get; set; }
        public Ware Ware { get; set; }
        public Owner Owner { get; set; }
    }
}
