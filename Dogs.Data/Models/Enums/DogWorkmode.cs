using System;

namespace Dogs.Data.Models.Enums
{
    //public enum DogWorkmode
    //{
    //    Tracking, //tropiacy
    //    Terrain, //terenowy
    //    Rescue, //ratowniczy
    //    Rubble, //gruzowy
    //    Water //wodny
    //}
    [Flags]
    [System.Text.Json.Serialization.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum DogWorkmode : int
    {
        Tracking = 1, //tropiacy
        Terrain = 2, //terenowy
        Rescue = 4, //ratowniczy
        Rubble = 8, //gruzowy
        Water = 16 //wodny
    };
}
