using DayForceIntegration.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DayForceIntegration.Controllers
{
    public class DayforceCalls
    {
        public string DayForceUserName = "";
        public string DayForcePassword = "";
        public DateTime LastRefreshDatetime;

        public  async Task<Employees> getEmployees(DateTime? LastRefreshDate, string userName, string Password)
        {
            await fetchSettings();
            var credentials=Convert.ToBase64String(Encoding.UTF8.GetBytes(userName+":"+ Password));
            Employees employees= new Employees();
            var client = new RestClient("dayforceUrltogetemployees");
            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("Authorization", "Basic "+ credentials);
            request.AddHeader("Content-Type", "application/json");
            var body = @"";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            var filterUpdatedStartDate = LastRefreshDatetime.AddHours(-24).ToString("yyyy-MM-ddTHH:mm:ss");
            var filterUpdatedEndDate = DateTime.Now.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ss");
            request.AddQueryParameter("filterUpdatedStartDate", filterUpdatedStartDate);
            request.AddQueryParameter("filterUpdatedEndDate", filterUpdatedEndDate);
            RestResponse response = client.Execute(request);
            employees = JsonConvert.DeserializeObject<Employees>( response.Content);
            LogService("found " + employees.Data.Count + " changed employees between " + filterUpdatedStartDate + " and " + filterUpdatedEndDate);
            return employees;
        }
        public async Task<EmployeeDetails> getEmployeeDetails(string Xref)
        {
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(DayForceUserName + ":" + DayForcePassword));
            EmployeeDetails employeeDetails = new EmployeeDetails();
            var client = new RestClient("dayforceUrltogetemployeesdetails/" + Xref);
            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("Authorization", "Basic " + credentials);
            request.AddHeader("Content-Type", "application/json");
            var body = @"";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            request.AddQueryParameter("expand", "EmployeeProperties");
            RestResponse response = client.Execute(request);
            LogService(" get employee details response" + response.Content);
            employeeDetails = JsonConvert.DeserializeObject<EmployeeDetails>(response.Content);
            return employeeDetails;
        }

        public async Task postEmployeeRawPunches(List<EmployeePunches> punches)
        {
            try
            {
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(DayForceUserName + ":" + DayForcePassword));
                foreach (EmployeePunches punch in punches)
                {
                    
                    var client = new RestClient("dayforceUrltogetemployeespunches");
                    var request = new RestRequest();
                    request.Method = Method.Post;
                    request.AddHeader("Authorization", "Basic "+ credentials);
                    request.AddHeader("Content-Type", "application/json");
                    var body = new
                    {
                        EmployeeBadge = punch.EmployeeNumber,
                        RawPunchTime = punch.RawPunchTime.ToString("yyyy-MM-ddTHH:mm:ss") + "+03:00",
                        PunchType = punch.PunchType == 0 ? "Punch_In" : punch.PunchType==128? "Punch_In" : "Punch_Out",
                        PunchDevice = punch.PunchDevice
                    };
                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                    RestResponse response = client.Execute(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content;
                        var errorModel = JsonConvert.DeserializeObject<PostPunchErrorResponse>(result);
                        var errorMessage = errorModel.processResults[0].Message;
                        updateEmployeePunches(punch, errorMessage);
                    }
                    else
                    {
                        var result=response.Content;
                        var errorModel=JsonConvert.DeserializeObject<PostPunchErrorResponse>(result);
                        var errorMessage = errorModel.processResults[0].Message;
                    }
                    LogService(response.Content);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService("post  punch to dayforce failed " + ex.Message);
            }
        }
        public async Task updateEmployeePunches(EmployeePunches employeePunch, string errorMessage)
        {
            try
            {

                using (DBDataContext db = new DBDataContext())
                {
                    var punch = (from b in db.employeePunches.Where(punch=>punch.EmployeeNumber== employeePunch.EmployeeNumber && punch.RawPunchTime==employeePunch.RawPunchTime)
                                    select b).FirstOrDefault();
                    punch.UploadedToDayforce = true;
                    punch.DayForceResponse = errorMessage;
                    db.SaveChanges();


                }
            }
            catch (Exception ex)
            {
                ErrorLogService("Update employee Punch failed Message: " + ex.Message + " Inner Exception: " + ex.InnerException + " STACKTRACE: " + ex.StackTrace);
            }
        }
        public  async Task fetchSettings()
        {
            using (DBDataContext db = new DBDataContext())
            {
                bool found = (from s in db.settings select s).Any();
                if (found)
                {
                    var Settings = (from b in db.settings
                                    select b).FirstOrDefault();
                    DayForceUserName = Settings.DayForceUserName;
                    DayForcePassword = Settings.DayForcePassword;
                    LastRefreshDatetime = Settings.LastRefreshDatetime;
                }
                else
                {

                }
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
