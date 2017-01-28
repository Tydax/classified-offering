using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace classified_offering.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Pseudo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public virtual ICollection<Participation> Participations { get; set; }
        public virtual ICollection<ClassifiedOffering> CreatedClassifiedOfferings { get; set; }
    }
}