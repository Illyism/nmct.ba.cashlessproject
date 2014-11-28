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
    class SalesVM : ObservableObject, IPage
    {
        public SalesVM()
        {
            GetSales();
        }


        public string Name
        {
            get { return "Sales"; }
        }

        private ObservableCollection<Sale> _obj;
        public ObservableCollection<Sale> Sales
        {
            get { return _obj; }
            set { _obj = value; OnPropertyChanged("Sales"); }
        }

        private async void GetSales()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:49321/api/Sale");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Sales = JsonConvert.DeserializeObject<ObservableCollection<Sale>>(json);
                }
            }
        }

    }
}
