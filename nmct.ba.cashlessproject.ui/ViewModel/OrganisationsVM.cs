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
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:49321/api/Organisation");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Organisations = JsonConvert.DeserializeObject<ObservableCollection<Organisation>>(json);
                }
            }
        }

    }
}
