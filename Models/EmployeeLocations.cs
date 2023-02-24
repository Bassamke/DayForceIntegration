using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayForceIntegration.Models
{
    public class EmployeeLocations
    {
        public int Id { get; set; } 
        public string? EmployeeNumber { get; set; }  
        public string? Location { get; set; }    
        public string? Name { get; set; }
        public bool Uploaded { get; set; }  
    }
}
