using Flurl;
using Newtonsoft.Json;
using RestApiCountriesTests.Models;
using System.IO;


namespace RestApiCountriesTests.TestUtils
{
    public static class TestHelpers
    {
        /// <summary>
        /// Converts json to CountryTestModel. 
        /// </summary>
        /// <param name="jsonFileName">Json file name placed in TestData folder</param>
        /// <returns></returns>
        public static CountryTestModel ConvertJsonToCountryTestModel(string jsonFileName)
        {
            string filePath = TestSettings.RootProjectDirectory + "\\TestData\\" + jsonFileName;
            CountryTestModel expectedCountry;

            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                expectedCountry = JsonConvert.DeserializeObject<CountryTestModel>(json);
            }
            return expectedCountry;
        }

        public static string GetCountryByNameUrlBuilder(string countryName)
        {
            return TestSettings.BaseUrl.AppendPathSegment("name").AppendPathSegment(countryName);
        }
    }
}
