using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using NUnit.Framework;
using RestApiCountriesTests.Models;
using RestApiCountriesTests.TestUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiCountriesTests
{
    [TestFixture]
    public class RestApiTests
    {
        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        [TestCase("Ukraine", "Ukrainian hryvnia")]
        public async Task RequestCountryByName_CountryCurrencyIsEqualsToExpected(string countryName, string expectedCurrency)
        {
            string url = TestSettings.BaseUrl.AppendPathSegment("name").AppendPathSegment(countryName);

            var getResponseStatus = await url.AllowAnyHttpStatus().GetAsync();
            var response = await url.GetJsonAsync<List<CountryTestModel>>();
            List<string> actualCurrencies = response.First().currencies.Select(o => o.name).ToList();

            var expectedCurrencies = new List<string>()
            {
               expectedCurrency
            };

            Assert.AreEqual(200, (int)getResponseStatus.StatusCode);
            Assert.AreEqual(expectedCurrencies, actualCurrencies);
        }

        [Test]
        [Category("Regression")]
        [TestCase("Wakanda", 404)]
        public async Task RequestCountryByName_CountryNameDoesNotExists_Error404ShouldBEReturned(string countryName, int expectedError)
        {
            var url = TestSettings.BaseUrl.AppendPathSegment("name").AppendPathSegment(countryName);
            var getResponse = await url.AllowAnyHttpStatus().GetAsync();

            Assert.AreEqual(expectedError, (int)getResponse.StatusCode);

        }

        [Test]
        [Category("Regression")]
        [TestCase("Oceania", "American Samoa", "USD")]
        public async Task GetResponseResultAndMakeSecondRequestBasedOnResultValue(string searchRegion, string countryName, string expectedCurrency)
        {
            var getAllCountriesUrl = TestSettings.BaseUrl.AppendPathSegment("region").AppendPathSegment(searchRegion);
            var getAllCountries = await getAllCountriesUrl.GetJsonAsync<List<CountryTestModel>>();

            CountryTestModel americanSamoa = getAllCountries.First(c => c.name == countryName);
            Assert.IsNotNull(americanSamoa);

            string url = TestSettings.BaseUrl.AppendPathSegment("name").AppendPathSegment(americanSamoa.name);
            var response = await url.GetJsonAsync<List<CountryTestModel>>();
            List<Currency> currencies = response.First().currencies;

            Assert.That(response.Count, Is.EqualTo(1));
            Assert.Contains(expectedCurrency, currencies.Select(c => c.code).ToList());
        }

        [Test]
        [Category("Regression")]
        [TestCase("Ukraine")]
        public async Task RequestCountryByName_AllReturnedPropertiesAreEqualsToExpected(string countryName)
        {
            string url = TestSettings.BaseUrl.AppendPathSegment("name").AppendPathSegment(countryName);

            var response = await url.GetJsonAsync<List<CountryTestModel>>();
            CountryTestModel ukraine = response.First();

            //TODO: Investigate this method implementation
            AssertObjectFieldPropertiesAreEqual(GetExpectedJsonResponseForUkraine(), ukraine);
        }

        private CountryTestModel GetExpectedJsonResponseForUkraine()
        {
            string filePath = TestSettings.RootProjectDirectory + "\\TestData\\ExpectedResponseUkraine.json";
            CountryTestModel expectedCountry;

            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                expectedCountry = JsonConvert.DeserializeObject<CountryTestModel>(json);
            }
            return expectedCountry;
        }

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
