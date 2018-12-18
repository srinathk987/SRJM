using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMS.Core.Interfaces;
using System.Data;
using HelperManager;
using HRMS.Infrastructure;

namespace RMS.Infrastructure
{
    public class POSReportsRepository : IPOSReports
    {
        public DataSet BillGenerationReport(string BillNumber)
        {
            DataSet ds = new System.Data.DataSet();
            try
            {
                // string strqry = "select *,'' as categoryname from vwbillgeneration  where vchbillno ='" + BillNumber + "';";
                //string strqry = "select * from vwbillgeneration vw join tabpositemmst ti on vw.itemiE:\KAPILAGROSTORE-23-05-2017\RMS.Infrastructure\POSReportsRepository.csd=ti.itemid where vchbillno ='" + BillNumber + "';";
                string strqry = "select * from vwbillprint vw join tabpositemmst ti on vw.itemid=ti.itemid where vchbillno ='" + BillNumber + "';";

                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillGeneration");
                throw ex;

            }


            return ds;
        }
        public DataSet companyname()
        {
            DataSet ds1 = new DataSet();
            try
            {
                string strquery = "select vchcompanyname,(vchdoorno||','||vchstreet||','||vcharea) as address from tabcompany tc join tabbranch tb on tc.branchid=tb.recordid;";

                //string strquery = "select vchcompanyname,vcharea as address from tabcompany tc join tabbranch tb on tc.branchid=tb.recordid;";
                ds1 = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strquery);

            }
            catch (Exception ex)
            {

                EventLogger.WriteToErrorLog(ex, "Company Name");
            }
            finally
            {

            }
            return ds1;
        }


        #region Categorywise Report

        public DataTable GetCategorynames()
        {
            DataTable dt = new DataTable();
            string strdata = string.Empty;
            try
            {
                strdata = "select itemcategoryname,categoryid from tabpositemcategorymst WHERE statusid=1 order by itemcategoryname;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strdata).Tables[0];
                DataRow dr = dt.NewRow();
                dr["categoryid"] = 0;
                dr["itemcategoryname"] = "ALL";
                dt.Rows.InsertAt(dr, 0);
            }
            catch (Exception)
            {

                throw;
            }
            return dt;
        }

        public DataSet CategorywiseReport(DateTime fromdate, DateTime todate, string Category)
        {
            DataSet ds = new System.Data.DataSet();
            string strqry = string.Empty;
            try
            {

                if (Category == "0")
                {

                    strqry = "select billno, vchbillno, billdate, billtime, sessionid, tableid, vchkotid, itemid,itemname, amount,totalbillamount, totaldiscount,itemdicount, numservicetaxvale,numservicechargevale, vchdiscounttype, vchdiscount ,x.categoryid,y.itemcategoryname from (select billno, vw.vchbillno, billdate, vw.billtime, vw.sessionid,vw. tableid, vchkotid, vw.itemid,vw.itemname, amount,totalbillamount, totaldiscount,itemdicount, numservicetaxvale,numservicechargevale,vchdiscounttype, vchdiscount ,ti.categoryid from vwitemdiscountdetails vw  left join tabpositemmst ti on vw.itemid=ti.itemid )x left join tabpositemcategorymst y on x.categoryid=y.categoryid where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "'";
                }
                else
                {
                    strqry = "select billno, vchbillno, billdate, billtime, sessionid, tableid, vchkotid, itemid,itemname, amount,totalbillamount, totaldiscount,itemdicount, numservicetaxvale,numservicechargevale, vchdiscounttype, vchdiscount ,x.categoryid,y.itemcategoryname from (select billno, vw.vchbillno, billdate, vw.billtime, vw.sessionid,vw. tableid, vchkotid, vw.itemid,vw.itemname, amount,totalbillamount, totaldiscount,itemdicount, numservicetaxvale,numservicechargevale,vchdiscounttype, vchdiscount ,ti.categoryid from vwitemdiscountdetails vw  left join tabpositemmst ti on vw.itemid=ti.itemid )x left join tabpositemcategorymst y on x.categoryid=y.categoryid where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' and y.categoryid=" + Category + ";";
                }
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Category Report");
                throw ex;

            }


            return ds;
        }

        #endregion

        #region subCategorywise Report

        public DataTable Getsubcategorynames(string categoryid)
        {
            DataTable dt = new DataTable();
            string strdata = string.Empty;
            try
            {
                strdata = "select itemsubcategoryname,subcategoryid from tabpositemsubcategorymst WHERE categoryid=" + categoryid + " and statusid=1 order by itemsubcategoryname;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strdata).Tables[0];
                DataRow dr = dt.NewRow();
                dr["subcategoryid"] = 0;
                dr["itemsubcategoryname"] = "ALL";
                dt.Rows.InsertAt(dr, 0);
            }
            catch (Exception)
            {

                throw;
            }
            return dt;
        }

