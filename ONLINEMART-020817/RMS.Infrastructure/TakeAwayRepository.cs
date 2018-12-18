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
    public class TakeAwayRepository : ITakeAway
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans;
        NpgsqlDataReader npgdr = null;
        string constr = NPGSqlHelper.SQLConnString;
        string GNextID = string.Empty;


        public string GenerateNextIDForGr(string strtablename, string strcolname, int prefix, string strdate, string strColumnDate, string strPrefix)
        {
            try
            {
                if (strPrefix == "RCQ")
                {
                    npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.StoredProcedure, "generatenextid('" + strtablename + "','" + strcolname + "','" + prefix + "','" + strdate + "','" + strColumnDate + "','" + strPrefix + "')", null);
                }
                else
                {
                    npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.StoredProcedure, "generatenextid_generalreceipt('" + strtablename + "','" + strcolname + "','" + prefix + "','" + strdate + "','" + strColumnDate + "','" + strPrefix + "')", null);
                }
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

        public string GenerateNextID(string Table, string Column, string FormName)
        {

            string prefix = Convert.ToString(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select trim(upper(vchprefix)) from tabposcode  where upper(vchmastername)='" + FormName.ToUpper().Trim() + "'"));
            string res = Convert.ToString(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT (coalesce(MAX(coalesce(cast(LTRIM(" + Column + ",'" + prefix.Trim() + "')as numeric),0)),0))+1 as nextno FROM  " + Table + " where  " + Column + " like '%" + prefix.Trim() + "%';"));

            return prefix + res;
        }

        public int GenerateNextIDINTAkeAway(string check)
        {
            int num = 0;
            int count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select COUNT(*) from tabposhdkotdetails  WHERE VCHORDERno   LIKE '" + check + "%';"));
            if (count == 0)
            {

                num = 0;
            }
            else
            {

                //  num = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select max(ltrim(VCHORDERno, '" + check + "')) from tabposhdkotdetails  where VCHORDERno in(  select  max(VCHORDERno) from tabposhdkotdetails WHERE VCHORDERno   LIKE '" + check + "%');"));
                num = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select max(ltrim(VCHORDERno, '" + check + "')) from tabposhdkot  where VCHORDERno in(select 'TW'||max(replace(VCHORDERno,'TW','')::int)  from tabposhdkot WHERE VCHORDERno   LIKE '" + check + "%')  "));
            }

            return num + 1;
        }
        public int GenerateNextThirdParty(string check)
        {
            int num = 0;
            int count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select COUNT(*) from tabposthirdpartykotdetails  WHERE VCHORDERno   LIKE '" + check + "%';"));
            if (count == 0)
            {

                num = 0;
            }
            else
            {

                //  num = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select max(ltrim(VCHORDERno, '" + check + "')) from tabposhdkotdetails  where VCHORDERno in(  select  max(VCHORDERno) from tabposhdkotdetails WHERE VCHORDERno   LIKE '" + check + "%');"));
                num = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select max(ltrim(VCHORDERno, '" + check + "')) from tabposthirdpartykot  where VCHORDERno in(select '" + check + "'||max(replace(VCHORDERno,'" + check + "','')::int)  from tabposthirdpartykot WHERE VCHORDERno   LIKE '" + check + "%')  "));
            }

            return num + 1;
        }

        public int GenerateNextID(string check)
        {
            int num = 0;
            num = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select coalesce(max(cast(coalesce(replace(vchbillno,'" + check + "',''),'0') as numeric)),0) from  (select vchbillno from tabposbillgeneration union select vchbillno from tabposbillcancel ) b where vchbillno like '" + check + "%'"));
            return num + 1;
        }
        public List<ItemsDTO> ShowItemsForSession()
        {
            List<ItemsDTO> lstItems = null;
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    lstItems = new List<ItemsDTO>();
                    //string strInsert = "SELECT ROW_NUMBER() OVER() as rowindex,ITEMID,TPS.ITEMNAME,TPS.ITEMCODE,tpm.numrate,tpm.numrate as ratefixed,tpc.itemcategoryname,tps.subcategoryname,tpc.categoryid,TPM.departmentname,0 AS ITEMCOUNT,'C' as ordervalue FROM TABPOSSESSIONDETAILSMST TPS JOIN TABPOSITEMMST TPM ON TPS.ITEMCODE=TPM.ITEMCODE join tabpositemcategorymst tpc on TPM.categoryid=TPc.categoryid where sessionid in(SELECT sessionid FROM  TABPOSSESSIONMST WHERE statusid=1 and (SPLIT_PART(FROMTIME, ':',1)::INT*60)+SPLIT_PART(FROMTIME, ':',2)::INT <= ((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) AND (SPLIT_PART(TOTIME, ':',1)::INT*60)+SPLIT_PART(TOTIME, ':',2)::INT >=((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) order by sessionid limit 1) union SELECT ROW_NUMBER() OVER() AS ROWINDEX,ITEMID,T1.ITEMNAME,T1.ITEMCODE,T2.NUMRATE,T2.NUMRATE,'DAY SPECIAL' as itemcategoryname,T2.SUBCATEGORYNAME as SUBCATEGORYNAME,0,T2.DEPARTMENTNAME,0 AS ITEMCOUNT,'B' as ordervalue FROM TABPOSDAYSPECIALITEMS T1 JOIN TABPOSITEMMST T2 ON T1.ITEMCODE=T2.ITEMCODE WHERE T1.DATDATE=CURRENT_DATE UNION SELECT ROW_NUMBER() OVER(partition by T2.SUBCATEGORYNAME) AS ROWINDEX,T1.ITEMID,T1.ITEMNAME,T1.ITEMCODE,T2.NUMRATE,T2.NUMRATE,'FREQUENT' as itemcategoryname,T2.SUBCATEGORYNAME,0,T2.DEPARTMENTNAME,COUNT(*) AS ITEMCOUNT,'A' as ordervalue FROM TABRMSKOTDETAILS T1 JOIN TABPOSITEMMST T2 ON T1.ITEMCODE=T2.ITEMCODE GROUP BY T1.ITEMID,T1.ITEMNAME,T1.ITEMCODE,T2.NUMRATE,T2.NUMRATE,SUBCATEGORYNAME,DEPARTMENTNAME order by ITEMCOUNT desc,ordervalue ;";
                    string strInsert = "SELECT ROW_NUMBER() OVER() as rowindex,ITEMID,TPS.ITEMNAME,TPS.ITEMCODE,tpm.numrate,tpm.numrate as ratefixed,tpc.itemcategoryname,tps.subcategoryname,tpc.categoryid,TPM.departmentname,0 AS ITEMCOUNT,'C' as ordervalue FROM TABPOSSESSIONDETAILSMST TPS JOIN TABPOSITEMMST TPM ON TPS.ITEMCODE=TPM.ITEMCODE join tabpositemcategorymst tpc on TPM.categoryid=TPc.categoryid where sessionid in(SELECT sessionid FROM  TABPOSSESSIONMST WHERE statusid=1 and (SPLIT_PART(FROMTIME, ':',1)::INT*60)+SPLIT_PART(FROMTIME, ':',2)::INT <= ((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) AND (SPLIT_PART(TOTIME, ':',1)::INT*60)+SPLIT_PART(TOTIME, ':',2)::INT >=((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) order by sessionid limit 1) union SELECT ROW_NUMBER() OVER() AS ROWINDEX,ITEMID,T1.ITEMNAME,T1.ITEMCODE,T2.NUMRATE,T2.NUMRATE,'DAY SPECIAL' as itemcategoryname,T2.SUBCATEGORYNAME as SUBCATEGORYNAME,0,T2.DEPARTMENTNAME,0 AS ITEMCOUNT,'B' as ordervalue FROM TABPOSDAYSPECIALITEMS T1 JOIN TABPOSITEMMST T2 ON T1.ITEMCODE=T2.ITEMCODE WHERE T1.DATDATE=CURRENT_DATE UNION SELECT ROW_NUMBER() OVER(partition by T2.SUBCATEGORYNAME) AS ROWINDEX,T1.ITEMID,T1.ITEMNAME,T1.ITEMCODE,T2.NUMRATE,T2.NUMRATE,'FREQUENT' as itemcategoryname,T2.SUBCATEGORYNAME,0,T2.DEPARTMENTNAME,COUNT(*) AS ITEMCOUNT,'A' as ordervalue FROM TABRMSKOTDETAILS T1 JOIN TABPOSITEMMST T2 ON T1.ITEMCODE=T2.ITEMCODE join TABPOSSESSIONDETAILSMST TPS on TPS.ITEMCODE=T1.ITEMCODE where sessionid in(SELECT sessionid FROM  TABPOSSESSIONMST WHERE statusid=1 and (SPLIT_PART(FROMTIME, ':',1)::INT*60)+SPLIT_PART(FROMTIME, ':',2)::INT <= ((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) AND (SPLIT_PART(TOTIME, ':',1)::INT*60)+SPLIT_PART(TOTIME, ':',2)::INT >=((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) order by sessionid limit 1) GROUP BY T1.ITEMID,T1.ITEMNAME,T1.ITEMCODE,T2.NUMRATE,T2.NUMRATE,T2.SUBCATEGORYNAME,T2.DEPARTMENTNAME order by ITEMCOUNT desc,ordervalue ;;";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ItemsDTO objlstItems = new ItemsDTO();
                            objlstItems.ItemName = Convert.ToString(npgdr["itemname"]);
                            objlstItems.ItemCode = Convert.ToString(npgdr["itemcode"]);
                            objlstItems.ItemId = Convert.ToInt32(npgdr["itemid"]);
                            objlstItems.Department = Convert.ToString(npgdr["departmentname"]);
                            objlstItems.Rate = Convert.ToDecimal(npgdr["numrate"]);
                            objlstItems.RateFixed = Convert.ToDecimal(npgdr["rateFixed"]);
                            objlstItems.CategoryName = Convert.ToString(npgdr["itemcategoryname"]);
                            objlstItems.subcategoryname = Convert.ToString(npgdr["subcategoryname"]);
                            objlstItems.CategoryId = Convert.ToInt32(npgdr["categoryid"]);
                            objlstItems.rowindex = Convert.ToInt32(npgdr["rowindex"]);
                            objlstItems.Quantity = 1;
                            objlstItems.Status = "N";
                            lstItems.Add(objlstItems);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowSessionForKOTtaking");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstItems;

        }

        public int SaveHomeDelivery(string Session, CustomerDetails CustomerDetails, List<ItemsDTO> ItemDetails)
        {

            int Count = 0;
            string CustomerID = string.Empty;

            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();
            try
            {
                if (CustomerDetails.OrderUpdate != "0")
                {
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "insert into tabposhdkotchange select * from tabposhdkot where vchorderno='" + ManageQuote(CustomerDetails.OrderUpdate) + "'");
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "insert into tabposhdkotchangedetails select * from tabposhdkotdetails where vchorderno='" + ManageQuote(CustomerDetails.OrderUpdate) + "'");
                    CustomerID = NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select customerid||'-'||vchcustid||'-'||recordid as curomerid from tabposhdkot where vchorderno='" + ManageQuote(CustomerDetails.OrderUpdate) + "'").ToString();
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "delete from tabposhdkotdetails where vchorderno='" + ManageQuote(CustomerDetails.OrderUpdate) + "'");
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "delete from tabposhdkot where vchorderno='" + ManageQuote(CustomerDetails.OrderUpdate) + "'");

                    if (CustomerDetails.Discount == "") { CustomerDetails.Discount = "0"; }
                    if (CustomerDetails.DeliverCharges == "") { CustomerDetails.DeliverCharges = "0"; }
                    if (CustomerDetails.servicetax == null || CustomerDetails.servicetax == "") { CustomerDetails.servicetax = "0"; }

                    var k = CustomerID.Split('-')[0];
                    string strInsertOrder = "INSERT INTO tabposhdkot(vchorderno, datorderdate, customerid, vchcustid, numitemtotal,vchdiscounttype,numdiscount, vchdeliverychargesstatus, numdeliverycharges, numnetpayable,vchmodeofpayment, vchstatus, statusid, createdby, createddate,vchpincode,sessionid,numservicetax,numservicetaxvalue,numvattax,numvattaxvalue) VALUES ('" + ManageQuote(CustomerDetails.OrderUpdate) + "','" + FormatDate(CustomerDetails.OrderDate) + "','" + CustomerID.Split('-')[0] + "','" + CustomerID.Split('-')[1] + "','" + ManageQuote(CustomerDetails.OrderTotal) + "','" + ManageQuote(CustomerDetails.discounttype) + "','" + ManageQuote(CustomerDetails.Discount) + "','" + CustomerDetails.vchdeliverychargesstatus + "','" + ManageQuote(CustomerDetails.DeliverCharges) + "','" + ManageQuote(CustomerDetails.OrderNetTotal) + "','" + ManageQuote(CustomerDetails.ModeOfPayment) + "','ORI'," + CustomerDetails.statusid + "," + CustomerDetails.createdby + ",current_timestamp,'" + ManageQuote(CustomerDetails.PinCode) + "'," + Session + "," + CustomerDetails.servicetax + "," + CustomerDetails.servicetaxvalue + "," + CustomerDetails.VatTax + "," + CustomerDetails.VatTaxAmount + ") returning recordid;";
                    int OrderidReturn = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strInsertOrder));
                    foreach (var Item in ItemDetails)
                    {
                        string Items = "INSERT INTO tabposhdkotdetails(orderno, vchorderno, itemid, itemname, itemcode, itemqty,numitemrate, statusid, createdby, createddate,vchitemstatus)    VALUES (" + OrderidReturn + ",'" + CustomerDetails.OrderUpdate + "','" + Item.ItemId + "','" + Item.ItemName + "','" + Item.ItemCode + "'," + Item.Quantity + "," + Item.ActualRate + "," + CustomerDetails.statusid + "," + CustomerDetails.createdby + ",current_timestamp,'" + Item.Status + "');";
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, Items);
                    }
                    //NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "UPDATE tabposhdkot SET  numitemtotal=" + CustomerDetails.OrderTotal + ", numdiscount=" + CustomerDetails.Discount + ", vchdeliverychargesstatus='" + CustomerDetails.NoDeliverycharges + "', numdeliverycharges=" + CustomerDetails.DeliverCharges + ", numnetpayable=" + CustomerDetails.OrderNetTotal + ", vchstatus='ORI',  vchdiscounttype='" + CustomerDetails.discounttype + "' , vchpincode='" + CustomerDetails.PinCode + "' WHERE vchorderno='" + CustomerDetails.OrderUpdate + "';");
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabposcustomers set vchcustomername='" + CustomerDetails.CustomerName + "', vchcustomeraddress='" + CustomerDetails.CustomerAddress + "',vchlandmark='" + CustomerDetails.LandMark + "' where vchcustomercontactno='" + CustomerDetails.Customermobile + "';");

                }
                else
                {

                    string orderid = GenerateNextID("tabposhdkot", "vchorderno", "New Order");
                    string custid = GenerateNextID("tabposcustomers", "vchcustid", "Customer");
                    if (CustomerDetails.Discount == null)
                    {

                        CustomerDetails.Discount = "0";
                    }
                    if (CustomerDetails.NoDeliverycharges == 'Y')
                    {

                        string chargesstr = "Insert into tabmmshomedeliverycharges (orderid,detailsid1,vchpincode,vchcharges,statusid,createdby,createddate)values('" + orderid + "','" + custid + "','" + CustomerDetails.PinCode + "','" + CustomerDetails.DeliverCharges + "','1','" + CustomerDetails.createdby + "',CURRENT_TIMESTAMP);";
                        Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, chargesstr));
                    }

                    if (CustomerDetails.servicetax == null || CustomerDetails.servicetax == "") { CustomerDetails.servicetax = "0"; }
                    string VchKOTID = string.Empty;




                    CustomerDetails.CustomerIDGen = custid;
                    string strInsert = "INSERT INTO tabposcustomers(vchcustid, vchcustomername, vchcustomeraddress,vchcustomercontactno, vchcustomeremail, createdby, createddate,statusid,vchlandmark)  VALUES ('" + ManageQuote(CustomerDetails.CustomerIDGen) + "','" + ManageQuote(CustomerDetails.CustomerName) + "','" + CustomerDetails.CustomerAddress + "','" + ManageQuote(CustomerDetails.Customermobile) + "','" + ManageQuote(CustomerDetails.Email) + "'," + CustomerDetails.createdby + ",current_timestamp," + CustomerDetails.statusid + ",'" + ManageQuote(CustomerDetails.LandMark) + "') returning customerid;";
                    int customerid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert));

                    string strInsertOrder = "INSERT INTO tabposhdkot(vchorderno, datorderdate, customerid, vchcustid, numitemtotal,vchdiscounttype,numdiscount, vchdeliverychargesstatus, numdeliverycharges, numnetpayable,vchmodeofpayment, vchstatus, statusid, createdby, createddate,vchpincode,sessionid,numservicetax,numservicetaxvalue,numvattax,numvattaxvalue) VALUES ('" + orderid + "','" + FormatDate(CustomerDetails.OrderDate) + "','" + customerid + "','" + custid + "','" + ManageQuote(CustomerDetails.OrderTotal) + "','" + ManageQuote(CustomerDetails.discounttype) + "','" + ManageQuote(CustomerDetails.Discount) + "','" + CustomerDetails.vchdeliverychargesstatus + "','" + ManageQuote(CustomerDetails.DeliverCharges) + "','" + ManageQuote(CustomerDetails.OrderNetTotal) + "','" + ManageQuote(CustomerDetails.ModeOfPayment) + "','ORI'," + CustomerDetails.statusid + "," + CustomerDetails.createdby + ",current_timestamp,'" + ManageQuote(CustomerDetails.PinCode) + "'," + Session + "," + CustomerDetails.servicetax + "," + CustomerDetails.servicetaxvalue + "," + CustomerDetails.VatTax + "," + CustomerDetails.VatTaxAmount + ") returning recordid;";
                    int OrderidReturn = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strInsertOrder));

                    foreach (var Item in ItemDetails)
                    {
                        string Items = "INSERT INTO tabposhdkotdetails(orderno, vchorderno, itemid, itemname, itemcode, itemqty,numitemrate, statusid, createdby, createddate)    VALUES (" + OrderidReturn + ",'" + orderid + "','" + Item.ItemId + "','" + Item.ItemName + "','" + Item.ItemCode + "'," + Item.Quantity + "," + Item.ActualRate + "," + CustomerDetails.statusid + "," + CustomerDetails.createdby + ",current_timestamp);";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Items);
                    }

                }
                trans.Commit();
            }

            catch (Exception ex)
            {
                Count = 1;
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "CreateKOT");
                CloseCon(con);
                throw;
            }
            return Count;
        }

        public DataTable DeliveryItemsForCustomer()
        {
            DataTable dt = new DataTable();

            try
            {
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select to_char(tphd.createddate,'dd-MM-YYYY HH12:MM') as ordertime,vchdeliveryboyderails,tpbg.vchbillno as BillNo,tphd.vchstatus  as status,case when tphd.vchstatus='ORI' then 'In Preparation' when tphd.vchstatus='ORO' then 'Out For Delivery' when tphd.vchstatus='ORP' then 'Payment Made' when tphd.vchstatus='ORRD' then 'Order Prepared' end as vchstatus  ,tpc.vchcustomername  ,tpc.vchcustomercontactno ,vchorderno, datorderdate, tphd.customerid, tphd.vchcustid, numitemtotal,tphd.vchdiscounttype,tphd.numdiscount, tphd.vchdeliverychargesstatus, tphd.numdeliverycharges, tphd.numnetpayable,tphd.vchmodeofpayment, tphd.statusid, tphd.createdby, tphd.createddate from tabposhdkot tphd join tabposcustomers tpc on tphd.customerid=tpc.customerid left join tabposbillgenerationdetails tpbg  on  tpbg.vchkotid=tphd.vchorderno where tphd.vchstatus not in('ORP','ORC','ORR') group by  tphd.recordid,tpbg.vchbillno,status, vchstatus  ,tpc.vchcustomername  ,tpc.vchcustomercontactno ,vchorderno, datorderdate, tphd.customerid, tphd.vchcustid, numitemtotal,tphd.vchdiscounttype,tphd.numdiscount, tphd.vchdeliverychargesstatus, tphd.numdeliverycharges, tphd.numnetpayable,tphd.vchmodeofpayment, tphd.statusid, tphd.createdby, tphd.createddate,vchdeliveryboyderails order by tphd.recordid desc  ").Tables[0];
            }
            catch (Exception ex)
            {

            }

            return dt;
        }

        public bool UpdateReturnStatus(string OrderReason, string Nameofcust, string OrderNo, string contactno, decimal netamount)
        {

            bool isSaved = false;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "insert into tabposhdorderreturn(vchorderno,vchcustomername ,vchcontactno , numbillamount,vchorderreturnreson,statusid,createdby,createdate) values('" + OrderNo + "','" + Nameofcust + "','" + contactno + "','" + netamount + "','" + OrderReason + "',1,1,current_timestamp)");
                NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "UPDATE tabposbillgeneration SET vchreturnstatus = 'R' where vchbillno in(select t1.vchbillno FROM  tabposbillgeneration t1 JOIN tabposbillgenerationdetails t2 ON t1.vchbillno = t2.vchbillno where vchkotid='" + OrderNo + "') ");
                NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "update tabposhdkot set vchstatus='ORR' where vchorderno='" + OrderNo + "';");
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
                    con.ClearPool();
                }
            }
            return isSaved;


        }

        public string UpdateeliveryStatus(string OderNo, string Status, string DeliveryBoydetails, string createdby)
        {
            string updateStataus = string.Empty;
            string billno = string.Empty;

            if (Status == "Out For Delivery")
            {
                updateStataus = "ORO";


            }
            if (Status == "In Preparation")
            {
                updateStataus = "ORI";

            }
            if (Status == "Payment Made")
            {
                updateStataus = "ORP";

            }
            if (Status == "Order Cancel")
            {
                updateStataus = "ORC";

            }
            if (Status == "Order Prepared")
            {
                updateStataus = "ORRD";

            }
            bool isSaved = false;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }



                if (Status == "Out For Delivery")
                {
                    int orderid = GenerateNextID("HB");
                    billno = "HB" + (orderid);

                    string StrInsert = "insert into tabposbillgeneration(vchbillno,datkotdate,tableid,numtotalamount,numdiscount,numgross,numnet,numvoucherdiscount,numservicetax,numservicetaxvale,numservicecharges,numservicechargevale,vchhomedeliverystatus,createdby,statusid,vchdeliverycharges,createddate,vattax,vattaxvalue) select '" + billno + "' as vchbillno,datorderdate,42 as tableid, numitemtotal,numdiscount,(numitemtotal-case when upper(vchdiscounttype)='PER' then (numitemtotal*numdiscount)/100 else numdiscount end )as numgross ,numnetpayable,0 as numvoucherdiscount,numservicetax,numservicetaxvalue,0 as numservicecharges,0 as numservicechargevale,(select ('H'||'-'||'" + OderNo + "') vchhomedeliverystatus from tabposhdkot where vchorderno='" + OderNo + "') as vchhomedeliverystatus," + createdby + " as createdby,1 as statusid,numdeliverycharges,current_timestamp as time,numvattax,numvattaxvalue  from tabposhdkot where vchorderno='" + OderNo + "'";
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, StrInsert);
                    string StrInsertDetails = "insert into  tabposbillgenerationdetails (detailsid,vchbillno,kotid,vchkotid,itemid,itemname,itemcode,itemqty,itemrate,vchitemchargableornot ,numamount) select  (select recordid from tabposbillgeneration where vchbillno='" + billno + "') as detailsid ,'" + billno + "' as vchbillno,recordid,  vchorderno,itemid,itemname,itemcode,itemqty,numitemrate,'N' as vchitemchargableornot ,numitemrate from tabposhdkotdetails where vchorderno='" + OderNo + "'";
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, StrInsertDetails);
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "update tabposhdkot set vchdeliveryboyderails ='" + DeliveryBoydetails + "' where vchorderno='" + OderNo + "'");
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "update tabposhdkot set vchstatus='" + updateStataus + "' where vchorderno='" + OderNo + "';");
                }
                if (Status == "Order Prepared")
                {
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "update tabposhdkot set vchstatus='" + updateStataus + "' where vchorderno='" + OderNo + "';");
                }
                if (Status == "Order Cancel")
                {
                    string StrInsertCancel = "INSERT INTO tabposhdkotcancel(vchorderno, datorderdate, customerid, vchcustid, numitemtotal,numdiscount, vchdeliverychargesstatus, numdeliverycharges, numnetpayable,vchmodeofpayment, vchstatus, statusid, createdby, createddate,vchdiscounttype) select vchorderno, datorderdate, customerid, vchcustid, numitemtotal, numdiscount, vchdeliverychargesstatus, numdeliverycharges, numnetpayable,  vchmodeofpayment, vchstatus, statusid, createdby, createddate,   vchdiscounttype from tabposhdkot where vchorderno='" + OderNo + "'";
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, StrInsertCancel);
                    string StrInsertCancelDetails = "INSERT INTO tabposhdkotcanceldetails(orderno, vchorderno, itemid, itemname, itemcode, itemqty, numitemrate, statusid, createdby, createddate) select orderno, vchorderno, itemid, itemname, itemcode, itemqty,  numitemrate, statusid, createdby, createddate from tabposhdkotdetails where vchorderno='" + OderNo + "'";
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, StrInsertCancelDetails);

                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "delete from tabposhdkotdetails where vchorderno='" + OderNo + "'");
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "delete from tabposhdkot where vchorderno='" + OderNo + "'");
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "update tabposhdkot set vchstatus='" + updateStataus + "' where vchorderno='" + OderNo + "';");
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
                    con.ClearPool();
                }
            }
            return isSaved + "-" + billno;

        }

        public int SaveHomeDeliveryCharges(DeliveryChargesDTO DeliveryCharges)
        {
            int Count = 0;
            string chargesstr = string.Empty;
            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();
            try
            {
                string orderid = GenerateNextID("tabposhdkot", "vchorderno", "New Order");
                string custid = GenerateNextID("tabposcustomers", "vchcustid", "Customer");
                chargesstr = "Insert into tabmmshomedeliverycharges (orderid,detailsid1,vchpincode,vchcharges,statusid,createdby,createddate)values('" + orderid + "','" + custid + "','" + DeliveryCharges.vchpincode + "','" + DeliveryCharges.vchcharges + "','" + DeliveryCharges.createdby + "','" + DeliveryCharges.vchstatus + "',CURRENT_TIMESTAMP);";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, chargesstr));
                if (Count == 0)
                {
                    trans.Commit();
                }
                else
                {
                    Count = 1;
                }

            }
            catch (Exception ex)
            {
                Count = 1;
                trans.Rollback();
                // EventLogger.WriteToErrorLog(ex, "CreateKOT");
                CloseCon(con);
                throw ex;
            }
            return Count;
        }

        //  string strInsert = "";

        public bool UpdateDeliveryCharges(DeliveryChargesDTO DeliveryCharges)
        {
            string Chargestatus = "Delivery Charges Updated";
            bool isSaved = false;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "update tabmmshomedeliverycharges set vchpincode='" + DeliveryCharges.vchpincode + "',vchcharges='" + DeliveryCharges.vchcharges + "',status='" + Chargestatus + "',modifiedby='" + DeliveryCharges.modifiedby + "',modifieddate=CURRENT_TIMESTAMP where recordid='" + DeliveryCharges.recordid + "';");
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
                    con.ClearPool();
                }
            }
            return isSaved;


        }

        public DataTable DeliveryChargesDetails()
        {
            DataTable dt = new DataTable();

            try
            {
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select recordid,orderid,detailsid,recordid,vchpincode,vchcharges,statusid from tabmmshomedeliverycharges where statusid=1 order by recordid ").Tables[0];
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public bool DeleteDeliveryCharges(DeliveryChargesDTO DeliveryCharges)
        {
            string Chargestatus = "Delivery Charges Deleted";
            bool isSaved = false;
            //status
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "update tabmmshomedeliverycharges set statusid='2',status='" + Chargestatus + "' where recordid='" + DeliveryCharges.recordid + "';");
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
                    con.ClearPool();
                }
            }
            return isSaved;

        }

        private void CloseCon(NpgsqlConnection con)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
                con.Dispose();
                con.ClearPool();
            }
        }


        public int SaveTakeAwayAthotel(string session, CustomerDetails CustomerDetails, List<ItemsDTO> ItemDetails, out string billNo, out string take)
        {

            int Count = 0;
            string CustomerID = string.Empty;
            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();



            string VchKOTID = string.Empty;
            try
            {


                if (CustomerDetails.OrderUpdate != "0")
                {

                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "delete from tabposbillgenerationdetails where vchbillno='" + CustomerDetails.OrderPreviousBillNo + "'");
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "delete from tabposbillgeneration where vchbillno='" + CustomerDetails.OrderPreviousBillNo + "'");

                    take = CustomerDetails.OrderUpdate;
                    int nextid = GenerateNextID("TB");
                    string billno = "TB" + (nextid);
                    billNo = billno;



                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "insert into tabposhdkotchange select * from tabposhdkot where vchorderno='" + ManageQuote(CustomerDetails.OrderUpdate) + "'");
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "insert into tabposhdkotchangedetails select * from tabposhdkotdetails where vchorderno='" + ManageQuote(CustomerDetails.OrderUpdate) + "'");

                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "delete from tabposhdkotdetails where vchorderno='" + ManageQuote(CustomerDetails.OrderUpdate) + "'");
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "delete from tabposhdkot where vchorderno='" + ManageQuote(CustomerDetails.OrderUpdate) + "'");

                    if (CustomerDetails.Discount == "") { CustomerDetails.Discount = "0"; }
                    if (CustomerDetails.servicetax == null || CustomerDetails.servicetax == "") { CustomerDetails.servicetax = "0"; }
                    string strInsertOrder = "INSERT INTO tabposhdkot(vchorderno, datorderdate,  numitemtotal,  numnetpayable, vchstatus, statusid, createdby, createddate, numdiscount,vchdiscounttype,sessionid,numservicetax,numservicetaxvalue,numvattax,numvattaxvalue, numdeliverycharges,vchdeliverychargesstatus) VALUES ('" + CustomerDetails.OrderUpdate + "','" + FormatDate(CustomerDetails.OrderDate) + "','" + ManageQuote(CustomerDetails.OrderTotal) + "','" + ManageQuote(CustomerDetails.OrderNetTotal) + "','TK'," + CustomerDetails.statusid + "," + CustomerDetails.createdby + ",current_timestamp,'" + ManageQuote(CustomerDetails.Discount) + "','" + ManageQuote(CustomerDetails.discounttype) + "'," + session + "," + CustomerDetails.servicetax + "," + CustomerDetails.servicetaxvalue + "," + CustomerDetails.VatTax + "," + CustomerDetails.VatTaxAmount + "," + CustomerDetails.OtherCharges + ",'" + CustomerDetails.vchdeliverychargesstatus + "') returning recordid;";
                    int OrderidReturn = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strInsertOrder));
                    foreach (var Item in ItemDetails)
                    {
                        string Items = "INSERT INTO tabposhdkotdetails(orderno, vchorderno, itemid, itemname, itemcode, itemqty,numitemrate, statusid, createdby, createddate,vchitemstatus)    VALUES (" + OrderidReturn + ",'" + CustomerDetails.OrderUpdate + "','" + Item.ItemId + "','" + Item.ItemName + "','" + Item.ItemCode + "'," + Item.Quantity + "," + Item.ActualRate + "," + CustomerDetails.statusid + "," + CustomerDetails.createdby + ",current_timestamp,'" + Item.Status + "');";
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, Items);
                    }


                    string StrInsert = "insert into tabposbillgeneration(vchbillno,datkotdate,tableid,numtotalamount,numdiscount,numgross,numnet,numvoucherdiscount,numservicetax,numservicetaxvale,numservicecharges,numservicechargevale,vchhomedeliverystatus,createdby,statusid,createddate,vattax,vattaxvalue,vchdeliverycharges) select '" + billno + "' as vchbillno,datorderdate,48  as tableid, numitemtotal, numdiscount,(numitemtotal-case when upper(vchdiscounttype)='PER' then (numitemtotal*numdiscount)/100 else numdiscount end )as numgross ,numnetpayable,0 as numvoucherdiscount, numservicetax,numservicetaxvalue,0 as numservicecharges,0 as numservicechargevale,'T-" + CustomerDetails.OrderUpdate + "' as vchhomedeliverystatus," + CustomerDetails.createdby + " as createdby,1 as statusid,current_timestamp as time,numvattax,numvattaxvalue,NUMDELIVERYCHARGES  from tabposhdkot where vchorderno='" + CustomerDetails.OrderUpdate + "'";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, StrInsert);
                    string StrInsertDetails = "insert into  tabposbillgenerationdetails (detailsid,vchbillno,kotid,vchkotid,itemid,itemname,itemcode,itemqty,itemrate,vchitemchargableornot ,numamount) select  (select recordid from tabposbillgeneration where vchbillno='" + billno + "') as detailsid ,'" + billno + "' as vchbillno,recordid,  vchorderno,itemid,itemname,itemcode,itemqty,numitemrate,'N' as vchitemchargableornot ,numitemrate from tabposhdkotdetails where vchorderno='" + CustomerDetails.OrderUpdate + "'";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, StrInsertDetails);
                }

                else
                {


                    int orderid = GenerateNextIDINTAkeAway("TW");
                    string ordername = "TW" + orderid;
                    take = ordername;
                    int nextid = GenerateNextID("TB");
                    string billno = "TB" + (nextid);
                    billNo = billno;
                    if (CustomerDetails.Discount == null)
                    {

                        CustomerDetails.Discount = "0";
                    }
                    if (CustomerDetails.servicetax == null || CustomerDetails.servicetax == "")
                    {

                        CustomerDetails.servicetax = "0";
                    }
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "insert into tabposkotprint(vchkotid,datkotdate,vchstatus) values('" + ordername + "','" + FormatDate(CustomerDetails.OrderDate) + "','N')");

                    string strInsertOrder = "INSERT INTO tabposhdkot(vchorderno, datorderdate,  numitemtotal,  numnetpayable, vchstatus, statusid, createdby, createddate, numdiscount,vchdiscounttype,sessionid,numservicetax,numservicetaxvalue,numvattax,numvattaxvalue, numdeliverycharges,vchdeliverychargesstatus) VALUES ('" + ordername + "','" + FormatDate(CustomerDetails.OrderDate) + "','" + ManageQuote(CustomerDetails.OrderTotal) + "','" + ManageQuote(CustomerDetails.OrderNetTotal) + "','TK'," + CustomerDetails.statusid + "," + CustomerDetails.createdby + ",current_timestamp,'" + ManageQuote(CustomerDetails.Discount) + "','" + ManageQuote(CustomerDetails.discounttype) + "'," + session + "," + CustomerDetails.servicetax + "," + CustomerDetails.servicetaxvalue + "," + CustomerDetails.VatTax + "," + CustomerDetails.VatTaxAmount + "," + CustomerDetails.OtherCharges + ",'" + CustomerDetails.vchdeliverychargesstatus + "') returning recordid;";
                    int OrderidReturn = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strInsertOrder));

                    foreach (var Item in ItemDetails)
                    {
                        string Items = "INSERT INTO tabposhdkotdetails(orderno, vchorderno, itemid, itemname, itemcode, itemqty,numitemrate, statusid, createdby, createddate)    VALUES (" + OrderidReturn + ",'" + ordername + "','" + Item.ItemId + "','" + Item.ItemName + "','" + Item.ItemCode + "'," + Item.Quantity + "," + Item.ActualRate + "," + CustomerDetails.statusid + "," + CustomerDetails.createdby + ",current_timestamp);";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Items);
                    }

                    string StrInsert = "insert into tabposbillgeneration(vchbillno,datkotdate,tableid,numtotalamount,numdiscount,numgross,numnet,numvoucherdiscount,numservicetax,numservicetaxvale,numservicecharges,numservicechargevale,vchhomedeliverystatus,createdby,statusid,createddate,vattax,vattaxvalue,vchdeliverycharges) select '" + billno + "' as vchbillno,datorderdate,48  as tableid, numitemtotal,numdiscount,(numitemtotal-case when upper(vchdiscounttype)='PER' then (numitemtotal*numdiscount)/100 else numdiscount end )as numgross  ,numnetpayable,0 as numvoucherdiscount, numservicetax, numservicetaxvalue,0 as numservicecharges,0 as numservicechargevalue,'T-" + ordername + "' as vchhomedeliverystatus," + CustomerDetails.createdby + " as createdby,1 as statusid,current_timestamp as time,numvattax,numvattaxvalue,NUMDELIVERYCHARGES  from tabposhdkot where vchorderno='" + ordername + "'";
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, StrInsert);
                    string StrInsertDetails = "insert into  tabposbillgenerationdetails (detailsid,vchbillno,kotid,vchkotid,itemid,itemname,itemcode,itemqty,itemrate,vchitemchargableornot ,numamount) select  (select recordid from tabposbillgeneration where vchbillno='" + billno + "') as detailsid ,'" + billno + "' as vchbillno,recordid,  vchorderno,itemid,itemname,itemcode,itemqty,numitemrate,'N' as vchitemchargableornot ,numitemrate from tabposhdkotdetails where vchorderno='" + ordername + "'";
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, StrInsertDetails);

                    if (Count == 0)
                    {
                        trans.Commit();
                    }

                    else
                    {
                        Count = 2;
                    }
                }
            }

            catch (Exception ex)
            {
                Count = 1;
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "CreateKOT");
                CloseCon(con);
                throw;
            }

            return Count;
        }

        public string getdeliveryCharges(string PinCode)
        {

            string charges = string.Empty;
            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();




            try
            {
                charges = Convert.ToString(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select vchcharges from tabmmshomedeliverycharges where vchpincode ='" + PinCode + "'"));

            }

            catch (Exception ex)
            {

                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "CreateKOT");
                CloseCon(con);
                throw;
            }

            return charges;
        }


        public DataTable getCustomerInformation(string MobileNo)
        {
            DataTable dt = new DataTable();

            try
            {
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select vchcustomername,vchcustomeraddress,vchlandmark from tabposcustomers where  vchcustomercontactno ='" + MobileNo + "' group by vchcustomername,vchcustomeraddress,vchlandmark").Tables[0];
            }
            catch (Exception ex)
            {

            }

            return dt;
        }

        public DataTable GetDeliveryBoyDetails()
        {
            DataTable dt = new DataTable();

            try
            {
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select vchdeliveryboyid, (vchname||'-'|| nummobileno||'-'||vchvehicleno) as vchname from tabrmsdeliveryboydetails  where statusid=1").Tables[0];
            }
            catch (Exception ex)
            {

            }

            return dt;
        }

        public DataTable GetItemsOfSpecicOrder(string orderno)
        {
            DataTable dt = new DataTable();

            try
            {
                if (orderno.StartsWith("HD"))
                {
                    dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select tphk.vchdiscounttype,tphk.numvattax,tphk.numvattaxvalue,tphk.NUMSERVICETAX,tphk.numservicetaxvalue,tphkd.itemname,itemqty ,numitemrate ,itemid,itemcode,numnetpayable,numdiscount,numdeliverycharges,vchcustomername,vchcustomercontactno,vchcustomeraddress,vchcustomeremail,vchlandmark,vchpincode,orderno,vchdiscounttype from tabposhdkot tphk join tabposhdkotdetails tphkd on tphk.vchorderno=tphkd.vchorderno join tabposcustomers tpc on tpc.customerid=tphk.customerid where tphk.vchorderno='" + orderno + "'").Tables[0];
                }
                else if (orderno.StartsWith("TW"))
                {

                    dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select  tphk.vchdiscounttype,tphk.numvattax,tphk.numvattaxvalue,tphk.NUMSERVICETAX,tphk.numservicetaxvalue,tphkd.itemname,itemqty ,numitemrate ,itemid,itemcode,numnetpayable,numdiscount,numdeliverycharges,orderno,vchdiscounttype from tabposhdkot tphk join tabposhdkotdetails tphkd on tphk.vchorderno=tphkd.vchorderno  where tphk.vchorderno='" + orderno + "'").Tables[0];
                }
                else
                {

                    dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select tphkd.itemname,itemqty ,numitemrate ,itemid,itemcode,numnetpayable,numdiscount,numdeliverycharges,orderno,vchdiscounttype from tabposthirdpartykot tphk join tabposthirdpartykotdetails tphkd on tphk.vchorderno=tphkd.vchorderno   where tphk.vchorderno='" + orderno + "'").Tables[0];

                }
            }
            catch (Exception ex)
            {

            }

            return dt;
        }

        public DataTable DeliveryItemsForCustomerInTakeAway()
        {
            DataTable dt = new DataTable();

            try
            {
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select tpb.recordid,to_char(tphd.createddate,'dd-MM-YYYY HH12:MM') as ordertimevchorderno,vchorderno,tpb.vchbillno,numnet from tabposhdkot tphd   join tabposbillgenerationdetails tpbg  on tpbg.vchkotid=tphd.vchorderno join tabposbillgeneration tpb on tpb.vchbillno=tpbg.vchbillno where vchstatus like 'TK' group by tpb.recordid,ordertimevchorderno,vchorderno,tpb.vchbillno,numnet  order by tpb.recordid desc").Tables[0];
            }
            catch (Exception ex)
            {

            }

            return dt;
        }

        #region ThirdParty


        public bool SaveThirdPartyVendor(ThirdParty Third)
        {
            int Count = 0;
            string CustomerID = string.Empty;

            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();

            if (Third.Duration == null)
            {

                Third.Duration = "00 Days";

            }

            try
            {
                string ThirdPartyId = Third.ThirdPartyName.ToUpper().Replace(" ", "").Substring(0, 3);
                string strInsertOrder = "INSERT INTO tabposthirdpartymst(vchpartyid, vchpartyname, vchcontactno, vchaddress,vchemail, vchmodeofpayment, datstartdate, vchbillgenerationmode, vchimageid,numnoofdays, nummargin, statusid, createdby, createddate,vchmargintype)VALUES ('" + ThirdPartyId + "','" + ManageQuote(Third.ThirdPartyName.ToUpper()) + "','" + ManageQuote(Third.ContactNo) + "','" + ManageQuote(Third.Address) + "','null','" + ManageQuote(Third.ModeOfPayment) + "','" + FormatDate(Third.fromdate) + "','null','" + ManageQuote(Third.VendorImage) + "'," + Third.Duration.Substring(0, 2) + "," + ManageQuote(Third.MarginAmount) + "," + Third.statusid + "," + Third.createdby + ",current_timestamp,'" + ManageQuote(Third.Margintype) + "');";
                (NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsertOrder)).ToString();

                trans.Commit();
            }

            catch (Exception ex)
            {
                Count = 1;
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "CreateKOT");
                CloseCon(con);
                throw;
            }
            return true;
        }


        public DataTable getImagesOfThirParty()
        {
            DataTable dt = new DataTable();

            try
            {
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select m.recordid,m.vchpartyname,m.vchpartyid,m.vchimageid,coalesce(count(datorderdate),0) as Nooforders from tabposthirdpartymst m left join tabposthirdpartykot t on m.vchpartyid=t.vchpartyid and datorderdate::date=current_date and vchstatus IN ('TPI','TPOR')  GROUP BY m.recordid,m.vchpartyname,m.vchpartyid,m.vchimageid ").Tables[0];
            }
            catch (Exception ex)
            {

            }

            return dt;
        }

        public int SaveThirdPartyNewOrderDetails(string session, CustomerDetails CustomerDetails, List<ItemsDTO> ItemDetails, out string billNo, out string take)
        {

            int Count = 0;
            string CustomerID = string.Empty;
            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();
            int orderid = GenerateNextThirdParty(CustomerDetails.ThirdpartyId);
            string ordername = CustomerDetails.ThirdpartyId + orderid;
            take = ordername;
            billNo = "No";

            string VchKOTID = string.Empty;
            try
            {
                if (CustomerDetails.Discount == null)
                {

                    CustomerDetails.Discount = "0";
                }

                if (CustomerDetails.OrderUpdate != "0")
                {

                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "insert into tabposthirdpartykotchange select * from tabposthirdpartykot where vchorderno='" + ManageQuote(CustomerDetails.OrderUpdate) + "'");
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "insert into tabposthirdpartykotchangedetails select * from tabposthirdpartykotdetails where vchorderno='" + ManageQuote(CustomerDetails.OrderUpdate) + "'");

                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "delete from tabposthirdpartykotdetails where vchorderno='" + ManageQuote(CustomerDetails.OrderUpdate) + "'");
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "delete from tabposthirdpartykot where vchorderno='" + ManageQuote(CustomerDetails.OrderUpdate) + "'");

                    string strInsertOrder = "INSERT INTO tabposthirdpartykot(vchorderno, datorderdate,thirdpartyid,vchpartyid,  numitemtotal,  numnetpayable, vchstatus, statusid, createdby, createddate, numdiscount,vchdiscounttype,sessionid,numdueamount) VALUES ('" + CustomerDetails.OrderUpdate + "','" + FormatDate(CustomerDetails.OrderDate) + "','" + ManageQuote(CustomerDetails.ThirdpartyRecorId) + "','" + ManageQuote(CustomerDetails.ThirdpartyId) + "','" + ManageQuote(CustomerDetails.OrderTotal) + "','" + ManageQuote(CustomerDetails.OrderNetTotal) + "','TPI'," + CustomerDetails.statusid + "," + CustomerDetails.createdby + ",current_timestamp,'" + ManageQuote(CustomerDetails.Discount) + "','" + ManageQuote(CustomerDetails.discounttype) + "'," + session + ",'" + ManageQuote(CustomerDetails.OrderNetTotal) + "') returning recordid;";
                    int OrderidReturn = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strInsertOrder));

                    foreach (var Item in ItemDetails)
                    {
                        string Items = "INSERT INTO tabposthirdpartykotdetails(orderno, vchorderno, itemid, itemname, itemcode, itemqty,numitemrate, statusid, createdby, createddate)    VALUES (" + OrderidReturn + ",'" + CustomerDetails.OrderUpdate + "','" + Item.ItemId + "','" + Item.ItemName + "','" + Item.ItemCode + "'," + Item.Quantity + "," + Item.ActualRate + "," + CustomerDetails.statusid + "," + CustomerDetails.createdby + ",current_timestamp);";
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, Items);
                    }

                }
                else
                {




                    string strInsertOrder = "INSERT INTO tabposthirdpartykot(vchorderno, datorderdate,thirdpartyid,vchpartyid,  numitemtotal,  numnetpayable, vchstatus, statusid, createdby, createddate, numdiscount,vchdiscounttype,sessionid,numdueamount) VALUES ('" + take + "','" + FormatDate(CustomerDetails.OrderDate) + "','" + ManageQuote(CustomerDetails.ThirdpartyRecorId) + "','" + ManageQuote(CustomerDetails.ThirdpartyId) + "','" + ManageQuote(CustomerDetails.OrderTotal) + "','" + ManageQuote(CustomerDetails.OrderNetTotal) + "','TPI'," + CustomerDetails.statusid + "," + CustomerDetails.createdby + ",current_timestamp,'" + ManageQuote(CustomerDetails.Discount) + "','" + ManageQuote(CustomerDetails.discounttype) + "'," + session + ",'" + ManageQuote(CustomerDetails.OrderNetTotal) + "') returning recordid;";
                    int OrderidReturn = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strInsertOrder));

                    foreach (var Item in ItemDetails)
                    {
                        string Items = "INSERT INTO tabposthirdpartykotdetails(orderno, vchorderno, itemid, itemname, itemcode, itemqty,numitemrate, statusid, createdby, createddate)    VALUES (" + OrderidReturn + ",'" + take + "','" + Item.ItemId + "','" + Item.ItemName + "','" + Item.ItemCode + "'," + Item.Quantity + "," + Item.ActualRate + "," + CustomerDetails.statusid + "," + CustomerDetails.createdby + ",current_timestamp);";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Items);
                    }



                    if (Count == 0)
                    {
                        trans.Commit();
                    }

                    else
                    {
                        Count = 2;
                    }
                }
            }

            catch (Exception ex)
            {
                Count = 1;
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "CreateKOT");
                CloseCon(con);
                throw;
            }

            return Count;
        }



        public DataTable GetThirdPartyOders(string ThirdpartyRecorId, string ThirdpartyId)
        {
            DataTable dt = new DataTable();

            try
            {
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select to_char(createddate,'dd-MM-YYYY HH12:MM') as ordertime,vchorderno,numitemtotal,case when vchstatus='TPI' then 'In Preparation' when vchstatus='TPO' then 'Out For Delivery' when vchstatus='TPP' then 'Payment Made' when vchstatus='TPOR' then 'Order Prepared' end as vchstatus,vchstatus as vchstatusCode from  tabposthirdpartykot where thirdpartyid=" + ThirdpartyRecorId + " and vchpartyid='" + ThirdpartyId + "'  and vchstatus not in('TPP','TPC','TPO' ) AND datorderdate::DATE =current_date   order by recordid desc ").Tables[0];
            }
            catch (Exception ex)
            {

            }

            return dt;
        }

        public bool UpdateDliveryStatusInThirdPartyOrders(string OderNo, string Status, string statusid, string createdby)
        {

            int Count = 0;
            bool ToF = false;
            string CustomerID = string.Empty;
            string sttustoUpdate = "";
            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();

            if (Status == "Order Prepared")
            {
                sttustoUpdate = "TPOR";
            }
            if (Status == "Out For Delivery")
            {
                sttustoUpdate = "TPO";
            }

            try
            {

                if (Status == "Order Cancel")
                {

                    string StrInsertCancel = "INSERT INTO tabposthirdpartykotcancel(vchorderno, datorderdate, thirdpartyid, vchpartyid, numitemtotal,numdiscount, vchdeliverychargesstatus, numdeliverycharges, numnetpayable, vchstatus, statusid, createdby, createddate,vchdiscounttype) select vchorderno, datorderdate, thirdpartyid, vchpartyid, numitemtotal, numdiscount, vchdeliverychargesstatus, numdeliverycharges, numnetpayable, vchstatus, statusid, createdby, createddate,   vchdiscounttype from tabposthirdpartykot where vchorderno='" + OderNo + "'";
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, StrInsertCancel);
                    string StrInsertCancelDetails = "INSERT INTO tabposthirdpartykotcanceldetails(orderno, vchorderno, itemid, itemname, itemcode, itemqty, numitemrate, statusid, createdby, createddate) select orderno, vchorderno, itemid, itemname, itemcode, itemqty,  numitemrate, statusid, createdby, createddate from tabposthirdpartykotdetails where vchorderno='" + OderNo + "'";
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, StrInsertCancelDetails);

                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "delete from tabposthirdpartykotdetails where vchorderno='" + OderNo + "'");
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "delete from tabposthirdpartykot where vchorderno='" + OderNo + "'");




                }

                if (Status == "Order Prepared" || Status == "Out For Delivery")
                {
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "update tabposthirdpartykot set vchstatus='" + sttustoUpdate + "' where vchorderno='" + OderNo + "';");
                }
                ToF = true;

                if (Count == 0)
                {
                    trans.Commit();
                }

                else
                {
                    Count = 2;
                }

            }

            catch (Exception ex)
            {
                Count = 1;
                trans.Rollback();
                ToF = false;
                EventLogger.WriteToErrorLog(ex, "CreateKOT");
                CloseCon(con);
                throw;
            }

            return ToF;
        }

        public DataTable GetItemsOfThirdPartySpecicOrder(string orderno)
        {
            DataTable dt = new DataTable();

            try
            {

                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select tphkd.itemname,itemqty ,numitemrate ,itemid,itemcode,numnetpayable,numdiscount,numdeliverycharges,orderno,vchdiscounttype from tabposthirdpartykot tphk join tabposthirdpartykotdetails tphkd on tphk.vchorderno=tphkd.vchorderno  where tphk.vchorderno='" + orderno + "'").Tables[0];
            }
            catch (Exception ex)
            {

            }

            return dt;
        }

        public DataTable GetThirdPartyOdersWeekWiseAndMonthWise(string ThirdpartyRecorId, string ThirdpartyId, string Wise)
        {
            DataTable dt = new DataTable();

            string Query = "";

            try
            {
                if (Wise == "W" || Wise == "M")
                {
                    int date = 0;
                    if (Wise == "W")
                    {
                        date = 7;
                    }
                    else
                    {
                        date = 30;
                    }
                    Query = "select to_char(createddate,'dd-MM-YYYY HH12:MM') as ordertime,vchorderno,numitemtotal from  tabposthirdpartykot where thirdpartyid=" + ThirdpartyRecorId + " and vchpartyid='" + ThirdpartyId + "'  and datorderdate::date between current_date -" + date + " and current_date   order by recordid desc ";
                }

                if (Wise == "T")
                {
                    Query = "select to_char(createddate,'dd-MM-YYYY HH12:MM') as ordertime,vchorderno,numitemtotal from  tabposthirdpartykot where thirdpartyid=" + ThirdpartyRecorId + " and vchpartyid='" + ThirdpartyId + "'  and datorderdate::date = current_date    order by recordid desc ";

                }
                if (Wise == "R")
                {

                    Query = "select to_char(createddate,'dd-MM-YYYY HH12:MM') as ordertime,vchorderno,numitemtotal,case when vchstatus='TPI' then 'In Preparation' when vchstatus='TPO' then 'Out For Delivery' when vchstatus='TPP' then 'Payment Made' when vchstatus='TPOR' then 'Order Prepared' end as vchstatus,vchstatus as vchstatusCode from  tabposthirdpartykot where thirdpartyid=" + ThirdpartyRecorId + " and vchpartyid='" + ThirdpartyId + "'  and vchstatus not in('TPP','TPC','TPO' ) AND datorderdate::DATE =current_date   order by recordid desc ";
                }

                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, Query).Tables[0];
            }
            catch (Exception ex)
            {

            }

            return dt;
        }

        public DataTable GetThirdpartyNames()
        {
            DataTable dt = new DataTable();

            string Query = "";

            try
            {

                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select vchpartyid,vchpartyname from tabposthirdpartymst where statusid=1;").Tables[0];
            }
            catch (Exception ex)
            {

            }

            return dt;
        }

        public DataTable GetThirdpartyBills(string Date, string ThirdpartyId)
        {
            DataTable dt = new DataTable();

            string Query = "";

            try
            {

                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT T1.* FROM  VWPOSTHIRDPARTYBINDORDERSTOTAL T1 WHERE numdueamount>0 and CAST(DATORDERDATE AS DATE) <='" + FormatDate(Date) + "' and  vchpartyid='" + ThirdpartyId + "' order by DATORDERDATE").Tables[0];
            }
            catch (Exception ex)
            {

            }

            return dt;
        }

        public bool saveThirdPartyBillsettlement(Thirpartysettlement ThirdT, List<Thirpartysettlement> ThirdDetailsTD)
        {

            int Count = 0;
            bool ToF = false;
            string CustomerID = string.Empty;
            int paidAmount = 0;
            ThirdT.PaidAmountforUpdate = ThirdT.PaidAfterDiscountAdd;
            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();
            int paidAincal = -1;


            try
            {

                for (int i = 0; i < ThirdDetailsTD.Count; i++)
                {

                    if (paidAincal == -1)
                    {
                        if (Convert.ToInt16(ThirdDetailsTD[i].nooforders) == 1)
                        {

                            var orders = (ThirdDetailsTD[i].vchorderno).Split('-');
                            var orderAmount = Convert.ToDecimal(orders[1]);

                            paidAmount = Convert.ToInt32(ThirdT.PaidAmountforUpdate);
                            var AfterPaid = orderAmount - paidAmount;

                            if (AfterPaid < 0)
                            {
                                ThirdT.PaidAmountforUpdate = Math.Abs(Convert.ToInt32(AfterPaid)).ToString();
                                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update  tabposthirdpartykot SET numdueamount =0 where vchorderno='" + orders[0].Trim() + "'");

                            }
                            else if (AfterPaid >= 0)
                            {

                                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update  tabposthirdpartykot SET numdueamount =" + AfterPaid + " where vchorderno='" + orders[0].Trim() + "'");
                                ThirdT.PaidAmountforUpdate = "0";
                                paidAincal = 0;
                                break;

                            }

                        }
                        else
                        {

                            var orders = (ThirdDetailsTD[i].vchorderno.Split(','));


                            for (int k = 0; k < Convert.ToInt32(orders.Length); k++)
                            {

                                var ordersforsplit = orders[k].Split('-');

                                var ordersforsplitAmount = Convert.ToDecimal(ordersforsplit[1]);
                                paidAmount = Convert.ToInt32(ThirdT.PaidAmountforUpdate);
                                var AfterPaidM = ordersforsplitAmount - paidAmount;


                                if (AfterPaidM < 0)
                                {

                                    ThirdT.PaidAmountforUpdate = Math.Abs(Convert.ToInt32(AfterPaidM)).ToString();
                                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update  tabposthirdpartykot SET numdueamount =0 where vchorderno='" + ordersforsplit[0].Trim() + "'");
                                }
                                else if (AfterPaidM >= 0)
                                {

                                    ThirdT.PaidAmountforUpdate = "0";
                                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update  tabposthirdpartykot SET numdueamount =" + AfterPaidM + " where vchorderno='" + ordersforsplit[0].Trim() + "'");
                                    paidAincal = 0;
                                    break;


                                }

                                ;

                            }
                        }

                    }



                }


                string strInsertOrder = "INSERT INTO tabposthirdpartybillsettlement(vchpartyid, vchpartyname, datfromdate, dattodate, numorderamount,numprevdueamount, numpaidamount, statusid, createdby, createddate,vchmodeofpayment ,vchchequeno ,vchchequedate,vchbankname ,vchtransactionno ,vchtransactiondate,nummarginamount)    VALUES ('" + ThirdT.ThirdPartyNames + "','" + ThirdT.ThirpartyName + "','" + FormatDate(ThirdT.Date) + "','" + FormatDate(ThirdT.Date) + "'," + ThirdT.TotalBillAmount + "," + ThirdT.DueAmount + "," + ThirdT.PaidAmount + "," + ThirdT.statusid + "," + ThirdT.createdby + ",current_timestamp,'" + ThirdT.ModeOfPayment + "','" + ThirdT.ChequeNo + "','" + ThirdT.Chequedate + "','" + ThirdT.BankName + "','" + ThirdT.TransactionNo + "','" + ThirdT.TransactionDate + "'," + ThirdT.MarginAmount + ")returning recordid;";
                int OrderidReturn = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strInsertOrder));

                foreach (var Bills in ThirdDetailsTD)
                {

                    string Items = "INSERT INTO tabposthirdpartybillsettlementdetails(detailsid, vchpartyid, datorderdate, nooforders, numorderamount,numpaidamount, statusid, createdby, createddate) VALUES (" + OrderidReturn + ",'" + Bills.vchpartyid + "','" + Bills.datorderdate + "'," + Bills.nooforders + "," + Bills.numdueamount + "," + 0 + "," + ThirdT.statusid + "," + ThirdT.createdby + ",current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, Items);
                }


                string strReceiptID = "";
                if (ThirdT.ModeOfPayment == "Cheque")
                {
                    strReceiptID = GenerateNextIDForGr("TABGENERALRECEIPT", "receiptid", 1, FormatDate(ThirdT.Date), "datdate", "RCQ");
                    strReceiptID = "RCQ" + strReceiptID;
                }
                else
                {
                    strReceiptID = GenerateNextIDForGr("TABGENERALRECEIPT", "receiptid", 1, FormatDate(ThirdT.Date), "datdate", "R");
                    strReceiptID = "R" + strReceiptID;
                }

                if (ThirdT.Chequedate == null)
                {

                    ThirdT.Chequedate = "01/01/0001";
                }

                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT INSERTACCOUNTHEADS('" + ThirdT.ThirpartyName.ToUpper() + "','acc11042','3'); ");
                using (npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT * FROM TABACCOUNTTREE WHERE VCHACCOUNTNAME='" + ThirdT.ThirpartyName.ToUpper() + "' AND VCHPARENTID='acc11042';"))
                {
                    while (npgdr.Read())
                    {
                        int detailsid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "INSERT INTO tabgeneralreceipt(receiptid, datdate, vchnumber, datcleardate, vchchequestatus,numamount, vchnarration, receiptstatusid, statusid, createdby,createddate,numaccountid,vchaccountid)VALUES ('" + strReceiptID + "','" + FormatDate(ThirdT.Date) + "','" + ThirdT.ChequeNo + "','" + FormatDate(ThirdT.Chequedate) + "',null,'" + ThirdT.PaidAmount + "','BEING THE AMOUNT RECEIVED TOWARDS  " + ThirdT.ThirpartyName + "',1,1," + ThirdT.createdby + ",current_timestamp," + npgdr["recordid"].ToString() + ",'" + npgdr["vchaccountid"].ToString() + "') returning recordid ;"));
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "INSERT INTO tabgeneralreceiptdetails(detailsid, vchreceiptid, numamount,accountid,vchaccountid)VALUES (" + detailsid + ",'" + strReceiptID + "'," + ThirdT.PaidAmount + "," + npgdr["recordid"].ToString() + ",'" + npgdr["vchaccountid"].ToString() + "');");


                        if (ThirdT.ModeOfPayment == "Cheque")
                        {

                            NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "INSERT INTO tabchequedraftreceived( vchreceiptno, vchbankname, vchbranch, chrtype, chequenumber,datdate, numamount, chrshiftstatus, chrclearstatus,vchparticulars, statusid, createdby,depositedbank, generalreceiptid,createddate)    VALUES ('" + strReceiptID + "','" + ManageQuote(ThirdT.BankName) + "',null,'C','" + ManageQuote(ThirdT.ChequeNo) + "','" + FormatDate(ThirdT.Date) + "'," + ThirdT.PaidAmount + ",'N','N',null,1," + ThirdT.createdby + ",null,null,current_timestamp);");

                        }
                    }
                }



                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "select FNTOTALTRANSACTIONS('" + strReceiptID + "','RECEIPT');");

                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "select accountsupdate();");
                if (Count == 0)
                {
                    trans.Commit();
                }

                else
                {
                    Count = 2;
                }

            }

            catch (Exception ex)
            {
                Count = 1;
                trans.Rollback();
                ToF = false;
                EventLogger.WriteToErrorLog(ex, "CreateKOT");
                CloseCon(con);
                throw;
            }

            return ToF;
        }

        public DataTable getBillToCompanyBills(string companyname)
        {
            DataTable dt = new DataTable();

            string Query = "";

            try
            {

                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select vchbillno,datdate||' '||billtime as datdate  ,numnet,(numnet-numpaidamount) as numdueamount,sum(numnet)as total,0 as checkedstatus,-1 as paidatstarting  from tabposbillsettlement where vchbillstatustoknow='BC' and vchcompanyname='" + companyname + "' and numpaidamount<numnet group by  vchbillno,datdate,numnet,numdueamount,billtime order by vchbillno").Tables[0];
            }
            catch (Exception ex)
            {

            }

            return dt;
        }
        public DataTable GetBillToCompanynames()
        {
            DataTable dt = new DataTable();

            string Query = "";

            try
            {

                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select distinct vchcompanyname from  tabposbillsettlement  where vchbillstatustoknow ='BC' and numpaidamount<numnet").Tables[0];
            }
            catch (Exception ex)
            {

            }

            return dt;
        }

        public bool SaveBillTocomanyBills(Thirpartysettlement ThirdT, List<Thirpartysettlement> ThirdDetailsTD)
        {

            int Count = 0;
            bool ToF = false;
            string CustomerID = string.Empty;
            int paidAmount = 0;
            ThirdT.PaidAmountforUpdate = ThirdT.PaidAfterDiscountAdd;
            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();
            int paidAincal = -1;

            if (ThirdT.CompanyName == "1")
            {
                ThirdT.CompanyName = "MICROSOFT";
            }


            try
            {



                for (int i = 0; i < ThirdDetailsTD.Count; i++)
                {
                    if (ThirdDetailsTD[i].paidatstarting != "-1")
                    {

                        string strInsertOrder = "INSERT INTO tabposbilltocompany(companyname, vchbillno, numdueamount, numpaidamount,datdate, statusid, createdby) VALUES ('" + ThirdT.CompanyName + "','" + ThirdDetailsTD[i].vchbillno + "','" + ThirdDetailsTD[i].numdueamount + "','" + ThirdDetailsTD[i].paidatstarting + "',current_timestamp,1,1)";
                        int OrderidReturn = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strInsertOrder));


                        string strInsertOrderUpdate = "update tabposbillsettlement set numpaidamount =numpaidamount+" + ThirdDetailsTD[i].paidatstarting + " where vchbillno='" + ThirdDetailsTD[i].vchbillno + "'";
                        (NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsertOrderUpdate)).ToString();
                    }

                }


                string strReceiptID = "";
                if (ThirdT.ModeOfPayment == "Cheque")
                {
                    strReceiptID = GenerateNextIDForGr("TABGENERALRECEIPT", "receiptid", 1, FormatDate(ThirdT.fromdate), "datdate", "RCQ");
                    strReceiptID = "RCQ" + strReceiptID;
                }
                else
                {
                    strReceiptID = GenerateNextIDForGr("TABGENERALRECEIPT", "receiptid", 1, FormatDate(ThirdT.fromdate), "datdate", "R");
                    strReceiptID = "R" + strReceiptID;
                }

                if (ThirdT.Chequedate == null)
                {

                    ThirdT.Chequedate = "01/01/0001";
                }

                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT INSERTACCOUNTHEADS('" + ThirdT.CompanyName.ToUpper() + "','acc11042','3'); ");
                using (npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT * FROM TABACCOUNTTREE WHERE VCHACCOUNTNAME='" + ThirdT.CompanyName.ToUpper() + "' AND VCHPARENTID='acc11042';"))
                {
                    while (npgdr.Read())
                    {
                        int detailsid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "INSERT INTO tabgeneralreceipt(receiptid, datdate, vchnumber, datcleardate, vchchequestatus,numamount, vchnarration, receiptstatusid, statusid, createdby,createddate,numaccountid,vchaccountid)VALUES ('" + strReceiptID + "','" + FormatDate(ThirdT.fromdate) + "','" + ThirdT.ChequeNo + "','" + FormatDate(ThirdT.Chequedate) + "',null,'" + ThirdT.PaidAmount + "','BEING THE AMOUNT RECEIVED TOWARDS  " + ThirdT.CompanyName.ToUpper() + "',1,1," + ThirdT.createdby + ",current_timestamp," + npgdr["recordid"].ToString() + ",'" + npgdr["vchaccountid"].ToString() + "') returning recordid ;"));
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "INSERT INTO tabgeneralreceiptdetails(detailsid, vchreceiptid, numamount,accountid,vchaccountid)VALUES (" + detailsid + ",'" + strReceiptID + "'," + ThirdT.PaidAmount + "," + npgdr["recordid"].ToString() + ",'" + npgdr["vchaccountid"].ToString() + "');");


                        if (ThirdT.ModeOfPayment == "Cheque")
                        {

                            NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "INSERT INTO tabchequedraftreceived( vchreceiptno, vchbankname, vchbranch, chrtype, chequenumber,datdate, numamount, chrshiftstatus, chrclearstatus,vchparticulars, statusid, createdby,depositedbank, generalreceiptid,createddate)    VALUES ('" + strReceiptID + "','" + ManageQuote(ThirdT.BankName) + "',null,'C','" + ManageQuote(ThirdT.ChequeNo) + "','" + FormatDate(ThirdT.fromdate) + "'," + ThirdT.PaidAmount + ",'N','N',null,1," + ThirdT.createdby + ",null,null,current_timestamp);");

                        }
                    }
                }


                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "select FNTOTALTRANSACTIONS('" + strReceiptID + "','RECEIPT');");

                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "select accountsupdate();");

                if (Count == 0)
                {
                    trans.Commit();
                }

                else
                {
                    Count = 2;
                }

            }

            catch (Exception ex)
            {
                Count = 1;
                trans.Rollback();
                ToF = false;
                EventLogger.WriteToErrorLog(ex, "CreateKOT");
                CloseCon(con);
                throw;
            }

            return ToF;
        }
        #endregion

    }
}
