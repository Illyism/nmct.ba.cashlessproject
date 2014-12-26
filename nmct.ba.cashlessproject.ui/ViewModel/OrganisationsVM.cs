using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.classlibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    class OrganisationsVM : ObservableObject, IPage
    {
        public OrganisationsVM()
        {
            GetOrganisations();
        }


        public string Name
        {
            get { return "Organisations"; }
        }

        private ObservableCollection<Organisation> _obj;
        public ObservableCollection<Organisation> Organisations
        {
            get { return _obj; }
            set { _obj = value; OnPropertyChanged("Organisations"); }
        }

        private async void GetOrganisations()
        {
            Console.WriteLine("Getting Organisations");
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:61505/api/Organisation");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Organisations = JsonConvert.DeserializeObject<ObservableCollection<Organisation>>(json);
                }
            }
        }
    }
}
