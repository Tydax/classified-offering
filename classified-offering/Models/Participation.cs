using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace classified_offering.Models
{
    public class Participation
    {
        public int OffererID { get; set; }
        public virtual ApplicationUser Offerer { get; set; }
        public int ClassifiedOfferingID { get; set; }
        public virtual ClassifiedOffering ClassifiedOffering { get; set; }
        public int ReceiverID { get; set; }
        public virtual ApplicationUser Receiver { get; set; }
    }
}