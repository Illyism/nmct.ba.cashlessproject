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

        private ObservableCollection<Register> _obj;
        public ObservableCollection<Register> Registers
        {
            get { return _obj; }
            set { _obj = value; OnPropertyChanged("Registers"); }
        }

        private async void GetRegisters()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:49321/api/Register");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Registers = JsonConvert.DeserializeObject<ObservableCollection<Register>>(json);
                }
            }
        }

    }
}
