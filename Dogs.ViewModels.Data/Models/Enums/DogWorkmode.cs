using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogs.ViewModels.Data.Models.Enums
{
    [Flags]
    public enum DogWorkmode : int
    {
        Tropiący = 1, //tropiacy
        Terenowy = 2, //terenowy
        Ratowniczy = 4, //ratowniczy
        Gruzowy = 8, //gruzowy
        Wodny = 16 //wodny
    }
}
