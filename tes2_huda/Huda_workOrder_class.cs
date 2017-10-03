using PayMedia.ApplicationServices.ClientProxy;
using PayMedia.ApplicationServices.Customers.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.SharedContracts;
using PayMedia.ApplicationServices.Workforce.ServiceContracts;
using PayMedia.ApplicationServices.Workforce.ServiceContracts.DataContracts;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayMedia.ApplicationServices.Customers.ServiceContracts;

namespace tes2_huda
{
    class Huda_workOrder_class
    {
        public string msg = null;

        public void getWorkOrderByCustomerId()
        {
            #region Authenticate and create proxies
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
            var workforceService = AsmRepository.GetServiceProxyCachedOrDefault<IWorkforceService>(ah);
            #endregion
            //Instantiate the request object and define filter criteria
            //GetWorkOrdersRequest implements the BaseQueryRequest, so you can filter, sort,
            //and page the returned records.
            //You can use the properties of WorkOrder to filter and sort.
            GetWorkOrdersRequest request = new GetWorkOrdersRequest();
            request.FilterCriteria.Add("CustomerId", 16190);
            request.FilterCriteria.Add("WorkOrderStatusId", 1);
            #region Get the customer's work orders and display the results.
            WorkOrderCollection coll = workforceService.GetWorkOrders(request);

            if (coll != null && coll.Items.Count > 0)
                foreach (WorkOrder w in coll)
                {
                    Console.WriteLine("Found Work Order ID = {0}; Create Date = {1}", w.Id, w.RegisteredDateTime);
                }
            else
            {
                Console.WriteLine("Sorry--No work orders found.");
            }
            Console.ReadLine();
            #endregion
            
        }

        public void getWorkOrderUpdateWorkOrder()
        {
            #region Authenticate and create proxies
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
            var workforceService = AsmRepository.GetServiceProxyCachedOrDefault<IWorkforceService>(ah);
            #endregion
            //Instantiate the request object and define filter criteria
            //GetWorkOrdersRequest implements the BaseQueryRequest, so you can filter, sort,
            //and page the returned records.
            //You can use the properties of WorkOrder to filter and sort.
            GetWorkOrdersRequest request = new GetWorkOrdersRequest();
            request.FilterCriteria.Add("CustomerId", 16190);
            request.FilterCriteria.Add("WorkOrderStatusId", 1);
            #region Get the customer's work orders and display the results.
            WorkOrderCollection coll = workforceService.GetWorkOrders(request);

            if (coll != null && coll.Items.Count > 0)
                foreach (WorkOrder w in coll)
                {
                    Console.WriteLine("Found Work Order ID = {0}; Create Date = {1}", w.Id, w.RegisteredDateTime);
                }
            else
            {
                Console.WriteLine("Sorry--No work orders found.");
            }
            Console.ReadLine();
            #endregion
            //Now, instantiate a new WorkOrder and call GetWorkOrder to populate it with the
            // values of the work order you want to change.

            //"1333" is the ID of the work order you want to change.
            WorkOrder workOrder = workforceService.GetWorkOrder(1333);
            //Pass in the values you want to change.
            //workOrder.ServiceProviderId = 32086;
            //workOrder.AddressId = 1215;
            workOrder.ProblemDescription = "bbbbbbbbbbbbbbbbbbbbbbb";


            //"0" is the default reason. ICC throws an error if a default reason is not configured.
            //int reasonKey = 0;
            int reasonKey = 12; // for update problem description field
            //Call UpdateWorkOrder to update the work order and display the results.
            WorkOrder updatedWorkOrder = workforceService.UpdateWorkOrder(workOrder, reasonKey);
            Console.WriteLine("Updated WorkOrder ID {0}; Service Provider ID = {1}", updatedWorkOrder.Id, updatedWorkOrder.ServiceProviderId);
            Console.ReadLine();
        }

