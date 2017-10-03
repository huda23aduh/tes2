using PayMedia.ApplicationServices.ClientProxy;
using PayMedia.ApplicationServices.Customers.ServiceContracts;
using PayMedia.ApplicationServices.Customers.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tes2_huda
{
    class Huda_validAddress_class
    {
        public void createValidAddress()
        {
            //Create the authentication header, initialize with credentials, and
            //create the service proxy. For details, see the API Developer's
            //Guide and the Reference Library CHM.
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
            var customersConfiguration = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersConfigurationService>(ah);
            //Instanstiate and initialize a ValidAddress object.
            //For a description of each property, see the API Reference Library CHM.
            ValidAddress validAddressParams = new ValidAddress();
            validAddressParams.Active = true;
            validAddressParams.BigCity = "zzzzzzzz";
            validAddressParams.CountryId = 1;


            validAddressParams.CreateDateTime = System.DateTime.Now;
            //To populate a CustomField, instantiate a collection, and...
            validAddressParams.CustomFields = new CustomFieldValueCollection();
            //...note that you must use the string value of the custom field's name
            //to add an instance to the collection.
            //validAddressParams.CustomFields.Add(new CustomFieldValue ("Number of Active Outlets", "12"));
            validAddressParams.ExternalAddressId = 2929134;
            validAddressParams.SmallCity = "qqqqqqqq";
            validAddressParams.ProvinceId = 2;
            //validAddressParams.ExternalReference = "MOR";
            //validAddressParams.HouseNumberNumeric = 2982;
            validAddressParams.ManuallyAdded = false;
            validAddressParams.PostalCode = "666666";
            validAddressParams.Street = "wwwwwww";
            validAddressParams.UniqueAddress = false;
            //Call the method and display the results.
            ValidAddress validAddress = customersConfiguration.CreateValidAddress(validAddressParams);
            Console.WriteLine("Valid address created. ID = {0}", validAddress.Id);
            Console.ReadLine();
        }

        public void getValidAddress()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            var customersService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersService>(authHeader);
            var valid_address_Service = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersConfigurationService>(authHeader);

            #region Find customer by phone number
            //Instantiate a BaseQueryRequest for your filter criteria.
            BaseQueryRequest request = new BaseQueryRequest();


            request.FilterCriteria = Op.Like("CountryId", "1");
            //request.FilterCriteria = Op.Like("ProvinceId", "1");
            request.FilterCriteria = Op.Like("PostalCode", "24410000");


            ValidAddressCollection val_address = valid_address_Service.GetValidAddresses(request);

            if (val_address != null)
            {
                foreach (ValidAddress c in val_address)
                {
                    Console.WriteLine("Found valid address ID {0}, countryId = {1} - provinceId = {2} -  bigcity = {3} -  smallcity = {4} - postalcode = {5} - street = {6}", c.Id, c.CountryId, c.ProvinceId, c.BigCity, c.SmallCity, c.PostalCode, c.Street);
                }
            }
            else
            {
                Console.WriteLine("Cannot find a customer with that phone number.");
            }
            Console.WriteLine("total customer = {0} ----", val_address.Count());
            //You will need to enter some error handling in case there is more than
            //one matching customer.
            Console.ReadLine();
            #endregion
        }

    }
}
