using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kgtwebClient.Models.Enums
{
    [Flags]
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum DogWorkmode : int
    {
        Tracking = 1, //tropiacy
        Terrain = 2, //terenowy
        Rescue = 4, //ratowniczy
        Rubble = 8, //gruzowy
        Water = 16 //wodny
    }
}
