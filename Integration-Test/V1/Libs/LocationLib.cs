using System.Text.Json.Nodes;

namespace Integration_Test.V1.Libs
{
    public class LocationLib
    {

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

    }
}
