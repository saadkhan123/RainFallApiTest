using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using TechTalk.SpecFlow;
using Xunit;

namespace RainfallApiTests.StepDefinitions
{
    [Binding]
    public class RainfallApiSteps
    {
        private RestClient _client;
        private RestResponse _response;
        private Root _responseData;


        [Given(@"the rainfall API is available")]
        public void GivenTheRainfallApiIsAvailable()
        {
            _client = new RestClient("http://environment.data.gov.uk/flood-monitoring");
        }

        [When(@"I request the rainfall measurements for station ""(.*)"" with a limit of (.*)")]
        public async Task WhenIRequestTheRainfallMeasurementsForStationWithALimitOf(string stationId, int limit)
        {
            var request = new RestRequest($"/id/measures/{stationId}-rainfall-tipping_bucket_raingauge-t-15_min-mm/readings", Method.Get);
            request.AddParameter("parameter", "rainfall");
            request.AddParameter("since", "2016-09-07T15:00:00Z");
            request.AddParameter("_limit", limit);
            _response = await _client.ExecuteAsync(request);
            // _responseData = JArray.Parse(_response.Content);
            _responseData = JsonConvert.DeserializeObject<Root>(_response.Content);

        }

        [When(@"I request the rainfall measurements for station ""(.*)"" on ""(.*)""")]
        public async Task WhenIRequestTheRainfallMeasurementsForStationOn(string stationId, string date)
        {
            
            var request = new RestRequest($"/id/stations/{stationId}/readings", Method.Get);
            request.AddParameter("date", date);

            _response = await _client.ExecuteAsync(request);
            _responseData = JsonConvert.DeserializeObject<Root>(_response.Content);
        }

        [Then(@"I should receive (.*) measurements")]
        public void ThenIShouldReceiveOrFewerMeasurements(int limit)
        {
            Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            Assert.True(_responseData?.items.Count == limit);
        }


        [Then(@"I should receive measurements only for ""(.*)""")]
        public void ThenIShouldReceiveMeasurementsOnlyFor(string date)
        {
            if (_responseData?.items.Count > 0)
            {
                foreach (var measurement in _responseData?.items)
                {
                    var measurementDate = DateTime.Parse(measurement.dateTime.ToString("yyyy/MM/dd"));
                    Assert.Equal(DateTime.Parse(date), measurementDate.Date);
                }
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);

            }
            else
            {
                Assert.True(_responseData?.items.Count > 0);
            }
        }

    }
}
