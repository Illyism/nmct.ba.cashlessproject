using be.belgium.eid;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.classlibrary;
using nmct.ba.cashlessproject.employee.Helper;
using nmct.ba.cashlessproject.employee.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.employee.ViewModel
{
    public class LoginVM : ObservableObject, IPage
    {
        public string Name
        {
            get { return "LoginPage";  }
        }

        public string Username
        {
            get { return Properties.Settings.Default.dblogin; }
        }
        public string Password
        {
            get { return Properties.Settings.Default.dbpass; }
        }

        private string _error;

        public string Error
        {
            get { return _error; }
            set { _error = value; OnPropertyChanged("Error"); }
        }

        public ICommand LoginCommand
        {
            get { return new RelayCommand(Login); }
        }

        private async void Login()
        {
            ApplicationVM appvm = App.Current.MainWindow.DataContext as ApplicationVM;
            ApplicationVM.token = GetToken();

            if (!ApplicationVM.token.IsError)
            {
                try
                {
                    Employee employee = Identity.EmployeeFromCardReader();
                    // Employee employee = new Employee{NationalNumber="93071251560", EmployeeName="Ilias"};
                    
                    Console.WriteLine("Getting Employee");
                    if (ApplicationVM.token == null) return;
                    using (HttpClient client = new HttpClient())
                    {
                        client.SetBearerToken(ApplicationVM.token.AccessToken);
                        HttpResponseMessage response = await client.GetAsync("http://localhost:61505/api/Employee/" + employee.NationalNumber);
                        if (response.IsSuccessStatusCode)
                        {
                            string json = await response.Content.ReadAsStringAsync();
                            ApplicationVM.Employee = JsonConvert.DeserializeObject<Employee>(json);
                        }
                        else
                        {
                            Error = "No such employee named " + employee.EmployeeName;
                            ApplicationVM.AddErrorLog("No employee found", employee.EmployeeName + " - " + employee.NationalNumber);
                            ApplicationVM.Employee = null;
                        }
                    }

                    appvm.LoggedIn();
                }
                catch (BEID_ExNoReader ex)
                {
                    Error = "No Reader Detected";
                    ApplicationVM.AddErrorLog("No Reader Detected", ex.Message);
                    Console.WriteLine(ex.Message);
                }
                catch (BEID_ExNoCardPresent ex)
                {
                    Error = "No Card Detected";
                    ApplicationVM.AddErrorLog("No Card Detected", ex.Message);
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                    ApplicationVM.AddErrorLog("Error with reader", ex.Message);
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                ApplicationVM.AddErrorLog("Token error " + Username, ApplicationVM.token.Error);
                Error = ApplicationVM.token.Error;
            }
        }

        private TokenResponse GetToken()
        {
            OAuth2Client client = new OAuth2Client(new Uri("http://localhost:61505/token"));
            return client.RequestResourceOwnerPasswordAsync(Username, Password).Result;
        }


        
    }
}
