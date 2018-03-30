using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace G.Extensions
{
    public static class Calculate
    {
        public static bool BestMatch(Type t, Dictionary<string, string> obj)
        {
            var instance = Activator.CreateInstance(t).GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            if (instance != null)
            {
                foreach (var objItem in obj)
                {
                    if (!instance.ToList().Exists(x => x.Name.Equals(objItem.Key)))
                        return false;
                }
            }
            return true;
        }

        public static Decimal NonNullPropertiesPercentage(Type t, Dictionary<string, string> obj)
        {
            // create an instance of the object being passed in
            var instance = Activator.CreateInstance(t).GetType().GetProperties();
            // compare the key values of the Type instance to the keys of the actual object being passed in
            int matchingKeyCount = 0;
            int instanceTotalKeyCount = instance.Count();
            if (instance != null)
            {
                foreach (var pair in instance)
                {
                    if (obj.ContainsKey(pair.Name))
                    {
                        matchingKeyCount++;
                    }
                }
            }
            var percent = Decimal.Divide(matchingKeyCount, instanceTotalKeyCount) * 100;
            return percent;
        }

        private static double? CharacterFrequncy(string[] deconstructedQuery, char character)
        {
            int commaCount = 0;
            foreach (var s in deconstructedQuery)
            {
                if (s.Contains(character))
                {
                    commaCount++;
                }
            }
            if (commaCount > 0)
            {
                return (double)commaCount / deconstructedQuery.Count();
            }
            return null;
        }
    }
}