        public void getServiceProvider(WorkOrder wo)
        {
            #region Authenticate and create proxies
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
            var custService = AsmRepository.AllServices.GetCustomersService(ah);
            var custcService = AsmRepository.AllServices.GetCustomersConfigurationService(ah);
            var woService = AsmRepository.AllServices.GetWorkforceService(ah);


            #endregion

            try
                {
                    var va = custService.GetAddress(wo.AddressId.Value);

                    var geo = custcService.FindGeoDefinitions(new GeoDefinitionCriteria()
                    {
                        PostalCode = va.PostalCode
                    });

                    int spid = wo.ServiceProviderId.Value;
                    int serviceTypeId = wo.ServiceTypeId.Value;
                    int[] geos = new int[1];
                    geos[0] = geo.Items[0].Id.Value;

                    Console.WriteLine("ServiceProvider Id : " + spid + " Service Type Id :  " + serviceTypeId + " GeoDef ID :  " + geos[0]);

                    // Get Service Provider Service Info
                    var sps = woService.GetServiceProviderServicesByServiceProviderIdServiceTypeIdAndGeoDefIds(
                        spid, serviceTypeId, geos, 0
                    );

                    foreach (var sps1 in sps.Items)
                    {
                        Console.WriteLine("Servoce Provider Service : " + sps1.Description + " Service Provider Id :  " + sps1.ServiceProviderId + " Service Type Id " + sps1.ServiceTypeId + " Geo Group ID :  " + sps1.GeoDefinitionGroupId);

                        var timeslot = woService.GetTimeSlotsByServiceProviderServiceId(sps1.Id.Value, DateTime.Now);

                        // print the timeslot for this service
                        for (int i = 0; i < timeslot.Length; i++)
                        {
                            Console.WriteLine("Date : " + timeslot[i].Date + "  Time Slot ID : " + timeslot[i].TimeSlotId + " Service Provider Service Id : " + timeslot[i].ServiceProviderServiceId + "Remain Slot : " + timeslot[i].RemainingCapacity + " Is Avaiable : " + timeslot[i].IsAvailable);
                        }
                    }

                }
                catch (Exception ex)
                {
                    var msg = "Exceptions : " + ex.Message + " Stack : " + ex.StackTrace;
                    Console.WriteLine(msg);
                    }
                
        }

        public void getTimeslotbyPostalcode(int address_id, int service_type_id)
        {
            #region Authenticate and create proxies
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
            var custService = AsmRepository.AllServices.GetCustomersService(ah);
            var custcService = AsmRepository.AllServices.GetCustomersConfigurationService(ah);
            var woService = AsmRepository.AllServices.GetWorkforceService(ah);

            #endregion

            DateTime myDate = DateTime.ParseExact("2017-08-08 09:00:00", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

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
                Console.WriteLine("Service Provider Id : " + serviceprovider.ServiceProviderId + "  Name : " + serviceprovider.ServiceProviderName + "  Service Type Id : " + serviceprovider.ServiceTypeId + " Service ID : " + serviceprovider.ServiceId + " Geo Group Id :" + serviceprovider.GeoDefinitionGroupId);

                var sps = woService.GetServiceProviderServiceByServiceTypeGeoDefGroupIdandProviderId(serviceprovider.ServiceTypeId.Value, serviceprovider.GeoDefinitionGroupId.Value, serviceprovider.ServiceProviderId.Value);
                var timeslot = woService.GetTimeSlotsByServiceProviderServiceId(sps.Id.Value, myDate);
                // print the timeslot for this service
                for (int i = 0; i < timeslot.Length; i++)
                {
                    Console.WriteLine("Date :" + timeslot[i].Date + "  Time Slot ID : " + timeslot[i].TimeSlotId + " Service Provider Service Id : " + timeslot[i].ServiceProviderServiceId + "Remain Slot : " + timeslot[i].RemainingCapacity + " Is Avaiable : " + timeslot[i].IsAvailable);
                }
            }

        }

        public void createworkorder()
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");

            var workforceService = AsmRepository.GetServiceProxyCachedOrDefault<IWorkforceService>(ah);
            var sandboxService = AsmRepository.AllServices.GetSandBoxManagerService(ah);

            int sandboxId = sandboxService.CreateSandbox();

            int var_starting_timeslot_id = 0;

            int reasonKey = 14659;

            WorkOrder workOrder = new WorkOrder();

