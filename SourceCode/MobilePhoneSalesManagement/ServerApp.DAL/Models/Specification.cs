using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.DAL.Models
{
    public class Specification
    {
        public int SpecificationId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