        public DataSet subCategorywiseReport(DateTime fromdate, DateTime todate, string CategoryName, string SubCategory)
        {
            DataSet ds = new System.Data.DataSet();
            string strqry = string.Empty;
            try
            {
                if (CategoryName != "0" && CategoryName != "undefined" && SubCategory != "0" && SubCategory != "undefined" && SubCategory != "ALL")
                {
                    //string strqry = " select billno, vchbillno, billdate, billtime, sessionid, tableid, vchkotid, itemid,itemname, amount,totalbillamount, totaldiscount,itemdicount, numservicetaxvale,numservicechargevale,numvoucherdiscountpercentage, numvoucherdiscountamount, vchdiscounttype, vchdiscount ,x.categoryid,y.itemsubcategoryname from (select billno, tbs.vchbillno, billdate, vw.billtime, vw.sessionid,vw. tableid, vchkotid, vw.itemid,vw.itemname, amount,totalbillamount, totaldiscount,itemdicount, numservicetaxvale,numservicechargevale,numvoucherdiscountpercentage, numvoucherdiscountamount, vchdiscounttype, vchdiscount ,ti.categoryid from vwitemdiscountdetails vw join tabposbillsettlement tbs on vw.vchbillno=tbs.vchbillno left join tabpositemmst ti on vw.itemid=ti.itemid )x left join tabpositemsubcategorymst y on x.categoryid=y.categoryid ; ";
                    //string strqry = "select billno, vchbillno, billdate, billtime, sessionid, tableid, vchkotid, itemid,itemname, amount,totalbillamount, totaldiscount,itemdicount, numservicetaxvale,numservicechargevale,vchdiscounttype, vchdiscount ,x.categoryid,y.itemsubcategoryname from (select billno, vw.vchbillno, billdate, vw.billtime, vw.sessionid,vw. tableid, vchkotid, vw.itemid,vw.itemname, amount,totalbillamount, totaldiscount,itemdicount, numservicetaxvale,numservicechargevale, vchdiscounttype, vchdiscount ,ti.categoryid from vwitemdiscountdetails vw  left join tabpositemmst ti on vw.itemid=ti.itemid )x left join tabpositemsubcategorymst y on x.categoryid=y.categoryid ;";
                    strqry = "select vw.itemid,y.itemsubcategoryname,billno, vw.vchbillno, billdate, vw.billtime, vw.sessionid,vw.itemqty,vw. tableid, vchkotid, vw.itemid,vw.itemname, amount,totalbillamount,  totaldiscount,itemdicount, numservicetaxvale,numservicechargevale, vchdiscounttype, vchdiscount ,y.categoryid,y.itemcategoryname from vwitemdiscountdetails vw left join (select itemid,z.itemsubcategoryname,z.categoryid,x.categoryname as itemcategoryname,z.subcategoryid from tabpositemmst x left join tabpositemsubcategorymst z on x.categoryid=z.categoryid and x.subcategoryid=z.subcategoryid)y on y.itemid=vw.itemid where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' and categoryid=" + CategoryName + " and subcategoryid='" + SubCategory + "';";
                }
                else if (CategoryName == "0" && SubCategory == "0")
                {
                    strqry = "select vw.itemid,y.itemsubcategoryname,billno, vw.vchbillno, billdate, vw.billtime, vw.sessionid,vw.itemqty,vw. tableid, vchkotid, vw.itemid,vw.itemname, amount,totalbillamount,  totaldiscount,itemdicount, numservicetaxvale,numservicechargevale, vchdiscounttype, vchdiscount ,y.categoryid,y.itemcategoryname from vwitemdiscountdetails vw left join (select itemid,z.itemsubcategoryname,z.categoryid,x.categoryname as itemcategoryname,z.subcategoryid from tabpositemmst x left join tabpositemsubcategorymst z on x.categoryid=z.categoryid and x.subcategoryid=z.subcategoryid)y on y.itemid=vw.itemid where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "';";
                }
                else if (CategoryName != "0" && SubCategory == "0")
                {
                    strqry = "select vw.itemid,y.itemsubcategoryname,billno, vw.vchbillno, billdate, vw.billtime, vw.sessionid,vw.itemqty,vw. tableid, vchkotid, vw.itemid,vw.itemname, amount,totalbillamount,  totaldiscount,itemdicount, numservicetaxvale,numservicechargevale, vchdiscounttype, vchdiscount ,y.categoryid,y.itemcategoryname from vwitemdiscountdetails vw left join (select itemid,z.itemsubcategoryname,z.categoryid,x.categoryname as itemcategoryname,z.subcategoryid from tabpositemmst x left join tabpositemsubcategorymst z on x.categoryid=z.categoryid and x.subcategoryid=z.subcategoryid)y on y.itemid=vw.itemid where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' and categoryid=" + CategoryName + ";";
                }
                else if (CategoryName == "0" && SubCategory != "0")
                {
                    strqry = "select vw.itemid,y.itemsubcategoryname,billno, vw.vchbillno, billdate, vw.billtime, vw.sessionid,vw.itemqty,vw. tableid, vchkotid, vw.itemid,vw.itemname, amount,totalbillamount,  totaldiscount,itemdicount, numservicetaxvale,numservicechargevale, vchdiscounttype, vchdiscount ,y.categoryid,y.itemcategoryname from vwitemdiscountdetails vw left join (select itemid,z.itemsubcategoryname,z.categoryid,x.categoryname as itemcategoryname,z.subcategoryid from tabpositemmst x left join tabpositemsubcategorymst z on x.categoryid=z.categoryid and x.subcategoryid=z.subcategoryid)y on y.itemid=vw.itemid where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' and subcategoryid='" + SubCategory + "';";
                }
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SubCategory Report");
                throw ex;

            }


            return ds;
        }

        #endregion

        #region ItemwiseSaleReport

        public DataTable GetItemnames()
        {
            DataTable dt = new DataTable();
            string strdata = string.Empty;
            try
            {
                strdata = "select itemid,itemname from tabpositemmst where statusid=1 order by itemname;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strdata).Tables[0];
                DataRow dr = dt.NewRow();
                dr["itemid"] = 0;
                dr["itemname"] = "ALL";
                dt.Rows.InsertAt(dr, 0);
            }
            catch (Exception)
            {

                throw;
            }
            return dt;
        }

