using System;

namespace ConnegWebsite.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string Name { get; set; }
        public GenderType Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string Twitter { get; set; }
        public string Self { get; set; }
    }

    public enum GenderType
    {
        Male,
        Female
    }
}