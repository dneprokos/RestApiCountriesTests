using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestApiCountriesTests.TestUtils
{
    //TODO: Clarify why list properties fail assertion and fix this problem
    public static class AssertHelpers
    {
        public static void AssertObjectFieldPropertiesAreEqual<T>(T expected, T actual)
        {
            var failures = new List<string>();
            var properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var property in properties)
            {
                var v1 = property.GetValue(expected);
                var v2 = property.GetValue(actual);
                if (v1 == null && v2 == null) continue;
                if (!v1.Equals(v2)) failures.Add(string.Format("{0}: Expected:<{1}> Actual:<{2}>", property.Name, v1, v2));
            }
            if (failures.Any())
                Assert.Fail("Following object properties are different. " + Environment.NewLine + string.Join(Environment.NewLine, failures));
        }
    }
}
