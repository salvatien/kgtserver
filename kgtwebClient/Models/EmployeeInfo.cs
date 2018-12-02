using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kgtwebClient.Models
{
    public class EmployeeInfo
    {
        public int EmployeeID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string City { get; set; }
    }

    public class ListOfEmployeeInfo
    {
        public List<EmployeeInfo> listOfEmployee { get; set; }
    }



}
