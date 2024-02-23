using System.ComponentModel.DataAnnotations.Schema;

namespace tutorialAPIs
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public bool InStock { get; set; }
        public string image { get; set; }
    }
}
