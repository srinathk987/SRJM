using RMS.Core;
using RMS.Core.Interfaces;
using RMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RMS.Controllers
{

    public class POSMastersController : Controller
    {


        private IPOSMasters posMasters = new POSMastersRepository();
        DataTable dt = null;
        string JsonString = string.Empty;

        #region Views...
        public ActionResult Origin()
        {
            return View();
        }
        public ActionResult ItemCategory()
        {
            return View();
        }
        public ActionResult ItemSubCategory()
        {
            return View();
        }
        public ActionResult ItemMaster()
        {
            return View();
        }
        public ActionResult SectionMaster()
        {
            return View();
        }
        public ActionResult TableandCovers()
        {
            return View();
        }
        public ActionResult SessionMaster()
        {
            return View();
        }
        public ActionResult Reasons()
        {
            return View();

        }

        public ActionResult Voucher()
        {
            return View();
        }
        public ActionResult VoucherType()
        {
            return View();
        }
        public ActionResult Department()
        {
            return View();
        }
        public ActionResult PaymentType()
        {
            return View();
        }
        public ActionResult Printer()
        {
            return View();

        }
        public ActionResult TaxDetails()
        {
            return View();

        }

        #endregion


        #region IteamCategory

        public ActionResult ShowItemCategory()
        {
            List<ItemCategoryDTO> lstTax = posMasters.ShowItemCategory();
            return new JsonResult { Data = lstTax, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SaveItemCategory(ItemCategoryDTO Ic)
        {
            Ic.statusid = 1;
            Ic.createdby = Convert.ToInt64(Session["UserId"]);
            int isSaved = posMasters.SaveItemCategory(Ic);
            return new JsonResult { Data = isSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult DeleteItemCategory(ItemCategoryDTO Ic)
        {
            var c = posMasters.DeleteItemCategory(Ic.CategoryId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateItemCategory(ItemCategoryDTO Ic)
        {
            int c = posMasters.UpdateItemCategory(Ic);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        #endregion

        #region ItemSubCategoryMaster

        public ActionResult ShowItemSubCategory()
        {
            List<ItemSubCategoryDTO> lstTax = posMasters.ShowItemSubCategory();
            return new JsonResult { Data = lstTax, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SaveItemSubCategory(ItemSubCategoryDTO Isc)
        {
            Isc.statusid = 1;
            Isc.createdby = Convert.ToInt64(Session["UserId"]);
            int isSaved = posMasters.SaveItemSubCategory(Isc);
            return new JsonResult { Data = isSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateItemSubCategory(ItemSubCategoryDTO Isc)
        {
            var c = posMasters.UpdateItemSubCategory(Isc);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeleteItemSubCategory(ItemSubCategoryDTO Isc)
        {
            bool c = posMasters.DeleteItemSubCategory(Isc.SubCategoryId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }




        #endregion


        #region Voucher...
        public ActionResult ShowVoucher()
        {
            List<VoucherDTO> lstVoucher = posMasters.ShowVoucher();
            return new JsonResult { Data = lstVoucher, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SaveVoucher(VoucherDTO vhr)
        {
            vhr.statusid = 1;
            vhr.createdby = Convert.ToInt64(Session["UserId"]);
            if (string.IsNullOrEmpty(vhr.ValidPeriod))
            {
                vhr.ValidPeriod = "0";
            }
            if (string.IsNullOrEmpty(vhr.ValidUpto))
            {
                vhr.ValidUpto = "01-01-0001";
            }

            var c = posMasters.SaveVoucher(vhr);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult aDDe156(DTO1 Ic)
        {
            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateVoucher(VoucherDTO vhr)
        {
            var c = posMasters.UpdateVoucher(vhr);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public ActionResult DeleteVoucher(VoucherDTO Vch)
        {
            var c = posMasters.DeleteVoucher(Vch.VoucherId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region DEPARTMENT...

        public ActionResult ShowDepartment()
        {
            List<DepartmentDTO> lstDepartment = posMasters.ShowDepartment();
            return new JsonResult { Data = lstDepartment, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SaveDepartment(DepartmentDTO Dept)
        {
            Dept.statusid = 1;
            Dept.createdby = Convert.ToInt64(Session["UserId"]);
            int Count = posMasters.SaveDepartment(Dept);
            return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeleteDepartment(DepartmentDTO Dept)
        {
            var c = posMasters.DeleteDepartment(Dept.RecordId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateDepartment(DepartmentDTO Dept)
        {
            int Cnt = posMasters.UpdateDepartment(Dept);
            return new JsonResult { Data = Cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        #endregion

        #region Printers...

        public ActionResult ShowPrinters()
        {
            List<PrinterDTO> lstDepartment = posMasters.ShowPrinter();
            return new JsonResult { Data = lstDepartment, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SavePrinter(PrinterDTO Prnt)
        {
            Prnt.statusid = 1;
            Prnt.createdby = Convert.ToInt64(Session["UserId"]);
            int Cnt = posMasters.SavePrinter(Prnt);
            return new JsonResult { Data = Cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeletePrinter(PrinterDTO Prnt)
        {
            var c = posMasters.DeletePrinter(Prnt.PrinterId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdatePrinter(PrinterDTO Prnt)
        {
            int cnt = posMasters.UpdatePrinter(Prnt);
            return new JsonResult { Data = cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        #endregion

        #region Tax...

        public ActionResult ShowTaxDetails()
        {
            List<TaxDTO> lstTax = posMasters.ShowTaxDetails();
            return new JsonResult { Data = lstTax, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SaveTax(TaxDTO Tx)
        {
            Tx.statusid = 1;
            Tx.createdby = Convert.ToInt64(Session["UserId"]);
            int Count = posMasters.SaveTax(Tx);

            return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeleteTax(int id, string name)
        {
            var c = posMasters.DeleteTax(id, name);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateTax(TaxDTO Tx)
        {
            int Count = posMasters.UpdateTax(Tx);
            return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public ActionResult CountUpdateTax(string name)
        {
            int Count = posMasters.CountUpdateTax(name);
            return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }


        #endregion




        #region Section...

        public ActionResult ShowSection()
        {
            List<SectionDTO> lstTax = posMasters.ShowSection();
            return new JsonResult { Data = lstTax, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //public ActionResult SaveItemCategory1(VoucherDTO vhr)
        //{
        //    return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}
        public JsonResult SaveItemCategory1(string JsonData)
        {
            //sec.statusid = 1;
            string createdby = Session["UserId"].ToString();
            int cnt = posMasters.SaveSection(JsonData, createdby);
            return new JsonResult { Data = cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeleteSection(int Id)
        {
            var c = posMasters.DeleteSection(Id);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return null;
        }
        public ActionResult UpdateSection(string JsonData)
        {
            string createdby = Session["UserId"].ToString();
            var c = posMasters.UpdateSection(JsonData, createdby);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        #endregion

        #region Tables...

        public ActionResult ShowTables()
        {
            List<TablesDTO> lstDepartment = posMasters.ShowTableandCovers();
            return new JsonResult { Data = lstDepartment, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SaveTable(TablesDTO tab)
        {
            tab.statusid = 1;
            tab.createdby = Convert.ToInt64(Session["UserId"]);
            var cnt = posMasters.SaveTableandCovers(tab);
            return new JsonResult { Data = cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeleteTable(TablesDTO tab)
        {
            var c = posMasters.DeleteTableandCovers(tab.TableId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateTableandCovers(TablesDTO tab)
        {
            var cnt = posMasters.UpdateTableandCovers(tab);
            return new JsonResult { Data = cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public ActionResult checkStatustoDelete(int TableId)
        {
            string Status = posMasters.checkStatustoDelete(TableId);
            return new JsonResult { Data = Status, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        #endregion


        #region origion

        public JsonResult CreateOrigin(string JsonData)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            int result = posMasters.SaveOrigin(JsonData, createdby);
            string JsonString = "";
            if (result == 0)
            {
                dt = new DataTable();
                dt = posMasters.ShowOrigin();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }

            var Totalresult = new { Data = JsonString, TorF = result };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ShowOrigin()
        {
            dt = new DataTable();
            dt = posMasters.ShowOrigin();
            var JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateOrigin(string JsonData)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            dt = new DataTable();
            int TorF = posMasters.UpdateOrigin(JsonData, createdby);
            if (TorF == 0)
            {
                dt = posMasters.ShowOrigin();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            var Totalresult = new { Data = JsonString, TorF = TorF };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);

        }

        public JsonResult DeleteOrigin(string Id)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            dt = new DataTable();
            bool TorF = posMasters.DeleteOrigin(Id, createdby);
            if (TorF == true)
            {
                dt = posMasters.ShowOrigin();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            var Totalresult = new { Data = JsonString, TorF = TorF };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Session

        public ActionResult GetTreeData(string btnPress, string SessionId)
        {
            DataTable dt = new DataTable();
            if (btnPress == "Save")
            {
                dt = posMasters.getTreeViewData();
            }
            if (btnPress == "Update")
            {
                dt = posMasters.getTreeBtnUpdateViewData(SessionId);
            }
            if (btnPress == "DaySpecial")
            {
                dt = posMasters.getTreeViewDataDayspecial(SessionId);
            }
            var JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            DataTable dt1 = new DataTable();
            int lengthofCate = 0;
            string nameslist = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                nameslist += dr["categoryname"].ToString() + ",";
            }

            nameslist = RemoveDuplicateWords(nameslist);
            nameslist = nameslist.TrimEnd(',');
            string[] categotyitems = nameslist.Split(',');
            lengthofCate = categotyitems.Length;

            var Totalresult = new { Data = JsonString, categotyitems = categotyitems, lengthofCate = lengthofCate };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);


        }

        public string RemoveDuplicateWords(string v)
        {
            var d = new Dictionary<string, bool>();
            StringBuilder b = new StringBuilder();
            string[] a = v.Split(new char[] { ',', ';', '.' },
            StringSplitOptions.RemoveEmptyEntries);

            foreach (string current in a)
            {


                string lower = current.ToLower();


                if (!d.ContainsKey(lower))
                {
                    b.Append(current + ",").Append(' ');
                    d.Add(lower, true);
                }
            }
            return b.ToString().Trim();
        }

        public ActionResult CreateSession(string JsonData, string JsonDatatab1)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            SessionDTO objAttendenceDetails = new SessionDTO();
            List<RMS.Core.SessionDTO> myDeserializedObjList = (List<RMS.Core.SessionDTO>)Newtonsoft.Json.JsonConvert.DeserializeObject(JsonData, typeof(List<RMS.Core.SessionDTO>));
            bool TrF = posMasters.SaveSession(myDeserializedObjList, JsonDatatab1, createdby);
            dt = posMasters.GetSessionData();
            var JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString, TrF = TrF };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateSession(string JsonData, string JsonDatatab1, string SId)//UpdateDaySpecial
        {

            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            SessionDTO objAttendenceDetails = new SessionDTO();
            List<RMS.Core.SessionDTO> myDeserializedObjList = (List<RMS.Core.SessionDTO>)Newtonsoft.Json.JsonConvert.DeserializeObject(JsonData, typeof(List<RMS.Core.SessionDTO>));

            bool TrF = posMasters.UpdateSession(myDeserializedObjList, JsonDatatab1, SId, createdby);
            dt = posMasters.GetSessionData();
            var JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString, TrF = TrF };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowSession()
        {
            DataTable dt = new DataTable();
            dt = posMasters.GetSessionData();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SessionExist(int From, int To, string SessionId, string Text)
        {
            int count = posMasters.GetSessionCount(From, To, SessionId, Text);
            var Totalresult = new { Data = count };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteSession(string SessionId)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            DataTable dt = new DataTable();
            bool TrF = posMasters.DeleteSessionData(SessionId, createdby);
            if (TrF == true)
            {
                dt = posMasters.GetSessionData();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }

            var Totalresult = new { Data = JsonString, TrF = TrF };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Item Master



        public JsonResult GetOriginComboData()
        {
            dt = new DataTable();
            dt = posMasters.GetOriginDropDownData();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ItemNameExist(string ItemName)
        {
            int count = posMasters.GetItemExistCount(ItemName);
            var Totalresult = new { count = count };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCategoryComboData()
        {
            dt = new DataTable();
            dt = posMasters.GetCategoryDropDownData();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubCategoryComboData(string Id)
        {
            dt = new DataTable();
            dt = posMasters.GetSubCategoryDropDownData(Id);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTaxComboData()
        {
            dt = new DataTable();
            dt = posMasters.GetTaxDropdownData();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRecommendedDrinkComboData()
        {
            dt = new DataTable();
            dt = posMasters.GetRecommendedDrinkData();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentComboData()
        {
            dt = new DataTable();
            dt = posMasters.GetDepartmentDropDownData();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ShowItemMaster()
        {
            dt = new DataTable();
            dt = posMasters.ShowItemmaster();
            var JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateItemMaster(string JsonData)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            bool result = posMasters.SaveItemMaster(JsonData, createdby);
            string JsonString = "";
            if (result == true)
            {
                dt = new DataTable();
                dt = posMasters.ShowItemmaster();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }

            var Totalresult = new { Data = JsonString, TorF = result };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateItemMaster(string JsonData)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            dt = new DataTable();
            bool TorF = posMasters.UpdateItemMaster(JsonData, createdby);
            if (TorF == true)
            {
                dt = posMasters.ShowItemmaster();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            var Totalresult = new { Data = JsonString, TorF = TorF };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);

        }

        public JsonResult DeleteItemMaster(string Id, string Name)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            dt = new DataTable();
            bool TorF = posMasters.DeleteItemMaster(Id, createdby, Name);
            if (TorF == true)
            {
                dt = posMasters.ShowItemmaster();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            var Totalresult = new { Data = JsonString, TorF = TorF };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CountUpdateItemMaster(string ItemName)
        {
            int Count = posMasters.CountUpdateItemMaster(ItemName);
            return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult getPrinters()
        {
            dt = new DataTable();
            dt = posMasters.GetPrintersData();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Reasons

        public JsonResult CreateReasons(string JsonData)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            int result = posMasters.SaveReasons(JsonData, createdby);
            string JsonString = "";
            if (result == 0)
            {
                dt = new DataTable();
                dt = posMasters.ShowReasons();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }

            var Totalresult = new { Data = JsonString, TorF = result };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ShowReasons()
        {
            dt = new DataTable();
            dt = posMasters.ShowReasons();
            var JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateReasons(string JsonData)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            dt = new DataTable();
            int TorF = posMasters.UpdateReasons(JsonData, createdby);
            if (TorF == 0)
            {
                dt = posMasters.ShowReasons();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            var Totalresult = new { Data = JsonString, TorF = TorF };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);

        }

        public JsonResult DeleteReasons(string Id)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            dt = new DataTable();
            bool TorF = posMasters.DeleteReasons(Id, createdby);
            if (TorF == true)
            {
                dt = posMasters.ShowReasons();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            var Totalresult = new { Data = JsonString, TorF = TorF };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PaymentType..
        public ActionResult showPayments()
        {
            List<PaymentsDTO> lstPayments = posMasters.showPayments();
            return new JsonResult { Data = lstPayments, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SavePayments(PaymentsDTO payment)
        {
            payment.statusid = 1;
            payment.createdby = Convert.ToInt64(Session["UserId"]);
            int Count = posMasters.SavePayments(payment);
            // List<PaymentsDTO> lstPayments = posMasters.showPayments();
            return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdatePayments(PaymentsDTO payment)
        {
            int cnt = posMasters.UpdatePayments(payment);
            //List<PaymentsDTO> lstPayments = posMasters.showPayments();
            return new JsonResult { Data = cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeletePayments(PaymentsDTO payment)
        {
            bool isSaved = posMasters.DeletePayments(payment);
            List<PaymentsDTO> lstPayments = posMasters.showPayments();
            return new JsonResult { Data = lstPayments, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region VoucherType
        public ActionResult ShowVoucherType()
        {
            List<VoucherTypeDTO> lstVoucher = posMasters.ShowVoucherType();
            return new JsonResult { Data = lstVoucher, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public ActionResult SaveVoucherType(VoucherTypeDTO vch)
        {
            vch.statusid = 1;
            vch.createdby = Convert.ToInt64(Session["UserId"]);
            int Count = posMasters.SaveVoucherType(vch);
            return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateVoucherType(VoucherTypeDTO vch)
        {
            int Count = posMasters.UpdateVoucherType(vch);
            return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeleteVoucherType(VoucherTypeDTO vch)
        {
            var c = posMasters.DeleteVoucherType(vch.RecordId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public ActionResult DDlVoucherType()
        {
            List<VoucherTypeDTO> ddlVoucher = posMasters.DDlVoucherType();
            return new JsonResult { Data = ddlVoucher, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        #endregion

        #region CodeMaster
        public ActionResult CodeMaster()
        {
            return View();
        }
        public ActionResult SaveCodeMaster(List<CodeMastreDTO> code)
        {
            bool isSaved = posMasters.SaveCodeMaster(code);
            return new JsonResult { Data = isSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }
        #endregion

        #region Day special

        public ActionResult DaySpecial()
        {
            return View();
        }

        #endregion





        #region tableSelection


        public ActionResult TableSelection()
        {
            return View();
        }


        public ActionResult ShowAlltablesbind()
        {


            List<DashboardDTO> lstAvailabletablesdata = posMasters.ShowAlltablesformodel();
            ViewData["tables"] = lstAvailabletablesdata;


        #endregion
            var Totalresult = new { lstAvailabletablesdata = lstAvailabletablesdata };
            return new JsonResult { Data = Totalresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }



        public JsonResult GetHostNamesComboData()
        {

            dt = new DataTable();



            dt = posMasters.GetHostNames();

            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowAlltablesAssigned(int UserId)
        {

            List<DashboardDTO> lstAvailabletable = posMasters.ShowAssignedtables(UserId);
            ViewData["tables"] = lstAvailabletable;
            var Totalresult = new { lstAvailabletable = lstAvailabletable };
            return new JsonResult { Data = Totalresult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public JsonResult SavetableDetails(string TableName, int Createdby, string Fromdate, string Todate)
        {
            bool isValid = false;

            try
            {

                //  int createdby = Convert.ToInt16(Session["UserId"]);
                isValid = posMasters.SaveTableDetails(TableName, Createdby, Fromdate, Todate);
                var result = new { Success = isValid };
                var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public JsonResult UpdatetableDetails(string TableNames, int createdby)
        {
            bool isValid = false;

            try
            {
                List<DashboardDTO> lstAvailabletablesdata = posMasters.RunningStatusTableNames(TableNames);

                if (lstAvailabletablesdata.Count == 0)
                {
                    isValid = posMasters.UpdateAssigenedTables(TableNames, createdby);
                    var result = new { Success = isValid };
                    var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    return data;
                }
                else
                {

                    isValid = false;

                    var data = new JsonResult { Data = lstAvailabletablesdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    return data;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        //public JsonResult UpdatetableDetails(string TableNames, int createdby)
        //{
        //    bool isValid = false;

        //    try
        //    {

        //        //int createdby = Convert.ToInt16(Session["UserId"]);
        //        isValid = posMasters.UpdateAssigenedTables(TableNames, createdby);
        //        var result = new { Success = isValid };
        //        var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //        return data;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        #region DaySpecial

        public ActionResult CreateDayspecial(string JsonData, string JsonDatatab1)
        {
            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            SessionDTO objAttendenceDetails = new SessionDTO();
            List<RMS.Core.SessionDTO> myDeserializedObjList = (List<RMS.Core.SessionDTO>)Newtonsoft.Json.JsonConvert.DeserializeObject(JsonData, typeof(List<RMS.Core.SessionDTO>));
            bool TrF = posMasters.SaveCreateDayspecial(myDeserializedObjList, JsonDatatab1, createdby);
            dt = posMasters.GetSessionData();
            var JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString, TrF = TrF };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateDaySpecial(string JsonData, string JsonDatatab1, string SId)
        {

            string createdby = Convert.ToInt64(Session["UserId"]).ToString();
            SessionDTO objAttendenceDetails = new SessionDTO();
            List<RMS.Core.SessionDTO> myDeserializedObjList = (List<RMS.Core.SessionDTO>)Newtonsoft.Json.JsonConvert.DeserializeObject(JsonData, typeof(List<RMS.Core.SessionDTO>));

            bool TrF = posMasters.UpdateDaySpecial(myDeserializedObjList, JsonDatatab1, SId, createdby);
            dt = posMasters.GetSessionData();
            var JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString, TrF = TrF };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        #endregion







        #region DeliveryBoyDetails




        public ActionResult ShowDeliveryBoyDeatils()
        {



            return View();

        }

        public JsonResult SaveDeliveryBoyDetails(DeliveryBoyDTO DeliveryBoyDTO, string Employeetype)
        {
            bool isValid = false;

            try
            {
                int statusid = 1;
                int createdby = Convert.ToInt16(Session["UserId"]);
                isValid = posMasters.SaveDeliveryBoyDetails(DeliveryBoyDTO, createdby, statusid, Employeetype);
              //  var result = new { Success = isValid };
                var data = new JsonResult { Data = isValid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult ShowDeliveyboydata()
        {
            List<DeliveryBoyDTO> lstDepartment = posMasters.ShowDeliverboydetailsbyid();
            return new JsonResult { Data = lstDepartment, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult UpdateDeliveyboydata(DeliveryBoyDTO DeliveryBoyDTO)
        {
            int Cnt = posMasters.UpdateDeliverboydetails(DeliveryBoyDTO);
            return new JsonResult { Data = Cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public ActionResult DeleteDeliveyboydata(DeliveryBoyDTO DeliveryBoyDTO)
        {
            var c = posMasters.DeleteDeliverboydetails(DeliveryBoyDTO.DeliveryBoyId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

    }
}