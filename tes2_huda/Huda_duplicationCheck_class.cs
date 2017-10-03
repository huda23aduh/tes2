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
    class Huda_duplicationCheck_class
    {
        public void duplicationCheck()
        {
            ICustomersService custService = null;
            ICustomersConfigurationService custcService = null;


            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            
            
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            custService = AsmRepository.AllServices.GetCustomersService(authHeader);
            custcService = AsmRepository.AllServices.GetCustomersConfigurationService(authHeader);

            var ads = custcService.GetValidAddresses(new BaseQueryRequest()
            {
                FilterCriteria = new CriteriaCollection()
                {
                    new Criteria()
                    {
                        Key = "PostalCode",
                        Operator = Operator.Equal,
                        Value = "11140000"
                    }
                }

            });

            //var ads = custcService.GetCountries(new BaseQueryRequest()
            //{
            //    FilterCriteria = new CriteriaCollection()
            //    {
            //        new Criteria()
            //        {
            //            Key = "Id",
            //            Operator = Operator.Equal,
            //            Value = "1"
            //        }
            //    }

            //});

            var findCustomers = custService.GetDuplicateCustomers(new DuplicateCustomerCriteria()
            {
                ValidAddressId = ads.Items[0].Id.Value,
                CustomerCaptureCategory = CustomerCaptureCategory.ResidentialCustomers,
                FirstName = "TEST NANA",
                Surname = "UAT LIVE",
                //HouseNumberAlpha = "123",

                //Extra = "chongqing",      // Address Line 1
                //ExtraExtra = "yuzhongqu", // Address Line 2
                HomePhone = "02192554175",
                WorkPhone = "6281808843210",
                Fax1 = "045465888452",   // emergency Number
                Fax2 = "02192554175",   // Mobile Phone
                //CustomField1 = "aaaaa",
                //CustomField2 = "bbbbb",
                ReferenceTypeKey = 1, // 
                ReferenceNumber = "RN123",

            });
            foreach (var cust in findCustomers)
            {
                //Console.WriteLine("ID : ");
                Console.WriteLine("ID : " + cust.Id);
                Console.WriteLine("ID : " + cust.Id.Value + " Name% : " + cust.SimilarityOfCustomerName + " Address% : " + cust.SimilarityOfAddress + "  Phone% : " + cust.SimilarityOfPhone + " RefNum% : " + cust.SimilarityOfReference);
            }
            //Console.WriteLine("ID : " );
            Console.Read();
        }
    }
}
