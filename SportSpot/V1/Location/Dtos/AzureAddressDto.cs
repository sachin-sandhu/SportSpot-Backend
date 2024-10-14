namespace SportSpot.V1.Location
{
    public class AzureAddressDto
    {
        public string? StreetNumber { get; set; }
        public string? StreetName { get; set; }
        public string? MunicipalitySubdivision { get; set; }
        public string? Municipality { get; set; }
        public string? CountrySecondarySubdivision { get; set; }
        public string? CountryTertiarySubdivision { get; set; }
        public string? CountrySubdivision { get; set; }
        public string? CountrySubdivisionName { get; set; }
        public string? PostalCode { get; set; }
        public string? ExtendedPostalCode { get; set; }
        public string? CountryCode { get; set; }
        public string? Country { get; set; }
        public string? CountryCodeISO3 { get; set; }
        public string? FreeformAddress { get; set; }
    }
}
