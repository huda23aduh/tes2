using PayMedia.ApplicationServices.ClientProxy;
using PayMedia.ApplicationServices.SharedContracts;
using PayMedia.ApplicationServices.Customers.ServiceContracts;
using PayMedia.ApplicationServices.Customers.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.Finance.ServiceContracts;
using PayMedia.ApplicationServices.Finance.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.ViewFacade.ServiceContracts;
using PayMedia.ApplicationServices.ViewFacade.ServiceContracts.DataContracts;
using System;
using System.Linq;
using PayMedia.ApplicationServices.CustomFields.ServiceContracts;
using PayMedia.ApplicationServices.SandBoxManager.ServiceContracts;
using PayMedia.ApplicationServices.ProductCatalog.ServiceContracts;
using PayMedia.ApplicationServices.AgreementManagement.ServiceContracts;
using PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.Pricing.ServiceContracts;
using PayMedia.ApplicationServices.Pricing.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.InvoiceRun.ServiceContracts;
using PayMedia.ApplicationServices.Workforce.ServiceContracts;
using PayMedia.ApplicationServices.Workforce.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.Contacts.ServiceContracts.DataContracts;
using System.Collections.Generic;
using PayMedia.ApplicationServices.OfferManagement.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.OfferManagement.ServiceContracts;

namespace tes2_huda
{
    class Huda_customer_class
    {
        static IViewFacadeService m_IViewFacadeService;
        private static object viewCriteria;
        private static object viewRequest;

        List<int> the_products = new List<int>(new int[] {308,  27, 308, 26 }); //1,2, 199, 200
        List<int> the_segmentation_list = new List<int>(new int[] { 1, 1, 2, 2 });
        List<int> the_finance_option_id_list = new List<int>(new int[] { 3, 2, 3, 2});
        List<int> offers = new List<int>(new int[] {  881, 882, 883, 884 , 893});

        public int total_offers = 5;
        public int total_segemntation_list = 4;
        public int total_finance_option_id_list = 4;
        public int total_products = 4;


        public void getCustomersByCountryId()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);
            var customersService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersService>(authHeader);
            var financeService = AsmRepository.GetServiceProxyCachedOrDefault<IFinanceService>(authHeader);
            var financeConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<IFinanceConfigurationService>(authHeader);


            //Instantiate a BaseQueryRequest for your filter criteria.
            BaseQueryRequest request = new BaseQueryRequest();

            //Because the phone number is a string, the value entered
            //must exactly match the customer's phone number in the database.
            //For an example that searches for a matching address,
            //see the code sample called Code_FindCustomerByAddressDetails.pdf


            request.FilterCriteria = Op.Like("CustomerStatusId", "1");

            //Call the method and display the results.
            //This method searches ALL the addresses for a matching phone number.
            FindCustomerCollection customerColl = customersService.GetCustomers(request);

