using System;

namespace Dogs.Data.DataTransferObjects.Enums
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
