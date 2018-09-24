using System;
using System.Configuration;
using System.Linq;

namespace RestApiCountriesTests.TestUtils
{
    public static class TestSettings
    {
        public static string BaseUrl => ConfigurationManager.AppSettings["baseUrl"];

        public static string RootProjectDirectory => GetRootProjectDirectory();

        private static string GetRootProjectDirectory()
        {
            string fullPath = AppDomain.CurrentDomain.BaseDirectory;
            string[] pathSplitted = fullPath.Split('\\');
            var path = pathSplitted.Take(pathSplitted.Count() - 3).ToArray();
            return string.Join("\\", path);
        }

    }
}
