using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayForceIntegration.Models
{
    public class EmployeeDetails
    {
        public EmployeeDataDTO Data { get; set; }  
        
    }
    public class EmployeeDataDTO
    {
        public string? XRefCode { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? DisplayName { get; set; }
        public string? PrimaryLocation { get; set; }
        public Location? HomeOrganization { get; set; }
        public EmployeeProperties? EmployeeProperties { get; set; }
        public string? TerminationDate { get; set; }

    }
    public class EmployeeData
    {
        [Key]
        public int Id { get; set; }
        public string? XRefCode { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? DisplayName { get; set; }
        public string? PrimaryLocation { get; set; }
        public string? Location { get; set; }
        public bool? Uploaded { get; set; }  
        public DateTime? LastModified { get; set; }
        public string? FaceTemplate { get; set; }
        public bool? Active { get; set; }
        public string? OldLocation { get; set; }

        public bool? DeletedFromOldLocation { get; set; }
        public string? TerminationDate { get; set; }
        public bool? MultipleDevices { get; set; }  
        public string? multipleDeviceProperty { get; set; }
    }

}
