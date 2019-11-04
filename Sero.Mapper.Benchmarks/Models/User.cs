using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.Benchmarks.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nick { get; set; }
        public DateTime CreationDate { get; set; }
        public Membership Membership { get; set; }
        public User UserCreator { get; set; }
        public User UserLastModifier { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
