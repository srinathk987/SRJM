using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Newtonsoft.Json;
using RMS.Core;
using RMS.Core.Interfaces;
using RMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace RMS.Controllers
{
    public class POSTransactionController : Controller
    {
        private ITakeAway objPOSTransactionTake = new TakeAwayRepository();
        private IPOSTransaction objPOSTransaction = new POSTransactionRepository();
        private IMMSMasters objMMSMasters = new MMSMastersRepository();
        private IPOSMasters posMasters = new POSMastersRepository();
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        DataTable dt = null;
        string JsonString = "";

        #region Views..

        public ActionResult DashBoard()
        {
            Session["modulename"] = "Home";
            return View();
        }
        public ActionResult KotTaking(string Id)
        {

            return View();
        }
        public ActionResult KotChange(string Id)
        {
            return View();
        }
        public ActionResult KotReprint()
        {
            return View();
        }
        public ActionResult TableChange()
        {
            return View();
        }
        public ActionResult MergeTable()
        {
            return View();
        }
        public ActionResult KotCancel()
        {
            return View();
        }
        public ActionResult BillGeneration()
        {
            return View();
        }

        public ActionResult BillEdit()
        {
            return View();
        }
        public ActionResult BillReprint()
        {
            return View();
        }
        public ActionResult BillSettlement()
        {
            return View();
        }
        public ActionResult ReprintSettledBill()
        {
            return View();
        }
        public ActionResult IssueVoucher()
        {
            return View();
        }

        public ActionResult BillCancel()
        {
            return View();
        }


        #endregion

        #region KOT-CHANGE

        public ActionResult ShowKotChangeDetails(string TableId)
        {
            //  string TableId = getData().Split('-')[1];

            List<KOTTakingDTO> lstKot = objPOSTransaction.ShowKotChangeDetails(TableId);
            return new JsonResult { Data = lstKot, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SaveKotChange(List<KOTTakingDTO> kc, string Delete)
        {
            int count = 0;
            KOTTakingDTO objkottaking = new KOTTakingDTO();
            int statusid = 1;
            int createdby = Convert.ToInt16(Session["UserId"]);
            kc.Insert(0, objkottaking);

            if (objPOSTransaction.SaveKOTChangeDetails(kc, Delete, createdby, statusid))
            {
                count = 1;

            }
            return new JsonResult { Data = count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        #region BILL-SETTLEMENT

        public ActionResult ShowBillNos()
        {
            List<BillSettlementDTO> lst = objPOSTransaction.ShowBillNos();
            return new JsonResult { Data = lst, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult SaveBillSettlement(string BS, CashDetails lstCashDetails, string lstCardDetails)
        {

            List<CardDetails> ListItems = serializer.Deserialize<List<CardDetails>>(lstCardDetails);
            BillSettlementDTO XYZ = serializer.Deserialize<BillSettlementDTO>(BS);
            XYZ.statusid = 1;
            XYZ.createdby = Convert.ToInt64(Session["UserId"]);
            var c = objPOSTransaction.SaveBillSettlement(XYZ, lstCashDetails, ListItems);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }

        public ActionResult GetPendingBillstatus(string TableId)
        {

            string Bills = objPOSTransaction.getStusForBillsPending(TableId);
            var Totalresult = new { Data = Bills };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveCompanyDetails(BillSettlementDTO C)
        {

            C.statusid = 1;
            C.createdby = Convert.ToInt64(Session["UserId"]);
            bool TorF = objPOSTransaction.SaveCompanyDetails(C);
            return new JsonResult { Data = TorF, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }
        public ActionResult getComapnynames()
        {

            DataTable dt = objPOSTransaction.getComapnynamesOfBilltocomapny();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

            var Alldata = new { JsonString = JsonString };
            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        #endregion

        #region Bill Cancel

        public ActionResult showBillno(string TableId)
        {

            dt = new DataTable();
            dt = objPOSTransaction.GetBillComboData(TableId);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);

        }
        public ActionResult getcancelBills(string TableId)
        {

            dt = new DataTable();
            dt = objPOSTransaction.getBillDetals(TableId);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);

        }
        public ActionResult CreateDeletedBills(string JsonS)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            dt = new DataTable();
            bool trf = objPOSTransaction.SaveBillSDeleted(JsonS, createdby);

            var Totalresult = new { Data = trf };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);

        }

        #endregion

        # region BillGeneration


        public ActionResult ShowkotDetails(int TableId)
        {
            Core.BillGeneration objBillGeneration = new Core.BillGeneration();
            List<BillGeneration> lstBillGeneration = new List<BillGeneration>();
            //string TableData = getData();
            //if (TableData != string.Empty && TableData != "")
            //    objBillGeneration.tableid = Convert.ToInt64(TableData.Split('-')[1]);
            //else
            objBillGeneration.tableid = TableId;
            lstBillGeneration = objPOSTransaction.ShowkotDetails(objBillGeneration);



            List<BillGeneration> lstvoucher = new List<BillGeneration>();
            lstvoucher = objPOSTransaction.ShowBillVouchers();

            List<BillEditDTO> lstBillEditDTO1 = new List<BillEditDTO>();
            lstBillEditDTO1 = objPOSTransaction.ShowReasons();

            List<BillGeneration> lstservicetax = new List<BillGeneration>();
            lstservicetax = objPOSTransaction.ShowServicetax();

            List<BillGeneration> lstservicecharge = new List<BillGeneration>();
            lstservicecharge = objPOSTransaction.ShowServiceCharge();

            //string billdate = System.DateTime.Now.Date.ToString();
            //string billtime = System.DateTime.Now.TimeOfDay.ToString();
            //objBillGeneration.billdate = billdate;
            //objBillGeneration.billtime = "15";

            var Alldata = new { griddetails = lstBillGeneration, Details = objBillGeneration, VoucherData = lstvoucher, Reasondata = lstBillEditDTO1, servicetax = lstservicetax, servicecharge = lstservicecharge };

            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult SaveBillGeneration(string objBillGeneration, string lstKotDetails)
        {
            int Count = 0;
            BillGeneration fgdfg = Newtonsoft.Json.JsonConvert.DeserializeObject<BillGeneration>(objBillGeneration);
            List<BillGeneration> dgdg = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BillGeneration>>(lstKotDetails);
            fgdfg.statusid = 1;

            fgdfg.createdby = Convert.ToInt64(Session["UserId"]);
            if (objPOSTransaction.SaveBillGeneration(fgdfg, dgdg))
            {

                Count = 1;

            }

            var Alldata = new { status = Count, billno = fgdfg.billno };

            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult BillGenerationReport(string billno)
        {
            IPOSReports posTransactions = new POSReportsRepository();
            System.Data.DataSet ds = posTransactions.BillGenerationReport(billno);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            string Hostname = "";
            string session = "";
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/rptBillGeneration.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("TableId", ds.Tables[0].Rows[0]["tableid"]);
            rptH.SetParameterValue("BillNo", ds.Tables[0].Rows[0]["vchbillno"]);
            rptH.SetParameterValue("HostName", Hostname);
            rptH.SetParameterValue("SessionNo", session);
            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #endregion

        #region BILL EDIT



        public ActionResult ShowReasons()
        {
            List<BillEditDTO> lstReasons = objPOSTransaction.ShowReasons();
            return new JsonResult { Data = lstReasons, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //public ActionResult ShowVouchers()
        //{
        //    List<BillEditDTO> lstVouchers = objPOSTransaction.ShowVouchers();
        //    return new JsonResult { Data = lstVouchers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        public ActionResult ShowVoucherDetails(string voucherid)
        {
            BillEditDTO lstVoucherDetails = new BillEditDTO();
            if (voucherid != null && voucherid != "" && voucherid != "0")

                lstVoucherDetails = objPOSTransaction.ShowVoucherDetails(voucherid);
            return new JsonResult { Data = lstVoucherDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ShowBillNosBillEdit()
        {
            List<BillEditDTO> lstBillNos = objPOSTransaction.ShowBillNosBillEdit();

            return new JsonResult { Data = lstBillNos, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ShowServicetax()
        {
            List<BillEditDTO> lstServiceTax = objPOSTransaction.ShowBillEditServicetax();

            return new JsonResult { Data = lstServiceTax, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ShowServiceCharge()
        {
            List<BillEditDTO> lstServiceCharge = objPOSTransaction.ShowBillEditServiceCharge();

            return new JsonResult { Data = lstServiceCharge, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public ActionResult ShowBillDetails(string Billid)
        {
            List<BillEditDTO> lstBillDetails = objPOSTransaction.ShowBillDetails(Billid);
            List<BillEditDTO> lstVouchers = objPOSTransaction.ShowVouchers(Billid);

            var Alldata = new { BillDetails = lstBillDetails, Vouchers = lstVouchers };

            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SaveBillEdit(BillEditDTO objBillEdit, List<BillEditDTO> lstKotDetails)
        {
            bool isValid = false;

            try
            {
                objBillEdit.statusid = 1;
                objBillEdit.createdby = Convert.ToInt64(Session["UserId"]);
                isValid = objPOSTransaction.SaveBillEdit(objBillEdit, lstKotDetails);

                var result = new { Success = isValid };
                var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion




        public ActionResult ShowAlltablesbind()
        {


            List<DashboardDTO> lstAvailabletablesdata = objPOSTransaction.ShowAlltablesformodel();
            ViewData["tables"] = lstAvailabletablesdata;
            var Totalresult = new { lstAvailabletablesdata = lstAvailabletablesdata };
            return new JsonResult { Data = Totalresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }






        public ActionResult ShowAlltables()
        {
            ILogin objlogin = new LoginRepository();
            List<Login> lstChildModules = new List<Login>();
            int userid = Convert.ToInt32(Session["UserId"]);
            lstChildModules = objlogin.GetChildFunctionNames(userid, "", "RMSDASHBOARD");
            List<DashboardDTO> lstAvailabletable = objPOSTransaction.ShowAlltables(userid);
            ViewData["tables"] = lstAvailabletable;
            DashboardDTO KotsCount = objPOSTransaction.ShowKOtsCount(userid);

            #region Charts
            List<DashboardDTO> chartlist = objPOSTransaction.ChartListForCategorywiseToday(userid);
            List<DashboardDTO> ChartListForSessionwise = objPOSTransaction.ChartListForCategorywiseWeek(userid);
            List<DashboardDTO> ChartListForSectionwise = objPOSTransaction.ChartListForCategorywiseMonth(userid);

            #endregion
            var Totalresult = new { lstAvailabletable = lstAvailabletable, KotsCount = KotsCount, chartsdata = chartlist, ChildNames = lstChildModules, username = Session["UserName"], ChartListForSessionwise = ChartListForSessionwise, ChartListForSectionwise = ChartListForSectionwise };
            return new JsonResult { Data = Totalresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }


        #region srinath
        public ActionResult ShowCategoryPeriodwise(string Period)
        {
            int userid = Convert.ToInt32(Session["UserId"]);

            List<DashboardDTO> chartlistForCategoryPeriod = objPOSTransaction.ChartListForCategorywisePeriod(userid, Period);

            var Totalresult = new { chartlistForCategoryPeriod = chartlistForCategoryPeriod };
            return new JsonResult { Data = Totalresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        public ActionResult ShowSessionwisePeriod(string Period)
        {
            int userid = Convert.ToInt32(Session["UserId"]);

            List<DashboardDTO> chartlistForSessionwisePeriod = objPOSTransaction.ChartListForSessionwisePeriod(userid, Period);

            var Totalresult = new { chartlistForSessionwisePeriod = chartlistForSessionwisePeriod };
            return new JsonResult { Data = Totalresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public ActionResult ShowSectionwisePeriod(string Period)
        {
            int userid = Convert.ToInt32(Session["UserId"]);

            List<DashboardDTO> chartlistForSectionwisePeriod = objPOSTransaction.ChartListForSectionwisePeriod(userid, Period);

            var Totalresult = new { chartlistForSectionwisePeriod = chartlistForSectionwisePeriod };
            return new JsonResult { Data = Totalresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion
        //public ActionResult ShowAlltables()
        //{
        //    ILogin objlogin = new LoginRepository();
        //    List<Login> lstChildModules = new List<Login>();
        //    int userid = Convert.ToInt32(Session["UserId"]);
        //    lstChildModules = objlogin.GetChildFunctionNames(userid, "", "RMSDASHBOARD");
        //    List<DashboardDTO> lstAvailabletable = objPOSTransaction.ShowAlltables(userid);
        //    ViewData["tables"] = lstAvailabletable;
        //    DashboardDTO KotsCount = objPOSTransaction.ShowKOtsCount(userid);

        //    #region Charts
        //    List<DashboardDTO> chartlist = objPOSTransaction.ChartListForCategorywise(userid);
        //    List<DashboardDTO> ChartListForSessionwise = objPOSTransaction.ChartListForSessionwise(userid);
        //    List<DashboardDTO> ChartListForSectionwise = objPOSTransaction.ChartListForSectionwise(userid);

        //    #endregion
        //    var Totalresult = new { lstAvailabletable = lstAvailabletable, KotsCount = KotsCount, chartsdata = chartlist, ChildNames = lstChildModules, username = Session["UserName"], ChartListForSessionwise = ChartListForSessionwise, ChartListForSectionwise = ChartListForSectionwise };
        //    return new JsonResult { Data = Totalresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        //}
        public ActionResult CheckBillStatus(string tableid)
        {
            //Session["TableName"] = Table;
            string billstatus = objPOSTransaction.CheckBillStatus(tableid);
            return new JsonResult { Data = billstatus, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public void PassData(string Table)
        {
            Session["TableName"] = Table;
        }

        public void ParentModuleData(string ParentName)
        {
            // Session["ParentModuleName"] = ParentName;
            TempData["ParentModuleName"] = ParentName;

        }

        public string getParentModuleData()
        {
            var ParentName = "";

            //if (Session["ParentModuleName"] != null)
            //{
            //    ParentName = Session["ParentModuleName"].ToString();
            //}
            if (TempData["ParentModuleName"] != null)
            {
                ParentName = TempData["ParentModuleName"].ToString();
                TempData.Keep("ParentModuleName");
            }

            return ParentName.ToString();

        }
        public string getData()
        {
            var Table1 = "";


            if (Session["TableName"] != null)
            {
                Table1 = Session["TableName"].ToString();
                // Session.Remove("TableName");

            }

            return Table1.ToString();

        }




        #region KOT Cancel//

        public ActionResult ShowKot()
        {
            //List<KOTCancel> lstKot = posMasters.ShowKot("1");
            return new JsonResult { Data = string.Empty, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ShowReason()
        {
            List<KOTCancel> lstKot = objPOSTransaction.ShowReason();
            return new JsonResult { Data = lstKot, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ShowKotCancelGridData(string TableNO, string KotId)
        {
            List<KOTCancel> lstKot = objPOSTransaction.ShowKotCancelGridData(TableNO, KotId);
            return new JsonResult { Data = lstKot, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public ActionResult ShowKot(string Id)
        {
            List<KOTCancel> lstKot = objPOSTransaction.ShowKot(Id);
            return new JsonResult { Data = lstKot, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult SaveKotCancel(string jsonData, string UserID)
        {
            List<KOTCancel> UserList = JsonConvert.DeserializeObject<List<KOTCancel>>(jsonData);
            KOTCancel kot = JsonConvert.DeserializeObject<KOTCancel>(UserID);
            kot.statusid = 1;
            kot.createdby = Convert.ToInt64(Session["UserId"]);
            UserList.Insert(0, kot);
            bool status = objPOSTransaction.SaveKotCancel(UserList);
            return new JsonResult { Data = status, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        #region Issue Voucher

        public ActionResult ShowUserNames()
        {
            List<UserMaster> lstUsers = objPOSTransaction.ShowUserNames();
            List<CutomerMaster> lstCustomers = objPOSTransaction.ShowCutomerNames();
            List<Vouchertype> lstVouchertype = objPOSTransaction.ShowVoucherTypes();
            var result = new { User = lstUsers, Customer = lstCustomers, Voucher = lstVouchertype };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ShowVouchers(string VouchName, string VouchID)
        {
            List<Vouchertype> lstVouchers = objPOSTransaction.ShowVouchers(VouchName, VouchID);
            return new JsonResult { Data = lstVouchers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult SaveIssueVoucher(IssueVoucher ISV)
        {
            ISV.statusid = 1;
            ISV.createdby = Convert.ToInt64(Session["UserId"]);
            bool str = objPOSTransaction.SaveIssueVoucher(ISV);
            return new JsonResult { Data = str, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region table Change



        public JsonResult ShowTableData(string TableId)
        {
            KOTTablesDTO lstTableChange = objPOSTransaction.ShowFromTableNoDetails(TableId);
            return new JsonResult { Data = lstTableChange, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult ShowTableDetails(string Table)
        {
            List<KOTTablesDTO> lstTableChange = objPOSTransaction.ShowToTableNoDetails(Table);
            List<KOTTablesDTO> lstTablesandMerge = objPOSTransaction.ShowToMergeTableNoDetails(Table);
            var result = new { lstTableChange = lstTableChange, lstTablesandMerge = lstTablesandMerge };

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public JsonResult ShowTableDetailsrunning(string Table)
        {
            List<KOTTablesDTO> lstTableChange = objPOSTransaction.ShowToTableNoDetailsRunning(Table);
            List<KOTTablesDTO> lstTablesandMerge = objPOSTransaction.ShowToMergeTableNoDetails(Table);
            var result = new { lstTableChange = lstTableChange, lstTablesandMerge = lstTablesandMerge };

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }






        public JsonResult ShowTableDetailsrunningKotTransfer(string Table)
        {
            List<KOTTablesDTO> lstTableChange = objPOSTransaction.ShowToTableNoDetailsRunningKotTransfer(Table);
            List<KOTTablesDTO> lstTablesandMerge = objPOSTransaction.ShowToMergeTableNoDetailsKotTransfer(Table);
            var result = new { lstTableChange = lstTableChange, lstTablesandMerge = lstTablesandMerge };

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }





        //to table change details     Runnnning
        //public JsonResult ToTableChange(TablesDTO tab)
        //{
        //    List<TablesDTO> lsttoTableChange = posMasters.ShowToTableNoDetails(tab);
        //    return new JsonResult { Data = lsttoTableChange, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}


        //frototable details saving
        public ActionResult SaveFroToTableDetails(KOTTablesDTO tab, string tableclick)
        {
            tab.statusid = 1;
            tab.createdby = Convert.ToInt64(Session["UserId"]);
            var cnt = objPOSTransaction.SaveFromToTableDetails(tab, tab.createdby, tableclick);
            // var res = new { cnt = cnt, Msg = "Saves" };
            return new JsonResult { Data = cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        #region Merge Tables

        public ActionResult getTableNoS(string TableId)
        {

            List<MergingTables> Avaialabletables = objPOSTransaction.GetTables(TableId, Convert.ToInt32(Session["UserId"]));
            // List<MergingTables> MergedTables = objPOSTransaction.GetTables(TableId);
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(Avaialabletables);
            var res = new { jsonString = jsonString };
            return new JsonResult { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult getKotNo(string JsonData)
        {
            DataTable dt = new DataTable();
            dt = objPOSTransaction.GetkotNo(JsonData);
            string kotID = "";
            string kot = "";
            var DD = "";
            var dd = "";
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    kotID = kotID + "," + dr["vchkotid"].ToString();
                }
                dd = kotID;
                dd = kotID.Substring(1);
                foreach (DataRow dr in dt.Rows)
                {
                    kot = kot + "," + dr["kotid"].ToString();
                }

                DD = kot.Substring(1);
            }
            var res = new { KOT = dd, DD = DD };
            return new JsonResult { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult CreateMerge(string TableId, string kotdatatosave)
        {

            Mergetables objAttendenceDetails = new Mergetables();
            List<RMS.Core.Mergetables> myDeserializedObjList = (List<RMS.Core.Mergetables>)Newtonsoft.Json.JsonConvert.DeserializeObject(kotdatatosave, typeof(List<RMS.Core.Mergetables>));
            myDeserializedObjList[0].createdby = Convert.ToInt64(Session["UserId"]);
            bool KotMergeresult = objPOSTransaction.SaveMerge(TableId, myDeserializedObjList);

            return new JsonResult { Data = KotMergeresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        #region KotTaking//

        public ActionResult ShowSessionForKOTtaking(string TName)
        {
            // string tablename = getData();

            string tablename = TName;

            int userid = Convert.ToInt32(this.Session["UserId"]);
            string Session = objPOSTransaction.ShowSessionForKOTtaking();
            List<ItemsDTO> lstItems = objPOSTransaction.ShowItemsForSession();
            List<KOTTakingDTO> lstSessionDetails = objPOSTransaction.ShowSessionDetailsForSession(tablename, userid);
            //List<KotAvailabletables> lstAvailabletable = objPOSTransaction.ShowAvailabletables(Convert.ToString(lstSessionDetails[0].TableNo), tablename);
            //List<Mergetables> lstMergetables = objPOSTransaction.ShowMergetables(Convert.ToString(lstSessionDetails[0].TableNo));
            List<ItemsDTO> CompletedOrderItem = objPOSTransaction.ShowPendingKOT(Convert.ToString(lstSessionDetails[0].TableNo));
            //  string JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lstAvailabletable);
            var result = new { Session = Session, lstItems = lstItems, lstSessionDetails = lstSessionDetails, CompletedOrderItem = CompletedOrderItem };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        /// <summary>
        /// This Method is used to show available tables and merged tables
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public ActionResult ShowAvailabletables(string Id, string TableName)
        {
            List<MergingTables> lstAvailabletable = objPOSTransaction.ShowAvailabletables(Id, TableName);
            List<KotAvailabletables> lstMergetables = objPOSTransaction.ShowMergetables(TableName);
            string JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lstAvailabletable);
            var Totalresult = new { Data = JsonString, lstMergetables = lstMergetables };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Saving KOT Details
        /// </summary>
        /// <param name="KOTDetails"></param>
        /// <param name="ItemDetails"></param>
        /// <param name="Mergetables"></param>
        /// <returns></returns>
        public ActionResult CreateKOT(KOTTakingDTO KOTDetails, List<ItemsDTO> ItemDetails, List<Mergetables> Mergetables)
        {
            KOTDetails.statusid = 1;
            KOTDetails.createdby = Convert.ToInt32(Session["UserId"]);
            int Count = objPOSTransaction.CreateKOT(KOTDetails, ItemDetails, Mergetables);
            var Alldata = new { Count = Count, vchKotId = KOTDetails.vchKotId };
            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };



        }

        public ActionResult Kotslip(string kotno)
        {

            DataSet ds = objPOSTransaction.Kotslip(kotno);
            //DataSet ds1 = payRollTranscactions.companyname();
            //string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            //string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            //string reportPath = Path.Combine(Server.MapPath("~/Reports"), "PayslipReport.rpt");
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/KotiSlipReport.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", "KAPIL ENGINEERS AND INFRASTRUCTURES PVT LTD");
            rptH.SetParameterValue("Address", "SY NO 115/1,Kapil Towers,15th Floor,Nanakramguda");
            //rptH.SetParameterValue("CompanyName", strcompanyname);
            //rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("Month", "kotno");
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        #endregion

        #region KOT REPRINT

        public ActionResult ShowKots(string tablid)
        {
            List<KOTTakingDTO> lstKot = objPOSTransaction.ShowReprintKot(tablid);

            return new JsonResult { Data = lstKot, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SaveKotReprint(KOTTakingDTO objKotReprint)
        {
            bool isValid = false;

            try
            {
                isValid = objPOSTransaction.SaveKotReprint(objKotReprint);
                var result = new { Success = isValid };
                var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion


        #region KOT TRANSFER


        public ActionResult KotTransfer()
        {
            return View();
        }


        public ActionResult ShowKotTransferDetails(string TableId)
        {
            //  string TableId = getData().Split('-')[1];
            List<KOTTakingDTO> lstKottransfer = objPOSTransaction.ShowKotTransferDetails(TableId);
            return new JsonResult { Data = lstKottransfer, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public ActionResult ShowKotDetailsbyTableid(string TableId)
        {
            //  string TableId = getData().Split('-')[1];
            List<KOTTakingDTO> lstKottransfer = objPOSTransaction.ShowKotDetailsBytableid(TableId);
            return new JsonResult { Data = lstKottransfer, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public JsonResult KOTTransfereDetailsList(string KotTransferDto, string TablesId, string date, string time, string hostname, int tttablesid)
        {
            bool isValid = false;

            try
            {
                isValid = objPOSTransaction.KOTTransfereDetails(KotTransferDto, TablesId, date, time, hostname, tttablesid);
                var result = new { Success = isValid };
                var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }






        public JsonResult GetCashierComboData()
        {
            // IPOSReports posTransactions = new POSReportsRepository();
            dt = new DataTable();


            int userid = Convert.ToInt32(this.Session["UserId"]);

            dt = objPOSTransaction.GetCashierComboData();
            // dt = objPOSTransaction.GetCashierComboData(userid);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCashierComboDataBinding(string tableidm)
        {
            // IPOSReports posTransactions = new POSReportsRepository();
            dt = new DataTable();


            // int userid = Convert.ToInt32(this.Session["UserId"]);

            dt = objPOSTransaction.GetCashierComboDataBind(tableidm);
            // dt = objPOSTransaction.GetCashierComboData(userid);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult KOTTransfereDetailsList(string KotTransferDto, string TablesId, string date, string time, string hostname)
        //{
        //    bool isValid = false;

        //    try
        //    {



        //        isValid = objPOSTransaction.KOTTransfereDetails(KotTransferDto, TablesId, date, time, hostname);

        //        var result = new { Success = isValid };
        //        var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //        return data;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        #endregion




        #region changedashboard


        public JsonResult SavetableDetails(string TableName)
        {
            bool isValid = false;

            try
            {

                int createdby = Convert.ToInt16(Session["UserId"]);
                isValid = objPOSTransaction.SaveTableDetails(TableName, createdby);
                var result = new { Success = isValid };
                var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public JsonResult UpdatetableDetails()
        {
            bool isValid = false;

            try
            {

                int createdby = Convert.ToInt16(Session["UserId"]);
                isValid = objPOSTransaction.updateTableDetails(createdby);
                var result = new { Success = isValid };
                var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }



        #endregion



        #region Old Code Indent Details By Vikram


        //public ActionResult ShowIndentProducts()
        //{
        //    List<MaterialIndentDTO> lstproduct = objPOSTransaction.ShowIndentProducts();
        //    List<MaterialIndentDTO> lststorage = objPOSTransaction.ShowStorage();
        //    List<MaterialIndentDTO> lstrequesteby = objPOSTransaction.showrequestedby();
        //    List<MaterialIndentDTO> lstdeportments = objPOSTransaction.ShowDepartment();

        //    //  List<MaterialIndentDTO> lstshowuoms = objPOSTransaction.ShowUOM1();
        //    var bindingdata = new { products = lstproduct, storages = lststorage, Requestedby = lstrequesteby, deportments = lstdeportments };
        //    return new JsonResult { Data = bindingdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}
        //public ActionResult GetSelfNames(String showroomselected)
        //{
        //    if (showroomselected != string.Empty)
        //    {
        //        showroomselected = showroomselected.Split(':')[1].ToString();
        //    }

        //    List<MaterialIndentDTO> lstselfdetails = objPOSTransaction.GetSelfdetails(showroomselected);
        //    var selfdata = new { selfnames = lstselfdetails };
        //    return new JsonResult { Data = selfdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        //public JsonResult SaveIndentDetails(List<MaterialIndentDTO> griddata, MaterialIndentDTO BE)
        //{

        //    bool issaved = false;

        //    try
        //    {
        //        int userid = Convert.ToInt32(this.Session["UserId"]);
        //        BE.createdby = userid;


        //        issaved = objPOSTransaction.SaveIndent(griddata, BE);
        //        var result = new { Success = issaved };
        //        var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public ActionResult GetExistedIndenNo(string IndentType)
        //{
        //    try
        //    {

        //        List<MaterialIndentDTO> lstIndentTypeDetails = objPOSTransaction.GetExistingIndentNo(IndentType);
        //        var Indentdata = new { Indentdetails = lstIndentTypeDetails };
        //        return new JsonResult { Data = Indentdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public ActionResult GetExistedIndents(string Indents)
        //{
        //    try
        //    {


        //        List<MaterialIndentDTO> lstIndentts = objPOSTransaction.GetExistingIndents(Indents);
        //        var Indentss = new { Indents = lstIndentts };

        //        return new JsonResult { Data = Indentss, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //public ActionResult GetProductAvailability(string productid, string productname)
        //{
        //    try
        //    {
        //        List<MaterialIndentDTO> lstproductavailble = objPOSTransaction.GetProductAvailability(productid, productname);
        //        var varavailability = new { productavailability = lstproductavailble };
        //        return new JsonResult { Data = varavailability, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        #endregion

        public ActionResult SaveThirPartyVendor(string Third)
        {
            ThirdParty ThirdDetails = serializer.Deserialize<ThirdParty>(Third);



            //// Convert Base64 String to byte[]
            //byte[] imageBytes = Convert.FromBase64String(ThirdDetails.VendorImage);
            //MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

            //// Convert byte[] to Image
            //ms.Write(imageBytes, 0, imageBytes.Length);
            //System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            //Guid id = Guid.NewGuid();
            //string Newid = id.ToString().Replace("'", "");
            //image.Save(Server.MapPath("~/assets/Css/images/thirdparty-vendors/'" + Newid + "'.png"), System.Drawing.Imaging.ImageFormat.Png);
            //ThirdDetails.VendorImage = Newid;
            string Message, fileName, actualFileName;
            Message = fileName = actualFileName = string.Empty;
            bool flag = false;
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {

                if (Request.Files != null)
                {
                    Console.WriteLine(file);
                    actualFileName = file.FileName;
                    fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    //int size = file.ContentLength;

                    file.SaveAs(Path.Combine(Server.MapPath("~/thirdparty-vendors"), fileName));




                }
            }
            else
            {

                Console.WriteLine("Nag");

            }
            ThirdDetails.VendorImage = fileName;
            ThirdDetails.statusid = 1;
            ThirdDetails.createdby = Convert.ToInt32(Session["UserId"]);
            bool Count = objPOSTransactionTake.SaveThirdPartyVendor(ThirdDetails);
            DataTable dt = objPOSTransactionTake.getImagesOfThirParty();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

            var Alldata = new { Count = Count, JsonString = JsonString };
            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult ShowThirPartyVendorImages()
        {

            DataTable dt = objPOSTransactionTake.getImagesOfThirParty();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

            var Alldata = new { JsonString = JsonString };
            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


    }
}