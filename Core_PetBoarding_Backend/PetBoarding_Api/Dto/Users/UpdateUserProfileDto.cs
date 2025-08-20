namespace PetBoarding_Api.Dto.Users
{
    public record UpdateUserProfileDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string PhoneNumber { get; set; }
        public AddressDto? Address { get; set; }

        public UpdateUserProfileDto(string firstname, string lastname, string phoneNumber, AddressDto? address = null)
        {
            Firstname = firstname;
            Lastname = lastname;
            PhoneNumber = phoneNumber;
            Address = address;
        }
    }
}
