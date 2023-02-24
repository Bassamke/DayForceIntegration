using DayForceIntegration.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayForceIntegration
{
    public class DBDataContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                var connectionstring = "Server=DESKTOP-3CDO770\\SQLEXPRESS; Database=TimeAndAttendance;User Id=sa;Password=123456; encrypt=false;";
                optionsBuilder.UseSqlServer(connectionstring);
            }
            catch (Exception ex)
            {

            }
        }
        public DbSet<Settings> settings { get; set; }
        public DbSet<EmployeeData> employeeData { get; set; }
        public DbSet<Location> location { get; set; }
        public DbSet<BiometricDevices> biometricDevices { get; set; }
        public DbSet<EmployeePunches> employeePunches { get; set; }
        public DbSet<EmployeeLocations> employeeLocations { get; set; }

    }
   
}