        public DataSet ItemwiseSaleReport(DateTime fromdate, DateTime todate, string item)
        {
            DataSet ds = new System.Data.DataSet();
            string strqry = string.Empty;
            try
            {
                if (item == "0")
                {

                    strqry = "select vw.itemid,y.itemsubcategoryname,billno, vw.vchbillno, billdate, vw.billtime, vw.sessionid,vw.itemqty,vw. tableid, vchkotid, vw.itemid,vw.itemname, amount,totalbillamount,  totaldiscount,itemdicount, numservicetaxvale,numservicechargevale, vchdiscounttype, vchdiscount ,y.categoryid from vwitemdiscountdetails vw left join (select itemid,z.itemsubcategoryname,z.categoryid from tabpositemmst x left join tabpositemsubcategorymst z on x.categoryid=z.categoryid and x.subcategoryid=z.subcategoryid)y on y.itemid=vw.itemid where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "';";
                }
                else
                {
                    strqry = "select vw.itemid,y.itemsubcategoryname,billno, vw.vchbillno, billdate, vw.billtime, vw.sessionid,vw.itemqty,vw. tableid, vchkotid, vw.itemid,vw.itemname, amount,totalbillamount,  totaldiscount,itemdicount, numservicetaxvale,numservicechargevale, vchdiscounttype, vchdiscount ,y.categoryid from vwitemdiscountdetails vw left join (select itemid,z.itemsubcategoryname,z.categoryid from tabpositemmst x left join tabpositemsubcategorymst z on x.categoryid=z.categoryid and x.subcategoryid=z.subcategoryid)y on y.itemid=vw.itemid where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' and vw.itemid=" + item + ";";
                }

                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Itemwise Report");
                throw ex;

            }


            return ds;
        }

        #endregion


        public DataSet Kotslip(string kotno)
        {
            DataSet ds = new DataSet();
            try
            {
                string strquery = "select vk.sessionid,vk.vchkotid,vk.itemname,vk.vchhostname,vk.itemqty,ts.sessionname,tt.tablesname from vwkotdetails vk left join tabpossessionmst ts on vk.sessionid=ts.sessionid left join tabpostablesandcoversmst tt on vk.tableid=tt.tableid where vk.vchkotid='" + kotno + "'";
                //string strquery = "select vk.sessionid,vk.vchkotid,vk.itemname,vk.vchhostname,vk.itemqty,ts.sessionname,tt.tablesname from vwkotdetails vk left join tabpossessionmst ts on vk.sessionid=ts.sessionid left join tabpostablesandcoversmst tt on vk.tableid=tt.tableid where vk.vchkotid='KOT7'";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strquery);

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Kotslip Report");
            }
            finally
            {

            }
            return ds;
        }

