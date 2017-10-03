using log4net.Core;
using log4net;
using PayMedia.ApplicationServices.AgreementManagement.ServiceContracts;
using PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.BillingEngine.ServiceContracts;
using PayMedia.ApplicationServices.BillingEngine.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.ClientProxy;
using PayMedia.ApplicationServices.Contacts.ServiceContracts;
using PayMedia.ApplicationServices.Contacts.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.Customers.ServiceContracts;
using PayMedia.ApplicationServices.Finance.ServiceContracts;
using PayMedia.ApplicationServices.ProductCatalog.ServiceContracts;
using PayMedia.ApplicationServices.SandBoxManager.ServiceContracts;
using PayMedia.ApplicationServices.SharedContracts;
using PayMedia.ApplicationServices.DocumentManagement.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayMedia.ApplicationServices.Pricing.ServiceContracts;
using PayMedia.ApplicationServices.Workforce.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.Workforce.ServiceContracts;
using System.Reflection;
using PayMedia.ApplicationServices.Customers.ServiceContracts.DataContracts;
using System.Collections;

using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;
using PayMedia.ApplicationServices.DocumentManagement.ServiceContracts.DataContracts;
using static tes2_huda.Pending_quote;
using PayMedia.ApplicationServices.OrderManagement.ServiceContracts.DataContracts;
using log4net.Repository.Hierarchy;
using PayMedia.ApplicationServices.OrderManagement.ServiceContracts;
using PayMedia.ApplicationServices.ViewFacade.ServiceContracts;
using PayMedia.ApplicationServices.Devices.ServiceContracts;
using PayMedia.ApplicationServices.ViewFacade.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.Devices.ServiceContracts.DataContracts;
using System.Net;
using Newtonsoft.Json;

using Excel = Microsoft.Office.Interop.Excel;
using OfficeOpenXml.Core.ExcelPackage;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using System.Data.OleDb;
using PayMedia.ApplicationServices.OrderableEvent.ServiceContracts;
using PayMedia.ApplicationServices.PriceAdjustment.ServiceContracts;
using PayMedia.ApplicationServices.InvoiceRun.ServiceContracts;
using PayMedia.ApplicationServices.ProductCatalog.ServiceContracts.DataContracts;

namespace tes2_huda
{
    class Yang_class
    {
        static StringBuilder sb = new StringBuilder();

        static string createText = null;
        static string createText_temp = null;
        static string path = @"c:\Log_folder_payment_icc_job\";

        static string full_path_file = null;
        static string file_log_name = null;


        public string msg = null;

        private ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        IOrderManagementService soService = null;
        ISandBoxManagerService sbService = null;
        IFinanceService faService = null;
        IAgreementManagementService agService = null;
        IDevicesService deviceService = null;
        IViewFacadeService viewfService = null;
        //string msg = null;
        string QuoteAccountTypeId = "1";
        //string WalletAccountTypeId = "2";
        const int SOPENDING = 1;
        const int SOMAYSHIP = 21;
        const int DECODER_TP_ID = 3;   // 3 Decoder SD, 2 : Decoder HD
        const int DECODER_HD_TP_ID = 2;
        const int SCTPID = 7;
        const string DEVICE_STATUS_STOCK = "1";
        const string DEVICE_STATUS_REPAIREDSTOCK = "21";
        const string DEVICE_STATUS_REFURSTOCK = "22";
        const int DECODER_MODEL_ID = 34;   // Put the real decoder model id here
        const int SC_MODEL_ID = 48;
        const int FORENT = 3;
        const int mayship_reason = 59331;
        const int shipso_reason = 99122;
        const int internet_router_tp_id = 10;
        const int xl_sim_tp_id = 12;
        const int indosat_sim_tp_id = 13;
        const int xl_sim_model_id = 55;
        const int indosat_sim_model_id = 56;
        const int router_model_id = 53;
        const int lnb_tp_id = 4; //lnb_tp_id
        const int antenna_tp_id = 30;
        const int DECODER_HD_MODEL_ID = 1;


        

        public void Yang_product_price()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();

            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            var agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
            var agcService = AsmRepository.AllServices.GetAgreementManagementConfigurationService(authHeader);
            var productService = AsmRepository.AllServices.GetProductCatalogService(authHeader);
            var productcService = AsmRepository.AllServices.GetProductCatalogConfigurationService(authHeader);
            var custcService = AsmRepository.AllServices.GetCustomersConfigurationService(authHeader);
            var fincService = AsmRepository.AllServices.GetFinanceConfigurationService(authHeader);

            var pl = productcService.GetCommercialProducts(new BaseQueryRequest()
            {
                FilterCriteria = Op.Le("SellFrom", DateTime.Now) && Op.Ge("SellTo", DateTime.Now),
                DeepLoad = true,
                PageCriteria = new PageCriteria()
                {
                    Page = 0
                }

            });


            Console.WriteLine("Total Commercial Product Count : " + pl.TotalCount);

            foreach (var pr in pl.Items)
            {
                int commercial_product_id = pr.Id.Value;
                var prices = productcService.GetListPrices(new BaseQueryRequest()
                {
                    FilterCriteria = Op.Eq("UsedBy", 0) && Op.Eq("Active", true) && Op.Eq("ApplyToId", commercial_product_id),
                    DeepLoad = true,
                    PageCriteria = new PageCriteria()
                    {
                        Page = 0
                    }

                });

                Console.WriteLine("Product Category ID : " + pr.CategoryId + "\n");
                Console.WriteLine("Product Category Name : " + productcService.GetCommercialProductCategory(pr.CategoryId.Value).Name + "\n");
                Console.WriteLine("Product ID : " + pr.Id + "\n");
                Console.WriteLine("Product Name : " + pr.Name + "\n");


                sb.Append("Product Category ID : " + pr.CategoryId + Environment.NewLine);
                sb.Append("Product Category Name : " + productcService.GetCommercialProductCategory(pr.CategoryId.Value).Name + Environment.NewLine);
                sb.Append("Product ID : " + pr.Id + Environment.NewLine);
                sb.Append("Product Name : " + pr.Name + Environment.NewLine);

                checking_file_log();
                write_info_to_log_file();

                foreach (var ccc in pr.CommercialProductIds)
                {
                    Console.WriteLine("value = {0}" + ccc.ToString());
                    sb.Append("value = {0}" + ccc.ToString() + Environment.NewLine);

                    checking_file_log();
                    write_info_to_log_file();
                }


                Console.WriteLine("Start to search all prices for commecial products : " + pr.Name + "\n");
                sb.Append("Start to search all prices for commecial products : " + pr.Name + Environment.NewLine);

                checking_file_log();
                write_info_to_log_file();

                if (prices == null)
                {
                    continue;
                }

                foreach (var listprice in prices.Items)
                {
                    string msg = "";

                    if (listprice.PriceConditions == null)
                    {
                        continue;
                    }

                    foreach (var pricecondition in listprice.PriceConditions.Items)
                    {
                        msg = "";
                        if (pricecondition.Active == false)
                        {
                            continue;
                        }

                        // Type - OnceOff, Recurrence
                        msg = " Price Type : " + pricecondition.Type + "\n";
                        msg = msg + " Ledger Account Name : " + fincService.GetLedgerAccount(pricecondition.LedgerAccountId.Value).Description + "\n";


                        // Charge Period

                        if (pricecondition.ChargePeriodId != 0)
                        {
                            msg = msg + " Charge Period " + pricecondition.ChargePeriodId + " Month" + "\n";
                        }

                        // The price amount
                        if (pricecondition.PriceAmounts != null)
                        {
                            string priceamount = "";
                            foreach (var pa in pricecondition.PriceAmounts)
                            {
                                priceamount += " Amount : " + pa.Amount + " From " + pa.FromDate + ",";
                            }

                            if (priceamount != "")
                            {
                                msg = msg + " " + priceamount + "\n";
                            }

                            priceamount = "";
                        }


                        // Allowed Agreement Type conditions

                        if (pricecondition.AgreementTypeIds != null)
                        {
                            string agreement_type_list = "";
                            foreach (var agtid in pricecondition.AgreementTypeIds)
                            {
                                agreement_type_list += agcService.GetAgreementType(agtid).Description + ",";

                            }
                            if (agreement_type_list != "")
                            {
                                msg = msg + "Allowed Agreement Types :  " + agreement_type_list + "\n";
                            }
                            agreement_type_list = "";
                        }

                        // Allowed Business Unit Attributes

                        if (pricecondition.BusinessUnitAttributeValues != null)
                        {
                            string business_unit_attribute = "";
                            foreach (var bu in pricecondition.BusinessUnitAttributeValues)
                            {
                                business_unit_attribute += bu + ",";
                            }
                            if (business_unit_attribute != "")
                            {
                                msg = msg + "   Allowed Business Unit Attribute : " + business_unit_attribute + "\n";
                            }
                            business_unit_attribute = "";
                        }

                        // Allowed Charge Period

                        if (pricecondition.ChargePeriodId != null)
                        {
                            string charge_period = "";
                            if (pricecondition.ChargePeriodId == 1)
                            {
                                charge_period = "Monthly";
                            }

                            if (pricecondition.ChargePeriodId == 3)
                            {
                                charge_period = "Quauterly";
                            }

                            if (pricecondition.ChargePeriodId == 6)
                            {
                                charge_period = "HalfYearly";
                            }
                            if (pricecondition.ChargePeriodId == 12)
                            {
                                charge_period = "Yearly";
                            }
                            if (charge_period != "")
                            {
                                msg = msg + "   Allowed Charge Period : " + charge_period + "\n";
                            }
                            charge_period = "";
                        }


                        // Allowed Country

                        if (pricecondition.CountryIds != null)
                        {
                            string country_list = "";
                            foreach (var c in pricecondition.CountryIds)
                            {
                                country_list += custcService.GetCountry(c).Description + ",";
                            }
                            if (country_list != "")
                            {
                                msg = msg + " Allowed Country : " + country_list + "\n";
                            }
                            country_list = "";
                        }



                        // Allowed Currency

                        if (pricecondition.CurrencyId != null)
                        {
                            string currency = "";

                            currency = fincService.GetCurrency(pricecondition.CurrencyId.Value).Description;

                            if (currency != "")
                            {
                                msg = msg + "  Allowed Currency : " + currency + "\n";
                            }
                            currency = "";
                        }

                        // Allowed Customer Class

                        if (pricecondition.CustomerClassIds != null)
                        {
                            string customer_class = "";

                            foreach (var ci in pricecondition.CustomerClassIds)
                            {
                                customer_class += custcService.GetCustomerClass(ci).Description + ",";
                            }
                            if (customer_class != "")
                            {
                                msg = msg + "  Allowed Customer Class : " + customer_class + "\n";
                            }
                            customer_class = "";
                        }

                        // Allowed Customer Type

                        if (pricecondition.CustomerTypeIds != null)
                        {
                            string customer_type = "";

                            foreach (var ct in pricecondition.CustomerTypeIds)
                            {
                                customer_type += custcService.GetCustomerType(ct).Name + ",";
                            }
                            if (customer_type != null)
                            {
                                msg = msg + "  Allowed Customer Type : " + customer_type + "\n";
                            }
                            customer_type = "";
                        }


                        // Allowed Finance Options

                        if (pricecondition.FinanceOptionIds != null)
                        {
                            string financeoption = "";

                            foreach (var fo in pricecondition.FinanceOptionIds)
                            {
                                financeoption += productcService.GetFinanceOption(fo).Description + ",";
                            }

                            if (financeoption != "")
                            {
                                msg = msg + " Allowed Finance Options : " + financeoption + "\n";
                            }

                        }

                        // Allowed Finance Account Type

                        if (pricecondition.FinancialAccountTypeIds != null)
                        {
                            string fatype = "";

                            foreach (var fatypeid in pricecondition.FinancialAccountTypeIds)
                            {
                                fatype += fincService.GetFinancialAccountType((fatypeid)).Description + ",";
                            }

                            if (fatype != "")
                            {
                                msg = msg + "  Allowed Financial Account Type : " + fatype + "\n";
                            }
                        }

                        // Allowed Postal Codes

                        if (pricecondition.PostalCodes != null)
                        {
                            string postcode = "";

                            foreach (var pcode in pricecondition.PostalCodes)
                            {
                                postcode += pcode + ",";
                            }

                            if (postcode != "")
                            {
                                msg = msg + "  Allowed Postal Code : " + postcode + "\n";
                            }
                        }


                        // Allowed Province

                        if (pricecondition.ProvinceIds != null)
                        {
                            string province = "";

                            foreach (var pro in pricecondition.ProvinceIds)
                            {
                                province += custcService.GetProvince(pro).Description + ",";
                            }

                            if (province != "")
                            {
                                msg = msg + "  Allowed Province : " + province + "\n";
                            }

                        }

                        Console.WriteLine("Product : " + pr.Name + " ---- " + msg + "\n");
                        sb.Append("Product : " + pr.Name + " ---- " + msg + Environment.NewLine);

                        checking_file_log();
                        write_info_to_log_file();
                    }

                    //Console.WriteLine("Product : " + pr.Name + " ---- " + msg);

                }

                Console.WriteLine("\n---------------------------------------------------------------\n");
                sb.Append("\n---------------------------------------------------------------\n" + Environment.NewLine + Environment.NewLine);

                checking_file_log();
                write_info_to_log_file();
            }
            Console.WriteLine("end of program");
            Console.Read();
        }

