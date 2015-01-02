using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.classlibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Input;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    class EmployeesVM : ObservableObject, IPage
    {
        public EmployeesVM()
        {
            GetEmployees();
        }

        public ApplicationVM applicationVM
        {
            get
            {
                return App.Current.MainWindow.DataContext as ApplicationVM;
            }
            set
            {
                OnPropertyChanged("applicationVM");
            }
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
            Console.WriteLine("Getting Employees");
            if (ApplicationVM.token == null) return;
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:61505/api/Employee");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Employees = JsonConvert.DeserializeObject<ObservableCollection<Employee>>(json);
                }
                else
                {
                    Employees = null;
                }
            }
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged("SelectedEmployee");
                DeleteEmployeeCommand.RaiseCanExecuteChanged();
                SaveEmployeeCommand.RaiseCanExecuteChanged();
            }
        }

        private bool IsEmployeeSelected()
        {
            return SelectedEmployee != null;
        }


        public ICommand AddEmployeeCommand
        {
            get { return new RelayCommand(AddEmployee); }
        }

        private void AddEmployee()
        {
            if (SelectedEmployee != null && SelectedEmployee.EmployeeName == "<New Employee>") return;
            Employee NewEmployee = new Employee { ID = -1, EmployeeName = "<New Employee>" };
            Employees.Add(NewEmployee);
            SelectedEmployee = NewEmployee;
        }

        private RelayCommand _deleteEmployeeCommand;
        public RelayCommand DeleteEmployeeCommand
        {
            get
            {
                if (_deleteEmployeeCommand == null)
                    _deleteEmployeeCommand = new RelayCommand(DeleteEmployee, IsEmployeeSelected);
                return _deleteEmployeeCommand;
            }
        }


        private async void DeleteEmployee()
        {
            if (SelectedEmployee != null && SelectedEmployee.ID == -1) { Employees.Remove(SelectedEmployee); return; }

            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.DeleteAsync("http://localhost:61505/api/Employee/" + SelectedEmployee.ID);
                if (response.IsSuccessStatusCode)
                {
                    Employees.Remove(SelectedEmployee);
                }

            }
        }

        private RelayCommand _saveEmployeeCommand;
        public RelayCommand SaveEmployeeCommand
        {
            get
            {
                if (_saveEmployeeCommand == null)
                    _saveEmployeeCommand = new RelayCommand(SaveEmployee, IsEmployeeSelected);
                return _saveEmployeeCommand;
            }
        }


        private async void SaveEmployee()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                string content = JsonConvert.SerializeObject(SelectedEmployee);
                if (SelectedEmployee.ID == -1)
                {
                    HttpResponseMessage response = await client.PostAsync("http://localhost:61505/api/Employee", new StringContent(content, Encoding.UTF8, "application/json"));
                }
                else
                {
                    HttpResponseMessage response = await client.PutAsync("http://localhost:61505/api/Employee", new StringContent(content, Encoding.UTF8, "application/json"));
                }
                GetEmployees();
            }
        }
        
    }
}
