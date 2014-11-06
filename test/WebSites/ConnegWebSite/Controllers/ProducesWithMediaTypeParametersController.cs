using System;
using ConnegWebsite.Models;
using Microsoft.AspNet.Mvc;

namespace ConnegWebsite
{
    public class ProducesWithMediaTypeParametersController : Controller
    {
        [Produces("text/vcard;VERSION=V3.0")]
        public Contact ContactInfoUsingV3Format()
        {
            return new Contact()
            {
                Name = "John Williams",
                Gender = GenderType.Male
            };
        }

        [Produces("text/vcard;VERSION=V4.0")]
        public Contact ContactInfoUsingV4Format()
        {
            return new Contact()
            {
                Name = "John Williams",
                Gender = GenderType.Male
            };
        }
    }
}