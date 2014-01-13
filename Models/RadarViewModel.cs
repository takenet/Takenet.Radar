using System.Collections.Generic;

namespace Takenet.Radar.Models
{
    public class RadarViewModel
    {
        public IEnumerable<Entity> Entities { get; set; }
        public IList<Category> Categories { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quadrant { get; set; }
    }

    public class Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }
        public string Top { get; set; }
        public string Left { get; set; }

    }

    public enum Status
    {
        Hold,
        Assess,
        Trial,
        Adopt
    }
}