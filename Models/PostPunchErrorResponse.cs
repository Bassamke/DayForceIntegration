using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayForceIntegration.Models
{
    public class PostPunchErrorResponse
    {
        public List<ProcessResults> processResults { get; set; }
    }
    public class ProcessResults
    {
        public string Code { get; set; }
        public string Context { get; set; } 
        public string Message { get; set; }
        public string Level { get; set; }
    }
}
