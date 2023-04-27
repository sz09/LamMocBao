using System.Collections.Generic;

namespace LamMocBaoWeb.Models
{
    public class Province
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<District> Districts { get; set; }
    }

    public class Ward
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }

    }

    public class District
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Ward> Wards { get; set; }
    }
}
