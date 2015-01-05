using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.classlibrary;
using nmct.ba.cashlessproject.employee.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.employee.ViewModel
{
    public class ApplicationVM : ObservableObject
    {
        public static TokenResponse token = null;
        public static Employee Employee { get; set; }
        public static Register Register { get; set; }
        public static Customer Customer { get; set; }
        public static RegisterEmployee RegEmp { get; set; }

        public ApplicationVM()
        {
            CurrentPage = new LoginVM();
            Register = new Register {
                ID = Properties.Settings.Default.RegisterID,
                RegisterName = Properties.Settings.Default.RegisterName
            };
            RegEmp = new RegisterEmployee
            {
                RegisterID = Properties.Settings.Default.RegisterID
            };
        }

        private ObservableCollection<IPage> _pages;
        public ObservableCollection<IPage> Pages
        {
            get {
                Console.WriteLine("Get Pages");
                if (_pages == null)
                    _pages = new ObservableCollection<IPage>();
                return _pages;
            }
        }

        private IPage _currentPage;
        public IPage CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; OnPropertyChanged("CurrentPage"); }
        }

        public ICommand ChangePageCommand
        {
            get { return new RelayCommand<IPage>(ChangePage);  }
        }

        public void ChangePage(IPage page)
        {
            CurrentPage = page;
        }

        public void LoggedIn()
        {
            Pages.Add(new MainPageVM());
            OnPropertyChanged("Pages");
            ChangePage(Pages[0]);
            RegEmp.Employee = Employee;
            RegEmp.FromTime = DateTime.Now;
        }

        public void LogOut()
        {
            RegEmp.UntilTime = DateTime.Now;
            AddRegEmp(RegEmp);

            ApplicationVM.token = null;
            Pages.Clear();
            OnPropertyChanged("Pages");

            ChangePage(new LoginVM());
        }

        public static async void AddErrorLog(string message, string stacktrace)
        {
            ErrorLog error = new ErrorLog { Message = message, Stacktrace = stacktrace, RegisterID = ApplicationVM.Register.ID };
            Console.WriteLine("Error sent: " + error.Message);
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                string content = JsonConvert.SerializeObject(error);
                HttpResponseMessage response = await client.PostAsync("http://localhost:61505/api/ErrorLog", new StringContent(content, Encoding.UTF8, "application/json"));
            }
        }

        public static async void AddRegEmp(RegisterEmployee regEmp)
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                string content = JsonConvert.SerializeObject(regEmp);
                HttpResponseMessage response = await client.PostAsync("http://localhost:61505/api/Register", new StringContent(content, Encoding.UTF8, "application/json"));
            }
        }
        
    }
}
