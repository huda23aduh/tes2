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
    class Huda_address_class
    {
        public void GetAddressEntityViewCollection(int cust_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
            var TSService = AsmRepository.GetServiceProxyCachedOrDefault<IViewFacadeService>(ah);

            BaseQueryRequest request = new BaseQueryRequest();
            request.FilterCriteria = new CriteriaCollection();
            request.FilterCriteria.Add(new Criteria("CustomerId", cust_id));

            var servicetype = TSService.GetAddressEntityViewCollection(request);
            if (servicetype != null)
            {
                Console.WriteLine(servicetype.Items[0].Id);
            }
            else
            {
                Console.WriteLine("kosong");
            }
        }
    }
}
