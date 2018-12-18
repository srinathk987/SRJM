using Newtonsoft.Json;
using RMS.Core;
using RMS.Core.Interfaces;
using RMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace RMS.Controllers
{
    public class TakeAwayController : Controller
    {
        private ITakeAway objPOSTransaction = new TakeAwayRepository();
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private IPOSMasters posMasters = new POSMastersRepository();
        private IPOSTransaction objPOSTransaction1 = new POSTransactionRepository();
        DataTable dt = null;
        string JsonString = "";

        public ActionResult PhoneDelivery()
        {
            return View();
        }
        public ActionResult ThirdParty()
        {
            return View();
        }
        public ActionResult TakeAwayAtHotel()
        {

            return View();
        }
        public ActionResult TakeAwayAtHotelNewOrder()
        {
            return View();
        }
        public ActionResult ThirdPartyDetails()
        {
            return View();
        }
        public ActionResult ThirdPartyBillSettlement()
        {
            return View();
        }
        public ActionResult ThirdPartyNewOrders()
        {
            return View();
        }
        public ActionResult NewOrder()
        {
            return View();

        }
        public ActionResult DeliveryCharges()
        {
            return View();
        }


        public ActionResult ItemsForDelivery()
        {
            int userid = Convert.ToInt32(this.Session["UserId"]);
            List<KOTTakingDTO> lstSessionDetails = objPOSTransaction1.ShowSessionDetailsForSession("48", userid);
            List<ItemsDTO> lstItems = objPOSTransaction.ShowItemsForSession();
            var result = new { lstItems = lstItems, lstSessionDetails = lstSessionDetails };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SaveHomeDelivery(string session, string CustomerDetails, string ItemDetails)
        {

            CustomerDetails Customer = serializer.Deserialize<CustomerDetails>(CustomerDetails);
            List<ItemsDTO> ListItems = serializer.Deserialize<List<ItemsDTO>>(ItemDetails);


            Customer.statusid = 1;
            Customer.createdby = Convert.ToInt32(Session["UserId"]);
            int Count = objPOSTransaction.SaveHomeDelivery(session, Customer, ListItems);
            var Alldata = new { Count = Count, vchKotId = "" };
            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeliveryItemsForCustomer()
        {
            DataTable dt = objPOSTransaction.DeliveryItemsForCustomer();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var result = new { lstItems = JsonString };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult UpdateeliveryStatus(string OderNo, string Status, string DeliveryBoydetails)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();

            string TorF = objPOSTransaction.UpdateeliveryStatus(OderNo, Status, DeliveryBoydetails, createdby);
            DataTable dt = objPOSTransaction.DeliveryItemsForCustomer();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString, TorF = TorF.Split('-')[0], BillNo = TorF.Split('-')[1] };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);

        }
        public JsonResult UpdateReturnStatus(string OrderReason, string Nameofcust, string OrderNo, string contactno, decimal netamount)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();

            bool TorF = objPOSTransaction.UpdateReturnStatus(OrderReason, Nameofcust, OrderNo, contactno, netamount);
            DataTable dt = objPOSTransaction.DeliveryItemsForCustomer();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString, TorF = TorF };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);

        }

        public ActionResult SaveTakeAwayAthotel(string session, CustomerDetails CustomerDetails, List<ItemsDTO> ItemDetails)
        {
            CustomerDetails.statusid = 1;
            CustomerDetails.createdby = Convert.ToInt32(Session["UserId"]);
            string billNo = string.Empty;
            string TAKEAWAYID = string.Empty;
            int Count = objPOSTransaction.SaveTakeAwayAthotel(session, CustomerDetails, ItemDetails, out billNo, out TAKEAWAYID);
            var Alldata = new { Count = Count, billNo = billNo, TAKEAWAYID = TAKEAWAYID };
            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #region Home Delivery Charges
        public ActionResult SaveHomeDeliveryCharges(DeliveryChargesDTO DeliveryCharges)
        {
            try
            {
                DeliveryCharges.vchstatus = 1;
                DeliveryCharges.createdby = Convert.ToInt32(Session["UserId"]);
                int Count = objPOSTransaction.SaveHomeDeliveryCharges(DeliveryCharges);
                var Saveddata = new { Count = Count };
                return new JsonResult { Data = Saveddata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult ShowDeliveryChargesDetails()
        {
            DataTable dt = new DataTable();
            dt = objPOSTransaction.DeliveryChargesDetails();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var result = new { lstItems = JsonString };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult UpdateDeliveryCharges(DeliveryChargesDTO country)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            bool TorF = objPOSTransaction.UpdateDeliveryCharges(country);
            var Totalresult = new { Data = TorF, TorF = TorF };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteDeliveryCharges(DeliveryChargesDTO DeliveryCharges)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            bool TorF = objPOSTransaction.DeleteDeliveryCharges(DeliveryCharges);
            var Totalresult = new { Data = TorF, TorF = TorF };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult getdeliveryCharges(string PinCode)
        {
            string Charges = objPOSTransaction.getdeliveryCharges(PinCode);
            var result = new { Charges = Charges };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult getCustomerInformation(string MobileNo)
        {

            DataTable dt = objPOSTransaction.getCustomerInformation(MobileNo);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var result = new { CustomerInformation = JsonString };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetDeliveryBoyDetails()
        {
            DataTable dt = objPOSTransaction.GetDeliveryBoyDetails();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var result = new { DeliveryBoyDetails = JsonString };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult GetItemsOfSpecicOrder(string orderNo)
        {
            DataTable dt = objPOSTransaction.GetItemsOfSpecicOrder(orderNo);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var result = new { DeliveryBoyDetails = JsonString };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult DeliveryItemsForCustomerInTakeAway()
        {

            DataTable dt = objPOSTransaction.DeliveryItemsForCustomerInTakeAway();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var result = new { lstItems = JsonString };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult SaveThirdPartyNewOrderDetails(string session, CustomerDetails CustomerDetails, List<ItemsDTO> ItemDetails)
        {
            CustomerDetails.statusid = 1;
            CustomerDetails.createdby = Convert.ToInt32(Session["UserId"]);
            string billNo = string.Empty;
            string TAKEAWAYID = string.Empty;
            int Count = objPOSTransaction.SaveThirdPartyNewOrderDetails(session, CustomerDetails, ItemDetails, out billNo, out TAKEAWAYID);
            var Alldata = new { Count = Count, ThirpartyId = CustomerDetails.ThirdpartyId, ThirpartyRecordId = CustomerDetails.ThirdpartyRecorId, ThirpartyName = CustomerDetails.ThirpartyName, TAKEAWAYID = TAKEAWAYID };
            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetThirdPartyOders(string ThirdpartyRecorId, string ThirdpartyId)
        {

            DataTable dt = objPOSTransaction.GetThirdPartyOders(ThirdpartyRecorId, ThirdpartyId);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Alldata = new { JsonString = JsonString, };
            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateDliveryStatusInThirdPartyOrders(string OderNo, string Status, string ThirdpartyRecorId, string ThirdpartyId)
        {
            string statusid = "1";
            string createdby = Convert.ToInt32(Session["UserId"]).ToString();
            string billNo = string.Empty;
            string TAKEAWAYID = string.Empty;
            bool Count = objPOSTransaction.UpdateDliveryStatusInThirdPartyOrders(OderNo, Status, statusid, createdby);
            DataTable dt = objPOSTransaction.GetThirdPartyOders(ThirdpartyRecorId, ThirdpartyId);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Alldata = new { Count = Count, JsonString = JsonString };
            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult GetItemsOfThirdPartySpecicOrder(string orderNo)
        {
            DataTable dt = objPOSTransaction.GetItemsOfThirdPartySpecicOrder(orderNo);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var result = new { DeliveryBoyDetails = JsonString };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult GetThirdPartyOdersWeekWiseAndMonthWise(string ThirdpartyRecorId, string ThirdpartyId, string Wise)
        {

            DataTable dt = objPOSTransaction.GetThirdPartyOdersWeekWiseAndMonthWise(ThirdpartyRecorId, ThirdpartyId, Wise);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Alldata = new { JsonString = JsonString, };
            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult GetThirdpartyNames()
        {

            DataTable dt = objPOSTransaction.GetThirdpartyNames();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Alldata = new { JsonString = JsonString, };
            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetThirdpartyBills(string Date, string ThirdpartyId)
        {

            DataTable dt = objPOSTransaction.GetThirdpartyBills(Date, ThirdpartyId);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Alldata = new { JsonString = JsonString, };
            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult saveThirdPartyBillsettlement(string Third, string ThirdDetails)
        {

            Thirpartysettlement ThirdT = serializer.Deserialize<Thirpartysettlement>(Third);
            ThirdT.statusid = "1";
            ThirdT.createdby = Convert.ToInt32(Session["UserId"]).ToString();
            List<Thirpartysettlement> ThirdDetailsTD = serializer.Deserialize<List<Thirpartysettlement>>(ThirdDetails);

            bool istorf = objPOSTransaction.saveThirdPartyBillsettlement(ThirdT, ThirdDetailsTD);

            var Alldata = new { JsonString = istorf };
            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult getBillToCompanyBills(string CompanyName)
        {
            DataTable dt = objPOSTransaction.getBillToCompanyBills(CompanyName);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var result = new { Billtocompany = JsonString };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult SaveBillTocomanyBills(string BillToCompany, string BillToCompanyDetails)
        {

            Thirpartysettlement ThirdT = serializer.Deserialize<Thirpartysettlement>(BillToCompany);
            ThirdT.statusid = "1";
            ThirdT.createdby = Convert.ToInt32(Session["UserId"]).ToString();
            List<Thirpartysettlement> ThirdDetailsTD = serializer.Deserialize<List<Thirpartysettlement>>(BillToCompanyDetails);

            bool istorf = objPOSTransaction.SaveBillTocomanyBills(ThirdT, ThirdDetailsTD);

            var Alldata = new { JsonString = istorf };
            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetBillToCompanynames()
        {
            DataTable dt = objPOSTransaction.GetBillToCompanynames();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var result = new { Billtocompany = JsonString };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

    }
}