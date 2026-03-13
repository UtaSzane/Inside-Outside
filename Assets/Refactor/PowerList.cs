using System.Collections.Generic;

#pragma warning disable IDE0130
namespace Test {
#pragma warning restore IDE0130

    public static class PowerList {
        private readonly static List<PowerClass> powers = new(); 
        public static int Count => powers.Count;
        public static PowerClass[] List()
        {
            return powers.ToArray();
        }
        public static void Init()
        {
            powers.Clear();
        }
        public static void Clear()
        {
            powers.Clear();
        }
        public static void AddPower(PowerClass pwr)
        {
            powers.Add(pwr);
        }
        public static bool RemovePower(PowerClass pwr)
        {
            if (!powers.Contains(pwr)) return false;
            powers.Remove(pwr);
            return true;
        }
    }
}