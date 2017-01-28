using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace classified_offering.Models
{
    public class User
    {
        public int ID { get; set; }
        [Required]
        
        public string Pseudo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public int Role { get; set; }
        public virtual ICollection<Participation> Participations { get; set; }
        public virtual ICollection<ClassifiedOffering> CreatedClassifiedOfferings { get; set; }
    }
}