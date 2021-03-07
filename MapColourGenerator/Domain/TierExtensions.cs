using System;

namespace MapColourGenerator.Domain
{
    public static class TierExtensions
    {
        public static int GetMinDistance(this Tier tier)
        {
            return tier switch
            {
                Tier.Kingdom => Settings.Kingdom_Min_Distance,
                Tier.Duchy => Settings.Duchy_Min_Distance,
                Tier.County => Settings.County_Min_Distance,
                _ => 0,
            };
        }

        public static int GetMaxDistance(this Tier tier)
        {
            return tier switch
            {
                Tier.Kingdom => Settings.Kingdom_Max_Distance,
                Tier.Duchy => Settings.Duchy_Max_Distance,
                Tier.County => Settings.County_Max_Distance,
                _ => 100,
            };
        }

        public static readonly string[] TitlePrefixes = { "e_", "k_", "d_", "c_" };

        public static Tier TierFromName(string name)
        {
            if (name.StartsWith("e_"))
            {
                return Tier.Empire;
            }
            if (name.StartsWith("k_"))
            {
                return Tier.Kingdom;
            }
            if (name.StartsWith("d_"))
            {
                return Tier.Duchy;
            }
            if (name.StartsWith("c_"))
            {
                return Tier.County;
            }
            if (name.StartsWith("b_"))
            {
                return Tier.Barony;
            }
            throw new ArgumentException();
        }
    }
}
