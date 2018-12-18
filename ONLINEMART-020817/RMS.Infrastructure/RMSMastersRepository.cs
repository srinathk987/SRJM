using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperManager;
using System.Data;
using RMS.Core;
using RMS.Core.Interfaces;
using Npgsql;

using System.Web.Script.Serialization;
using HRMS.Infrastructure;

namespace RMS.Infrastructure
{
    public class RMSMastersRepository : IRMSMasters
    {
        NpgsqlConnection con = null;
        NpgsqlDataReader npgdr = null;

        NpgsqlDataReader dr = null;

        NpgsqlTransaction trans = null;
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


        #region RMS
        public DataTable GetItemNames()
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "select itemcode,itemname from tabpositemmst  where statusid=1 and categoryname='FOOD' and itemname not in(select vchitemname from tabreceipemanagement);";


              //  strData = "select itemcode,itemname from tabpositemmst  where statusid=1 and itemname not in(select vchitemname from tabreceipemanagement);";

                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "RMS");
            }

            return dt;
        }




        public DataTable GetCategoryAndSubCategory(string ItemName)
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "select categoryname,subcategoryname from tabpositemmst where itemname='" + ItemName + "' and statusid=1;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "RMS");
            }

            return dt;
        }



        public DataTable GetProducts()
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "select productcode,productname from tabmmsproductmst where  statusid=1;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "RMS");
            }

            return dt;
        }


        public List<RmsDTO> ShowProductCategory(string ProductNames)
        {
            List<RmsDTO> lstRmsDTOProducts = new List<RmsDTO>();
            try
            {




                string Names = ProductNames;

                string Productwords = Names.Replace(",", "','");


                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand("select Productcode,productname,uomname from tabmmsproductmst where productname in('" + Productwords + "');", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            RmsDTO RmsDTOProducts = new RmsDTO();
                            RmsDTOProducts.ProductName = npgdr["productname"].ToString();
                            RmsDTOProducts.UOM = npgdr["uomname"].ToString();

                            RmsDTOProducts.ProductCode = npgdr["Productcode"].ToString();
                            RmsDTOProducts.ReceipeUOM = npgdr["uomname"].ToString();
                            RmsDTOProducts.ConversionUom = npgdr["uomname"].ToString();
                          //  RmsDTOProducts.GroupName = "Group - Product";
                            RmsDTOProducts.GroupName = "Product";

                            lstRmsDTOProducts.Add(RmsDTOProducts);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowProductCategory");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstRmsDTOProducts;

        }





        public DataTable GetItemsFromRms()
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "select vchrmsno,vchitemname from tabreceipemanagement where statusid=1;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "RMS");
            }

            return dt;
        }



        public string GenerateNextID(string strtablename, string strcolname, int prefix, string strdate, string strColumnDate, string strPrefix)
        {

            try
            {
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.StoredProcedure, "GENERATENEXTID('" + strtablename + "','" + strcolname + "','" + prefix + "','" + strdate + "','" + strColumnDate + "','" + strPrefix + "')", null);
                while (npgdr.Read())
                {
                    GNextID = npgdr[0].ToString();
                }
                npgdr.Close();


            }
            catch (Exception ex)
            {
                string excp = ex.Message.ToString();
                return null;
            }
            return GNextID;
        }


        public bool SaveRMSDetails(RmsDTO RmsDTOdetails, List<RmsDTO> lstRmsDTO)
        {

            bool isSaved = false;
            try
            {
                string strInsert = string.Empty;
                string strInsertDetails = string.Empty;

                string strNextID = string.Empty;

                long RecordID = 0;

                string date = DateTime.Now.ToString("yyyy-MM-dd");

                if (lstRmsDTO.Count > 0)
                {
                    con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    trans = con.BeginTransaction();
                    if (RmsDTOdetails!= null)
                    {

                        strNextID = GenerateNextID("tabreceipemanagement", "vchrmsno", 2, date, "datcreateddate", "RM");
                        strNextID = "RM" + strNextID;
                      


                        //string GetId = "select substring(vchrmsno,4)::int +1 from tabRMSManagement order by recordid desc limit 1;";
                        //int ID = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, GetId));

                        //strNextID = "RMS" + ID;

                        //trans = con.BeginTransaction();

                        string strgetitemid = "select itemcode from tabpositemmst where itemname='" + RmsDTOdetails.ItemName + "' and statusid=1 ";

                        using (NpgsqlCommand cmd = new NpgsqlCommand(strgetitemid, con))
                        {
                            npgdr = cmd.ExecuteReader();
                            while (npgdr.Read())
                            {


                                RmsDTOdetails.ItemId = npgdr["itemcode"].ToString();


                            }
                        }






                        strInsert = "INSERT INTO tabreceipemanagement(vchrmsno,datcreateddate,vchitemid,vchitemname,vchpreparationsteps,statusid,createdby,createddate)VALUES('" + strNextID + "',current_date,'" + ManageQuote(RmsDTOdetails.ItemId) + "','" + ManageQuote(RmsDTOdetails.ItemName) + "','" + ManageQuote(RmsDTOdetails.PreparationSteps) + "',1,1,CURRENT_TIMESTAMP) RETURNING RECORDID;";

                        RecordID = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strInsert));



                        for (int i = 0; i < lstRmsDTO.Count; i++)
                        {



                            strInsertDetails = "insert into tabreceipemanagementdetails(detailsid,vchrmsno,vchitemname,vchproductid,vchproductname,vchuom,vchreceipeuom,numqty,vchconversionuom,statusid,createdby,createddate,vchgroupname) values('" + RecordID + "','" + strNextID + "','" + ManageQuote(RmsDTOdetails.ItemName) + "','" + ManageQuote(lstRmsDTO[i].ProductCode) + "','" + ManageQuote(lstRmsDTO[i].ProductName) + "','" + ManageQuote(lstRmsDTO[i].UOM) + "','" + ManageQuote(lstRmsDTO[i].ReceipeUOM) + "'," + lstRmsDTO[i].Qty + ",'" + ManageQuote(lstRmsDTO[i].ConversionUom) + "',1,1,CURRENT_TIMESTAMP,'"+ManageQuote(lstRmsDTO[i].GroupName)+"');";


                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsertDetails);




                        }


                    }

                    else
                    {
                        for (int i = 0; i < lstRmsDTO.Count; i++)
                        {



                            strInsertDetails = "insert into tabreceipemanagementdetails(detailsid,vchrmsno,vchitemname,vchproductid,vchproductname,vchuom,vchreceipeuom,numqty,vchconversionuom,statusid,createdby,createddate) values(" + lstRmsDTO[i].Detailsid + ",'" + lstRmsDTO[i].RMSNO + "','" + lstRmsDTO[i].ItemName + "','" + ManageQuote(lstRmsDTO[i].ProductCode) + "','" + ManageQuote(lstRmsDTO[i].ProductName) + "','" + ManageQuote(lstRmsDTO[i].UOM) + "','" + ManageQuote(lstRmsDTO[i].ReceipeUOM) + "'," + lstRmsDTO[i].Qty + ",'" + ManageQuote(lstRmsDTO[i].ConversionUom) + "',1,1,CURRENT_TIMESTAMP);";


                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsertDetails);




                        }

                    }







                }

                trans.Commit();
                isSaved = true;





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
                }

            }
            return isSaved;
        }

        public List<RmsDTO> ShowItmCategory(string ItmNames)
        {
            List<RmsDTO> lstRmsDTOProducts = new List<RmsDTO>();
            try
            {


                string ProductName = string.Empty;

                string Names = ItmNames;
                string INames = string.Empty;

                string ItmWords = Names.Replace(",", "','");


                string RmsNos = null;
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                string RmsNO = "select vchrmsno from tabreceipemanagement where vchitemname in('" + ItmWords + "');";
                using (NpgsqlCommand cmd = new NpgsqlCommand(RmsNO, con))
                {
                    npgdr = cmd.ExecuteReader();
                    while (npgdr.Read())
                    {

                        RmsNos = RmsNos + "," + npgdr["vchrmsno"].ToString();
                        //RmsDTOdetails.ItemId = npgdr["itemcode"].ToString();


                    }



                }








                string Itemss = RmsNos.Substring(1);

                string ItmNo = Itemss.Replace(",", "','");









                using (con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                   // using (NpgsqlCommand cmd = new NpgsqlCommand("select detailsid,vchrmsno,vchitemname,vchproductid,vchproductname,vchuom,vchreceipeuom,numqty,vchconversionuom from tabreceipemanagementdetails where vchrmsno in('" + ItmNo + "');", con))

                    using (NpgsqlCommand cmd = new NpgsqlCommand("select detailsid,vchrmsno,vchitemname,vchproductid,vchproductname,vchuom,vchreceipeuom,numqty,vchconversionuom,vchpreparationsteps from vwreceiprmanagement where vchrmsno in('" + ItmNo + "');", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            RmsDTO RmsDTOProducts = new RmsDTO();
                            RmsDTOProducts.ProductName = npgdr["vchproductname"].ToString();
                            RmsDTOProducts.UOM = npgdr["vchuom"].ToString();

                           // string Name=npgdr["vchgroupname"].ToString();

                            RmsDTOProducts.ProductCode = npgdr["vchproductid"].ToString();
                            RmsDTOProducts.ReceipeUOM = npgdr["vchreceipeuom"].ToString();
                            RmsDTOProducts.ConversionUom = npgdr["vchconversionuom"].ToString();

                            RmsDTOProducts.Qty = Convert.ToInt32(npgdr["numqty"]);//.ToString();

                            RmsDTOProducts.ItemName = npgdr["vchitemname"].ToString();
                            RmsDTOProducts.RMSNO = npgdr["vchrmsno"].ToString();

                            RmsDTOProducts.Detailsid = Convert.ToInt32(npgdr["detailsid"].ToString());
                            RmsDTOProducts.PreparationSteps = npgdr["vchpreparationsteps"].ToString();
                           // RmsDTOProducts.GroupName = "Group - Items" + RmsDTOProducts.RMSNO + "-" + RmsDTOProducts.ItemName;

                            RmsDTOProducts.GroupName = RmsDTOProducts.ItemName;

                            lstRmsDTOProducts.Add(RmsDTOProducts);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowProductCategory");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstRmsDTOProducts;

        }

        #endregion
















    }
}
