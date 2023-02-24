using Anviz.SDK;
using Anviz.SDK.Responses;
using DayForceIntegration.Migrations;
using DayForceIntegration.Models;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DayForceIntegration.Controllers
{
    public class AnvizController
    {
        public List<BiometricDevices> devices= new List<BiometricDevices>();
        AnvizManager manager = new AnvizManager();
        public async Task getDevices()
        {
            using (DBDataContext db = new DBDataContext())
            {
                bool found = (from s in db.biometricDevices select s).Any();
                if (found)
                {
                     devices = (from b in db.biometricDevices
                                    select b).ToList();
                }
                else
                {

                }
            }
            uplaodData(devices);
        }
        public async void uplaodData(List<BiometricDevices> devices)
        {
            List<EmployeeData> employees = new List<EmployeeData>();
            foreach (BiometricDevices device in devices) {
                await markAlreadyUploaded(device.IPAddress);
                employees =getEmployeesByLocation(device.Location);
                List<UserInfo> users = new List<UserInfo>();
                foreach (EmployeeData employee in employees)
                {              
                        long employeeNumber = long.Parse(employee.EmployeeNumber);
                        string[] splitName = employee.DisplayName.Split(',');
                        string name = splitName[0];
                        var anvizEmployee = new UserInfo((ulong)(employeeNumber), name);
                        users.Add(anvizEmployee);
                }
                uploadToAnviz(users, device.IPAddress);

            }
        }
        public async  Task markAlreadyUploaded(string IpAddress)
        {
            try
            {
               
                var Kilimanidevice = await manager.Connect("192.168.167.222");
                var users = await Kilimanidevice.GetEmployeesData();
                foreach (UserInfo user in users)
                {
                    using (DBDataContext db = new DBDataContext())
                    {
                        var userInDatabase = (from s in db.employeeData.Where(s => s.EmployeeNumber == (user.Id.ToString().Trim().PadLeft(6, '0'))) select s).SingleOrDefault();
                        if (userInDatabase != null)
                        {
                            userInDatabase.Uploaded = true;
                            db.SaveChanges();
                        }
                    }
                }

                Kilimanidevice.Dispose();
            }
            catch(Exception ex)
            {
                ErrorLogService("Failed: " + IpAddress + " " + ex.Message +" StackTrace: " +ex.StackTrace);
            }
        }
        public List<EmployeeData> getEmployeesByLocation(string location)
        {
            List<EmployeeData> employees= new List<EmployeeData>();
            using (DBDataContext db = new DBDataContext())
            {
                    employees = (from b in db.employeeData.Where(b=>b.Location==location.ToUpper().Trim() && b.Uploaded==false)
                               select b).ToList();

            }
            return employees;
        }
        public async Task uploadToAnviz(List<UserInfo> users, string IpAddress)
        {
           
                
            try
            {
                var device = await manager.Connect(IpAddress);
                await device.SetEmployeesData(users);
                LogService("Successs "  + IpAddress);
                
                device.Dispose();
            }
            catch(Exception ex)
            {
                ErrorLogService("Failed: " + IpAddress + " " + ex.Message);
            }
            
        }
        private static void LogService(string content)
        {
            try
            {
                string exe = Process.GetCurrentProcess().MainModule.FileName;
                string path = Path.GetDirectoryName(exe);
                FileStream fs = new FileStream(path + @"\Logs\Logs" + DateTime.Now.ToString("dd-MMM-yy"), FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(DateTime.Now.ToString("dd/MMM/yy HH:mm:ss") + ": " + content);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {

            }
        }
        private static void ErrorLogService(string content)
        {
            try
            {

                string exe = Process.GetCurrentProcess().MainModule.FileName;
                string path = Path.GetDirectoryName(exe);
                FileStream fs = new FileStream(path + @"\Errors\Errors" + DateTime.Now.ToString("dd-MMM-yy"), FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(DateTime.Now.ToString("dd/MMM/yy HH:mm:ss") + ": " + content);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
