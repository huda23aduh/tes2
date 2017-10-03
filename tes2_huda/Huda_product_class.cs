using PayMedia.ApplicationServices.ClientProxy;
using PayMedia.ApplicationServices.ProductCatalog.ServiceContracts;
using PayMedia.ApplicationServices.ProductCatalog.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tes2_huda
{
    class Huda_product_class
    {
        static IProductCatalogService m_IProductCatalogService;

        public void getproductCustomerByCustId(int id)
        {
            

            CategoryProductPriceCollection products = GetProductsForCustomerId(id);
            if (products != null && products.Items.Count > 0)
            {
                foreach (CategoryProductPrice pp in products.Items)
                {
                    Console.WriteLine("Found eligible CP: {0}. Price = {1}", pp.CommercialProductName, pp.ListPriceAmount);
                }
            }
            else
            {
                Console.WriteLine("No eligible products for this customer ID.");
            }
            Console.ReadLine();
        }
        static IProductCatalogService ProductCatalogService
        {
            get
            {
                if (m_IProductCatalogService == null)
                {
                    AuthenticationHeader ah = new AuthenticationHeader { Dsn = "MSKY-UAT", Proof = "ICCMsky2016", UserName = "hdkartika" };
                    AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
                    m_IProductCatalogService = AsmRepository.GetServiceProxyCachedOrDefault<IProductCatalogService>(ah);
                }
                return m_IProductCatalogService;
            }
        }
        static CategoryProductPriceCollection GetProductsForCustomerId(int customerId)
        {
            CategoryProductPriceCollection cpp = null;
            IProductCatalogService pcService = ProductCatalogService;
            cpp = pcService.GetProductsForCustomerId(customerId);
            return cpp;
        }

    }
}
