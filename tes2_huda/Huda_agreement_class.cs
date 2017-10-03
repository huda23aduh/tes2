using PayMedia.ApplicationServices.AgreementManagement.ServiceContracts;
using PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.ClientProxy;
using PayMedia.ApplicationServices.SharedContracts;
using PayMedia.ApplicationServices.ViewFacade.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tes2_huda
{
    class Huda_agreement_class
    {

        public void getAgreementDetail()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            var m_IAgreementManagementService = AsmRepository.GetServiceProxyCachedOrDefault<IAgreementManagementService>(authHeader);
            var viewService = AsmRepository.GetServiceProxy<IViewFacadeService>(authHeader);


            AgreementDetailCollection adc = m_IAgreementManagementService.GetAgreementDetailsForCustomer(500016733, 1);
            if (adc != null && adc.Items.Count > 0)
            {
                foreach (AgreementDetail a in adc.Items)
                {
                    Console.WriteLine("Found product ID {0}: Status ID = {1}", a.CommercialProductId, a.Status);
                }
            }
            else
            {
                Console.WriteLine("I found nothing.");
            }



            var viewD = viewService.GetCustomerDeviceView(new BaseQueryRequest()
            {
                FilterCriteria = Op.Eq("CustomerId", 500016733),
                PageCriteria = new PageCriteria()
                {
                    Page = 0,
                    PageSize = 100
                },
                DeepLoad = true

            });



            foreach (var device in viewD.Items)
            {

                // here you can call GetDeviceView to get more info, parameters are the same like above GetCustomerDeviceView , just change Op.Eq("CustomerId", customer_id) to Op,Eq("Id",device.Id.Value)
                Console.WriteLine("Device ID : " + device.DeviceId.Value + " Serial Number : " + device.SerialNumber
                                  + " Status : " + device.DeviceStatusName + " Model:" + device.ModelDescription);
            }


            var viewAg = viewService.GetAgreementDetailView(new BaseQueryRequest()
            {
                FilterCriteria = Op.Eq("CustomerId", 500016733),
                PageCriteria = new PageCriteria()
                {
                    Page = 0,
                    PageSize = 100
                }
            });

            foreach (var product in viewAg.Items)
            {
                Console.WriteLine("Agreement Detail ID : " + product.Id.Value + " Product Name :" + product.CommercialProductName + " Charge Until Date : " + product.ChargeUntilDate + " Charge Period : "
                                  + product.ChargePeriodName + " Finance Option : " + product.FinanceOptionName + " Recurring Price : " + product.RecurringListPrice
                                  + " Status :" + product.StatusName);
            }

            Console.ReadLine();

        }
    }
}