        public DataSet KotChangeslip(string kotno)
        {
            DataSet ds = new DataSet();
            try
            {
                string strquery = "select vk.sessionid,vk.vchkotid,vk.itemname,vk.vchhostname,vk.itemqty ,tp.itemqty as prasentqty,ts.sessionname,tt.tablesname from vwkotdetails vk left join tabpossessionmst ts on vk.sessionid=ts.sessionid left join tabpostablesandcoversmst tt on vk.tableid=tt.tableid left join tabposkotchangedetails tp on vk.vchkotid=tp.vchkotid where vk.vchkotid='" + kotno + "'";
                //string strquery = "select vk.sessionid,vk.vchkotid,vk.itemname,vk.vchhostname,vk.itemqty,ts.sessionname,tt.tablesname from vwkotdetails vk left join tabpossessionmst ts on vk.sessionid=ts.sessionid left join tabpostablesandcoversmst tt on vk.tableid=tt.tableid where vk.vchkotid='KOT7'";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strquery);

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "KotChangeslip Report");
            }
            finally
            {

            }
            return ds;
        }

        #region SalesReport
        public DataTable GetCashierComboData()
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
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }
        public DataSet SalesGenerateReports(DateTime fromdate, DateTime todate, int userid)
        {
            DataSet ds = new System.Data.DataSet();
            try
            {
                //string strqry = "select vchbillno,(numpaidamount+numbalanceamount) as numpaidamount from tabposbillsettlement where datdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' and createdby=" + userid + ";";
                string strqry = "select distinct (totalbillamount-totaldiscount) as numpaidamount,billno,vchbillno from vwitemdiscountdetails  where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' and createdby=" + userid + ";";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillEdit");
                throw ex;

            }


            return ds;
        }


        public DataSet usernmaere(int userid)
        {
            DataSet ds1 = new DataSet();
            try
            {
                string strData = "select username from tabuserinfo where userid=" + userid + ";";


                ds1 = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData);

            }
            catch (Exception ex)
            {

                EventLogger.WriteToErrorLog(ex, "Company Name");
            }
            finally
            {

            }
            return ds1;
        }
        #endregion


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

        #region TableWiseReport

        public DataTable GetTablenames()
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "select tableid,tablesname from tabpostablesandcoversmst WHERE statusid=1 order by tableid;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
                DataRow dr = dt.NewRow();
                dr["tableid"] = 0;
                dr["tablesname"] = "ALL";

                dt.Rows.InsertAt(dr, 0);
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public DataSet TableGenerateReports(DateTime fromdate, DateTime todate, string tablename)
        {
            DataSet ds = new System.Data.DataSet();
            string strqry = string.Empty;
            try
            {
                if (tablename == "0")
                {
                    //strqry = "select tableid,tablesname,sectionname,round(amt,2) as amt,round(dis,2)as dis,round(tax,2) as tax,((amt-dis)+tax) as saleofamt from(select vw.tableid as tableid,tablesname,tab.sectionname as sectionname,sum(vw.amount) as amt,sum(vw.itemdicount) as dis,sum(vw.numservicetaxvale +vw.numservicechargevale) as tax from vwitemdiscountdetails vw join tabpostablesandcoversmst tab on vw.tableid=tab.tablesname where vw.billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' group by vw.tableid,tablesname,tab.sectionname) x  ";
                    strqry = "select tableid,tablesname,sectionname,round(amt,2) as amt,round(dis,2)as dis,round(tax,2) as tax,numservicechargevale,((amt-dis)+coalesce(tax,0)+coalesce(numservicechargevale,0)) as saleofamt from(select vw.tableid as tableid,tablesname,tab.sectionname as sectionname,sum(vw.amount) as amt,sum(vw.itemdicount) as dis,sum(vw.numservicetaxvale) as tax,sum(vw.numservicechargevale)as numservicechargevale from vwitemdiscountdetails vw join tabpostablesandcoversmst tab on vw.tableid=tab.tablesname where vw.billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' group by vw.tableid,tablesname,tab.sectionname) x ";
                }
                else
                {
                    //strqry = "select tableid,tablesname,sectionname,round(amt,2) as amt,round(dis,2)as dis,round(tax,2) as tax,((amt-dis)+tax) as saleofamt from(select vw.tableid as tableid,tablesname,tab.sectionname as sectionname,sum(vw.amount) as amt,sum(vw.itemdicount) as dis,sum(vw.numservicetaxvale +vw.numservicechargevale) as tax from vwitemdiscountdetails vw join tabpostablesandcoversmst tab on vw.tableid=tab.tablesname where vw.billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' and tab.tableid='" + tablename + "'  group by vw.tableid,tablesname,tab.sectionname) x  ";
                    strqry = "select tableid,tablesname,sectionname,round(amt,2) as amt,round(dis,2)as dis,round(tax,2) as tax,numservicechargevale,((amt-dis)+coalesce(tax,0)+coalesce(numservicechargevale,0)) as saleofamt from(select vw.tableid as tableid,tablesname,tab.sectionname as sectionname,sum(vw.amount) as amt,sum(vw.itemdicount) as dis,sum(vw.numservicetaxvale) as tax,sum(vw.numservicechargevale)as numservicechargevale from vwitemdiscountdetails vw join tabpostablesandcoversmst tab on vw.tableid=tab.tablesname where vw.billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' and tab.tableid='" + tablename + "' group by vw.tableid,tablesname,tab.sectionname) x ";
                }
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Tablewise Report");
                throw ex;

            }


            return ds;
        }
        #endregion
        #region Waiterwisereport
        public DataSet WaiterwiseGenerateReports(DateTime fromdate, DateTime todate)
        {
            DataSet ds = new System.Data.DataSet();
            try
            {
                //string strqry = "select vchhostname,numservicetaxvale,numservicechargevale,itemdicount,amount from vwitemdiscountdetails where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "';";
                string strqry = "select vchhostname,round(tax,2) as tax,round(dis,2) as dis,round(amt,2) as amt,round(((amt-dis)+tax+numservicechargevale),2) as saleofamt,numservicechargevale from(select vchhostname,sum(numservicetaxvale) as tax,sum(numservicechargevale) as numservicechargevale,sum(amount) as amt,sum(itemdicount) as dis from vwitemdiscountdetails where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' group by vchhostname)g group by vchhostname,tax,dis,amt,numservicechargevale";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillEdit");
                throw ex;

            }


            return ds;
        }
        #endregion


        #region WeeklyWiseReport

        public DataSet WeeklyWiseReport(string fromdate, string todate)
        {
            DataSet ds = new System.Data.DataSet();
            try
            {
                string strqry = "select vw.itemid, billno, vw.vchbillno, mdate as billdate, vchkotid, vw.itemid,vw.itemname,totaldiscount,itemdicount,vchdiscount,  amount-itemdicount+numservicechargevale+numservicetaxvale as amount,'Amt' as subname  from vwitemdiscountdetails vw  Right join (select  generate_series('" + fromdate + "','" + todate + "', '1 day'::interval)::date  as mdate) z on vw.billdate=z.mdate union all select vw.itemid, billno, vw.vchbillno, mdate as billdate, vchkotid, vw.itemid,vw.itemname, totaldiscount,itemdicount,vchdiscount,itemqty as amount,'Qty' from vwitemdiscountdetails vw   Right join (select  generate_series('" + fromdate + "','" + todate + "', '1 day'::interval)::date  as mdate) z on vw.billdate=z.mdate; ";
                //string strqry = "select vw.itemid, billno, vw.vchbillno, mdate as billdate, vchkotid, vw.itemid,vw.itemname,  amount-itemdicount+numservicechargevale+numservicetaxvale as amount,totaldiscount,itemdicount,vchdiscount from vwitemdiscountdetails vw   Right join (select  generate_series('" + fromdate + "','" + todate + "', '1 day'::interval)::date  as mdate) z on vw.billdate=z.mdate;";
                // string strqry = "select vw.itemid, billno, vw.vchbillno, billdate, vchkotid, vw.itemname,  amount-itemdicount+numservicechargevale+numservicetaxvale as amount,totaldiscount,itemdicount,vchdiscount from vwitemdiscountdetails vw  union all select  0 as itemid,0 as billno,'' as vchbillno, generate_series('" + fromdate + "','" + todate + "', '1 day'::interval)::date  as billdate,'' as vchkotid, '' itemname, 0 as amount,0 as totaldiscount,0 as itemdicount,0 as vchdiscount    ";

                // string strqry = "select itemname,sum(amount)as amount,billdate from(select vw.itemid, billno, vw.vchbillno, mdate as billdate, vchkotid, vw.itemid,vw.itemname,  amount-itemdicount+numservicechargevale+numservicetaxvale as amount,totaldiscount,itemdicount,vchdiscount from vwitemdiscountdetails vw   Right join (select  generate_series('2016-03-22','2016-03-26', '1 day'::interval)::date  as mdate) z on vw.billdate=z.mdate)x group by itemname,billdate;";

                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "WeeklyWise Report");
                throw ex;

            }


            return ds;
        }

        #endregion

        #region SectionWiseSaleReport

        public DataTable GetSectionnames()
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "select sectionid,sectionname from tabpossectionmst WHERE statusid=1 order by sectionid;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
                DataRow dr = dt.NewRow();
                dr["sectionid"] = 0;
                dr["sectionname"] = "ALL";

                dt.Rows.InsertAt(dr, 0);
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }
        public DataSet SectionWiseSaleReport(DateTime fromdate, DateTime todate, string section)
        {
            DataSet ds = new System.Data.DataSet();
            string strqry = string.Empty;
            try
            {
                if (section == "0")
                {
                    //string strqry = "select vw.itemid,y.itemcategoryname,billno, vw.vchbillno,vw. tableid, vchkotid, vw.itemid,vw.itemname, amount,totalbillamount, totaldiscount,itemdicount, numservicetaxvale,numservicechargevale, vchdiscounttype, vchdiscount ,y.categoryid,s.sectionid,s.sectionname from vwitemdiscountdetails vw  left join (select itemid,z.itemcategoryname,z.categoryid from tabpositemmst x left join tabpositemcategorymst z on x.categoryid=z.categoryid and  x.categoryid=z.categoryid)y on y.itemid=vw.itemid left join tabpostablesandcoversmst s on s.tableid=vw.tableid;";
                    strqry = "select vw.itemid,y.itemcategoryname,billno, vw.vchbillno,vw. tableid, vchkotid, vw.itemid,vw.itemname, amount,totalbillamount, totaldiscount,itemdicount, numservicetaxvale,numservicechargevale, vchdiscounttype, vchdiscount ,y.categoryid,s.sectionid,s.sectionname from vwitemdiscountdetails vw  left join (select itemid,z.itemcategoryname,z.categoryid from tabpositemmst x left join tabpositemcategorymst z on x.categoryid=z.categoryid and  x.categoryid=z.categoryid)y on y.itemid=vw.itemid left join tabpostablesandcoversmst s on s.tablesname=vw.tableid where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "';";
                }
                else
                {
                    strqry = "select vw.itemid,y.itemcategoryname,billno, vw.vchbillno,vw. tableid, vchkotid, vw.itemid,vw.itemname, amount,totalbillamount, totaldiscount,itemdicount, numservicetaxvale,numservicechargevale, vchdiscounttype, vchdiscount ,y.categoryid,s.sectionid,s.sectionname from vwitemdiscountdetails vw  left join (select itemid,z.itemcategoryname,z.categoryid from tabpositemmst x left join tabpositemcategorymst z on x.categoryid=z.categoryid and  x.categoryid=z.categoryid)y on y.itemid=vw.itemid left join tabpostablesandcoversmst s on s.tablesname=vw.tableid where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' and sectionid=" + section + ";";
                }
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SectionWiseSale Report");
                throw ex;

            }


            return ds;
        }


        #endregion


        #region ItemCancelGenerateReports
        public DataSet ItemCancelGenerateReports(DateTime fromdate, DateTime todate)
        {
            DataSet ds = new System.Data.DataSet();
            try
            {
                string strqry = "select vw.vchkotid,vw.tableid,tab.tablesname,vw.itemname,vw.itemqty,vw.amount,vw.reason,vw.canceldate,vw.vchhostname,vw.ordertime as kotordertime,vw.canceltime as kotcanceltime from vwitemcancel  vw join tabpostablesandcoversmst tab on vw.tableid=tab.tableid where vw.canceldate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' order by tableid,itemname;";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ItemCancel");
                throw ex;
            }
            return ds;
        }
        #endregion

        #region BillWise Sale Report

        public DataTable GetBillNos()
        {
            DataTable dt = new DataTable();
            string strdta = string.Empty;
            try
            {
                strdta = "select vchbillno,recordid from tabposbillsettlement WHERE statusid=1 order by recordid;";
                // strdta = "select vchbillno,recordid,regexp_replace(vchbillno, '[^0-9]+', '')::text as ordernumber from tabposbillsettlement WHERE statusid=1 order by ordernumber;";
                //strdta = "select vchbillno,recordid,replace(vchbillno,'B','')::int as ordernumber from tabposbillsettlement WHERE statusid=1 order by ordernumber;";
                //strdta = "select vchbillno,recordid,replace(vchbillno,'B','')::int as ordernumber from tabposbillsettlement WHERE statusid=1 order by ordernumber;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strdta).Tables[0];
                DataRow dr = dt.NewRow();
                dr["recordid"] = 0;
                dr["vchbillno"] = "ALL";
                dt.Rows.InsertAt(dr, 0);

            }
            catch (Exception ex)
            {

                throw;
            }
            return dt;

        }
        public DataSet BillwiseReport(DateTime fromdate, DateTime todate, string billno)
        {

            DataSet ds = new System.Data.DataSet();
            string strqry = string.Empty;
            try
            {
                if (billno == "ALL")
                {
                    //strqry = "select billno,billdate,vchbillno,tablesname,totalbillamount,replace(vchbillno ,'B','')::int as ordernumber,numservicetaxvale,numservicechargevale,itemdicount as totaldiscount,vchhostname,round((totalbillamount+numservicetaxvale+numservicechargevale-itemdicount),2) as withtaxsale,round((totalbillamount+numservicetaxvale+numservicechargevale-itemdicount),0) as Roundedsale from(select  billno,billdate,vchbillno,tablesname,sum(amount) as totalbillamount,sum(coalesce(itemdicount,0)) as itemdicount,sum(coalesce(numservicetaxvale,0)) as numservicetaxvale,sum(coalesce(numservicechargevale,0)) as numservicechargevale,vchhostname from vwitemdiscountdetails vw join tabpostablesandcoversmst tab on vw.tableid=tab.tableid  where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' group by billno,billdate,vchbillno,tablesname,totalbillamount,vchhostname ) g order by ordernumber;";
                    //strqry = "select billno,billdate,vchbillno,tablesname,totalbillamount,numservicetaxvale,numservicechargevale,itemdicount as totaldiscount,vchhostname,round((totalbillamount+numservicetaxvale+numservicechargevale-itemdicount),2) as withtaxsale,round((totalbillamount+numservicetaxvale+numservicechargevale-itemdicount),0) as Roundedsale from(select  billno,billdate,vchbillno,tablesname,sum(amount) as totalbillamount,sum(coalesce(itemdicount,0)) as itemdicount,sum(coalesce(numservicetaxvale,0)) as numservicetaxvale,sum(coalesce(numservicechargevale,0)) as numservicechargevale,vchhostname from vwitemdiscountdetails vw join tabpostablesandcoversmst tab on vw.tableid=tab.tablesname  where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' group by billno,billdate,vchbillno,tablesname,totalbillamount,vchhostname ) g order by billno;";
                    strqry = "select billno,billdate,vchbillno,tablesname,totalbillamount,numservicetaxvale,numservicechargevale,itemdicount as totaldiscount,vchhostname,round((totalbillamount+numservicetaxvale+numservicechargevale-itemdicount),2) as withtaxsale,round((totalbillamount+numservicetaxvale+numservicechargevale-itemdicount),0) as Roundedsale,billtype from(select  billno,billdate,vchbillno,tablesname,sum(amount) as totalbillamount,sum(coalesce(itemdicount,0)) as itemdicount,sum(coalesce(numservicetaxvale,0)) as numservicetaxvale,sum(coalesce(numservicechargevale,0)) as numservicechargevale,vchhostname,case when vchbillno like 'B%' then 'B'  when vchbillno like 'T%' then 'T' else 'H' end as  billtype from vwitemdiscountdetails vw join tabpostablesandcoversmst tab on vw.tableid=tab.tablesname  where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' group by billno,billdate,vchbillno,tablesname,totalbillamount,vchhostname ) g order by billtype,billno;";
                }
                else
                {
                    //strqry = "select * from(select billno,billdate,vchbillno,tablesname,totalbillamount,numservicetaxvale,numservicechargevale,itemdicount as totaldiscount,vchhostname,round((totalbillamount+numservicetaxvale+numservicechargevale-itemdicount),2) as withtaxsale,round((totalbillamount+numservicetaxvale+numservicechargevale-itemdicount),0) as Roundedsale from(select  billno,billdate,vchbillno,tablesname,sum(amount) as totalbillamount,sum(coalesce(itemdicount,0)) as itemdicount,sum(coalesce(numservicetaxvale,0)) as numservicetaxvale,sum(coalesce(numservicechargevale,0)) as numservicechargevale,vchhostname from vwitemdiscountdetails vw join tabpostablesandcoversmst tab on vw.tableid=tab.tablesname  where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' group by billno,billdate,vchbillno,tablesname,totalbillamount,vchhostname ) g  order by billno)x where vchbillno='" + billno + "';";
                    strqry = "select * from(select billno,billdate,vchbillno,tablesname,totalbillamount,numservicetaxvale,numservicechargevale,itemdicount as totaldiscount,vchhostname,round((totalbillamount+numservicetaxvale+numservicechargevale-itemdicount),2) as withtaxsale,round((totalbillamount+numservicetaxvale+numservicechargevale-itemdicount),0) as Roundedsale from(select  billno,billdate,vchbillno,tablesname,sum(amount) as totalbillamount,sum(coalesce(itemdicount,0)) as itemdicount,sum(coalesce(numservicetaxvale,0)) as numservicetaxvale,sum(coalesce(numservicechargevale,0)) as numservicechargevale,vchhostname from vwitemdiscountdetails vw join tabpostablesandcoversmst tab on vw.tableid=tab.tablesname  where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' group by billno,billdate,vchbillno,tablesname,totalbillamount,vchhostname ) g  order by billno)x where vchbillno='" + billno + "';";
                }
                //string strqry = "select DISTINCT billno,billdate,vchbillno,tablesname,totalbillamount,replace(vchbillno ,'B','')::int as ordernumber,numservicetaxvale,totaldiscount,vchhostname,round((totalbillamount+numservicetaxvale),2) as withtaxsale,round((totalbillamount+numservicetaxvale),0) as Roundedsale from vwitemdiscountdetails vw join tabpostablesandcoversmst tab on vw.tableid=tab.tableid  where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' order by ordernumber;";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillWise Report");
                throw ex;

            }
            return ds;
        }

        #endregion



        #region ShiftWise Item Sale Report

        public DataTable GetSessionnames()
        {
            DataTable dt = new DataTable();
            string strdata = string.Empty;
            try
            {
                strdata = "select sessionid,sessionname from tabpossessionmst WHERE statusid=1 order by sessionid ;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strdata).Tables[0];
                DataRow dr = dt.NewRow();
                dr["sessionid"] = 0;
                dr["sessionname"] = "ALL";
                dt.Rows.InsertAt(dr, 0);
            }
            catch (Exception)
            {

                throw;
            }
            return dt;
        }

        public DataSet ShiftwiseItemSaleReport(DateTime fromdate, DateTime todate, string Shift)
        {
            DataSet ds = new System.Data.DataSet();
            string strqry = string.Empty;
            try
            {
                if (Shift == "0")
                {
                    strqry = " select itemid,itemname,sessionname,case when amount>0 then(amount+numservicetaxvale+numservicechargevale-itemdicount) else 0 end as itemqty,'Amount' as subname from vwitemdiscountdetails vw join tabpossessionmst s on s.sessionid=vw.sessionid where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' union all select itemid,itemname,sessionname,itemqty,'Quantity' from vwitemdiscountdetails vw join tabpossessionmst s on s.sessionid=vw.sessionid where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' order by itemname;";
                }
                else
                {
                    strqry = " select itemid,itemname,sessionname,case when amount>0 then(amount+numservicetaxvale+numservicechargevale-itemdicount) else 0 end as itemqty,'Amount' as subname from vwitemdiscountdetails vw join tabpossessionmst s on s.sessionid=vw.sessionid where  vw.sessionid='" + Shift + "' and billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' union all select itemid,itemname,sessionname,itemqty,'Quantity' from vwitemdiscountdetails vw join tabpossessionmst s on s.sessionid=vw.sessionid where  vw.sessionid='" + Shift + "' and billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' order by itemname;";
                }

                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillWise Report");
                throw ex;

            }
            return ds;
        }

        #endregion



        #region Item Consumption SaleReport

        public DataTable GetDepartmentnames()
        {
            DataTable dt = new DataTable();
            string strdata = string.Empty;
            try
            {
                strdata = "select departmentid,departmentname from tabposdepartmentmst where statusid=1 order by departmentname;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strdata).Tables[0];
                DataRow dr = dt.NewRow();
                dr["departmentid"] = 0;
                dr["departmentname"] = "ALL";
                dt.Rows.InsertAt(dr, 0);
            }
            catch (Exception)
            {

                throw;
            }
            return dt;
        }
        public DataSet ItemConsumptionSaleReport(DateTime fromdate, DateTime todate, string department)
        {
            DataSet ds = new System.Data.DataSet();
            string strqry = string.Empty;
            try
            {
                if (department == "0")
                {
                    strqry = "select vw.itemid,vw.itemname,sum(vw.itemqty) as itemqty,ti.departmentname from vwitemdiscountdetails vw join tabpositemmst ti on vw.itemid=ti.itemid where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' group by vw.itemid,vw.itemname,ti.departmentname order by vw.itemname;";
                    //  string strqry = "select x.*,y.itemid as itemid2,y.itemname as itemname2,y.itemqty as itemqty2,y.departmentname as departmentname2 from (select Row_Number() OVER(order  BY departmentname) as rowno,itemid,itemname,itemqty,departmentname,Rowid  from (select * from (select Row_Number() OVER(order  BY ti.departmentname) AS Rowid ,vw.itemid,vw.itemname,sum(vw.itemqty) as itemqty,ti.departmentname from vwitemdiscountdetails vw join tabpositemmst ti on vw.itemid=ti.itemid where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' group by vw.itemid,vw.itemname,ti.departmentname )a where Rowid % 2=1)x)x full join (select Row_Number() OVER(order  BY departmentname)as rowno,itemid,itemname,itemqty,departmentname,Rowid  from ( select * from (select Row_Number() OVER(order  BY ti.departmentname) AS Rowid ,vw.itemid,vw.itemname,sum(vw.itemqty) as itemqty,ti.departmentname from vwitemdiscountdetails vw join tabpositemmst ti on vw.itemid=ti.itemid where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' group by vw.itemid,vw.itemname,ti.departmentname )b where Rowid % 2=0)z)y on x.rowno=y.rowno and x.departmentname=y.departmentname";
                }
                else
                {
                    strqry = "select vw.itemid,vw.itemname,sum(vw.itemqty) as itemqty,ti.departmentname from vwitemdiscountdetails vw join tabpositemmst ti on vw.itemid=ti.itemid where departmentid='" + department + "' and billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' group by vw.itemid,vw.itemname,ti.departmentname order by vw.itemname;";
                }
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Item consumption Report");
                throw ex;

            }
            return ds;
        }

        #endregion


        #region BillNo Details Report


        public DataSet BillDetailsReport(string billno)
        {

            DataSet ds = new System.Data.DataSet();
            string strqry = string.Empty;
            try
            {
                if (billno == "ALL")
                {
                    strqry = "select DISTINCT billno,billdate,vw.vchbillno,itemname,itemqty,amount,itemdicount,vchkotid,totalbillamount,vchhostname,numservicetaxvale,numservicechargevale from vwitemdiscountdetails vw join tabposbillsettlement tab on vw.vchbillno=vw.vchbillno  order by billno;";
                }
                else
                {
                    strqry = "select * from(select DISTINCT billno,billdate,itemqty,vw.vchbillno,itemname,amount,itemdicount,vchkotid,totalbillamount,vchhostname,numservicetaxvale,numservicechargevale from vwitemdiscountdetails vw join tabposbillsettlement tab on vw.vchbillno=vw.vchbillno  order by billno) x where vchbillno='" + billno + "';";
                }
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillNo Details Report");
                throw ex;

            }
            return ds;
        }

        #endregion

        #region Complementary Report
        public DataSet ComplementarySaleReport(DateTime fromdate, DateTime todate)
        {

            DataSet ds = new System.Data.DataSet();
            try
            {

                string strqry = "select vchbillno,billdate,billtime,tab.tablesname,vchkotid,vw.tableid,itemid,itemname,itemrate,itemqty,(itemrate*itemqty) as amount from vwbillgeneration vw join tabpostablesandcoversmst tab on vw.tableid=tab.tablesname where vchitemchargableornot='Y' and billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' ;";

                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Item consumption Report");
                throw ex;

            }
            return ds;
        }

        #endregion

        #region Bill Reprint

        public DataTable GetBillNumbers(string Billtype)
        {
            DataTable dt = new DataTable();
            string strdta = string.Empty;
            try
            {
                if (Billtype == "Dinning")
                {
                    strdta = "select vchbillno,recordid from tabposbillsettlement WHERE statusid=1 and vchbillno like 'B%' order by recordid;";
                }
                else if (Billtype == "TakeAway")
                {
                    strdta = "select vchbillno,recordid from tabposbillsettlement WHERE statusid=1 and vchbillno like 'TB%' order by recordid;";
                }
                else if (Billtype == "HomeDelivery")
                {
                    strdta = "select vchbillno,recordid from tabposbillsettlement WHERE statusid=1 and vchbillno like 'HB%' order by recordid;";
                }

                //strdta = "select vchbillno,recordid,regexp_replace(vchbillno, '[^0-9]+', '')::text as ordernumber from tabposbillsettlement WHERE statusid=1 order by ordernumber;";
                // strdta = "select vchbillno,recordid,replace(vchbillno,'B','')::int as ordernumber from tabposbillsettlement WHERE statusid=1 order by ordernumber;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strdta).Tables[0];
            }
            catch (Exception ex)
            {

                throw;
            }
            return dt;

        }


        public DataSet BillReprintReport(string BillNumber, string Billtype)
        {
            DataSet ds = new System.Data.DataSet();
            try
            {
                string strqry = "select * from vwbillprint vw join tabpositemmst ti on vw.itemid=ti.itemid where vchbillno ='" + BillNumber + "';";
                //string strqry = "select billno,vchbillno,billdate,billtime,itemid,itemname,itemqty,itemrate,itemdiscount,numdiscount,numamount,numservicetaxvale,numservicechargevale,numgross,numnet from vwbillgeneration where vchbillno ='" + BillNumber + "';";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
                //if(ds.Tables[0].Rows.Count!=0)
                //{
                //    string insert = "INSERT INTO tabposbillprintdetails(vchbillno,billtype,billdate,createdby)VALUES ('"+BillNumber+"','"+Billtype+"',current_timestamp,1);";
                //     NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, insert);

                //}
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillGeneration");
                throw ex;

            }


            return ds;
        }


        public bool Billprintdetails(string Billno, string Billtype)
        {
            bool issaved = false;
            try
            {
                string insert = "INSERT INTO tabposbillprintdetails(vchbillno,billtype,billdate,createdby)VALUES ('" + Billno + "','" + Billtype + "',current_timestamp,1);";
                NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, insert);
                issaved = true;
            }
            catch (Exception)
            {

                throw;
            }
            return issaved;

        }
        #endregion


        #region DayWiseSale Report

        public DataSet DaywisesaleReport(DateTime fromdate, DateTime todate)
        {

            DataSet ds = new System.Data.DataSet();
            string strqry = string.Empty;
            try
            {
                strqry = " select * from vwdaywisesale where billdate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "';";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DayWiseSale Report");
                throw ex;

            }
            return ds;
        }
        #endregion


        # region MMSSalebillReport


        // Generate Bill on Sale 
        public DataSet SaleBillGenerationReport(string SaleNo)
        {
            DataSet ds = new DataSet();
            DataSet dss = new DataSet();            
            try
            {               
                string counttt = "SELECT count(*) FROM TABMMSCOMBOSALE WHERE SALENO='" + SaleNo + "' and statusid=1;";               
                string val = NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, counttt).ToString();
                if(val!="0")
                {                    
                    string Getcombo = "SELECT DISTINCT COUNT(DISTINCT COMBONAME) AS ITEMS, TC.COMBONAME AS VCHPRODUCTNAME,TC.DETAILSID,VCHQUANTITY AS NUMQUANTITY,TC.COMBOCOST AS NUMSALEAMOUNT,'NO.'::TEXT AS VCHUOM,TC.COMBOCOST AS NUMUNITRATE FROM TABMMSCOMBOSALE TC JOIN TABMMSSALEDETAILS TS ON TS.DETAILSID=TC.DETAILSID WHERE SALENO='" + SaleNo + "' and TC.statusid=1 GROUP BY TC.COMBONAME,VCHUOM,NUMUNITRATE,TC.COMBOCOST,VCHQUANTITY,TC.DETAILSID;";
                    dss = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, Getcombo);
                    string strGetDetails = "SELECT COUNT(DISTINCT VCHPRODUCTNAME) AS ITEMS, VCHPRODUCTNAME,SUM(NUMQUANTITY) AS NUMQUANTITY,SUM(tm.NUMSALEAMOUNT) AS NUMSALEAMOUNT,CASE WHEN VCHUOM ='KILOGRAMS' THEN 'KG' ELSE 'NO.' END AS VCHUOM,DETAILSID,NUMUNITRATE FROM TABMMSSALEDETAILS TM JOIN TABMMSSALE TS ON TS.RECORDID=TM.DETAILSID WHERE SALENO='" + SaleNo + "' and combocode='' GROUP BY VCHPRODUCTNAME,VCHUOM,DETAILSID,NUMUNITRATE;";
                    ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strGetDetails);
                    ds.Merge(dss);
                }
                else
                {
                    string strGtDetails = "SELECT COUNT(DISTINCT VCHPRODUCTNAME) AS ITEMS, VCHPRODUCTNAME,SUM(NUMQUANTITY) AS NUMQUANTITY,SUM(tm.NUMSALEAMOUNT) AS NUMSALEAMOUNT,CASE WHEN VCHUOM ='KILOGRAMS' THEN 'KG' ELSE 'NO.' END AS VCHUOM,DETAILSID,NUMUNITRATE FROM TABMMSSALEDETAILS TM JOIN TABMMSSALE TS ON TS.RECORDID=TM.DETAILSID WHERE SALENO='" + SaleNo + "' group by vchproductname,vchuom,detailsid,numunitrate";
                    ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strGtDetails);
                }           
            }
            catch(Exception Ex)
            {
                throw Ex;
            }            
            return ds;
        }
        //public DataSet Quantity(string SaleNo)
        //{
        //    string Quantity = string.Empty;
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        string strSaledetails = "select recordid from tabmmssale where saleno='" + SaleNo + "'";
        //        long id = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strSaledetails));
        //        string qntty = "select sum(numquantity) as sum,count(distinct vchproductname) as Items from tabmmssaledetails where detailsid=" + id + "";
        //        ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, qntty);

        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //    return ds;
        //}

        public string GetUsername(int m)
        {
            string uname = string.Empty;
            try
            {
                string strGetUname = "select username from tabuserinfo where userid=" + m + "";
                uname = NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strGetUname).ToString();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return uname;
        }

        #endregion

    }
}
