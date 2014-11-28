using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.classlibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    class EmployeesVM : ObservableObject, IPage
    {
        public EmployeesVM()
        {
            GetEmployees();
        }


        public string Name
        {
            get { return "Employees"; }
        }

        private ObservableCollection<Employee> _obj;
        public ObservableCollection<Employee> Employees
        {
            get { return _obj; }
            set { _obj = value; OnPropertyChanged("Employees"); }
        }

        private async void GetEmployees()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:49321/api/Employee");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Employees = JsonConvert.DeserializeObject<ObservableCollection<Employee>>(json);
                }
            }
        }

    }
}
