using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.classlibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Data;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    class EmployeeRegister : Employee
    {
        public EmployeeRegister(Employee e)
        {
            this.EmployeeName = e.EmployeeName;
            this.ID = e.ID;
            this.Address = e.Address;
            this.Phone = e.Phone;
            this.Email = e.Email;
        }

        public List<RegisterEmployee> RegisterEmployees { get; set; }

        public static ObservableCollection<EmployeeRegister> FromRegisterEmployeeList(List<RegisterEmployee> regEmpList)
        {
            ObservableCollection<EmployeeRegister> result = new ObservableCollection<EmployeeRegister>();

            var linq = from regEmp in regEmpList
                        group regEmp by regEmp.Employee.EmployeeName into g
                        select new { EmployeeName = g.Key, RegisterEmployees = g };

            foreach (var item in linq)
            {
                List<RegisterEmployee> regEmps = item.RegisterEmployees.ToList() as List<RegisterEmployee>;
                EmployeeRegister empReg = new EmployeeRegister((regEmps)[0].Employee);
                empReg.RegisterEmployees = regEmps;
                result.Add(empReg);
            }
            return result;
        }
    }

    class RegistersVM : ObservableObject, IPage
    {
        public RegistersVM()
        {
            GetRegisters();
        }

        public string Name
        {
            get { return "Registers"; }
        }

        private ObservableCollection<Register> _reg;
        public ObservableCollection<Register> Registers
        {
            get { return _reg; }
            set { _reg = value; OnPropertyChanged("Registers"); GetRegistersEmployees(); }
        }

        //private ObservableCollection<RegisterEmployee> _regemp;
        //public ObservableCollection<RegisterEmployee> RegisterEmployees
        //{
        //    get { return _regemp; }
        //    set { _regemp = value; OnPropertyChanged("RegisterEmployees"); }
        //}

        private ObservableCollection<EmployeeRegister> _empreg;
        public ObservableCollection<EmployeeRegister> EmployeeRegisters
        {
            get { return _empreg; }
            set { _empreg = value; OnPropertyChanged("EmployeeRegisters"); }
        }

        private async void GetRegisters()
        {
            Console.WriteLine("Getting Registers");
            if (ApplicationVM.token == null) return;
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:61505/api/Register");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Registers = JsonConvert.DeserializeObject<ObservableCollection<Register>>(json);
                } else {
                    Registers = null;
                }
            }
        }

        private async void GetRegistersEmployees()
        {
            if (ApplicationVM.token == null || SelectedRegister == null) return;
            Console.WriteLine("Getting Register Employees");
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:61505/api/Register/" + SelectedRegister.ID);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<RegisterEmployee> list = JsonConvert.DeserializeObject<List<RegisterEmployee>>(json);
                    EmployeeRegisters = EmployeeRegister.FromRegisterEmployeeList(list);
                }
                else
                {
                    EmployeeRegisters = null;
                }
            }
        }

        private Register _selectedRegister;
        public Register SelectedRegister
        {
            get { return _selectedRegister; }
            set
            {
                _selectedRegister = value;
                OnPropertyChanged("SelectedRegister");
            }
        }

        private bool IsRegisterSelected()
        {
            return SelectedRegister != null;
        }

    }
}
