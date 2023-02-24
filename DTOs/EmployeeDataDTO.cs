using DayForceIntegration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayForceIntegration.DTOs
{
    internal class EmployeeDataDTO
    {
        public string? XRefCode { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? DisplayName { get; set; }
        public string? PrimaryLocation { get; set; }
        public Location? HomeOrganization { get; set; }
    }
}
