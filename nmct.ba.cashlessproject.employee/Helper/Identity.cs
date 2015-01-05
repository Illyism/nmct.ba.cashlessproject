using be.belgium.eid;
using nmct.ba.cashlessproject.classlibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.employee.Helper
{
    public class Identity {

        private static BEID_ReaderContext _reader;
        public static BEID_ReaderContext Reader
        {
            get
            {
                if (_reader == null)
                {
                    BEID_ReaderSet readerSet = BEID_ReaderSet.instance();
                    _reader = readerSet.getReader();
                }
                return _reader;
            }
        }

        public static EIDPerson PersonFromCardReader()
        {
            EIDPerson person = null;

            if (Reader.isCardPresent())
            {
                BEID_EIDCard card = Reader.getEIDCard();

                BEID_EId doc = card.getID();
                person = new EIDPerson
                {
                    FirstName = doc.getFirstName(),
                    Surname = doc.getSurname(),
                    Birthday = doc.getDateOfBirth(),
                    Gender = doc.getGender(),
                    Nationality = doc.getNationality(),
                    NationalNumber = doc.getNationalNumber(),
                    Street = doc.getStreet(),
                    Muncipality = doc.getMunicipality(),
                    Country = doc.getCountry(),
                    Zip = doc.getZipCode(),
                    Picture = card.getPicture().getData().GetBytes()
                };
            }
            return person;            
        }

        public static Employee EmployeeFromCardReader()
        {
            EIDPerson cardPerson = PersonFromCardReader();
            Employee person = new Employee {
                EmployeeName = cardPerson.FirstName + " " + cardPerson.Surname,
                Address = cardPerson.Street + ", " + cardPerson.Zip + " " + cardPerson.Muncipality + ", " + cardPerson.Country,
                NationalNumber = cardPerson.NationalNumber
            };
            return person;
        }

        internal static Customer CustomerFromCardReader()
        {
            EIDPerson cardPerson = PersonFromCardReader();
            Customer person = new Customer
            {
                CustomerName = cardPerson.FirstName + " " + cardPerson.Surname,
                Address = cardPerson.Street + ", " + cardPerson.Zip + " " + cardPerson.Muncipality + ", " + cardPerson.Country,
                NationalNumber = cardPerson.NationalNumber
            };
            return person;
        }
    }
}
