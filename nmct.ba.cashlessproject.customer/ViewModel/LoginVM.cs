using be.belgium.eid;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.classlibrary;
using nmct.ba.cashlessproject.customer.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.customer.ViewModel
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
                    Customer customer = Identity.CustomerFromCardReader();
                    // Customer customer = new Customer{NationalNumber="93071251560", CustomerName="Ilias"};
                    
                    Console.WriteLine("Getting Customer");
                    if (ApplicationVM.token == null) return;
                    using (HttpClient client = new HttpClient())
                    {
                        client.SetBearerToken(ApplicationVM.token.AccessToken);
                        HttpResponseMessage response = await client.GetAsync("http://localhost:61505/api/Customer/" + customer.NationalNumber);
                        if (response.IsSuccessStatusCode)
                        {
                            string json = await response.Content.ReadAsStringAsync();
                            Customer cust = JsonConvert.DeserializeObject<Customer>(json);
                            if (cust != null)
                            {
                                IsNeWCustomer = false;
                                ApplicationVM.Customer = cust;
                                appvm.LoggedIn();
                                return;
                            }
                        }
                        ApplicationVM.AddErrorLog("No customer found", customer.NationalNumber);
                        ApplicationVM.Customer = null;
                        NewCustomer = customer;
                        IsNeWCustomer = true;
                    }
                }
                catch (BEID_ExNoReader ex)
                {
                    Error = "No Reader Detected";
                    ApplicationVM.AddErrorLog("No Reader Detected", ex.StackTrace);
                    Console.WriteLine(ex.Message);
                }
                catch (BEID_ExNoCardPresent ex)
                {
                    Error = "No Card Detected";
                    ApplicationVM.AddErrorLog("No Card Detected", ex.StackTrace);
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                    ApplicationVM.AddErrorLog("Reader error", ex.StackTrace);
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

        private bool _isNewCustomer = false;
        public bool IsNeWCustomer { get { return _isNewCustomer; } set {
            _isNewCustomer = value;
            OnPropertyChanged("IsNewCustomer");
            RegisterCommand.RaiseCanExecuteChanged();
        } }
        private bool CheckNeWCustomer()
        {
            return IsNeWCustomer;
        }


        private Customer _newCustomer;
        public Customer NewCustomer
        {
            get
            {
                return _newCustomer;
                
            }
            set
            {
                _newCustomer = value;
                OnPropertyChanged("NewCustomer");
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        private RelayCommand _registerCustomer;
        public RelayCommand RegisterCommand
        {
            get
            {
                if (_registerCustomer == null)
                    _registerCustomer = new RelayCommand(RegisterCustomer, CheckNeWCustomer);
                return _registerCustomer;
            }
        }


        private async void RegisterCustomer()
        {
            ApplicationVM appvm = App.Current.MainWindow.DataContext as ApplicationVM;
            using (HttpClient client = new HttpClient())
            {
                Console.WriteLine("Registering Customer");
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                string content = JsonConvert.SerializeObject(NewCustomer);
                HttpResponseMessage response = await client.PostAsync("http://localhost:61505/api/Customer", new StringContent(content, Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode) ApplicationVM.AddErrorLog("Couldn't register customer", response.ReasonPhrase);
                GetCustomer(NewCustomer);
            }
        }

        private async void GetCustomer(Customer customer)
        {
            ApplicationVM appvm = App.Current.MainWindow.DataContext as ApplicationVM;
            Console.WriteLine("Getting Customer");
            if (ApplicationVM.token == null) return;
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:61505/api/Customer/" + customer.NationalNumber);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Customer cust = JsonConvert.DeserializeObject<Customer>(json);
                    if (cust != null)
                    {
                        IsNeWCustomer = false;
                        ApplicationVM.Customer = cust;
                        appvm.LoggedIn();
                        return;
                    }
                }
                else
                {
                    ApplicationVM.AddErrorLog("Couldn't get customers", response.ReasonPhrase);
                }
            }
        }
    }
}
