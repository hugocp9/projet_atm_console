using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Atmv1.lib.utils
{
    public static class Validator
    {
        
        public static T Convert<T>(this string input)
        {
            bool valid = false;
            string rawInput;

            while (!valid)
            {
                rawInput = Utility.GetRawInput(input);

                try
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null)
                        
                        return (T)converter.ConvertFromString(rawInput);

                    return default;
                }
                catch
                {
                    Utility.PrintMessage("Invalid input. Try again.", false);
                }
            }
            return default;
        }
    }
}