        public void Yang_offerdefinition()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();

            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);
            var offercService = AsmRepository.AllServices.GetOfferManagementConfigurationService(authHeader);
            var offerService = AsmRepository.AllServices.GetOfferManagementService(authHeader);
            var priceadService = AsmRepository.AllServices.GetPriceAdjustmentService(authHeader);
            var priceadcService = AsmRepository.AllServices.GetPriceAdjustmentConfigurationService(authHeader);
            var custcService = AsmRepository.AllServices.GetCustomersConfigurationService(authHeader);
            var productcService = AsmRepository.AllServices.GetProductCatalogConfigurationService(authHeader);

            try
            {
                var offers = offercService.GetOfferDefinitions(new BaseQueryRequest()
                {
                    FilterCriteria = Op.Eq("Active", true),
                    DeepLoad = true,
                    PageCriteria = new PageCriteria()
                    {
                        Page = 0,
                    }
                });

                Console.WriteLine("Find " + offers.TotalCount + " Promo defenitions...");

                sb.Append("Find " + offers.TotalCount  + "Promo defenitions..." + Environment.NewLine);
                checking_file_log();
                write_info_to_log_file();

                foreach (var offer in offers.Items)
                {
                    Console.WriteLine("-----------------------------------------------------------------------");
                    Console.WriteLine("Promo Id : " + offer.Id);
                    Console.WriteLine("Promo Name : " + offer.Description);
                    Console.WriteLine("Promo Start Date : " + offer.StartDate);
                    Console.WriteLine("Promo End Date : " + offer.EndDate);
                    Console.WriteLine("Promo categoryId : " + offer.CategoryId);


                    sb.Append("----------------------------------------------------------------------- " + Environment.NewLine);
                    sb.Append("Promo Id :  " + offer.Id + Environment.NewLine);
                    sb.Append("Promo Name :  " + offer.Description + Environment.NewLine);
                    sb.Append("Promo Start Date :  " + offer.StartDate + Environment.NewLine);
                    sb.Append("Promo End Date :  " + offer.EndDate + Environment.NewLine);
                    sb.Append("Promo categoryId :  " + offer.CategoryId + Environment.NewLine);

                    checking_file_log();
                    write_info_to_log_file();

                    Console.WriteLine("Can be manual captured ? : " + offer.MayGiveManually);
                    sb.Append("Can be manual captured ? :  " + offer.MayGiveManually + Environment.NewLine);
                    checking_file_log();
                    write_info_to_log_file();

                    var string_kategori = offercService.GetOfferDefinitionCategory(offer.CategoryId.Value).Description;
                    Console.WriteLine("promo kategori : " + string_kategori);
                    sb.Append("promo kategori :  " + string_kategori + Environment.NewLine);
                    checking_file_log();
                    write_info_to_log_file();

                    foreach (var pocondition in offer.AllOneNoneCondition.Items)
                    {
                        var pc = productcService.GetProductCombination(pocondition.ProductCombination.Value);
                        Console.WriteLine("Product Combination Condition : -- Type : " + pocondition.ProductCombinationConditionType + "  Name : " + pc.Description);
                        sb.Append("Product Combination Condition : -- Type : " + pocondition.ProductCombinationConditionType + "  Name : " + pc.Description + Environment.NewLine);
                        checking_file_log();
                        write_info_to_log_file();
                    }

                    var offer_adjustments = priceadcService.GetPriceAdjustmentDefinitionsForOffer(offer.Id.Value, 0);

                    Console.WriteLine(" Find total price adjustment : " + offer_adjustments.TotalCount);
                    sb.Append(" Find total price adjustment : " + offer_adjustments.TotalCount + Environment.NewLine);
                    checking_file_log();
                    write_info_to_log_file();

                    foreach (var offer_adjust in offer_adjustments.Items)
                    {

                        string condition = "";
                        string apply_level = "";
                        string price = "";
                        string charge_type = "";

                        string apply_to_customer_class = "";
                        string apply_to_customer_type = "";
                        string apply_to_product = "";



                        foreach (var cust_class in offer_adjust.ApplicableCustomerClasses)
                        {
                            apply_to_customer_class += custcService.GetCustomerClass(cust_class).Description + ",";
                        }

                        foreach (var cust_type in offer_adjust.ApplicableCustomerTypes)
                        {
                            apply_to_customer_type += custcService.GetCustomerType(cust_type).LongDescription + ",";
                        }

                        if (offer_adjust.ApplicableProducts != 0)
                        {
                            //apply_to_product = productcService.GetCommercialProduct(offer_adjust.ApplicableProducts.Value).Name;
                            var apply_to_products = productcService.GetProductCombination(offer_adjust.ApplicableProducts.Value);
                            Console.WriteLine("Combinations Count : " + apply_to_products.Combinations.TotalCount);

                            sb.Append("Combinations Count : " + apply_to_products.Combinations.TotalCount + Environment.NewLine);
                            checking_file_log();
                            write_info_to_log_file();

                            foreach (var pp in apply_to_products.Combinations.Items)
                            {
                                apply_to_product += productcService.GetCommercialProduct(pp.CommercialProductId.Value).Name + ",";
                            }
                        }

                        if (apply_to_customer_class != "")
                        {
                            condition += "  apply to customer class : " + apply_to_customer_class;
                        }

                        if (apply_to_customer_type != "")
                        {
                            condition += "  apply to customer type : " + apply_to_customer_type;
                        }

                        if (apply_to_product != "")
                        {
                            condition += "  apply to product : " + apply_to_product;
                        }

                        if (offer_adjust.Type == PriceAdjustmentTypes.PercentOff)
                        {
                            price = " discount is percent :" + offer_adjust.Value;
                        }

                        if (offer_adjust.Type == PriceAdjustmentTypes.FixedPrice)
                        {
                            price = " discount is fixed price : " + offer_adjust.Value;
                        }

                        if (offer_adjust.Type == PriceAdjustmentTypes.AmountOff)
                        {
                            price = " discount is off amount " + offer_adjust.Value;
                        }

                        if (offer_adjust.ChargeType == ChargeTypes.OnceOff)
                        {
                            charge_type = " charge type is  Once-Off";
                        }
                        else if (offer_adjust.ChargeType == ChargeTypes.Recurring)
                        {
                            charge_type = " charge type is Recurring";
                        }

                        Console.WriteLine("Apply level :  0 - ListPrice, 1 - Settlement 2 - OrderableEventDiscount 3 - PrepaidFAPayment 4 - QuoteProduct");
                        Console.WriteLine("Adjustment " + offer_adjust.Description + " apply level : " + offer_adjust.ApplyToType.ToString() + "   |   " + condition + " Price : " + price + " " + charge_type);

                        sb.Append("Apply level :  0 - ListPrice, 1 - Settlement 2 - OrderableEventDiscount 3 - PrepaidFAPayment 4 - QuoteProduct" + Environment.NewLine);
                        sb.Append("Adjustment " + offer_adjust.Description + " apply level : " + offer_adjust.ApplyToType.ToString() + "   |   " + condition + " Price : " + price + " " + charge_type + Environment.NewLine);

                        checking_file_log();
                        write_info_to_log_file();


                    }
                    Console.WriteLine("########################################################################");
                    sb.Append("########################################################################" + Environment.NewLine + Environment.NewLine + Environment.NewLine);

                    checking_file_log();
                    write_info_to_log_file();
                }

                Console.WriteLine("End");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Exception Stack : " + ex.StackTrace);
            }
            Console.Read();
        }

        public void Yang_add_product()
        {
            try
            {
                Authentication_class var_auth = new Authentication_class();
                AuthenticationHeader authHeader = var_auth.getAuthentication_header();
                AsmRepository.SetServiceLocationUrl("http://192.168.177.4/asm/all/servicelocation.svc");


                IAgreementManagementService agService = null;
                IAgreementManagementConfigurationService agcService = null;
                IProductCatalogService productService = null;
                IProductCatalogConfigurationService productcService = null;
                ICustomersConfigurationService custcService = null;
                IFinanceConfigurationService fincService = null;
                ISandBoxManagerService sandboxService = null;
                ICustomersService custService = null;
                IProductCatalogConfigurationService productCatalogConfigurationService = null;


                agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
                agcService = AsmRepository.AllServices.GetAgreementManagementConfigurationService(authHeader);
                productService = AsmRepository.AllServices.GetProductCatalogService(authHeader);
                productcService = AsmRepository.AllServices.GetProductCatalogConfigurationService(authHeader);
                custcService = AsmRepository.AllServices.GetCustomersConfigurationService(authHeader);
                fincService = AsmRepository.AllServices.GetFinanceConfigurationService(authHeader);
                sandboxService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
                custService = AsmRepository.AllServices.GetCustomersService(authHeader);
                productCatalogConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<IProductCatalogConfigurationService>(authHeader);

                int sandbox_id = sandboxService.CreateSandbox();
                var commercial_product = productcService.GetCommercialProduct(45);
                if (commercial_product == null)
                {
                    Console.WriteLine("Commercial Product with ID : " + commercial_product + " Not Exist!!!");
                    //return 0;
                }

                int business_unit_id = custService.GetCustomer(30399).BusinessUnitId.Value;

                int reason120 = 65;
                int agreement_id = 0;
                var agreements = agService.GetAgreementsForCustomerId(30399, 1);

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
                                    CustomerId = 30399,
                                    AgreementId = agreement_id,
                                    ChargePeriod = PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.ChargePeriods.Monthly,
                                    CommercialProductId = commercial_product.Id,
                                    // DevicesOnHand : For software product, put false, for hardware product, put true.
                                                                        DevicesOnHand = false,
                                    // Quantity : Please do not put number more than 1. It must be 1 only.
                                    Quantity = 1,  
                                    // FinanceOptionId : 2 is Subscription for software product, if you need add hardware product, you need use 1 (Sold) or 3 (Rent).
                                    FinanceOptionId = 2,
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
                        //return 0;

                    }
                    else
                    {
                        //The "true" value in this line of code commits the sandbox.
                        sandboxService.FinalizeSandbox(sandbox_id, true);

                        foreach (var item in result.AgreementDetails)
                        {
                            Console.WriteLine("Congratulations! New Product ID = {0}.", item.Id);
                        }
                        //return result.AgreementDetails.Items[0].Id.Value;
                    }
                }
                else
                {
                    Console.WriteLine("Can't find agreement for customer id : " + 30399);
                    //return 0;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Exception Stack : " + ex.StackTrace);
                //return 0;
            }
            Console.Read();
        }

        public void Yang_contact()
        {
            IContactsService ctService = null;
            IContactsConfigurationService ctcService = null;

            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();

            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            ctService = AsmRepository.AllServices.GetContactsService(authHeader);
            ctcService = AsmRepository.AllServices.GetContactsConfigurationService(authHeader);

            Contact ctt = new Contact()
            {
                CustomerId = 33896,
                CategoryKey = 43, // Contact category Id
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
            var new_contact = ctService.CreateContact(ctt, reasonid);
            Console.WriteLine("idcontact = {0}", new_contact.Id);


            // Display All Category Tree
            var category = ctcService.GetAllContactCategories();
            foreach (var cat in category)
            {
                if (cat.ParentId.Value == 0)
                {
                    Console.WriteLine(cat.Description);
                    displayNode(cat.Id.Value);
                }
            }
            Console.Read();
        }
        public void getcategorycontact()
        {
            IContactsService ctService = null;
            IContactsConfigurationService ctcService = null;

            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();

            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            ctService = AsmRepository.AllServices.GetContactsService(authHeader);
            ctcService = AsmRepository.AllServices.GetContactsConfigurationService(authHeader);

            // Display All Category Tree
            var category = ctcService.GetAllContactCategories();
            foreach (var cat in category)
            {
                if (cat.ParentId.Value == 0)
                {
                    Console.WriteLine(cat.Description);
                    displayNode(cat.Id.Value);
                }
            }
            Console.Read();
        }

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


        public void useCreditByAgreementDetail(int agreement_detail_id)
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
            IProductCatalogConfigurationService productCatalogConfigurationService = null;
            IPricingService priceServce = null;
            IBillingEngineService billService = null;
            IFinanceService finService = null;


            agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
            agcService = AsmRepository.AllServices.GetAgreementManagementConfigurationService(authHeader);
            productService = AsmRepository.AllServices.GetProductCatalogService(authHeader);
            productcService = AsmRepository.AllServices.GetProductCatalogConfigurationService(authHeader);
            custcService = AsmRepository.AllServices.GetCustomersConfigurationService(authHeader);
            fincService = AsmRepository.AllServices.GetFinanceConfigurationService(authHeader);
            sandboxService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
            custService = AsmRepository.AllServices.GetCustomersService(authHeader);
            priceServce = AsmRepository.AllServices.GetPricingService(authHeader);
            billService = AsmRepository.GetServiceProxy<IBillingEngineService>(authHeader);
            finService = AsmRepository.AllServices.GetFinanceService(authHeader);
            //IPrepaidConfigurationService prepaidcService = AsmRepository.GetServiceProxy<IPrepaidConfigurationService>(authHeader);
            try
            {
                int customer_id;
                int quote_id;
                var ag = agService.GetAgreementDetail(agreement_detail_id);

                if (ag == null)
                {
                    Console.WriteLine("Agreement detail id :" + agreement_detail_id + " not exist!");
                    return;
                }

                var fa = finService.GetFinancialAccountsForCustomer(ag.CustomerId.Value, new CriteriaCollection() {
                new Criteria() {
                    Key="TypeId",
                    Operator=Operator.Equal,
                    Value="1"
                    }
                }, 0);

                if (fa.Items.Count == 0)
                {
                    Console.WriteLine("There are no finanical account on customer with id : ");
                    return;
                }

                var quote = GetLastPendingQuoteByAgreementDetail(agreement_detail_id);   // method definition is blow

                List<int> products = new List<int>();

                if (quote != null)
                {


                    if (quote.QuoteStatusId != 1 || quote.LineItems == null || quote.LineItems.Items.Count == 0)
                    {
                        Console.WriteLine("quote with id :  " + quote.Id.Value + " can't being fulfilled! it is not in pending status or no line item in it!");
                        return;
                    }

                    foreach (QuoteInvoiceLineItem item in quote.LineItems)
                    {
                        if (item.EntityType == AgreementDetail.ENTITY_ID)
                        {
                            products.Add(item.EntityId.GetValueOrDefault());
                        }
                    }
                }
                else
                {
                    Console.WriteLine("There are no pending quote for product with id : " + agreement_detail_id);
                    return;
                }

                var criteria = new CreateVoucherChargesCriteria()
                {
                    CreateCharges = true,
                    AutomaticRedistributionForAccountId = fa.Items[0].Id.Value,
                    QuoteId = quote.Id.Value,
                    AutomaticRedistributionForProductIds = products,
                    CreateInvoiceAfterDistribution = true,
                    //TransferFundsToVoucherProductReason = prepaidcService.GetPrepaidDefaults().ReasonForAutomaticFundTransfer

                };

                var result = billService.CreateConditionalVoucherCharges(criteria);

                if (result == null || result.SuccessfulVoucherCharges == null || result.SuccessfulVoucherCharges.Items.Count == 0)
                {
                    if (result.FailedVoucherCharges != null && result.FailedVoucherCharges.Items.Count > 0)
                        Console.WriteLine("Failed use credit because : " + result.FailedVoucherCharges.Items[0].ValidationMessage);
                    else
                        Console.WriteLine("Failed to use credit, maybe the credit is not enough!");
                }
                else
                {
                    Console.WriteLine(" use Credit Successfully!");
                }

                Console.Read();

            }
            catch (Exception ex)
            {
                msg = "Error : " + ex.Message + " --- Exceptions Stack ---- ；  " + ex.StackTrace;
                //logger.Error(msg);
                Console.WriteLine(msg);
            }

        }
        

        public long InsertPendingQuote(long max_quote_id)
        {
            int customer_id = 0;
            String start_date = null;
            String end_date = null;
            int var_break = 0;

            List<Pending_quote_data> the_list_data = new List<Pending_quote.Pending_quote_data>();
            
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            var billService = AsmRepository.GetServiceProxy<IBillingEngineService>(authHeader);
            var finService = AsmRepository.AllServices.GetFinanceService(authHeader);
            ICustomersService customerService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersService>(authHeader);

            QuoteInvoiceCollection quotes = null;
            decimal total_amount = 0;
            ArrayList falist = new ArrayList();
            long new_max_quote_id = max_quote_id;
            try
            {
                int total_record = 0;

                for (int i = 1; i < 10000; i++)
                {
                    //quotes = billService.FindQuoteInvoices(new BaseQueryRequest()
                    //{
                    //    FilterCriteria = Op.Eq("QuoteStatusId", "1") & Op.Gt("Id", max_quote_id),
                    //    PageCriteria = new PageCriteria()
                    //    {
                    //        Page = i,
                    //        PageSize = 300
                    //    },
                    //    SortCriteria = new SortCriteriaCollection()
                    //    {
                    //        new SortCriteria()
                    //            {
                    //                SortDirection = SortDirections.Ascending,
                    //                Key = "Id"
                    //            }
                    //    },
                    //    DeepLoad = false
                    //});
                    quotes = billService.FindQuoteInvoices(new BaseQueryRequest()
                    {
                        FilterCriteria = Op.Eq("QuoteStatusId", "1") & Op.Gt("Id", max_quote_id),
                        PageCriteria = new PageCriteria()
                        {
                            Page = i,
                            PageSize = 300
                        },
                        DeepLoad = false
                    });
                    Console.WriteLine("Loop " + i + " Times!");
                    if (quotes.TotalCount == 0)
                    {
                        Console.WriteLine("End Loop ...... ");
                        break;
                    }
                    // set the new max quote id for next run

                    int count = quotes.TotalCount - 1;
                    new_max_quote_id = quotes.Items[count].Id.Value;

                    foreach (var quote in quotes.Items)
                    {
                        // Avoid duplicate records
                        if (falist.Contains(quote.FinancialAccountId))
                        {
                            continue;
                        }
                        else
                        {
                            falist.Add(quote.FinancialAccountId);
                        }

                        total_amount = 0;
                        total_amount = quote.TotalAmount.Value;

                        var fa = finService.GetFinancialAccount(quote.FinancialAccountId.Value);

                        // search all pending quote for same financial account and count total
                        // Why :  Customer may have multiple pending quote if he request more products or upgrade in different time, so you need count all of them.
                        foreach (var quotet in quotes.Items)
                        {
                            if (quotet.FinancialAccountId == quote.FinancialAccountId && quote.Id != quotet.Id)
                            {
                                total_amount = total_amount + quotet.TotalAmount.Value;
                                total_record++;
                            }
                        }

                        // Add the account debit like payment fee, downgrade fee...etc. so you need to add account balance
                        total_amount = total_amount + fa.Balance.Value;

                        Console.WriteLine( quote.CustomerId.Value );

                       
                       

                        if (quote.LineItems.TotalCount > 0)
                        {
                            //Console.WriteLine("From " + quote.LineItems.Items[0].StartDate + " To " + quote.LineItems.Items[0].EndDate);
                            start_date = quote.LineItems.Items[0].StartDate.ToString();
                            end_date = quote.LineItems.Items[0].EndDate.ToString();
                        }
                        else
                        {
                            start_date = "0000-00-00";
                            end_date = "0000-00-00";
                        }
                        total_record++;

                        customer_id = quote.CustomerId.Value;

                        // get full name
                        var customersService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersService>(authHeader);
                        var customers = customersService.GetCustomer(customer_id);
                        var customer_name = customers.DefaultAddress.FirstName + " " + customers.DefaultAddress.MiddleName+ " " + customers.DefaultAddress.Surname;
                        
                        customer_name = customer_name.Trim();
                        customer_name = Regex.Replace(customer_name, @"[^\w\.@-]", "");
                        //var panjang_string = customer_name.Length;
                        //if(panjang_string > 50)
                        //{
                        //    customer_name.Substring(0, 50); //potong dan sisakan hanya 50 huruf
                        //}
                        // get full name

                        the_list_data.Add(new Pending_quote.Pending_quote_data { cust_id = customer_id ,  cust_name = customer_name, sfrom = start_date, sto = end_date, amount = total_amount , paid = "3" });
                        

                        //do_insert_data_pending_quote(customer_id, customer_name, start_date, end_date, total_amount, "0");
                    }
                    
                    
                    quotes = null;
                    
                }

                Console.WriteLine("Total pending quote " + total_record + " data");

                //OracleConnection connection = new OracleConnection();
                //connection.ConnectionString = "User Id=mnc_subscribe;Password=mncsubsd3v;Data Source=192.168.177.102:1521/MNCSVDRC"; //Data Source Format -> //IP_HOST:PORT/SERVICE_NAME e.g. //127.0.0.1:1521/Service_Name
                //connection.Open();
                //Console.WriteLine("Connected to Oracle" + connection.ServerVersion);

                // insert data to database from datalist
                //int a = 0;
                //foreach (var money in the_list_data)
                //{
                //    if (a == 10000)
                //    {
                //        var_break = 1;
                //        break;
                //    }

                //    // checking if CUST_ID value is already exist
                //    var var_exist = count_data_in_oracle(money.cust_id.ToString());
                //    //var var_exist = count_data(money.cust_id.ToString());
                //    // checking if CUST_ID value is already exist

                //    if (var_exist == 0)
                //    {
                //        //do_insert_data_pending_quote_into_mysql(money.cust_id.ToString(), money.cust_name, money.sfrom, money.sto, money.amount.ToString(), "3");
                //        do_insert_data_pending_quote_into_oracle(money.cust_id.ToString(), money.cust_name, money.sfrom, money.sto, money.amount, "3");
                //    }

                //    else if (var_exist > 0)
                //    {
                //        //update_data_on_mysql(money.cust_id, money.amount);
                //        update_data_on_oracle(money.cust_id, money.amount);
                //        Console.WriteLine("updated");
                //    }

                //    a++;
                //    Console.WriteLine(a);
                //}
                // insert data to database from datalist
                Console.WriteLine("Total amount of quote per 1 customer " + the_list_data.Count() + " data");
                Console.WriteLine("max quote id " + new_max_quote_id + " ");
                //Console.WriteLine("Total a data " + a + " ");
                return new_max_quote_id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Trace Stack : " + ex.StackTrace);
                return max_quote_id;

            }
        }

        public void do_insert_data_pending_quote_into_mysql(string cust_id_param, string cust_name_param, string start_date_param, string end_date_param, string total_amount_param, string paid_param)
        {
            string cs = @"server=192.168.177.202;userid=root;password=Password@123;database=valsys_dev";
            //string cs = @"server=192.168.177.102;userid=mnc_subscribe;password=mncsubsd3v;database=MNCT44_102";

            MySqlConnection conn = null;
            MySqlTransaction transaction = null;

            try
            {
                conn = new MySqlConnection(cs);
                conn.Open();
                transaction = conn.BeginTransaction();

                MySqlCommand command = new MySqlCommand();
                command.Connection = conn;
                command.Transaction = transaction;

                command.CommandText = @"INSERT INTO ICC_PENDING_QUOTE (CUST_ID, CUST_NAME, SFROM, STO, AMOUNT, PAID) VALUES (?CUST_ID, ?CUST_NAME, ?SFROM, ?STO, ?AMOUNT, ?PAID)";
                command.Parameters.AddWithValue("?CUST_ID", cust_id_param);
                command.Parameters.AddWithValue("?CUST_NAME", cust_name_param);
                command.Parameters.AddWithValue("?SFROM", start_date_param);
                command.Parameters.AddWithValue("?STO", end_date_param);
                command.Parameters.AddWithValue("?AMOUNT", total_amount_param);
                command.Parameters.AddWithValue("?PAID", paid_param);

                command.ExecuteNonQuery();
                transaction.Commit();

                Console.WriteLine("successfully inserted to mysql");

            }
            catch (MySqlException ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (MySqlException ex1)
                {
                    Console.WriteLine("Error: {0}", ex1.ToString());
                }

                Console.WriteLine("Error: {0}", ex.ToString());

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }
        public void do_insert_data_pending_quote_into_oracle(string cust_id_param, string cust_name_param, string start_date_param, string end_date_param, decimal total_amount_param, string paid_param)
        {
            //string cs = @"server=192.168.177.102;userid=mnc_subscribe;password=mncsubsd3v;database=MNCT44_102";
            string cs = "User Id=mnc_subscribe;Password=mncsubsd3v;Data Source=192.168.177.102:1521/MNCSVDRC";

            OracleConnection conn = null;
            OracleTransaction transaction = null;

            decimal var_round_amount = Math.Round(total_amount_param);

            //DateTime start_date_time = DateTime.ParseExact(start_date_param, "dd-MM-yy HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
            //string var_start_date_format = DateTime.ParseExact(start_date_time.ToString(), "dd-mm-yy HH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyyMMddHHmmss");

            //DateTime end_date_time = DateTime.ParseExact(end_date_param, "dd-MM-yy HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
            //string var_end_date_format = DateTime.ParseExact(end_date_time.ToString(), "dd-mm-yy HH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyyMMddHHmmss");

            try
            {
                conn = new OracleConnection(cs);
                conn.Open();
                transaction = conn.BeginTransaction();

                
                var commandText = "INSERT INTO custinq_idv_x@igateway (NOPEL, NMPEL, SFROM, STO, AMMOUNT, PAID) VALUES (:NOPEL, :NMPEL, :SFROM, :STO, :AMMOUNT, :PAID)";

                using (OracleConnection connection = new OracleConnection(cs))
                using (OracleCommand command = new OracleCommand(commandText, connection))
                {
                    command.Parameters.Add("NOPEL", cust_id_param);
                    command.Parameters.Add("NMPEL", cust_name_param);
                    command.Parameters.Add("SFROM", start_date_param);
                    command.Parameters.Add("STO", end_date_param);
                    command.Parameters.Add("AMMOUNT", var_round_amount);
                    command.Parameters.Add("PAID", 3);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                    
                }
                //transaction.Commit();

                Console.WriteLine(cust_id_param + " " + cust_name_param + " " + start_date_param + " " + end_date_param + " " + var_round_amount  + " successfully inserted to oracle");

            }
            catch (OracleException ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (OracleException ex1)
                {
                    Console.WriteLine("Error: {0}", ex1.ToString());
                }

                Console.WriteLine("Error: {0}", ex.ToString());

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }
        public void deletepending()
        {
            string cs = @"server=192.168.177.202;userid=root;password=Password@123;database=valsys_dev";

            MySqlConnection conn = null;
            MySqlTransaction transaction = null;

            try
            {
                using (var connection = new MySqlConnection(cs))
                {
                    connection.Open();
                    connection.BeginTransaction();
                    using (var command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;

                        command.CommandText = @"delete from MASTER_ADDR_ALIAS WHERE ";
                        command.Prepare();

                        //foreach (var rooker in Players)
                        //{
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@CUST_ID", 134);
                        command.Parameters.AddWithValue("@CUST_NAME", "AAAAAA");
                        command.Parameters.AddWithValue("@SFROM", "2017-05-05");
                        //command.Parameters.AddWithValue("@STO", "2017-05-05");
                        //command.Parameters.AddWithValue("@AMMOUNT", "100000");
                        //command.Parameters.AddWithValue("@PAID", "0");
                        //command.Parameters.AddWithValue("@lastLogin", rooker.LastLogin);
                        //command.Parameters.AddWithValue("@accountStatus", rooker.AccountStatus);
                        //command.Parameters.AddWithValue("@onlineStatus", rooker.OnlineStatus);
                        command.ExecuteNonQuery();
                        transaction.Commit();
                        //}
                    }
                }

                Console.WriteLine("finish insert");

            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }

            }
        }
        public void update_data_on_mysql(int cust_id_param, decimal amount_param)
        {
            string cs = @"server=192.168.177.202;userid=root;password=Password@123;database=valsys_dev";

            MySqlConnection conn = null;
            MySqlTransaction transaction = null;


            try
            {
                conn = new MySqlConnection(cs);
                conn.Open();
                transaction = conn.BeginTransaction();

                MySqlCommand command = new MySqlCommand();
                command.Connection = conn;
                command.Transaction = transaction;
                command.CommandText = @"UPDATE ICC_PENDING_QUOTE SET AMOUNT='" + amount_param + "'  WHERE CUST_ID='" + cust_id_param + "'";

                command.ExecuteNonQuery();
                transaction.Commit();

                
                //Console.WriteLine("successfully UPDATED");

            }
            catch (MySqlException ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (MySqlException ex1)
                {
                    Console.WriteLine("Error: {0}", ex1.ToString());
                }

                Console.WriteLine("Error: {0}", ex.ToString());

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        public void update_data_on_oracle(int cust_id_param, decimal amount_param)
        {
            string cs = "User Id=mnc_subscribe;Password=mncsubsd3v;Data Source=192.168.177.102:1521/MNCSVDRC";

            OracleConnection conn = null;
            OracleTransaction transaction = null;


            try
            {
                using (OracleConnection con = new OracleConnection(cs))
                {
                    con.Open();
                    OracleCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "update custinq_idv_x@igateway set AMMOUNT = :param1, PAID = :param2 where NOPEL = :keyValue";
                    cmd.Parameters.Add("param1", amount_param);
                    cmd.Parameters.Add("param2", "99");
                    cmd.Parameters.Add("keyValue", cust_id_param);
                    cmd.ExecuteNonQuery();
                    
                }
                //transaction.Commit();
                
            }
            catch (OracleException ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (OracleException ex1)
                {
                    Console.WriteLine("Error: {0}", ex1.ToString());
                }

                Console.WriteLine("Error: {0}", ex.ToString());

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        public int count_data(string cust_id_param)
        {
            string cs = @"server=192.168.177.202;userid=root;password=Password@123;database=valsys_dev";

            MySqlConnection conn = null;
            MySqlTransaction transaction = null;

            int count = 0;
            var data_exist = 0;
            try
            {
                conn = new MySqlConnection(cs);
                conn.Open();
                transaction = conn.BeginTransaction();

                string commandLine = "SELECT COUNT(*) FROM ICC_PENDING_QUOTE WHERE CUST_ID='" + cust_id_param +"'";

                using (MySqlConnection connect = new MySqlConnection(cs))
                using (MySqlCommand cmd = new MySqlCommand(commandLine, connect))
                {
                    connect.Open();

                    count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count == 0)
                        data_exist = 0;
                    else if (count > 0)
                        data_exist = 1;

                    
                }
                transaction.Commit();
                
            }
            catch (MySqlException ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (MySqlException ex1)
                {
                    Console.WriteLine("Error: {0}", ex1.ToString());
                }

                Console.WriteLine("Error: {0}", ex.ToString());

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            

            return data_exist;
        }
        public int count_data_in_oracle(string cust_id_param)
        {
            string cs = "User Id=mnc_subscribe;Password=mncsubsd3v;Data Source=192.168.177.102:1521/MNCSVDRC";

            OracleConnection conn = null;
            OracleTransaction transaction = null;
            
            //int count = 0;
            int data_exist = 1;
            try
            {
                conn = new OracleConnection(cs);
                conn.Open();

                //OracleCommand command = new OracleCommand();
                //command.CommandText = "Select count(*) from custinq_idv_x where NOPEL = :SomeValue";
                //command.CommandType = CommandType.Text;
                
               

                var commandText = "Select count(*) from custinq_idv_x@igateway where NOPEL = :SomeValue";

                using (OracleConnection connection = new OracleConnection(cs))
                using (OracleCommand command = new OracleCommand(commandText, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add("SomeValue", cust_id_param);
                    command.Connection.Open();




                    //command.ExecuteNonQuery();
                    object count = command.ExecuteScalar();
                    

                    if (count.ToString() == "0") //kondisi jika blm ada data
                        data_exist = 0;

                    command.Connection.Close();

                }

            }
            catch (OracleException ex)
            {
                try
                {
                    transaction.Rollback();


                }
                catch (OracleException ex1)
                {
                    Console.WriteLine("Error: {0}", ex1.ToString());
                }

                Console.WriteLine("Error: {0}", ex.ToString());

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }


            return data_exist;
        }
        public void do_insert_max_quote_id_to_table(long max_quote_id_param)
        {
            string cs = @"server=192.168.177.202;userid=root;password=Password@123;database=valsys_dev";

            MySqlConnection conn = null;
            MySqlTransaction transaction = null;

            try
            {
                conn = new MySqlConnection(cs);
                conn.Open();
                transaction = conn.BeginTransaction();

                MySqlCommand command = new MySqlCommand();
                command.Connection = conn;
                command.Transaction = transaction;

                command.CommandText = @"INSERT INTO MAX_QUOTE_ID (MAX_QUOTE) VALUES (?MAX_QUOTE)";
                command.Parameters.AddWithValue("?MAX_QUOTE", max_quote_id_param);

                command.ExecuteNonQuery();
                transaction.Commit();

                Console.WriteLine("successfully inserted to mysql");

            }
            catch (MySqlException ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (MySqlException ex1)
                {
                    Console.WriteLine("Error: {0}", ex1.ToString());
                }

                Console.WriteLine("Error: {0}", ex.ToString());

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }
        public void do_update_max_quote_id_to_table(long max_quote_id_param)
        {
            string cs = @"server=192.168.177.202;userid=root;password=Password@123;database=valsys_dev";

            MySqlConnection conn = null;
            MySqlTransaction transaction = null;

            try
            {
                conn = new MySqlConnection(cs);
                conn.Open();
                transaction = conn.BeginTransaction();

                MySqlCommand command = new MySqlCommand();
                command.Connection = conn;
                command.Transaction = transaction;
                command.CommandText = @"UPDATE MAX_QUOTE_ID SET MAX_QUOTE_ID='" + max_quote_id_param + "'  WHERE ID='" + 1 + "'";

                command.ExecuteNonQuery();
                transaction.Commit();

                Console.WriteLine("successfully updated to mysql");

            }
            catch (MySqlException ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (MySqlException ex1)
                {
                    Console.WriteLine("Error: {0}", ex1.ToString());
                }

                Console.WriteLine("Error: {0}", ex.ToString());

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }
        
        // Get pending quote by agreement_detail_id  
        public QuoteInvoice GetLastPendingQuoteByAgreementDetail(int agreement_detail_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://192.168.177.4/asm/all/servicelocation.svc");


            IAgreementManagementService agService = null;
            IAgreementManagementConfigurationService agcService = null;
            IProductCatalogService productService = null;
            IProductCatalogConfigurationService productcService = null;
            ICustomersConfigurationService custcService = null;
            IFinanceConfigurationService fincService = null;
            ISandBoxManagerService sandboxService = null;
            ICustomersService custService = null;
            IFinanceService finService = null;
            IBillingEngineService billService = null;
            IProductCatalogConfigurationService productCatalogConfigurationService = null;


            agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
            agcService = AsmRepository.AllServices.GetAgreementManagementConfigurationService(authHeader);
            productService = AsmRepository.AllServices.GetProductCatalogService(authHeader);
            productcService = AsmRepository.AllServices.GetProductCatalogConfigurationService(authHeader);
            custcService = AsmRepository.AllServices.GetCustomersConfigurationService(authHeader);
            fincService = AsmRepository.AllServices.GetFinanceConfigurationService(authHeader);
            finService = AsmRepository.AllServices.GetFinanceService(authHeader);
            sandboxService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
            custService = AsmRepository.AllServices.GetCustomersService(authHeader);
            billService = AsmRepository.GetServiceProxy<IBillingEngineService>(authHeader);
            productCatalogConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<IProductCatalogConfigurationService>(authHeader);
            try
            {

                QuoteInvoice qi = null;
                var ag = agService.GetAgreementDetail(agreement_detail_id);

                if (ag == null)
                {
                    Console.WriteLine("Agreement detail id :" + agreement_detail_id + " not exist!");
                    return null;
                }

                var fa = finService.GetFinancialAccountsForCustomer(ag.CustomerId.Value, new CriteriaCollection() {
                new Criteria() {
                    Key="TypeId",
                    Operator=Operator.Equal,
                    Value="1"
                    }
                }, 0).Items[0];

                Console.WriteLine("Financial Account Id : " + fa.Id.Value);

                var quoteinvoice = billService.FindQuoteInvoices(new BaseQueryRequest()
                {
                    FilterCriteria = Op.Eq("FinancialAccountId", fa.Id.Value) & Op.Eq("QuoteStatusId", "1")
                });

                foreach (var quote in quoteinvoice)
                {
                    string quote_type = "";
                    Console.WriteLine("Find quote Id : " + quote.Id.Value);
                    if (quote.QuoteGenerationMethod == QuoteGenerationMethod.EventBased)
                    {
                        quote_type = "Immediate Quote";
                    }
                    else if (quote.QuoteGenerationMethod == QuoteGenerationMethod.ServiceRetention)
                    {
                        quote_type = "Regular Quote";
                    }
                    Console.WriteLine("Quote Type" + quote_type);

                    if (quote.LineItems.Items.Count > 0)
                    {
                        foreach (var item in quote.LineItems.Items)
                        {
                            if (item.EntityId == agreement_detail_id)
                            {
                                qi = quote;
                            }
                        }
                    }

                }

                return qi;

            }
            catch (Exception ex)
            {
                msg = "Error : " + ex.Message + " --- Exceptions Stack ---- ；  " + ex.StackTrace;
                //logger.Error(msg);
                Console.WriteLine(msg);
                return null;
            }
        }

        public long GetPendingQuote(long max_quote_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://192.168.177.4/asm/all/servicelocation.svc");

            var billService = AsmRepository.GetServiceProxy<IBillingEngineService>(authHeader);
            var finService = AsmRepository.AllServices.GetFinanceService(authHeader);

            QuoteInvoiceCollection quotes = null;
            decimal total_amount = 0;
            ArrayList falist = new ArrayList();
            long new_max_quote_id = max_quote_id;
            try
            {
                int total_record = 0;

                for (int i = 1; i < 10000; i++)
                {
                    quotes = billService.FindQuoteInvoices(new BaseQueryRequest()
                    {
                        FilterCriteria = Op.Eq("QuoteStatusId", "1") & Op.Gt("Id", max_quote_id),
                        PageCriteria = new PageCriteria()
                        {
                            Page = i,
                            PageSize = 300
                        },
                        SortCriteria = new SortCriteriaCollection()
                        {
                            new SortCriteria()
                                {
                                    SortDirection = SortDirections.Ascending,
                                    Key = "Id"
                                }
                        },
                        DeepLoad = true
                    });
                    Console.WriteLine("Loop " + i + " Times!");
                    if (quotes.TotalCount == 0)
                    {
                        Console.WriteLine("End Loop ...... ");
                        break;
                    }
                    // set the new max quote id for next run

                    int count = quotes.TotalCount - 1;
                    new_max_quote_id = quotes.Items[count].Id.Value;

                    foreach (var quote in quotes.Items)
                    {
                        // Avoid duplicate records
                        if (falist.Contains(quote.FinancialAccountId))
                        {
                            continue;
                        }
                        else
                        {
                            falist.Add(quote.FinancialAccountId);
                        }

                        total_amount = 0;
                        total_amount = quote.TotalAmount.Value;

                        var fa = finService.GetFinancialAccount(quote.FinancialAccountId.Value);

                        // search all pending quote for same financial account and count total
                        // Why :  Customer may have multiple pending quote if he request more products or upgrade in different time, so you need count all of them.
                        foreach (var quotet in quotes.Items)
                        {
                            if (quotet.FinancialAccountId == quote.FinancialAccountId && quote.Id != quotet.Id)
                            {
                                total_amount = total_amount + quotet.TotalAmount.Value;
                                total_record++;
                            }
                        }

                        // Add the account debit like payment fee, downgrade fee...etc. so you need to add account balance
                        total_amount = total_amount + fa.Balance.Value;

                        Console.WriteLine("Customer Id : " + quote.CustomerId.Value + " FA ID : " + fa.Id.Value + " , unpaid quote amount : " + total_amount);
                        if (quote.LineItems.TotalCount > 0)
                        {
                            Console.WriteLine("From " + quote.LineItems.Items[0].StartDate + " To " + quote.LineItems.Items[0].EndDate);
                        }
                        total_record++;

                    }
                    quotes = null;

                }

                Console.WriteLine("Total " + total_record + " pending quotes");
                return new_max_quote_id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Trace Stack : " + ex.StackTrace);
                return max_quote_id;

            }
        }

        public decimal getPendingQuoteAmount(int customerid)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://192.168.177.4/asm/all/servicelocation.svc");

            var billService = AsmRepository.GetServiceProxy<IBillingEngineService>(authHeader);
            var finService = AsmRepository.AllServices.GetFinanceService(authHeader);

            decimal totalamount = 0;

            var fa = finService.GetFinancialAccountsForCustomer(customerid, new CriteriaCollection() {
                new Criteria() {
                    Key="TypeId",
                    Operator=Operator.Equal,
                    Value="1"
                    }
                }, 0).Items[0];

            //Console.WriteLine("Financial Account Id : " + fa.Id.Value);

            var quoteinvoice = billService.FindQuoteInvoices(new BaseQueryRequest()
            {
                FilterCriteria = Op.Eq("FinancialAccountId", fa.Id.Value) & Op.Eq("QuoteStatusId", "1")
            });

            foreach (var quote in quoteinvoice)
            {
                string quote_type = "";
                //Console.WriteLine("Find quote Id : " + quote.Id.Value);
                if (quote.QuoteGenerationMethod == QuoteGenerationMethod.EventBased)
                {
                    quote_type = "Immediate Quote";
                }
                else if (quote.QuoteGenerationMethod == QuoteGenerationMethod.ServiceRetention)
                {
                    quote_type = "Regular Quote";
                }
                //Console.WriteLine("Quote Type" + quote_type);
                totalamount = quote.TotalAmount.Value + totalamount;

            }

            Console.WriteLine("Total Amount : " + (totalamount + fa.Balance.Value));

            return totalamount + fa.Balance.Value;

        }

        public void reconnectProduct(int agreement_detail_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://192.168.177.4/asm/all/servicelocation.svc");


            IAgreementManagementService agService = null;
            IAgreementManagementConfigurationService agcService = null;
            IProductCatalogService productService = null;
            IProductCatalogConfigurationService productcService = null;
            ICustomersConfigurationService custcService = null;
            IFinanceConfigurationService fincService = null;
            ISandBoxManagerService sandboxService = null;
            ICustomersService custService = null;
            IProductCatalogConfigurationService productCatalogConfigurationService = null;


            agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
            agcService = AsmRepository.AllServices.GetAgreementManagementConfigurationService(authHeader);
            productService = AsmRepository.AllServices.GetProductCatalogService(authHeader);
            productcService = AsmRepository.AllServices.GetProductCatalogConfigurationService(authHeader);
            custcService = AsmRepository.AllServices.GetCustomersConfigurationService(authHeader);
            fincService = AsmRepository.AllServices.GetFinanceConfigurationService(authHeader);
            sandboxService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
            custService = AsmRepository.AllServices.GetCustomersService(authHeader);
            productCatalogConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<IProductCatalogConfigurationService>(authHeader);

            try
            {
                int[] agreement_details = new int[1];
                var agreement_detail = agService.GetAgreementDetail(agreement_detail_id);
                if (agreement_detail != null)
                {
                    if (agreement_detail.Status.Value == 3 || agreement_detail.Status.Value == 9)
                    {
                        agreement_details[0] = agreement_detail_id;
                        agService.ReconnectAgreementDetails(agreement_details, 116);
                        Console.WriteLine("Reconnect product with id : " + agreement_detail_id + " successfully");
                    }
                    else
                    {
                        Console.WriteLine("Product (ID : " + agreement_detail_id + ")Status is not Expired or Disconnected, can't been reconnected!");
                    }
                }
                else
                {
                    Console.WriteLine("Product with id : " + agreement_detail_id + " not exist!");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't process reconnect on " + agreement_detail_id);
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Stack : " + ex.StackTrace);
            }
        }



        public WorkOrder CompleteWorkOrderMethod()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://192.168.177.4/asm/all/servicelocation.svc");

            const int reason_complete_wo = 23035;
            int workorder_id = 7294;
            var wo = AsmRepository.AllServices.GetWorkforceService(authHeader).GetWorkOrder(workorder_id);

            var wo_Result = CompleteWorkOrder(wo, reason_complete_wo);
            return wo_Result;

        }

        public WorkOrder CompleteWorkOrder(WorkOrder wo_param, int reason_param)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
            var woService = AsmRepository.AllServices.GetWorkforceService(authHeader);
            var wo = AsmRepository.AllServices.GetWorkforceService(authHeader).GetWorkOrder(wo_param.Id.Value);
            WorkOrder wou = null;
            try
            {
                wou = woService.CompleteWorkOrder(wo, 23035);
                msg = "Work Order Completed by using reason id : " + 23035;
            }
            catch (Exception ex)
            {
                msg = "Exceptions : " + ex.Message + " Stack : " + ex.StackTrace;
                Console.WriteLine(msg);
            }
            //
            Console.WriteLine(msg);
            return wou;
        }

        // Update Work Order with time slot
        public WorkOrder UpdateWorkOrderWithTimeSlot(WorkOrder wo, int reasonid)
        {

            WorkOrder wou = null;
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);
            var custService = AsmRepository.AllServices.GetCustomersService(authHeader);
            var custcService = AsmRepository.AllServices.GetCustomersConfigurationService(authHeader);
            var woService = AsmRepository.AllServices.GetWorkforceService(authHeader);
            var wocService = AsmRepository.AllServices.GetWorkforceConfigurationService(authHeader);
            try
            {
                TimeSlotDescription[] timeslots = null;

                var va = custService.GetAddress(wo.AddressId.Value);

                var geo = custcService.FindGeoDefinitions(new GeoDefinitionCriteria()
                {
                    PostalCode = va.PostalCode
                });

                int spid = wo.ServiceProviderId.Value;
                int serviceTypeId = wo.ServiceTypeId.Value;
                int[] geos = new int[1];
                geos[0] = geo.Items[0].Id.Value;

                // Get Service Provider Service Info
                var sps = woService.GetServiceProviderServicesByServiceProviderIdServiceTypeIdAndGeoDefIds(
                    spid, serviceTypeId, geos, 0
                );

                // Get all avaiable timeslot
                foreach (var sps1 in sps.Items)
                {
                    timeslots = woService.GetTimeSlotsByServiceProviderServiceId(sps1.Id.Value, DateTime.Now);
                }

                if (timeslots == null)
                {
                    Console.WriteLine("No avaiable time slot , please check the setting! work order id : " + wo.Id.Value);
                    return null;
                }
                else
                {
                    // Here, i used the first avaiable time slot. For MNC,  please use the timeslot  that the CSR choose. It is better pass TimeSlotDescription as input parameter
                    wou = woService.UpdateWorkOrderWithStartingTimeslot(wo, new TimeSlotAllocationParameters
                    {
                        StartingTimeSlotId = timeslots[0].TimeSlotId.Value,
                        AllocationOverrideType = TimeSlotAllocationOverrideType.Capacity,
                        ServiceDate = DateTime.Now.AddDays(2), // 
                        ServiceProviderServiceId = timeslots[0].ServiceProviderServiceId.Value
                    }, reasonid);
                }

            }
            catch (Exception ex)
            {
                msg = "Exceptions : " + ex.Message + " Stack : " + ex.StackTrace;
                Console.WriteLine(msg);
            }
            msg = "Work Order " + wo.Id + " Updated by using reason id : " + reasonid;
            Console.WriteLine(msg);
            return wou;
        }

        public WorkOrder UpdateWorkOrderWithTimeSlotForReschedule(WorkOrder wo, int reasonid)
        {

            WorkOrder wou = null;
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);
            var custService = AsmRepository.AllServices.GetCustomersService(authHeader);
            var custcService = AsmRepository.AllServices.GetCustomersConfigurationService(authHeader);
            var woService = AsmRepository.AllServices.GetWorkforceService(authHeader);
            var wocService = AsmRepository.AllServices.GetWorkforceConfigurationService(authHeader);

            var wocs = AsmRepository.AllServices.GetWorkforceService(authHeader).GetWorkOrder(wo.Id.Value);  // change 123 to your work order id.

            try
            {
                TimeSlotDescription[] timeslots = null;

                var va = custService.GetAddress(wo.AddressId.Value);

                var geo = custcService.FindGeoDefinitions(new GeoDefinitionCriteria()
                {
                    PostalCode = va.PostalCode
                });

                int spid = wo.ServiceProviderId.Value;
                int serviceTypeId = wo.ServiceTypeId.Value;
                int[] geos = new int[1];
                geos[0] = geo.Items[0].Id.Value;

                // Get Service Provider Service Info
                var sps = woService.GetServiceProviderServicesByServiceProviderIdServiceTypeIdAndGeoDefIds(
                    spid, serviceTypeId, geos, 0
                );

                // Get all avaiable timeslot
                foreach (var sps1 in sps.Items)
                {
                    timeslots = woService.GetTimeSlotsByServiceProviderServiceId(sps1.Id.Value, DateTime.Now);
                }

                if (timeslots == null)
                {
                    Console.WriteLine("No avaiable time slot , please check the setting! work order id : " + wo.Id.Value);
                    return null;
                }
                else
                {
                    DateTime the_dateTime = DateTime.ParseExact("2017_10_03 01:00:00 PM", "yyyy_MM_dd hh:mm:ss tt", CultureInfo.InvariantCulture);

                    // Here, i used the first avaiable time slot. For MNC,  please use the timeslot  that the CSR choose. It is better pass TimeSlotDescription as input parameter
                    wou = woService.UpdateWorkOrderWithStartingTimeslot(wo, new TimeSlotAllocationParameters
                    {
                        StartingTimeSlotId = 3,
                        AllocationOverrideType = TimeSlotAllocationOverrideType.Capacity,

                        ServiceDate = the_dateTime, //
                        ServiceProviderServiceId = wocs.ServiceProviderServiceId // sesuaikan dengan customer
                    }, reasonid);

                    wou.ServiceDateTime = the_dateTime;

                    var wou_1 = woService.UpdateWorkOrder(wou, 20941);
                }

            }
            catch (Exception ex)
            {
                msg = "Exceptions : " + ex.Message + " Stack : " + ex.StackTrace;
                Console.WriteLine(msg);
            }
            msg = "Work Order " + wo.Id + " Updated by using reason id : " + reasonid;
            Console.WriteLine(msg);
            return wou;
        }

        //update workorder without timeslot for reschedule
        public WorkOrder UpdateWorkOrderWithOutTimeSlotForReschedule(WorkOrder wo, int reason_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);
            var woService = AsmRepository.AllServices.GetWorkforceService(authHeader);
            try
            {
                wo.ProblemDescription = wo.ProblemDescription + " updated by reason" + reason_id;
                DateTime the_dateTime = DateTime.ParseExact("2017_10_03 12:00:00 AM", "yyyy_MM_dd hh:mm:ss tt", CultureInfo.InvariantCulture);
                wo.ServiceDateTime = the_dateTime;
                WorkOrder ww = woService.UpdateWorkOrder(wo, reason_id);
                return ww;
            }
            catch (Exception ex)
            {
                msg = "Exceptions : " + ex.Message + " Stack : " + ex.StackTrace;
                Console.WriteLine(msg);
                return null;
            }

        }

        // Update Work Order without time slot
        public WorkOrder UpdateWorkOrderWithOutTimeSlot(WorkOrder wo, int reason_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
            var woService = AsmRepository.AllServices.GetWorkforceService(authHeader);
            try
            {
                wo.ProblemDescription = wo.ProblemDescription + " updated by reason" + reason_id;
                wo.ServiceDateTime = DateTime.Now.AddDays(2);
                WorkOrder ww = woService.UpdateWorkOrder(wo, reason_id);
                return ww;
            }
            catch (Exception ex)
            {
                msg = "Exceptions : " + ex.Message + " Stack : " + ex.StackTrace;
                Console.WriteLine(msg);
                return null;
            }

        }
        // Cancel Work Order
        public void CancelWorkOrder(WorkOrder wo, int reason_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
            var woService = AsmRepository.AllServices.GetWorkforceService(authHeader);
            try
            {
                woService.CancelWorkOrder(wo, reason_id);
            }
            catch (Exception ex)
            {
                msg = "Exceptions : " + ex.Message + " Stack : " + ex.StackTrace;
                Console.WriteLine(msg);

            }

        }


        //document handler
        public void linkDocument(int customer_id, string file, string file_uri, long file_size)
        {
            IDocumentManagementService docmService = null;

            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            docmService = AsmRepository.GetServiceProxy<IDocumentManagementService>(authHeader);

            try
            {
                var doc = new Document
                {
                    Name = file,
                    Uri = file_uri, // File Path , must be correct. And it must be ICC configured file server
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = 45, // ICC User ID
                    HostId = 23,      // Fixed Value 23
                    FileSize = file_size  // must be the exact file size in Bytes
                };
                var newDoc = docmService.CreateDocument(doc);

                var docLink = new DocumentLink
                {
                    DocumentId = newDoc.Id,
                    CustomerId = customer_id,
                    //DocumentType = 1
                };
                var linkReason = 0;
                var newDocLink = docmService.CreateDocumentLink(docLink, linkReason);

                Console.WriteLine("asasasasasas");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Stack : " + ex.StackTrace);
            }
        }
        
        public int getBubyZipcode(string postcode)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            int buid = 0;
            var buService = AsmRepository.AllServices.GetBusinessUnitsService(authHeader);
            var bucService = AsmRepository.AllServices.GetBusinessUnitsConfigurationService(authHeader);
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
                    Console.WriteLine("Business Unit ID: " + businessunit.Id);
                    break;
                }

            }
            return buid;

        }

        // Ship shipping order with devices
        public ShippingOrder ShipShippingOrder_ver1(int custid, string decoder_serial_number, string smartcard_serial_number, bool isInternet = false)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            var soService = AsmRepository.AllServices.GetOrderManagementService(authHeader);
            var faService = AsmRepository.AllServices.GetFinanceService(authHeader);
            var sbService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
            var agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
            var deviceService = AsmRepository.AllServices.GetDevicesService(authHeader);
            var viewfService = AsmRepository.AllServices.GetViewFacadeService(authHeader);

            msg = "Ship the shipping order...";
            Console.WriteLine(msg);
            logger.Info(msg);

            int d_tp = 0;
            int s_tp = 0;
            int d_model = 0;
            int s_model = 0;

            int lnb_model = 0;
            int antenna_model = 0;

            if (isInternet)
            {
                d_tp = internet_router_tp_id;
                s_tp = xl_sim_tp_id;
                d_model = router_model_id;
                s_model = xl_sim_model_id;
            }
            else
            {
                d_tp = DECODER_TP_ID;
                s_tp = SCTPID;
                d_model = DECODER_MODEL_ID;
                s_model = SC_MODEL_ID;
            }

            try
            {
                // get the pending shipping order and shipping order line
                var soc = soService.GetShippedOrders(custid, 0).Items.Find(t => t.StatusId == SOMAYSHIP);


                if (soc == null)
                {
                    Console.WriteLine("No Shipping Order with status May Ship, please check customer id : " + custid);
                    return null;
                }

                // Get the hardware agreement detail of customer
                AgreementDetailView hardwaread = null;
                var hardwareads = viewfService.GetAgreementDetailView(new BaseQueryRequest()
                {
                    FilterCriteria = Op.Eq("DeviceIncluded", true) & Op.Eq("CustomerId", custid) & Op.Gt("Id", 0) & Op.IsNull("ProvisionedDevices"),
                    PageCriteria = new PageCriteria(1),
                    SortCriteria = new SortCriteriaCollection(){
                        new SortCriteria()
                        {
                            Key = "Id",
                            SortDirection = SortDirections.Descending
                        }
                    }
                });

                if (hardwareads.TotalCount == 0)
                {
                    Console.WriteLine("Hardware product is not captured, can't ship the shipping order for customer id : " + custid);
                    return null;
                }
                else
                {
                    hardwaread = hardwareads.Items[0];
                }

                // Find the shipping order lines for decoder and smartcard
                var decodersoline = soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == d_tp);
                var scsoline = soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == s_tp);


                // Get random decoder and smartcard which are in stock, you should not use this since you have real device information. 
                Device decoder = null;
                Device smartcard = null;
                if (decoder_serial_number == "")
                {
                    var decoders = deviceService.GetDevices(
                        new BaseQueryRequest()
                        {
                            FilterCriteria = new CriteriaCollection()
                            {
                                new Criteria()
                                {
                                    Key="ModelId",
                                    Operator = Operator.Equal,
                                    Value = d_model.ToString()
                                },
                                new Criteria()
                                {
                                    Key="StatusId",
                                    Operator = Operator.Equal,
                                    Value = DEVICE_STATUS_STOCK
                                }
                            }
                        });
                    if (decoders.Items.Count == 0)
                    {
                        Console.WriteLine("There are no decoder avaiable to use!");
                        return null;
                    }
                    else
                    {
                        decoder_serial_number = decoders.Items[0].SerialNumber;
                    }

                }
                else
                {
                    decoder = deviceService.GetDeviceBySerialNumber(decoder_serial_number);
                    if (decoder.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || decoder.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                        || decoder.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                       )
                    {

                    }
                    else
                    {
                        Console.WriteLine(" Decoder with serial number " + decoder_serial_number + " is not in allowed capture status!");
                        return null;
                    }
                }


                if (smartcard_serial_number == "")
                {
                    var smartcards = deviceService.GetDevices(new BaseQueryRequest()
                    {
                        FilterCriteria = new CriteriaCollection()
                        {
                            new Criteria()
                            {
                                Key="ModelId",
                                Operator = Operator.Equal,
                                Value = s_model.ToString()
                            },
                            new Criteria()
                            {
                                Key="StatusId",
                                Operator = Operator.Equal,
                                Value = DEVICE_STATUS_STOCK
                            }
                        }
                    });
                    if (smartcards.Items.Count == 0)
                    {
                        Console.WriteLine("There are no smartcard avaiable to use!");
                        return null;
                    }
                    else
                    {
                        smartcard_serial_number = smartcards.Items[0].SerialNumber;
                    }

                }
                else
                {
                    smartcard = deviceService.GetDeviceBySerialNumber(smartcard_serial_number);
                    if (smartcard.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || smartcard.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                        || smartcard.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                       )
                    {

                    }
                    else
                    {
                        Console.WriteLine(" Smartcard with serial number " + smartcard_serial_number + " is not in allowed capture status!");
                        return null;
                    }
                }

                // Identify device info to the shipping order lines
                var buildListdecoder = new BuildList
                {
                    OrderLineId = decodersoline.Id,
                    OrderId = soc.Id,
                    ModelId = decodersoline.HardwareModelId.GetValueOrDefault(),
                    StockHandlerId = soc.ShipFromStockHandlerId,
                    TransactionType = BuildListTransactionType.ShippingSerializedProducts
                };

                var bldecoder = deviceService.CreateBuildList(buildListdecoder);

                var buildListsc = new BuildList
                {
                    OrderLineId = scsoline.Id,
                    OrderId = soc.Id,
                    ModelId = scsoline.HardwareModelId.GetValueOrDefault(),
                    StockHandlerId = soc.ShipFromStockHandlerId,
                    TransactionType = BuildListTransactionType.ShippingSerializedProducts
                };

                var blsc = deviceService.CreateBuildList(buildListsc);

                var d_a_list = deviceService.AddDeviceToBuildListManually(bldecoder.Id.Value, decoder_serial_number);
                var s_a_list = deviceService.AddDeviceToBuildListManually(blsc.Id.Value, smartcard_serial_number);

                if (d_a_list.Accepted.Value)
                {

                }
                else
                {
                    Console.WriteLine("Failed to attach decoder with SN : " + decoder_serial_number + " to Shipping Order : Error : " + d_a_list.Error);
                    return null;
                }

                if (s_a_list.Accepted.Value)
                {

                }
                else
                {
                    Console.WriteLine("Failed to attach smartcard with SN : " + smartcard_serial_number + " to Shipping Order : Error : " + d_a_list.Error);
                    return null;
                }

                msg = "Starting perform build list";
                Console.WriteLine(msg);
                logger.Debug(msg);

                var dblist = deviceService.PerformBuildListAction(bldecoder.Id.Value);
                var scblist = deviceService.PerformBuildListAction(blsc.Id.Value);


                // Ship the Shipping Order
                msg = "Starting link the device to customer";
                Console.WriteLine(msg);
                logger.Debug(msg);

                // Fill the agreement detail id on the shipping order line to link the device to customer
                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == d_tp).AgreementDetailId = hardwaread.Id.Value;
                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == s_tp).AgreementDetailId = hardwaread.Id.Value;

                // Fill the correct model id
                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == d_tp).HardwareModelId = d_model;
                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == s_tp).HardwareModelId = s_model;
                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == lnb_tp_id).HardwareModelId = lnb_model;
                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == antenna_tp_id).HardwareModelId = antenna_model;

                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == d_tp).ReceivedQuantity = 1;
                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == s_tp).ReceivedQuantity = 1;

                soService.ShipOrder(soc, shipso_reason, null);
                msg = "Shipping Order :" + soc.Id.Value + " has been shipped!";
                logger.Info(msg);


                return soc;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Inner Exception : " + ex.InnerException.Message);
                msg = "Errors : " + ex.Message + "  ------  Exception Stack : " + ex.StackTrace;
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Stack : " + ex.StackTrace);
                logger.Error(msg);

                return null;
            }


        }

        public ShippingOrder ShipShippingOrder_ver2(int custid, string decoder_serial_number, string smartcard_serial_number, bool isInternet, string lnb_sn , string antenna_sn, int shippingorder_id, bool isHD)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            var soService = AsmRepository.AllServices.GetOrderManagementService(authHeader);
            var faService = AsmRepository.AllServices.GetFinanceService(authHeader);
            var sbService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
            var agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
            var deviceService = AsmRepository.AllServices.GetDevicesService(authHeader);
            var viewfService = AsmRepository.AllServices.GetViewFacadeService(authHeader);

            msg = "Ship the shipping order...";
            Console.WriteLine(msg);
            logger.Info(msg);

            int lnb_model = 0;
            int antenna_model = 0;

            int d_tp = 0;
            int s_tp = 0;
            int d_model = 0;
            int s_model = 0;

            if (isInternet)
            {
                d_tp = internet_router_tp_id;
                s_tp = xl_sim_tp_id;
                d_model = router_model_id;
                s_model = xl_sim_model_id;
            }
            else
            {
                d_tp = DECODER_TP_ID;
                
                s_tp = SCTPID;
                d_model = DECODER_MODEL_ID;
                s_model = SC_MODEL_ID;
            }

            if (isHD)
            {
                //d_model = DECODER_HD_TP_ID;
                d_tp = DECODER_HD_TP_ID;
                d_model = DECODER_HD_MODEL_ID;
            }

            try
            {
                // Get the hardware agreement detail of customer
                AgreementDetailView hardwaread = null;
                var hardwareads = viewfService.GetAgreementDetailView(new BaseQueryRequest()
                {
                    FilterCriteria = Op.Eq("DeviceIncluded", true) & Op.Eq("CustomerId", custid) & Op.Gt("Id", 0) & Op.IsNull("ProvisionedDevices"),
                    PageCriteria = new PageCriteria(1),
                    SortCriteria = new SortCriteriaCollection(){
                        new SortCriteria()
                        {
                            Key = "Id",
                            SortDirection = SortDirections.Descending
                        }
                    }
                });

                if (hardwareads.TotalCount == 0)
                {
                    Console.WriteLine("Hardware product is not captured, can't ship the shipping order for customer id : " + custid);
                    return null;
                }
                else
                {
                    hardwaread = hardwareads.Items[0];
                }

                // If not input the shipping order id then find the shipping order id by the random hardware product which don't assign device info yet.
                if (shippingorder_id == 0)
                {
                    shippingorder_id = getShippingOrderByHardware(hardwaread.Id.Value);
                    Console.WriteLine("Ship the Shipping Order Id is : " + shippingorder_id);
                }
                // get the May shipping order and shipping order line( with smartcard or Sim card in it).
                var soc = soService.GetShippedOrders(custid, 0).Items.Find(t => (t.StatusId == SOMAYSHIP && t.Id.Value == shippingorder_id));


                if (soc == null)
                {
                    Console.WriteLine("No Shipping Order with status May Ship, please check customer id : " + custid);
                    return null;
                }



                // Find the shipping order lines for decoder and smartcard

                var scsoline = soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == s_tp);


                // Get random decoder and smartcard which are in stock, you should not use this since you have real device information. 
                Device decoder = null;
                Device smartcard = null;
                if (decoder_serial_number == "")
                {
                    //var decoders = deviceService.GetDevices(
                    //    new BaseQueryRequest()
                    //    {
                    //        FilterCriteria = new CriteriaCollection()
                    //        {
                    //            new Criteria()
                    //            {
                    //                Key="ModelId",
                    //                Operator = Operator.Equal,
                    //                Value = d_model.ToString()
                    //            },
                    //            new Criteria()
                    //            {
                    //                Key="StatusId",
                    //                Operator = Operator.Equal,
                    //                Value = DEVICE_STATUS_STOCK
                    //            }
                    //        }
                    //    });
                    //if (decoders.Items.Count == 0)
                    //{
                    //    Console.WriteLine("There are no decoder avaiable to use!");
                    //    return null;
                    //}
                    //else
                    //{
                    //    decoder_serial_number = decoders.Items[0].SerialNumber;
                    //}

                }
                else
                {
                    decoder = deviceService.GetDeviceBySerialNumber(decoder_serial_number);
                    if (decoder.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || decoder.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                        || decoder.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                       )
                    {

                    }
                    else
                    {
                        Console.WriteLine(" Decoder with serial number " + decoder_serial_number + " is not in allowed capture status!");
                        return null;
                    }
                }


                if (smartcard_serial_number == "")
                {
                    //var smartcards = deviceService.GetDevices(new BaseQueryRequest()
                    //{
                    //    FilterCriteria = new CriteriaCollection()
                    //    {
                    //        new Criteria()
                    //        {
                    //            Key="ModelId",
                    //            Operator = Operator.Equal,
                    //            Value = s_model.ToString()
                    //        },
                    //        new Criteria()
                    //        {
                    //            Key="StatusId",
                    //            Operator = Operator.Equal,
                    //            Value = DEVICE_STATUS_STOCK
                    //        }
                    //    }
                    //});
                    //if (smartcards.Items.Count == 0)
                    //{
                    //    Console.WriteLine("There are no smartcard avaiable to use!");
                    //    return null;
                    //}
                    //else
                    //{
                    //    smartcard_serial_number = smartcards.Items[0].SerialNumber;
                    //}

                }
                else
                {
                    smartcard = deviceService.GetDeviceBySerialNumber(smartcard_serial_number);
                    if (smartcard.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || smartcard.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                        || smartcard.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                       )
                    {

                    }
                    else
                    {
                        Console.WriteLine(" Smartcard with serial number " + smartcard_serial_number + " is not in allowed capture status!");
                        return null;
                    }
                }

                // Identify device info to the shipping order lines
                //var dd = identifyDevice(soc, decoder_serial_number, d_tp, d_model);
                //var sc = identifyDevice(soc, smartcard_serial_number, s_tp, s_model);
                if (lnb_sn.Length > 0)
                {
                    var lnb = deviceService.GetDeviceBySerialNumber(lnb_sn);
                    if (lnb.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || lnb.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                        || lnb.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                       )
                    {
                        lnb_model = lnb.ModelId.Value;
                    }
                    else
                    {
                        Console.WriteLine(" LNB with serial number " + lnb_sn + " is not in allowed capture status!");
                        //return null;
                    }
                    //identifyDevice(soc, lnb_sn, lnb_tp_id, lnb.ModelId.Value);
                }
                if (antenna_sn.Length > 0)
                {
                    var antenna = deviceService.GetDeviceBySerialNumber(antenna_sn);
                    if (antenna.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || antenna.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                        || antenna.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                       )
                    {
                        antenna_model = antenna.ModelId.Value;
                    }
                    else
                    {
                        Console.WriteLine(" Antenna with serial number " + antenna_sn + " is not in allowed capture status!");
                        //return null;
                    }
                    //identifyDevice(soc, antenna_sn, antenna_tp_id, antenna.ModelId.Value);
                }

                //if (dd == null || sc == null)
                //{
                //    return null;
                //}

                msg = "Starting perform build list";
                Console.WriteLine(msg);
                logger.Debug(msg);


                // Ship the Shipping Order
                msg = "Starting link the device to customer";
                Console.WriteLine(msg);
                logger.Debug(msg);

                // Fill the agreement detail id on the shipping order line to link the device to customer
                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == d_tp).AgreementDetailId = hardwaread.Id.Value;
                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == s_tp).AgreementDetailId = hardwaread.Id.Value;

                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == d_tp).ReceivedQuantity = 1;
                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == s_tp).ReceivedQuantity = 1;

                // Fill the agreement detail id on the shipping order line to link the device to customer antenna n lnb
                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == lnb_tp_id).AgreementDetailId = hardwaread.Id.Value;
                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == antenna_tp_id).AgreementDetailId = hardwaread.Id.Value;

                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == lnb_tp_id).ReceivedQuantity = 1;
                soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == antenna_tp_id).ReceivedQuantity = 1;

                //code for ship the so, then serial number will show in tab device
                soService.ShipOrder(soc, shipso_reason, null);
                msg = "Shipping Order :" + soc.Id.Value + " has been shipped!";
                logger.Info(msg);


                return soc;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Inner Exception : " + ex.InnerException.Message);
                msg = "Errors : " + ex.Message + "  ------  Exception Stack : " + ex.StackTrace;
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Stack : " + ex.StackTrace);
                logger.Error(msg);

                return null;
            }


        }

        public void huda_shippingOrder( int cust_id_param, int so_id_param, List<int> tech_prod_id_format_param, List<string> serial_number_param)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            var soService = AsmRepository.AllServices.GetOrderManagementService(authHeader);
            var faService = AsmRepository.AllServices.GetFinanceService(authHeader);
            var sbService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
            var agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
            var deviceService = AsmRepository.AllServices.GetDevicesService(authHeader);
            var viewfService = AsmRepository.AllServices.GetViewFacadeService(authHeader);
            var productCatalogConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<IProductCatalogConfigurationService>(authHeader);

            int lnb_model = 0;
            int antenna_model = 0;
            

            int d_tp = 0;
            int s_tp = 0;
            int d_model = 0;
            int s_model = 0;



            //check instock
            #region check instock
            int status_instock = 1;
            for (int i = 0; i < serial_number_param.Count; i++)
            {
                if (serial_number_param[i] == "")
                    continue;
                else
                {
                    Device the_device = deviceService.GetDeviceBySerialNumber(serial_number_param[i]);
                    if (the_device.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || the_device.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                                || the_device.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                               )
                    {
                        status_instock = 1;
                    }
                    else
                    {
                        status_instock = 0;
                        break;
                    }
                }

                
            }
            #endregion check instock

            


            // Get the hardware agreement detail of customer
            AgreementDetailView hardwaread = null;
            var hardwareads = viewfService.GetAgreementDetailView(new BaseQueryRequest()
            {
                FilterCriteria = Op.Eq("DeviceIncluded", true) & Op.Eq("CustomerId", cust_id_param) & Op.Gt("Id", 0) & Op.IsNull("ProvisionedDevices"),
                PageCriteria = new PageCriteria(1),
                SortCriteria = new SortCriteriaCollection(){
                        new SortCriteria()
                        {
                            Key = "Id",
                            SortDirection = SortDirections.Descending
                        }
                    }
            });

            if (hardwareads.TotalCount == 0)
            {
                Console.WriteLine("Hardware product is not captured, can't ship the shipping order for customer id : " + cust_id_param);
                //return null;
            }
            else
            {
                hardwaread = hardwareads.Items[0];


            }

            // If not input the shipping order id then find the shipping order id by the random hardware product which don't assign device info yet.
            if (so_id_param == 0)
            {
                so_id_param = getShippingOrderByHardware(hardwaread.Id.Value);
                Console.WriteLine("Ship the Shipping Order Id is : " + so_id_param);
            }


            // get the May shipping order and shipping order line( with smartcard or Sim card in it).
            var soc = soService.GetShippedOrders(cust_id_param, 0).Items.Find(t => (t.StatusId == SOMAYSHIP && t.Id.Value == so_id_param));


            if (soc == null)
            {
                Console.WriteLine("No Shipping Order with status May Ship, please check customer id : " + cust_id_param);
                //return null;
            }

            else if (soc != null)
            {
                int lnb_group = 0;
                int antenna_group = 0;
                try
                {
                    for (int i = 0; i < tech_prod_id_format_param.Count; i++)
                    {

                        //for decoder
                        if (tech_prod_id_format_param[i] == 3 || tech_prod_id_format_param[i] == 2)
                        {
                            if (serial_number_param[i] == "")
                                continue;
                            else
                            {
                                
                                string decoder_serial_number = serial_number_param[i];
                                // Get random decoder and smartcard which are in stock, you should not use this since you have real device information. 
                                Device decoder = null;
                                Device smartcard = null;

                                //decoder
                                decoder = deviceService.GetDeviceBySerialNumber(decoder_serial_number);
                                if (
                                    decoder.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || decoder.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                                        || decoder.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                                    )
                                {

                                }
                                else
                                {
                                    Console.WriteLine(" Decoder with serial number " + decoder_serial_number + " is not in allowed capture status!");
                                    //return null;
                                }

                                // Identify device info to the shipping order lines
                                //var dd = identifyDevice(soc, serial_number_param[i], tech_prod_id_format_param[i], decoder.ModelId.Value);
                            }

                        }

                        //for VC
                        else if (tech_prod_id_format_param[i] == 7)
                        {
                            if (serial_number_param[i] == "")
                                continue;
                            else
                            {
                                

                                string smartcard_serial_number = serial_number_param[i];
                                // Find the shipping order lines for decoder and smartcard

                                var scsoline = soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == s_tp); //

                                //vc
                                var smartcard = deviceService.GetDeviceBySerialNumber(smartcard_serial_number);
                                if (smartcard.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || smartcard.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                                    || smartcard.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                                   )
                                {

                                }
                                else
                                {
                                    Console.WriteLine(" Smartcard with serial number " + smartcard_serial_number + " is not in allowed capture status!");
                                    //return null;
                                }

                                //var sc = identifyDevice(soc, serial_number_param[i], tech_prod_id_format_param[i], smartcard.ModelId.Value);
                            }
                        }

                        //for antenna
                        else if (tech_prod_id_format_param[i] == 30)
                        {
                            antenna_group++;

                            if (serial_number_param[i] == "")
                                continue;
                            else
                            {
                                string antenna_sn = serial_number_param[i];

                                //antenna
                                var antenna = deviceService.GetDeviceBySerialNumber(antenna_sn);
                                if (antenna.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || antenna.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                                    || antenna.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                                   )
                                {
                                    antenna_model = antenna.ModelId.Value;
                                }
                                else
                                {
                                    Console.WriteLine(" Antenna with serial number " + antenna_sn + " is not in allowed capture status!");
                                    //return null;
                                }
                                ///identifyDevice(soc, serial_number_param[i], tech_prod_id_format_param[i], antenna.ModelId.Value);
                            }
                        }

                        //for LNB
                        else if (tech_prod_id_format_param[i] == 393 || tech_prod_id_format_param[i] == 4)
                        {
                            lnb_group++;
                            if (serial_number_param[i] == "")
                                continue;
                            else
                            {
                                string lnb_sn = serial_number_param[i];

                                //lnb
                                var lnb = deviceService.GetDeviceBySerialNumber(lnb_sn);
                                if (lnb.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || lnb.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                                    || lnb.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                                   )
                                {
                                    lnb_model = lnb.ModelId.Value;
                                }
                                else
                                {
                                    Console.WriteLine(" LNB with serial number " + lnb_sn + " is not in allowed capture status!");
                                    //return null;
                                }
                                //identifyDevice(soc, serial_number_param[i], tech_prod_id_format_param[i], lnb.ModelId.Value);
                            }

                        }


                        //for router dan simcard
                        if (tech_prod_id_format_param[i] == 10 || tech_prod_id_format_param[i] == 13 || tech_prod_id_format_param[i] == 12)
                        {

                            string inet_item_serial_number = serial_number_param[i];
                            // Get random decoder and smartcard which are in stock, you should not use this since you have real device information. 
                            Device inet_item = null;
                            Device smartcard = null;

                            //inet_item
                            inet_item = deviceService.GetDeviceBySerialNumber(inet_item_serial_number);
                            if (
                                inet_item.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || inet_item.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                                || inet_item.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                                )
                            {

                            }
                            else
                            {
                                Console.WriteLine(" inet_item with serial number " + inet_item_serial_number + " is not in allowed capture status!");
                                //return null;
                            }

                            // Identify device info to the shipping order lines
                            //var dd = identifyDevice(soc, serial_number_param[i], tech_prod_id_format_param[i], inet_item.ModelId.Value);

                        }


                        var devices_per_cust = get_device_per_cust(cust_id_param).OrderBy(t => t.AgreementDetailId).ThenBy(t => t.TechnicalProductId);
                        int ss = 0;



                        for (int a = 0; a < tech_prod_id_format_param.Count(); a++)
                        {
                            if (serial_number_param[i] == "")
                                continue;

                            if (tech_prod_id_format_param[i] == devices_per_cust.ToList()[a].TechnicalProductId )
                            {
                                ss = devices_per_cust.ToList()[a].AgreementDetailId.Value;
                            }
                            //if (tech_prod_id_format_param[i] != devices_per_cust.ToList()[a].TechnicalProductId )
                            //{
                            //    continue;
                            //}
                        }

                        if (serial_number_param[i] == "")
                            continue;
                        else
                        {
                            
                            // Fill the agreement detail id on the shipping order line to link the device to customer
                            soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == tech_prod_id_format_param[i] ).AgreementDetailId = ss; //assign sesuai commercialproductid yg terdapat di var hardwarereads

                            soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == tech_prod_id_format_param[i] ).ReceivedQuantity = 1;
                        }

                            
                        
                    }

                    msg = "Starting perform build list";
                    Console.WriteLine(msg);
                    logger.Debug(msg);



                    // Ship the Shipping Order
                    msg = "Starting link the device to customer";
                    Console.WriteLine(msg);
                    logger.Debug(msg);

                    //code for ship the so, then serial number will show in tab device
                    soService.ShipOrder(soc, shipso_reason, null);
                    msg = "Shipping Order :" + soc.Id.Value + " has been shipped!";
                    logger.Info(msg);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    msg = "Errors : " + ex.Message + "  ------  Exception Stack : " + ex.StackTrace;
                    Console.WriteLine("Errors : " + ex.Message);
                    Console.WriteLine("Stack : " + ex.StackTrace);
                    logger.Error(msg);

                    //return null;
                }

            }

            
        }
        public void huda_ship_so(int cust_id_param, int so_id_param)
        {
            try
            {
                Authentication_class var_auth = new Authentication_class();
                AuthenticationHeader authHeader = var_auth.getAuthentication_header();
                AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

                var soService = AsmRepository.AllServices.GetOrderManagementService(authHeader);
                var faService = AsmRepository.AllServices.GetFinanceService(authHeader);
                var sbService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
                var agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
                var deviceService = AsmRepository.AllServices.GetDevicesService(authHeader);
                var viewfService = AsmRepository.AllServices.GetViewFacadeService(authHeader);


                ShippingOrder soc = soService.GetShippedOrders(cust_id_param, 0).Items.Find(t => (t.StatusId == 3 && t.Id.Value == so_id_param));

                soService.ShipOrder(soc, shipso_reason, null);
                msg = "Shipping Order :" + soc.Id.Value + " has been shipped!";
                logger.Info(msg);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Inner Exception : " + ex.InnerException.Message);
                msg = "Errors : " + ex.Message + "  ------  Exception Stack : " + ex.StackTrace;
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Stack : " + ex.StackTrace);
                logger.Error(msg);
            }
            
        }

        public void huda_shippingOrder_1(int cust_id_param, int so_id_param, List<Format_tp_agdid> tech_prod_id_format_param, List<string> serial_number_param)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            var soService = AsmRepository.AllServices.GetOrderManagementService(authHeader);
            var faService = AsmRepository.AllServices.GetFinanceService(authHeader);
            var sbService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
            var agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
            var deviceService = AsmRepository.AllServices.GetDevicesService(authHeader);
            var viewfService = AsmRepository.AllServices.GetViewFacadeService(authHeader);
            var productCatalogConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<IProductCatalogConfigurationService>(authHeader);

            int lnb_model = 0;
            int antenna_model = 0;


            int d_tp = 0;
            int s_tp = 0;
            int d_model = 0;
            int s_model = 0;

            //check instock
            #region check instock
            int status_instock = 1;
            for (int i = 0; i < serial_number_param.Count; i++)
            {
                if (serial_number_param[i] == "")
                    continue;
                else
                {
                    Device the_device = deviceService.GetDeviceBySerialNumber(serial_number_param[i]);
                    if (the_device.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || the_device.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                                || the_device.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                               )
                    {
                        status_instock = 1;
                    }
                    else
                    {
                        status_instock = 0;
                        break;
                    }
                }


            }
            #endregion check instock
            
            // Get the hardware agreement detail of customer
            AgreementDetailView hardwaread = null;
            var hardwareads = viewfService.GetAgreementDetailView(new BaseQueryRequest()
            {
                FilterCriteria = Op.Eq("DeviceIncluded", true) & Op.Eq("CustomerId", cust_id_param) & Op.Gt("Id", 0) & Op.IsNull("ProvisionedDevices"),
                PageCriteria = new PageCriteria(1),
                SortCriteria = new SortCriteriaCollection(){
                        new SortCriteria()
                        {
                            Key = "Id",
                            SortDirection = SortDirections.Descending
                        }
                    }
            });

            if (hardwareads.TotalCount == 0)
            {
                Console.WriteLine("Hardware product is not captured, can't ship the shipping order for customer id : " + cust_id_param);
                //return null;
            }
            else
            {
                hardwaread = hardwareads.Items[0];
            }

            // If not input the shipping order id then find the shipping order id by the random hardware product which don't assign device info yet.
            if (so_id_param == 0)
            {
                so_id_param = getShippingOrderByHardware(hardwaread.Id.Value);
                Console.WriteLine("Ship the Shipping Order Id is : " + so_id_param);
            }


            // get the May shipping order and shipping order line( with smartcard or Sim card in it).
            var soc = soService.GetShippedOrders(cust_id_param, 0).Items.Find(t => (t.StatusId == SOMAYSHIP && t.Id.Value == so_id_param));



            if (soc == null)
            {
                Console.WriteLine("No Shipping Order with status May Ship, please check customer id : " + cust_id_param);
                //return null;
            }

            else if (soc != null)
            {
                int lnb_group = 0;
                int antenna_group = 0;
                try
                {
                    for (int i = 0; i < tech_prod_id_format_param.Count; i++)
                    {

                        if (serial_number_param[i] == "")
                            continue;
                        else
                        {
                            //for decoder
                            if (tech_prod_id_format_param[i].tp_id == 3 || tech_prod_id_format_param[i].tp_id == 2)
                            {
                                if (serial_number_param[i] == "")
                                    continue;
                                else
                                {

                                    string decoder_serial_number = serial_number_param[i];
                                    // Get random decoder and smartcard which are in stock, you should not use this since you have real device information. 
                                    Device decoder = null;
                                    Device smartcard = null;

                                    //decoder
                                    decoder = deviceService.GetDeviceBySerialNumber(decoder_serial_number);
                                    if (
                                        decoder.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || decoder.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                                            || decoder.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                                        )
                                    {

                                    }
                                    else
                                    {
                                        Console.WriteLine(" Decoder with serial number " + decoder_serial_number + " is not in allowed capture status!");
                                        //return null;
                                    }

                                    // Identify device info to the shipping order lines
                                    var dd = identifyDevice(soc, serial_number_param[i], tech_prod_id_format_param[i].tp_id, tech_prod_id_format_param[i].agd_id, decoder.ModelId.Value);
                                }

                            }

                            //for VC
                            else if (tech_prod_id_format_param[i].tp_id == 7)
                            {
                                if (serial_number_param[i] == "")
                                    continue;
                                else
                                {


                                    string smartcard_serial_number = serial_number_param[i];
                                    // Find the shipping order lines for decoder and smartcard

                                    var scsoline = soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == s_tp); //

                                    //vc
                                    var smartcard = deviceService.GetDeviceBySerialNumber(smartcard_serial_number);
                                    if (smartcard.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || smartcard.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                                        || smartcard.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                                       )
                                    {

                                    }
                                    else
                                    {
                                        Console.WriteLine(" Smartcard with serial number " + smartcard_serial_number + " is not in allowed capture status!");
                                        //return null;
                                    }

                                    var sc = identifyDevice(soc, serial_number_param[i], tech_prod_id_format_param[i].tp_id, tech_prod_id_format_param[i].agd_id, smartcard.ModelId.Value);
                                }
                            }

                            //for antenna
                            else if (tech_prod_id_format_param[i].tp_id == 30)
                            {
                                antenna_group++;

                                if (serial_number_param[i] == "")
                                    continue;
                                else
                                {
                                    string antenna_sn = serial_number_param[i];

                                    //antenna
                                    var antenna = deviceService.GetDeviceBySerialNumber(antenna_sn);
                                    if (antenna.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || antenna.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                                        || antenna.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                                       )
                                    {
                                        antenna_model = antenna.ModelId.Value;
                                    }
                                    else
                                    {
                                        Console.WriteLine(" Antenna with serial number " + antenna_sn + " is not in allowed capture status!");
                                        //return null;
                                    }
                                    identifyDevice(soc, serial_number_param[i], tech_prod_id_format_param[i].tp_id, tech_prod_id_format_param[i].agd_id, antenna.ModelId.Value);
                                }
                            }

                            //for LNB
                            else if (tech_prod_id_format_param[i].tp_id == 393 || tech_prod_id_format_param[i].tp_id == 4)
                            {
                                lnb_group++;
                                if (serial_number_param[i] == "")
                                    continue;
                                else
                                {
                                    string lnb_sn = serial_number_param[i];

                                    //lnb
                                    var lnb = deviceService.GetDeviceBySerialNumber(lnb_sn);
                                    if (lnb.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || lnb.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                                        || lnb.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                                       )
                                    {
                                        lnb_model = lnb.ModelId.Value;
                                    }
                                    else
                                    {
                                        Console.WriteLine(" LNB with serial number " + lnb_sn + " is not in allowed capture status!");
                                        //return null;
                                    }
                                    identifyDevice(soc, serial_number_param[i], tech_prod_id_format_param[i].tp_id, tech_prod_id_format_param[i].agd_id, lnb.ModelId.Value);
                                }

                            }


                            //for router dan simcard
                            if (tech_prod_id_format_param[i].tp_id == 10 || tech_prod_id_format_param[i].tp_id == 13 || tech_prod_id_format_param[i].tp_id == 12)
                            {

                                string inet_item_serial_number = serial_number_param[i];
                                // Get random decoder and smartcard which are in stock, you should not use this since you have real device information. 
                                Device inet_item = null;
                                Device smartcard = null;

                                //inet_item
                                inet_item = deviceService.GetDeviceBySerialNumber(inet_item_serial_number);
                                if (
                                    inet_item.StatusId.Value == Int32.Parse(DEVICE_STATUS_STOCK) || inet_item.StatusId.Value == Int32.Parse(DEVICE_STATUS_REFURSTOCK)
                                    || inet_item.StatusId.Value == Int32.Parse(DEVICE_STATUS_REPAIREDSTOCK)
                                    )
                                {

                                }
                                else
                                {
                                    Console.WriteLine(" inet_item with serial number " + inet_item_serial_number + " is not in allowed capture status!");
                                    //return null;
                                }

                                // Identify device info to the shipping order lines
                                var dd = identifyDevice(soc, serial_number_param[i], tech_prod_id_format_param[i].tp_id, tech_prod_id_format_param[i].agd_id, inet_item.ModelId.Value);

                            }


                            
                            // Fill the agreement detail id on the shipping order line to link the device to customer
                            //soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == tech_prod_id_format_param[i].tp_id && t.OrderLineNumber == i+1).AgreementDetailId = tech_prod_id_format_param[i].agd_id;

                            //soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == tech_prod_id_format_param[i].tp_id && t.OrderLineNumber == i+1).ReceivedQuantity = 1;
                            


                            
                        }
                        
                    }

                    msg = "Starting perform build list";
                    Console.WriteLine(msg);
                    logger.Debug(msg);



                    // Ship the Shipping Order
                    msg = "Starting link the device to customer";
                    Console.WriteLine(msg);
                    logger.Debug(msg);

                    //code for ship the so, then serial number will show in tab device
                    soService.ShipOrder(soc, shipso_reason, null);
                    //msg = "Shipping Order :" + soc.Id.Value + " has been shipped!";
                    //logger.Info(msg);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    msg = "Errors : " + ex.Message + "  ------  Exception Stack : " + ex.StackTrace;
                    Console.WriteLine("Errors : " + ex.Message);
                    Console.WriteLine("Stack : " + ex.StackTrace);
                    logger.Error(msg);

                    //return null;
                }

            }


        }

        public DevicePerAgreementDetailCollection get_device_per_cust( int cust_id_param)
        {
            //Create the authentication header, initialize with credentials, and
            //create the service proxy. For details, see the API Developer's
            //Guide and the Reference Library CHM.
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);
            var agreementManagementService = AsmRepository.GetServiceProxyCachedOrDefault<IAgreementManagementService>(authHeader);
            int agreementId = 99;
            //In ICC, a Page contains up to 20 records. For ALL
            //records, set Page to 0;
            int page = 0;
            //Call the method and display the results.
            DevicePerAgreementDetailCollection dpads = agreementManagementService.GetDevicesPerAgreementDetailForCustomer(cust_id_param, page);
           
            //DevicePerAgreementDetailCollection dpads = agreementManagementService.GetDevicesPerAgreementDetailByTechnicalProductsAndCustomer(list_tech_prod_id_param.ToArray(), cust_id_param, false, page);

            return dpads;
        }

        public DevicePerAgreementDetailCollection get_device_per_cust_1(int cust_id_param)
        {
            //Create the authentication header, initialize with credentials, and
            //create the service proxy. For details, see the API Developer's
            //Guide and the Reference Library CHM.
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);
            var agreementManagementService = AsmRepository.GetServiceProxyCachedOrDefault<IAgreementManagementService>(authHeader);
            int agreementId = 99;
            //In ICC, a Page contains up to 20 records. For ALL
            //records, set Page to 0;
            int page = 0;
            //Call the method and display the results.
            DevicePerAgreementDetailCollection dpads = agreementManagementService.GetDevicesPerAgreementDetailForCustomer(cust_id_param, page);

            //DevicePerAgreementDetailCollection dpads = agreementManagementService.GetDevicesPerAgreementDetailByTechnicalProductsAndCustomer(list_tech_prod_id_param.ToArray(), cust_id_param, false, page);

            return dpads;
        }


        public void tes_11()
        {
            //Create the authentication header, initialize with credentials, and
            //create the service proxy. For details, see the API Developer's
            //Guide and the Reference Library CHM.
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);
            var agreementManagementService = AsmRepository.GetServiceProxyCachedOrDefault<IAgreementManagementService>(authHeader);
            int agreementId = 99;
            //In ICC, a Page contains up to 20 records. For ALL
            //records, set Page to 0;
            int page = 0;
            //Call the method and display the results.
            DevicePerAgreementDetailCollection dpads = agreementManagementService.GetDevicesPerAgreementDetailForCustomer(500168261, page);
            if (dpads != null && dpads.Items.Count > 0)
            {
                foreach (var d in dpads)
                {
                    Console.WriteLine("Device ID = {0}; Technical Product ID = {1} ", d.DeviceId, d.TechnicalProductId);
                    if (d.SoftwarePerAgreementDetails != null)
                    {
                        foreach (var s in d.SoftwarePerAgreementDetails)
                        {
                            Console.WriteLine("Linked to Software Technical Product ID {0}",
                            s.TechnicalProductId);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("This device has no DpADs.");
            }
            Console.ReadLine();


        }

        // Get Shipping Order info by Customer ID
        public void getShippingOrderbyCustomerID(int customer_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

           var soService = AsmRepository.AllServices.GetOrderManagementService(authHeader);
            var faService = AsmRepository.AllServices.GetFinanceService(authHeader);
            var sbService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
            var agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
            var deviceService = AsmRepository.AllServices.GetDevicesService(authHeader);
            var viewfService = AsmRepository.AllServices.GetViewFacadeService(authHeader);
            var prodcService = AsmRepository.AllServices.GetProductCatalogConfigurationService(authHeader);

            try
            {
                int so_shipped = 3;
                int so_pending = 1;
                int so_mayship = 21;
                int so_reject = 5;
                int so_approved = 2;
                string sn = "";

                var sos = soService.GetShippedOrders(customer_id, 0);

                List<ShippingOrder> shipped_so = sos.Items.FindAll(T => T.StatusId == so_shipped);

                List<ShippingOrder> pending_so = sos.Items.FindAll(T => T.StatusId == so_pending);

                List<ShippingOrder> mayship_so = sos.Items.FindAll(T => T.StatusId == so_mayship);

                // get shipped ship order info.
                foreach (var so in shipped_so)
                {
                    Console.WriteLine("Shipped Shipping Order ID : " + so.Id.Value);
                    foreach (var item in so.ShippingOrderLines.Items)
                    {
                        var tp = prodcService.GetTechnicalProduct(item.TechnicalProductId.Value);
                        var device = viewfService.GetDeviceView(new BaseQueryRequest()
                        {
                            FilterCriteria = Op.Eq("ShipOrderId", so.Id.Value) && Op.Eq("TechnicalProductName", tp.Name),
                            PageCriteria = new PageCriteria()
                            {
                                Page = 0,
                                PageSize = 5
                            }

                        });

                        if (device.TotalCount > 0)
                        {
                            sn = device.Items[0].SerialNumber;
                        }

                        Console.WriteLine("Item Hardware Model ID : " + item.HardwareModelId.Value + " Serial Number : " + sn);
                        sn = "";
                    }
                }


                // get pending ship order
                foreach (var so in pending_so)
                {
                    Console.WriteLine("Please update shipping order : " + so.Id.Value + " to May Ship status");
                }


                // get may ship ship order
                foreach (var so in mayship_so)
                {
                    Console.WriteLine("You can input the device info to the May Ship shippinng order : " + so.Id.Value);

                    foreach (var item in so.ShippingOrderLines.Items)
                    {
                        var tp = prodcService.GetTechnicalProduct(item.TechnicalProductId.Value);

                        Console.WriteLine("Techinical Product : " + tp.Description);

                    }
                }

            }
            catch (Exception ex)
            {
                msg = "Errors : " + ex.Message + "  ------  Exception Stack : " + ex.StackTrace;
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Stack : " + ex.StackTrace);
                logger.Error(msg);
            }
        }

        //  Get Shipping Order by Hardware Product Agreeemntdetail ID
        public int getShippingOrderByHardware(int hw_agreementdetail_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            var soService = AsmRepository.AllServices.GetOrderManagementService(authHeader);
            var faService = AsmRepository.AllServices.GetFinanceService(authHeader);
            var sbService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
            var agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
            var deviceService = AsmRepository.AllServices.GetDevicesService(authHeader);
            var viewfService = AsmRepository.AllServices.GetViewFacadeService(authHeader);

            int so = 0;
            try
            {
                var sol = soService.FindShippingOrderLines(new BaseQueryRequest()
                {
                    FilterCriteria = Op.Eq("AgreementDetailId", hw_agreementdetail_id),
                    PageCriteria = new PageCriteria() { Page = 0, PageSize = 100 }
                });

                if (sol.Items.Count > 0)
                {
                    so = sol.Items[0].ShippingOrderId.Value;
                }
                else
                {
                    return 0;
                }

                return so;
            }
            catch (Exception ex)
            {
                msg = "Errors : " + ex.Message + "  ------  Exception Stack : " + ex.StackTrace;
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Stack : " + ex.StackTrace);
                logger.Error(msg);
                return 0;
            }
        }
        
        // Identify device to shipping order
        public ShippingOrder identifyDevice(ShippingOrder soc, string serial_number, int tp_id, int agd_id, int model_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

           var soService = AsmRepository.AllServices.GetOrderManagementService(authHeader);
            var faService = AsmRepository.AllServices.GetFinanceService(authHeader);
            var sbService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
            var agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
            var deviceService = AsmRepository.AllServices.GetDevicesService(authHeader);

            var viewfService = AsmRepository.AllServices.GetViewFacadeService(authHeader);
            try
            {
                var so_lineitem = soc.ShippingOrderLines.Items.Find(t => t.TechnicalProductId == tp_id && t.AgreementDetailId == agd_id);

                if (so_lineitem == null)
                {
                    Console.WriteLine("Can't find techinical product with id : " + tp_id + " in shipping order with id : " + soc.Id.Value);
                    return null;
                }

                var buildlist = new BuildList
                {
                    OrderLineId = so_lineitem.Id,
                    OrderId = soc.Id,
                    ModelId = model_id,
                    StockHandlerId = soc.ShipFromStockHandlerId,
                    TransactionType = BuildListTransactionType.ShippingSerializedProducts
                };

                var b_build_list = deviceService.CreateBuildList(buildlist);

                var d_a_list = deviceService.AddDeviceToBuildListManually(b_build_list.Id.Value, serial_number);

                if (d_a_list.Accepted.Value)
                {
                    var dblist = deviceService.PerformBuildListAction(b_build_list.Id.Value);
                }
                else
                {
                    Console.WriteLine("Failed to attach decoder with SN : " + serial_number + " to Shipping Order : Error : " + d_a_list.Error);
                    return null;
                }

                return soc;
            }
            catch (Exception ex)
            {
                msg = "Errors : " + ex.Message + "  ------  Exception Stack : " + ex.StackTrace;
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Stack : " + ex.StackTrace);
                logger.Error(msg);
                return null;
            }

        }

        public List<Format_tp_agdid> getformat_bycallshippingorderbyid_1(int cust_id)
        {
            var format = new List<Format_tp_agdid>();


            string the_json = get_request_to_api("http://192.168.177.186:1111/api/hdkartika/ICCMsky2016/device/getcustomerdeviceview/" + cust_id);
            dynamic parsedJson = JsonConvert.DeserializeObject(the_json);

            //var devices_per_cust = get_device_per_cust_1(cust_id).OrderBy(t => t.AgreementDetailId).ThenBy(t => t.TechnicalProductId);

            //Console.WriteLine(parsedJson.ShippingOrderLines.Count);

            for (int i = 0; i < parsedJson.Count; i++)
            {
                Format_tp_agdid dc = new Format_tp_agdid();
                dc.agd_id = Convert.ToInt32(parsedJson[i].AgreementDetailId);
                dc.tp_id = Convert.ToInt32(parsedJson[i].TechnicalProductId);

                format.Add(dc);

                //format.Add(Convert.ToInt32(parsedJson.ShippingOrderLines[i].TechnicalProductId));
            }

            return format;


        }

        public List<int> getformat_bycallshippingorderbyid(int so_id)
        {
            var format = new List<int>();


            string the_json = get_request_to_api("http://192.168.177.186:1111/api/hdkartika/ICCMsky2016/shippingorder/GetShippingOrderById/" + so_id);
            dynamic parsedJson = JsonConvert.DeserializeObject(the_json);

            //Console.WriteLine(parsedJson.ShippingOrderLines.Count);

            for(int i = 0; i < parsedJson.ShippingOrderLines.Count; i++)
            {
                //Format_tp_agdid dc = new Format_tp_agdid();
                //dc.agd_id = Convert.ToInt32(parsedJson.ShippingOrderLines[i].AgreementDetailId);
                //dc.tp_id = Convert.ToInt32(parsedJson.ShippingOrderLines[i].TechnicalProductId);

                format.Add(Convert.ToInt32(parsedJson.ShippingOrderLines[i].TechnicalProductId));
            }

            return format;

            
        }

        public List<Format_tp_agdid> getformat_bycallshippingorderbyid_2(int so_id)
        {
            var format = new List<Format_tp_agdid>();


            string the_json = get_request_to_api("http://192.168.177.186:1111/api/hdkartika/ICCMsky2016/shippingorder/GetShippingOrderById/" + so_id);
            dynamic parsedJson = JsonConvert.DeserializeObject(the_json);

            //Console.WriteLine(parsedJson.ShippingOrderLines.Count);

            for (int i = 0; i < parsedJson.ShippingOrderLines.Count; i++)
            {
                Format_tp_agdid dc = new Format_tp_agdid();
                dc.agd_id = Convert.ToInt32(parsedJson.ShippingOrderLines[i].AgreementDetailId);
                dc.tp_id = Convert.ToInt32(parsedJson.ShippingOrderLines[i].TechnicalProductId);

                format.Add(dc);
            }

            return format;


        }

        //shippingorder handler
        //Change Pending Shipping Order to May Ship
        public void MayShipSO_ver1(int custid)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            var soService = AsmRepository.AllServices.GetOrderManagementService(authHeader);
            var faService = AsmRepository.AllServices.GetFinanceService(authHeader);
            var sbService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
            var agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
            var deviceService = AsmRepository.AllServices.GetDevicesService(authHeader);
            var viewfService = AsmRepository.AllServices.GetViewFacadeService(authHeader);

            string msg = "Update SO status to May Ship...";
            Console.WriteLine(msg);
            //Logger.Info(msg);
            try
            {
                // get the pending shipping order and shipping order line
                var soc = soService.GetShippedOrders(custid, 0).Items.Find(t => t.StatusId == SOPENDING);

                if (soc == null)
                {
                    Console.WriteLine("No pending shipping order please check!");
                    return;
                }

                soService.UpdateShippingOrderStatus(soc.Id.Value, mayship_reason);
            }
            catch (Exception ex)
            {

                msg = "Errors : " + ex.Message + "  ------  Exception Stack : " + ex.StackTrace;
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Stack : " + ex.StackTrace);
                logger.Error(msg);

            }


        }

        // Update device status(event 156)
        public void updateDeviceStatus(string serial_number, int reason_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            var whService = AsmRepository.AllServices.GetLogisticsService(authHeader);
            var prodcService = AsmRepository.AllServices.GetProductCatalogConfigurationService(authHeader);
            var deviceService = AsmRepository.AllServices.GetDevicesService(authHeader);

            try
            {
                var device = deviceService.GetDeviceBySerialNumber(serial_number);
                if (device != null)
                {
                    deviceService.UpdateDeviceStatus(device.Id.Value, reason_id);
                    Console.WriteLine("device id = " + device.Id, "serial number = " + device.SerialNumber);
                }
                else
                {
                    Console.WriteLine("Can't find device with serial number : " + serial_number);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Stack : " + ex.StackTrace);
            }
        }

        public void transferDevice(int reason183, int to_stockhandler_id, string sn)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            var whService = AsmRepository.AllServices.GetLogisticsService(authHeader);
            var prodcService = AsmRepository.AllServices.GetProductCatalogConfigurationService(authHeader);
            var deviceService = AsmRepository.AllServices.GetDevicesService(authHeader);

            try
            {
                BuildList bl = new BuildList();
                bl.TransactionType = BuildListTransactionType.TransferToAnotherStockHandler;
                bl.Reason = reason183;
                bl.StockHandlerId = to_stockhandler_id;


                // Create build list
                var nbl = deviceService.CreateBuildList(bl);

                // Add device to build list
                deviceService.AddDeviceToBuildListManually(nbl.Id.Value, sn);

                // Perform build list
                var bl1 = deviceService.PerformBuildListAction(nbl.Id.Value);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Stack : " + ex.StackTrace);
            }
        }

        // import devices to stock handler

        public void importDevices()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            var whService = AsmRepository.AllServices.GetLogisticsService(authHeader);
            var prodcService = AsmRepository.AllServices.GetProductCatalogConfigurationService(authHeader);
            var deviceService = AsmRepository.AllServices.GetDevicesService(authHeader);

            try
            {

                var stockhandlers = whService.GetStockHandlers(new BaseQueryRequest()
                {
                    FilterCriteria = Op.Eq("Active", true),
                    PageCriteria = new PageCriteria()
                    {
                        Page = 1,
                        PageSize = 100
                    }
                });

                var models = prodcService.GetHardwareModels(new BaseQueryRequest()
                {
                    FilterCriteria = Op.Like("Description", "%Decoder%"),
                    PageCriteria = new PageCriteria()
                    {
                        Page = 1,
                        PageSize = 100
                    }
                });

                //models.Items.Sort();

                foreach (var model in models.Items)
                {
                    string start_sn = "ID" + model.Id.Value + "YY";
                    int count = 100;

                    StockReceiveDetails stockReceiveDetails = new StockReceiveDetails();
                    stockReceiveDetails.FromStockHanderId = 1;
                    stockReceiveDetails.ToStockHanderId = 1;
                    stockReceiveDetails.Reason = 71197;
                    stockReceiveDetails = deviceService.CreateStockReceiveDetails(stockReceiveDetails);


                    BuildList bl = new BuildList();
                    bl.TransactionType = BuildListTransactionType.ReceiveNewStock;
                    bl.Reason = 71197;
                    bl.ModelId = model.Id.Value;
                    bl.StockReceiveDetailsId = stockReceiveDetails.Id.Value;

                    // Create build list
                    var nbl = deviceService.CreateBuildList(bl);

                    // Add device to build list
                    for (int i = 0; i < 300; i++)
                    {
                        string sn = start_sn + count.ToString().PadLeft(model.HardwareNumberLength.Value - start_sn.Length, '0');
                        Console.WriteLine(sn);
                        count++;
                        deviceService.AddDeviceToBuildListManually(nbl.Id.Value, sn);
                    }

                    // Perform build list
                    var bl1 = deviceService.PerformBuildListAction(nbl.Id.Value);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Stack : " + ex.StackTrace);
            }
        }

        // Send Special Command
        public void sendCommand(string smartcard_serial_number)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            var soService = AsmRepository.AllServices.GetOrderManagementService(authHeader);
            var faService = AsmRepository.AllServices.GetFinanceService(authHeader);
            var sbService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
            var agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
            var deviceService = AsmRepository.AllServices.GetDevicesService(authHeader);
            var viewfService = AsmRepository.AllServices.GetViewFacadeService(authHeader);

            int pre_act_7 = 99177; // pre-activation for 7 days.
            var device = deviceService.GetDeviceBySerialNumber(smartcard_serial_number);
            if (device != null)
            {
                deviceService.SendCommandToDevice(device.Id.Value, pre_act_7);
            }
            else
            {
                Console.WriteLine("Can't find device with SN :" + smartcard_serial_number);
            }

        }

        //  Create Relationship for customer

        public void createRelationshipForCustomer(int customer_id, int member_cutomer_id, string relationship_type_key)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
            
            var custService = AsmRepository.AllServices.GetCustomersService(authHeader);
            
            //asasasas
            custService.CreateRelation(new Relation()
            {
                CustomerIdFrom = customer_id,
                CustomerIdTo = member_cutomer_id,
                RelationTypeKey = relationship_type_key
            }, 0);
        }

        // update relationship for customer
        public void updateRelatioshipForCustomer(int customer_id, int member_customer_id, string relationship_type_key)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            ICustomersService custService = null;

            custService = AsmRepository.AllServices.GetCustomersService(authHeader);

            var relationships = custService.GetRelations(customer_id, new CriteriaCollection(){
                new Criteria()
                {
                    Key = "RelationTypeKey",
                    Operator = Operator.Equal,
                    Value = relationship_type_key
                }
            }, 0);

            if (relationships.Items.Count > 0)
            {
                var relationship = relationships.Items[0];
                relationship.CustomerIdFrom = customer_id;
                relationship.CustomerIdTo = member_customer_id;
                relationship.RelationTypeKey = relationship_type_key;
                custService.UpdateRelation(relationship, 0);
            }
            else
            {
                Console.WriteLine("Can't find relationship type : " + relationship_type_key + " on customer : " + customer_id);
            }

        }

        public void deleteRelationshipForCustomer(int customer_id, string relationship_type_key)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://192.168.177.4/asm/all/servicelocation.svc");

            ICustomersService custService = null;

            custService = AsmRepository.AllServices.GetCustomersService(authHeader);

            int[] r_ids = new int[1];
            var relationships = custService.GetRelations(customer_id, new CriteriaCollection(){
                new Criteria()
                {
                    Key = "RelationTypeKey",
                    Operator = Operator.Equal,
                    Value = relationship_type_key
                }
            }, 0);
            if (relationships.Items.Count > 0)
            {
                r_ids[0] = relationships.Items[0].Id.Value;
                custService.DeleteRelations(r_ids, 0);
            }

        }

        // import device by file
        public void importDevicesbyFile(string filepath, int stockhandlerid, int device_model_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();

            AsmRepository.SetServiceLocationUrl("http://192.168.177.4/asm/all/servicelocation.svc");

            
            var soService = AsmRepository.AllServices.GetOrderManagementService(authHeader);
            var faService = AsmRepository.AllServices.GetFinanceService(authHeader);
            var sbService = AsmRepository.AllServices.GetSandBoxManagerService(authHeader);
            var agService = AsmRepository.AllServices.GetAgreementManagementService(authHeader);
            var deviceService = AsmRepository.AllServices.GetDevicesService(authHeader);
            var viewfService = AsmRepository.AllServices.GetViewFacadeService(authHeader);

            try
            {
                var file = File.ReadAllLines(@filepath);
                string[] sns = new string[file.Length];
                int i = 0;
                DateTime dt = DateTime.Now;
                StreamWriter badfile = new StreamWriter("import_device_badfile_" + dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second + ".txt");

                // Create build list
                StockReceiveDetails stockReceiveDetails = new StockReceiveDetails();
                stockReceiveDetails.FromStockHanderId = stockhandlerid;
                stockReceiveDetails.ToStockHanderId = stockhandlerid;
                stockReceiveDetails.Reason = 71197;
                stockReceiveDetails = deviceService.CreateStockReceiveDetails(stockReceiveDetails);

                BuildList bl = new BuildList();
                bl.TransactionType = BuildListTransactionType.ReceiveNewStock;
                bl.Reason = 71197;
                bl.ModelId = device_model_id;
                bl.StockReceiveDetailsId = stockReceiveDetails.Id.Value;

                var nbl = deviceService.CreateBuildList(bl);

                foreach (var line in file)
                {
                    try
                    {

                        Console.WriteLine("import device with serial number ： " + line);

                        // Add device to build list
                        var add_result = deviceService.AddDeviceToBuildListManually(nbl.Id.Value, line);

                        // If add device failed
                        if (add_result.Error.Length > 0)
                        {
                            badfile.WriteLine(line);
                            Console.WriteLine(line + " import error : " + add_result.Error);
                        }

                    }
                    catch (Exception ex)
                    {
                        badfile.WriteLine(line);
                        Console.WriteLine(line + " -- Errors : " + ex.Message);
                    }

                }

                var bl1 = deviceService.PerformBuildListAction(nbl.Id.Value);

                Console.WriteLine("Import " + bl1.TotalAccepted + " successfully, and " + bl1.TotalFailed + " failed");

                badfile.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Errors found when import devices by file" + filepath);
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Stack : " + ex.StackTrace);
            }


        }
        
        public void get_value_from_request()
        {
            string the_json = get_request_to_api("http://192.168.177.185:1111/api/iccapi/ap1msky!/customer/GetCustDetailByCustId/304125649");
            dynamic parsedJson = JsonConvert.DeserializeObject(the_json);
            Console.WriteLine(parsedJson.DefaultAddress.FirstName);
            Console.WriteLine(parsedJson.DefaultAddress.MiddleName);
            Console.WriteLine(parsedJson.DefaultAddress.Surname);
        }
        public string get_request_to_api(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                    
                }
                throw;
            }
        }

        public void get_wo_id()
        {
            int[] arr1 = new int[] {

            500003005,
            500003640,
            500003506,
            500003168,
            500003300,
            500003266,
            500003041,
            500005580,
            500004619,
            500004261,
            500004708,
            500003990,
            500005848,
            500004280,
            500004655,
            500005230,
            500005650,
            500005632,
            500004637,
            500003702,
            500004360


            };

           

            for (int i =0; i < arr1.Length; i++)
            {
                string the_url = "http://192.168.177.185:1111/api/iccapi/ap1msky!/workorder/GetWorkOrderByCustomerId/" + arr1[i].ToString();
                string api_cust_detail = get_request_to_api(the_url);
                dynamic JsonResult = JsonConvert.DeserializeObject(api_cust_detail);

                if (JsonResult.Count == 0)
                    continue;
                else
                    Console.WriteLine(arr1[i].ToString() + "--" +JsonResult[0].Id);
                


            }

            




        }

        public void simple_do_payment_by_cust_id()
        {
            int[] arr1 = new int[] {

            301851452,
            400051984
            };

            int[] arr2 = new int[] {

            179900,
            74900
            };

            string[] arr3 = new string[] {

            "mobile banking",
            "mobile banking"
            };



            for (int i = 0; i < arr1.Length; i++)
            {
                string the_url = "http://192.168.177.185:1111/api/iccapi/ap1msky!/payment/doPaymentWithFulfillQuote/" + arr1[i].ToString() + "/" + arr2[i].ToString() + "/" + arr3[i].ToString();
                string api_cust_detail = get_request_to_api(the_url);
                dynamic JsonResult = JsonConvert.DeserializeObject(api_cust_detail);

                if (JsonResult.Count == 0)
                    continue;
                else
                    Console.WriteLine(arr1[i].ToString() + "--" + JsonResult[0].Id);



            }






        }

        public void read_file_on_server()
        {
            
            WebRequest request = WebRequest.Create(new Uri("http://192.168.177.207/posting_manual/log_folder/tes.txt"));
            request.Method = "HEAD";

            using (WebResponse response = request.GetResponse())
            {
                Console.WriteLine(response.Headers);
                Console.WriteLine("{0} {1}", response.ContentLength, response.ContentType);
            }
        }

        public void posting_manual_read_from_excel_file()
        {
            StringBuilder sb = new StringBuilder();

            string path = @"c:\manual_posting_log\";
            string file_name = "POSTING_MANUAL_" + DateTime.Now.ToString("dd_MM_yyyy").ToString() + ".txt";

            string full_path_file = path + file_name;

            string file = @"C:\\manual_posting_excel_file\Belum posting as of 28 Agust2017.xlsx";
            Console.WriteLine(file);

            Microsoft.Office.Interop.Excel.Application xl = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbook = xl.Workbooks.Open(@"C:\\manual_posting_excel_file\Belum posting as of 28 Agust2017.xlsx");
            Microsoft.Office.Interop.Excel.Worksheet sheet = workbook.Sheets[1];


            int numRows = sheet.UsedRange.Rows.Count;
            int numColumns = 6;     // according to your sample

            List<string[]> contents = new List<string[]>();
            string[] record = new string[6]; // according to your sample

            for (int rowIndex = 2; rowIndex <= numRows; rowIndex++)  // assuming the data starts at row 2
            {
                for (int colIndex = 1; colIndex <= numColumns; colIndex++)
                {
                    Range cell = (Range)sheet.Cells[rowIndex, colIndex];
                    if (cell.Value != null)
                    {
                        record[colIndex - 1] = Convert.ToString(cell.Value);
                    }
                }

                try
                {
                    Console.WriteLine(rowIndex);
                    string the_url = "http://192.168.177.185:1111/api/iccapi/ap1msky!/payment/doPaymentWithFulfillQuote/" + record[1].ToString() + "/" + record[2].ToString() + "/" + record[4].ToString();
                    Console.WriteLine(the_url);

                    string api_cust_detail = get_request_to_api(the_url);
                    dynamic JsonResult = JsonConvert.DeserializeObject(api_cust_detail);

                    if (JsonResult.Count == 0)
                    {
                        sb.Append("posting_manual#" + record[0].ToString() + '#' + record[1].ToString() + "#" + record[3].ToString() + "#" + "error JsonResult.Count" + Environment.NewLine);
                        #region checking file log
                        if (!Directory.Exists(path))  // if it doesn't exist, create
                            Directory.CreateDirectory(path);


                        if (!File.Exists(full_path_file))
                        {
                            File.Create(full_path_file).Dispose();
                            using (TextWriter tw = new StreamWriter(full_path_file))
                            {
                                tw.WriteLine("The very first line!" + Environment.NewLine);
                                tw.Close();
                            }
                        }

                        #endregion checking file log

                        File.AppendAllText(full_path_file, sb.ToString());
                        sb.Clear();
                        continue;
                    }

                    else
                    {
                        sb.Append("posting_manual#" + record[0].ToString() + '#' + record[1].ToString() + "#" + record[3].ToString() + "#" + "success" + Environment.NewLine);

                        #region checking file log
                        if (!Directory.Exists(path))  // if it doesn't exist, create
                            Directory.CreateDirectory(path);


                        if (!File.Exists(full_path_file))
                        {
                            File.Create(full_path_file).Dispose();
                            using (TextWriter tw = new StreamWriter(full_path_file))
                            {
                                tw.WriteLine("The very first line!" + Environment.NewLine);
                                tw.Close();
                            }
                        }

                        #endregion checking file log

                        File.AppendAllText(full_path_file, sb.ToString());
                        sb.Clear();

                        Console.WriteLine(record[0].ToString() + "--" + JsonResult[0].Id);
                    }
                }
                catch(Exception exc)
                {
                    sb.Append("posting_manual#" + record[0].ToString() + '#' + record[1].ToString() + "#" + record[3].ToString() + "#" + "error catch (Exception exc)" + Environment.NewLine);

                    #region checking file log
                    if (!Directory.Exists(path))  // if it doesn't exist, create
                        Directory.CreateDirectory(path);


                    if (!File.Exists(full_path_file))
                    {
                        File.Create(full_path_file).Dispose();
                        using (TextWriter tw = new StreamWriter(full_path_file))
                        {
                            tw.WriteLine("The very first line!" + Environment.NewLine);
                            tw.Close();
                        }
                    }

                    #endregion checking file log

                    #region write_to_file
                    File.AppendAllText(full_path_file, sb.ToString());
                    sb.Clear();
                    #endregion write_to_file
                }




                //Console.WriteLine(record[0] + record[1] + record[2] + record[3]);

                contents.Add(record);
            }

            xl.Quit();
            Marshal.ReleaseComObject(xl);
            
            Console.WriteLine(full_path_file);
            
        }
        
        public void read_log_of_posting_manual()
        {
            string path = @"c:\Log_folder_payment_icc_job\";
            string file_name = "POSTING_MANUAL_" + DateTime.Now.ToString("dd_MM_yyyy").ToString() + ".txt";

            string full_path_file = path + file_name;

            using (StreamReader sr = new StreamReader(full_path_file, Encoding.Default))
            {
                string line = sr.ReadLine();
                string text = sr.ReadToEnd();

                string[] lines = text.Split(new[] { "\n" }, StringSplitOptions.None);
                foreach (string s in lines)
                {
                    Console.Write(Regex.Replace(s, @"\t|\n|\r", Environment.NewLine));
                }

                //string[] lines_1 = text.Split(new[] { "#success" }, StringSplitOptions.None);
                //foreach (string s_1 in lines_1)
                //{
                //    Console.WriteLine(s_1 + "#success" + Environment.NewLine);
                //}
            }


            
        }

        public void find_geodefinition()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://192.168.177.4/asm/all/servicelocation.svc");

            var customersConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersConfigurationService>(ah);
            var customersService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersService>(ah);
            
            //Instantiate a request object
            GeoDefinitionCriteria criteria = new GeoDefinitionCriteria();
            //If you know the customer's address ID, you can replace "481" with the addressId
            //and use the returned address to populate the address criteria.
            Address address = customersService.GetAddress(10310193);
            //If your GeoDefinitions are configured on address details, and if you do not
            //know the customer's addressId, set FindByFullAddressDetails to True and
            //enter the address details.
            //criteria.FindByFullAddressDetails = true;
            //criteria.CountryId = 1;
            //criteria.Province = 1;
            //criteria.BigCity = "LOS ANGELES";
            //criteria.SmallCity = null;
            //criteria.PostalCode = "95BH1";
            //criteria.Street = "VINE ST";
            
            //If your GeoDefinitions are configured on postal code, and if you do not
            //know the customer's addressId, set FindByPostalCode to True and
            //enter the postal code.
            //criteria.FindByPostalCode = true;
            //criteria.PostalCode = "11520001";
            //Get the GeoDefinition for the address. There should be only one.
            //If there are more than one, then there is an error in the GeoDefinition
            //configuration.
            GeoDefinitionCollection collection = customersConfigurationService.FindGeoDefinitions(criteria);
            //Display the results.
            if (collection != null && collection.Items.Count > 0)
            { 
                foreach (var item in collection)
                {
                    Console.WriteLine("GeoDefId {0} <==> geolocation {1}", item.Id, item.GeoLocations);
                }
             }
            else
            {
                Console.WriteLine("I found nothing.");
            }
                Console.ReadLine();


        }

        public void getserviceproviderservice_byserviceprovideridandgeoid()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://192.168.177.4/asm/all/servicelocation.svc");

            var customersConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersConfigurationService>(ah);
            var customersService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersService>(ah);
            var aa = AsmRepository.AllServices.GetWorkforceService(ah);
            
            int[] arr1 = new int[] { 153};

            var bb = aa.GetServiceProviderServicesByServiceProviderIdServiceTypeIdAndGeoDefIds(32958, 1, arr1, 1);

            foreach (var item in bb.Items)
            {
                Console.WriteLine("serviceproviderserviceid {0} <==> ServiceProviderId {1} <==> servicetytpeid = {2}", item.Id, item.ServiceProviderId, item.ServiceTypeId);
            }

            Console.ReadLine();


        }

        public void tes()
        {
            DateTime myDate = DateTime.ParseExact("2017-09-03 09:00:00 AM", "yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);

            int dy = myDate.Day;
            int mn = myDate.Month;
            int yy = myDate.Year;

            int hh = myDate.Hour;
            int mm = myDate.Minute;
            int ss = myDate.Second;

            Console.WriteLine("{0} {1} {2} {3} {4} {5}", dy, mn, yy, hh, mm, ss);

        }

        public void get_area_data()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            var aa = AsmRepository.AllServices.GetCustomersConfigurationService(ah).GetProvinceByCountry(1);
            foreach (var aa_item in aa)
            {
                Console.WriteLine("province id = {0} -- province name = {1}", aa_item.Id, aa_item.Name);

                var valid_address_Service = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersConfigurationService>(ah);

                if(aa_item.Id.Value == 23)
                {
                    #region Find customer by phone number
                    ////Instantiate a BaseQueryRequest for your filter criteria.
                    BaseQueryRequest request = new BaseQueryRequest();


                    request.FilterCriteria = Op.Like("CountryId", "1");
                    //request.FilterCriteria = Op.Like("ProvinceId", aa_item.Id);
                    //request.FilterCriteria = Op.Like("PostalCode", "24410000");


                    var val_address = valid_address_Service.GetGeoLocations(request);

                    if (val_address != null)
                    {
                        foreach (var c in val_address)
                        {
                            Console.WriteLine("Found GeoLocation ID {0}, countryId = {1} - provinceId = {2} -  bigcity = {3} -  smallcity = {4} - street = {5}", c.Id, c.CountryId, c.Province.Value, c.BigCity, c.SmallCity, c.StreetName);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cannot find a customerr.");
                    }
                    Console.WriteLine("total customer = {0} ----", val_address.Count());


                    #endregion
                }




            }
        }

        public void getValidAddress()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            var customersService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersService>(authHeader);
            var valid_address_Service = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersConfigurationService>(authHeader);

            #region Find customer by phone number
            //Instantiate a BaseQueryRequest for your filter criteria.
            BaseQueryRequest request = new BaseQueryRequest();


            request.FilterCriteria = Op.Like("CountryId", "1");
            //request.FilterCriteria = Op.Like("ProvinceId", "1");
            //request.FilterCriteria = Op.Like("PostalCode", "24410000");


            ValidAddressCollection val_address = valid_address_Service.GetValidAddresses(request);

            if (val_address != null)
            {
                foreach (ValidAddress c in val_address)
                {
                    sb.Append("'" + c.Id + "','" + c.Street + "','" + c.SmallCity + "','" + c.BigCity + "','" + c.Active + "','" + c.ProvinceId + "','" + c.PostalCode + "','" + c.CountryId + "'" + Environment.NewLine);
                    checking_file_log();
                    write_info_to_log_file();
                    Console.WriteLine("Found valid address ID {0}, countryId = {1} - provinceId = {2} -  bigcity = {3} -  smallcity = {4} - postalcode = {5} - street = {6}", c.Id, c.CountryId, c.ProvinceId, c.BigCity, c.SmallCity, c.PostalCode, c.Street);
                }
            }
            else
            {
                Console.WriteLine("Cannot find a customer with that phone number.");
            }
            Console.WriteLine("total data = {0} ----", val_address.Count());
            //You will need to enter some error handling in case there is more than
            //one matching customer.
            Console.ReadLine();
            #endregion
        }

        public void addcommercialproductsinglequote()
        {
            List<int> the_products = new List<int>(new int[] { 1 }); //1,2, 199, 200
            List<int> offers = new List<int>(new int[] { 2169, 2170, 2168 });
            int customer_id = 500052122;
            int agreement_id = 0;
            int fa_id = 4282846;

            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            var customersService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersService>(ah);
            var customersConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersConfigurationService>(ah);
            var financeService = AsmRepository.GetServiceProxyCachedOrDefault<IFinanceService>(ah);
            var financeConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<IFinanceConfigurationService>(ah);
            var invoiceRunService = AsmRepository.GetServiceProxyCachedOrDefault<IInvoiceRunService>(ah);
            var agreementManagementService = AsmRepository.GetServiceProxyCachedOrDefault<IAgreementManagementService>(ah);
            var pricingService = AsmRepository.GetServiceProxyCachedOrDefault<IPricingService>(ah);
            var productCatalogService = AsmRepository.GetServiceProxyCachedOrDefault<IProductCatalogService>(ah);
            var sandboxService = AsmRepository.GetServiceProxyCachedOrDefault<ISandBoxManagerService>(ah);

            var agService = AsmRepository.AllServices.GetAgreementManagementService(ah);
            var agcService = AsmRepository.AllServices.GetAgreementManagementConfigurationService(ah);
            var productService = AsmRepository.AllServices.GetProductCatalogService(ah);
            var productcService = AsmRepository.AllServices.GetProductCatalogConfigurationService(ah);
            var custcService = AsmRepository.AllServices.GetCustomersConfigurationService(ah);
            var fincService = AsmRepository.AllServices.GetFinanceConfigurationService(ah);
            var custService = AsmRepository.AllServices.GetCustomersService(ah);
            
            try
            {
                int sandbox_id = sandboxService.CreateSandbox();
                
                int business_unit_id = custService.GetCustomer(customer_id).BusinessUnitId.Value;

                int reason120 = 65;
                
                var agreements = agService.GetAgreementsForCustomerId(customer_id, 1);

                if (agreements.Items.Count > 0)
                {
                    agreement_id = agreements.Items[1].Id.Value;

                    AgreementDetailWithOfferDefinitionsCollection productList = new AgreementDetailWithOfferDefinitionsCollection();
                    for (int i = 0; i < the_products.Capacity; i++) //total product
                    {
                        // 1, 2, 199, 200
                        if (the_products[i] == 1 || the_products[i] == 2 || the_products[i] == 199 || the_products[i] == 200) // for product (hardware) with id 1
                        {
                            productList.Add(new AgreementDetailWithOfferDefinitions()
                            {
                                AgreementDetail = new AgreementDetail()
                                {
                                    CustomerId = customer_id,
                                    AgreementId = agreement_id,
                                    ChargePeriod = PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.ChargePeriods.Monthly,
                                    CommercialProductId = the_products[i],    // comm_prod_id
                                    DeviceIncluded = true,
                                    DevicesOnHand = false,
                                    Quantity = 1,
                                    FinanceOptionId = 3,   // FinanceOptionId : 2 is Subscription for software product, if you need add hardware product, you need use 1 as sold, 3 as rent
                                    BusinessUnitId = business_unit_id,
                                    //DisconnectionSetting = PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.DisconnectionDateSettings.QuoteBased
                                    DisconnectionSetting = PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.DisconnectionDateSettings.AccountBased
                                },
                            });
                        }
                        else
                        {
                            productList.Add(new AgreementDetailWithOfferDefinitions()
                            {

                                AgreementDetail = new AgreementDetail()
                                {
                                    CustomerId = customer_id,
                                    AgreementId = agreement_id,
                                    ChargePeriod = PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.ChargePeriods.Monthly,
                                    CommercialProductId = the_products[i],   // comm_prod_id
                                    DeviceIncluded = true,
                                    DevicesOnHand = false,
                                    Quantity = 1,
                                    FinanceOptionId = 2,   // FinanceOptionId : 2 is Subscription for software product, if you need add hardware product, you need use 1 as sold, 3 as rent
                                    BusinessUnitId = business_unit_id,
                                    DisconnectionSetting = PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.DisconnectionDateSettings.QuoteBased
                                },
                            });
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
                        OfferDefinitions = offers,
                        SkipWorkOrderGeneration = true,
                        SkipShippingOrderGeneration = false,
                        AgreementDetailWithOfferDefinitions = productList
                        

                    };

                    var result = agService.ManageProductCapture(param);

                    if (result.AgreementDetails.Items.Count == 0)
                    {
                        Console.WriteLine("Warnings : " + result.WarningMessage);

                        Console.WriteLine("Something bad happened. Sandbox rolled back.");

                        //The "false" value in this line of code rolls back the sandbox.
                        sandboxService.FinalizeSandbox(sandbox_id, false);
                        //return 0;

                    }
                    else
                    {
                        //The "true" value in this line of code commits the sandbox.
                        sandboxService.FinalizeSandbox(sandbox_id, true);

                        foreach (var item in result.AgreementDetails)
                        {
                            Console.WriteLine("Congratulations! New Product ID = {0}.", item.Id);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Can't find agreement for customer id : " + customer_id);
                    //return 0;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Exception Stack : " + ex.StackTrace);
                //return 0;
            }
        }

        static void write_info_to_log_file()
        {
            // flush every 20 seconds as you do it
            File.AppendAllText(full_path_file, sb.ToString());
            sb.Clear();


            //File.WriteAllText(full_path_file, createText);
        }
        static void checking_file_log()
        {
            #region checking file log
            if (!Directory.Exists(path))  // if it doesn't exist, create
                Directory.CreateDirectory(path);

            file_log_name = DateTime.Now.ToString("dd_M_yyyy").ToString();

            full_path_file = path + "data_product_" + file_log_name + ".txt";

            if (!File.Exists(full_path_file))
            {
                File.Create(full_path_file).Dispose();
                using (TextWriter tw = new StreamWriter(full_path_file))
                {
                    tw.WriteLine("The very first line!" + Environment.NewLine);
                    tw.Close();
                }
            }
            //else if (File.Exists(full_path_file))
            //{
            //    using (TextWriter tw = new StreamWriter(full_path_file))
            //    {
            //        tw.WriteLine("The next line!");
            //        tw.Close();
            //    }
            //}
            #endregion checking file log
        }


    }
}
