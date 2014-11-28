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
    class ErrorLogsVM : ObservableObject, IPage
    {
        public ErrorLogsVM()
        {
            GetErrorLogs();
        }


        public string Name
        {
            get { return "ErrorLogs"; }
        }

        private ObservableCollection<ErrorLog> _obj;
        public ObservableCollection<ErrorLog> ErrorLogs
        {
            get { return _obj; }
            set { _obj = value; OnPropertyChanged("ErrorLogs"); }
        }

        private async void GetErrorLogs()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:49321/api/ErrorLog");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    ErrorLogs = JsonConvert.DeserializeObject<ObservableCollection<ErrorLog>>(json);
                }
            }
        }

    }
}
