using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogs.ViewModels.Data.Models.Enums
{
    [Flags]
    public enum DogWorkmode : int
    {
        Tracking = 1, //tropiacy
        Terrain = 2, //terenowy
        Rescue = 4, //ratowniczy
        Rubble = 8, //gruzowy
        Water = 16 //wodny
    }
}
