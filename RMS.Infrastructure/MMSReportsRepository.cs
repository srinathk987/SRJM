using RMS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using HelperManager;
using RMS.Core;
using HRMS.Infrastructure;

namespace RMS.Infrastructure
{
    public class MMSReportsRepository : IMMSReports
    {
        #region User Declaration

        NpgsqlDataReader npgdr = null;

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

        #endregion

        #region Purchase Order

        public DataTable ShowStorageLocation()
        {
            DataTable dt = new DataTable();
            string strQuery = string.Empty;
            try
            {
                strQuery = "select storagelocationid,storagelocationname from tabmmsstoragelocationmst where statusid=1 order by storagelocationname;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery).Tables[0];
                DataRow dr = dt.NewRow();
                dr["storagelocationid"] = 0;
                dr["storagelocationname"] = "ALL";
                dt.Rows.InsertAt(dr, 0);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowStorageLocation");
                throw ex;

            }
            return dt;

        }

        public DataTable ShowFromStorageLocation()
        {
            DataTable dt = new DataTable();
            string strQuery = string.Empty;
            try
            {
                strQuery = "select storagelocationid,storagelocationname from tabmmsstoragelocationmst where statusid=1 order by storagelocationname;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery).Tables[0];
                //DataRow dr = dt.NewRow();
                //dr["storagelocationid"] = 0;
                //dr["storagelocationname"] = "ALL";
                //dt.Rows.InsertAt(dr, 0);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowFromStorageLocation");
                throw ex;

            }
            return dt;

        }

        public DataTable ShowPONo(string strVendorid)
        {
            DataTable dt = new DataTable();
            string strdta = string.Empty;
            try
            {

                if (strVendorid == "" || strVendorid == "ALL" || strVendorid == "0")
                {
                    strdta = "select recordid,vchpurchaseorderno from tabmmspurchaseorder where statusid=1 order by recordid desc;";
                }
                else
                {
                    strdta = "select recordid,vchpurchaseorderno from tabmmspurchaseorder where vendorid='" + ManageQuote(strVendorid) + "' and statusid=1 order by recordid desc;";
                }
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strdta).Tables[0];
                //DataRow dr = dt.NewRow();
                //dr["recordid"] = 0;
                //dr["vchpurchaseorderno"] = "Select PO No.";
                //dt.Rows.InsertAt(dr, 0);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;

        }

        public DataSet GetPurchaseOrderDetailsByID(string PurchaseOrderNo, string vendorname)
        {
            DataSet ds = new DataSet();
            string strPO = string.Empty;
            try
            {
                //using (NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from vwmmspurchaseorder where vchpurchaseorderno='" + ManageQuote(PurchaseOrderNo) + "' and vchvendorname='" + ManageQuote(vendorname) + "'", NPGSqlHelper.SQLConnString))
                if (PurchaseOrderNo != "ALL" && PurchaseOrderNo != "" && vendorname != "ALL" && vendorname != "")
                {
                    strPO = "select orderid,tp.vchpurchaseorderno,datpurchaseorderdate,vchpurchaserequestno,vchpurchasequoteno,vchpurchaseordertype,vchrequestedby,vchapprovalby,tp.vchvendorname,vchcontactperson,vchcontactno,vchplaceofdelivery,vchterms,productid,productname,vchuom,vchorderuom,numorderedqty,numrate,datdeliverybefore,numorderconvertionqty,vchpovendorno,numtransportcharges, tpd.excisetaxamount,tpd.discountamount as productdiscountamount,tp.discountamount,tpd.cesstaxamount,tpd.shcesstaxamount,tpd.vatorcsttaxamount,basicamount,totalamount,vchuomid from tabmmspurchaseorder tp  left join tabmmspurchaseorderdetails tpd on tp.recordid=tpd.orderid left join tabinvuommst tbi on tbi.vchuomdescription=vchuom where tp.vchpurchaseorderno='" + ManageQuote(PurchaseOrderNo) + "' and tp.vchvendorname='" + ManageQuote(vendorname) + "'";
                }
                else if (PurchaseOrderNo != "ALL" && vendorname == "ALL")
                {
                    strPO = "select orderid,tp.vchpurchaseorderno,datpurchaseorderdate,vchpurchaserequestno,vchpurchasequoteno,vchpurchaseordertype,vchrequestedby,vchapprovalby,tp.vchvendorname,vchcontactperson,vchcontactno,vchplaceofdelivery,vchterms,productid,productname,vchuom,vchorderuom,numorderedqty,numrate,datdeliverybefore,numorderconvertionqty,vchpovendorno,numtransportcharges, tpd.excisetaxamount,tpd.discountamount as productdiscountamount,tp.discountamount,tpd.cesstaxamount,tpd.shcesstaxamount,tpd.vatorcsttaxamount,basicamount,totalamount,vchuomid from tabmmspurchaseorder tp  left join tabmmspurchaseorderdetails tpd on tp.recordid=tpd.orderid left join tabinvuommst tbi on tbi.vchuomdescription=vchuom where tp.vchpurchaseorderno='" + ManageQuote(PurchaseOrderNo) + "'";
                }

                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strPO);

            }
            catch
            {


            }
            return ds;
        }

        public DataSet GetPurchaseOrderVendorsDetailsByID(string PurchaseOrderNo)
        {
            DataSet ds = new DataSet();
            try
            {
                using (NpgsqlDataAdapter da = new NpgsqlDataAdapter("select distinct vchvendorname,vchpurchaseorderno from vwmmspurchaseorder where vchpurchaseorderno='" + ManageQuote(PurchaseOrderNo) + "'", NPGSqlHelper.SQLConnString))
                {
                    da.Fill(ds);
                }
            }
            catch
            {


            }
            return ds;
        }

        #endregion

        #region Stock Report

