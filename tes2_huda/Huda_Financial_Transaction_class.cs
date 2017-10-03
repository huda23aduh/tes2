using PayMedia.ApplicationServices.ClientProxy;
using PayMedia.ApplicationServices.SharedContracts;
using PayMedia.ApplicationServices.ViewFacade.ServiceContracts;
using PayMedia.ApplicationServices.ViewFacade.ServiceContracts.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tes2_huda
{
    class Huda_Financial_Transaction_class
    {
        public void getFinancialTransactionView(int FA_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
            var viewFacade = AsmRepository.GetServiceProxyCachedOrDefault<IViewFacadeService>(authHeader);


            #region BaseQueryRequest FilterCriteria
            //As of MR24, these are the properties you can use in the
            //BaseQueryRequest filter. Use Visual Studio's Intellisense to get the
            //list of properties that are valid in your version of ICC.
            //FinancialTransactionView f = new FinancialTransactionView
            //f.AmtExclTax;
            //f.AmtInclTax;
            //f.AppearedOnInvoiceNumber;
            //f.ApprdOnInvoiceId;
            //f.BankDatetime;
            //f.BaseAmount;
            //f.BusinessUnitDescription;
            //f.BusinessUnitId;
            //f.CreateDatetime;
            //f.CreatedByEvent;
            //f.CreatedByUserId;
            //f.CreatedByUserName;
            //f.CurrencyId;
            //f.CurrencyMnemonic;
            //f.CustomerId;
            //f.DebitOrCredit;
            //f.EntityId;
            //f.EntityType;
            //f.ExternalAgentId;
            //f.ExternalAgentName;
            //f.ExternalTaxAreaId;
            //f.ExternalTransactionId;
            //f.ExtraPaymentInfo;
            //f.FinanceBatchId;
            //f.FinancialAccountId;
            //f.HistoryId;
            //f.Id;
            //f.InternalComment;
            //f.InvoiceLineText;
            //f.InvoiceRunId;
            //f.IsPending;
            //f.LedgerAccountDescription;
            //f.LedgerAccountId;
            //f.ListPriceAmount;
            //f.ListPriceConditionId;
            //f.ListPriceTypeName;
            //f.MarketSegmentId;
            //f.MarketSegmentName;
            //f.ModelId;
            //f.NumberOfUnits;
            //f.OriginalAmount;
            //f.OriginalCurrencyId;
            //f.PaidForAccountId;
            //f.PaidForCustomerName;
            //f.PaidForInformation;
            //f.PaidForInvoiceId;
            //f.PaymentRefNum;
            //f.PeriodFrom;
            //f.PeriodizationRunId;
            //f.PeriodTo;
            //f.PriceAdjustDefId;
            //f.ProductId;
            //f.ReceiptNumber;
            //f.ReversedTransId;
            //f.TaxAmt1;
            //f.TaxAmt2;
            //f.TaxAmt3;
            //f.TaxType1;
            //f.TaxType2;
            //f.TaxType3;
            //f.TransactionSubType;
            //f.TransactionType;
            //f.TransferAccountId;
            //f.UserKey;
            //f.UserLocationName;
            #endregion

            //Instantiate a request object.
            BaseQueryRequest request = new BaseQueryRequest();
            //Page 0 returns ALL matching records. Page 0 can have a major
            //performance impact. Use it only when you know that
            //the number of matching records is small.
            request.PageCriteria = new PageCriteria { Page = 0 };
            //Instantiate the CollectionCriteria.
            request.FilterCriteria = new CriteriaCollection();
            //In this example, we have only one criteria:
            //FinancialAccountId is 401.
            //To learn how to set selection criteria in a BaseQueryRequest,
            //see the code sample called Code_BaseQuery_Request.pdf.
            request.FilterCriteria.Add(new Criteria("FinancialAccountId", FA_id));
            //Call the method.
            var ftViewCollection = viewFacade.GetFinancialTransactionView(request);
            //Display the results, one screenful at a time.
            if (ftViewCollection != null && ftViewCollection.Items.Count > 0)
                foreach (FinancialTransactionView f in ftViewCollection.Items)
                {
                    #region Pause after a screenful of records
                    //This snippet pauses when the screen is full. When the user presses
                    //Enter, the screen clears and another screenful of records is loaded.
                    //if (Console.WindowHeight == Console.CursorTop)
                    //{
                    //    Console.WriteLine("Press Enter for another screen of records.");
                    //    Console.ReadLine();
                    //    Console.Clear();
                    //}
                    #endregion
                    Console.WriteLine("Found FT {0}, total amount = {1}", f.Id, f.AmtInclTax);
                }
            else
            {
                Console.WriteLine("I found nothing.");
            }
            Console.ReadLine();
        }
    }
}
