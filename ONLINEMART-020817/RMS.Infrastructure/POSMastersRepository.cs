using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelperManager;
using System.Data;
using RMS.Core;
using RMS.Core.Interfaces;
using Npgsql;
using System.Web.Script.Serialization;
using HRMS.Infrastructure;
namespace RMS.Infrastructure
{
    public class POSMastersRepository : IPOSMasters
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans;
        NpgsqlDataReader npgdr = null;
        string GNextID = string.Empty;

        private string ManageQuote(string strMessage)
        {
            try
            {
                if (strMessage != null && strMessage != "")
                {
                    strMessage = strMessage.Replace("'", "''");
                }
            }
            catch (Exception)
            {
                return strMessage;
            }
            return strMessage;
        }
        public static string FormatDate(string strDate)
        {
            string Date = null;
            string[] dat = null;
            if (strDate != null)
            {
                if (strDate.Contains("/"))
                {
                    dat = strDate.Split('/');
                }
                else if (strDate.Contains("-"))
                {
                    dat = strDate.Split('-');
                }
                if (dat[2].Length > 4)
                {
                    dat[2] = dat[2].Substring(0, 4);
                }
                Date = dat[2] + "-" + dat[1] + "-" + dat[0];
            }
            return Date;
        }
        private void CloseCon()
        {
            con.Close();
            con.Dispose();
            con.ClearPool();
        }
        public int CheckCount(string strCheckValue, string strTableName, string strColumnName)
        {
            string strcount = "SELECT COUNT(*)  FROM " + strTableName + " WHERE upper(" + strColumnName + ")='" + ManageQuote(strCheckValue.Trim().ToUpper()) + "'and statusid=1;";
            int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
            return res;
        }


