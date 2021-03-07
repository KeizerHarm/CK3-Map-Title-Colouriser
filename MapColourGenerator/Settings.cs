using System;
using System.Collections.Generic;
using System.Linq;

namespace MapColourGenerator
{
    class Settings
    {
        public static int Kingdom_Min_Distance = 10;
        public static int Kingdom_Max_Distance = 20;
        public static int Duchy_Min_Distance = 8;
        public static int Duchy_Max_Distance = 12;
        public static int County_Min_Distance = 3;
        public static int County_Max_Distance = 8;
        public static int Number_Of_Tries = 5000;

        public static bool UseCustomOutputLocation = false;
        public static string CustomOutputLocation;

        private static readonly string Kingdom_Min_Distance_Key = "Kingdom_Min_Distance";
        private static readonly string Kingdom_Max_Distance_Key = "Kingdom_Max_Distance";
        private static readonly string Duchy_Min_Distance_Key = "Duchy_Min_Distance";
        private static readonly string Duchy_Max_Distance_Key = "Duchy_Max_Distance";
        private static readonly string County_Min_Distance_Key = "County_Min_Distance";
        private static readonly string County_Max_Distance_Key = "County_Max_Distance";
        private static readonly string Number_Of_Tries_Key = "Number_Of_Tries";
        private static readonly string UseCustomOutputLocation_Key = "UseCustomOutputLocation";
        private static readonly string CustomOutputLocation_Key = "CustomOutputLocation";

        private static Dictionary<string, string> SettingsDictionary;

        public static void SetSettings(Dictionary<string, string> dict, ref string msg, ref bool @continue)
        {
            SettingsDictionary = dict;

            if (!TrySetIntValue(dict, ref Kingdom_Min_Distance, Kingdom_Min_Distance_Key, ref msg))
            {
                @continue = false;
            }
            if (!TrySetIntValue(dict, ref Kingdom_Max_Distance, Kingdom_Max_Distance_Key, ref msg))
            {
                @continue = false;
            }
            if (!TrySetIntValue(dict, ref Duchy_Min_Distance, Duchy_Min_Distance_Key, ref msg))
            {
                @continue = false;
            }
            if (!TrySetIntValue(dict, ref Duchy_Max_Distance, Duchy_Max_Distance_Key, ref msg))
            {
                @continue = false;
            }
            if (!TrySetIntValue(dict, ref County_Min_Distance, County_Min_Distance_Key, ref msg))
            {
                @continue = false;
            }
            if (!TrySetIntValue(dict, ref County_Max_Distance, County_Max_Distance_Key, ref msg))
            {
                @continue = false;
            }
            if (!TrySetIntValue(dict, ref Number_Of_Tries, Number_Of_Tries_Key, ref msg))
            {
                @continue = false;
            }

            string useCustomOutputString = dict.GetValueOrDefault(UseCustomOutputLocation_Key);
            UseCustomOutputLocation = useCustomOutputString == "yes";

            CustomOutputLocation = dict.GetValueOrDefault(CustomOutputLocation_Key);
        }

        private static bool TrySetIntValue(Dictionary<string, string> dict, ref int valueToSet, string key, ref string msg)
        {
            if(!dict.TryGetValue(key, out string stringValue))
            {
                msg += $"ERROR: Missing setting: {key}\n";
                return false;
            }
            if(!int.TryParse(stringValue, out valueToSet))
            {
                msg += $"ERROR: Setting {key} has value '{stringValue}' which cannot be parsed as an integer.\n";
                return false;
            }
            return true;
        }

        public new static string ToString()
        {
            return string.Join('\n', SettingsDictionary.Select(x => $"{x.Key} = {x.Value}"));
        }
    }
}
