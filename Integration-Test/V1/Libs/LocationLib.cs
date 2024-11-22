using NSubstitute.Core;
using System.Text.Json.Nodes;

namespace Integration_Test.V1.Libs
{
    public class LocationLib
    {

        private readonly HttpClient _client;

        public LocationLib(string baseUri)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUri)
            };
        }

        public async Task<JsonArray> SearchLocations(string accessToken, string query, string entityType = "ALL")
        {
            HttpResponseMessage response = await SearchLocationsWithResponse(accessToken, query, entityType);
            response.EnsureSuccessStatusCode();
            string responseString = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<JsonArray>(responseString);
        }

        public async Task<HttpResponseMessage> SearchLocationsWithResponse(string accessToken, string query, string entityType = "ALL")
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Get, $"location/search?SearchQuery={query}&EntityType={entityType}");
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            HttpResponseMessage response = await _client.SendAsync(requestMessage);
            return response;
        }

        public async Task<JsonObject> ReverseLocation(string accessToken, double latitude, double longitude)
        {
            HttpResponseMessage response = await ReverseLocationWithResponse(accessToken, latitude, longitude);
            response.EnsureSuccessStatusCode();
            string responseString = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<JsonObject>(responseString);
        }

        public async Task<HttpResponseMessage> ReverseLocationWithResponse(string accessToken, double latitude, double longitude)
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Get, $"location/reverse?latitude={latitude}&longitude={longitude}");
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            HttpResponseMessage response = await _client.SendAsync(requestMessage);
            return response;
        }

        public static JsonObject GetDefaultReverseResponse()
        {
            string response = """
                                {
                    "summary": {
                        "queryTime": 16,
                        "numResults": 1
                    },
                    "addresses": [
                        {
                            "address": {
                                "buildingNumber": "15",
                                "streetNumber": "15",
                                "routeNumbers": [],
                                "street": "Schuter",
                                "streetName": "Schuter",
                                "streetNameAndNumber": "Schuter 15",
                                "countryCode": "DE",
                                "countrySubdivision": "Nordrhein-Westfalen",
                                "countrySecondarySubdivision": "Warendorf",
                                "municipality": "Everswinkel",
                                "postalCode": "48351",
                                "country": "Deutschland",
                                "countryCodeISO3": "DEU",
                                "freeformAddress": "Schuter 15, 48351 Everswinkel",
                                "boundingBox": {
                                    "northEast": "51.903220,7.842525",
                                    "southWest": "51.902977,7.842498",
                                    "entity": "position"
                                },
                                "countrySubdivisionName": "Nordrhein-Westfalen",
                                "countrySubdivisionCode": "NW",
                                "localName": "Everswinkel"
                            },
                            "position": "51.903034,7.842505",
                            "id": "gjX1bE2Ldmo9j2go93n8YQ"
                        }
                    ]
                }
                """;
            return JsonSerializer.Deserialize<JsonObject>(response);
        }


        public static JsonObject GetDefaultSearchResult()
        {
            string response = """
                                {
                    "summary": {
                        "query": "schuter 15 everswinkel",
                        "queryType": "NON_NEAR",
                        "queryTime": 67,
                        "numResults": 1,
                        "offset": 0,
                        "totalResults": 1,
                        "fuzzyLevel": 1
                    },
                    "results": [
                        {
                            "type": "Point Address",
                            "id": "gjX1bE2Ldmo9j2go93n8YQ",
                            "score": 1,
                            "matchConfidence": {
                                "score": 1
                            },
                            "address": {
                                "streetNumber": "15",
                                "streetName": "Schuter",
                                "municipality": "Everswinkel",
                                "countrySecondarySubdivision": "Warendorf",
                                "countrySubdivision": "Nordrhein-Westfalen",
                                "countrySubdivisionName": "Nordrhein-Westfalen",
                                "countrySubdivisionCode": "NW",
                                "postalCode": "48351",
                                "countryCode": "DE",
                                "country": "Deutschland",
                                "countryCodeISO3": "DEU",
                                "freeformAddress": "Schuter 15, 48351 Everswinkel",
                                "localName": "Everswinkel"
                            },
                            "position": {
                                "lat": 51.90303,
                                "lon": 7.84269
                            },
                            "viewport": {
                                "topLeftPoint": {
                                    "lat": 51.90393,
                                    "lon": 7.84123
                                },
                                "btmRightPoint": {
                                    "lat": 51.90213,
                                    "lon": 7.84415
                                }
                            },
                            "entryPoints": [
                                {
                                    "type": "main",
                                    "position": {
                                        "lat": 51.90304,
                                        "lon": 7.8425
                                    }
                                }
                            ]
                        }
                    ]
                }
                """;
            return JsonSerializer.Deserialize<JsonObject>(response);
        }
    }
}
