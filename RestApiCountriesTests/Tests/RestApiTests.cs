using FluentAssertions;
using Flurl;
using Flurl.Http;
using NUnit.Framework;
using RestApiCountriesTests.Models;
using RestApiCountriesTests.TestUtils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiCountriesTests   
{
    [TestFixture]
    public class RestApiTests
    {
        private string ukraineResponseJsonName = "ExpectedResponseUkraine.json";
        private string ukraineResponseWithThreeFieldsOnlyJsonName = "ExpectedResponseUkraineWithThreeFieldsOnly.json";

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        [TestCase("Ukraine", "Ukrainian hryvnia")]
        public async Task  RequestCountryByName_CountryCurrencyIsEqualsToExpected(string countryName, string expectedCurrency)
        {
            string url = TestHelpers.GetCountryByNameUrlBuilder(countryName);

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
            var url = TestHelpers.GetCountryByNameUrlBuilder(countryName);
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
            string url = TestHelpers.GetCountryByNameUrlBuilder(countryName);

            var response = await url.GetJsonAsync<List<CountryTestModel>>();
            CountryTestModel actualUkraine = response.First();
            CountryTestModel expectedUkraine = TestHelpers.ConvertJsonToCountryTestModel(ukraineResponseJsonName);

            actualUkraine.Should().BeEquivalentTo(expectedUkraine);
        }

        [Test]
        [Category("Regression")]
        [TestCase("Ukraine")]
        public async Task RequestCountryByNameAndFilterByFields_OnlyFilteredFieldsShouldBeReturned(string countryName)
        {
            string url =
                TestHelpers.GetCountryByNameUrlBuilder(countryName).SetQueryParam("fields", "name;capital;currencies");

            var response = await url.GetJsonAsync<List<CountryTestModel>>();
            CountryTestModel actualUkraine = response.First();
            CountryTestModel expectedUkraine = TestHelpers.ConvertJsonToCountryTestModel(ukraineResponseWithThreeFieldsOnlyJsonName);

            actualUkraine.Should().BeEquivalentTo(expectedUkraine);
        }       
    }
}
