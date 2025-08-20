namespace PetBoarding_Api.Dto.Users
{
    public record AddressDto
    {
        public string StreetNumber { get; set; } = string.Empty;
        public string StreetName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? Complement { get; set; }

        public AddressDto() { }

        public AddressDto(string streetNumber, string streetName, string city, string postalCode, string country, string? complement = null)
        {
            StreetNumber = streetNumber;
            StreetName = streetName;
            City = city;
            PostalCode = postalCode;
            Country = country;
            Complement = complement;
        }
    }
}
