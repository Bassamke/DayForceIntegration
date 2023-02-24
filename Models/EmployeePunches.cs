using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayForceIntegration.Models
{
    public class EmployeePunches
    {
        public int Id { get; set; } 
        public string EmployeeNumber { get; set; }  
        public DateTime RawPunchTime { get; set; }
        public int PunchType { get; set; }   
        public string PunchDevice { get; set; }
        public bool UploadedToDayforce { get; set; }
        public string? DayForceResponse { get; set; }    

    }
}
