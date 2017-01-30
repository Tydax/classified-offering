using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace classified_offering.Models
{
    public class ClassifiedOffering
    {
        public int ID { get; set; }
        public string Name { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }
        public int CreatorID { get; set; }
        public virtual User Creator { get; set; }
        public bool isLocked { get; set; }

        public virtual ICollection<Participation> Participations { get; set; }
    }
}