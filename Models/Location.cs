using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayForceIntegration.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; } 
        public string ClockTransferCode { get; set; }   
        public string XRefCode { get; set; }    
        public string ShortName { get; set; }   
        public string LongName { get; set; }
    }
}
