using System;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.Web.Data.Mock
{
    public class Person
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }
      
        public string Occupation { get; set; }

        public string Email { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string FullName
        {
            get {
                return FirstName + " " + MiddleName + " " + LastName;
            }
        }

        public string UserName
        {
            get {
                return FullName.Replace(" ", "_").ToLower();
            }
        }
    }
}
