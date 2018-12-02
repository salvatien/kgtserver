using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kgtwebClient.Helpers
{
    public class EnumHelpers
    {
        public static IEnumerable<Enum> GetFlags(Enum input)
        {
            if (input == null)
                return new List<Enum>();
            else return GetFlagsInternal(input);

        }

        private static IEnumerable<Enum> GetFlagsInternal(Enum input) {

            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }
    }
}