using PayMedia.ApplicationServices.BillingEngine.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.ClientProxy;
using PayMedia.ApplicationServices.SharedContracts;
using PayMedia.ApplicationServices.Workforce.ServiceContracts.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json; //library package from nuget
using Newtonsoft.Json.Linq;
using System.Globalization;
using PayMedia.ApplicationServices.AgreementManagement.ServiceContracts;
using System.Threading;

namespace tes2_huda
{
    class Program
    {
        private static Mutex mutex;

        static void Main(string[] args)
        {
            string file_uri = @"\\192.168.177.109\filesfa\images\valsys\T725751608111619_instalasi_IMG_20160811_155102.jpg";  // Please make sure you upload your file to this folder before you call this API!!!
            string file_name = "T725751608111619_instalasi_IMG_20160811_155102.jpg";
            long file_size = 1290970;

            string[] arr1 = new string[] { "500016733", "500016662", "500016635", "500016564", "500016555", "500016537", "500016493", "500016368", "500018609", "500018645" };

            Huda_customer_class var_yang_class = new Huda_customer_class();
            Huda_workOrder_class var_yang_class_wo = new Huda_workOrder_class();
            Yang_class var_yang_class_1 = new Yang_class();
            Huda_device_class var_device = new Huda_device_class();

            mutex = new Mutex(true, "MyMutex");
            if (!mutex.WaitOne(0, false))
            {
                return;
            }
            else
            {
                //var_yang_class.create_new_customer();
                //var_device.retrieveDevice("DS170417000090");

                //var_yang_class_1.huda_ship_so(500167847, 156809);

                #region shippping order //2, 3, 4, 7, 10, 13, 30, 393


                int so_id = 157260;
                int cust_id = 500171897;


                var tech_prod_id_format = var_yang_class_1.getformat_bycallshippingorderbyid_2(so_id);

                List<string> serial_number = new List<string>(new string[] {

                      "",
                      "PE14070001586D",
                      "02000055756852",
                      "DS170417000392",

                      "67069A01181525967",
                      "",
                      "02000055756860",
                      "",

                      "67069A01181576599",
                      "",
                      "02000055756902",
                      ""

                });

                var_yang_class_1.MayShipSO_ver1(cust_id);
                var_yang_class_1.huda_shippingOrder_1(cust_id, so_id, tech_prod_id_format, serial_number);

                //var_yang_class_1.huda_ship_so(cust_id, so_id);

                //var_yang_class_1.getformat_bycallshippingorderbyid(156757);
                #endregion shippping order

                //var_yang_class.create_offer();
                //var_yang_class.create_new_customer();

                ////update SO to may ship
                //Console.WriteLine("Change shipping order to May Ship of customer " + 500167604 + " ");
                //var_yang_class_1.MayShipSO_ver1(500167604);

                //// Ship shipping order
                //Console.WriteLine("Ship shipping order of customer " + 500167604 + " ");
                //var var_shipping_order = var_yang_class_1.ShipShippingOrder_ver2(500167604, "67069A01184309334", "02000055748685", false, "PRSD0000001067", "DS170417000066", 156778, false);

                #region workorder operation
                //Authentication_class var_auth = new Authentication_class();
                //AuthenticationHeader authHeader = var_auth.getAuthentication_header();
                //AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);
                //var wocs = AsmRepository.AllServices.GetWorkforceService(authHeader).GetWorkOrder(60347);  // change 123 to your work order id.
                //Huda_workOrder_class var_wo = new Huda_workOrder_class();

                //var wo = new Yang_class();
                //const int reason_assign_wo = 19545;
                //const int reason_inprogress_wo = 99125;
                //const int reason_reschedule_wo = 20941;
                //const int reason_complete_wo = 23035;
                //const int reason_update_wo = 12;
                //const int reason_cancel_wo_RequestedBySub = 99367;

                //Edit WorkOrder, only change the service date time, you can change more like technician(AssociatedId) or the problemdescription.

                //wocs.AssociateId = AsmRepository.AllServices.GetCustomersService(authHeader).GetAssociatesByRequest(new BaseQueryRequest()
                //{
                //    FilterCriteria = new CriteriaCollection() {
                //            new Criteria() {
                //                Key = "CustomerId", Operator=Operator.Equal,Value=wocs.ServiceProviderId.Value.ToString()
                //            },
                //            new Criteria() {
                //                Key = "Active", Operator=Operator.Equal,Value="true"
                //            },
                //            new Criteria() {
                //                Key = "AssociateTypeId", Operator=Operator.Equal,Value="1"
                //            },
                //       }
                //}).Items[0].Id.Value;


                //Assign WorkOder
                //Console.WriteLine("Assgin workorder " + wocs.Id.Value);
                //var wou = wo.UpdateWorkOrderWithTimeSlot(wocs, reason_assign_wo);
                //var wou = wo.UpdateWorkOrderWithOutTimeSlot(wocs, reason_assign_wo);

                // update work order without change status
                //Console.WriteLine("Update work order with out change anything " + wocs.Id.Value);
                //wou.ProblemDescription = "It has been changed!!!!!";
                //var wou = wo.UpdateWorkOrderWithTimeSlot(wocs, reason_update_wo);

                //Change WorK Order to Working status
                //Console.WriteLine("change workorder " + wocs.Id.Value + " to In Progress.");
                //wou = wo.UpdateWorkOrderWithOutTimeSlot(wocs, reason_inprogress_wo);

                // reschedule work order : current work order status must Working
                //Console.WriteLine("Reschedule Work order after wait for rescheduled workorder " + wocs.Id.Value);
                //var wou = wo.UpdateWorkOrderWithTimeSlotForReschedule(wocs, reason_reschedule_wo);

                // assign work order after reschedule: current work order status must be Assigned
                //Console.WriteLine("Assgine Work order after reschedule workorder " + wocs.Id.Value);
                //wou = wo.UpdateWorkOrderWithOutTimeSlot(wou, reason_assign_wo);

                //Change WorKOrder to In Progress status
                //Console.WriteLine("change workorder " + wou.Id.Value + " to In Progress.");
                //wou = wo.UpdateWorkOrderWithOutTimeSlot(wou, reason_inprogress_wo);

                // Cancel Work Order, uncomment it if you want to use. When you use it, you can’t complete work order anymore. So comment below complete work order when you use it
                //Console.WriteLine("Cancel workorder " + wocs.Id.Value + "");
                //wo.CancelWorkOrder(wocs, reason_cancel_wo_RequestedBySub);


                //Complete WorkOrder
                //var wou = var_yang_class_wo.CompleteWorkOrder(wocs, 99470, "aaaaaa", "bbbbbbb", 99470);
                //Console.WriteLine(DateTime.Now + " : Close workorder " + wou.Id.Value);
                #endregion workorder operation


            }




            //var_yang_class.getTimeslotbyPostalcode(11140000);

            //var_yang_class.importDevicesbyFile(@"c:\foo\bar\blop\blap.txt", 1, 1);
            //var_yang_class.sendCommand("01000006011381");
            //var_yang_class.createRelationshipForCustomer(500016733, 500016635, "G");
            //var_yang_class.updateRelatioshipForCustomer(500016733, 500016635, "G");
            string decodersn = "12345";   // can be used for router sn
            string smartcardsn = "54321";   // can be used for sim card sn

            //Yang_class var_yang_class = new Yang_class();
            //var_yang_class.uploadFile("D:/muhammadali.jpg", 500020335);
            //var var_uri = new System.Uri("//192.168.177.109/filesfa/images/valsys/T725751608111619_instalasi_IMG_20160811_155102.jpg");
            //Console.WriteLine(var_uri);
            //var_yang_class_1.linkDocument(500016733, file_name, "//192.168.177.109/filesfa/images/valsys/T725751608111619_instalasi_IMG_20160811_155102.jpg", file_size);
            //var_yang_class.GetCustomerDeviceView(500039270);

            //get shipping order
            //yang_class.getShippingOrderbyCustomerID(500039725);
            string str_decoder = null;
            string str_vc = null;

            int[] arr_cust_id = new int[] {
                500008595,
                500008530,
                500008432,
                500008432,
                500008334,
                500008334,
                500008399,
                500008343,
                500008236,
                500008165,
                500008218,
                500008129,
                500008192,
                500008138,
                500007983,
                500006960,
                500007221,
                500007203,
                500007080,
                500007070,
                500006600,
                500006914,
                500006899,
                500006880,
                500006816,
                500006763,

                500006727,
                500006727,
                500006683,
                500006656,
                500006629,
                500006576,
                500006530,
                500006389,
                500006324,
                500006217,
                500006164,
                500006137,
                500006084,
                500006039,
                500006020


            };
            int[] arr_so_id = new int[] {
                0,
                155962,
                155955,
                155955,
                155947,
                155947,
                155951,
                155948,
                0,
                155934,
                156125,
                156118,
                155936,
                155931,
                155921,
                155909,
                156116,
                155916,
                156115,
                155910,
                155880,
                155904,
                155902,
                156107,
                155898,
                155895,

                155892,
                155892,
                156136,
                156134,
                155881,
                155879,
                155878,
                156103,
                155865,
                155860,
                155856,
                156099,
                155852,
                155849,
                155845





            };
            string[] arr_decoder = new string[] {
                "0",
                "0",
                "5941134164711044",
                "5941131496285143",
                "3872732097761214",
                "0",
                "3872732104210205",
                "3872732108584829",
                "5505934191696912",
                "3872732106017293",
                "5505934134005692",
                "3873232086434088",
                "0",
                "3872732108694172",
                "3872732108605327",
                "3872732104220790",
                "5505934134176170",
                "0",
                "5505934130401259",
                "0",
                "3867534166229363",
                "0",
                "0",
                "3873232101328133",
                "3873232090918043",
                "3867531490949245",

                "3872732092248977",
                "3873232098616177",
                "5505934184093861",
                "5941131490983669",
                "0",
                "5505934164647322",
                "5941134165886464",
                "3873232096838591",
                "0",
                "3867531481267045",
                "0",
                "1978832095739977",
                "3873232SMG001815",
                "3872732104216368",
                "1978832108802770",


            };
            string[] arr_vc = new string[] {
                "02000043345768",
                "02000027358506",
                "02000043385780",
                "02000043385780",
                "02000063997647",
                "02000060471182",
                "02000046470530",
                "02000043205236",
                "02000043323765",
                "02000056245731",
                "02000043287606",
                "02000056871726",
                "02000043339670",
                "02000042844811",
                "02000048210074",
                "02000043392810",
                "02000043267384",
                "02000043265016",
                "02000042763672",
                "02000043286095",
                "02000024365249",
                "02000043339761",
                "02000043287028",
                "02000047622246",
                "02000062055090",
                "02000043266741",


                "02000037317963",
                "02000037347226",
                "02000043345024",
                "02000062531793",
                "02000043339795",
                "02000026619064",
                "02000030074678",
                "02000045083763",
                "02000055823785",
                "02000042612614",
                "02000042775791",
                "02000043283613",
                "02000021917059",
                "02000053292710",
                "02000053598355"

            };


            //for(int i =0; i < arr_cust_id.Length; i++)
            //{
            //    Console.WriteLine("================================================================================");
            //    Console.WriteLine(arr_cust_id[i].ToString() +"===="+arr_decoder[i]+"====="+arr_vc[i] +"====="+arr_so_id[i]);

            //    if(arr_so_id[i].ToString() == "0")
            //    {
            //        Console.WriteLine("================================================================================");
            //        continue;
            //    }
            //    else
            //    {
            //        // update SO to may ship
            //        Console.WriteLine("Change shipping order to May Ship of customer " + arr_cust_id[i] + " ");
            //        var_yang_class_1.MayShipSO_ver1(arr_cust_id[i]);

            //        if (arr_decoder[i].ToString() == "0")
            //            str_decoder = "";

            //        if (arr_vc[i].ToString() == "0")
            //            str_vc = "";

            //        // Ship shipping order
            //        Console.WriteLine("Ship shipping order of customer " + arr_cust_id[i] + " ");
            //        var var_shipping_order = var_yang_class_1.ShipShippingOrder_ver2(arr_cust_id[i], str_decoder, str_vc, false, "", "", 155948, false);

            //        Authentication_class var_auth = new Authentication_class();
            //        AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            //        AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            //        if (var_shipping_order != null)
            //        {

            //            var agreementManagementService = AsmRepository.GetServiceProxyCachedOrDefault<IAgreementManagementService>(authHeader);
            //            var adc = agreementManagementService.GetAgreementDetailsForCustomer(arr_cust_id[i], 1); //zzzzzzzzzzz
            //            Console.WriteLine(adc);
            //        }
            //        else
            //            Console.WriteLine("null");




            //        Console.WriteLine("================================================================================");
            //    }


            //}


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////



            //Console.WriteLine("================================================================================");

            //// update SO to may ship
            //Console.WriteLine("Change shipping order to May Ship of customer " + 500004323 + " ");
            //var_yang_class_1.MayShipSO_ver1(500004323);

            //// Ship shipping order
            //Console.WriteLine("Ship shipping order of customer " + 500004323 + " ");
            //var var_shipping_order = var_yang_class_1.ShipShippingOrder_ver2(500004323, "3882823595776404", "02000042896571", false, "", "", 155697, false);

            //Authentication_class var_auth = new Authentication_class();
            //AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            //AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            //if (var_shipping_order != null)
            //{

            //    var agreementManagementService = AsmRepository.GetServiceProxyCachedOrDefault<IAgreementManagementService>(authHeader);
            //    var adc = agreementManagementService.GetAgreementDetailsForCustomer(500004323, 1); //zzzzzzzzzzz
            //    Console.WriteLine(adc);
            //}
            //else
            //    Console.WriteLine("null");




            //Console.WriteLine("================================================================================");



            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////


            #region retrieve device
            //-----------  Retrive Device   ----------------

            //string device_serial_number = "01000006011548";    // change real device serial number
            //int reason156_RetriveDevice = 99146;
            //int reason183_Returned = 99149;
            //int reason156_InStock = 99152;
            //int reason156_ReturnToManufactory = 99151;
            //int reason156_RepairedStock = 99153;
            //int reason156_RefurbishedStock = 99154;
            //int reason156_Damaged = 99155;

            //int to_stockhandler_id = 1015;   // the destination stocck handler of the device

            //var deviceh = new deviceHandler(authHeader);

            // First step - change to to be return
            //var_yang_class.updateDeviceStatus(device_serial_number, reason156_RetriveDevice);

            // Second step - change stock handler
            //var_yang_class.transferDevice(reason183_Returned, to_stockhandler_id, device_serial_number);


            // Third Step - option 1 : change to in stock
            //var_yang_class.updateDeviceStatus(device_serial_number, reason156_InStock);

            // Third Step - option 2 : change to repaired stock
            //var_yang_class.updateDeviceStatus(device_serial_number, reason156_RepairedStock);

            // Third Step - option 3 : change to refurbished stock
            //var_yang_class.updateDeviceStatus(device_serial_number, reason156_RefurbishedStock);

            // Third Step - option 4 : change to return to manufactory
            //var_yang_class.updateDeviceStatus(device_serial_number, reason156_ReturnToManufactory);

            // Third Step - option 5 : change to damaged
            // var_yang_class.updateDeviceStatus(device_serial_number, reason156_Damaged);


            //-----------       ---------------
            #endregion retrieve device

            //int aaa = var_yang_class.getBubyZipcode("13430000");

            //Console.WriteLine(yang_class.getPendingQuoteAmount(500039485));

            //Huda_payment_class var_payment = new Huda_payment_class();

            //string the_amount = var_payment.fetch_data_from_oracle("500016635");
            //string the_json =  var_payment.get_request_to_api("http://192.168.177.185:1111/api/hdkartika/ICCMsky2016/payment/doPaymentWithFulfillQuote/500016635/" + the_amount);
            //dynamic parsedJson = JsonConvert.DeserializeObject(the_json);
            //Console.WriteLine(parsedJson);
            //Console.Read();
            //Yang_class var_yang_class = new Yang_class();
            //var_yang_class.getPendingQuoteAmount(500020308);
            //var max_quote_id = var_yang_class.InsertPendingQuote(3630223); //3630223



            //insert max_quote_id
            //var_yang_class.do_update_max_quote_id_to_table(max_quote_id);



            //DBConnect var_db = new DBConnect();
            //var_db.Insert();


           


            Console.Read();

           

        }


    }
}