            workOrder.AgreementId = 6420654;
            workOrder.CustomerId = 500038560; // 500045570
            workOrder.FinancialAccountId = 4281525;
            workOrder.ServiceProviderId = 32958;
            workOrder.ServiceTypeId = 1;
            
            DateTime myDate = DateTime.ParseExact("2017-09-04 01:00:00 PM", "yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);
            workOrder.ServiceDateTime = myDate;

            int dy = myDate.Day;
            int mn = myDate.Month;
            int yy = myDate.Year;

            int hh = myDate.Hour;
            int mm = myDate.Minute;
            int ss = myDate.Second;


            //DateTime date1 = DateTime.ParseExact(yy + "-" + mn + "-" + dy + " " + "09:00:00 AM", "yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);
            //DateTime date2 = DateTime.ParseExact(yy + "-" + mn + "-" + dy + " " + "12:00:00 PM", "yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);
            //DateTime date3 = DateTime.ParseExact(yy + "-" + mn + "-" + dy + " " + "01:00:00 PM", "yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);
            //DateTime date4 = DateTime.ParseExact(yy + "-" + mn + "-" + dy + " " + "04:00:00 PM", "yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);


            if (hh.ToString() == "09" || hh.ToString() == "9")
                var_starting_timeslot_id = 1;
            else if (hh.ToString() == "12")
                var_starting_timeslot_id = 2;
            else if (hh.ToString() == "13" || hh.ToString() == "1")
                var_starting_timeslot_id = 3;
            else if (hh.ToString() == "16" || hh.ToString() == "4")
                var_starting_timeslot_id = 4;

            //workOrder.ServiceProviderServiceId = 1;
            workOrder.AddressId = 10310193;

            int var_geodef_id = find_geodefinition("11520001");
            int var_servprovserv_id = getserviceproviderservice_byserviceprovideridandgeoid(workOrder.ServiceProviderId.Value, workOrder.ServiceTypeId.Value, var_geodef_id);

            workOrder.ServiceProviderServiceId = var_servprovserv_id;

            workOrder.ProblemDescription = "Customer spilled coffee on decoder.";
            workOrder.SandboxSkipValidation = false;

            workOrder.WorkOrderServices = new WorkOrderServiceCollection
                {
                    new WorkOrderService
                    {
                        //Required. Replace this value with a valid ID for the work order
                        //service.
                        ServiceId = 84,
                        //Replace this value with the serial number of the device that needs to
                        //be repaired.
                        DeviceSerialNumber = "2010001019",
                        //Replace this value with a valid Reason ID for event 62.
                        //ReasonId = 14659,
                        //Replace this value with how many of the services the customer needs.
                        Quantity = 1,
                    }
                };


            AsmRepository.AllServices.GetCustomersService(ah).GetCustomer(workOrder.CustomerId.Value);
            var wo = workforceService.CreateWorkOrderWithStartingTimeslot(workOrder, new TimeSlotAllocationParameters { StartingTimeSlotId = 1 }, reasonKey);
            //WorkOrder wo = workforceService.CreateWorkOrder(workOrder, reasonKey);
            if (wo.WorkOrderServices.Items.Count == 0)
            {
                Console.WriteLine("Something bad happened. Sandbox rolled back.");
                Console.ReadLine();
                //The "false" value in this line of code rolls back the sandbox.
                sandboxService.FinalizeSandbox(sandboxId, false);
                return;
            }

            sandboxService.FinalizeSandbox(sandboxId, true);
            Console.WriteLine("Congratulations! Created Work Order ID = {0}.", wo.Id);
            Console.WriteLine(var_starting_timeslot_id);
            //UpdateWorkOrderWithTimeSlot(wo, 62);

            Console.ReadLine();
        }

