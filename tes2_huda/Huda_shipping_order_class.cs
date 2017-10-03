using PayMedia.ApplicationServices.ClientProxy;
using PayMedia.ApplicationServices.OrderManagement.ServiceContracts;
using PayMedia.ApplicationServices.OrderManagement.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tes2_huda
{
    class Huda_shipping_order_class
    {
        public void GetShippingOrderByCustomerId(int cust_id)
        {
            //Create the authentication header, initialize with credentials, and
            //create the service proxy. For details, see the API Developer's
            //Guide and the Reference Library CHM.
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
            IOrderManagementService orderManagementService = AsmRepository.GetServiceProxyCachedOrDefault<IOrderManagementService>(authHeader);
            
            BaseQueryRequest request = new BaseQueryRequest();
            //Page = 0 returns a maximum of 20 records. If you want all reecords, you must
            //iterate through the pages.
            request.PageCriteria = new PageCriteria { Page = 0 };
            CriteriaCollection criteria = new CriteriaCollection();
            //Here is a list of properties you can search on. This list is valid as of MR22.
            //For property descriptions, see the API Reference Library (CHM file).
            //Use Intellisense for the current list of properties.
            //ShippingOrder so = new ShippingOrder();
            //so.AgreementId;
            //so.Comment;
            //so.CreateDateTime;
            //so.CustomerId;
            //so.CustomFields.Add;
            //so.Destination;
            //so.FinancialAccountId;
            //so.FullyReceiveReturnedOrder;
            //so.Id;
            //so.IgnoreAgreementId;
            //so.OldStatusId;
            //so.ParentOrderId;
            //so.ReceivedQuantity;
            //so.Reference;
            //so.ReturnedQuantity;
            //so.SandboxId;
            //so.SandboxSkipValidation;
            //so.ShipByDate;
            //so.ShipFromStockHandlerId;
            //so.ShippedDate;
            //so.ShippedQuantity;
            //so.ShippingMethodId;
            //so.ShippingOrderLines.Add;
            //so.ShipToAddressId;
            //so.ShipToPartyId;
            //so.ShipToPostalCode;
            //so.StatusId;
            //so.TotalQuantity;
            //so.TrackingNumbers;
            //so.TypeId;
            //By default, the search uses a logical AND between search criteria.
            //Use || to perform a logical OR.
            criteria.Add("CustomerId", cust_id);
            request.FilterCriteria = criteria;

            
            ShippingOrderCollection soc = orderManagementService.GetShippingOrders(request);
            if (soc != null && soc.Items.Count > 0)
            {
                foreach (ShippingOrder shippingOrder in soc.Items)
                {
                    Console.WriteLine("Found Shipping Order ID: {0}", shippingOrder.Id);
                }
            }
            else
            {
                Console.WriteLine("No records found.");
            }
            Console.ReadLine();


        }
    }
}