        public string GenerateNextID(string Table, string Column, string FormName)
        {

            string prefix = Convert.ToString(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select trim(upper(vchprefix)) from tabposcode  where upper(vchmastername)='" + FormName.ToUpper().Trim() + "'"));
            string res = Convert.ToString(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT (coalesce(MAX(coalesce(cast(LTRIM(" + Column + ",'" + prefix.Trim() + "')as numeric),0)),0))+1 as nextno FROM  " + Table + " where  " + Column + " like '%" + prefix.Trim() + "%';"));

            return prefix + res;
        }






        public int ItemcategoryCheck(int strCheckValue, string strTableName, string strColumnName)
        {
            string strcount = "SELECT COUNT(*)  FROM " + strTableName + " WHERE " + strColumnName + "=" + strCheckValue + " and statusid=1;";
            int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
            return res;
        }



        public bool DeleteItemcategoryCheck(ItemCategoryDTO objItemCategoryDTO)
        {
            bool Is_Valid = false;
            try
            {
                int res = ItemcategoryCheck(objItemCategoryDTO.CategoryId, "tabpositemsubcategorymst", "categoryid");
                if (res == 0)
                {
                    string strQuery = "delete from tabpositemcategorymst where categoryid=" + objItemCategoryDTO.CategoryId + ";";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery);
                    Is_Valid = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Is_Valid;
        }







        #region ITEM-CATEGORY...

        public List<ItemCategoryDTO> ShowItemCategory()
        {
            List<ItemCategoryDTO> lstItemCategory = new List<ItemCategoryDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT CATEGORYID, ITEMCATEGORYNAME, ITEMCATEGORYCODE FROM TABPOSITEMCATEGORYMST WHERE STATUSID=1 ORDER BY ITEMCATEGORYNAME;", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ItemCategoryDTO objItemCategory = new ItemCategoryDTO();
                            objItemCategory.CategoryId = Convert.ToInt32(npgdr["CATEGORYID"]);
                            objItemCategory.CategoryName = npgdr["ITEMCATEGORYNAME"].ToString();
                            objItemCategory.CategoryCode = Convert.ToString(npgdr["ITEMCATEGORYCODE"]);

                            lstItemCategory.Add(objItemCategory);
                        }
                    }
                    con.Close();
                    con.ClearPool();
                    con.Dispose();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowItemCategory");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstItemCategory;

        }

        public int SaveItemCategory(ItemCategoryDTO ItemCategory)
        {
            int Count = 0;
            try
            {

                string strCode = GenerateNextID("TABPOSITEMCATEGORYMST", "ITEMCATEGORYCODE", "ItemCategory");
                string check_exist = "SELECT COUNT(ITEMCATEGORYNAME) FROM TABPOSITEMCATEGORYMST  WHERE  UPPER(ITEMCATEGORYNAME)='" + ManageQuote(ItemCategory.CategoryName.Trim().ToUpper()) + "'   and  statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));

                //  Count = CheckCount(ItemCategory.CategoryName, "TABPOSITEMCATEGORYMST", "ITEMCATEGORYNAME");
                if (Count == 0)
                {
                    string strInsert = "INSERT INTO TABPOSITEMCATEGORYMST(ITEMCATEGORYNAME, ITEMCATEGORYCODE,statusid, createdby, createddate)  VALUES ('" + ManageQuote(ItemCategory.CategoryName.ToUpper()) + "','" + strCode + "'," + 1 + ", '" + 1 + "', Current_timestamp)";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveItemCategory");
                throw;

            }

            return Count;
        }


        public int UpdateItemCategory(ItemCategoryDTO Ic)
        {
            int Count = 0;
            int Count1 = 0;
            try
            {
                Count = ItemcategoryCheck(Ic.CategoryId, "tabpositemsubcategorymst", "categoryid");
                // string check_exist = "SELECT COUNT(ITEMCATEGORYNAME) FROM TABPOSITEMCATEGORYMST  WHERE   UPPER(ITEMCATEGORYNAME)='" + ManageQuote(Ic.CategoryName.Trim().ToUpper()) + "' and statusid=1 ";
                // Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));

                if (Count == 0)
                {

                    //if (Ic.EditCheck == "U")
                    //{
                    string check_exist = "SELECT COUNT(ITEMCATEGORYNAME) FROM TABPOSITEMCATEGORYMST  WHERE   UPPER(ITEMCATEGORYNAME)='" + ManageQuote(Ic.CategoryName.Trim().ToUpper()) + "'  and statusid=1 ";
                    Count1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                    if (Count1 == 0)
                    {
                        string strInsert = "UPDATE TABPOSITEMCATEGORYMST   SET  ITEMCATEGORYNAME='" + ManageQuote(Ic.CategoryName) + "', ITEMCATEGORYCODE='" + ManageQuote(Ic.CategoryCode) + "', modifieddate=current_timestamp,modifiedby=1 WHERE CATEGORYID='" + Ic.CategoryId + "';";
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    }
                    // }
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateItemCategory");
                throw;
            }


            return Count + Count1;
        }
        public bool DeleteItemCategory(int categoryid)
        {
            bool IsValid = false;
            try
            {
                // string check_exist = "SELECT COUNT(categoryid) FROM tabpositemsubcategorymst  WHERE   UPPER(ITEMCATEGORYNAME)='" + ManageQuote(Ic.CategoryName.Trim().ToUpper()) + "'  and statusid=1 ";
                // Count1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                int res = ItemcategoryCheck(categoryid, "tabpositemsubcategorymst", "categoryid");
                if (res == 0)
                {

                    string strQuery = "UPDATE TABPOSITEMCATEGORYMST SET STATUSID=2 WHERE categoryid='" + categoryid + "';";
                    // string strQuery = "delete from tabpositemcategorymst where categoryid=" + categoryid + ";";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery);
                    IsValid = true;
                }


                //string strDelete = "UPDATE TABPOSITEMCATEGORYMST SET STATUSID=2 WHERE CATEGORYID='" + categoryid + "';";

                //int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                //if (res == 1)
                //{
                //    IsValid = true;
                //}

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteItemCategory");
                throw;
            }

            return IsValid;
        }
        #endregion

        #region ITEM-SUBCATEGORY...

        public List<ItemSubCategoryDTO> ShowItemSubCategory()
        {
            List<ItemSubCategoryDTO> lstItemSubCategory = new List<ItemSubCategoryDTO>();
            try
            {
                //  string strInsert = "SELECT SUBCATEGORYID,ITEMSUBCATEGORYNAME,ITEMSUBCATEGORYCODE,CATEGORYID,STATUSID FROM TABPOSITEMSUBCATEGORYMST WHERE STATUSID=1 ORDER BY ITEMSUBCATEGORYNAME;";
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT SUBCATEGORYID,ITEMSUBCATEGORYNAME, ITEMSUBCATEGORYCODE, TC.CATEGORYID,TC.ITEMCATEGORYNAME, TC.ITEMCATEGORYCODE FROM TABPOSITEMCATEGORYMST TC JOIN TABPOSITEMSUBCATEGORYMST TSC ON TC.CATEGORYID =TSC.CATEGORYID  WHERE tsc.STATUSID=1 ORDER BY ITEMSUBCATEGORYNAME;", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ItemSubCategoryDTO objItemSubCategory = new ItemSubCategoryDTO();
                            objItemSubCategory.SubCategoryId = Convert.ToInt32(npgdr["SUBCATEGORYID"]);
                            objItemSubCategory.ItemSubcategory = npgdr["ITEMSUBCATEGORYNAME"].ToString();
                            objItemSubCategory.ItemSubcategoryCode = Convert.ToString(npgdr["ITEMSUBCATEGORYCODE"]);
                            objItemSubCategory.Category = Convert.ToString(npgdr["ITEMCATEGORYNAME"]);
                            objItemSubCategory.CategoryCode = Convert.ToString(npgdr["ITEMCATEGORYCODE"]);

                            lstItemSubCategory.Add(objItemSubCategory);
                        }
                    }
                    con.Close();
                    con.ClearPool();
                    con.Dispose();

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowItemSubCategory");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstItemSubCategory;

        }
        public int SaveItemSubCategory(ItemSubCategoryDTO Isc)
        {

            int Count = 0;
            try
            {
                string strCode = GenerateNextID("TABPOSITEMSUBCATEGORYMST", "ITEMSUBCATEGORYCODE", "ItemSubCategory");

                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT COUNT(ITEMSUBCATEGORYNAME) FROM TABPOSITEMSUBCATEGORYMST  WHERE  UPPER(ITEMSUBCATEGORYNAME)='" + ManageQuote(Isc.ItemSubcategory.Trim().ToUpper()) + "' and categoryid=" + Isc.CategoryId + " and statusid=1"));
                if (Count == 0)
                {
                    string strInsert = "INSERT INTO TABPOSITEMSUBCATEGORYMST(ITEMSUBCATEGORYNAME, ITEMSUBCATEGORYCODE, CATEGORYID,statusid, createdby, createddate)  VALUES ('" + ManageQuote(Isc.ItemSubcategory.Trim().ToUpper()) + "','" + strCode + "'," + Isc.CategoryId + "," + 1 + ", '" + 1 + "', Current_timestamp)";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveItemSubCategory");
                throw;

            }

            return Count;
        }
        public int UpdateItemSubCategory(ItemSubCategoryDTO Isc)
        {
            int Count = 0;
            int Count1 = 0;

            try
            {
                Count1 = ItemcategoryCheck(Isc.SubCategoryId, "tabpositemmst", "subcategoryid");

                if (Count1 == 0)
                {
                    //string check_exist = "SELECT COUNT(ITEMSUBCATEGORYNAME) FROM TABPOSITEMSUBCATEGORYMST  WHERE  UPPER(ITEMSUBCATEGORYNAME)='" + ManageQuote(Isc.ItemSubcategory.Trim().ToUpper()) + "' and  statusid=1";
                    Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT COUNT(ITEMSUBCATEGORYNAME) FROM TABPOSITEMSUBCATEGORYMST  WHERE  UPPER(ITEMSUBCATEGORYNAME)='" + ManageQuote(Isc.ItemSubcategory.Trim().ToUpper()) + "'  and  statusid=1"));
                    if (Count == 0)
                    {
                        //string strInsert = "UPDATE TABPOSITEMSUBCATEGORYMST   SET CATEGORYID=" + Isc.CategoryId + ",  ITEMSUBCATEGORYNAME='" + ManageQuote(Isc.ItemSubcategory) + "', ITEMSUBCATEGORYCODE='" + ManageQuote(Isc.ItemSubcategoryCode) + "', modifieddate=current_timestamp,modifiedby=1 WHERE SUBCATEGORYID='" + Isc.SubCategoryId + "';";
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "UPDATE TABPOSITEMSUBCATEGORYMST   SET CATEGORYID=" + Isc.CategoryId + ",  ITEMSUBCATEGORYNAME='" + ManageQuote(Isc.ItemSubcategory) + "', ITEMSUBCATEGORYCODE='" + ManageQuote(Isc.ItemSubcategoryCode) + "', modifieddate=current_timestamp,modifiedby=1 WHERE SUBCATEGORYID='" + Isc.SubCategoryId + "';");
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateItemSubCategory");
                throw;
            }

            return Count + Count1;
        }
        public bool DeleteItemSubCategory(int SubCategoryId)
        {
            bool IsValid = false;
            try
            {


                int res = ItemcategoryCheck(SubCategoryId, "tabpositemmst", "subcategoryid");
                if (res == 0)
                {
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "UPDATE TABPOSITEMSUBCATEGORYMST SET STATUSID=2 WHERE SUBCATEGORYID='" + SubCategoryId + "';");

                    IsValid = true;
                }




                ////string strDelete = "UPDATE TABPOSITEMSUBCATEGORYMST SET STATUSID=2 WHERE SUBCATEGORYID='" + SubCategoryId + "';";
                //int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "UPDATE TABPOSITEMSUBCATEGORYMST SET STATUSID=2 WHERE SUBCATEGORYID='" + SubCategoryId + "';");
                //if (res == 1)
                //{
                //    IsValid = true;
                //}

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteItemSubCategory");
                throw;
            }

            return IsValid;
        }
        #endregion

        #region ORIGIN

        public int SaveOrigin(string Json, string createby)
        {
            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(Json);
            int Count = 0;
            try
            {

                string OrName = JsonData["originname"].ToString();
                string OriginName = OrName.Trim();


                string strCode = GenerateNextID("TABPOSORIGINMST", "origincode", "Origin");

                // string check_exist = "SELECT COUNT(ORIGINNAME) FROM TABPOSORIGINMST  WHERE  UPPER(originname)='" + ManageQuote( OriginName.ToUpper() )+ "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT COUNT(ORIGINNAME) FROM TABPOSORIGINMST  WHERE  UPPER(originname)='" + ManageQuote(OriginName.ToUpper()) + "' and statusid=1"));
                if (Count == 0)
                {
                    // string strQuery = "INSERT INTO tabposoriginmst(originname, origincode, statusid, createdby,createddate) VALUES ('" + ManageQuote(OriginName) + "','" + ManageQuote(strCode) + "',1," + createby + ",current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "INSERT INTO tabposoriginmst(originname, origincode, statusid, createdby,createddate) VALUES ('" + ManageQuote(OriginName) + "','" + ManageQuote(strCode) + "',1," + createby + ",current_timestamp);");
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveOrigin");
                throw;
            }

            return Count;

        }

        public bool DeleteOrigin(string ID, string createby)
        {


            bool isSaved = false;

            try
            {

                int id = Convert.ToInt32(ID);


                int res = ItemcategoryCheck(id, "tabpositemmst", "originid");
                if (res == 0)
                {

                    // string strQuery = "UPDATE tabposoriginmst   SET  statusid=2,createdby=" + createby + " WHERE originid='" + ManageQuote(ID) + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "UPDATE tabposoriginmst   SET  statusid=2,createdby=" + createby + " WHERE originid='" + ManageQuote(ID) + "';");

                    isSaved = true;
                }

            }
            catch (Exception ex)
            {


                isSaved = false;
                throw ex;
            }

            return isSaved;


        }

        public int UpdateOrigin(string Json, string createby)
        {

            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(Json);

            int Count = 0;
            int count1 = 0;
            try
            {

                if (JsonData.ContainsKey("origincode") == false)
                {
                    JsonData.Add("origincode", "");
                }

                string OriginId = JsonData["originid"].ToString();
                string OrName = JsonData["originname"].ToString();
                string OriginName = OrName.Trim();
                string OrCode = JsonData["origincode"].ToString();
                string OriginCode = OrCode.Trim();


                int orignid = Convert.ToInt32(OriginId);




                count1 = ItemcategoryCheck(orignid, "tabpositemmst", "originid");
                if (count1 == 0)
                {
                    string check_exist = "SELECT COUNT(ORIGINNAME) FROM tabposoriginmst  WHERE  UPPER(originname)='" + OriginName.ToUpper() + "' and statusid=1 ";
                    Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                    if (Count == 0)
                    {
                        string strQuery = "UPDATE tabposoriginmst   SET  originname='" + ManageQuote(OriginName) + "', origincode='" + ManageQuote(OriginCode) + "',createdby=" + createby + ",modifiedby=1,modifieddate=current_timestamp  WHERE originid=" + OriginId + ";";
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery);
                    }
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateOrigin");
                throw;
            }

            return Count + count1;


        }

        public DataTable ShowOrigin()
        {
            DataTable dt = new DataTable();
            // string strData = string.Empty;
            try
            {

                // strData = "select originid,originname,origincode from tabposoriginmst where statusid=1";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select originid,originname,origincode from tabposoriginmst where statusid=1").Tables[0];
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowOrigin");
            }

            return dt;
        }

        #endregion

        #region Item Master

        public DataTable GetPrintersData()
        {
            DataTable dt = new DataTable();

            try
            {


                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select upper(printername) as vchprintername,upper(printername) as printername from tabposprintermst").Tables[0];
            }
            catch (Exception ex)
            {

            }

            return dt;
        }

        public DataTable GetOriginDropDownData()
        {
            DataTable dt = new DataTable();
            //string strData = string.Empty;
            try
            {

                // strData = "select originname ,originid  from tabposoriginmst where statusid=1;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select originname ,originid  from tabposoriginmst where statusid=1;").Tables[0];
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public DataTable GetDepartmentDropDownData()
        {
            DataTable dt = new DataTable();
            //string strData = string.Empty;
            try
            {

                //strData = "select departmentid,departmentname from tabposdepartmentmst Department where statusid=1;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select departmentid,departmentname from tabposdepartmentmst Department where statusid=1;").Tables[0];
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public DataTable GetCategoryDropDownData()
        {
            DataTable dt = new DataTable();
            // string strData = string.Empty;
            try
            {

                //  strData = "select categoryid, itemcategoryname from tabpositemcategorymst where statusid=1;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select categoryid, itemcategoryname from tabpositemcategorymst where statusid=1;").Tables[0];
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public DataTable GetSubCategoryDropDownData(string Id)
        {
            DataTable dt = new DataTable();
            // string strData = string.Empty;
            try
            {

                //strData = "select subcategoryid,itemsubcategoryname from tabpositemsubcategorymst where categoryid='" + Id + "' and statusid=1";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select subcategoryid,itemsubcategoryname from tabpositemsubcategorymst where categoryid='" + Id + "' and statusid=1").Tables[0];
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public DataTable GetTaxDropdownData()
        {
            DataTable dt = new DataTable();
            // string strData = string.Empty;
            try
            {

                //  strData = "select taxname,numpercentage from tabpostaxmst where statusid=1";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select taxname,numpercentage from tabpostaxmst where statusid=1").Tables[0];
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public DataTable GetRecommendedDrinkData()
        {
            DataTable dt = new DataTable();
            // string strData = string.Empty;
            try
            {

                // strData = "select itemid as recId, itemname as RecDrink from tabpositemmst where UPPER( categoryname)='BEVERAGES' and statusid=1";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select itemid as recId, itemname as RecDrink from tabpositemmst where UPPER( categoryname)='BEVERAGES' and statusid=1").Tables[0];
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public bool SaveItemMaster(string JsonItems, string createby)
        {


            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(JsonItems);

            bool isSaved = false;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                if (JsonData.ContainsKey("subcategoryid") == false)
                {

                    JsonData.Add("subcategoryid", null);

                }
                if (JsonData.ContainsKey("itemsubcategoryname") == false)
                {

                    JsonData.Add("itemsubcategoryname", "");

                }

                if (JsonData.ContainsKey("originname") == false)
                {

                    JsonData.Add("originname", "");

                }
                if (JsonData.ContainsKey("originid") == false)
                {

                    JsonData.Add("originid", 0);

                }

                if (JsonData.ContainsKey("taxapplicable") == false)
                {

                    JsonData.Add("taxapplicable", false);

                }
                if (JsonData.ContainsKey("taxtype") == false)
                {

                    JsonData.Add("taxtype", 0);

                }
                if (JsonData.ContainsKey("numtaxamount") == false)
                {

                    JsonData.Add("numtaxamount", 0);

                }

                if (JsonData.ContainsKey("taxid") == false)
                {

                    JsonData.Add("taxid", 0);

                }

                if (JsonData.ContainsKey("numrate") == false)
                {

                    JsonData.Add("numrate", 0);

                }

                if (JsonData.ContainsKey("description") == false)
                {

                    JsonData.Add("description", "");

                }
                if (JsonData.ContainsKey("RecommendedDrink") == false)
                {

                    JsonData.Add("RecommendedDrink", "");

                }
                var jj = JsonData["numrate"].ToString();
                if (JsonData["numrate"].ToString() == null)
                {
                    JsonData.Remove("numrate");
                    JsonData.Add("numrate", 0);
                }

                if (JsonData["numtaxamount"].ToString() == null)
                {
                    JsonData.Remove("numtaxamount");
                    JsonData.Add("numtaxamount", 0);
                }



                string ItName = ManageQuote(JsonData["itemname"].ToString().ToUpper()); ;
                string ItemName = ItName.ToUpper().Trim();

                string strCode = GenerateNextID("tabpositemmst", "itemcode", "ItemMaster");

                int res = CheckCount(ItemName, "tabpositemmst", "itemname");
                if (res == 0)
                {
                    string strQuery = "INSERT INTO tabpositemmst(departmentid, departmentname,itemname, itemcode, categoryid,categoryname, subcategoryid,subcategoryname, numratewithouttaxes,originid,originname, taxapplicable,taxpercentage, taxtype, numtaxamount, recommendeddrink,description,numrate, statusid, createdby, createddate,recdrinkid,vchprintername) VALUES (" + ManageQuote(JsonData["departmentid"].ToString().ToUpper()) + ",'" + ManageQuote(JsonData["departmentname"].ToString().ToUpper()) + "','" + ManageQuote(JsonData["itemname"].ToString().ToUpper()) + "','" + strCode + "','" + ManageQuote(JsonData["categoryid"].ToString().ToUpper()) + "','" + ManageQuote(JsonData["itemcategoryname"].ToString().ToUpper()) + "','" + ManageQuote(JsonData["subcategoryid"].ToString().ToUpper()) + "','" + ManageQuote(JsonData["itemsubcategoryname"].ToString().ToUpper()) + "','" + ManageQuote(JsonData["numratewithouttaxes"].ToString().ToUpper()) + "','" + ManageQuote(JsonData["originid"].ToString().ToUpper()) + "','" + ManageQuote(JsonData["originname"].ToString().ToUpper()) + "','" + ManageQuote(JsonData["taxapplicable"].ToString().ToUpper()) + "','" + ManageQuote(JsonData["taxid"].ToString().ToUpper()) + "','" + ManageQuote(JsonData["taxtype"].ToString().ToUpper()) + "','" + ManageQuote(JsonData["numtaxamount"].ToString().ToUpper()) + "','" + ManageQuote(JsonData["RecommendedDrink"].ToString().ToUpper()) + "','" + ManageQuote(JsonData["description"].ToString().ToUpper()) + "','" + JsonData["numrate"].ToString().ToUpper() + "',1," + createby + ",current_timestamp," + JsonData["recid"].ToString().ToUpper() + ",'" + JsonData["vchprintername"].ToString().ToUpper() + "');";
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, strQuery);
                    isSaved = true;
                }
            }
            catch (Exception ex)
            {



                isSaved = false;
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                }
            }
            return isSaved;

        }

        public DataTable ShowItemmaster()
        {
            DataTable dt = new DataTable();
            // string strData = string.Empty;
            try
            {

                // strData = "SELECT recdrinkid,departmentid, departmentname,itemid, itemname, itemcode, categoryid,categoryname, subcategoryid,subcategoryname, numratewithouttaxes,originid,originname, taxapplicable,taxpercentage, taxtype, numtaxamount, recommendeddrink,description,numrate  FROM tabpositemmst where statusid=1;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT recdrinkid,departmentid, departmentname,itemid, itemname, itemcode, categoryid,categoryname, subcategoryid,subcategoryname, numratewithouttaxes,originid,originname, taxapplicable,taxpercentage, taxtype, numtaxamount, recommendeddrink,description,numrate,upper(vchprintername) as vchprintername  FROM tabpositemmst where statusid=1;").Tables[0];
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public bool UpdateItemMaster(string JsonItems, string createby)
        {


            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(JsonItems);

            bool isSaved = false;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                if (JsonData.ContainsKey("subcategoryid") == false)
                {

                    JsonData.Add("subcategoryid", null);

                }
                if (JsonData.ContainsKey("itemsubcategoryname") == false)
                {

                    JsonData.Add("itemsubcategoryname", "");

                }

                if (JsonData.ContainsKey("originname") == false)
                {

                    JsonData.Add("originname", "");

                }
                if (JsonData.ContainsKey("originid") == false)
                {

                    JsonData.Add("originid", 0);

                }

                if (JsonData.ContainsKey("taxapplicable") == false)
                {

                    JsonData.Add("taxapplicable", false);

                }
                if (JsonData.ContainsKey("taxtype") == false)
                {

                    JsonData.Add("taxtype", 0);

                }
                if (JsonData.ContainsKey("numtaxamount") == false)
                {

                    JsonData.Add("numtaxamount", 0);

                }

                if (JsonData.ContainsKey("taxid") == false)
                {

                    JsonData.Add("taxid", 0);

                }

                if (JsonData.ContainsKey("numrate") == false)
                {

                    JsonData.Add("numrate", 0);

                }

                if (JsonData.ContainsKey("description") == false)
                {

                    JsonData.Add("description", "");

                }
                if (JsonData.ContainsKey("RecommendedDrink") == false)
                {

                    JsonData.Add("RecommendedDrink", "");

                }
                if (JsonData.ContainsKey("taxapplicable") == true)
                {
                    var TorF = JsonData["taxapplicable"].ToString().ToUpper();
                    if (TorF == "FALSE")
                    {

                        JsonData["taxtype"] = 0;
                        JsonData["numtaxamount"] = 0;
                    }

                }

                if (JsonData["numrate"].ToString() == "")
                {
                    JsonData.Remove("numrate");
                    JsonData.Add("numrate", 0);
                }







                // int res1="select count(itemname) from tabpossessiondetailsmst where itemname='"++"',statusid=1";
                //int res1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT COUNT(itemname) FROM tabpossessiondetailsmst  WHERE  UPPER(itemname)='" + ManageQuote(JsonData["itemname"].ToString().ToUpper()) + "' and  statusid=1"));
                //if (res1 == 0)
                //{
                // string check_exist = "SELECT COUNT(itemname) FROM tabpositemmst  WHERE  UPPER(itemname)='" + ManageQuote(JsonData["itemname"].ToString().ToUpper()) + "' and  itemid<>'" + ManageQuote(JsonData["itemid"].ToString()) + "'  and  statusid=1";
                int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT COUNT(itemname) FROM tabpositemmst  WHERE  UPPER(itemname)='" + ManageQuote(JsonData["itemname"].ToString().ToUpper()) + "' and  itemid<>'" + ManageQuote(JsonData["itemid"].ToString()) + "'  and  statusid=1"));

                if (res == 0)
                {
                    // string strQuery = "UPDATE tabpositemmst SET departmentid=" + JsonData["departmentid"].ToString() + ", departmentname='" + ManageQuote(JsonData["departmentname"].ToString().ToUpper()) + "', itemname='" + ManageQuote(JsonData["itemname"].ToString().ToUpper()) + "', itemcode='" + ManageQuote(JsonData["itemcode"].ToString().ToUpper()) + "', categoryid='" + ManageQuote(JsonData["categoryid"].ToString().ToUpper()) + "', categoryname='" + ManageQuote(JsonData["itemcategoryname"].ToString().ToUpper()) + "', subcategoryid='" + ManageQuote(JsonData["subcategoryid"].ToString().ToUpper()) + "', subcategoryname='" + ManageQuote(JsonData["itemsubcategoryname"].ToString().ToUpper()) + "', numratewithouttaxes='" + ManageQuote(JsonData["numratewithouttaxes"].ToString().ToUpper()) + "', originid='" + ManageQuote(JsonData["originid"].ToString().ToUpper()) + "', originname='" + ManageQuote(JsonData["originname"].ToString().ToUpper()) + "', taxapplicable='" + ManageQuote(JsonData["taxapplicable"].ToString().ToUpper()) + "', taxtype='" + ManageQuote(JsonData["taxtype"].ToString().ToUpper()) + "', numtaxamount='" + ManageQuote(JsonData["numtaxamount"].ToString().ToUpper()) + "', recommendeddrink='" + ManageQuote(JsonData["RecommendedDrink"].ToString().ToUpper()) + "',description='" + ManageQuote(JsonData["description"].ToString().ToUpper()) + "',taxpercentage='" + ManageQuote(JsonData["taxid"].ToString().ToUpper()) + "',numrate='" + ManageQuote(JsonData["numrate"].ToString().ToUpper()) + "',recdrinkid='" + ManageQuote(JsonData["recid"].ToString().ToUpper()) + "', statusid=1,createdby=" + createby + " WHERE itemid='" + ManageQuote(JsonData["itemid"].ToString()) + "';";

                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "UPDATE tabpositemmst SET departmentid=" + JsonData["departmentid"].ToString() + ", departmentname='" + ManageQuote(JsonData["departmentname"].ToString().ToUpper()) + "', itemname='" + ManageQuote(JsonData["itemname"].ToString().ToUpper()) + "', itemcode='" + ManageQuote(JsonData["itemcode"].ToString().ToUpper()) + "', categoryid='" + ManageQuote(JsonData["categoryid"].ToString().ToUpper()) + "', categoryname='" + ManageQuote(JsonData["itemcategoryname"].ToString().ToUpper()) + "', subcategoryid='" + ManageQuote(JsonData["subcategoryid"].ToString().ToUpper()) + "', subcategoryname='" + ManageQuote(JsonData["itemsubcategoryname"].ToString().ToUpper()) + "', numratewithouttaxes='" + ManageQuote(JsonData["numratewithouttaxes"].ToString().ToUpper()) + "', originid='" + ManageQuote(JsonData["originid"].ToString().ToUpper()) + "', originname='" + ManageQuote(JsonData["originname"].ToString().ToUpper()) + "', taxapplicable='" + ManageQuote(JsonData["taxapplicable"].ToString().ToUpper()) + "', taxtype='" + ManageQuote(JsonData["taxtype"].ToString().ToUpper()) + "', numtaxamount='" + ManageQuote(JsonData["numtaxamount"].ToString().ToUpper()) + "', recommendeddrink='" + ManageQuote(JsonData["RecommendedDrink"].ToString().ToUpper()) + "',description='" + ManageQuote(JsonData["description"].ToString().ToUpper()) + "',taxpercentage='" + ManageQuote(JsonData["taxid"].ToString().ToUpper()) + "',numrate='" + ManageQuote(JsonData["numrate"].ToString().ToUpper()) + "',vchprintername='" + ManageQuote(JsonData["vchprintername"].ToString().ToUpper()) + "',recdrinkid='" + ManageQuote(JsonData["recid"].ToString().ToUpper()) + "', statusid=1,modifiedby=" + createby + " WHERE itemid='" + ManageQuote(JsonData["itemid"].ToString()) + "';");
                    isSaved = true;
                }
                // }
            }
            catch (Exception ex)
            {
                isSaved = false;
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                }
            }
            return isSaved;

        }



        public int CountUpdateItemMaster(string ItemName)
        {

            int Count = 0;
            try
            {

                Count = CheckCount(ItemName, "tabpossessiondetailsmst", "itemname");

                if (Count == 0)
                {

                }


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Tax");
                throw;
            }
            return Count;
        }










        public bool DeleteItemMaster(string Id, string createby, string Name)
        {



            bool isSaved = false;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                //string strQuery = "UPDATE tabpositemmst SET statusid=2,createdby=" + createby + " WHERE itemid='" + Id + "';";

                int res = CheckCount(Name, "tabpossessiondetailsmst", "itemname");
                if (res == 0)
                {


                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "UPDATE tabpositemmst SET statusid=2,createdby=" + createby + " WHERE itemid='" + Id + "';");
                    isSaved = true;
                }

            }
            catch (Exception ex)
            {
                isSaved = false;
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();

                }
            }
            return isSaved;

        }

        public int GetItemExistCount(string ItemName)
        {
            int count = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                //string strQuery = "select count(*) from tabposkotdetails   where itemname='" + ItemName + "'";
                count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select count(*) from tabposkotdetails   where itemname='" + ManageQuote(ItemName) + "'"));


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();

                }
            }
            return count;
        }

        #endregion

        #region   SECTION DETAILS...

        public List<SectionDTO> ShowSection()
        {
            List<SectionDTO> lstSection = new List<SectionDTO>();
            try
            {
                // string strInsert = "SELECT SECTIONID,SECTIONNAME,SECTIONCODE,STATUSID FROM tabpossectionmst WHERE STATUSID=1 ORDER BY SECTIONID desc;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT SECTIONID,SECTIONNAME,SECTIONCODE,STATUSID FROM tabpossectionmst WHERE STATUSID=1 ORDER BY SECTIONID desc;");
                while (npgdr.Read())
                {
                    SectionDTO objSection = new SectionDTO();
                    objSection.SectionId = Convert.ToInt32(npgdr["sectionid"]);
                    objSection.SectionName123 = npgdr["sectionname"].ToString();
                    objSection.SectionCode = npgdr["sectioncode"].ToString();
                    lstSection.Add(objSection);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowSection");
                throw ex;
                // ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstSection;

        }
        public int SaveSection(string SecName, string createdby)
        {
            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(SecName);
            int Count = 0;
            try
            {
                string strCode = GenerateNextID("TABPOSSECTIONMST", "SECTIONCODE", "SectionMaster");
                string check_exist = "SELECT COUNT(sectionname) FROM TABPOSSECTIONMST  WHERE  UPPER(sectionname)='" + ManageQuote(JsonData["SectionName123"].ToString().ToUpper()) + "'  and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strInsert = "INSERT INTO TABPOSSECTIONMST(SECTIONNAME, SECTIONCODE, STATUSID, CREATEDBY, CREATEDDATE)VALUES ('" + ManageQuote(JsonData["SectionName123"].ToString().ToUpper()) + "','" + strCode + "',  1," + createdby + ",current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveSection");
                throw;
            }

            return Count;
        }

        public int UpdateSection(string Sec, string cre)
        {
            int Count = 0;
            int Count1 = 0;
            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(Sec);


            string sectid = JsonData["SectionId"].ToString();

            try
            {

                int idsec = Convert.ToInt32(sectid);
                Count1 = ItemcategoryCheck(idsec, "tabpostablesandcoversmst", "sectionid");
                if (Count1 == 0)
                {
                    string check_exist = "SELECT COUNT(sectionname) FROM TABPOSSECTIONMST  WHERE  UPPER(sectionname)='" + ManageQuote(JsonData["SectionName123"].ToString().ToUpper()) + "' and statusid=1  ";
                    Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                    if (Count == 0)
                    {
                        string strUpdate = "UPDATE TABPOSSECTIONMST SET  SECTIONNAME='" + ManageQuote(JsonData["SectionName123"].ToString().ToUpper()) + "',   modifieddate=current_timestamp,modifiedby=1 WHERE SECTIONID='" + ManageQuote(JsonData["SectionId"].ToString().ToUpper()) + "';";
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strUpdate);

                    }
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateSection");
                throw;
            }

            return Count + Count1;
        }
        public bool DeleteSection(int SectionId)
        {
            bool IsValid = false;
            try
            {

                string selectquery = "select count(*) from tabpostablesandcoversmst where sectionid=" + SectionId + " and statusid=1";

                int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, selectquery));
                // int res = ItemcategoryCheck(SectionId, "tabpostablesandcoversmst", "sectionid");
                if (res == 0)
                {


                    string strDelete = "UPDATE tabpossectionmst SET STATUSID=2 WHERE SECTIONID='" + SectionId + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                    //if (res == 1)
                    //{
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteSection");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }
        #endregion

        #region TABLES AND COVERS...

        public List<TablesDTO> ShowTableandCovers()
        {
            List<TablesDTO> lstPrinterdetails = new List<TablesDTO>();
            try
            {
                string strInsert = "SELECT TABLEID,TABLESNAME,TABLECODE,NUMCOVERS,SECTIONID,SECTIONNAME,SECTIONCODE,STATUSID  FROM TABPOSTABLESANDCOVERSMST WHERE STATUSID=1 ORDER BY TABLEID desc;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                while (npgdr.Read())
                {
                    TablesDTO objTables = new TablesDTO();
                    objTables.TableId = Convert.ToInt32(npgdr["TABLEID"]);
                    objTables.TablesName = npgdr["TABLESNAME"].ToString();
                    objTables.TableCode = npgdr["TABLECODE"].ToString();
                    objTables.Convers = npgdr["NUMCOVERS"].ToString();
                    objTables.SectionId = npgdr["SECTIONID"].ToString();
                    objTables.SectionName = Convert.ToString(npgdr["SECTIONNAME"]);
                    objTables.SectionCode = Convert.ToString(npgdr["SECTIONCODE"]);

                    lstPrinterdetails.Add(objTables);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowTableandCovers");
                throw ex;
                // ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstPrinterdetails;

        }
        public int SaveTableandCovers(TablesDTO Tab)
        {

            int Count = 0;
            try
            {
                string strCode = GenerateNextID("TABPOSTABLESANDCOVERSMST", "TABLECODE", "Department");

                string check_exist = "SELECT COUNT(TABLESNAME) FROM TABPOSTABLESANDCOVERSMST  WHERE UPPER(TABLESNAME)='" + Tab.TablesName.Trim().ToUpper() + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strInsert = "INSERT INTO TABPOSTABLESANDCOVERSMST(TABLESNAME,TABLECODE,NUMCOVERS,SECTIONID,SECTIONNAME,SECTIONCODE,STATUSID,CREATEDBY,CREATEDDATE,status,datassigntodate) VALUES ( '" + ManageQuote(Tab.TablesName) + "', '" + strCode + "', '" + ManageQuote(Tab.Convers) + "','" + Tab.SectionId + "', '" + ManageQuote(Tab.SectionName) + "','" + ManageQuote(Tab.SectionCode) + "', 1,1,current_timestamp,'A',(select DATE 'yesterday'));";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveTableandCovers");
                throw;
            }

            return Count;
        }
        public int UpdateTableandCovers(TablesDTO Tab)
        {

            int Count = 0;
            try
            {

                string check_exist = "SELECT COUNT(TABLESNAME) FROM TABPOSTABLESANDCOVERSMST  WHERE  UPPER(TABLESNAME)='" + Tab.TablesName.Trim().ToUpper() + "' and TABLEID<>'" + Tab.TableId + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strUpdate = "UPDATE TABPOSTABLESANDCOVERSMST   SET  TABLESNAME='" + ManageQuote(Tab.TablesName) + "', NUMCOVERS='" + ManageQuote(Tab.Convers) + "', SECTIONNAME='" + ManageQuote(Tab.SectionName) + "', SECTIONCODE='" + ManageQuote(Tab.SectionCode) + "', modifieddate=current_timestamp,modifiedby=1 WHERE TABLEID='" + Tab.TableId + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strUpdate);
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateTableandCovers");
                throw;
            }

            return Count;
        }
        public bool DeleteTableandCovers(int tableid)
        {
            bool IsValid = false;
            try
            {

                string strDelete = "UPDATE TABPOSTABLESANDCOVERSMST SET STATUSID=2 WHERE TABLEID='" + tableid + "';";
                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                if (res == 1)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteTableandCovers");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }
        public string checkStatustoDelete(int tableid)
        {
            string res = "";

            try
            {

                string strDelete = "select status from tabpostablesandcoversmst where tablesname='" + tableid + "'";
                res = NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete).ToString();



            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteTableandCovers");
                throw;
            }
            finally
            {

            }
            return res;

        }

        #endregion

        #region REASONS...

        public DataTable ShowReasons()
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {
                strData = "select reasonid,reasonname,reasoncode,applicableto from tabposreasonmst where statusid=1 ORDER BY reasonid desc";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowReasons");
            }

            return dt;
        }

        public int SaveReasons(string Json, string createby)
        {
            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(Json);
            int Count = 0;

            try
            {

                if (JsonData.ContainsKey("origincode") == false)
                {

                    JsonData.Add("origincode", "");

                }

                string resname = JsonData["reasonname"].ToString();
                string Reasonname = resname.Trim();

                string resapplicableto = JsonData["applicableto"].ToString();
                string ReasonApplicableto = resapplicableto.Trim();

                string strCode = GenerateNextID("tabposreasonmst", "reasoncode", "Reasons");

                string check_exist = "SELECT COUNT(reasonname) FROM tabposreasonmst  WHERE  UPPER(reasonname)='" + Reasonname.ToUpper() + "'  and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strQuery = "INSERT INTO tabposreasonmst(reasonname, reasoncode, applicableto, statusid, createdby, createddate)  VALUES ('" + ManageQuote(Reasonname) + "','" + strCode + "','" + ManageQuote(ReasonApplicableto) + "',1," + createby + ",current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery);
                }
            }

            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveReasons");
                throw;
            }

            return Count;

        }

        public int UpdateReasons(string Json, string createby)
        {

            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(Json);
            int Count = 0;

            try
            {

                if (JsonData.ContainsKey("origincode") == false)
                {

                    JsonData.Add("origincode", "");

                }
                string Reasonid = JsonData["reasonid"].ToString();
                string resname = JsonData["reasonname"].ToString();
                string Reasonname = resname.Trim();
                string resCode = JsonData["reasoncode"].ToString();
                string ReasonCode = resCode.Trim();
                string resapplicableto = JsonData["applicableto"].ToString();
                string ReasonApplicableto = resapplicableto.Trim();

                string check_exist = "SELECT COUNT(reasonname) FROM tabposreasonmst  WHERE  UPPER(reasonname)='" + Reasonname.ToUpper() + "'  and reasonid<>'" + Reasonid + "' and statusid=1 ";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strQuery = "UPDATE tabposreasonmst   SET  reasonname='" + ManageQuote(Reasonname) + "', reasoncode='" + ManageQuote(ReasonCode) + "', applicableto='" + ManageQuote(ReasonApplicableto) + "',createdby=" + createby + ",modifiedby=1,modifieddate=current_timestamp  WHERE reasonid=" + Reasonid + ";";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateReasons");
                throw;
            }

            return Count;


        }
        public bool DeleteReasons(string ID, string createby)
        {


            bool isSaved = false;

            try
            {


                string strQuery = "UPDATE tabposreasonmst   SET  statusid=2,createdby=" + createby + " WHERE reasonid='" + ManageQuote(ID) + "';";
                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery);

                isSaved = true;

            }
            catch (Exception ex)
            {


                isSaved = false;
                throw ex;
            }

            return isSaved;


        }
        #endregion

        #region VOUCHER...
        public List<VoucherDTO> ShowVoucher()
        {
            List<VoucherDTO> lstVoucherdetails = new List<VoucherDTO>();
            try
            {
                string strInsert = "SELECT VOUCHERID,VOUCHERNAME,VOUCHERCODE,VOUCHERTYPE,NUMPERCENTAGE,NUMAMOUNT,CASE WHEN NUMPERCENTAGE=0 THEN  NUMAMOUNT::TEXT ELSE NUMPERCENTAGE::TEXT||' %'   END AS TOTAL, NUMVALIDPERIOD,DATVALIDUPTO,DESCRIPTION,TERMSANDCONDITIONS,CASE WHEN NUMPERCENTAGE=0 THEN 'Amount' ELSE 'Percentage' END AS TYPE  FROM TABPOSVOUCHERMST WHERE STATUSID=1 ORDER BY VOUCHERID DESC ;";
                //"SELECT VOUCHERID,VOUCHERNAME,VOUCHERCODE,VOUCHERTYPE,NUMPERCENTAGE,NUMAMOUNT,NUMVALIDPERIOD,DATVALIDUPTO,DESCRIPTION,TERMSANDCONDITIONS FROM TABPOSVOUCHERMST WHERE STATUSID=1 ORDER BY VOUCHERID desc ;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                while (npgdr.Read())
                {
                    VoucherDTO objVoucherDTO = new VoucherDTO();

                    objVoucherDTO.VoucherId = Convert.ToInt32(npgdr["VOUCHERID"]);
                    objVoucherDTO.VoucherCode = Convert.ToString(npgdr["VOUCHERCODE"]);
                    objVoucherDTO.VoucherName = Convert.ToString(npgdr["VOUCHERNAME"]);
                    objVoucherDTO.VoucherType = Convert.ToString(npgdr["VOUCHERTYPE"]);
                    objVoucherDTO.Percentage = Convert.ToString(npgdr["NUMPERCENTAGE"]);
                    objVoucherDTO.Amount = Convert.ToString(npgdr["NUMAMOUNT"]);
                    if (Convert.ToString(npgdr["NUMVALIDPERIOD"]) != "0")
                    {
                        objVoucherDTO.ValidPeriod = Convert.ToString(npgdr["NUMVALIDPERIOD"]);
                    }

                    if (Convert.ToDateTime(npgdr["DATVALIDUPTO"]).ToString("dd-MM-yyyy") != "01-01-0001")
                    {
                        objVoucherDTO.ValidUpto = Convert.ToDateTime(npgdr["DATVALIDUPTO"]).ToString("dd-MM-yyyy");
                    }

                    objVoucherDTO.Description = Convert.ToString(npgdr["DESCRIPTION"]);
                    objVoucherDTO.TermsandConditions = Convert.ToString(npgdr["TERMSANDCONDITIONS"]);
                    objVoucherDTO.Type = Convert.ToString(npgdr["TYPE"]);
                    objVoucherDTO.Total = Convert.ToString(npgdr["TOTAL"]);
                    lstVoucherdetails.Add(objVoucherDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowVoucher");
                throw ex;
                // ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstVoucherdetails;

        }
        //public int SaveVoucher(VoucherDTO Vch)
        //{
        //    int Count = 0;
        //    try
        //    {

        //        string check_exist = "SELECT COUNT(vouchername) FROM tabposvouchermst  WHERE  vouchername='" + Vch.VoucherName.Trim().ToUpper() + "' and statusid=1";
        //        Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
        //        if (Count == 0)
        //        {
        //            string strInsert = "INSERT INTO tabposvouchermst( vouchername, vouchercode, vouchertype, numpercentage,numamount, numvalidperiod, datvalidupto, description, termsandconditions,statusid, createdby, createddate)  VALUES ('" + ManageQuote(Vch.VoucherName) + "', 5, '" + ManageQuote(Vch.VoucherType) + "', '" + Vch.Percentage + "','" + Vch.Amount + "', '" + ManageQuote(Vch.ValidPeriod) + "' , '" + ManageQuote(Vch.ValidUpto) + "', '" + ManageQuote(Vch.Description) + "', '" + ManageQuote(Vch.TermsandConditions) + "',1,1,current_timestamp);";
        //            NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "SaveVoucher");
        //        throw;
        //    }

        //    return Count;
        //}
        public int SaveVoucher(VoucherDTO Vch)
        {
            int Count = 0;
            try
            {
                string strCode = GenerateNextID("TABPOSVOUCHERMST", "vouchercode", "Voucher");
                string check_exist = "SELECT COUNT(vouchername) FROM TABPOSVOUCHERMST  WHERE  upper(vouchername)='" + Vch.VoucherName.Trim().ToUpper() + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    if (Vch.ValidPeriod == null || Vch.ValidPeriod == string.Empty)
                    {
                        Vch.ValidPeriod = "0";
                    }
                    if (Vch.ValidUpto == null || Vch.ValidUpto == string.Empty)
                    {
                        Vch.ValidUpto = "01-01-0001";
                    }
                    string strInsert = "INSERT INTO TABPOSVOUCHERMST(vouchername,vouchercode, vouchertype, numpercentage,numamount, numvalidperiod, datvalidupto, description, termsandconditions,statusid, createdby, createddate) VALUES ('" + ManageQuote(Vch.VoucherName) + "','" + strCode + "','" + ManageQuote(Vch.VoucherType) + "', '" + Vch.Percentage + "','" + Vch.Amount + "', '" + ManageQuote(Vch.ValidPeriod) + "' , '" + FormatDate(Vch.ValidUpto) + "', '" + ManageQuote(Vch.Description) + "', '" + ManageQuote(Vch.TermsandConditions) + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveVoucher");
                throw;
            }

            return Count;
        }
        public int UpdateVoucher(VoucherDTO Vch)
        {
            int Count = 0;
            try
            {
                if (string.IsNullOrEmpty(Vch.ValidPeriod))
                {
                    Vch.ValidPeriod = "0";
                }
                if (string.IsNullOrEmpty(Vch.ValidUpto))
                {
                    Vch.ValidUpto = "01-01-0001";
                }

                //string check_exist = "SELECT COUNT(vouchername) FROM tabposvouchermst  WHERE  vouchername='" + Vch.VoucherName.Trim().ToUpper() + "' and statusid=1";
                string check_exist = "SELECT COUNT(vouchername) FROM tabposvouchermst  WHERE  UPPER(vouchername)='" + Vch.VoucherName.Trim().ToUpper() + "' and voucherid<>'" + Vch.VoucherId + "' and statusid=1 ";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strInsert = "UPDATE tabposvouchermst   SET  vouchername='" + ManageQuote(Vch.VoucherName) + "',VoucherCode='" + ManageQuote(Vch.VoucherCode.Trim().ToUpper()) + "', vouchertype='" + ManageQuote(Vch.VoucherType) + "', numpercentage='" + Vch.Percentage + "',numamount='" + Vch.Amount + "', numvalidperiod='" + ManageQuote(Vch.ValidPeriod) + "', datvalidupto='" + ManageQuote(Vch.ValidUpto) + "', description='" + ManageQuote(Vch.Description) + "',termsandconditions='" + ManageQuote(Vch.TermsandConditions) + "',  modifiedby=1, modifieddate=current_timestamp WHERE VoucherId='" + Vch.VoucherId + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateVoucher");
                throw;
            }

            return Count;
        }
        public bool DeleteVoucher(int voucherid)
        {
            bool IsValid = false;
            try
            {

                string strDelete = "UPDATE TABPOSVOUCHERMST SET STATUSID=2 WHERE voucherid='" + voucherid + "';";

                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                if (res == 1)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteVoucher");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }
        #endregion

        #region DEPARTMENT...

        public List<DepartmentDTO> ShowDepartment()
        {
            List<DepartmentDTO> lstDepartmentdetails = new List<DepartmentDTO>();
            try
            {
                string strInsert = "SELECT DEPARTMENTID,DEPARTMENTNAME,DEPARTMENTCODE FROM TABPOSDEPARTMENTMST WHERE STATUSID=1 ORDER BY DEPARTMENTID desc;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                while (npgdr.Read())
                {
                    DepartmentDTO objDepartmentDTO = new DepartmentDTO();
                    objDepartmentDTO.RecordId = Convert.ToInt32(npgdr["DEPARTMENTID"]);
                    objDepartmentDTO.DeparmentName = npgdr["DEPARTMENTNAME"].ToString();
                    objDepartmentDTO.DeparmentCode = Convert.ToString(npgdr["DEPARTMENTCODE"]);

                    lstDepartmentdetails.Add(objDepartmentDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowDepartment");
                throw ex;
                // ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstDepartmentdetails;

        }
        public int SaveDepartment(DepartmentDTO Dept)
        {

            int Count = 0;
            try
            {
                string strCode = GenerateNextID("TABPOSDEPARTMENTMST", "DEPARTMENTCODE", "Department");

                //  string check_exist = "SELECT COUNT(DEPARTMENTNAME) FROM TABPOSDEPARTMENTMST  WHERE  UPPER(DEPARTMENTNAME)='" + Dept.DeparmentName.Trim().ToUpper() + "' or  UPPER(DEPARTMENTCODE)='" + Dept.DeparmentCode.Trim().ToUpper() + "' and statusid=1";
                string check_exist = "SELECT COUNT(DEPARTMENTNAME) FROM TABPOSDEPARTMENTMST  WHERE  UPPER(DEPARTMENTNAME)='" + Dept.DeparmentName.Trim().ToUpper() + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strInsert = "INSERT INTO TABPOSDEPARTMENTMST(DEPARTMENTNAME,DEPARTMENTCODE,STATUSID,createdby,createddate)VALUES('" + Dept.DeparmentName.Trim().ToUpper() + "','" + strCode + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveDepartment");
                throw;
            }

            return Count;
        }
        public int UpdateDepartment(DepartmentDTO Dept)
        {
            int Count = 0;
            int Count1 = 0;
            try
            {


                string check = "select COUNT(*) from tabpositemmst where UPPER(DEPARTMENTNAME)='" + ManageQuote(Dept.DeparmentName.Trim().ToUpper()) + "' and statusid=1;";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check));
                // string check = "SELECT COUNT(DEPARTMENTNAME) FROM TABPOSDEPARTMENTMST  WHERE  UPPER(DEPARTMENTNAME)='" + Dept.DeparmentName.Trim().ToUpper() + "'  and statusid=1 ";
                //  Count = ItemcategoryCheck(Dept.RecordId, "tabpositemmst", "departmentid");
                if (Count == 0)
                {
                    string check_exist = "SELECT COUNT(DEPARTMENTNAME) FROM TABPOSDEPARTMENTMST  WHERE  UPPER(DEPARTMENTNAME)='" + Dept.DeparmentName.Trim().ToUpper() + "'  and statusid=1 ";
                    Count1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                    if (Count1 == 0)
                    {
                        string strInsert = "UPDATE TABPOSDEPARTMENTMST SET DEPARTMENTCODE='" + Dept.DeparmentCode + "',DEPARTMENTNAME='" + Dept.DeparmentName + "',modifieddate=current_timestamp,modifiedby=1 WHERE DEPARTMENTID='" + Dept.RecordId + "';";
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateDepartment");
                throw;
            }

            return Count + Count1;
        }
        public bool DeleteDepartment(int RecordId)
        {
            bool IsValid = false;
            try
            {



                int res = ItemcategoryCheck(RecordId, "tabpositemmst", "departmentid");
                if (res == 0)
                {
                    string strDelete = "UPDATE TABPOSDEPARTMENTMST SET STATUSID=2 WHERE DEPARTMENTID='" + RecordId + "';";

                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);
                    IsValid = true;
                }





                //if (res == 1)
                //{

                //}

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteDepartment");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }
        #endregion

        #region PRINTERS...

        public List<PrinterDTO> ShowPrinter()
        {
            List<PrinterDTO> lstPrinterdetails = new List<PrinterDTO>();
            try
            {

                string strInsert = "SELECT departmentname,PRINTERID,PRINTERNAME,PRINTERCODE,IPADDRESS,NUMPORT,STATUSID  FROM TABPOSPRINTERMST WHERE STATUSID=1 ORDER BY PRINTERID desc;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                while (npgdr.Read())
                {
                    PrinterDTO objPrinter = new PrinterDTO();
                    objPrinter.PrinterId = Convert.ToInt32(npgdr["PRINTERID"]);
                    objPrinter.DeparmentName = npgdr["departmentname"].ToString();
                    objPrinter.PrinterName = npgdr["PRINTERNAME"].ToString();
                    objPrinter.PrinterCode = npgdr["PRINTERCODE"].ToString();
                    objPrinter.IpAddress = npgdr["IPADDRESS"].ToString();
                    objPrinter.ProtNo = Convert.ToString(npgdr["NUMPORT"]);

                    lstPrinterdetails.Add(objPrinter);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowPrinter");
                throw ex;
                // ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstPrinterdetails;

        }
        public int SavePrinter(PrinterDTO Prnt)
        {
            int Count = 0;
            try
            {

                string strCode = GenerateNextID("TABPOSPRINTERMST", "PRINTERCODE", "Printer");
                string check_exist = "SELECT COUNT(PRINTERNAME) FROM TABPOSPRINTERMST  WHERE  upper(PRINTERNAME)='" + ManageQuote(Prnt.PrinterName.Trim().ToUpper()) + "'  and  UPPER(departmentname)='" + ManageQuote(Prnt.DeparmentName.Trim().ToUpper()) + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strInsert = "INSERT INTO TABPOSPRINTERMST(departmentname,PRINTERNAME,PRINTERCODE,IPADDRESS,NUMPORT,STATUSID,CREATEDBY, CREATEDDATE) VALUES (   '" + ManageQuote(Prnt.DeparmentName) + "','" + ManageQuote(Prnt.PrinterName.ToUpper()) + "', '" + strCode + "', '" + ManageQuote(Prnt.IpAddress) + "', '" + ManageQuote(Prnt.ProtNo) + "', 1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SavePrinter");
                throw;
            }

            return Count;
        }
        public int UpdatePrinter(PrinterDTO Prnt)
        {
            int Count = 0;
            try
            {

                string check_exist = "SELECT COUNT(PRINTERNAME) FROM TABPOSPRINTERMST  WHERE  (UPPER(PRINTERNAME)='" + ManageQuote(Prnt.PrinterName.Trim().ToUpper()) + "'  or UPPER(IpAddress)='" + ManageQuote(Prnt.IpAddress.Trim().ToUpper()) + "') and  UPPER(departmentname)='" + ManageQuote(Prnt.DeparmentName.Trim().ToUpper()) + "' and  statusid=1 and PRINTERID<>'" + Prnt.PrinterId + "' ";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strUpdate = "UPDATE TABPOSPRINTERMST   SET  departmentname='" + ManageQuote(Prnt.DeparmentName) + "',  PRINTERNAME='" + ManageQuote(Prnt.PrinterName.ToUpper()) + "', PRINTERCODE='" + ManageQuote(Prnt.PrinterCode) + "', IPADDRESS='" + ManageQuote(Prnt.IpAddress) + "', NUMPORT='" + ManageQuote(Prnt.ProtNo) + "', modifieddate=current_timestamp,modifiedby=1 WHERE PRINTERID='" + Prnt.PrinterId + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strUpdate);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdatePrinter");
                throw;
            }

            return Count;
        }
        public bool DeletePrinter(int printId)
        {
            bool IsValid = false;
            try
            {

                string strDelete = "UPDATE TABPOSPRINTERMST SET STATUSID=2 WHERE PRINTERID='" + printId + "';";
                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                if (res == 1)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeletePrinter");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }
        #endregion

        #region PAYMENT TYPE..

        public List<PaymentsDTO> showPayments()
        {
            List<PaymentsDTO> lstPayments = new List<PaymentsDTO>();
            try
            {
                string strPayments = "select paymenttypeid,paymentmodename,paymentmodecode,chargesapplicable,taxtype,percentage,numamount from tabpospaymenttypemst where statusid=1 order by paymenttypeid";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strPayments, null);
                while (npgdr.Read())
                {
                    string chrgApp = string.Empty;
                    PaymentsDTO objPaymentsDTO = new PaymentsDTO();
                    objPaymentsDTO.paymenttypeid = Convert.ToInt32(npgdr["paymenttypeid"]);
                    objPaymentsDTO.paymentmodename = npgdr["paymentmodename"].ToString();
                    objPaymentsDTO.paymentmodecode = npgdr["paymentmodecode"].ToString();
                    objPaymentsDTO.chargesAppl = npgdr["chargesapplicable"].ToString();
                    objPaymentsDTO.taxtype = npgdr["taxtype"].ToString();

                    objPaymentsDTO.amount = Convert.ToDouble(npgdr["numamount"].ToString());


                    lstPayments.Add(objPaymentsDTO);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstPayments;
        }
        public int SavePayments(PaymentsDTO payment)
        {
            //Boolean isValid = false;
            int cnt = 0;
            string strchrgAppl = string.Empty;
            try
            {
                if (Convert.ToBoolean(payment.chargesAppl))
                {
                    strchrgAppl = "Y";
                }
                else
                {
                    strchrgAppl = "N";
                    payment.amount = 0;
                    payment.taxtype = string.Empty;
                }
                string strCode = GenerateNextID("tabpospaymenttypemst", "paymentmodecode", "PaymentType");
                string strCount = "select count(*) from tabpospaymenttypemst where upper(paymentmodename)='" + ManageQuote(payment.paymentmodename.ToUpper()) + "'  and statusid=1";
                cnt = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strCount, null));
                if (cnt == 0)
                {
                    if (payment.paymentmodecode == null)
                        payment.paymentmodecode = "";

                    string strPayInsert = "insert into tabpospaymenttypemst(paymentmodename,paymentmodecode,chargesapplicable,taxtype,numamount,statusid,createddate)";
                    strPayInsert = strPayInsert + "Values('" + ManageQuote(payment.paymentmodename.Trim()) + "','" + strCode + "','" + ManageQuote(strchrgAppl.Trim()) + "','" + payment.taxtype.Trim() + "'," + Convert.ToDouble(payment.amount) + ",1," + "current_timestamp)";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strPayInsert, null);
                    //isValid = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cnt;
        }
        public int UpdatePayments(PaymentsDTO payment)
        {

            string strchrgAppl = string.Empty;
            int cnt = 0;
            try
            {
                if (Convert.ToBoolean(payment.chargesAppl))
                {
                    strchrgAppl = "Y";
                }
                else
                {
                    strchrgAppl = "N";
                    payment.amount = 0;
                    payment.taxtype = string.Empty;
                }

                string strCount = "select count(*) from tabpospaymenttypemst where upper(paymentmodename)='" + ManageQuote(payment.paymentmodename.ToUpper().Trim()) + "'  and paymenttypeid<>" + payment.paymenttypeid + " and statusid=1 ";
                cnt = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strCount, null));
                if (cnt == 0)
                {
                    string strUpdate = "update tabpospaymenttypemst set paymentmodename='" + ManageQuote(payment.paymentmodename.Trim()) + "',paymentmodecode='" + ManageQuote(payment.paymentmodecode.Trim()) + "',chargesapplicable='" + ManageQuote(strchrgAppl) + "',taxtype='" + ManageQuote(payment.taxtype.Trim()) + "',numamount=" + Convert.ToDouble(payment.amount) + ",modifieddate=current_timestamp,modifiedby=1 where paymenttypeid=" + payment.paymenttypeid;
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strUpdate, null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return cnt;
        }
        public bool DeletePayments(PaymentsDTO payment)
        {
            bool isValid = false;
            try
            {
                string strDelete = "update tabpospaymenttypemst set statusid=2  where paymenttypeid=" + payment.paymenttypeid;
                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete, null);
                isValid = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return isValid;
        }

        #endregion

        #region   TAX DETAILS...

        public List<TaxDTO> ShowTaxDetails()
        {
            List<TaxDTO> lstTaxDetails = new List<TaxDTO>();
            try
            {
                string strlocation = "SELECT TAXID,TAXNAME,NUMPERCENTAGE,STATUSID FROM TABPOSTAXMST WHERE STATUSID=1 ORDER BY TAXID desc;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strlocation);
                while (npgdr.Read())
                {
                    TaxDTO objTaxDTO = new TaxDTO();
                    objTaxDTO.TaxId = Convert.ToInt32(npgdr["TAXID"]);
                    objTaxDTO.TaxName = npgdr["TAXNAME"].ToString();
                    objTaxDTO.Percentage = npgdr["NUMPERCENTAGE"].ToString();
                    //objTaxDTO.StatusId = Convert.ToString(npgdr["STATUSID"]);

                    lstTaxDetails.Add(objTaxDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Tax");
                throw ex;
                // ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstTaxDetails;

        }
        public int SaveTax(TaxDTO tx)
        {


            int Count = 0;
            try
            {


                string check_exist = "SELECT COUNT(TAXNAME) FROM TABPOSTAXMST  WHERE  TAXNAME='" + ManageQuote(tx.TaxName.Trim().ToUpper()) + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strInsert = "INSERT INTO TABPOSTAXMST(TAXNAME, NUMPERCENTAGE, STATUSID, CREATEDBY, CREATEDDATE)  VALUES ('" + ManageQuote(tx.TaxName.ToUpper()) + "', '" + tx.Percentage + "',  1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    //IsValid = true;
                }


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Tax");
                throw;
            }
            finally
            {

            }
            //return IsValid;
            return Count;
        }


        public int CountUpdateTax(string name)
        {

            int Count = 0;
            try
            {

                Count = CheckCount(name, "tabpositemmst", "taxtype");

                if (Count == 0)
                {

                }


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Tax");
                throw;
            }
            return Count;
        }






        public int UpdateTax(TaxDTO tx)
        {

            int Count = 0;
            try
            {

                Count = CheckCount(tx.TaxName, "tabpositemmst", "taxtype");

                // string check_exist = "SELECT COUNT(TAXNAME) FROM TABPOSTAXMST  WHERE  TAXNAME='" + ManageQuote(tx.TaxName.Trim().ToUpper()) + "' AND TAXID<>" + tx.TaxId + " and statusid=1";
                // Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strInsert = "UPDATE tabpostaxmst SET  taxname='" + ManageQuote(tx.TaxName) + "', numpercentage='" + ManageQuote(tx.Percentage) + "',  modifieddate=current_timestamp,modifiedby=1 WHERE taxid='" + tx.TaxId + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Tax");
                throw;
            }
            return Count;
        }
        public bool DeleteTax(int id, string name)
        {
            bool IsValid = false;
            try
            {



                int res = CheckCount(name, "tabpositemmst", "taxtype");
                if (res == 0)
                {
                    string strDelete = "UPDATE tabpostaxmst SET STATUSID=2 WHERE TAXID='" + id + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);
                    IsValid = true;
                }

                //if (res == 1)
                //{
                //    IsValid = true;
                //}

            }
            catch (Exception ex)
            {
                // EventLogger.WriteToErrorLog(ex, "Department");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }
        #endregion

        #region VOUCHER TYPE...
        public List<VoucherTypeDTO> ShowVoucherType()
        {
            List<VoucherTypeDTO> lstvouchertypedetails = new List<VoucherTypeDTO>();
            try
            {
                string strlocation = "SELECT VOUCHERTYPENAME,VOUCHERTYPECODE,VOUCHERTYPEID FROM tabposvouchertypemst where statusid=1 ORDER BY vouchertypeid desc;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strlocation);
                while (npgdr.Read())
                {
                    VoucherTypeDTO objVoucherTypeDTO = new VoucherTypeDTO();
                    objVoucherTypeDTO.RecordId = Convert.ToInt32(npgdr["VOUCHERTYPEID"]);
                    objVoucherTypeDTO.VoucherTypeName = npgdr["VOUCHERTYPENAME"].ToString();
                    objVoucherTypeDTO.VoucherTypeCode = Convert.ToString(npgdr["VOUCHERTYPECODE"]);


                    lstvouchertypedetails.Add(objVoucherTypeDTO);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstvouchertypedetails;
        }
        public int SaveVoucherType(VoucherTypeDTO Vch)
        {
            int Count = 0;
            try
            {
                string strCode = GenerateNextID("tabposvouchertypemst", "vouchertypecode", "VoucherType");
                string check_exist = "SELECT COUNT(vouchertypename) FROM tabposvouchertypemst  WHERE  UPPER(vouchertypename)='" + ManageQuote(Vch.VoucherTypeName.Trim().ToUpper()) + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strInsert = "INSERT INTO tabposvouchertypemst(vouchertypename,vouchertypecode,statusid, createdby, createddate) VALUES ('" + ManageQuote(Vch.VoucherTypeName.Trim()) + "','" + strCode + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "VoucherType");
                throw;
            }

            return Count;
        }
        public int UpdateVoucherType(VoucherTypeDTO Vch)
        {
            int Count = 0;
            int count1 = 0;

            try
            {



                string exist = "SELECT COUNT(VOUCHERTYPE) FROM tabposvouchermst where vouchertype='" + ManageQuote(Vch.VoucherTypeName.Trim().ToUpper()) + "' and statusid=1;";

                count1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, exist));

                if (count1 == 0)
                {
                    string check_exist = "SELECT COUNT(VOUCHERTYPENAME) FROM tabposvouchertypemst where UPPER(vouchertypename)='" + ManageQuote(Vch.VoucherTypeName.Trim().ToUpper()) + "' and vouchertypeid<>" + Vch.RecordId + " AND statusid=1 ";
                    Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                    if (Count == 0)
                    {
                        string strInsert = "UPDATE tabposvouchertypemst SET vouchertypecode='" + ManageQuote(Vch.VoucherTypeCode.Trim().ToUpper()) + "',vouchertypename='" + ManageQuote(Vch.VoucherTypeName.Trim().ToUpper()) + "',modifieddate=current_timestamp,modifiedby=1 WHERE vouchertypeid='" + Vch.RecordId + "';";
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "VoucherType");
                throw;
            }

            return Count + count1;
        }
        public bool DeleteVoucherType(int RecordId)
        {
            bool IsValid = false;
            try
            {










                string strDelete = "UPDATE tabposvouchertypemst SET STATUSID=2 WHERE vouchertypeid='" + RecordId + "';";

                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                if (res == 1)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "VoucherType");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        public List<VoucherTypeDTO> DDlVoucherType()
        {
            List<VoucherTypeDTO> lstddlvouchertypes = new List<VoucherTypeDTO>();
            try
            {
                string strlocation = "SELECT VOUCHERTYPENAME,VOUCHERTYPECODE FROM tabposvouchertypemst where statusid=1 ORDER BY VOUCHERTYPENAME;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strlocation);
                while (npgdr.Read())
                {
                    VoucherTypeDTO objVoucherTypeDTO = new VoucherTypeDTO();
                    objVoucherTypeDTO.VoucherTypeName = npgdr["VOUCHERTYPENAME"].ToString();
                    objVoucherTypeDTO.VoucherTypeCode = Convert.ToString(npgdr["VOUCHERTYPECODE"]);


                    lstddlvouchertypes.Add(objVoucherTypeDTO);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstddlvouchertypes;
        }
        #endregion

        #region Session

        public DataTable getTreeViewData()
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "SELECT itemcode as itemid, itemname, categoryid, categoryname, subcategoryid,subcategoryname FROM tabpositemmst where statusid=1";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public bool SaveSession(List<SessionDTO> myDeserializedObjList, string Json, string Createdby)
        {

            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(Json);
            bool isSaved = false;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                long recordid = 0;

                string check_exist = "SELECT COUNT(sessionname) FROM tabpossessionmst  WHERE  UPPER(sessionname)='" + ManageQuote(JsonData["SessionName"].ToString().ToUpper()) + "'   and  statusid=1";
                int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));


                if (res == 0)
                {
                    string strQuerytree = "";

                    string strQuery = "INSERT INTO tabpossessionmst(sessionname, fromtime, totime, statusid, createdby, createddate)  VALUES ('" + ManageQuote(JsonData["SessionName"].ToString().ToUpper()) + "','" + ManageQuote(JsonData["datstartdate"].ToString().ToUpper()) + "','" + ManageQuote(JsonData["datTodate"].ToString().ToUpper()) + "',1," + Createdby + ",current_timestamp) returning sessionid;";
                    recordid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery));


                    for (int i = 0; i < myDeserializedObjList.Count; i++)
                    {
                        if (myDeserializedObjList[i].Selected == "True")
                        {
                            if (myDeserializedObjList[i].subcategoryid == null || myDeserializedObjList[i].subcategoryid == "0")
                            {

                                myDeserializedObjList[i].subcategoryid = DBNull.Value.ToString();
                                strQuerytree = "INSERT INTO tabpossessiondetailsmst(sessionid, itemname, itemcode, categoryid,categoryname, subcategoryid,subcategoryname,statusid, createdby, createddate)    VALUES ('" + recordid + "','" + myDeserializedObjList[i].itemname + "','" + myDeserializedObjList[i].itemid + "','" + myDeserializedObjList[i].categoryid + "','" + myDeserializedObjList[i].categoryname + "',null,'',1,1,current_timestamp);";

                            }
                            else
                            {

                                strQuerytree = "INSERT INTO tabpossessiondetailsmst(sessionid, itemname, itemcode, categoryid,categoryname, subcategoryid,subcategoryname,statusid, createdby, createddate)    VALUES ('" + recordid + "','" + myDeserializedObjList[i].itemname + "','" + myDeserializedObjList[i].itemid + "','" + myDeserializedObjList[i].categoryid + "','" +ManageQuote( myDeserializedObjList[i].categoryname) + "','" + myDeserializedObjList[i].subcategoryid + "','" + ManageQuote(myDeserializedObjList[i].subcategoryname) + "',1,1,current_timestamp);";
                            }


                            NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuerytree);
                        }
                    }
                    isSaved = true;
                }






            }
            catch (Exception ex)
            {



                isSaved = false;
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();

                }
            }
            return isSaved;

        }

        public bool UpdateSession(List<SessionDTO> myDeserializedObjList, string Json, string Id, string Createdby)
        {

            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(Json);
            bool isSaved = false;
            long recordid = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }


                string check_exist = "SELECT COUNT(sessionname) FROM tabpossessionmst  WHERE  UPPER(sessionname)='" + ManageQuote(JsonData["SessionName"].ToString().ToUpper()) + "' and  sessionid<>'" + Id + "'  and  statusid=1";
                int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));

                if (res == 0)
                {
                    string strQuery = "UPDATE tabpossessionmst   SET  sessionname='" + ManageQuote(JsonData["SessionName"].ToString().ToUpper()) + "', fromtime='" + ManageQuote(JsonData["datstartdate"].ToString().ToUpper()) + "', totime='" + ManageQuote(JsonData["datTodate"].ToString().ToUpper()) + "', statusid=1,createdby=" + Createdby + " WHERE sessionid='" + Id + "'";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery);

                    string dele = "delete from tabpossessiondetailsmst  where sessionid='" + Id + "'";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, dele);
                    string strQuerytree = "";

                    for (int i = 0; i < myDeserializedObjList.Count; i++)
                    {
                        if (myDeserializedObjList[i].Selected == "True")
                        {
                            if (myDeserializedObjList[i].subcategoryid == null || myDeserializedObjList[i].subcategoryid == "0")
                            {

                                myDeserializedObjList[i].subcategoryid = DBNull.Value.ToString();
                                strQuerytree = "INSERT INTO tabpossessiondetailsmst(sessionid, itemname, itemcode, categoryid,categoryname, subcategoryid,subcategoryname,statusid, createdby, createddate)    VALUES ('" + Id + "','" + myDeserializedObjList[i].itemname + "','" + myDeserializedObjList[i].itemid + "','" + myDeserializedObjList[i].categoryid + "','" + myDeserializedObjList[i].categoryname + "',null,'',1," + Createdby + ",current_timestamp);";

                            }
                            else
                            {

                                strQuerytree = "INSERT INTO tabpossessiondetailsmst(sessionid, itemname, itemcode, categoryid,categoryname, subcategoryid,subcategoryname,statusid, createdby, createddate)    VALUES ('" + Id + "','" + myDeserializedObjList[i].itemname + "','" + myDeserializedObjList[i].itemid + "','" + myDeserializedObjList[i].categoryid + "','" + ManageQuote(myDeserializedObjList[i].categoryname) + "','" + myDeserializedObjList[i].subcategoryid + "','" + ManageQuote(myDeserializedObjList[i].subcategoryname) + "',1," + Createdby + ",current_timestamp);";
                            }


                            NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuerytree);
                        }
                    }

                    isSaved = true;
                }
            }
            catch (Exception ex)
            {



                isSaved = false;
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();

                }
            }
            return isSaved;

        }

        public DataTable GetSessionData()
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "select sessionid,sessionname,fromtime,totime from tabpossessionmst where statusid=1 ;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public DataTable getTreeBtnUpdateViewData(string SessionId)
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "SELECT itemcode as itemid, itemname, categoryid, categoryname, subcategoryid,subcategoryname FROM tabpossessiondetailsmst where sessionid='" + SessionId + "'";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public DataTable getTreeViewDataDayspecial(string Date)
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "SELECT itemcode as itemid, itemname, categoryid, categoryname, subcategoryid,subcategoryname FROM tabposdayspecialitems where datdate='" + FormatDate(Date) + "'";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }


        public int GetSessionCount(int From, int To, string SessionId, string Text)
        {
            int count = 0;
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "select count(*) from tabpossessionmst where ( (" + From + " between ((EXTRACT( HOUR FROM  fromtime::time) * 60)+EXTRACT( MINUTES FROM  fromtime::time)) and ((EXTRACT( HOUR FROM  totime::time) * 60)+EXTRACT( MINUTES FROM  totime::time))) or (" + To + " between ((EXTRACT( HOUR FROM  fromtime::time) * 60)+EXTRACT( MINUTES FROM  fromtime::time)) and ((EXTRACT( HOUR FROM  totime::time) * 60)+EXTRACT( MINUTES FROM  totime::time)))) and statusid=1 and sessionid <> " + SessionId + "";
                count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strData));
            }
            catch (Exception ex)
            {

            }

            return count;

        }

        public bool DeleteSessionData(string sessionId, string Createdby)
        {
            bool isSaved = false;







            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                string strQueryKOT = "select count(*) from tabposkotdetails as  tkd join tabposkot as tk on tkd.vchkotid=tk.vchkotid where sessionid=" + sessionId + " and status='N'";
                int count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strQueryKOT));

                if (count == 0)
                {
                    string strQuery = "update tabpossessiondetailsmst  set statusid=2,createdby=" + Createdby + "  where sessionid=" + sessionId + "";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery);
                    string stsession = "update tabpossessionmst set statusid=2,createdby=" + Createdby + " where sessionid=" + sessionId + "";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, stsession);
                    isSaved = true;
                }


            }
            catch (Exception ex)
            {



                isSaved = false;
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();

                }
            }
            return isSaved;

        }


        #endregion

        #region CODEMASTER
        public bool SaveCodeMaster(List<CodeMastreDTO> code)
        {
            bool isSaved = false;
            int count = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                for (int i = 0; i < code.Count; i++)
                {
                    //string check = "select count(*) from tabposcode where vchprefix='"+code[i].Prefix+"';";
                    //count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check));
                    //if (count == 0)
                    //{
                    string check_exist = "insert into tabposcode(vchmastername,vchprefix,statusid,createdby,createddate)values('" + code[i].MasterName + "','" + code[i].Prefix + "'," + 1 + ", " + 1 + ", Current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist);
                    isSaved = true;
                    //}
                    //else {
                    //    isSaved = false;
                    //}

                }


            }
            catch (Exception ex)
            {
                isSaved = false;
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();

                }
            }
            return isSaved;
        }
        #endregion

        #region Table Selection

        public List<DashboardDTO> ShowAlltablesformodel()
        {
            List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {

                    con.Open();



               //--old   string strInsert = "SELECT sectionname,array_to_string(array_agg(distinct tablesname),',') AS tablesname,array_to_string(array_agg(distinct tableid),',') AS tableid FROM tabpostablesandcoversmst WHERE  statusid=1 and datassigntodate<'" + DateTime.Now.ToString("yyyy-MM-dd") + "' GROUP BY sectionname";



                   //  string strInsert = "SELECT sectionname,array_to_string(array_agg(distinct tablesname),',') AS tablesname,array_to_string(array_agg(distinct tableid),',') AS tableid FROM tabpostablesandcoversmst WHERE  statusid=1  and datassigntodate<'" + DateTime.Now.ToString("yyyy-MM-dd") + "' GROUP BY sectionname";
                    string strInsert = "SELECT sectionname,array_to_string(array_agg(distinct tablesname),',') AS tablesname,array_to_string(array_agg(distinct tableid),',') AS tableid FROM tabpostablesandcoversmst WHERE  statusid=1 and status='A' and datassigntodate<CURRENT_DATE GROUP BY sectionname";
                    // string strInsert = "SELECT sectionname,array_to_string(array_agg(distinct tablesname),',') AS tablesname,array_to_string(array_agg(distinct tableid),',') AS tableid FROM tabpostablesandcoversmst WHERE  statusid=1 and modifieddate::date!= '" + DateTime.Now.ToString("yyyy-MM-dd") + "' GROUP BY sectionname";
                   // string strInsert = "SELECT sectionname,array_to_string(array_agg(distinct tablesname),',') AS tablesname,array_to_string(array_agg(distinct tableid),',') AS tableid FROM tabpostablesandcoversmst WHERE  statusid=1 and modifieddate::date<(select distinct dattodate from tabpostablesandcoversmstmaster where dattodate<='" + DateTime.Now.ToString("yyyy-MM-dd") + "') GROUP BY sectionname";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();

                        while (npgdr.Read())
                        {
                            DashboardDTO objKOTTakingDTO = new DashboardDTO();

                            objKOTTakingDTO.SectioName = npgdr["sectionname"].ToString();
                            objKOTTakingDTO.Tablesid = npgdr["TABLEID"].ToString();
                            objKOTTakingDTO.TableName = Convert.ToString(npgdr["tablesname"]);
                            // objKOTTakingDTO.TableStatus = Convert.ToString(npgdr["STATUS"]);

                            var mystring = objKOTTakingDTO.Tablesid;

                            var mytablename = objKOTTakingDTO.TableName;
                            objKOTTakingDTO.Mytableid = mystring.Split(',');

                            objKOTTakingDTO.MytableName = mytablename.Split(',');
                            lstKOTTakingDTO.Add(objKOTTakingDTO);

                        }

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowAvailabletables");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstKOTTakingDTO;
        }


        public List<DashboardDTO> RunningStatusTableNames(string TableNames)
        {
            List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {

                    con.Open();

                    string strcount = "select count(*) from tabpostablesandcoversmst where status in('R','M','B')  and tablesname in(" + TableNames + ") and statusid=1";

                    int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, strcount));

                    if (count != 0)
                    {


                        string strInsert = "SELECT array_to_string(array_agg(distinct tablesname),',') AS tablesname from tabpostablesandcoversmst where status in('R','M','B')  and tablesname in(" + TableNames + ")and statusid=1";

                        using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                        {
                            npgdr = cmd.ExecuteReader();

                            while (npgdr.Read())
                            {
                                DashboardDTO objKOTTakingDTO = new DashboardDTO();


                                objKOTTakingDTO.TableName = Convert.ToString(npgdr["tablesname"]);




                                var mytablename = objKOTTakingDTO.TableName;

                                objKOTTakingDTO.MytableName = mytablename.Split(',');
                                lstKOTTakingDTO.Add(objKOTTakingDTO);

                            }

                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowAvailabletables");
                throw ex;

            }
            finally
            {
                // con.Close();
                // npgdr.Dispose();
            }
            return lstKOTTakingDTO;
        }

        public List<DashboardDTO> GetrunningTablenames(string TableNames)
        {
            List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {

                    con.Open();

                    string strInsert = "SELECT array_to_string(array_agg(distinct tablesname),',') AS tablesname where status in('R','M','B')  and tablesname in(" + TableNames + ")";
                    //  string strInsert = "SELECT sectionname,array_to_string(array_agg(distinct tablesname),',') AS tablesname,array_to_string(array_agg(distinct tableid),',') AS tableid FROM tabpostablesandcoversmst WHERE  statusid=1 and modifieddate::date!= '" + DateTime.Now.ToString("yyyy-MM-dd") + "' GROUP BY sectionname";
                    //string strInsert = "SELECT sectionname,array_to_string(array_agg(distinct tablesname),',') AS tablesname,array_to_string(array_agg(distinct tableid),',') AS tableid FROM tabpostablesandcoversmst WHERE  statusid=1 and modifieddate::date<(select distinct dattodate from tabpostablesandcoversmstmaster where dattodate<='" + DateTime.Now.ToString("yyyy-MM-dd") + "') GROUP BY sectionname";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();

                        while (npgdr.Read())
                        {
                            DashboardDTO objKOTTakingDTO = new DashboardDTO();

                            //  objKOTTakingDTO.SectioName = npgdr["sectionname"].ToString();
                            // objKOTTakingDTO.Tablesid = npgdr["TABLEID"].ToString();
                            objKOTTakingDTO.TableName = Convert.ToString(npgdr["tablesname"]);
                            // objKOTTakingDTO.TableStatus = Convert.ToString(npgdr["STATUS"]);

                            //var mystring = objKOTTakingDTO.Tablesid;

                            var mytablename = objKOTTakingDTO.TableName;
                            // objKOTTakingDTO.Mytableid = mystring.Split(',');

                            objKOTTakingDTO.MytableName = mytablename.Split(',');
                            lstKOTTakingDTO.Add(objKOTTakingDTO);

                        }

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowAvailabletables");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstKOTTakingDTO;
        }






        public DataTable GetHostNames()
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "select userid,username from tabuserinfo where statusid=1;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowAvailabletables");
            }

            return dt;
        }

        public List<DashboardDTO> ShowAssignedtables(int Userid)
        {
            List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();




              //-old string strInsert = "SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST  WHERE STATUS IN ('A')  and statusid=1 and tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) UNION ALL SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B','M') AND statusid=1 and  tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) order by TABLEID";


                 //  string strInsert = "SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST  WHERE STATUS IN ('A')  and statusid=1 and tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) UNION all  SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B','M') AND statusid=1 and  tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) order by TABLEID";


                    string strInsert = "SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST  WHERE STATUS IN ('A')  and statusid=1 and tablesname in(select tablesname from tabpostablesassignedmst where dattodate>=CURRENT_DATE and hostid=" + Userid + " and statusid=1) UNION  SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B','M') AND statusid=1 and  tablesname in(select tablesname from tabpostablesassignedmst where  hostid=" + Userid + " and statusid=1 and dattodate>=(CURRENT_DATE -INTERVAL '1 day')::date) and datassigntodate>=(CURRENT_DATE -INTERVAL '1 day')::date union  SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B','M') AND statusid=1 and  tablesname in(select tablesname from tabpostablesassignedmst where dattodate>=CURRENT_DATE and hostid=" + Userid + " and statusid=1) order by TABLEID";

                  //string strInsert = "SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST  WHERE STATUS IN ('A')  and statusid=1 and tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) UNION ALL SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B','M') AND statusid=1 and  tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) union all SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B','M') AND statusid=1 and  tablesname in(select tablesname from tabpostablesassignedmst where hostid=1 and statusid=1) and datassigntodate<'" + DateTime.Now.ToString("yyyy-MM-dd") + "' order by TABLEID";




                   // string strInsert = "SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST  WHERE STATUS IN ('A')  and statusid=1 and tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) UNION  SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B','M') AND statusid=1 and  tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) union  SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B','M') AND statusid=1 and  tablesname in(select tablesname from tabpostablesassignedmst where hostid=1 and statusid=1) and datassigntodate>'" + DateTime.Now.ToString("yyyy-MM-dd") + "' order by TABLEID";


                    // string strInsert = "SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST  WHERE STATUS IN ('A')  and statusid=1 and tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) UNION ALL SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B') AND  TABLEID IN(select TABLEID from tabrmskot where kotid in(select max(kotid) from tabrmskot group by TABLEID ) and createdby=" + Userid + ")   and statusid=1 and  tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) order by TABLEID";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();

                        while (npgdr.Read())
                        {
                            DashboardDTO objKOTTakingDTO = new DashboardDTO();
                            objKOTTakingDTO.SectioName = npgdr["sectionname"].ToString();
                            objKOTTakingDTO.Tableid = Convert.ToInt32(npgdr["TABLEID"]);
                            objKOTTakingDTO.TableName = Convert.ToString(npgdr["TABLESNAME"]);
                            objKOTTakingDTO.TableStatus = Convert.ToString(npgdr["STATUS"]);
                            lstKOTTakingDTO.Add(objKOTTakingDTO);
                        }

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowAvailabletables");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstKOTTakingDTO;
        }
        public bool SaveTableDetails(string TableName, int Createdby, string Fromdate, string Todate)
        {
            bool IssaveDetails = false;
            string strsaveDetails = string.Empty;
            List<String> listStrtablename;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                listStrtablename = TableName.Split(',').ToList();

                for (int i = 0; i < listStrtablename.Count; i++)
                {
                    string savedetailsTable = "insert into tabpostablesassignedmst(tablesname,hostid,datfromdate,dattodate,statusid,createddate) values(" + listStrtablename[i] + "," + Createdby + ",'" + FormatDate(Fromdate) + "','" + FormatDate(Todate) + "',1,current_timestamp);";

                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, savedetailsTable);
                    strsaveDetails = "update tabpostablesandcoversmst set datassigntodate='" + FormatDate(Todate) + "'  where tablesname in(" + listStrtablename[i] + ")";

                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strsaveDetails);
                    IssaveDetails = true;
                }


            }
            catch (Exception ex)
            {
                throw ex;


            }
            finally
            {

                con.Close();

            }
            return IssaveDetails;
        }

        public bool UpdateAssigenedTables(string TableNames, int hostid)
        {
            bool IsUpdate = false;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                string strcount = "select count(*) from tabpostablesandcoversmst where status in('R','M','B')  and tablesname in(" + TableNames + ") and statusid=1";

                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, strcount));

                if (count == 0)
                {

                    string strUpdateassignedmst = "update  tabpostablesassignedmst set statusid=2 where tablesname in(" + TableNames + ") and hostid=" + hostid + " and dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, strUpdateassignedmst);
                    string strupdateCOVERSMST = "update TABPOSTABLESANDCOVERSMST set datassigntodate=(select DATE 'yesterday') where tablesname in(" + TableNames + ");";

                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, strupdateCOVERSMST);


                    IsUpdate = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                con.Close();
            }


            return IsUpdate;



        }

        #endregion

        #region DaySpecial

        public bool SaveCreateDayspecial(List<SessionDTO> myDeserializedObjList, string Json, string Createdby)
        {

            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(Json);
            bool isSaved = false;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }




                string strQuerytree = "";



                for (int i = 0; i < myDeserializedObjList.Count; i++)
                {
                    if (myDeserializedObjList[i].Selected == "True")
                    {
                        if (myDeserializedObjList[i].subcategoryid == null || myDeserializedObjList[i].subcategoryid == "0")
                        {

                            myDeserializedObjList[i].subcategoryid = DBNull.Value.ToString();
                            strQuerytree = "INSERT INTO tabposdayspecialitems(datdate, itemname, itemcode, categoryid,categoryname, subcategoryid,subcategoryname,statusid, createdby, createddate)    VALUES ('" + FormatDate(JsonData["SessionName"].ToString().ToUpper()) + "','" + myDeserializedObjList[i].itemname + "','" + myDeserializedObjList[i].itemid + "','" + myDeserializedObjList[i].categoryid + "','" + myDeserializedObjList[i].categoryname + "',null,'',1,1,current_timestamp);";

                        }
                        else
                        {

                            strQuerytree = "INSERT INTO tabposdayspecialitems(datdate, itemname, itemcode, categoryid,categoryname, subcategoryid,subcategoryname,statusid, createdby, createddate)    VALUES ('" + FormatDate(JsonData["SessionName"].ToString().ToUpper()) + "','" + myDeserializedObjList[i].itemname + "','" + myDeserializedObjList[i].itemid + "','" + myDeserializedObjList[i].categoryid + "','" + myDeserializedObjList[i].categoryname + "','" + myDeserializedObjList[i].subcategoryid + "','" + myDeserializedObjList[i].subcategoryname + "',1,1,current_timestamp);";
                        }


                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuerytree);
                    }
                }
                isSaved = true;






            }
            catch (Exception ex)
            {



                isSaved = false;
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();

                }
            }
            return isSaved;

        }

        public bool UpdateDaySpecial(List<SessionDTO> myDeserializedObjList, string Json, string Date, string Createdby)
        {

            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(Json);
            bool isSaved = false;
            long recordid = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }



                string dele = "delete from tabposdayspecialitems  where datdate='" + FormatDate(Date) + "'";
                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, dele);
                string strQuerytree = "";

                for (int i = 0; i < myDeserializedObjList.Count; i++)
                {
                    if (myDeserializedObjList[i].Selected == "True")
                    {
                        if (myDeserializedObjList[i].subcategoryid == null || myDeserializedObjList[i].subcategoryid == "0")
                        {

                            myDeserializedObjList[i].subcategoryid = DBNull.Value.ToString();
                            strQuerytree = "INSERT INTO tabposdayspecialitems(datdate, itemname, itemcode, categoryid,categoryname, subcategoryid,subcategoryname,statusid, createdby, createddate)    VALUES ('" + FormatDate(JsonData["SessionName"].ToString().ToUpper()) + "','" + myDeserializedObjList[i].itemname + "','" + myDeserializedObjList[i].itemid + "','" + myDeserializedObjList[i].categoryid + "','" + myDeserializedObjList[i].categoryname + "',null,'',1,1,current_timestamp);";

                        }
                        else
                        {

                            strQuerytree = "INSERT INTO tabposdayspecialitems(datdate, itemname, itemcode, categoryid,categoryname, subcategoryid,subcategoryname,statusid, createdby, createddate)    VALUES ('" + FormatDate(JsonData["SessionName"].ToString().ToUpper()) + "','" + myDeserializedObjList[i].itemname + "','" + myDeserializedObjList[i].itemid + "','" + myDeserializedObjList[i].categoryid + "','" + myDeserializedObjList[i].categoryname + "','" + myDeserializedObjList[i].subcategoryid + "','" + myDeserializedObjList[i].subcategoryname + "',1,1,current_timestamp);";
                        }


                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuerytree);
                    }


                    isSaved = true;
                }
            }
            catch (Exception ex)
            {



                isSaved = false;
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();

                }
            }
            return isSaved;

        }

        #endregion




        #region DeliveryBoy
        public List<DeliveryBoyDTO> ShowDeliverboydetailsbyid()
        {
            List<DeliveryBoyDTO> lstDeliveryBoyDTO = new List<DeliveryBoyDTO>();
            try
            {
                string strInsert = "select vchDeliveryboyid,vchemployeetype,vchname,nummobileno,vchvehicleno from tabrmsDeliveryBoyDetails where  STATUSID=1 ORDER BY Deliveryboyid desc;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                while (npgdr.Read())
                {
                    DeliveryBoyDTO objDeliveryBoyDTO = new DeliveryBoyDTO();

                    objDeliveryBoyDTO.DeliveryBoyId = npgdr["vchDeliveryboyid"].ToString();
                    objDeliveryBoyDTO.EmployeeType = npgdr["vchemployeetype"].ToString();
                    objDeliveryBoyDTO.BoyName = npgdr["vchname"].ToString();
                    objDeliveryBoyDTO.MobileNo = npgdr["nummobileno"].ToString();
                    objDeliveryBoyDTO.VehicleNo = npgdr["vchvehicleno"].ToString();
                    lstDeliveryBoyDTO.Add(objDeliveryBoyDTO);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeliveryBoyDetails");
                throw ex;
                // ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstDeliveryBoyDTO;

        }
        public int UpdateDeliverboydetails(DeliveryBoyDTO DeliveryBoyDTO)
        {


            int Count = 0;

            int Count1 = 0;
            int Count2 = 0;
            int Count3 = 0;





            // int Count = 0;
            // int res = 0;
            // int getcount = 0;
            string DobDetails = null;
            try
            {


                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();

                    string strname = "SELECT vchname||'-'||nummobileno||'-'||vchvehicleno as details from tabrmsdeliveryboydetails where vchdeliveryboyid='" + DeliveryBoyDTO.DeliveryBoyId + "'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(strname, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {


                            DobDetails = npgdr["details"].ToString();


                        }
                    }
                }




                string check = "select COUNT(*) from tabposhdkot where upper(vchdeliveryboyderails)='" + ManageQuote(DobDetails.Trim().ToUpper()) + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check));

                if (Count == 0)
                {

                    string checknumandvehicleno = "select count(*) from tabrmsdeliveryboydetails where nummobileno=" + DeliveryBoyDTO.MobileNo + " and vchvehicleno='" + DeliveryBoyDTO.VehicleNo + "' and statusid=1 and vchdeliveryboyid not in('" + DeliveryBoyDTO.DeliveryBoyId + "') ;";
                    Count1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, checknumandvehicleno));
                    if (Count1 == 0)
                    {



                        string checkmobileno = "select count(*) from tabrmsdeliveryboydetails where nummobileno=" + DeliveryBoyDTO.MobileNo + " and statusid=1 and vchdeliveryboyid not in('" + DeliveryBoyDTO.DeliveryBoyId + "');";


                        Count2 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, checkmobileno));

                        string checkVehicleno = "select count(*) from tabrmsdeliveryboydetails where vchvehicleno='" + DeliveryBoyDTO.VehicleNo + "' and statusid=1 and vchdeliveryboyid not in('" + DeliveryBoyDTO.DeliveryBoyId + "');";


                        Count3 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, checkVehicleno));

                        if (Count2 == 0 && Count3 == 0)
                        {

                            string strInsert = "UPDATE tabrmsdeliveryboydetails SET nummobileno='" + DeliveryBoyDTO.MobileNo + "',vchvehicleno='" + DeliveryBoyDTO.VehicleNo + "',modifieddate=current_timestamp,modifiedby=1 WHERE vchDeliveryboyid='" + DeliveryBoyDTO.DeliveryBoyId + "';";
                            NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);

                        }
                    }

                }




            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeliveryBoyDetails");
                throw;
            }

            return Count + Count1 + Count2 + Count3;
        }
        public bool DeleteDeliverboydetails(string ID)
        {
            bool IsValid = false;

            string DobDetails = null;
            try
            {

                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();

                    string strname = "SELECT vchname||'-'||nummobileno||'-'||vchvehicleno as details from tabrmsdeliveryboydetails where vchdeliveryboyid='" + ID + "'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(strname, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {


                            DobDetails = npgdr["details"].ToString();


                        }
                    }
                }




                string check_exist = "SELECT COUNT(*) FROM tabposhdkot  WHERE  upper(vchdeliveryboyderails)='" + ManageQuote(DobDetails.Trim().ToUpper()) + "' and statusid=1";
                int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));

                //int res = ItemcategoryCheck(DobDetails, "tabposhdkot", "vchdeliveryboyderails");
                if (res == 0)
                {

                    string strDelete = "UPDATE tabrmsdeliveryboydetails SET STATUSID=2 WHERE vchDeliveryboyid='" + ID + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);
                    IsValid = true;
                }


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeliveryBoyDetails");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }
        public bool SaveDeliveryBoyDetails(DeliveryBoyDTO DeliveryBoyDTO, int createdby, int statusid, string Employeetype)
        {
            bool IssaveDetails = false;
            string strsaveDetails = string.Empty;
            int Count = 0;
            int Count1 = 0;
            int Count2 = 0;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();




                //string check_exist = "SELECT COUNT(vchname) FROM tabrmsdeliveryboydetails  WHERE  upper(vchname)='" + ManageQuote(DeliveryBoyDTO.BoyName.Trim().ToUpper()) + "' and statusid=1";
                //Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));


                string check_existmobileno = "SELECT COUNT(nummobileno) FROM tabrmsdeliveryboydetails  WHERE  nummobileno=" + DeliveryBoyDTO.MobileNo + " and statusid=1";
                Count1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_existmobileno));


                string check_existvehicleno = "SELECT COUNT(vchvehicleno) FROM tabrmsdeliveryboydetails  WHERE upper(vchvehicleno)='" + DeliveryBoyDTO.VehicleNo.ToUpper() + "' and statusid=1";
                Count2 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_existvehicleno));



                if (Count1 == 0 && Count2 == 0)
                {

                    string GetId = "select substring(vchDeliveryboyid,4)::int +1 from tabrmsDeliveryBoyDetails order by Deliveryboyid desc limit 1;";
                    int ID = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, GetId));

                    string BoyId = "DBD" + ID;

                    string strSaveDeliveryBoyDetails = "INSERT INTO tabrmsdeliveryboydetails(vchDeliveryboyid,vchemployeetype,vchname,nummobileno,vchvehicleno,statusid,createdby,createddate) VALUES('" + BoyId + "','" + ManageQuote(Employeetype) + "','" + ManageQuote(DeliveryBoyDTO.BoyName) + "'," + Convert.ToInt64(DeliveryBoyDTO.MobileNo) + ",'" + ManageQuote(DeliveryBoyDTO.VehicleNo) + "'," + statusid + "," + createdby + ",current_timestamp) ";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strSaveDeliveryBoyDetails);
                    IssaveDetails = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;

                EventLogger.WriteToErrorLog(ex, "DeliveryBoyDetails");
            }
            finally
            {

                con.Close();

            }
            return IssaveDetails;
        }




        //public bool SaveTableDetails(string TableName, int Createdby)
        //{
        //    bool IssaveDetails = false;
        //    string strsaveDetails = string.Empty;
        //    List<String> listStrtablename;

        //    try
        //    {
        //        con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        //        if (con.State != ConnectionState.Open)
        //        {
        //            con.Open();
        //        }

        //        listStrtablename = TableName.Split(',').ToList();
        //        // string date = FormatDate(System.DateTime.Now.ToString());
        //        for (int i = 0; i < listStrtablename.Count; i++)
        //        {
        //            string savedetailsTable = "insert into tabpostablesandcoversmstmaster(tablesname,hostid,datfromdate,dattodate,createddate) values(" + listStrtablename[i] + "," + Createdby + ",'" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "',current_timestamp);";
        //            NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, savedetailsTable);
        //            strsaveDetails = "update tabpostablesandcoversmst set modifieddate=current_timestamp,hostid=" + Createdby + " where tablesname in(" + listStrtablename[i] + ")";
        //            NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strsaveDetails);

        //        }
        //        IssaveDetails = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;

        //        //EventLogger.WriteToErrorLog(ex, "DeliveryBoyDetails");
        //    }
        //    finally
        //    {

        //        con.Close();

        //    }
        //    return IssaveDetails;
        //}




        //public bool updateTableDetails(int Createdby)
        //{
        //    bool IsUpdateDetails = false;
        //    string strupdateDetails = string.Empty;

        //    try
        //    {
        //        con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        //        if (con.State != ConnectionState.Open)
        //        {
        //            con.Open();
        //        }


        //        string strcount = "select count(*) from tabpostablesandcoversmst where status in('R','M') and hostid=" + Createdby + "";

        //        int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, strcount));

        //        if (count == 0)
        //        {

        //            strupdateDetails = "update tabpostablesandcoversmst set vchassignedstatus='N' where hostid=" + Createdby + "";
        //            NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strupdateDetails);
        //            IsUpdateDetails = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;

        //    }
        //    finally
        //    {
        //        con.Close();

        //    }
        //    return IsUpdateDetails;
        //}





        #endregion


    }
}






