            if (customerColl != null)
            {
                foreach (FindCustomer c in customerColl)
                {
                    Console.WriteLine("Found Customer ID {0}, firstname = {1} - surname = {2} -  bigcity = {3} -  postalcode = {4} - street = {5} - validaddressID = {6} - Email = {7} - defaultaddressId = {8} - cust_status_id = {9}", c.Id, c.FirstName, c.Surname, c.BigCity, c.PostalCode, c.Street, c.ValidAddressId, c.EmailAddress, c.Id.Value, c.CustomerStatusId.Value);
                }
            }
            else
            {
                Console.WriteLine("Cannot find a customer with that phone number.");
            }
            Console.WriteLine("total customer = {0} ----", customerColl.Count());
            //You will need to enter some error handling in case there is more than
            //one matching customer.
            Console.ReadLine();
        }

        public void getCustomer_UpdateCustomer()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            ICustomersService customerService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersService>(authHeader);
            Customer cust = new Customer();
            Console.Write("Enter customer ID: ");
            int id = Convert.ToInt32(Console.ReadLine());
            //This method checks the database for the customerId.
            bool exists = customerService.CustomerExists(id);
            if (exists == true)
            {
                // Retrieve customer and display information.
                cust = customerService.GetCustomer(id);
                Console.WriteLine("Customer ID {0}: {1} {2}", cust.Id, cust.DefaultAddress.FirstName, cust.DefaultAddress.Surname);
                //Prompt for update.
                Console.Write("Enter new first name: ");
                cust.DefaultAddress.FirstName = Console.ReadLine();
                Console.Write("Enter new surname: ");
                cust.DefaultAddress.Surname = Console.ReadLine();
                //User configuration determines if reason keys are required.

                cust.DefaultAddress.Directions = "directions saya ganti";

                cust.DefaultAddress.CustomFields[0].Value = "ini contact name diganti huda";
                cust.DefaultAddress.CustomFields[1].Value = "Saudara";
                cust.DefaultAddress.CustomFields[2].Value = "ini Emergency Contact Address diganti siapa";
                cust.DefaultAddress.CustomFields[3].Value = "ini GPS latitude and longitude";

                var cf = new customFieldHandler(authHeader);
                cf.addAddressCustomField("Emergency Contact Name", cust.DefaultAddress.Id.Value, "ini contact name");
                cf.addAddressCustomField("Emergency Contact Relationship", cust.DefaultAddress.Id.Value, "Saudara");
                cf.addAddressCustomField("Emergency Contact Address", cust.DefaultAddress.Id.Value, "ini alamat emergency");
                cf.addAddressCustomField("GPS latitude and longitude", cust.DefaultAddress.Id.Value, "-88888888888, -9999999999999");



                int reasonKey = 0;
                int reasonTypeKey = 0;
                int reasonStatusKey = 0;
                int reasonAddressKey = 0;
                customerService.UpdateCustomer(cust, reasonKey, reasonTypeKey, reasonStatusKey, reasonAddressKey);
            }
            else
            {
                Console.WriteLine("Customer ID is not valid.");
            }
            Console.WriteLine("New name: {0} {1}", cust.DefaultAddress.FirstName, cust.DefaultAddress.Surname);
            Console.WriteLine("Update successful. {0}", cust.DefaultAddress.CustomFields[0].Value);
            Console.ReadLine();

        }
        public class customFieldHandler
        {
            ICustomFieldsService custfService = null;
            ICustomersService custService = null;

            public customFieldHandler(AuthenticationHeader authHeader)
            {
                Authentication_class var_auth = new Authentication_class();
                AuthenticationHeader ah = var_auth.getAuthentication_header();
                //This is the location of the ASM services.
                AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

                custfService = AsmRepository.AllServices.GetCustomFieldsService(ah);
                custService = AsmRepository.AllServices.GetCustomersService(ah);
            }


            public void addAddressCustomField(string field_name, int address_id, string field_value)
            {

                var c = custfService.GetCustomFieldValues(address_id, 2);


                if (c.Find(c1 => (c1.Name == field_name && c1.Value != null)) != null)
                {
                    custfService.UpdateCustomFieldValues(address_id, 2, new CustomFieldValueCollection()
                    {
                        new CustomFieldValue()
                        {
                            Name = field_name,
                            Value = field_value
                        }
                    });
                }
                else
                {
                    custfService.CreateCustomFieldValues(address_id, 2, new CustomFieldValueCollection(){
                        new CustomFieldValue()
                            {
                                Name = field_name,
                                Value = field_value
                            }
                    });
                }
            }
        }

        public void create_new_customer()
        {
            
            var period = ChargePeriods.Monthly;

            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            //This is the location of the ASM services.
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);
            //Here we create proxies for all the services we will need.
            var customersService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersService>(ah);
            var customersConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersConfigurationService>(ah);
            var financeService = AsmRepository.GetServiceProxyCachedOrDefault<IFinanceService>(ah);
            var financeConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<IFinanceConfigurationService>(ah);
            var invoiceRunService = AsmRepository.GetServiceProxyCachedOrDefault<IInvoiceRunService>(ah);
            var agreementManagementService = AsmRepository.GetServiceProxyCachedOrDefault<IAgreementManagementService>(ah);
            var pricingService = AsmRepository.GetServiceProxyCachedOrDefault<IPricingService>(ah);
            var productCatalogService = AsmRepository.GetServiceProxyCachedOrDefault<IProductCatalogService>(ah);
            var sandboxService = AsmRepository.GetServiceProxyCachedOrDefault<ISandBoxManagerService>(ah);
            var productCatalogConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<IProductCatalogConfigurationService>(ah);
           

            #region Create Customer
            //Configuration controls many default values for a new customer.
            //Call this method to get the defaults. You can pass them in when you create
            //a new customer, or you can override the defaults by passing in different
            //values when you create the new customer.
            CustomerDefaults customerDefault = customersConfigurationService.GetCustomerDefaults();
            //This value is a reason ID for ICC system event 100.
            //You can get this from ICustomersConfigurationService.GetLookups
            //(LookupLists.ReasonCustomerCreate).
            int reasonKey = 0;
            //Instantiate a new Customer.
            Customer customer = new Customer();
            //Configuration controls which values are required. In our environment,
            //customer class and customer type are required, so we pass in the
            //default values here.
            //customer.ClassId = customerDefault.ClassId;
            customer.ClassId = 1;
            customer.TypeId = customerDefault.Type;
            //customer.TypeId = 1;
            customer.BirthDate = DateTime.Now;
            customer.ReferenceTypeKey = 1;
            customer.ReferenceNumber = "111";
            customer.LanguageKey = "E";
            customer.BusinessUnitId = getBubyZipcode("98373011");

            //This value is a reference number from an external system. You can pass it in,
            //and ICC will store it.

            //Then, you can use it later to find the customer.
            customer.ExternalReference = "AB9829";
            //ICC can store many different types of addresses, but the DefaultAddress
            //is always required when you create a new Customer object.
            customer.DefaultAddress = new DefaultAddress();
            //Notice that the customer's name is stored in the DefaultAddress.
            customer.DefaultAddress.FirstName = "PELANGGAN";
            customer.DefaultAddress.MiddleName = "TES";
            customer.DefaultAddress.Surname = "SATU";
            customer.DefaultAddress.CareOfName = "contoh careof name";
            //Descriptions of the DefaultAddress properties can be found in
            //The API Reference Library (CHM file) and the Configuration Module Online Help.
            customer.DefaultAddress.BigCity = "TELUK BINTUNI";
            customer.DefaultAddress.SmallCity = "MERDEY";
            customer.DefaultAddress.ValidAddressId = 81233;
            customer.DefaultAddress.PropertyTypeId = 2;
            customer.DefaultAddress.Street = "ANAJERO";
            customer.DefaultAddress.TitleId = 1;
            customer.DefaultAddress.CountryId = 1;
            customer.DefaultAddress.PostalCode = "98373011";
            customer.DefaultAddress.HomePhone = "222222222222";
            customer.DefaultAddress.WorkPhone = "333333333";
            customer.DefaultAddress.Fax1 = "4444444";
            customer.DefaultAddress.Fax2 = "555555555";
            customer.DefaultAddress.Directions = "contoh directions";
            customer.DefaultAddress.LandMark = "contoh landmark";
            customer.DefaultAddress.Email = "email@dd.dd";

            //customer.DefaultAddress.Street = "APPLE WAY";
            customer.DefaultAddress.HouseNumberNumeric = 100;

            //NOTE: If your configuration requires a Valid Address, or if it is
            //configured to validate addresses, you must pass in
            //values that correspond to a Valid Address in the database. Otherwise, ICC will
            //throw an error. If your configuration requires ValidAddresses, you can
            //simply pass in a ValidAddressId like this:
            //customer.DefaultAddress.ValidAddressId = 217;
            //This call creates the customer and commits him to the database.


            Customer newCustomer = customersService.CreateCustomer(customer, reasonKey);

            //getTimeslotbyPostalcode(newCustomer.DefaultAddress.Id.Value, 1);

            //Test that the customer was created.
            Console.WriteLine("Customer ID {0}: Name = {1} {2}", newCustomer.Id, newCustomer.DefaultAddress.FirstName, newCustomer.DefaultAddress.Surname, newCustomer.BirthDate, newCustomer.ReferenceNumber);


            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            #endregion Create Customer

            #region emergency field
            var cf = new customFieldHandler(ah);
            cf.addAddressCustomField("Emergency Contact Name", newCustomer.DefaultAddress.Id.Value, "Huda");
            cf.addAddressCustomField("Emergency Contact Relationship", newCustomer.DefaultAddress.Id.Value, "Saudara");
            cf.addAddressCustomField("Emergency Contact Address", newCustomer.DefaultAddress.Id.Value, "Jakarta East");
            #endregion emergency field

            #region Create multiple address
            //address biling
            addAddress(1, "13550000", "address BILLING 1", "address BILLING 2", newCustomer.Id.Value, "1111111", "2222222", "333333");

            //address work order
            addAddress(2, "11810000", "address WO 1", "address WO 2", newCustomer.Id.Value, "1111111", "2222222", "333333");

            //address agreement
            addAddress(3, "12110000", "address AGREEMENT 1", "address AGREEMENT 2", newCustomer.Id.Value, "1111111", "2222222", "333333");

            #endregion Create multiple address

            #region Create Financial Account
            //If you ever need to get the default values for a new financial account,
            //you can use these methods.
            //This method returns general financial defaults.
            //FinanceDefaults financeDefault = financeConfigurationService.GetFinanceDefaults();
            //This method returns the default invoice profile ID for a given customer.
            //InvoicingProfile invoicingProfile = invoiceRunService.GetDefaultInvoicingProfile(DateTime.Today, (int)newCustomer.Id,
            //(InvoicingMethod)financeDefault.DefaultInvoiceMethod);
            //This method returns the due date method for a given customer.
            //DueIn dueIn = financeConfigurationService.GetDefaultDueInByCustomerId
            //(int)newCustomer.Id);
            //This value is a reason ID for ICC system event 115.
            //You can get this from IFinancialConfigurationService.GetLookups
            //(LookupLists.FinancialAccountCreateReasons)
            int faReason = 0;
            //Instantiate a new FinancialAccount object.
            FinancialAccount financialAccount = new FinancialAccount();
            //You must pass in the customer ID.
            financialAccount.CustomerId = newCustomer.Id;
            financialAccount.Balance = 10000;
            financialAccount.BankName = "INI BANK DEBET CARD";
            financialAccount.AnniversaryMonth = 12;
            financialAccount.ProxyCodeId = 2;
            //ICC uses configured default values to create the financial account.
            //You can override the defaults by passing in other values.
            //Here we override the default InvoicingProfileId.
            financialAccount.InvoicingProfileId = 1;
            //Here we override the default Due Date Method.
            financialAccount.DueIn = 1;
            //Here we override the default currency.
            financialAccount.CurrencyId = 1;
            //financialAccount.BankAccountId = "1";



            //Type is used to identify whether the customer buys his products on
            //a pre-paid or post-paid basis.
            financialAccount.TypeId = customerDefault.FinancialAccountType;
            //If you need to explicitly assign the default Method of Payment ID, you can
            //use this statement:
            //financialAccount.MopId = customerDefault.MethodOfPayment;
            //In this example, we will override the default method of payment
            //to assign ID 1 ("VISA" credit card).
            financialAccount.MopId = 110;
            financialAccount.BankAccountId = "110";
            financialAccount.InvoiceDeliveryMethodId = 1;

            //Our system has been configured to require additional values when the MOP
            //is credit card, so here we instantiate a new CreditCardCollection
            //to hold those values.
            //financialAccount.CreditCard = new CreditCardCollection();
            //CreditCard creditcardinfo = new CreditCard();
            //financialAccount.CreditCard.Add(creditcardinfo);


            //Use the CreditCardDisplay property to pass in the credit card number.
            //In the database, the credit card number is encrypted.
            //Configuration controls whether the credit card number is masked when it
            //is returned in a method call.
            //For information about encryption and masked credit card numbers,
            //see the "API FAQs" section
            //of the ICC API Developer's Guide.
            //creditcardinfo.CreditCardDisplay = "1234567890123456";
            //creditcardinfo.AuthorizationCode = "987";
            //creditcardinfo.FirstName = "credit card first name";

            //creditcardinfo.AuthorizationDate = new DateTime(2017, 1, 18);
            //creditcardinfo.ExpirationDate = new DateTime(2019, 1, 18);
            //This call creates the financial account and commits it to the database.
            FinancialAccount newFinancialAccount = financeService.CreateFinancialAccount(financialAccount, faReason);
            //Test that the financial account was created.
            Console.WriteLine("Financial account ID {0}: Account balance = {1} ", newFinancialAccount.Id, newFinancialAccount.Balance);
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            #endregion

            #region Get valid products for customer
            ////This cast forces int? newCustomer.Id to be an int.
            //int customerId = (int)newCustomer.Id;
            ////The products a customer can buy is based on a wide
            ////variety of configured values and configured business rules.
            ////You can use this call to find out which Products the customer is
            ////eligible to buy.
            //CategoryProductPriceCollection products = productCatalogService.GetProductsForCustomerId(customerId);
            ////Iterate through the list of products the customer can buy.
            //foreach (var item in products)
            //{
            //    #region Pause after a screenful of records
            //    //This snippet pauses when the screen is full. When the user presses
            //    //Enter, the screen clears and another screenful of records is loaded.
            //    if (Console.WindowHeight == Console.CursorTop)
            //    {
            //        Console.WriteLine("Press Enter for another screen of records.");
            //        Console.ReadLine();
            //        Console.Clear();
            //    }
            //    #endregion

            //    Console.WriteLine("Customer can buy {0}, ID {1}", item.CommercialProductName, item.CommercialProductId);
            //}
            //Console.WriteLine("Press Enter to continue.");
            //Console.ReadLine();
            #endregion

            #region Get Pricing
            //The price a customer pays for a product depends on a very large number
            //of conditions, including the customer's address, type, and class,
            //the product's finance option, charge period, and business unit, the currency,
            //the financial account type, the agreement type, and the pricing date
            //(to name a few).
            //To get the correct price for a particular product,
            //you need to know all the details related to the configured conditions.
            //The main methods to get prices are
            // o IPricingService.GetPricing()
            // o IAgreementManagementService.ManageProductCapture()
            // o IAgreementManagementService.GetManageFullProductDetails()
            //The benefit to GetPricing() is that you can use it before the customer
            //has an agreement.
            //For details, see the API Reference Library (CHM file).
            //This example shows how to get the price for a product that was
            //returned by GetProductsForCustomerId().
            //In this example, we will not bother checking for a price override.
            bool checkForOverrides = false;
            //Instantiate the collection and the criteria for the method.
            PricingCriteriaCollection pricingCriteriaCollection = new PricingCriteriaCollection();
            PricingCriteria pricingCriteria = new PricingCriteria();
            //Add the pricing criteria to the collection.
            pricingCriteriaCollection.Add(pricingCriteria);
            #region These properties in this region are required to get a price quote.
            //Choose the object for which you want prices (in this case, a CommercialProduct).
            pricingCriteria.PricingForType = PricingForType.CommercialProduct;
            //Pass in the ID of the product for which you want prices.
            pricingCriteria.PricingForId = 1;
            //You use Pricing Date to select the price that is effective today, or for a date in
            //the future or in the past.
            pricingCriteria.PricingDate = DateTime.Today;
            //Assign the currency of the financial account you created with
            //CreateFinancialAccount()
            pricingCriteria.CurrencyId = newFinancialAccount.CurrencyId;
            //PricingChargeTypes can be recurring, once-off, or both.
            pricingCriteria.PricingChargeType = PricingChargeType.Both;
            //Financial account type controls whether the customer can buy pre-paid or
            //post-paid products.
            pricingCriteria.FinancialAccountTypeId = newFinancialAccount.TypeId;
            //Not required but very important. This property helps prevent
            //a null response for those parameters where a null value means “none.”
            pricingCriteria.NullCriteriaMatchesAll = true;
            #endregion
            //Assign the customer ID you created with CreateCustomer().
            pricingCriteria.CustomerId = newCustomer.Id;
            //You can use this parameter to limit the returned prices to ones that match a
            //configured finance option. In our environment,
            //FinanceOptionId 1 = Rent and FinanceOptionId 2 = Sold.
            pricingCriteria.FinanceOptionId = 3;
            //Call the method to get prices that match the input criteria.
            //For another example of GetPricing(), see the Code Samples in the API
            //Developer's Guide.
            PricingInfoCollection pricingInfo = pricingService.GetPricing(pricingCriteriaCollection, checkForOverrides);
            //Test that you can get prices for the product.
            foreach (var p in pricingInfo)
            {
                //We are writing a lot on info here, only so you can get an idea
                //of how prices can vary due to different conditions.
                Console.WriteLine("Product ID = {0}, Invoice Text = {1}",
                p.PricingForId, p.InvoiceText);
                Console.WriteLine("Price = {0}, Price Condition ID = {1}",
                p.ListPriceAmount, p.ListPriceConditionId);
                Console.WriteLine("Event ID = {0}, Reason ID = {1}, Charge Type = {2}",
                p.EventId, p.EventReasonId, p.PricingChargeType);
                Console.WriteLine();
            }
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            #endregion Get Pricing


            #region Create Agreement
            //This value is a reason ID for ICC system event 960.
            //You can get this from IAgreementManagementConfigurationService.GetLookups
            //(LookupLists.AgreementCreationEditReasons)
            int agReason = 0;
            //Instantiate a new Agreement.
            Agreement agreement = new Agreement();


            //agreement.ServiceAddressId = 125;
            //agreement.ContractEndDate = DateTime.Now;

            //Required. Assign the financial account ID to the agreement.
            agreement.FinancialAccountId = newFinancialAccount.Id;
            //Required. The default frequency for generating recurring charges
            //for products on the agreement. AgreementDetails on this agreement
            //inherit this value.
            agreement.ChargePeriod = ChargePeriods.Monthly;
            //The default Type is a configured value. You can override the default by passing
            //in a new value here.
            agreement.Type = 1;
            //A configured value that represents the duration of the agreement.
            agreement.ContractPeriodId = 12;
            //Market segment is derived from the customer's service address.
            //You can override the value, but it is not recommended because the choice
            //of available products can depend on market segment configuration.
            //agreement.MarketSegmentId = 1;
            //This call creates the agreement and commits it to the database.
            Agreement newAgreement = agreementManagementService.CreateAgreement(agreement, agReason);
            //Test that the agreement was created.
            Console.WriteLine("Agreement ID {0}: Market Segment ID = {1}", newAgreement.Id,
            newAgreement.MarketSegmentId);
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            #endregion Create Agreement

            #region getCommercialProductByCustId
            //getAvaiableCPbyCustomer(newCustomer.Id.Value);
            #endregion getCommercialProductByCustId

            #region addcommercialproduct
            //BaseQueryRequest request = new BaseQueryRequest();
            //CriteriaCollection criteria = new CriteriaCollection();
            ////request.FilterCriteria.Add(new Criteria("ExternalId", 92591));
            //request.FilterCriteria.Add(new Criteria("IconId", ""));
            ////Call the method and display the results.
            //CommercialProductCollection coll = productCatalogConfigurationService.GetCommercialProducts(request);
            //if (coll != null && coll.Items.Count > 0)
            //{
            //    foreach (CommercialProduct p in coll)
            //    {
            //        if(p.CategoryId == 1)
            //        {
            //            AddCommercialProduct(p.Id.Value, newCustomer.Id.Value, 3);
            //        }
            //        else if (p.CategoryId == 67 || p.CategoryId == 3)
            //        {
            //            AddCommercialProduct(p.Id.Value, newCustomer.Id.Value, 2);
            //        }
            //        Console.WriteLine("successfully add commercial product ", p.Id, p.Name);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Nothing matches your criteria.");
            //}

            #endregion addcommercialproduct

            #region add_commercial_product_single_quote
            int alert = 0;
            int i = 0;

            

            #region add offer/promo
            //for (i = 0; i < total_offers; i++)
            //{
            //    offers.Add(offers[i]);
            //}
            #endregion add offer/promo


            alert = addCommercialProductsSingleQuote(customer, newCustomer.Id.Value, period, offers);

            #endregion addCommercialProductsSingleQuote

            #region Create product, device links, shipping order, and work order.
            ////Before you can sell a product to a customer, the customer MUST have a
            ////financial account and an agreement.
            ////Create the sandbox workspace. A sandbox is required when
            ////you create a product.
            //int sandboxId = sandboxService.CreateSandbox();
            ////Instantiate an object for the product capture (input) parameters.
            //ProductCaptureParams productCaptureParams = new ProductCaptureParams();
            ////Assign the sandbox ID to the ProductCaptureParams.
            //productCaptureParams.SandboxId = sandboxId;
            ////Assign the customer's agreement to the new product.
            //productCaptureParams.AgreementId = newAgreement.Id;
            ////To assign an agreement-level Offer, pass in the
            ////OfferDefinitionId(s) here.
            ////To assign product-level Offers, pass their IDs in the
            ////AgreementDetailWithOfferDefinitions.ProductCaptureOfferInfos property.
            ////This line of code is commented out here because in this
            ////example, the Offer is assigned at the product-level.
            ////productCaptureParams.OfferDefinitions = new List<int>(31);
            ////If False, ICC creates shipping orders that have been configured for
            ////the TechnicalProducts that make up the selected CommercialProduct.
            //productCaptureParams.SkipShippingOrderGeneration = false;
            ////If False, ICC creates work orders that have been configured for
            ////the TechnicalProducts that make up the selected CommercialProduct.
            //productCaptureParams.SkipWorkOrderGeneration = false;
            ////Valid only when a distributor owns the existing devices at the
            ////customer's address. If True, the system attempts to find
            ////distributor-owned devices at the associated address, and
            ////automatically links the existing hardware to the DPADs
            ////created during the capture process.
            //productCaptureParams.DevicesOnHand = false;
            ////Here we instantiate an object for the reason associated with this
            ////product's capture.
            //productCaptureParams.CaptureReasons = new ProductCaptureReasons();
            ////In this example, we are creating a new product, so we assign a reason ID
            ////that is valid for ICC system event 120.
            //productCaptureParams.CaptureReasons.CreateReason = 0;
            ////The next set of values set up the customer's product and, optionally,
            ////product-level offers.
            ////NOTE: An AgreementDetail is an instance of a CommercialProduct that is linked to
            ////a specific customer. (CommercialProducts are the things you sell. When you sell one
            ////to a customer, the particular instance that is linked to a customer is called
            ////an AgreementDetail.)
            //productCaptureParams.AgreementDetailWithOfferDefinitions = new AgreementDetailWithOfferDefinitionsCollection
            //{
            //    new AgreementDetailWithOfferDefinitions
            //    {
            //        AgreementDetail = new AgreementDetail
            //        {
            //            //Not required but highly recommended.
            //            //This property is valid only when you create
            //            //a new AgreementDetail. If your users have configured a Default
            //            //Disconnection Setting in the Configuration Module, you
            //            //should set this property to
            //            //UseDefaultProductDisconnectionsSettingOnCreate.
            //            //This will ensure that ICC uses the configured Disconnection
            //            //Settings on the new product that you are creating.
            //            DisconnectionSetting = PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.DisconnectionDateSettings.UseDefaultProductDisconnectionSettingOnCreate,
            //            //Replace this value with the ID of the address
            //            //where the product will be used.
            //            AddressId = newCustomer.DefaultAddress.Id,
            //            //Set this value to define whether a dealer can receive
            //            //commission for selling this product.
            //            CommissionOption=CommissionOptionTypes.NoCommission,
            //            //Replace this value with the customer's ID.
            //            CustomerId = newCustomer.Id,
            //            //This value is the Agreement to which the product
            //            //will be linked.
            //            AgreementId = productCaptureParams.AgreementId,
            //            //Choose the value that ICC will use when creating
            //            //charges for the product.
            //            ChargePeriod = ChargePeriods.Monthly,
            //            //ICC uses this property during an invoice run to determine
            //            //whether to create charges for the product.
            //            //If you don't specify this value, ICC defaults the value to
            //            //yesterday (which is the correct value to use when creating
            //            //a new product).
            //            ChargeUntilDate = DateTime.Now.AddDays(-1),
            //            //Replace this value with the ID of the CommercialProduct that
            //            //the customer is buying.
            //            CommercialProductId = 21,
            //            //You can use the agreement's contract period, or you can
            //            //enter a different value (depending on your company's business rules).
            //            ContractPeriodId = newAgreement.ContractPeriodId,
            //            //Replace this value with the start date of the contract.
            //            //It can be the same or different from the Agreement start date.
            //            ContractStartDate=newAgreement.ContractStartDate,
            //            //Replace this value with the end date of the contract.
            //            //It can be the same or different from the Agreement end date.
            //            ContractEndDate=newAgreement.ContractEndDate,
            //            //If True, indicates that the product includes a hardware
            //            //device, and ICC will create the necessary DPAD (Device Per Agreement
            //            //Detail) records for you.
            //            DeviceIncluded = true,
            //            //Used only when a customer is linked to a Distributor.
            //            //If True, ICC will attempt to find distributor-owned devices
            //            //at the customer's address, and will automatically link the
            //            //devices to the DPADs that are created during the transaction.
            //            DevicesOnHand = false,
            //            //Replace this value with the ID of a FinanceOption that is
            //            //valid in your environment. We are using the same Rent finance option
            //            //that we used when we called GetPricing().
            //            FinanceOptionId = pricingCriteria.FinanceOptionId,
            //            //Assign the customer's financial account ID to the new AgreementDetail.
            //            FinancialAccountId = newFinancialAccount.Id,
            //            //Replace this value with the number of this product (ID = 21) that
            //            //the customer wants to buy.
            //            Quantity = 1
            //        },
            //        //Use this section if you want to apply a product-level
            //        //offer to the new product.
            //        ProductCaptureOfferInfos = new ProductCaptureOfferInfoCollection
            //        {
            //            new ProductCaptureOfferInfo
            //            {
            //                //Replace this value with a valid Offer ID.
            //                AppliedOfferDefinitionId = 22
            //            }
            //        },
            //        //Use this section if you want to override the user-configured
            //        //price for this product.
            //        PriceOverrides = new PriceOverrideExCollection
            //        {
            //            new PriceOverrideEx
            //            {
            //                //Replace this value with the ID of the list price condition
            //                //that you want to override. It must be a valid condition ID
            //                //for the product you are creating.
            //                ListPriceConditionId = 61,
            //                //ICC uses this enum, together with Amount, to calculate
            //                //the override price.
            //                OverrideType = AgreementOverrideTypes.PercentOff,
            //                //ICC uses this value, together with OverrideType,
            //                //to calculate the override price. In this example,
            //                //the override price will be 12% off the regular list price.
            //                Amount = 12,
            //                //Replace this value with the ID of a valid reason
            //                //for event 5231, the Create Price Override event.
            //                CreationReason = 0,
            //                //This text appears on FTs and invoices to describe
            //                //the charge.
            //                OverrideInvoiceText = "Your Special Price",
            //                //Replace this value with a ledger account ID that is
            //                //valid in your environment.
            //                OverrideLedgerAccountId = 711
            //            }
            //        }
            //    }
            //};

            ////Call the method to create the AgreementDetail, device links, shipping order,
            ////and work order.
            //ProductCaptureResults newProduct = agreementManagementService.ManageProductCapture(productCaptureParams);
            //if (newProduct.AgreementDetails.Items.Count == 0)
            //{
            //    //Console.WriteLine("Something bad happened. Sandbox rolled back.");
            //    //Console.ReadLine();
            //    //The "false" value in this line of code rolls back the sandbox.
            //    sandboxService.FinalizeSandbox(sandboxId, false);
            //    //return;
            //}
            ////The "true" value in this line of code commits the sandbox.
            //sandboxService.FinalizeSandbox(sandboxId, true);
            ////foreach (var item in newProduct.AgreementDetails)
            ////{
            ////    Console.WriteLine("Customer purchased Commercial Product ID {0}; AgreementDetail ID = { 1}", item.CommercialProductId, item.Id);
            ////}
            #endregion Create product, device links, shipping order, and work order.


            //This keeps the console window open until the user presses Enter.
            Console.ReadLine();
        }

        public void create_offer()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            var service = AsmRepository.AllServices.GetOfferManagementService(authHeader);

            Offer aa = new Offer() {
                ApplyToLevel = ApplyToLevelTypes.AgreementDetail,
                AgreementDetailId = 4484486 ,
                OfferDefinitionId = 896,
                StartDate = DateTime.Today
            };

            try
            {
                service.CreateOffer(aa, 80);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            

        }
        public int addCommercialProductsSingleQuote(Customer customer_data_json, int customer_id, ChargePeriods param_periods, List<int> offers)
        {
            try
            {
                Authentication_class var_auth = new Authentication_class();
                AuthenticationHeader authHeader = var_auth.getAuthentication_header();
                //This is the location of the ASM services.
                AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

                IAgreementManagementService agService = null;
                IAgreementManagementConfigurationService agcService = null;
                IProductCatalogService productService = null;
                IProductCatalogConfigurationService productcService = null;
                ICustomersConfigurationService custcService = null;
                IFinanceConfigurationService fincService = null;
                ISandBoxManagerService sandboxService = null;
                ICustomersService custService = null;


                agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
                agcService = AsmRepository.AllServices.GetAgreementManagementConfigurationService(authHeader);
                productService = AsmRepository.AllServices.GetProductCatalogService(authHeader);
                productcService = AsmRepository.AllServices.GetProductCatalogConfigurationService(authHeader);
                custcService = AsmRepository.AllServices.GetCustomersConfigurationService(authHeader);
                fincService = AsmRepository.AllServices.GetFinanceConfigurationService(authHeader);
                sandboxService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
                custService = AsmRepository.AllServices.GetCustomersService(authHeader);

                int sandbox_id = sandboxService.CreateSandbox();
                //var commercial_product = productcService.GetCommercialProduct(commercial_product_id);
                //if (commercial_product == null)
                //{
                //    //Console.WriteLine("Commercial Product with ID : " + commercial_product_id + " Not Exist!!!");
                //    return 0;
                //}

                int business_unit_id = custService.GetCustomer(customer_id).BusinessUnitId.Value;

                int first_prod = 0;
                int agd_id = 0;
                int reason120 = 65;
                int agreement_id = 0;
                var agreements = agService.GetAgreementsForCustomerId(customer_id, 1);

                if (agreements.Items.Count > 0)
                {
                    agreement_id = agreements.Items[0].Id.Value;

                    AgreementDetailWithOfferDefinitionsCollection productList = new AgreementDetailWithOfferDefinitionsCollection();
                    ProductCaptureOfferInfoCollection offerInfos = new ProductCaptureOfferInfoCollection();
                    
                    for (int i = 0; i < total_products; i++)
                    {
                        //offerInfos = null;

                        // for product (hardware)
                        if(
                            the_products[i] == 1 || the_products[i] == 2 ||
                            the_products[i] == 199 || the_products[i] == 200 ||
                            the_products[i] == 308 || the_products[i] == 309
                          ) 
                        {
                            productList.Add(new AgreementDetailWithOfferDefinitions()
                            {
                                AgreementDetail = new AgreementDetail()
                                {
                                    CustomerId = customer_id,
                                    AgreementId = agreement_id,
                                    Segmentation = the_segmentation_list[i],


                                    ChargePeriod = param_periods,

                                    CommercialProductId = the_products[i],    // comm_prod_id
                                    DeviceIncluded = true,
                                    DevicesOnHand = false,
                                    Quantity = 1,
                                    FinanceOptionId = the_finance_option_id_list[i],
                                    //FinanceOptionId = 3,   // FinanceOptionId : 2 is Subscription for software product, if you need add hardware product, you need use 1 as sold, 3 as rent
                                    BusinessUnitId = business_unit_id,
                                    //DisconnectionSetting = PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.DisconnectionDateSettings.QuoteBased
                                    DisconnectionSetting = PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.DisconnectionDateSettings.AccountBased
                                },
                            });
                        }
                        else
                        {
                            if (first_prod == 0)
                            {
                                foreach (var zz in offers)
                                {
                                    offerInfos.Add(new ProductCaptureOfferInfo() { AppliedOfferDefinitionId = zz });
                                }
                            }
                            else
                                offerInfos = null;
                            

                            productList.Add(new AgreementDetailWithOfferDefinitions()
                            {
                                
                                AgreementDetail = new AgreementDetail()
                                {
                                    CustomerId = customer_id,
                                    AgreementId = agreement_id,
                                    ChargePeriod = param_periods,
                                    Id = agd_id,
                                    Segmentation = the_segmentation_list[i],
                                    CommercialProductId = the_products[i],   // comm_prod_id
                                    DeviceIncluded = true,
                                    DevicesOnHand = false,
                                    Quantity = 1,
                                    FinanceOptionId = 2,   // FinanceOptionId : 2 is Subscription for software product, if you need add hardware product, you need use 1 as sold, 3 as rent
                                    BusinessUnitId = business_unit_id,
                                    DisconnectionSetting = PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.DisconnectionDateSettings.QuoteBased
                                },

                                ProductCaptureOfferInfos = offerInfos
                                
                            });
                            first_prod = 1;
                        }
                    }


                    ProductCaptureParams param = new ProductCaptureParams()
                    {
                        SandboxId = sandbox_id,
                        AgreementId = agreement_id,
                        CaptureReasons = new ProductCaptureReasons()
                        {
                            CreateReason = reason120
                        },
                        //OfferDefinitions = offers,
                        SkipWorkOrderGeneration = true,
                        SkipShippingOrderGeneration = false,
                        AgreementDetailWithOfferDefinitions = productList
                        //AgreementDetailWithOfferDefinitions = new AgreementDetailWithOfferDefinitionsCollection()
                        //{
                        //    new AgreementDetailWithOfferDefinitions()
                        //    {
                        //        AgreementDetail = new AgreementDetail()
                        //        {
                        //            CustomerId = customer_id,
                        //            AgreementId = agreement_id,
                        //            ChargePeriod = ChargePeriods.Monthly,
                        //            CommercialProductId = commercial_product_id,
                        //            // DevicesOnHand : For software product, put false, for hardware product, put true.
                        //            DevicesOnHand = false,
                        //            // Quantity : Please do not put number more than 1. It must be 1 only.
                        //            Quantity = 1,  
                        //            // FinanceOptionId : 2 is Subscription for software product, if you need add hardware product, you need use 1 (Sold) or 3 (Rent).
                        //            FinanceOptionId = financeoption,
                        //            BusinessUnitId = business_unit_id,
                        //            DisconnectionSetting = PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.DisconnectionDateSettings.QuoteBased
                        //        },
                        //    }
                        //}

                    };

                    var result = agService.ManageProductCapture(param);

                    if (result.AgreementDetails.Items.Count == 0)
                    {
                       Console.WriteLine("Warnings : " + result.WarningMessage);

                        Console.WriteLine("Something bad happened. Sandbox rolled back.");

                        //The "false" value in this line of code rolls back the sandbox.
                        sandboxService.FinalizeSandbox(sandbox_id, false);
                        return 0;

                    }
                    else
                    {
                        //The "true" value in this line of code commits the sandbox.
                        sandboxService.FinalizeSandbox(sandbox_id, true);

                        foreach (var item in result.AgreementDetails)
                        {
                            Console.WriteLine("Congratulations! New Product ID = {0}.", item.Id);
                        }
                        return result.AgreementDetails.Items[0].Id.Value;
                    }
                }
                else
                {
                    //Console.WriteLine("Can't find agreement for customer id : " + customer_id);
                    return 0;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Exception Stack : " + ex.StackTrace);
                return 0;
            }

        }

        static int AddCommercialProduct(int commercial_product_id, int customer_id, int financeoption)
        {

            try
            {
                Authentication_class var_auth = new Authentication_class();
                AuthenticationHeader authHeader = var_auth.getAuthentication_header();
                AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
                
                IAgreementManagementService agService = null;
                IAgreementManagementConfigurationService agcService = null;
                IProductCatalogService productService = null;
                IProductCatalogConfigurationService productcService = null;
                ICustomersConfigurationService custcService = null;
                IFinanceConfigurationService fincService = null;
                ISandBoxManagerService sandboxService = null;
                ICustomersService custService = null;


                agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
                agcService = AsmRepository.AllServices.GetAgreementManagementConfigurationService(authHeader);
                productService = AsmRepository.AllServices.GetProductCatalogService(authHeader);
                productcService = AsmRepository.AllServices.GetProductCatalogConfigurationService(authHeader);
                custcService = AsmRepository.AllServices.GetCustomersConfigurationService(authHeader);
                fincService = AsmRepository.AllServices.GetFinanceConfigurationService(authHeader);
                sandboxService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
                custService = AsmRepository.AllServices.GetCustomersService(authHeader);

                int sandbox_id = sandboxService.CreateSandbox();
                var commercial_product = productcService.GetCommercialProduct(commercial_product_id);
                if (commercial_product == null)
                {
                    Console.WriteLine("Commercial Product with ID : " + commercial_product_id + " Not Exist!!!");
                    return 0;
                }

                int business_unit_id = custService.GetCustomer(customer_id).BusinessUnitId.Value;

                int reason120 = 65;
                int agreement_id = 0;
                var agreements = agService.GetAgreementsForCustomerId(customer_id, 1);

                if (agreements.Items.Count > 0)
                {
                    agreement_id = agreements.Items[0].Id.Value;

                    ProductCaptureParams param = new ProductCaptureParams()
                    {
                        SandboxId = sandbox_id,
                        AgreementId = agreement_id,
                        CaptureReasons = new ProductCaptureReasons()
                        {
                            CreateReason = reason120
                        },
                        OfferDefinitions = null,
                        SkipWorkOrderGeneration = true,
                        SkipShippingOrderGeneration = true,
                        AgreementDetailWithOfferDefinitions = new AgreementDetailWithOfferDefinitionsCollection()
                        {
                            new AgreementDetailWithOfferDefinitions()
                            {
                                AgreementDetail = new AgreementDetail()
                                {
                                    CustomerId = customer_id,
                                    AgreementId = agreement_id,
                                    ChargePeriod = ChargePeriods.Monthly,
                                    CommercialProductId = commercial_product_id,
                                    // DevicesOnHand : For software product, put false, for hardware product, put true.
                                    DevicesOnHand = false,
                                    // Quantity : Please do not put number more than 1. It must be 1 only.
                                    Quantity = 1,  
                                    // FinanceOptionId : 2 is Subscription for software product, if you need add hardware product, you need use 1 (Sold) or 3 (Rent).
                                    FinanceOptionId = financeoption,
                                    BusinessUnitId = business_unit_id,
                                    DisconnectionSetting = PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.DisconnectionDateSettings.QuoteBased
                                },
                            }
                        }

                    };

                    var result = agService.ManageProductCapture(param);

                    if (result.AgreementDetails.Items.Count == 0)
                    {
                        Console.WriteLine("Warnings : " + result.WarningMessage);

                        Console.WriteLine("Something bad happened. Sandbox rolled back.");

                        //The "false" value in this line of code rolls back the sandbox.
                        sandboxService.FinalizeSandbox(sandbox_id, false);
                        return 0;

                    }
                    else
                    {
                        //The "true" value in this line of code commits the sandbox.
                        sandboxService.FinalizeSandbox(sandbox_id, true);

                        foreach (var item in result.AgreementDetails)
                        {
                            Console.WriteLine("Congratulations! New Product ID = {0}.", item.Id);
                        }
                        return result.AgreementDetails.Items[0].Id.Value;
                    }
                }
                else
                {
                    Console.WriteLine("Can't find agreement for customer id : " + customer_id);
                    return 0;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Exception Stack : " + ex.StackTrace);
                return 0;
            }


        }
        static void addAddress(int address_type, string postcode, string addr1, string addr2, int customerid, string mobile, string homephone, string workphone)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            //This is the location of the ASM services.
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            ICustomersService custService = null;
            ICustomersConfigurationService custcService = null;
            custService = AsmRepository.AllServices.GetCustomersService(ah);
            custcService = AsmRepository.AllServices.GetCustomersConfigurationService(ah);

            var va = custcService.GetValidAddresses(new BaseQueryRequest()
            {
                FilterCriteria = new CriteriaCollection() {
                    new Criteria() {
                        Key = "PostalCode",
                        Operator=Operator.Equal,
                         Value = postcode
                    }
                }

            });

            Address addr = new Address();
            addr.Street = va.Items[0].Street;
            addr.BigCity = va.Items[0].BigCity;
            addr.SmallCity = va.Items[0].SmallCity;
            addr.CustomerId = customerid;

            if (address_type == 1)
            {
                // Billing address
                addr.Type = AddressTypes.Billing;
            }
            else if (address_type == 2)
            {
                // Billing address
                addr.Type = AddressTypes.WorkOrder;
            }


            else if (address_type == 3)
            {
                // Billing address
                addr.Type = AddressTypes.Agreement;
            }


            // Work Order Address
            //addr.Type = AddressTypes.WorkOrder;

            addr.CountryId = va.Items[0].CountryId;
            addr.ProvinceKey = va.Items[0].ProvinceId;
            addr.Extra = addr1;
            addr.ExtraExtra = addr2;
            addr.Fax2 = mobile;
            addr.HomePhone = homephone;
            addr.WorkPhone = workphone;
            addr.PropertyTypeId = 1;
            addr.TitleId = 1;
            addr.EntityId = 1;
            addr.ValidAddressId = va.Items[0].Id;
            addr.Directions = "directions";

            custService.CreateAddress(addr, 0);

        }
        static void getTimeslotbyPostalcode(int address_id, int service_type_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            IWorkforceService woService = AsmRepository.AllServices.GetWorkforceService(ah);

            var sp = woService.FindSPServiceWithGeoInfo(new FindSPServiceCriteria()
            {
                AddressId = address_id,
                ServiceTypeId = service_type_id,
                Active = true,
                PageSize = 100
            },
            1);

            foreach (var serviceprovider in sp.Items)
            {
                Console.WriteLine("addres id = " + address_id);
                Console.WriteLine("Service Provider Id : " + serviceprovider.ServiceProviderId + "  Name : " + serviceprovider.ServiceProviderName + "  Service Type Id : " + serviceprovider.ServiceTypeId + " Service ID : " + serviceprovider.ServiceId + " Geo Group Id :" + serviceprovider.GeoDefinitionGroupId);

                var sps = woService.GetServiceProviderServiceByServiceTypeGeoDefGroupIdandProviderId(serviceprovider.ServiceTypeId.Value, serviceprovider.GeoDefinitionGroupId.Value, serviceprovider.ServiceProviderId.Value);
                var timeslot = woService.GetTimeSlotsByServiceProviderServiceId(sps.Id.Value, DateTime.Now);
                // print the timeslot for this service
                for (int i = 0; i < timeslot.Length; i++)
                {
                    Console.WriteLine("Date :D " + timeslot[i].Date + "  Time Slot ID : " + timeslot[i].TimeSlotId + " Service Provider Service Id : " + timeslot[i].ServiceProviderServiceId + "Remain Slot : " + timeslot[i].RemainingCapacity + " Is Avaiable : " + timeslot[i].IsAvailable);
                }
            }

        }
        static int getBubyZipcode(string postcode)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            //This is the location of the ASM services.
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            int buid = 0;
            var buService = AsmRepository.AllServices.GetBusinessUnitsService(ah);
            var bucService = AsmRepository.AllServices.GetBusinessUnitsConfigurationService(ah);
            var bus = buService.GetBusinessUnits(new AddressCriteria()
            {
                CountryId = 1,
                PostalCode = postcode
            });

            foreach (var bu in bus)
            {

                var businessunit = bucService.GetBusinessUnitById(Int32.Parse(bu.Key.ToString()));
                if (businessunit.Attributes.Items.Count == 4)
                {
                    buid = businessunit.Id.Value;
                    Console.WriteLine("Business Unit : " + businessunit.Description);
                    break;
                }

            }
            return buid;

        }


        public void getCustomerBySerialNumberDevice()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
            var customersService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersService>(ah);

            string serialNumber = "2000000000010";
            FindCustomerCollection customers = customersService.GetCustomerBySerialNumber(serialNumber);
            if (customers != null && customers.Items.Count > 0)
            {
                foreach (FindCustomer c in customers)
                    Console.WriteLine("Customer ID = {0}", c.Id);
            }
            else
            {
                Console.WriteLine("Cannot find customer ID");
            }
            Console.ReadLine();
        }


        public void getCustomerDeviceView()
        {
            #region Here are the parameters you can use with GetCustomerDeviceView()
            //Note: Parameters are correct as of MR22.
            //Use Intellisense or check the API Reference Libarry for the latest parameters.
            //CustomerDeviceView d = new CustomerDeviceView();
            //d.AgreementDetailId;
            //d.CommercialProductName;
            //d.CustomerId;
            //d.DeviceId;
            //d.DeviceLinkDate;
            //d.DeviceLocationComment;
            //d.DevicePerProductId;
            //d.DeviceStatusId;
            //d.DeviceStatusName;
            //d.EventSettingId;
            //d.ExternalId;
            //d.IsCurrent;
            //d.ModelDescription;
            //d.ModelId;
            //d.PairedExternalId;
            //d.PairedProductId;
            //d.PreferredModel;
            //d.PreferredModelId;
            //d.ProductId;
            //d.SerialNumber;
            //d.
            #endregion

            BaseQueryRequest viewRequest = new BaseQueryRequest();
            viewRequest.PageCriteria = new PageCriteria { Page = 0 };
            CriteriaCollection viewCriteria = new CriteriaCollection();
            viewCriteria.Add("DeviceStatusId", 2);

            viewRequest.FilterCriteria = viewCriteria;
            {
                CustomerDeviceViewCollection cdvc = GetCustDeviceView(viewRequest);
                if (cdvc != null && cdvc.Items.Count > 0)
                {
                    foreach (CustomerDeviceView device in cdvc.Items)
                    {
                        Console.WriteLine(string.Format("Found Device ID: {0} for Customer ID {1}", device.DeviceId, device.CustomerId));
                        Console.WriteLine(string.Format("Device status: {0}. Device model: {1}", device.DeviceStatusName, device.ModelDescription));
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("Found nothing."));
                }
            }
            Console.WriteLine();
            Console.WriteLine("Press Enter to close.");
            Console.ReadLine();
        }
        private static CustomerDeviceViewCollection GetCustDeviceView(object viewRequest)
        {
            throw new NotImplementedException();
        }
        public static IViewFacadeService ViewFacade
        {
            get
            {
                if (m_IViewFacadeService == null)
                {
                    //TO DO: Change the authentication header parameters
                    Authentication_class var_auth = new Authentication_class();
                    AuthenticationHeader ah = var_auth.getAuthentication_header();
                    //TO DO: Change the Service Location
                    AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
                    AsmRepository.AsmOperationTimeout = 360;
                    m_IViewFacadeService = AsmRepository.GetServiceProxyCachedOrDefault<IViewFacadeService>(ah);
                }
                return m_IViewFacadeService;
            }
        }
        static CustomerDeviceViewCollection GetCustDeviceView(BaseQueryRequest baseQueryRequest)
        {
            CustomerDeviceViewCollection custDeviewViewCollection = null;
            IViewFacadeService viewFacadeService = Huda_customer_class.ViewFacade;
            custDeviewViewCollection = viewFacadeService.GetCustomerDeviceView(baseQueryRequest);
            return custDeviewViewCollection;
        }

        public void getCustomer()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            var customersService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersService>(authHeader);
            var financeService = AsmRepository.GetServiceProxyCachedOrDefault<IFinanceService>(authHeader);
            var financeConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<IFinanceConfigurationService>(authHeader);

            
            //Instantiate a BaseQueryRequest for your filter criteria.
            BaseQueryRequest request = new BaseQueryRequest();

            //Because the phone number is a string, the value entered
            //must exactly match the customer's phone number in the database.
            //For an example that searches for a matching address,
            //see the code sample called Code_FindCustomerByAddressDetails.pdf


            request.FilterCriteria = Op.Like("HomePhone", "%0%") || Op.Like("WorkPhone", "%1%") || Op.Like("Fax2", "%1%") || Op.Like("Fax1", "%1%");

            //Call the method and display the results.
            //This method searches ALL the addresses for a matching phone number.
            FindCustomerCollection customerColl = customersService.GetCustomers(request);

            if (customerColl != null)
            {
                foreach (FindCustomer c in customerColl)
                {
                    Console.WriteLine("Found Customer ID {0}, name {1} {2} {3} {4}", c.Id, c.FirstName, c.Surname, c.BigCity, c.PostalCode);
                }
            }
            else
            {
                Console.WriteLine("Cannot find a customer with that phone number.");
            }
            Console.WriteLine("total customer = {0} ----", customerColl.Count());
            //You will need to enter some error handling in case there is more than
            //one matching customer.
            Console.ReadLine();
        }

        public void getCustomerType()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
            var customersConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersConfigurationService>(ah);
            //This value is the ID (primary key) of the user-configured
            //Customer Type. To get all Customer Types, call GetCustomerTypes().
            //ICC returns a FaultException if the value does not exist.

            var products = customersConfigurationService.GetCustomerTypes(new BaseQueryRequest()
            {
                FilterCriteria = new CriteriaCollection()
                {
                    new Criteria("Id",1,Operator.Equal)
                }
            });

            Console.WriteLine("-----------------------------------------");

            foreach (var product in products.Items)
            {

                Console.WriteLine("Product ID : " + product.Id);
                Console.WriteLine("Product Name : " + product.Name);

            }


            int id = 1;
            //Call the method and return the results.
            CustomerType customerType = customersConfigurationService.GetCustomerType(id);
            Console.WriteLine("Is the '{0}' customer type Active? {1}", customerType.Name, customerType.Active);
            Console.ReadLine();
        }

        public void doSendContact()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            var ctService = AsmRepository.AllServices.GetContactsService(authHeader);
            var ctcService = AsmRepository.AllServices.GetContactsConfigurationService(authHeader);

            Contact ctt = new Contact()
            {
                CustomerId = 500039734,
                CategoryKey = 490, // Contact category Id
                MethodKey = "T",
                WorkOrderId = null,
                OrderId = null,  // Shipping Order Id
                ProductId = null,   // Commercial Product Id
                CustomerProductId = null, // Agreement Detail Id
                ProblemDescription = "This is a contact test",
                ActionTaken = "",
                DeviceId = null
            };
            int reasonid = 42579;  // The reason to create contact.
            Contact new_contact = ctService.CreateContact(ctt, reasonid);
            Console.WriteLine("idcontact = {0}", new_contact.Id);


            // Display All Category Tree
            ContactCategoryCollection category = ctcService.GetAllContactCategories();
            foreach (var cat in category)
            {
                if (cat.ParentId.Value == 0)
                {
                    Console.WriteLine(cat.Description);
                    displayNode(cat.Id.Value);
                }
            }
            Console.Read();
            Console.ReadLine();
        }
        // Display Category info
        static void displayNode(int parentId, int n = 4)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            var ctService = AsmRepository.AllServices.GetContactsService(authHeader);

            var ctcService = AsmRepository.AllServices.GetContactsConfigurationService(authHeader);
            var cc = ctcService.GetContactCategories(parentId);
            foreach (var cc1 in cc.Items)
            {
                Console.WriteLine("{0," + n + "}{1} -- {2}", " ", cc1.Id.Value, cc1.Description);
                displayNode(cc1.Id.Value, n + 4);
            }

        }

        // Display All Contact Categories
        public void displayAllCategory()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            var ctService = AsmRepository.AllServices.GetContactsService(authHeader);

            var ctcService = AsmRepository.AllServices.GetContactsConfigurationService(authHeader);

            // Display All Category Tree
            var category = ctcService.GetAllContactCategories();
            foreach (var cat in category)
            {
                if (cat.ParentId.Value == 0 && cat.Active == true)
                {
                    Console.WriteLine(cat.Description);
                    displayNode(cat.Id.Value);
                }
            }
        }

        
    }
}
