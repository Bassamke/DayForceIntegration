using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayForceIntegration.Models
{
    public class EmployeeProperties
    {
        public ItemsModel[] Items { get; set; }  
    }
    public class ItemsModel
    {
        public string EffectiveStart { get; set; }
        public string LastModifiedTimestamp { get; set; }
        public bool BitValue { get; set; }
        public EmployeePropertyModel EmployeeProperty { get; set; }
    }
    public class EmployeePropertyModel
    {
        public int DataType { get; set; }
        public int EmployeeCardinality { get; set; }
        public bool GenerateHREvent { get; set; }
        public string? XRefCode { get; set; }
        public string? ShortName { get; set; }
    }
}