        public int find_geodefinition(string param_postal_code)
        {
            int var_geodefid = 0;
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://192.168.177.4/asm/all/servicelocation.svc");

            var customersConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersConfigurationService>(ah);
            var customersService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersService>(ah);

            //Instantiate a request object
            GeoDefinitionCriteria criteria = new GeoDefinitionCriteria();
            //If you know the customer's address ID, you can replace "481" with the addressId
            //and use the returned address to populate the address criteria.
            //Address address = customersService.GetAddress(param_serv_addr_id);
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
            criteria.FindByPostalCode = true;
            criteria.PostalCode = param_postal_code;
            //Get the GeoDefinition for the address. There should be only one.
            //If there are more than one, then there is an error in the GeoDefinition
            //configuration.
            GeoDefinitionCollection collection = customersConfigurationService.FindGeoDefinitions(criteria);
            //Display the results.
            if (collection != null && collection.Items.Count > 0)
            {
                foreach (var item in collection)
                {
                    var_geodefid = item.Id.Value;
                    //Console.WriteLine("GeoDefId {0} <==> geolocation {1}", item.Id, item.GeoLocations);
                }
                return var_geodefid;
            }
            else
            {
                return 0;
                //Console.WriteLine("I found nothing.");
            }
            //Console.ReadLine();


        }
        public int getserviceproviderservice_byserviceprovideridandgeoid(int param_serv_prov_id, int param_serv_type_id, int param_geodef_id)
        {
            int var_servprovservid = 0;
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://192.168.177.4/asm/all/servicelocation.svc");

            var customersConfigurationService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersConfigurationService>(ah);
            var customersService = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersService>(ah);
            var aa = AsmRepository.AllServices.GetWorkforceService(ah);

            int[] arr1 = new int[] { param_geodef_id };

            var bb = aa.GetServiceProviderServicesByServiceProviderIdServiceTypeIdAndGeoDefIds(param_serv_prov_id, param_serv_type_id, arr1, 1);

            foreach (var item in bb.Items)
            {
                var_servprovservid = item.Id.Value;
                //Console.WriteLine("serviceproviderserviceid {0} <==> ServiceProviderId {1} <==> servicetytpeid = {2}", item.Id, item.ServiceProviderId, item.ServiceTypeId);
            }

            return var_servprovservid;


        }


        public WorkOrder UpdateWorkOrderWithTimeSlot(WorkOrder wo, int reasonid)
        {

            WorkOrder wou = null;
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
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
                        ServiceDate = wo.ServiceDateTime, // 
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
        // Update Work Order without time slot
        public WorkOrder UpdateWorkOrderWithOutTimeSlot(WorkOrder wo, int reason_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);
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

        public WorkOrder UpdateWorkOrderWithOutTimeSlotComplete(WorkOrder wo, int reason_id)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);
            var woService = AsmRepository.AllServices.GetWorkforceService(authHeader);
            try
            {
                wo.ProblemDescription = wo.ProblemDescription + " updated by reason";
                wo.ReasonKey = 99470;
                WorkOrder ww = woService.UpdateWorkOrder(wo, 99470);
                return ww;
            }
            catch (Exception ex)
            {
                msg = "Exceptions : " + ex.Message + " Stack : " + ex.StackTrace;
                Console.WriteLine(msg);
                return null;
            }

        }

        public WorkOrder CompleteWorkOrder(WorkOrder wo_param, int reason_param,  string work_desc_param, string action_taken, int completion_reason_id_param)
        {
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader authHeader = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl(var_auth.var_url_asm);

            WorkOrder ww = null;
            var woService = AsmRepository.AllServices.GetWorkforceService(authHeader);
            var wo = AsmRepository.AllServices.GetWorkforceService(authHeader).GetWorkOrder(wo_param.Id.Value);

           
            try
            {
                string temp_work_description = wo_param.ProblemDescription;
                string temp_action_taken = wo_param.ActionTaken;

                wo.ProblemDescription = temp_work_description + "#" + work_desc_param;
                wo.ActionTaken = temp_action_taken + "#" + action_taken;
                wo.CompletedDateTime = DateTime.Now;
                wo.ReasonKey = 99470;

                ww = woService.CompleteWorkOrder(wo, 99470);
                return ww;

            }
            catch (Exception ex)
            {
                msg = "Exceptions : " + ex.Message + " Stack : " + ex.StackTrace;
                Console.WriteLine(msg);
            }
            //
            Console.WriteLine(msg);
            return ww;
        }

    }
}  