        public DataSet Getstockbystore(string fromdate, string todate, string store, string strReportType)
        {
            DataSet ds = new DataSet();
            try
            {
                string strQuery = string.Empty;
                if (strReportType == "Closing Stock")
                {
                    //if (store == "" || store == "All" || store == "0" || store == "undefined")
                    //{
                    //    strQuery = "select productid,productname,vchorderuom as vchuom,sum(purchaseqty) as availableqty,storagelocation from vwproductpurchaseconsumereturnqty where datgrndate <= '" + FormatDate(fromdate) + "'  group by productid,productname,vchorderuom,storagelocation order by productname";
                    //}
                    //else
                    //{
                    //strQuery = "select productid,productname,vchorderuom as vchuom,sum(purchaseqty) as availableqty,storagelocation from vwproductpurchaseconsumereturnqty where datgrndate <= '" + FormatDate(fromdate) + "' and  storagelocation='MAIN STORAGE' group by productid,productname,vchorderuom,storagelocation order by productname";

                    //// Code commented on 09-06-17 due to removal of between dates of closing stock///////

                    //if (fromdate != null && fromdate != "" && todate != null && todate != "" && todate != "undefined")
                    //{
                    //   // strQuery = "select t3.productid,t3.productname,vchuom,availableqty,productrate,storagelocation from (select productid,productname,vchorderuom as vchuom,sum(purchaseqty) as availableqty,storagelocation from vwproductpurchaseconsumereturnqty where datgrndate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and  storagelocation='MAIN STORAGE'  group by productid,productname,vchorderuom,storagelocation order by productname)t3 join (select productid,productname,sum(productamount) /sum(numreceivedqty) as productrate from (select productid,productname,numreceivedqty,case when coalesce(t1.vchdiscounttype,'FLAT')='FLAT' then (numreceivedqty*numgrnrate)-coalesce(numdiscountvalue,0) else ((numreceivedqty*numgrnrate)*coalesce(numdiscountvalue,0))/100 end as productamount from tabmmsgoodsreceivednotedetails t1 join tabmmsgoodsreceivednote t2 on t1.vchgrnno=t2.vchgrnno where datgrndate <='" + FormatDate(todate) + "') t3 group by productid,productname)t4 on t3.productid=t4.productid order by productname";

                    //    strQuery = "select t3.productid,t3.productname,vchuom,availableqty,productrate,storagelocation from (select productid,productname,vchorderuom as vchuom,sum(purchaseqty) as availableqty,storagelocation from vwproductpurchaseconsumereturnqty where datgrndate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and  storagelocation='MAIN STORAGE'  group by productid,productname,vchorderuom,storagelocation order by productname)t3 join (select materialid1,qty1,rate1 as productrate from fnmaterialclosingstockratefifo('" + FormatDate(todate) + "'))t4 on t3.productid::text=t4.materialid1 order by productname";
                    //}
                    //else
                    //{
                    //   // strQuery = "select t3.productid,t3.productname,vchuom,availableqty,productrate,storagelocation from (select productid,productname,vchorderuom as vchuom,sum(purchaseqty) as availableqty,storagelocation from vwproductpurchaseconsumereturnqty where datgrndate <= '" + FormatDate(fromdate) + "' and  storagelocation='MAIN STORAGE'  group by productid,productname,vchorderuom,storagelocation order by productname)t3 join (select productid,productname,sum(productamount) /sum(numreceivedqty) as productrate from (select productid,productname,numreceivedqty,case when coalesce(t1.vchdiscounttype,'FLAT')='FLAT' then (numreceivedqty*numgrnrate)-coalesce(numdiscountvalue,0) else ((numreceivedqty*numgrnrate)*coalesce(numdiscountvalue,0))/100 end as productamount from tabmmsgoodsreceivednotedetails t1 join tabmmsgoodsreceivednote t2 on t1.vchgrnno=t2.vchgrnno where datgrndate <='" + FormatDate(fromdate) + "') t3 group by productid,productname)t4 on t3.productid=t4.productid order by productname";

                    //    strQuery = "select t3.productid,t3.productname,vchuom,availableqty,productrate,storagelocation from (select productid,productname,vchorderuom as vchuom,sum(purchaseqty) as availableqty,storagelocation from vwproductpurchaseconsumereturnqty where datgrndate <='" + FormatDate(fromdate) + "' and  storagelocation='MAIN STORAGE'  group by productid,productname,vchorderuom,storagelocation order by productname)t3 join (select materialid1,qty1,rate1 as productrate from fnmaterialclosingstockratefifo('" + FormatDate(fromdate) + "'))t4 on t3.productid::text=t4.materialid1  order by productname";
                    //}

                    /////  End  Code commented on 09-06-17 due to removal of between dates of closing stock///////

                 //  strQuery = "select t3.productid,t3.productname,case when vchuom='KILOGRAMS' then 'KG' when vchuom='NUMBERS' then 'NO.' end as vchuom,availableqty,productrate,storagelocation from (select productid,productname,vchorderuom as vchuom,sum(purchaseqty) as availableqty,storagelocation from vwproductpurchaseconsumereturnqty where datgrndate <='" + FormatDate(fromdate) + "' and  storagelocation='MAIN STORAGE'  group by productid,productname,vchorderuom,storagelocation order by productname)t3 join (select materialid1,qty1,rate1 as productrate from fnmaterialclosingstockratefifo('" + FormatDate(fromdate) + "'))t4 on t3.productid::text=t4.materialid1  order by productname";

                    strQuery = "select t3.productid,t3.productname,case when vchuom='KILOGRAMS' then 'KG' when vchuom='NUMBERS' then 'NO.' end as vchuom,availableqty,productrate,storagelocation from (select productid,productname,vchorderuom as vchuom,sum(purchaseqty) as availableqty,storagelocation from vwproductpurchaseconsumereturnqty where datgrndate <='" + FormatDate(fromdate) + "' and  storagelocation='MAIN STORAGE'  group by productid,productname,vchorderuom,storagelocation order by productname)t3 join (select materialid1,qty1,rate1 as productrate from fnmaterialclosingstockratefifo('" + FormatDate(fromdate) + "') where qty1<>0 and rate1<>0)t4 on t3.productid::text=t4.materialid1  order by productname";

                    //}
                }
                else if (strReportType == "Stock Ledger")
                {
                    //if (store == "" || store == "All" || store == "0" || store == "undefined")
                    //{
                    //    //strQuery = "select * from (select t1.productid,t1.productname,uomname,t3.storagelocation,t3.shelfname,coalesce(openingqty,0) as openingqty,purchaseqty,consumptionqty from tabmmsproductmst t1 left join (select productid,productname,vchorderuom  ,sum(purchaseqty) as openingqty,storagelocation,shelfname from vwproductpurchaseconsumereturnqty where datgrndate<'" + FormatDate(fromdate) + "'  group by productid,productname,vchorderuom ,storagelocation,shelfname)t2 on t1.productid=t2.productid and t1.productname=t2.productname and t1.uomname=t2.vchorderuom join (select productid,productname,vchorderuom,storagelocation,shelfname,case when type!='ISSUE' then availableqty else 0 end as purchaseqty, case when type='ISSUE' then availableqty else 0 end as consumptionqty from(select productid,productname,vchorderuom,sum(purchaseqty) as availableqty,storagelocation,shelfname,type from vwproductpurchaseconsumereturnqty where datgrndate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' group by productid,productname,vchorderuom,storagelocation,shelfname,type) g)t3 on t1.productid=t3.productid and t1.productname=t3.productname and t1.uomname=t3.vchorderuom ) t4 ";
                    //    strQuery = "select * from (select t1.productid,t1.productname,uomname,t3.storagelocation,t3.shelfname,coalesce(openingqty,0) as openingqty,purchaseqty,consumptionqty,returnqty from tabmmsproductmst t1 left join (select productid,productname,vchorderuom ,sum(purchaseqty) as openingqty,storagelocation,shelfname from vwproductpurchaseconsumereturnqty where datgrndate<'" + FormatDate(fromdate) + "'  group by productid,productname,vchorderuom ,storagelocation,shelfname)t2 on t1.productid=t2.productid and t1.productname=t2.productname and t1.uomname=t2.vchorderuom join (select productid,productname,vchorderuom,storagelocation,shelfname,sum(case when type='PURCHASE' then availableqty else 0 end) as purchaseqty,sum( case when type='ISSUE' then availableqty else 0 end) as consumptionqty,sum( case when type='CANCEL' then availableqty else 0 end) as Returnqty from(select productid,productname,vchorderuom,sum(purchaseqty) as availableqty,storagelocation,shelfname,type from vwproductpurchaseconsumereturnqty where datgrndate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' group by productid,productname,vchorderuom,storagelocation,shelfname,type)g group by productid,productname,vchorderuom,storagelocation,shelfname)t3 on t1.productid=t3.productid and t1.productname=t3.productname and t1.uomname=t3.vchorderuom ) t4 order by productname ";
                    //}
                    //else
                    //{
                    strQuery = "select * from (select t1.productid,t1.productname,case when uomname='KILOGRAMS' then 'KG' when uomname='NUMBERS' then 'NO.' end as vchuom,t3.storagelocation,t3.shelfname,coalesce(openingqty,0) as openingqty,purchaseqty,consumptionwasteqty,consumptionsaleqty,returnqty from tabmmsproductmst t1 left join (select productid,productname,vchorderuom ,sum(purchaseqty) as openingqty,storagelocation,shelfname from vwproductpurchaseconsumereturnqty where datgrndate<'" + FormatDate(fromdate) + "' and storagelocation='MAIN STORAGE'  group by productid,productname,vchorderuom ,storagelocation,shelfname)t2 on t1.productid=t2.productid and t1.productname=t2.productname and t1.uomname=t2.vchorderuom join (select productid,productname,vchorderuom,storagelocation,shelfname,sum(case when type='PURCHASE' then availableqty else 0 end) as purchaseqty,sum( case when type='ISSUE' then availableqty else 0 end) as consumptionwasteqty,sum( case when type='SALE'   then availableqty else 0 end) as consumptionsaleqty,sum( case when type='CANCEL' then availableqty else 0 end) as Returnqty from(select productid,productname,vchorderuom,sum(purchaseqty) as availableqty,storagelocation,shelfname,type from vwproductpurchaseconsumereturnqty where datgrndate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and storagelocation='MAIN STORAGE' group by productid,productname,vchorderuom,storagelocation,shelfname,type)g group by productid,productname,vchorderuom,storagelocation,shelfname)t3 on t1.productid=t3.productid and t1.productname=t3.productname and t1.uomname=t3.vchorderuom ) t4 where storagelocation='MAIN STORAGE' order by productname";
                    //}
                }
                using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(strQuery, NPGSqlHelper.SQLConnString))
                {
                    da.Fill(ds, strReportType);
                }
            }
            catch
            {


            }
            return ds;
        }

        #endregion

        #region Purchase Report

        public DataSet GetPurchaseData(string fromdate, string todate, string strReportType, string VendorName)
        {
            DataSet ds = new DataSet();
            try
            {
                string strQuery = string.Empty;
                if (strReportType == "Purchse Statement")
                {
                    if (VendorName == "ALL")
                    {
                        //strQuery = "select distinct vchinvoiceno,cast(datinvoicedate as date)as datinvoicedate,vchvendorname,totalbasic,itemdiscount as totaldiscount,(servicetaxamount+vatorcstamount)as servicetaxamount,transportcharges from vwmmspurchasestmt where datgrndate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by vchvendorname,datinvoicedate;";
                        strQuery = "select distinct vchinvoiceno,cast(datinvoicedate as date)as datinvoicedate,vchvendorname,sum(basic-returnamount)as totalbasic,sum(itemdiscount) as totaldiscount,(servicetaxamount+vatorcstamount)as servicetaxamount,transportcharges from vwmmspurchasestmt where datgrndate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' group by vchinvoiceno,datinvoicedate,vchvendorname,totalbasic,servicetaxamount,vatorcstamount,transportcharges order by vchvendorname,datinvoicedate;";
                    }
                    else
                    {
                        //strQuery = "select distinct vchinvoiceno,cast(datinvoicedate as date)as datinvoicedate,vchvendorname,totalbasic,itemdiscount as totaldiscount,(servicetaxamount+vatorcstamount)as servicetaxamount,transportcharges from vwmmspurchasestmt where datgrndate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and vchvendorname='" + ManageQuote(VendorName) + "' order by vchvendorname,datinvoicedate;";
                        strQuery = "select distinct vchinvoiceno,cast(datinvoicedate as date)as datinvoicedate,vchvendorname,sum(basic-returnamount)as totalbasic,sum(itemdiscount) as totaldiscount,(servicetaxamount+vatorcstamount)as servicetaxamount,transportcharges from vwmmspurchasestmt where datgrndate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and vchvendorname='" + ManageQuote(VendorName) + "' group by vchinvoiceno,datinvoicedate,vchvendorname,totalbasic,servicetaxamount,vatorcstamount,transportcharges order by vchvendorname,datinvoicedate;";
                    }
                }
                else if (strReportType == "Purchase Bill Statement")
                {
                    strQuery = "select vchgrnno,vchinvoiceno,datinvoicedate,vchvendorname,productname,vchorderuom,purchaseqty,numgrnrate,basic,itemdiscount,itemtaxamount,numorderuomconversion,itemtransportcharges from vwmmspurchasestmt where datgrndate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by vchinvoiceno;";
                }
                else if (strReportType == "Purchase Item Statement")
                {
                    //strQuery = "select productname,purchaseqty,vchorderuom,numgrnrate,vchinvoiceno,datinvoicedate,vchvendorname from vwmmspurchasestmt where datgrndate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by productname;";
                    strQuery = "select productname,(purchaseqty) as purchaseqty,vchorderuom,round(numgrnrate/numorderuomconversion,2)as numgrnrate,vchinvoiceno,datinvoicedate,vchvendorname from vwmmspurchasestmt where datgrndate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by productname;";
                }
                using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(strQuery, NPGSqlHelper.SQLConnString))
                {
                    da.Fill(ds, strReportType);
                }
            }
            catch
            {


            }
            return ds;
        }

        //public List<VendorDetailsDTO> ShowVendors()
        //{
        //    List<VendorDetailsDTO> lstVendors = new List<VendorDetailsDTO>();
        //    try
        //    {

        //        string strVendor = "select vchvendorcode,vchvendorname from tabmmsvendormst where statusid=1 order by vchvendorname;";
        //        npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strVendor);
        //        while (npgdr.Read())
        //        {
        //            VendorDetailsDTO objVendorsDTO = new VendorDetailsDTO();
        //            objVendorsDTO.vendorcode = npgdr["vchvendorcode"].ToString();
        //            objVendorsDTO.vendorname = npgdr["vchvendorname"].ToString();
        //            lstVendors.Add(objVendorsDTO);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "ShowVendors");
        //        throw ex;

        //    }
        //    finally
        //    {
        //        npgdr.Dispose();
        //    }
        //    return lstVendors;

        //}

        public DataTable ShowVendors()
        {
            DataTable dt = new DataTable();
            string strdta = string.Empty;
            try
            {
                strdta = "select vendorid,vchvendorname from tabmmsvendormst where statusid=1 order by vchvendorname;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strdta).Tables[0];
                DataRow dr = dt.NewRow();
                dr["vendorid"] = 0;
                dr["vchvendorname"] = "ALL";
                dt.Rows.InsertAt(dr, 0);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;

        }

        public DataTable ShowInvoiceNo(string strVendorid)
        {
            DataTable dt = new DataTable();
            string strdta = string.Empty;
            try
            {

                if (strVendorid == "" || strVendorid == "ALL" || strVendorid == "0")
                {
                    strdta = "select distinct vchinvoiceno from tabmmsgoodsreceivednote where statusid=1 order by vchinvoiceno;";
                }
                else
                {
                    strdta = "select distinct vchinvoiceno from tabmmsgoodsreceivednote where vendorid='" + ManageQuote(strVendorid) + "' and statusid=1 order by vchinvoiceno;";
                }
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strdta).Tables[0];
                DataRow dr = dt.NewRow();
                dr["vchinvoiceno"] = 0;
                dr["vchinvoiceno"] = "ALL";
                dt.Rows.InsertAt(dr, 0);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;

        }


        public DataSet PurchaseInvoiceReport(string fromdate, string todate, string strInvoiceNo, string strVendorname)
        {
            DataSet ds = new DataSet();
            string strQuery = string.Empty;
            try
            {
                if (strInvoiceNo != "" && strInvoiceNo != "ALL" && strVendorname != "" && strVendorname != "ALL")
                {
                    strQuery = "select vchgrnno,vchinvoiceno,datinvoicedate,vchvendorname,productname,vchorderuom,purchaseqty,numgrnrate,basic,itemdiscount,itemtaxamount,numorderuomconversion,itemtransportcharges from vwmmspurchasestmt where datinvoicedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and vchinvoiceno='" + ManageQuote(strInvoiceNo) + "' and vchvendorname='" + ManageQuote(strVendorname) + "' order by vchinvoiceno;";
                }
                else if (strVendorname == "ALL" && strInvoiceNo != "ALL")
                {
                    strQuery = "select vchgrnno,vchinvoiceno,datinvoicedate,vchvendorname,productname,vchorderuom,purchaseqty,numgrnrate,basic,itemdiscount,itemtaxamount,numorderuomconversion,itemtransportcharges from vwmmspurchasestmt where datinvoicedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and vchinvoiceno='" + ManageQuote(strInvoiceNo) + "' order by vchinvoiceno;";
                }
                else if (strVendorname != "ALL" && strInvoiceNo == "ALL")
                {
                    strQuery = "select vchgrnno,vchinvoiceno,datinvoicedate,vchvendorname,productname,vchorderuom,purchaseqty,numgrnrate,basic,itemdiscount,itemtaxamount,numorderuomconversion,itemtransportcharges from vwmmspurchasestmt where datinvoicedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and vchvendorname='" + ManageQuote(strVendorname) + "' order by vchinvoiceno;";
                }
                else
                {
                    strQuery = "select vchgrnno,vchinvoiceno,datinvoicedate,vchvendorname,productname,vchorderuom,purchaseqty,numgrnrate,basic,itemdiscount,itemtaxamount,numorderuomconversion,itemtransportcharges from vwmmspurchasestmt where datinvoicedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by vchinvoiceno;";
                    //strQuery = "select vchgrnno,vchinvoiceno,datinvoicedate,vchvendorname,productname,vchorderuom,purchaseqty,numgrnrate,basic,itemdiscount,itemtaxamount,numorderuomconversion,itemtransportcharges from vwmmspurchasestmt where vchinvoiceno='" + ManageQuote(strInvoiceNo) + " and vchvendorname='" + ManageQuote(strVendorname) + "' order by vchinvoiceno;";
                }
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery);
            }
            catch
            {


            }
            return ds;
        }



        public DataSet GetVendorPurchaseData(string strVendorName)
        {
            DataSet ds = new DataSet();
            string strQuery = string.Empty;
            try
            {
                if (strVendorName == "" || strVendorName == "ALL")
                {
                    strQuery = "select distinct vchinvoiceno,cast(datinvoicedate as date)as datinvoicedate,vchvendorname,(totalbasic-returnamount) AS totalbasic,totaldiscount,(servicetaxamount+vatorcstamount)as servicetaxamount,transportcharges from vwmmspurchasestmt order by vchinvoiceno;";
                }
                else
                {
                    strQuery = "select distinct vchinvoiceno,cast(datinvoicedate as date)as datinvoicedate,vchvendorname,(totalbasic-returnamount) AS totalbasic,totaldiscount,(servicetaxamount+vatorcstamount)as servicetaxamount,transportcharges from vwmmspurchasestmt where vchvendorname='" + ManageQuote(strVendorName) + "' order by vchinvoiceno;";
                }

                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery);
            }
            catch
            {

            }
            return ds;
        }

        #endregion

        #region Outward Report

        public DataSet GetOutwardData(string fromdate, string todate, string store, string tostore, string category, string subcategory)
        {
            DataSet ds = new DataSet();
            try
            {
                string strQuery = string.Empty;


                if (store != "ALL" && store != "" && tostore != "ALL" && tostore != "" && category != "ALL" && category != "" && subcategory != "ALL" && subcategory != "")
                {
                    strQuery = "select  datissuedate,productname,numissueconvertionqty,vchuom,numrate,(numissueconvertionqty*numrate) as amount,vchdepartment,categoryname,storagelocation,tostorage from vwmmsproductsoutwardstmt where storagelocation='" + ManageQuote(store) + "' and tostorage='" + ManageQuote(tostore) + "' and categoryname='" + ManageQuote(category) + "' and subcategoryname='" + ManageQuote(subcategory) + "' and datissuedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by datissuedate,productname,tostorage;";
                }
                else if (store == "ALL" && store != "" && tostore != "ALL" && tostore != "" && category != "ALL" && category != "" && subcategory != "ALL" && subcategory != "")
                {
                    strQuery = "select  datissuedate,productname,numissueconvertionqty,vchuom,numrate,(numissueconvertionqty*numrate) as amount,vchdepartment,categoryname,storagelocation,tostorage from vwmmsproductsoutwardstmt where tostorage='" + ManageQuote(tostore) + "' and categoryname='" + ManageQuote(category) + "' and subcategoryname='" + ManageQuote(subcategory) + "' and datissuedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by datissuedate,productname,tostorage;";
                }
                else if (store != "ALL" && store != "" && tostore != "ALL" && tostore != "" && category == "ALL" && category != "" && subcategory == "ALL" && subcategory != "")
                {
                    strQuery = "select  datissuedate,productname,numissueconvertionqty,vchuom,numrate,(numissueconvertionqty*numrate) as amount,vchdepartment,categoryname,storagelocation,tostorage from vwmmsproductsoutwardstmt where  storagelocation='" + ManageQuote(store) + "' and tostorage='" + ManageQuote(tostore) + "' and datissuedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by datissuedate,productname,tostorage;";
                }
                else if (store != "ALL" && store != "" && tostore == "ALL" && tostore != "" && category != "ALL" && category != "" && subcategory != "ALL" && subcategory != "")
                {
                    strQuery = "select  datissuedate,productname,numissueconvertionqty,vchuom,numrate,(numissueconvertionqty*numrate) as amount,vchdepartment,categoryname,storagelocation,tostorage from vwmmsproductsoutwardstmt where storagelocation='" + ManageQuote(store) + "' and categoryname='" + ManageQuote(category) + "' and subcategoryname='" + ManageQuote(subcategory) + "' and datissuedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by datissuedate,productname,tostorage;";
                }
                else if (store != "ALL" && store != "" && tostore == "ALL" && tostore != "" && category == "ALL" && category != "" && subcategory == "ALL" && subcategory != "")
                {
                    strQuery = "select  datissuedate,productname,numissueconvertionqty,vchuom,numrate,(numissueconvertionqty*numrate) as amount,vchdepartment,categoryname,storagelocation,tostorage from vwmmsproductsoutwardstmt where storagelocation='" + ManageQuote(store) + "' and datissuedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by datissuedate,productname,tostorage;";
                }
                else if (store != "ALL" && store != "" && tostore == "ALL" && tostore != "" && category != "ALL" && category != "" && subcategory == "ALL" && subcategory != "")
                {
                    strQuery = "select  datissuedate,productname,numissueconvertionqty,vchuom,numrate,(numissueconvertionqty*numrate) as amount,vchdepartment,categoryname,storagelocation,tostorage from vwmmsproductsoutwardstmt where storagelocation='" + ManageQuote(store) + "' and categoryname='" + ManageQuote(category) + "' and datissuedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by datissuedate,productname,tostorage;";
                }
                else if (store == "ALL" && store != "" && tostore == "ALL" && tostore != "" && category != "ALL" && category != "" && subcategory != "ALL" && subcategory != "")
                {
                    strQuery = "select  datissuedate,productname,numissueconvertionqty,vchuom,numrate,(numissueconvertionqty*numrate) as amount,vchdepartment,categoryname,storagelocation,tostorage from vwmmsproductsoutwardstmt where categoryname='" + ManageQuote(category) + "' and subcategoryname='" + ManageQuote(subcategory) + "' and datissuedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by datissuedate,productname,tostorage;";
                }
                else if (store == "ALL" && store != "" && tostore == "ALL" && tostore != "" && category == "ALL" && category != "" && subcategory != "ALL" && subcategory != "")
                {
                    strQuery = "select  datissuedate,productname,numissueconvertionqty,vchuom,numrate,(numissueconvertionqty*numrate) as amount,vchdepartment,categoryname,storagelocation,tostorage from vwmmsproductsoutwardstmt where subcategoryname='" + ManageQuote(subcategory) + "' and datissuedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by datissuedate,productname,tostorage;";
                }
                else if (store == "ALL" && store != "" && tostore == "ALL" && tostore != "" && category == "ALL" && category != "" && subcategory == "ALL" && subcategory != "")
                {
                    strQuery = "select  datissuedate,productname,numissueconvertionqty,vchuom,numrate,(numissueconvertionqty*numrate) as amount,vchdepartment,categoryname,storagelocation,tostorage from vwmmsproductsoutwardstmt where  datissuedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by datissuedate,productname,tostorage;";
                }
                else if (store != "ALL" && store != "" && tostore != "ALL" && tostore != "" && category != "ALL" && category != "" && subcategory == "ALL" && subcategory != "")
                {
                    strQuery = "select  datissuedate,productname,numissueconvertionqty,vchuom,numrate,(numissueconvertionqty*numrate) as amount,vchdepartment,categoryname,storagelocation,tostorage from vwmmsproductsoutwardstmt where storagelocation='" + ManageQuote(store) + "' and tostorage='" + ManageQuote(tostore) + "' and categoryname='" + ManageQuote(category) + "' and  datissuedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by datissuedate,productname,tostorage;";
                }

                using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(strQuery, NPGSqlHelper.SQLConnString))
                {
                    da.Fill(ds);
                }
            }
            catch
            {

            }
            return ds;
        }

        #endregion

        #region Product Indent By Vikram Chathilla

        public DataSet GetProductIndentReport(string indent)
        {
            DataSet ds = new DataSet();
            try
            {
                //  string sxfk = "select TA.recordid,TA.vchindentno,TA.datindentdate,TA.vchindenttype,TA.vchrequestedby,TA.vchapprovalby,TA.vchdepartment,TA.statusid,TA.createdby,TA.createddate,TA.modifiedby,TA.modifieddate,TB.recordid as recordid2,TB.indentno as indentno2,TB.vchindentno as vchindentno2,TB.productid as productid2,TB.productcode as productcode2,TB.productcategoryid as productcategoryid2,TB.categoryname as categoryname2,TB.productsubcategoryid as productsubcategoryid2,TB.subcategoryname as subcategoryname2,TB.vchuom as vchuom2,TB.vchindentuom as vchindentuom2,TB.numindentqty as numindentqty2,TB.numindentconvertionqty as numindentconvertionqty2,TB.statusid as statusid2,TB.createdby as createdby2,TB.AvailableQty as AvailableQty2,TB.createddate as createddate2,TB.modifiedby as modifiedby2,TB.modifieddate as modifieddate2,TB.productname as productname2 ,TB.storagelocationname as storagelocationname2,TB.storagelocationcode as storagelocationcode2,TB.shelfname as shelfname2,TB.availableqty as  availableqty2,TB.vchmaxstock as vchmaxstock2,TB.vchminstock as vchminstock2 ,TB.vchproductype as vchproductype2 from tabmmsmaterialindent TA   join  tabmmsmaterialindentdetails TB on TA.recordid= TB.indentno where TB.vchindentno='" + indent + "'and  TB.statusid=1  ";
                using (NpgsqlDataAdapter da = new NpgsqlDataAdapter("select TA.recordid,TA.vchindentno,to_char(TA.datindentdate,'dd-MM-yyyy') as datindentdate,TA.vchindenttype,TA.vchrequestedby,TA.vchapprovalby,TA.vchdepartment,TA.statusid,TA.createdby,TA.createddate,TA.modifiedby,TA.modifieddate,TB.recordid as recordid2,TB.indentno as indentno2,TB.vchindentno as vchindentno2,TB.productid as productid2,TB.productcode as productcode2,TB.productcategoryid as productcategoryid2,TB.categoryname as categoryname2,TB.productsubcategoryid as productsubcategoryid2,TB.subcategoryname as subcategoryname2,TB.vchuom as vchuom2,TB.vchindentuom as vchindentuom2,TB.numindentqty as numindentqty2,TB.numindentconvertionqty as numindentconvertionqty2,TB.statusid as statusid2,TB.createdby as createdby2,TB.AvailableQty as AvailableQty2,TB.createddate as createddate2,TB.modifiedby as modifiedby2,TB.modifieddate as modifieddate2,TB.productname as productname2 ,TB.storagelocationname as storagelocationname2,TB.storagelocationcode as storagelocationcode2,TB.shelfname as shelfname2,TB.availableqty as  availableqty2,TB.vchmaxstock as vchmaxstock2,TB.vchminstock as vchminstock2 ,TB.vchproductype as vchproductype2 from tabmmsmaterialindent TA   join  tabmmsmaterialindentdetails TB on TA.recordid= TB.indentno where TB.vchindentno='" + indent + "' and  TB.statusid=1 ", NPGSqlHelper.SQLConnString))
                {
                    da.Fill(ds);
                }
            }
            catch
            {


            }
            return ds;
        }

        #endregion

        #region Productdetailsreport
        public DataSet Productdetailsreport()
        {
            DataSet ds = new System.Data.DataSet();
            string strqry = string.Empty;
            try
            {

                strqry = "select productid,productcode,productname,categoryname,vchuomid,subcategoryname,producttype from tabmmsproductmst where statusid=1 order by productname;";

                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);
            }
            catch (Exception ex)
            {

                throw ex;

            }


            return ds;
        }

        #endregion

        #region Category

        public DataTable GetmmsCategorynames()
        {
            DataTable dt = new DataTable();
            string strdata = string.Empty;
            try
            {
                strdata = "select productcategoryid,productcategoryname  from tabmmsproductcategorymst where statusid=1 order by productcategoryname;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strdata).Tables[0];
                DataRow dr = dt.NewRow();
                dr["productcategoryid"] = 0;
                dr["productcategoryname"] = "ALL";
                dt.Rows.InsertAt(dr, 0);
            }
            catch (Exception)
            {

                throw;
            }
            return dt;
        }


        public DataSet mmsCategoryReport(string Category, DateTime fromdate, DateTime todate)
        {
            DataSet ds = new System.Data.DataSet();
            string strqry = string.Empty;
            try
            {

                if (Category == "0")
                {

                    //strqry = "select distinct productcategoryid,categoryname,totalbasic,totaldiscount,transportcharges,servicetaxamount,vatorcstamount from vwmmspurchasestmt order by categoryname;";
                    strqry = "select distinct productcategoryid,categoryname,sum(basic-returnamount) as totalbasic,sum(itemdiscount) as totaldiscount,sum(itemtransportcharges) as transportcharges,sum(itemtaxamount)as servicetaxamount,vatorcstamount from vwmmspurchasestmt where datgrndate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' group by productcategoryid,categoryname,vatorcstamount ";
                }
                else
                {
                    //strqry = "select distinct productcategoryid,categoryname,totalbasic,totaldiscount,transportcharges,servicetaxamount,vatorcstamount from vwmmspurchasestmt where productcategoryid=" + Category + " order by categoryname;";
                    strqry = "select distinct productcategoryid,categoryname,sum(basic-returnamount) as totalbasic,sum(itemdiscount) as totaldiscount,sum(itemtransportcharges) as transportcharges,sum(itemtaxamount)as servicetaxamount,vatorcstamount from vwmmspurchasestmt  where productcategoryid='" + ManageQuote(Category) + "' and datgrndate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' group by productcategoryid,categoryname,vatorcstamount  ";
                }
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);

            }
            catch (Exception ex)
            {
                throw ex;

            }
            return ds;
        }


        public DataTable Getmmsproductnames()
        {
            DataTable dt = new DataTable();
            string strdata = string.Empty;
            try
            {
                strdata = "select productid,productname from tabmmsproductmst where statusid=1 order by productname;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strdata).Tables[0];
                DataRow dr = dt.NewRow();
                dr["productid"] = 0;
                dr["productname"] = "ALL";
                dt.Rows.InsertAt(dr, 0);
            }
            catch (Exception)
            {

                throw;
            }
            return dt;
        }


        public DataSet MmsproductReport(string product, DateTime fromdate, DateTime todate)
        {
            DataSet ds = new System.Data.DataSet();
            string strqry = string.Empty;
            try
            {

                if (product == "0")
                {

                    strqry = "select  case when  vchorderuom='KILOGRAMS' then 'KG' when vchorderuom='NUMBERS' then 'NO.' end as vchorderuom,vchuom,productid,productname,categoryname,subcategoryname,vchinvoiceno,vchvendorname,purchaseqty,round(coalesce(numgrnrate/numorderuomconversion,0),2) as rate,itemdiscount,itemcharges,itemtransportcharges,datgrndate,itemtaxamount,returnamount from vwmmspurchasestmt where datgrndate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' order by productname;";
                }
                else
                {
                    strqry = "select  case when  vchorderuom='KILOGRAMS' then 'KG' when vchorderuom='NUMBERS' then 'NO.' end as vchorderuom,vchuom,productid,productname,categoryname,subcategoryname,vchinvoiceno,vchvendorname,purchaseqty,round(coalesce(numgrnrate/numorderuomconversion,0),2) as rate,itemdiscount,itemcharges,itemtransportcharges,datgrndate,itemtaxamount,returnamount from vwmmspurchasestmt where productid=" + product + " and  datgrndate between '" + FormatDate(fromdate.ToString("dd/MM/yyyy")) + "' and '" + FormatDate(todate.ToString("dd/MM/yyyy")) + "' order by productname;";
                }
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);

            }
            catch (Exception ex)
            {
                throw ex;

            }
            return ds;
        }

        public DataSet SubcategoryPurchaseandSaleReport(string Categoryid, string SubCategoryid, string FromDate, string ToDate)
        {
            DataSet ds = new DataSet();
            try
            {
                string strGetdata = string.Empty;
                if (Categoryid == "ALL")
                {
                    strGetdata = "select * from (select distinct t1.categoryname as productcategoryname ,t1.subcategoryname as productsubcategoryname,(coalesce(baiscamount,0)+coalesce(tax,0)+coalesce(othercharges,0)-coalesce(discount,0)) as purchaseamount,coalesce(saleamount,0) as  saleamount from tabmmsproductmst  t1 left join (select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname, round(sum(basic-returnamount),2) as baiscamount,sum(coalesce(itemtransportcharges,0)) as othercharges,sum(coalesce(itemtaxamount,0)) as tax,sum(coalesce(itemdiscount,0)) as discount from vwmmspurchasestmt tv where  datgrndate between '" + FromDate + "' and '" + ToDate + "' group by productcategoryid,categoryname,productsubcategoryid,subcategoryname) t2 on t1.productcategoryid=t2.productcategoryid and coalesce( t1.productsubcategoryid,0)= coalesce( t2.productsubcategoryid,0) left join (select sum(t1.numsaleamount) as saleamount,productcategoryid,productsubcategoryid,categoryname,subcategoryname from tabmmssaledetails t1 join tabmmsproductmst t2 on t1.vchproductid=t2.productid::text join tabmmssale t3 on t3.recordid=t1.detailsid where datsaledate between  '" + FromDate + "' and '" + ToDate + "'  group by categoryname,subcategoryname,productcategoryid,productsubcategoryid) t3 on  t1.productcategoryid=t3.productcategoryid and coalesce(t1.productsubcategoryid,0)= coalesce(t3.productsubcategoryid,0)) g where purchaseamount<>0 or saleamount<>0 order by 1,2 ";
                   // strGetdata = "select * from (select distinct t1.categoryname  ,t1.subcategoryname ,(coalesce(baiscamount,0)+coalesce(tax,0)+coalesce(othercharges,0)-coalesce(discount,0)) as purchaseamount,coalesce(saleamount,0) as  saleamount from tabmmsproductmst  t1 left join (select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname, round(sum(basic-returnamount),2) as baiscamount,sum(coalesce(itemtransportcharges,0)) as othercharges,sum(coalesce(itemtaxamount,0)) as tax,sum(coalesce(itemdiscount,0)) as discount from vwmmspurchasestmt tv where  datgrndate between '" + FromDate + "' and '" + ToDate + "' group by productcategoryid,categoryname,productsubcategoryid,subcategoryname) t2 on t1.productcategoryid=t2.productcategoryid and  t1.productsubcategoryid= t2.productsubcategoryid left join (select sum(t1.numsaleamount) as saleamount,productcategoryid,productsubcategoryid,categoryname,subcategoryname from tabmmssaledetails t1 join tabmmsproductmst t2 on t1.vchproductid=t2.productid::text join tabmmssale t3 on t3.recordid=t1.detailsid where datsaledate between  '" + FromDate + "' and '" + ToDate + "'  group by categoryname,subcategoryname,productcategoryid,productsubcategoryid) t3 on  t1.productcategoryid=t3.productcategoryid and t1.productsubcategoryid=t3.productsubcategoryid) g where purchaseamount<>0 or saleamount<>0 order by 1,2 ";
                   //strGetdata = "select * from (select productcategoryname,coalesce(productsubcategoryname,'') as productsubcategoryname,coalesce(purchaseamount,0) as purchaseamount,coalesce(saleamount,0) as  saleamount  from tabmmsproductsubcategorymst t1 right join  tabmmsproductcategorymst t2 on t1.productcategoryid=t2.productcategoryid left join (select distinct t1.categoryname,t1.subcategoryname, t1.productcategoryid,t1.productsubcategoryid,(coalesce(baiscamount,0)+coalesce(tax,0)+coalesce(othercharges,0)-coalesce(discount,0)) as purchaseamount from tabmmsproductmst  t1 left join (select distinct tv.productcategoryid,tv.categoryname,coalesce(tv.productsubcategoryid::text,'') as productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname, round(sum(basic-returnamount),2) as baiscamount,sum(coalesce(itemtransportcharges,0)) as othercharges, sum(coalesce(itemtaxamount,0)) as tax,sum(coalesce(itemdiscount,0)) as discount from vwmmspurchasestmt tv where  datgrndate between '"+FromDate+"' and '"+ToDate+"' group by productcategoryid,categoryname,productsubcategoryid,subcategoryname) t2 on  t1.productcategoryid=t2.productcategoryid and coalesce(t1.productsubcategoryid::text,'')=coalesce(t2.productsubcategoryid::text,'') ) t3 on t2.productcategoryid=t3.productcategoryid and coalesce(t1.productsubcategoryid::text,'')=coalesce(t3.productsubcategoryid::text,'') left join (select sum(t1.numsaleamount) as saleamount,productcategoryid,productsubcategoryid,categoryname,subcategoryname from tabmmssaledetails t1 join tabmmsproductmst t2 on t1.vchproductid=t2.productid::text join tabmmssale t3 on t3.recordid=t1.detailsid where datsaledate between  '"+FromDate+"' and '"+ToDate+"'  group by categoryname,subcategoryname,productcategoryid,productsubcategoryid) t4 on  t1.productcategoryid=t4.productcategoryid and coalesce(t1.productsubcategoryid::text,'')=coalesce(t4.productsubcategoryid::text,'')) g where purchaseamount<>0 or saleamount<>0 ORDER BY 1,2";

                    
                }
                else if (Categoryid != null && Categoryid != "" && Categoryid != string.Empty && Categoryid != "undefined" && Categoryid != "ALL" && SubCategoryid != null && SubCategoryid != "" && SubCategoryid != string.Empty && SubCategoryid != "undefined")
                {
                    if (SubCategoryid == "ALL" || SubCategoryid == "0")
                    {
                        // strGetdata = "select * from (select distinct t1.categoryname,t1.subcategoryname,(coalesce(baiscamount,0)+coalesce(tax,0)+coalesce(othercharges,0)-coalesce(discount,0)) as purchaseamount,coalesce(saleamount,0) as  saleamount from tabmmsproductmst  t1 left join (select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname, round(sum(basic-returnamount),2) as baiscamount,sum(coalesce(itemtransportcharges,0)) as othercharges,sum(coalesce(itemtaxamount,0)) as tax,sum(coalesce(itemdiscount,0)) as discount from vwmmspurchasestmt tv where productcategoryid=" + Categoryid + " and subcategoryname !='' and datgrndate between '" + FromDate + "' and '" + ToDate + "' group by productcategoryid,categoryname,productsubcategoryid,subcategoryname) t2 on t1.productcategoryid=t2.productcategoryid and t1.productsubcategoryid=t2.productsubcategoryid left join (select sum(t1.numsaleamount) as saleamount,productcategoryid,productsubcategoryid,categoryname,subcategoryname from tabmmssaledetails t1 join tabmmsproductmst t2 on t1.vchproductid=t2.productid::text join tabmmssale t3 on t3.recordid=t1.detailsid where datsaledate between  '" + FromDate + "' and '" + ToDate + "'  and productcategoryid=" + Categoryid + " group by categoryname,subcategoryname,productcategoryid,productsubcategoryid) t3 on   t1.productcategoryid=t3.productcategoryid and t1.productsubcategoryid=t3.productsubcategoryid) g where purchaseamount<>0 or saleamount<>0 order by 1,2 ";
                        //strGetdata = "select * from (select distinct t1.categoryname,t1.subcategoryname,(coalesce(baiscamount,0)+coalesce(tax,0)+coalesce(othercharges,0)-coalesce(discount,0)) as purchaseamount,coalesce(saleamount,0) as  saleamount from tabmmsproductmst  t1 left join (select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname, round(sum(basic-returnamount),2) as baiscamount,sum(coalesce(itemtransportcharges,0)) as othercharges,sum(coalesce(itemtaxamount,0)) as tax,sum(coalesce(itemdiscount,0)) as discount from vwmmspurchasestmt tv where productcategoryid=" + Categoryid + "  and datgrndate between '" + FromDate + "' and '" + ToDate + "' group by productcategoryid,categoryname,productsubcategoryid,subcategoryname) t2 on t1.productcategoryid=t2.productcategoryid and t1.productsubcategoryid=t2.productsubcategoryid left join (select sum(t1.numsaleamount) as saleamount,productcategoryid,productsubcategoryid,categoryname,subcategoryname from tabmmssaledetails t1 join tabmmsproductmst t2 on t1.vchproductid=t2.productid::text join tabmmssale t3 on t3.recordid=t1.detailsid where datsaledate between  '" + FromDate + "' and '" + ToDate + "'  and productcategoryid=" + Categoryid + " group by categoryname,subcategoryname,productcategoryid,productsubcategoryid) t3 on   t1.productcategoryid=t3.productcategoryid and t1.productsubcategoryid=t3.productsubcategoryid) g where purchaseamount<>0 or saleamount<>0 order by 1,2 ";
                       // strGetdata = "select * from (select productcategoryname,coalesce(productsubcategoryname,'') as productsubcategoryname,coalesce(purchaseamount,0) as purchaseamount,coalesce(saleamount,0) as  saleamount  from tabmmsproductsubcategorymst t1 right join  tabmmsproductcategorymst t2 on t1.productcategoryid=t2.productcategoryid left join (select distinct t1.categoryname,t1.subcategoryname, t1.productcategoryid,t1.productsubcategoryid,(coalesce(baiscamount,0)+coalesce(tax,0)+coalesce(othercharges,0)-coalesce(discount,0)) as purchaseamount from tabmmsproductmst  t1 left join (select distinct tv.productcategoryid,tv.categoryname,coalesce(tv.productsubcategoryid::text,'') as productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname, round(sum(basic-returnamount),2) as baiscamount,sum(coalesce(itemtransportcharges,0)) as othercharges, sum(coalesce(itemtaxamount,0)) as tax,sum(coalesce(itemdiscount,0)) as discount from vwmmspurchasestmt tv where  datgrndate between '" + FromDate + "' and '" + ToDate + "' and productcategoryid=" + Categoryid + " group by productcategoryid,categoryname,productsubcategoryid,subcategoryname) t2 on  t1.productcategoryid=t2.productcategoryid and coalesce(t1.productsubcategoryid::text,'')=coalesce(t2.productsubcategoryid::text,'') ) t3 on t2.productcategoryid=t3.productcategoryid and coalesce(t1.productsubcategoryid::text,'')=coalesce(t3.productsubcategoryid::text,'') left join (select sum(t1.numsaleamount) as saleamount,productcategoryid,productsubcategoryid,categoryname,subcategoryname from tabmmssaledetails t1 join tabmmsproductmst t2 on t1.vchproductid=t2.productid::text join tabmmssale t3 on t3.recordid=t1.detailsid where datsaledate between  '" + FromDate + "' and '" + ToDate + "' and productcategoryid=" + Categoryid + " group by categoryname,subcategoryname,productcategoryid,productsubcategoryid) t4 on  t1.productcategoryid=t4.productcategoryid and coalesce(t1.productsubcategoryid::text,'')=coalesce(t4.productsubcategoryid::text,'')) g where purchaseamount<>0 or saleamount<>0 ORDER BY 1,2";
                        strGetdata = "select * from (select distinct t1.categoryname as productcategoryname ,t1.subcategoryname as productsubcategoryname,(coalesce(baiscamount,0)+coalesce(tax,0)+coalesce(othercharges,0)-coalesce(discount,0)) as purchaseamount,coalesce(saleamount,0) as  saleamount from tabmmsproductmst  t1 left join (select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname, round(sum(basic-returnamount),2) as baiscamount,sum(coalesce(itemtransportcharges,0)) as othercharges,sum(coalesce(itemtaxamount,0)) as tax,sum(coalesce(itemdiscount,0)) as discount from vwmmspurchasestmt tv where  datgrndate between '" + FromDate + "' and '" + ToDate + "' and productcategoryid=" + Categoryid + " group by productcategoryid,categoryname,productsubcategoryid,subcategoryname) t2 on t1.productcategoryid=t2.productcategoryid and coalesce( t1.productsubcategoryid,0)= coalesce( t2.productsubcategoryid,0) left join (select sum(t1.numsaleamount) as saleamount,productcategoryid,productsubcategoryid,categoryname,subcategoryname from tabmmssaledetails t1 join tabmmsproductmst t2 on t1.vchproductid=t2.productid::text join tabmmssale t3 on t3.recordid=t1.detailsid where datsaledate between  '" + FromDate + "' and '" + ToDate + "' and productcategoryid=" + Categoryid + "  group by categoryname,subcategoryname,productcategoryid,productsubcategoryid) t3 on  t1.productcategoryid=t3.productcategoryid and coalesce(t1.productsubcategoryid,0)= coalesce(t3.productsubcategoryid,0)) g where purchaseamount<>0 or saleamount<>0 order by 1,2 ";
                    }
                    else if (SubCategoryid != "ALL" || SubCategoryid != "0")
                    {
                       // strGetdata = "select * from (select distinct t1.categoryname,t1.subcategoryname,(coalesce(baiscamount,0)+coalesce(tax,0)+coalesce(othercharges,0)-coalesce(discount,0)) as purchaseamount,coalesce(saleamount,0) as  saleamount from tabmmsproductmst  t1 left join (select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname, round(sum(basic-returnamount),2) as baiscamount,sum(coalesce(itemtransportcharges,0)) as othercharges,sum(coalesce(itemtaxamount,0)) as tax,sum(coalesce(itemdiscount,0)) as discount from vwmmspurchasestmt tv where productcategoryid=" + Categoryid + " and productsubcategoryid=" + SubCategoryid + " and subcategoryname !='' and datgrndate between '" + FromDate + "' and '" + ToDate + "' group by productcategoryid,categoryname,productsubcategoryid,subcategoryname) t2 on t1.productcategoryid=t2.productcategoryid and t1.productsubcategoryid=t2.productsubcategoryid left join (select sum(t1.numsaleamount) as saleamount,productcategoryid,productsubcategoryid,categoryname,subcategoryname from tabmmssaledetails t1 join tabmmsproductmst t2 on t1.vchproductid=t2.productid::text join tabmmssale t3 on t3.recordid=t1.detailsid where datsaledate between  '" + FromDate + "' and '" + ToDate + "'  and productcategoryid=" + Categoryid + " and productsubcategoryid=" + SubCategoryid + " group by categoryname,subcategoryname,productcategoryid,productsubcategoryid) t3 on t1.productcategoryid=t3.productcategoryid and t1.productsubcategoryid=t3.productsubcategoryid) g where purchaseamount<>0 or saleamount<>0 order by 1,2 ";
                        //strGetdata = "select * from (select distinct t1.categoryname,t1.subcategoryname,(coalesce(baiscamount,0)+coalesce(tax,0)+coalesce(othercharges,0)-coalesce(discount,0)) as purchaseamount,coalesce(saleamount,0) as  saleamount from tabmmsproductmst  t1 left join (select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname,round(sum(basic-returnamount),2) as baiscamount,sum(coalesce(itemtransportcharges,0)) as othercharges,sum(coalesce(itemtaxamount,0)) as tax,sum(coalesce(itemdiscount,0)) as discount from vwmmspurchasestmt tv where productcategoryid=" + Categoryid + " and productsubcategoryid=" + SubCategoryid + " and subcategoryname !=''  and datgrndate between '" + FromDate + "' and '" + ToDate + "' group by productcategoryid,categoryname,productsubcategoryid,subcategoryname) t2 on t1.productcategoryid=t2.productcategoryid and t1.productsubcategoryid=t2.productsubcategoryid left join (select sum(t1.numsaleamount) as saleamount,productcategoryid,productsubcategoryid,categoryname,subcategoryname from tabmmssaledetails t1 join tabmmsproductmst t2 on t1.vchproductid=t2.productid::text join tabmmssale t3 on t3.recordid=t1.detailsid where datsaledate between  '" + FromDate + "' and '" + ToDate + "'  and productcategoryid=" + Categoryid + " and productsubcategoryid=" + SubCategoryid + " group by categoryname,subcategoryname,productcategoryid,productsubcategoryid) t3 on   t1.productcategoryid=t3.productcategoryid and t1.productsubcategoryid=t3.productsubcategoryid) g where purchaseamount<>0 or saleamount<>0 order by 1,2";
                       // strGetdata = "select * from (select productcategoryname,coalesce(productsubcategoryname,'') as productsubcategoryname,coalesce(purchaseamount,0) as purchaseamount,coalesce(saleamount,0) as  saleamount  from tabmmsproductsubcategorymst t1 right join  tabmmsproductcategorymst t2 on t1.productcategoryid=t2.productcategoryid left join (select distinct t1.categoryname,t1.subcategoryname, t1.productcategoryid,t1.productsubcategoryid,(coalesce(baiscamount,0)+coalesce(tax,0)+coalesce(othercharges,0)-coalesce(discount,0)) as purchaseamount from tabmmsproductmst  t1 left join (select distinct tv.productcategoryid,tv.categoryname,coalesce(tv.productsubcategoryid::text,'') as productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname, round(sum(basic-returnamount),2) as baiscamount,sum(coalesce(itemtransportcharges,0)) as othercharges, sum(coalesce(itemtaxamount,0)) as tax,sum(coalesce(itemdiscount,0)) as discount from vwmmspurchasestmt tv where  datgrndate between '" + FromDate + "' and '" + ToDate + "' and productsubcategoryid=" + SubCategoryid + " and productcategoryid=" + Categoryid + " group by productcategoryid,categoryname,productsubcategoryid,subcategoryname) t2 on  t1.productcategoryid=t2.productcategoryid and coalesce(t1.productsubcategoryid::text,'')=coalesce(t2.productsubcategoryid::text,'') ) t3 on t2.productcategoryid=t3.productcategoryid and coalesce(t1.productsubcategoryid::text,'')=coalesce(t3.productsubcategoryid::text,'') left join (select sum(t1.numsaleamount) as saleamount,productcategoryid,productsubcategoryid,categoryname,subcategoryname from tabmmssaledetails t1 join tabmmsproductmst t2 on t1.vchproductid=t2.productid::text join tabmmssale t3 on t3.recordid=t1.detailsid where datsaledate between  '" + FromDate + "' and '" + ToDate + "' and productcategoryid=" + Categoryid + " and productsubcategoryid=" + SubCategoryid + " group by categoryname,subcategoryname,productcategoryid,productsubcategoryid) t4 on  t1.productcategoryid=t4.productcategoryid and coalesce(t1.productsubcategoryid::text,'')=coalesce(t4.productsubcategoryid::text,'')) g where purchaseamount<>0 or saleamount<>0 ORDER BY 1,2";
                        strGetdata = "select * from (select distinct t1.categoryname as productcategoryname ,t1.subcategoryname as productsubcategoryname,(coalesce(baiscamount,0)+coalesce(tax,0)+coalesce(othercharges,0)-coalesce(discount,0)) as purchaseamount,coalesce(saleamount,0) as  saleamount from tabmmsproductmst  t1 left join (select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname, round(sum(basic-returnamount),2) as baiscamount,sum(coalesce(itemtransportcharges,0)) as othercharges,sum(coalesce(itemtaxamount,0)) as tax,sum(coalesce(itemdiscount,0)) as discount from vwmmspurchasestmt tv where  datgrndate between '" + FromDate + "' and '" + ToDate + "' and productcategoryid=" + Categoryid + " and  productsubcategoryid=" + SubCategoryid + " group by productcategoryid,categoryname,productsubcategoryid,subcategoryname) t2 on t1.productcategoryid=t2.productcategoryid and coalesce( t1.productsubcategoryid,0)= coalesce( t2.productsubcategoryid,0) left join (select sum(t1.numsaleamount) as saleamount,productcategoryid,productsubcategoryid,categoryname,subcategoryname from tabmmssaledetails t1 join tabmmsproductmst t2 on t1.vchproductid=t2.productid::text join tabmmssale t3 on t3.recordid=t1.detailsid where datsaledate between  '" + FromDate + "' and '" + ToDate + "' and productcategoryid=" + Categoryid + " and  productsubcategoryid=" + SubCategoryid + "  group by categoryname,subcategoryname,productcategoryid,productsubcategoryid) t3 on  t1.productcategoryid=t3.productcategoryid and coalesce(t1.productsubcategoryid,0)= coalesce(t3.productsubcategoryid,0)) g where purchaseamount<>0 or saleamount<>0 order by 1,2 ";
                    }
                }
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strGetdata);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return ds;
        }
        #endregion


        public DataSet VendorDetailsReport()
        {
            DataSet ds = new DataSet();
            try
            {
                using (NpgsqlDataAdapter da = new NpgsqlDataAdapter("select tv.vchvendorname as vendorname,tv.vchvendorcode as  vendorcode,tvc.vchcontactperson as contactperson,tvc.vchemail as email,case when vchofficephone !='' then  tvc.vchmobilenumber||','||vchofficephone else tvc.vchmobilenumber end  as contactnumber,tva.vchaddress1||','||tva.vchaddress2||','||td.countryname||','||td.statename||','||td.cityname||','||tva.pincode  as address from tabmmsvendormst tv  join tabmmsvendorcontactmst tvc on tv.vendorid=tvc.vendorid join tabmmsvendoraddressmst tva on tva.vendorid=tv.vendorid join (select tc.countryname,ts.statename,tc1.cityname,tc.countryid, tc1.stateid,tc1.cityid from tabcountry tc left join tabstate ts on ts.countryid=tc.countryid left join tabcity tc1 on tc1.countryid=tc.countryid and tc1.stateid=ts.stateid) td  on tva.countryid=td.countryid and tva.stateid=td.stateid and tva.cityid=td.cityid", NPGSqlHelper.SQLConnString))
                {
                    da.Fill(ds);
                }
            }
            catch
            {
            }
            return ds;
        }
        public DataSet SubcategorywisepurchaseReport(string Categoryid, string SubCategoryid, string FromDate, string ToDate)
        {
            DataSet ds = new DataSet();
            try
            {
                string strQuery = string.Empty;
                //strQuery = "select distinct tp.productcategoryid,tp.categoryname,tp.productsubcategoryid,tp.subcategoryname,sum(totalbasic) as baiscamount,sum(coalesce(transportcharges,0)) as othercharges,sum(coalesce(servicetaxamount,0)+coalesce(vatorcstamount,0)) as tax,sum(coalesce(totaldiscount,0)) as discount from vwmmspurchasestmt tv left join tabmmsproductmst tp on tp.productid=tv.productid  where  datgrndate between '" + FormatDate(FromDate) + "' and '" + FormatDate(ToDate) + "' and subcategoryname !=''  group by tp.productcategoryid,tp.categoryname,tp.productsubcategoryid,tp.subcategoryname order by 2,4";
                //strQuery = "select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid,tv.subcategoryname,sum(totalbasic) as baiscamount,sum(coalesce(transportcharges,0)) as othercharges,sum(coalesce(servicetaxamount,0)+coalesce(vatorcstamount,0)) as tax,sum(coalesce(totaldiscount,0)) as discount,datgrndate as date from vwmmspurchasestmt tv  where  datgrndate between '" + FormatDate(FromDate) + "' and '" + FormatDate(ToDate) + "' and subcategoryname !=''  group by productcategoryid,categoryname,productsubcategoryid,subcategoryname,datgrndate order by 2,4";
                if (Categoryid == "ALL")
                {
                    //strQuery = "select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname,sum(totalbasic) as baiscamount,sum(coalesce(transportcharges,0)) as othercharges,sum(coalesce(servicetaxamount,0)+coalesce(vatorcstamount,0)) as tax,sum(coalesce(totaldiscount,0)) as discount from vwmmspurchasestmt tv group by productcategoryid,categoryname,productsubcategoryid,subcategoryname order by 2,4;";
                    strQuery = "select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname,round(sum(basic-returnamount),2) as baiscamount,sum(coalesce(itemtransportcharges,0)) as othercharges,sum(coalesce(itemtaxamount,0)) as tax,sum(coalesce(itemdiscount,0)) as discount from vwmmspurchasestmt tv where datgrndate between '" + FormatDate(FromDate) + "' and '" + FormatDate(ToDate) + "' group by productcategoryid,categoryname,productsubcategoryid,subcategoryname order by 2,4;";
                }
                else if (Categoryid != null && Categoryid != "" && Categoryid != string.Empty && Categoryid != "undefined" && Categoryid != "ALL" && SubCategoryid != null && SubCategoryid != "" && SubCategoryid != string.Empty && SubCategoryid != "undefined")
                {
                    if (SubCategoryid == "ALL")
                    {                   
                        strQuery = "select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname,round(sum(basic-returnamount),2) as baiscamount,sum(coalesce(itemtransportcharges,0)) as othercharges,sum(coalesce(itemtaxamount,0)) as tax,sum(coalesce(itemdiscount,0)) as discount from vwmmspurchasestmt tv where productcategoryid=" + Categoryid + " and datgrndate between '" + FormatDate(FromDate) + "' and '" + FormatDate(ToDate) + "' group by productcategoryid,categoryname,productsubcategoryid,subcategoryname order by 2,4;";
                        //strQuery = "select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname,sum(totalbasic) as baiscamount,sum(coalesce(transportcharges,0)) as othercharges,sum(coalesce(servicetaxamount,0)+coalesce(vatorcstamount,0)) as tax,sum(coalesce(totaldiscount,0)) as discount from vwmmspurchasestmt tv where  productcategoryid=" + Categoryid + " group by productcategoryid,categoryname,productsubcategoryid,subcategoryname order by 2,4";
                        //strQuery = "select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid,coalesce(tv.subcategoryname,tv.categoryname) as subcategoryname,sum(totalbasic) as baiscamount,sum(coalesce(transportcharges,0)) as othercharges,sum(coalesce(servicetaxamount,0)+coalesce(vatorcstamount,0)) as tax,sum(coalesce(totaldiscount,0)) as discount from vwmmspurchasestmt tv   group by productcategoryid,categoryname,productsubcategoryid,subcategoryname order by 2,4";
                    }
                    else if (SubCategoryid != "ALL")
                    {
                        strQuery = "select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname,round(sum(basic-returnamount),2) as baiscamount,sum(coalesce(itemtransportcharges,0)) as othercharges,sum(coalesce(itemtaxamount,0)) as tax,sum(coalesce(itemdiscount,0)) as discount from vwmmspurchasestmt tv where productcategoryid=" + Categoryid + " and productsubcategoryid=" + SubCategoryid + " and subcategoryname !='' and datgrndate between '" + FormatDate(FromDate) + "' and '" + FormatDate(ToDate) + "' group by productcategoryid,categoryname,productsubcategoryid,subcategoryname order by 2,4;";
                        //strQuery = "select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid, case when tv.productsubcategoryid is null then tv.categoryname else  tv.subcategoryname end as subcategoryname,sum(totalbasic) as baiscamount,sum(coalesce(transportcharges,0)) as othercharges,sum(coalesce(servicetaxamount,0)+coalesce(vatorcstamount,0)) as tax,sum(coalesce(totaldiscount,0)) as discount from vwmmspurchasestmt tv where  productcategoryid=" + Categoryid + " and productsubcategoryid=" + SubCategoryid + " and subcategoryname !=''  group by productcategoryid,categoryname,productsubcategoryid,subcategoryname order by 2,4";
                        //strQuery = "select distinct tv.productcategoryid,tv.categoryname,tv.productsubcategoryid,coalesce(tv.subcategoryname,tv.categoryname) as subcategoryname,sum(totalbasic) as baiscamount,sum(coalesce(transportcharges,0)) as othercharges,sum(coalesce(servicetaxamount,0)+coalesce(vatorcstamount,0)) as tax,sum(coalesce(totaldiscount,0)) as discount from vwmmspurchasestmt tv  where  productcategoryid=" + Categoryid + " and productsubcategoryid=" + SubCategoryid + " and subcategoryname !=''  group by productcategoryid,categoryname,productsubcategoryid,subcategoryname order by 2,4";
                    }
                }
                if (strQuery.Length > 0)
                {
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(strQuery, NPGSqlHelper.SQLConnString))
                    {
                        da.Fill(ds);
                    }
                }
            }
            catch
            {
            }
            return ds;
        }
        public DataSet VendorItemList(string VendorID)
        {
            DataSet ds = new DataSet();
            try
            {
                string strQuery = string.Empty;
                //strQuery = "select distinct tp.productcategoryid,tp.categoryname,tp.productsubcategoryid,tp.subcategoryname,sum(totalbasic) as baiscamount,sum(coalesce(transportcharges,0)) as othercharges,sum(coalesce(servicetaxamount,0)+coalesce(vatorcstamount,0)) as tax,sum(coalesce(totaldiscount,0)) as discount from vwmmspurchasestmt tv left join tabmmsproductmst tp on tp.productid=tv.productid  where  datgrndate between '" + FormatDate(FromDate) + "' and '" + FormatDate(ToDate) + "' and subcategoryname !=''  group by tp.productcategoryid,tp.categoryname,tp.productsubcategoryid,tp.subcategoryname order by 2,4";
                if (VendorID == "ALL")
                {
                    strQuery = "select vchvendorcode as vendorcode,vchvendorname as vendorname,productname,numproductcost as price,vchmeasureduomid  as uom from tabmmsvendormst  tv join tabmmsproductvendors tvp on tv.vendorid=tvp.vendorid left join tabmmsproductmst tp on tvp.productid=tp.productid order by 2,3,5";
                }
                else if (VendorID != null && VendorID != "" && VendorID != string.Empty && VendorID != "undefined" && VendorID != "ALL")
                {
                    strQuery = "select vchvendorcode as vendorcode,vchvendorname as vendorname,productname,numproductcost as price,vchmeasureduomid  as uom from tabmmsvendormst  tv join tabmmsproductvendors tvp on tv.vendorid=tvp.vendorid left join tabmmsproductmst tp on tvp.productid=tp.productid where tv.vendorid in (" + VendorID + ") order by 2,3,5";
                }
                if (strQuery.Length > 0)
                {
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(strQuery, NPGSqlHelper.SQLConnString))
                    {
                        da.Fill(ds);
                    }
                }
            }
            catch
            {
            }
            return ds;
        }
        public DataTable getSubcategoryNames(string CategoryID)
        {
            DataTable dt = new DataTable();
            string strdata = string.Empty;
            try
            {
                // strdata = "select '0' as productsubcategoryid ,'ALL' as productsubcategoryname union all select productsubcategoryid::text,productsubcategoryname from tabmmsproductsubcategorymst where productcategoryid=" + CategoryID + " order by 2;";
                strdata = "select productsubcategoryid,productsubcategoryname from tabmmsproductsubcategorymst where productcategoryid=" + CategoryID + " order by 2;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strdata).Tables[0];
                DataRow dr = dt.NewRow();
                dr["productsubcategoryid"] = 0;
                dr["productsubcategoryname"] = "ALL";
                dt.Rows.InsertAt(dr, 0);
                if (dt.Rows.Count == 0)
                {
                    dt = null;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dt;
        }


        #region MMSSale

        // Duplicate Sale Bill
        public DataSet SaleBillGenerationReport(string SaleNo)
        {
            DataSet ds = new DataSet();
            DataSet dss = new DataSet();
            try
            {
                //string strSaledetails = "select recordid from tabmmssale where saleno='" + SaleNo + "'";
                // long id = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strSaledetails));
                // string strGetDetails = "select vchproductname,numquantity,numsalerate,numsaleamount,vchuom,detailsid from tabmmssaledetails where detailsid=" + id + "";
                //string strGetDetails = "select vchproductname,sum(numquantity) as numquantity,sum(numsaleamount) as numsaleamount,vchuom,detailsid,numunitrate from tabmmssaledetails where detailsid=" + id + " group by vchproductname,vchuom,detailsid,numunitrate";

                string strGetDetails = "SELECT COUNT(DISTINCT VCHPRODUCTNAME) AS ITEMS, VCHPRODUCTNAME,SUM(NUMQUANTITY) AS NUMQUANTITY,SUM(tm.NUMSALEAMOUNT) AS NUMSALEAMOUNT,CASE WHEN VCHUOM ='KILOGRAMS' THEN 'KG' ELSE 'NO.' END AS VCHUOM,DETAILSID,NUMUNITRATE FROM TABMMSSALEDETAILS TM JOIN TABMMSSALE TS ON TS.RECORDID=TM.DETAILSID WHERE SALENO='" + SaleNo + "' group by vchproductname,vchuom,detailsid,numunitrate";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strGetDetails);

            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return ds;
        }

        // Between Dates
        public DataSet GetDataBetweenDates(string fromdate, string todate)
        {
            DataSet ds = new DataSet();
            string GetReportData = string.Empty;
            try
            {
                GetReportData = "select datreconcilationdate,productname,case when vchuom='KILOGRAMS' THEN 'KG' WHEN VCHUOM='NUMBERS' THEN 'NO.' END AS VCHUOM,numstockinstore,numstockinsystem,numvarianceqty from tabmmsstockreconcilation where datreconcilationdate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "'";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, GetReportData);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return ds;
        }

        // Day wise sale
        public DataSet GetMMSSalesData(string fromdate, string todate)
        {
            DataSet ds = new DataSet();

            try
            {
                //string GetMmsSalesData = "select ts.datsaledate,numunitrate,vchproductname,sum(numquantity) as numquantity ,CASE WHEN vchuom='KILOGRAMS' THEN 'KG' WHEN VCHUOM='NUMBERS' THEN 'NO.' END AS VCHUOM,sum(tsd.numsaleamount) as numsaleamount,tp.categoryname from tabmmssaledetails tsd  join tabmmssale ts on tsd.detailsid=ts.recordid join  TABMMSPRODUCTMST tp on vchproductname=tp.productname where datsaledate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' group by vchproductname,numquantity,vchuom,tsd.numsaleamount,tp.categoryname,ts.datsaledate,numunitrate order by vchproductname";
                string GetMmsSalesData = "select datsaledate,vchproductname,sum(numquantity) as numquantity,sum(amount)/sum(numquantity) as numunitrate,sum(amount) as numsaleamount,categoryname,VCHUOM from (select ts.datsaledate,numunitrate,vchproductname,(numquantity) as numquantity,numunitrate* (numquantity) as amount,CASE WHEN vchuom='KILOGRAMS' THEN 'KG' WHEN VCHUOM='NUMBERS' THEN 'NO.' END AS VCHUOM,tp.categoryname from tabmmssaledetails tsd  join tabmmssale ts on tsd.detailsid=ts.recordid join  TABMMSPRODUCTMST tp on vchproductname=tp.productname where datsaledate between '"+FormatDate(fromdate)+"' and '"+FormatDate(todate)+"' )g group by vchproductname,vchuom,categoryname,datsaledate order by vchproductname";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, GetMmsSalesData);                
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return ds;

        }

        public DataSet GetProductWisePurchaseSaleIssueQty(string fromdate,string todate)
        {
            DataSet ds = new DataSet();
            try
            {
                //string GetDetails = "select * from vwproductwisepurchasesaleissueqty order by productname";
                
               // string GetDetails = "select * from vwproductwisepurchasesaleissueqty where purchasedate between '" + fromdate + "' and '" + todate + "' or saledate between '" + fromdate + "' and '" + todate + "' or issuedate between '" + fromdate + "' and '" + todate + "' order by productname";
                string GetDetails = "select t1.productid::text,t1.productname,case when uomname='KILOGRAMS' THEN 'KG' ELSE 'NO.' END AS UOMNAME,coalesce(availableqty,0) as openingqty,t2.*  from   tabmmsproductmst  t1 left join (select vw.productid,coalesce(sum(purchaseqty),0)  as availableqty from vwproductpurchaseconsumereturnqty vw where datgrndate<'" + FormatDate(fromdate) + "' group by productid) vw on t1.productid =vw.productid left join (select PRODUCTID,PRODUCTNAME,case when VCHUOM='KILOGRAMS' THEN 'KG' ELSE 'NO.' END AS VCHUOM,sum(coalesce(purchaseqty,0)) as purchaseqty,sum(coalesce(purchaserate,0)) as purchaserate,sum(coalesce(purchaseamount,0)) as purchaseamount,sum(coalesce(saleqty,0)) as saleqty,sum(coalesce(salerate,0)) as salerate,sum(coalesce(saleamount,0)) as saleamount,sum(coalesce(issueqty,0)) as issueqty,sum(coalesce(issuerate,0)) as issuerate,sum(coalesce(issueamount,0)) as issueamount,sum(coalesce(returnqty,0)) as returnqty,sum(coalesce(returnrate,0)) as returnrate,sum(coalesce(returnamount,0)) as returnamount from(select PRODUCTID,PRODUCTNAME,VCHUOM ,case when TYPE='PURCHASE' then qty end as purchaseqty,case when TYPE='PURCHASE' then RATE end as purchaserate,case when TYPE='PURCHASE' then AMOUNT end as purchaseamount,case when TYPE='SALE' then qty end as saleqty,case when TYPE='SALE' then RATE end as salerate,case when TYPE='SALE' then AMOUNT end as saleamount,case when TYPE='ISSUE' then qty end as issueqty,case when TYPE='ISSUE' then RATE end as  issuerate,case when TYPE='ISSUE' then AMOUNT end as  issueamount,case when TYPE='CANCEL' then qty end as returnqty,case when TYPE='CANCEL' then RATE end as  returnrate,case when TYPE='CANCEL' then AMOUNT end as  returnamount from(SELECT PRODUCTID,PRODUCTNAME,VCHUOM,SUM(NUMRECEIVEDQTY) AS QTY,SUM(AMOUNT) AS AMOUNT ,SUM(AMOUNT)/SUM(NUMRECEIVEDQTY) AS RATE ,TYPE FROM VWMMSPRODUCTWISETRASANCTIONS WHERE DATGRNDATE BETWEEN '" + FormatDate(fromdate) + "' AND '" + FormatDate(todate) + "' GROUP BY PRODUCTID,PRODUCTNAME,VCHUOM,TYPE) g)h group by PRODUCTID,PRODUCTNAME,VCHUOM) t2 on t1.PRODUCTID::text=t2.PRODUCTID order by t2.productname;";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, GetDetails);
                //int Rcount = ds.Tables[0].Rows.Count;
                //for (int i = 0; i < Rcount; i++)
                //{
                //    if (ds.Tables[0].Rows[i][6].ToString() == "KILOGRAMS")
                //    {
                //        ds.Tables[0].Rows[i][6] = "KG";
                //    }
                //    else if (ds.Tables[0].Rows[i][6].ToString() == "NUMBERS")
                //    {
                //        ds.Tables[0].Rows[i][6] = "NO.";
                //    }
                //}
                //ds.AcceptChanges();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return ds;
        }

        public DataTable GetSaleNos()
        {
            DataTable dt = new DataTable();
            string strdata = string.Empty;
            try
            {
                strdata = "select recordid as saleid,saleno from tabmmssale where statusid=1 order by recordid desc limit 20;";
                //strdata = "select saleno from tabmmssale where statusid=1 order by recordid desc limit 20;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strdata).Tables[0];
                DataRow dr = dt.NewRow();
                dr["saleid"] = 0;
                dr["saleno"] = "Select Sale No.";
                dt.Rows.InsertAt(dr, 0);
            }
            catch (Exception)
            {

                throw;
            }
            return dt;
        }

        public DataSet GetBillWiseSalesReport(string fromdate,string todate)
        {
            string strGetData = string.Empty;
            DataSet dt = new DataSet();
            try
            {
                strGetData = "select saleno,datsaledate,vchproductname,numquantity,case when vchuom='KILOGRAMS' then 'KG' when vchuom='NUMBERS' then 'NO.' end as vchuom,numunitrate,td.numsaleamount,tp.vchpaymentmode from tabmmssaledetails td  join tabmmssale ts on ts.recordid=td.detailsid  join tabmmssalepayment tp on td.detailsid=tp.detailsid where datsaledate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' order by datsaledate asc";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strGetData);
                return dt;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Bill Wise Sale Report");
                throw ex;
            }
        }

        public DataTable GetGRNvendornames()
        {
            DataTable dt = new DataTable();
            string strGetData = string.Empty;
            try
            {
                strGetData = "select distinct vchvendorname,vendorid from tabmmsgoodsreceivednote;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strGetData).Tables[0];
                DataRow dr = dt.NewRow();
                dr["vendorid"] = 0;
                dr["vchvendorname"] = "ALL";
                dt.Rows.InsertAt(dr, 0);
            }
            catch (Exception)
            {
                
                throw;
            }
            return dt;
        }

        public DataSet GetGrnReport(string fromdate,string todate,string vendorid)
        {
            DataSet ds = new DataSet();
            string strGetData = string.Empty;
            try
            {
                if(vendorid=="0" || vendorid=="ALL")
                {
                    strGetData = "select datgrndate,tm.vchgrnno,vchvendorname,vendorid,productname,numreceivedqty,numgrnrate,numorderedqty,case when tg.vchuom='KILOGRAMS' then 'KG' when  tg.vchuom='NUMBERS' then 'NO.' end as vchuom,numreceivedqty*numgrnrate as amount from tabmmsgoodsreceivednote tm join tabmmsgoodsreceivednotedetails tg on tm.recordid=tg.grnno where datgrndate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "'order by datgrndate asc; ";                    
                }
                else if(vendorid!="0" || vendorid!="ALL")
                {
                    strGetData = "select datgrndate,tm.vchgrnno,vchvendorname,vendorid,productname,numreceivedqty,numgrnrate,numorderedqty,case when tg.vchuom='KILOGRAMS' then 'KG' when  tg.vchuom='NUMBERS' then 'NO.' end as vchuom,numreceivedqty*numgrnrate as amount from tabmmsgoodsreceivednote tm join tabmmsgoodsreceivednotedetails tg on tm.recordid=tg.grnno where datgrndate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and vendorid='" + vendorid + "' order by datgrndate asc";
                }
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strGetData);
                
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "GRN Wise RePort");
                
                throw;
            }
            return ds;
        }



        #endregion


        #region Productwise Stock Ledger

        public DataSet GetProductwiseStockLedgerReport(string fromdate, string todate, string productid)
        {
            DataSet ds = new DataSet();
            string strGetData = string.Empty;
            try
            {
                if (productid != "0" && productid != "undefined" && productid != null)
                {
                    string strgetData = "select productname from tabmmsproductmst where productid=" + Convert.ToInt32(productid) + "";
                    string product = NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strgetData).ToString();
                    strGetData = "select '' AS TRANSNO, '" + FormatDate(fromdate) + "' AS TRANSDATE,t1.productid::text,t1.productname,coalesce(availableqty,0) as availableqty,CASE WHEN UOMNAME='KILOGRAMS' THEN 'KG' WHEN UOMNAME='NUMBERS' THEN 'NO.' END AS UOMNAME ,'OPENING' AS TYPE from   tabmmsproductmst  t1 left join (select vw.productid,coalesce(sum(purchaseqty),0)  as availableqty from vwproductpurchaseconsumereturnqty vw where datgrndate<'" + FormatDate(fromdate) + "' group by productid) vw on t1.productid =vw.productid  WHERE T1.PRODUCTNAME='" + ManageQuote(product) + "' UNION ALL SELECT VCHGRNNO,DATGRNDATE,PRODUCTID,PRODUCTNAME,SUM(NUMRECEIVEDQTY) AS QTY,CASE WHEN VCHUOM='KILOGRAMS' THEN 'KG' WHEN VCHUOM='NUMBERS' THEN 'NO.' END AS VCHUOM,TYPE FROM vwmmsproductwisetrasanctions WHERE DATGRNDATE BETWEEN '" + FormatDate(fromdate) + "' AND '" + FormatDate(todate) + "'  AND PRODUCTNAME='" + ManageQuote(product) + "' GROUP BY  VCHGRNNO,DATGRNDATE,PRODUCTID,PRODUCTNAME,VCHUOM,TYPE order by transdate,TYPE";
                }
                //else if (productid == "0")
                //{
                //    strGetData = "select '' AS TRANSNO, '" + FormatDate(fromdate) + "' AS TRANSDATE,t1.productid::text,t1.productname,coalesce(availableqty,0) as availableqty,UOMNAME ,'OPENING' AS TYPE from   tabmmsproductmst  t1 left join (select vw.productid,coalesce(sum(purchaseqty),0)  as availableqty from vwproductpurchaseconsumereturnqty vw where datgrndate<'" + FormatDate(fromdate) + "' group by productid) vw on t1.productid =vw.productid  UNION ALL SELECT VCHGRNNO,DATGRNDATE,PRODUCTID,PRODUCTNAME,SUM(NUMRECEIVEDQTY) AS QTY,VCHUOM,TYPE FROM vwmmsproductwisetrasanctions WHERE DATGRNDATE BETWEEN '" + FormatDate(fromdate) + "' AND '" + FormatDate(todate) + "' GROUP BY  VCHGRNNO,DATGRNDATE,PRODUCTID,PRODUCTNAME,VCHUOM,TYPE order by transdate";
                //}

                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strGetData);
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
            return ds;
        }
        public DataTable Getmmsproductnameswithselect()
        {
            DataTable dt = new DataTable();
            string strdata = string.Empty;
            try
            {
                strdata = "select productid,productname from tabmmsproductmst where statusid=1 order by productname;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strdata).Tables[0];
                DataRow dr = dt.NewRow();
                dr["productid"] = 0;
                dr["productname"] = "Select";
                dt.Rows.InsertAt(dr, 0);
            }
            catch (Exception)
            {

                throw;
            }
            return dt;
        }


        #endregion

        #region Daywisewastage

        public DataSet GetDaywiseWastageReport(string fromdate,string todate)
        {
            string strGetData = string.Empty;
            DataSet ds = new DataSet();
            try
            {
               // strGetData = "select datreconcilationdate,productname,case when vchuom='KILOGRAMS' then 'KG' when vchuom='NUMBERS' then 'NO.' end as vchuom,numvarianceqty as wastage from  tabmmsstockreconcilation  where statusid=1 and  datreconcilationdate between '"+FormatDate(fromdate)+"' and '"+FormatDate(todate)+"' group by datreconcilationdate,productname,vchuom,numvarianceqty order by datreconcilationdate";
                strGetData = "select datreconcilationdate,tr.productname,case when tr.vchuom='KILOGRAMS' then 'KG' when tr.vchuom='NUMBERS' then 'NO.' end as vchuom,numvarianceqty as wastage,numgrnrate,numgrnrate*numvarianceqty as amount from  tabmmsstockreconcilation tr  join tabmmsgoodsreceivednotedetails tg on tg.productid=tr.productid where tr.statusid=1 and  datreconcilationdate between '"+FormatDate(fromdate)+"' and '"+FormatDate(todate)+"' group by datreconcilationdate,tr.productname,tr.vchuom,numvarianceqty,numgrnrate order by datreconcilationdate;";
               // strGetData = "select datreconcilationdate,tr.productname,case when tr.vchuom='KILOGRAMS' then 'KG' when tr.vchuom='NUMBERS' then 'NO.' end as vchuom,numvarianceqty as wastage,numgrnrate,numgrnrate*numvarianceqty as amount from  tabmmsstockreconcilation tr  join tabmmsgoodsreceivednotedetails tg on tg.productid=tr.productid  JOIN tabmmsgoodsreceivednote t5 on t5.vchgrnno=tg.vchgrnno where tr.statusid=1  and t5.datgrndate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "'   and  datreconcilationdate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' group by datreconcilationdate,tr.productname,tr.vchuom,numvarianceqty,numgrnrate order by datreconcilationdate;";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strGetData);
                return ds;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        #endregion
    }
}
