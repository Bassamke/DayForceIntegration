using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayForceIntegration.Models
{
    public class Settings
    {
        [Key]
        public int Id { get; set; } 
        public DateTime LastRefreshDatetime { get; set; } 
        public DateTime? LicenceValidUntil { get;set; }
        public string DayForceUserName { get; set; }
        public string DayForcePassword { get; set;}

    }
}
