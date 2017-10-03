using PayMedia.ApplicationServices.AgreementManagement.ServiceContracts;
using PayMedia.ApplicationServices.AgreementManagement.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.ClientProxy;
using PayMedia.ApplicationServices.Devices.ServiceContracts;
using PayMedia.ApplicationServices.Devices.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tes2_huda
{
    class Huda_device_class
    {
        public void GetCustomerDeviceView(int cust_id)
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
            DevicePerAgreementDetailCollection dpads = agreementManagementService.GetDevicesPerAgreementDetail(agreementId, page);
            if (dpads != null && dpads.Items.Count > 0)
            {
                foreach (var d in dpads)
                {
                    Console.WriteLine("Device ID = {0}; Technical Product ID = {1} " ,  d.DeviceId, d.TechnicalProductId);
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

        public void retrieveDevice(string serial_number)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            var devicesService = AsmRepository.GetServiceProxyCachedOrDefault<IDevicesService>(ah);

            string device_serial_number = serial_number;    //device serial number
            int reason156_RetriveDevice = 99146;
            int reason183_Returned = 99149;
            int reason156_InStock = 99152;
            int reason156_ReturnToManufactory = 99151;
            int reason156_RepairedStock = 99153;
            int reason156_RefurbishedStock = 99154;
            int reason156_Damaged = 99155;

            int to_stockhandler_id = 1015;   // the destination stocck handler of the device

            //var deviceh = new deviceHandler(authHeader);

            // First step - change to to be return
            updateDeviceStatus(device_serial_number, reason156_RetriveDevice);

            // Second step - change stock handler
            transferDevice(reason183_Returned, to_stockhandler_id, device_serial_number);


            // Third Step - option 1 : change to in stock
            updateDeviceStatus(device_serial_number, reason156_InStock);

            // Third Step - option 2 : change to repaired stock
            //var the_device = updateDeviceStatus(username_ad, password_ad, device_serial_number, reason156_RepairedStock);

            // Third Step - option 3 : change to refurbished stock
            //var the_device = updateDeviceStatus(username_ad, password_ad, device_serial_number, reason156_RefurbishedStock);

            // Third Step - option 4 : change to return to manufactory
            //var the_device = updateDeviceStatus(username_ad, password_ad, device_serial_number, reason156_ReturnToManufactory);

            // Third Step - option 5 : change to damaged
            //var the_device = updateDeviceStatus(username_ad, password_ad, device_serial_number, reason156_Damaged);


            //-----------       ---------------
            Device devices = devicesService.GetDeviceBySerialNumber(serial_number);
            
        }
        public void updateDeviceStatus( string serial_number, int reason_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            var whService = AsmRepository.AllServices.GetLogisticsService(authHeader);
            var prodcService = AsmRepository.AllServices.GetProductCatalogConfigurationService(authHeader);
            IDevicesService deviceService = AsmRepository.AllServices.GetDevicesService(authHeader);

            int return_status = 0;

            try
            {
                Device device = deviceService.GetDeviceBySerialNumber(serial_number);
                if (device != null)
                {
                    deviceService.UpdateDeviceStatus(device.Id.Value, reason_id);
                    //return_status = 1;
                    Console.WriteLine("device id = " + device.Id, "serial number = " + device.SerialNumber);
                }
                else
                {
                    //return_status = 0;
                    Console.WriteLine("Can't find device with serial number : " + serial_number);
                }
            }
            catch (Exception ex)
            {
                //return null;
                Console.WriteLine("Errors : " + ex.Message);
                Console.WriteLine("Stack : " + ex.StackTrace);
            }

        }

        public void transferDevice( int reason183, int to_stockhandler_id, string sn)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

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
    }
}
