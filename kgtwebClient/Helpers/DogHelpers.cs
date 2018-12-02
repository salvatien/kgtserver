using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using kgtwebClient.Models;

namespace kgtwebClient.Helpers
{
    public class DogHelpers
    {
        static public bool ValidateUpdateDog(DogModel updatedDog)
        {
            if (!ValidateDogName(updatedDog.Name) || !ValidateDogDateOfBirth(updatedDog.DateOfBirth))
                return false;           
            return true;
        }
        
        static private bool ValidateDogName(string name)
        {
            if (!Regex.IsMatch(name, @"^[a-zA-Z]+$"))
                return false;
            return true;
        }

        static private bool ValidateDogDateOfBirth(DateTime date)
        {
            if (date > DateTime.Now)
                return false;
            return true;
        }
    }
}