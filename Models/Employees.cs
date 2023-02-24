using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayForceIntegration.Models
{
    public class Employees
    {
        public List<Xref> Data { get; set; }  
    }
    public class Xref
    {
        public string XRefCode { get; set; }    
    }
}
