using DayForceIntegration.Controllers;
using DayForceIntegration.Migrations;
using DayForceIntegration.Models;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace DayForceIntegration
{
    public partial class Form1 : Form
    {
        public string ConnectionString = "";
        public string DayForceUserName = "";
        public string DayForcePassword = "";
        public DateTime? LastRefreshDatetime;
        public bool busy = false;
        DayforceCalls DayforceCall= new DayforceCalls();
        AnvizController anvizController = new AnvizController();
        public Form1()
        {
            InitializeComponent();
            getIp();
            fetchSettings();
            refeshEmployeesTimer.Start();
        }

        private async void refeshEmployeesTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!busy)
                {
                    busy = true;
                    await refreshEmployeeData();
                    busy = false;
                }
               
            }
            catch(Exception ex)
            {
                ErrorLogService("REFRESH EMPLOYEE DATA Message: " + ex.Message + " Inner Exception: " + ex.InnerException + " STACKTRACE: " + ex.StackTrace);
            }

        }
        public async Task refreshEmployeeData()
        {
            try
            {
                List<EmployeeData> employeesToAdd = new List<EmployeeData>();
                Employees employees = new Employees();
                employees = await DayforceCall.getEmployees(LastRefreshDatetime, DayForceUserName, DayForcePassword);
                foreach (Xref employeeXref in employees.Data)
                {
                    employeesToAdd = new List<EmployeeData>();
                    EmployeeData currentEmployee = new EmployeeData();
                    //var employeeDetails = await DayforceCall.getEmployeeDetails(employeeXref.XRefCode);
                    if (checkIfEmployeeExists(employeeXref.XRefCode))
                    {
                        var employeeDetails = await DayforceCall.getEmployeeDetails(employeeXref.XRefCode);
                        LogService("employee details " + JsonConvert.SerializeObject(employeeDetails));
                        currentEmployee = mapToEmployeeData(employeeDetails);
                        await updateEmployeesList(currentEmployee);
                    }
                    else
                    {
                        var employeeDetails = await DayforceCall.getEmployeeDetails(employeeXref.XRefCode);
                        currentEmployee = mapToEmployeeData(employeeDetails);
                        employeesToAdd.Add(currentEmployee);
                        insertEmployeesList(employeesToAdd);
                    }
                }
                insertEmployeesList(employeesToAdd);
                await updateLastRefreshTime();
                await getEmployeePunches();
            }
            catch (Exception ex) 
            { 
            
            }
            
        }
        public EmployeeData mapToEmployeeData(EmployeeDetails employee)
        {
            try
            {
                var location = employee.Data.HomeOrganization.XRefCode;
                location.Trim();
                string[] splitLocation = location.Split('-');
                location = splitLocation[0];
                EmployeeData employeeData = new EmployeeData
                {
                    XRefCode = employee.Data.EmployeeNumber,
                    PrimaryLocation = employee.Data.HomeOrganization.XRefCode,
                    DisplayName = employee.Data.DisplayName,
                    EmployeeNumber = employee.Data.EmployeeNumber,
                    Location = location.Trim(),
                    TerminationDate = employee.Data.TerminationDate,
                    Active = true,
                    Uploaded = false,
                    multipleDeviceProperty = employee.Data.EmployeeProperties.Items.Count() == 0 ? "": employee.Data.EmployeeProperties.Items[0].EmployeeProperty.ShortName
                };



                return employeeData;
            }
            catch (Exception ex) 
            {
                return new EmployeeData();
            }

        }

        public bool checkIfEmployeeExists(string  xrefCode)
        {
            using (DBDataContext db = new DBDataContext())
            {
                bool found = (from Employee in db.employeeData where Employee.XRefCode == xrefCode select Employee).Any();
                return found;

            }
        }

        public async Task updateEmployeesList(EmployeeData employeesToAdd)
        {
            try
            {

                using (DBDataContext db = new DBDataContext())
                {
                   var employee= (from Employee in db.employeeData where Employee.XRefCode == employeesToAdd.XRefCode select Employee).FirstOrDefault();
                    if (employee.Location != employeesToAdd.Location)
                    {
                        employee.PrimaryLocation= employeesToAdd.PrimaryLocation;
                        employee.LastModified= DateTime.Now;
                        employee.Uploaded = false;
                        employee.OldLocation = employee.Location;
                        employee.DeletedFromOldLocation = false;
                        employee.Location= employeesToAdd.Location;
                        db.SaveChanges();
                    }
                    LogService("check termination " + employeesToAdd.EmployeeNumber + " " + employeesToAdd.TerminationDate);
                    if (employeesToAdd.TerminationDate != null && Convert.ToDateTime(employeesToAdd.TerminationDate) < DateTime.Now)
                    {
                        employee.Active = false;
                        //employee.Uploaded = false;
                        db.SaveChanges();
                    }
                    if (employeesToAdd.TerminationDate == null && employee.Active==false)
                    {
                        employee.Active = true;
                        employee.Uploaded = false;
                        db.SaveChanges();
                    }
                    if(employeesToAdd.multipleDeviceProperty == "Ability to clock Multiple location")
                    {
                        employee.MultipleDevices = true;
                        await insertMultipleEmployeeLocations(employeesToAdd);
                        await db.SaveChangesAsync();
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorLogService("UPDATE EMPLOYEE Message: " + ex.Message + " Inner Exception: " + ex.InnerException + " STACKTRACE: " + ex.StackTrace);
            }



        }
        public async Task insertMultipleEmployeeLocations(EmployeeData Employee)
        {
            try
            {
                using (DBDataContext db = new DBDataContext())
                {
                    var biometricDevices = (from b in db.biometricDevices
                                            select b).ToList();
                    foreach (BiometricDevices biometricDevice in biometricDevices)
                    {
                        bool found = (from b in db.employeeLocations.Where(b => b.EmployeeNumber == Employee.EmployeeNumber && b.Location == biometricDevice.Location)
                                      select b).Any();
                        if (!found)
                        {
                            EmployeeLocations employeelocations = new EmployeeLocations
                            {
                                EmployeeNumber = Employee.EmployeeNumber,
                                Location = biometricDevice.Location,
                                Name = Employee.DisplayName,
                                Uploaded = false
                            };
                            await db.employeeLocations.AddAsync(employeelocations);
                        }

                    }
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorLogService("INSERT EMPLOYEE LOCATIONS: " + ex.Message + " Inner Exception: " + ex.InnerException + " STACKTRACE: " + ex.StackTrace);

            }

        }
        public void insertEmployeesList(List<EmployeeData> employeesToAdd)
        {
            try
            {
                
                using (DBDataContext db = new DBDataContext())
                {
                    db.employeeData.AddRange(employeesToAdd);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogService("INSERT EMPLOYEE Message: " + ex.Message + " Inner Exception: " + ex.InnerException + " STACKTRACE: " + ex.StackTrace);
            }



        }

        public async Task updateLastRefreshTime()
        {
            try
            {

                using (DBDataContext db = new DBDataContext())
                {
                    var settings = (from b in db.settings
                                    select b).FirstOrDefault();
                    settings.LastRefreshDatetime= DateTime.Now;
                    db.SaveChanges();
                    

                }
            }
            catch (Exception ex)
            {
                ErrorLogService("INSERT EMPLOYEE Message: " + ex.Message + " Inner Exception: " + ex.InnerException + " STACKTRACE: " + ex.StackTrace);
            }



        }
        public async Task getEmployeePunches()
        {
            try
            {
                var rawPunches = new List<EmployeePunches>();

                using (DBDataContext db = new DBDataContext())
                {
                    bool found= (from b in db.employeePunches.Where(b => b.UploadedToDayforce == false)
                                 select b).Any();
                    if (found)
                    {
                        rawPunches = (from b in db.employeePunches.Where(b => b.UploadedToDayforce == false)
                                      select b).ToList();

                    }
                }
                await DayforceCall.postEmployeeRawPunches(rawPunches);
            }
            catch (Exception ex)
            {
                ErrorLogService("get employee Punches error : " + ex.Message + " Inner Exception: " + ex.InnerException + " STACKTRACE: " + ex.StackTrace);
            }
        }

        
        public async Task uploadEmployeesToAnviz()
        {
            await anvizController.getDevices();
        }
        public string getIp()
        {
            string path = System.Environment.CurrentDirectory;
            string fileName = path + "\\SVRNAME.ini";
            StreamReader readFile = new StreamReader(fileName);
            string serveName = readFile.ReadLine();
            string serverLogin = readFile.ReadLine();
            string serverPassword = readFile.ReadLine();
            ConnectionString = "Data Source =" + serveName + "; User ID = " + serverLogin + "; Password = " + serverPassword + "; Initial Catalog = TimeAndAttendance";
            return ConnectionString;
        }

        public void fetchSettings()
        {
            using (DBDataContext db = new DBDataContext())
            {
                bool found = (from s in db.settings select s).Any();
                if (found)
                {
                    var Settings = (from b in db.settings
                                   select b).FirstOrDefault();
                    DayForceUserName = Settings.DayForceUserName;
                    DayForcePassword=Settings.DayForcePassword;
                    LastRefreshDatetime=Settings.LastRefreshDatetime;
                }
                else
                {
                    
                }
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
    }
}