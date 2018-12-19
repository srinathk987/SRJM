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
    public class POSTransactionRepository : IPOSTransaction
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans;
        NpgsqlDataReader npgdr = null;
        string constr = NPGSqlHelper.SQLConnString;
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



        #region  KOT CHANGE...
        public List<KOTTakingDTO> ShowKotChangeDetails(string TableId)
        {
            List<KOTTakingDTO> lstKOT = new List<KOTTakingDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();


                    string strInsert = "select  trkd.kotid,trkd.vchkotid,trkd.itemid,trkd.itemname,trkd.itemcode,trkd.itemqty,trkd.itemrate,trkd.itemsdivideinto,trk.tableid,trkd.remarks,trkd.vchitemstatus from tabrmskotdetails trkd join tabrmskot trk on trk.vchkotid=trkd.vchkotid where tableid=" + TableId + " and  vchitemstatus='N' and trkd.vchkotid not in (select vchkotid from tabposbillgenerationdetails) order by kotid";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            KOTTakingDTO objKot = new KOTTakingDTO();
                            objKot.TableId = Convert.ToInt32(npgdr["tableid"]);
                            objKot.ItemId = Convert.ToInt32(npgdr["ITEMid"]);
                            objKot.ItemCode = Convert.ToString(npgdr["ITEMCODE"]);
                            objKot.ItemName = npgdr["ITEMNAME"].ToString();
                            objKot.Quantity = Convert.ToInt32(npgdr["itemqty"]);
                            objKot.Remarks = npgdr["REMARKS"].ToString();
                            objKot.KotName = npgdr["VCHKOTID"].ToString();
                            objKot.KotId = Convert.ToInt32(npgdr["kotid"]);
                            objKot.Rate = Convert.ToInt32(npgdr["itemrate"]);
                            objKot.QuantityHidden = (npgdr["itemsdivideinto"]).ToString(); ;
                            objKot.Status = (npgdr["vchitemstatus"]).ToString(); ;
                            lstKOT.Add(objKot);
                        }
                    }
                    con.Close();
                    con.ClearPool();
                    con.Dispose();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowKotChangeDetails");
                throw ex;
            }
            return lstKOT;

        }


        public bool SaveKOTChangeDetails(List<KOTTakingDTO> lstkotChange, string Delete, int Create, int Statusid)
        {
            bool IsValid = false;
            int Count = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                string strRmskotchangedetails = "delete  from tabrmskotchangedetails  where vchkotid in(" + Delete + ")";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strRmskotchangedetails);

                string strRmskotchange = "delete  from tabrmskotchange  where vchkotid in(" + Delete + ")";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strRmskotchange);


                string strInsertinkotchange = "insert into tabrmskotchange  select * from tabrmskot where vchkotid in(" + Delete + ");";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsertinkotchange);

                string strInsertinkotchangeDetails = "insert into  tabrmskotchangedetails(kotid, vchkotid, itemid, itemname, itemcode, itemqty,remarks, statusid, createdby, createddate,itemrate,itemsdivideinto) select kotid, vchkotid, itemid, itemname, itemcode, itemqty,remarks, statusid," + Create + " ,current_timestamp,itemrate,itemsdivideinto from tabrmskotdetails where vchkotid in(" + Delete + ");";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsertinkotchangeDetails);

                string strRmskotdetailsDelete = "delete  from tabrmskotdetails  where vchkotid in(" + Delete + ")";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strRmskotdetailsDelete);

                foreach (var Item in lstkotChange)
                {
                    if (Item.KotName != null)
                    {
                        string Items = "INSERT INTO tabrmskotdetails(kotid, vchkotid, itemid, itemname, itemcode, itemqty,remarks, statusid, createdby, createddate,itemrate,itemsdivideinto)  VALUES (" + Item.KotId + ",'" + ManageQuote(Item.KotName) + "'," + Item.ItemId + ",'" + ManageQuote(Item.ItemName) + "','" + ManageQuote(Item.ItemCode) + "'," + Item.Quantity + ",'" + ManageQuote(Item.Remarks) + "'," + Statusid + "," + Create + ",current_timestamp," + Item.Rate + ",'" + Item.QuantityHidden + "' );";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Items);
                    }
                }

                trans.Commit();
                IsValid = true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "SaveKOTChangeDetails");
                IsValid = false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                    trans.Dispose();
                }
            }

            return IsValid;
        }


        #endregion

        #region KOT TAKING...

        //
        public string ShowSessionForKOTtaking()
        {

            string Session = string.Empty;
            string SessionDetails = string.Empty;
            try
            {
                string Query_Session = "SELECT SESSIONNAME||'@'||sessionid FROM TABPOSSESSIONMST WHERE (SPLIT_PART(FROMTIME, ':',1)::INT*60)+SPLIT_PART(FROMTIME, ':',2)::INT <= ((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) AND (SPLIT_PART(TOTIME, ':',1)::INT*60)+SPLIT_PART(TOTIME, ':',2)::INT >=((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT)";
                Session = Convert.ToString(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, Query_Session));

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowSessionForKOTtaking");
                throw;

            }

            return Session;
        }

        /// <summary>
        /// Items List Based On Session
        /// </summary>
        /// <param name="Sessionid"></param>
        /// <returns></returns>
        public List<ItemsDTO> ShowItemsForSession()
        {
            List<ItemsDTO> lstItems = null;
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    lstItems = new List<ItemsDTO>();
                    string strInsert = "SELECT ROW_NUMBER() OVER() as rowindex,ITEMID,TPS.ITEMNAME,TPS.ITEMCODE,tpm.numrate,tpm.numrate as ratefixed,tpc.itemcategoryname,tps.subcategoryname,tpc.categoryid,TPM.departmentname,0 AS ITEMCOUNT,'C' as ordervalue FROM TABPOSSESSIONDETAILSMST TPS JOIN TABPOSITEMMST TPM ON TPS.ITEMCODE=TPM.ITEMCODE join tabpositemcategorymst tpc on TPM.categoryid=TPc.categoryid where sessionid in(SELECT sessionid FROM  TABPOSSESSIONMST WHERE statusid=1 and (SPLIT_PART(FROMTIME, ':',1)::INT*60)+SPLIT_PART(FROMTIME, ':',2)::INT <= ((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) AND (SPLIT_PART(TOTIME, ':',1)::INT*60)+SPLIT_PART(TOTIME, ':',2)::INT >=((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) order by sessionid limit 1) union SELECT ROW_NUMBER() OVER() AS ROWINDEX,ITEMID,T1.ITEMNAME,T1.ITEMCODE,T2.NUMRATE,T2.NUMRATE,'DAY SPECIAL' as itemcategoryname,T2.SUBCATEGORYNAME as SUBCATEGORYNAME,0,T2.DEPARTMENTNAME,0 AS ITEMCOUNT,'B' as ordervalue FROM TABPOSDAYSPECIALITEMS T1 JOIN TABPOSITEMMST T2 ON T1.ITEMCODE=T2.ITEMCODE WHERE T1.DATDATE=CURRENT_DATE UNION SELECT ROW_NUMBER() OVER(partition by T2.SUBCATEGORYNAME) AS ROWINDEX,T1.ITEMID,T1.ITEMNAME,T1.ITEMCODE,T2.NUMRATE,T2.NUMRATE,'FREQUENT' as itemcategoryname,T2.SUBCATEGORYNAME,0,T2.DEPARTMENTNAME,COUNT(*) AS ITEMCOUNT,'A' as ordervalue FROM TABRMSKOTDETAILS T1 JOIN TABPOSITEMMST T2 ON T1.ITEMCODE=T2.ITEMCODE GROUP BY T1.ITEMID,T1.ITEMNAME,T1.ITEMCODE,T2.NUMRATE,T2.NUMRATE,SUBCATEGORYNAME,DEPARTMENTNAME order by ITEMCOUNT desc,ordervalue ;";

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


        /// <summary>
        /// Sesssion Details
        /// </summary>
        /// <param name="Tablename"></param>
        /// <returns></returns>
        public List<KOTTakingDTO> ShowSessionDetailsForSession(string Tablename, int Userid)
        {

            var pos = Tablename.LastIndexOf('?');
            if (pos >= 0)
            {
                Tablename = Tablename.Substring(0, pos);
            }
            List<KOTTakingDTO> lstItems = null;
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    lstItems = new List<KOTTakingDTO>();
                    //string Query_SessionDetails = "SELECT TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI')AS TIME,TABLESNAME,TABLECODE,TABLEID,NUMCOVERS,SECTIONID,SECTIONNAME,SECTIONCODE,to_char(current_date,'DD-MON-yyyy') as date,(SELECT SESSIONNAME FROM TABPOSSESSIONMST WHERE (SPLIT_PART(FROMTIME, ':',1)::INT*60)+SPLIT_PART(FROMTIME, ':',2)::INT <= ((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) AND (SPLIT_PART(TOTIME, ':',1)::INT*60)+SPLIT_PART(TOTIME, ':',2)::INT >=((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT))as SESSIONNAME,(SELECT SESSIONid FROM TABPOSSESSIONMST WHERE (SPLIT_PART(FROMTIME, ':',1)::INT*60)+SPLIT_PART(FROMTIME, ':',2)::INT <= ((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) AND (SPLIT_PART(TOTIME, ':',1)::INT*60)+SPLIT_PART(TOTIME, ':',2)::INT >=((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT)) as SESSIONid FROM TABPOSTABLESANDCOVERSMST  where TABLESNAME='" + Tablename + "' ;";
                    //  string Query_SessionDetails = "SELECT TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI')AS TIME,TABLESNAME,TABLECODE,TABLEID,NUMCOVERS,SECTIONID,SECTIONNAME,SECTIONCODE,to_char(current_date,'DD-MON-yyyy') as date,(SELECT SESSIONNAME FROM TABPOSSESSIONMST WHERE (SPLIT_PART(FROMTIME, ':',1)::INT*60)+SPLIT_PART(FROMTIME, ':',2)::INT <= ((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) AND (SPLIT_PART(TOTIME, ':',1)::INT*60)+SPLIT_PART(TOTIME, ':',2)::INT >=((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) limit 1 )as SESSIONNAME,(SELECT SESSIONid FROM TABPOSSESSIONMST WHERE (SPLIT_PART(FROMTIME, ':',1)::INT*60)+SPLIT_PART(FROMTIME, ':',2)::INT <= ((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) AND (SPLIT_PART(TOTIME, ':',1)::INT*60)+SPLIT_PART(TOTIME, ':',2)::INT >=((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) limit 1 ) as SESSIONid FROM TABPOSTABLESANDCOVERSMST  where TABLESNAME='" + Tablename + "' ;";
                    string Query_SessionDetails = "SELECT TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI')AS TIME,TABLESNAME,(select username from tabuserinfo where userid=" + Userid + ")as Hostname,TABLECODE,tb.TABLEID,NUMCOVERS,SECTIONID,SECTIONNAME,SECTIONCODE,to_char(current_date,'DD-MON-yyyy') as date,(SELECT SESSIONNAME FROM TABPOSSESSIONMST WHERE  statusid=1 and (SPLIT_PART(FROMTIME, ':',1)::INT*60)+SPLIT_PART(FROMTIME, ':',2)::INT <= ((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) AND (SPLIT_PART(TOTIME, ':',1)::INT*60)+SPLIT_PART(TOTIME, ':',2)::INT >=((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) limit 1 )as SESSIONNAME,(SELECT SESSIONid FROM TABPOSSESSIONMST WHERE statusid=1 and (SPLIT_PART(FROMTIME, ':',1)::INT*60)+SPLIT_PART(FROMTIME, ':',2)::INT <= ((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) AND (SPLIT_PART(TOTIME, ':',1)::INT*60)+SPLIT_PART(TOTIME, ':',2)::INT >=((SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',1)::INT*60)+SPLIT_PART(TO_CHAR(CURRENT_TIMESTAMP, 'HH24:MI'), ':',2)::INT) limit 1 ) as SESSIONid,case when status='A' then 0 else coalesce (numadults,0) end as numadults ,case when status='A' then 0 else coalesce (numkids,0) end as numkids FROM TABPOSTABLESANDCOVERSMST tb left  join ( select tableid,  numadults,numkids from tabrmskot kot join (select  vchkotid,createddate from tabrmskotdetails where vchitemstatus in('N','Y')  union all select vchkotid,createddate from tabrmskotchangedetails  where vchitemstatus in('N','Y') ) kotd on kot.vchkotid=kotd.vchkotid where tableid=" + Tablename + " order by kotd.createddate desc limit 1) kot on kot.tableid=tb.tableid where tb.tableid=" + Tablename + ";";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(Query_SessionDetails, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            KOTTakingDTO objlstItems = new KOTTakingDTO();
                            objlstItems.TableName = Convert.ToString(npgdr["TABLESNAME"]);
                            objlstItems.TableCode = Convert.ToString(npgdr["TABLECODE"]);
                            objlstItems.TableNo = Convert.ToInt32(npgdr["TABLEID"]);
                            objlstItems.Covers = Convert.ToInt32(npgdr["NUMCOVERS"]);
                            objlstItems.Adults = Convert.ToInt32(npgdr["numadults"]);
                            objlstItems.Kids = Convert.ToInt32(npgdr["numkids"]);
                            objlstItems.Sectionid = Convert.ToInt32(npgdr["SECTIONID"]);
                            objlstItems.SectionName = Convert.ToString(npgdr["SECTIONNAME"]);
                            objlstItems.SectionCode = Convert.ToString(npgdr["SECTIONCODE"]);
                            objlstItems.Date = Convert.ToString(npgdr["date"]);
                            objlstItems.SessionName = Convert.ToString(npgdr["SESSIONNAME"]);
                            objlstItems.SessionId = Convert.ToString(npgdr["SESSIONid"]);
                            objlstItems.Time = Convert.ToString(npgdr["TIME"]);
                            objlstItems.Host = Convert.ToString(npgdr["Hostname"]);
                            lstItems.Add(objlstItems);
                        }
                    }
                    con.Close();
                    con.ClearPool();
                    con.Dispose();
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

        /// <summary>
        /// Showing Available Tables for Merging
        /// </summary>
        /// <param name="SectionID"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        /// 
        public List<MergingTables> ShowAvailabletables(string SectionID, string TableId)
        {
            var pos = TableId.LastIndexOf('?');
            if (pos >= 0)
            {
                TableId = TableId.Substring(0, pos);
            }
            List<MergingTables> lstKOTTakingDTO = new List<MergingTables>();
            string strData = string.Empty;
            try
            {

                string strSectionid = "SELECT SECTIONID from TABPOSTABLESANDCOVERSMST WHERE tableid=" + TableId + ";";
                int SectionId = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strSectionid));
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    // strData = "select tableid||'-'||'KOT' as tableid  ,tablesname from tabpostablesandcoversmst where status='A' and tableid<>" + TableId + " and sectionid=" + SectionId + " order by tableid";
                    strData = "select tableid||'-'||'KOT' as tableid  ,tablesname from tabpostablesandcoversmst where status='A' and tableid<>" + TableId + " and statusid<>2 order by createddate";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strData, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            MergingTables objKOTTakingDTO = new MergingTables();
                            objKOTTakingDTO.label = Convert.ToString(npgdr["tablesname"]);
                            objKOTTakingDTO.id = Convert.ToString(npgdr["tableid"]);

                            lstKOTTakingDTO.Add(objKOTTakingDTO);
                        }
                    }
                    con.Close();
                    con.ClearPool();
                    con.Dispose();
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
        //public List<KotAvailabletables> ShowAvailabletables(string SectionID, string TableName)
        //{
        //    List<KotAvailabletables> lstKOTTakingDTO = new List<KotAvailabletables>();
        //    try
        //    {
        //        // string strInsert = "SELECT TABLESNAME,TABLEID  FROM  TABPOSTABLESANDCOVERSMST WHERE sectioncode='" + SectionID + "' AND STATUS in('A','M') AND STATUSID=1 and TABLESNAME not in('" + TableName + "');";
        //        string strInsert = "SELECT TABLESNAME,TABLEID  FROM  TABPOSTABLESANDCOVERSMST WHERE  STATUS in('A','M','') AND STATUSID=1 and TABLEID not in(" + TableName + ");";
        //        npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
        //        while (npgdr.Read())
        //        {
        //            KotAvailabletables objKOTTakingDTO = new KotAvailabletables();
        //            objKOTTakingDTO.text = Convert.ToString(npgdr["TABLESNAME"]);
        //            objKOTTakingDTO.value = Convert.ToString(npgdr["TABLEID"]);

        //            lstKOTTakingDTO.Add(objKOTTakingDTO);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "ShowAvailabletables");
        //        throw ex;

        //    }
        //    finally
        //    {
        //        npgdr.Dispose();
        //    }
        //    return lstKOTTakingDTO;
        //}

        public List<KotAvailabletables> ShowMergetables(string TableName)
        {
            var pos = TableName.LastIndexOf('?');
            if (pos >= 0)
            {
                TableName = TableName.Substring(0, pos);
            }
            List<KotAvailabletables> lstMergetables = new List<KotAvailabletables>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    string strInsert = "select distinct mergetableid,mergetableidname,tm.tableid from tabposkotmergedtables tm join tabposkot tk on tm.tableid=tk.tableid where tk.status='N' and tm.tableid=" + TableName + " ;";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            KotAvailabletables objMergetables = new KotAvailabletables();
                            objMergetables.text = Convert.ToString(npgdr["mergetableidname"]);
                            objMergetables.value = Convert.ToString(npgdr["mergetableid"]);
                            lstMergetables.Add(objMergetables);
                        }
                    }
                    con.Close();
                    con.ClearPool();
                    con.Dispose();
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
            return lstMergetables;
        }

        public int CreateKOT(KOTTakingDTO KOTTakingDTO, List<ItemsDTO> ItemDetails, List<Mergetables> Mergetables)
        {

            int Count = 0;
            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();

            string VchKOTID = string.Empty;
            try
            {
                string ExistUser = "select  distinct td.createdby from tabrmskotdetails td join tabrmskot tk on td.vchkotid=tk.vchkotid  where vchitemstatus='N' and tableid=" + KOTTakingDTO.TableNo + " ;";
                int UserExist = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, ExistUser));
                string tablestatus = Convert.ToString(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, " select status from tabpostablesandcoversmst where tableid=" + KOTTakingDTO.TableNo + ""));

                if ((UserExist == 0 || UserExist == KOTTakingDTO.createdby) && tablestatus != "M")
                {
                    string Maxkot = "select substring(vchkotid,4)::int +1 from tabrmskot order by kotid desc limit 1  ;";
                    int vchKOTID = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, Maxkot));

                    KOTTakingDTO.vchKotId = "KOT" + vchKOTID;
                    string strInsert = "INSERT INTO tabrmskot(VCHKOTID, SESSIONID, SECTIONID, DATKOTDATE, KOTTIME, VCHHOSTNAME,TABLEID, NUMCOVERS, NUMADULTS, NUMKIDS, MERGETABLES, STATUS,STATUSID, CREATEDBY, CREATEDDATE)VALUES ('" + ManageQuote(KOTTakingDTO.vchKotId) + "'," + KOTTakingDTO.SessionId + "," + KOTTakingDTO.Sectionid + ",current_date,'" + ManageQuote(KOTTakingDTO.Time) + "','" + ManageQuote(KOTTakingDTO.Host) + "'," + KOTTakingDTO.TableNo + "," + KOTTakingDTO.Covers + "," + KOTTakingDTO.Adults + "," + KOTTakingDTO.Kids + ",'','N'," + KOTTakingDTO.statusid + "," + KOTTakingDTO.createdby + ",current_timestamp) returning kotid;";
                    int KOTId = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert));
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "UPDATE TABPOSTABLESANDCOVERSMST SET STATUS='R' WHERE TABLEsname='" + ManageQuote(KOTTakingDTO.TableName) + "'");
                    foreach (var Item in ItemDetails)
                    {
                        string Items = "INSERT INTO tabrmskotdetails(kotid, vchkotid, itemid, itemname, itemcode, itemqty,remarks, statusid, createdby, createddate,itemrate,itemsdivideinto)  VALUES (" + KOTId + ",'" + ManageQuote(KOTTakingDTO.vchKotId) + "'," + Item.ItemId + ",'" + ManageQuote(Item.ItemName) + "','" + ManageQuote(Item.ItemCode) + "'," + Item.Quantity + ",'" + ManageQuote(Item.Remarks) + "'," + KOTTakingDTO.statusid + "," + KOTTakingDTO.createdby + ",current_timestamp," + Item.ActualRate + ",'" + Item.QuantityHidden + "' );";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Items);
                    }

                    if (Mergetables != null)
                    {
                        foreach (var Item in Mergetables)
                        {
                            int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from TABPOSTABLESANDCOVERSMST where status='A' and tableid=" + Item.id.Split('-')[0] + ""));
                            // string tablestatus = Convert.ToString(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, " select status from tabpostablesandcoversmst where tableid=" + KOTTakingDTO.TableNo + ""));
                            if (count == 1)
                            {
                                string Merge = "INSERT INTO tabposmergetable(fromtableid,totableid,kotid, statusid, createdby, createddate,tablestatus)VALUES ('" + KOTTakingDTO.TableNo + "'," + Item.id.Split('-')[0] + ",'" + null + "'," + KOTTakingDTO.statusid + "," + KOTTakingDTO.createdby + ",current_timestamp,'MG');";

                                // KOTTakingDTO.TableNo     INSERT INTO tabposmergetable(fromtableid, totableid, kotid, statusid, createdby, createddate,tablestatus)

                                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Merge);
                                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "UPDATE TABPOSTABLESANDCOVERSMST SET STATUS='M',modifiedby=" + KOTTakingDTO.createdby + ",modifieddate=current_timestamp WHERE TABLEID=" + Item.id.Split('-')[0] + "");
                            }
                            else
                            {
                                Count = 3;
                            }

                        }
                    }
                    if (Count == 0)
                    {
                        trans.Commit();
                        con.Close();
                        con.ClearPool();
                        con.Dispose();
                    }
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
                EventLogger.WriteToErrorLog(ex, "CreateKOT");

                throw;
            }

            return Count;
        }

        public List<ItemsDTO> ShowPendingKOT(string Tableid)
        {
            List<ItemsDTO> lstItems = null;
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    lstItems = new List<ItemsDTO>();
                    //string strInsert = "SELECT TK.TABLEID,TK.VCHKOTID,TT.ITEMNAME,ITEMQTY,TT.ITEMID,TMST.NUMRATE FROM TABPOSKOT TK RIGHT JOIN TABPOSKOTDETAILS TT ON TK.VCHKOTID=TT.VCHKOTID LEFT JOIN TABPOSITEMMST TMST ON TT.ITEMID=TMST.ITEMID WHERE TK.STATUS='N' and TK.TABLEID=" + Tableid + ";";
                    string strInsert = "SELECT trk.TABLEID,tt.VCHKOTID,TT.ITEMNAME,ITEMQTY,TT.ITEMID,ROUND(ITEMQTY*TMST.NUMRATE)AS NUMRATE FROM tabrmskotdetails  TT join  tabrmskot trk on trk.VCHKOTID=TT.VCHKOTID  LEFT JOIN TABPOSITEMMST TMST ON TT.ITEMID=TMST.ITEMID WHERE  TABLEID IN( SELECT DISTINCT TOTABLEID::INT FROM TABPOSMERGETABLE TM JOIN TABPOSTABLESANDCOVERSMST TC ON TM.TOTABLEID::INT=TC.TABLEID  WHERE FROMTABLEID=" + Tableid + " AND TC.STATUS='M' )  OR TABLEID=" + Tableid + " and TT.VCHKOTID||'-'||TT.ITEMID not in(select vchkotid||'-'||itemid from tabrmskotdetails where vchitemstatus='Y');";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ItemsDTO objlstItems = new ItemsDTO();
                            objlstItems.TableNo = Convert.ToInt32(npgdr["TABLEID"]);
                            objlstItems.KotName = Convert.ToString(npgdr["VCHKOTID"]);
                            objlstItems.ItemId = Convert.ToInt32(npgdr["ITEMID"]);
                            objlstItems.ItemName = Convert.ToString(npgdr["ITEMNAME"]);
                            objlstItems.Quantity = Convert.ToInt32(npgdr["ITEMQTY"]);
                            objlstItems.Rate = Convert.ToDecimal(npgdr["NUMRATE"]);

                            lstItems.Add(objlstItems);
                        }
                    }
                    con.Close();
                    con.ClearPool();
                    con.Dispose();
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

        public DataSet Kotslip(string kotno)
        {
            DataSet ds = new DataSet();

            try
            {
                string strquery = "SELECT VK.SESSIONID,VK.VCHKOTID,VK.ITEMNAME,VK.VCHHOSTNAME,VK.ITEMQTY,TS.SESSIONNAME,TT.TABLESNAME FROM VWKOTDETAILS VK LEFT JOIN TABPOSSESSIONMST TS ON VK.SESSIONID=TS.SESSIONID LEFT JOIN TABPOSTABLESANDCOVERSMST TT ON VK.TABLEID=TT.TABLEID WHERE VK.VCHKOTID='" + kotno + "'";

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

        #endregion

        #region KOT CANCEL...
        public List<KOTCancel> ShowKot(string TableId)
        {
            List<KOTCancel> lstKOTCancel = new List<KOTCancel>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();


                    string strSelect = "select distinct kotid ,vchkotid from vwrmskotdetails where tableid=" + TableId + " and  vchitemstatus ='N' and vchkotid not in (select  vchkotid from tabposbillgenerationdetails ) order by vchkotid";


                    using (NpgsqlCommand cmd = new NpgsqlCommand(strSelect, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            KOTCancel objKOTCancel = new KOTCancel();
                            objKOTCancel.KotId = Convert.ToInt32(npgdr["kotid"].ToString());
                            objKOTCancel.Kot = npgdr["vchkotid"].ToString();
                            lstKOTCancel.Add(objKOTCancel);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowKot");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstKOTCancel;
        }
        public List<KOTCancel> ShowReason()
        {
            List<KOTCancel> lstKOTCancel = new List<KOTCancel>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    string strInsert = "select reasonname,reasoncode from tabposreasonmst where statusid=1;";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            KOTCancel objKOTCancel = new KOTCancel();
                            objKOTCancel.Reason = npgdr["reasonname"].ToString();
                            objKOTCancel.ReasonID = npgdr["reasoncode"].ToString();
                            lstKOTCancel.Add(objKOTCancel);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowReason");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstKOTCancel;
        }
        public List<KOTCancel> ShowKotCancelGridData(string TableNO, string KotId)
        {
            List<KOTCancel> lstKOTCancel = new List<KOTCancel>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();



                    string strSelect = "select * from vwrmskotdetails where  kotid='" + KotId + "' and vchitemstatus ='N'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(strSelect, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            KOTCancel objKOTCancel = new KOTCancel();





                            objKOTCancel.Sectionid = Convert.ToInt32(npgdr["sectionid"].ToString());

                            objKOTCancel.SessionId = Convert.ToInt32(npgdr["sessionid"].ToString());

                            objKOTCancel.Date = npgdr["datkotdate"].ToString();

                            objKOTCancel.Time = npgdr["kottime"].ToString();
                            // Convert.ToString(npgdr["kottime"]);
                            objKOTCancel.Host = npgdr["vchhostname"].ToString();
                            objKOTCancel.Covers = Convert.ToInt32(npgdr["numcovers"].ToString());
                            objKOTCancel.Adults = Convert.ToInt32(npgdr["numadults"].ToString());
                            objKOTCancel.Rate = Convert.ToDouble(npgdr["itemrate"]);
                            objKOTCancel.Kids = Convert.ToInt32(npgdr["numkids"].ToString());
                            objKOTCancel.Status = npgdr["status"].ToString();
                            objKOTCancel.Item = npgdr["itemname"].ToString();
                            objKOTCancel.Itemid = npgdr["itemid"].ToString();
                            objKOTCancel.ItemCode = npgdr["itemcode"].ToString();
                            objKOTCancel.ItemQty = npgdr["itemqty"].ToString();
                            objKOTCancel.Reason = npgdr["remarks"].ToString();
                            lstKOTCancel.Add(objKOTCancel);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowKotCancelGridData");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstKOTCancel;
        }
        public bool SaveKotCancel(List<KOTCancel> KTC)
        {
            bool IsValid = false;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();





                string strinsertrmscancel = string.Empty;

                string strinsertrmscanceldetails = string.Empty;

                string strdeletekotDetails = string.Empty;

                string strdeletekot = string.Empty;

                strinsertrmscancel = "insert into tabrmskotcancel(kotid,vchkotid,sessionid,sectionid,datkotdate,kottime,vchhostname,tableid,numcovers,numadults,numkids,status,statusid,createdby,createddate) values(" + KTC[0].KotId + ",'" + KTC[0].KotName + "'," + KTC[1].SessionId + "," + KTC[1].Sectionid + ",current_date,'" + KTC[1].Time + "','" + KTC[1].Host + "'," + KTC[0].TableNo + "," + KTC[1].Covers + "," + KTC[1].Adults + ",'" + KTC[1].Kids + "','" + KTC[1].Status + "'," + KTC[0].statusid + "," + KTC[0].createdby + ",current_timestamp)";


                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strinsertrmscancel);




                for (int i = 1; i < KTC.Count; i++)
                {
                    strinsertrmscanceldetails = "insert into tabrmskotcanceldetails(kotid,vchkotid,itemid,itemname,itemcode,itemqty,itemrate,vchreason,statusid,createdby,createddate) values(" + KTC[0].KotId + ",'" + KTC[0].KotName + "'," + KTC[i].Itemid + ",'" + KTC[i].Item + "','" + KTC[i].ItemCode + "','" + KTC[i].ItemQty + "'," + Convert.ToDouble(KTC[i].Rate) + ",'" + KTC[0].ReasonID + "'," + KTC[0].statusid + "," + KTC[0].createdby + ",current_timestamp)";

                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strinsertrmscanceldetails);
                }



                strdeletekotDetails = "delete from tabrmskotdetails where vchkotid='" + KTC[0].KotName + "'";

                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strdeletekotDetails);

                //strdeletekot = "delete from tabrmskot where vchkotid='" + KTC[0].KotName + "'";

                //NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strdeletekot);

                trans.Commit();

                string countKots = "select count(vchkotid) from vwrmskotdetails where tableid=" + KTC[0].TableNo + " and vchitemstatus='N'";

                int iscount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, countKots));

                if (iscount == 0)
                {
                    int countOfBillsIntable = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select count(vchbillno) from tabposbillgeneration where tableid=" + KTC[0].TableNo + ""));
                    if (countOfBillsIntable != 0)
                    {
                        string strcounttableid = "select count(vchbillno) from tabposbillgeneration  where vchbillno not in(select vchbillno from tabposbillsettlement where tableid=" + KTC[0].TableNo + ") and tableid=" + KTC[0].TableNo + "";
                        int iscounttableid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, strcounttableid));

                        if (iscounttableid > 0)
                        {
                            string strcovertmst = "update tabpostablesandcoversmst set status ='B',modifieddate=current_timestamp  where  tableid in(" + KTC[0].TableNo + " ) ;";
                            NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, strcovertmst);
                        }
                        else
                        {
                            string strcovertmst = "update tabpostablesandcoversmst set status ='A',modifieddate=current_timestamp  where  tableid in(select tableid from tabpostablesandcoversmst where tableid=" + KTC[0].TableNo + " union select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + KTC[0].TableNo + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + KTC[0].TableNo + " union select totableid::int from tabposmergetable where fromtableid in (select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + KTC[0].TableNo + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + KTC[0].TableNo + ") and totableid::int in(select tableid from tabpostablesandcoversmst where status='M') union  select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid in( select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + KTC[0].TableNo + ") order by tableid) ;";
                            NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, strcovertmst);

                        }

                    }
                    else
                    {
                        string strcovertmst = "update tabpostablesandcoversmst set status ='A',modifieddate=current_timestamp  where  tableid in(select tableid from tabpostablesandcoversmst where tableid=" + KTC[0].TableNo + " union select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + KTC[0].TableNo + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + KTC[0].TableNo + " union select totableid::int from tabposmergetable where fromtableid in (select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + KTC[0].TableNo + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + KTC[0].TableNo + ") and totableid::int in(select tableid from tabpostablesandcoversmst where status='M') union  select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid in( select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + KTC[0].TableNo + ") order by tableid) ;";
                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, strcovertmst);
                    }
                }

                IsValid = true;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveKotCancel");
                trans.Rollback();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                    trans.Dispose();
                }

            }
            return IsValid;
        }

























        ////--------------Main Table-------------//
        ////SELECT recordid, tableid, kotid, vchkotid, reason, statusid, createdby, createddate, modifiedby, modifieddate FROM tabposkotcancel;

        ////--------------Details Table-------------//
        ////SELECT recorid, detailsid, itemid, itemname, itemcode, itemqty, remarks, statusid, createdby, createddate, modifiedby, modifieddate FROM tabposkotcanceldetails;
        //strinsert = "insert into tabposkotcancel(tableid, kotid, reason, statusid,vchkotid, createdby, createddate)  values ('" + KTC[0].TableNo + "','" + KTC[0].KotId + "','" + KTC[0].ReasonID + "','" + KTC[0].statusid + "','" + KTC[0].KotName + "'," + KTC[0].createdby + ",current_timestamp) returning recordid;";

        //int Recordid = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strinsert));

        //strInsert = "insert into tabposkotcanceldetails(detailsid, itemid, itemname, itemcode, itemqty, remarks, statusid, createdby, createddate) values ";
        //foreach (var K in KTC)
        //{
        //    if (count != 0)
        //    {
        //        strInsert += "(" + Recordid + "," + K.Itemid + ",'" + K.Item + "','" + K.ItemCode + "'," + K.ItemQty + ",'" + KTC[0].ReasonID + "'," + KTC[0].statusid + "," + KTC[0].createdby + ",current_timestamp),";
        //    }
        //    count++;
        //}
        //strInsert = strInsert.Substring(0, strInsert.Length - 1);
        //NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);

        //// string KotUpdate = "  SELECT (SELECT COUNT(DISTINCT TK.VCHKOTID)  FROM TABPOSKOTDETAILS TK JOIN TABPOSKOT TP ON TK.VCHKOTID=TP.VCHKOTID   WHERE VCHITEMSTATUS='N' AND TABLEID=" + KTC[0].TableNo + ")||'-'||(SELECT COUNT(VCHKOTID) FROM TABPOSKOTCANCEL WHERE TABLEID=" + KTC[0].TableNo + " AND VCHKOTID IN(SELECT VCHKOTID FROM TABPOSKOTDETAILS  WHERE VCHITEMSTATUS='N' AND TABLEID=" + KTC[0].TableNo + "))AS KOTCANCELCOUNT;";
        //// string KotUpdate = "select count(*) from (SELECT DISTINCT TABLEID,TK.VCHKOTID   FROM TABPOSKOTDETAILS TK JOIN TABPOSKOT TP ON TK.VCHKOTID=TP.VCHKOTID   WHERE VCHITEMSTATUS='N' AND TP.VCHKOTID not in (select VCHKOTID from TABPOSKOTchangeDETAILS where VCHITEMSTATUS='N') union all SELECT DISTINCT tk.TABLEID, TK.VCHKOTID FROM TABPOSKOTchangeDETAILS TK JOIN TABPOSKOT TP ON TK.VCHKOTID=TP.VCHKOTID   WHERE VCHITEMSTATUS='N'  ) x where VCHKOTID not in (select VCHKOTID from TABPOSKOTCANCEL) and tableid in (select kot.tableid from (select  tableid from tabpostablesandcoversmst where tableid=" + KTC[0].TableNo + " union all select distinct  cast( totableid as bigint) from tabposmergetable where cast( fromtableid as bigint) = " + KTC[0].TableNo + ") kot join tabpostablesandcoversmst tab on tab.tableid =kot.tableid and status in ('R','M') )";

        ////Madhu
        //string KotUpdate = "select count(*) from (SELECT DISTINCT TABLEID,TK.VCHKOTID   FROM TABPOSKOTDETAILS TK JOIN TABPOSKOT TP ON TK.VCHKOTID=TP.VCHKOTID   WHERE VCHITEMSTATUS='N' AND TP.VCHKOTID not in (select VCHKOTID from TABPOSKOTchangeDETAILS where VCHITEMSTATUS='N') union all SELECT DISTINCT tk.TABLEID, TK.VCHKOTID FROM TABPOSKOTchangeDETAILS TK JOIN TABPOSKOT TP ON TK.VCHKOTID=TP.VCHKOTID   WHERE VCHITEMSTATUS='N' ) x where VCHKOTID not in (select VCHKOTID from TABPOSKOTCANCEL) and tableid in ( select tableid from tabpostablesandcoversmst where tableid=" + KTC[0].TableNo + " union select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + KTC[0].TableNo + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + KTC[0].TableNo + " union select totableid::int from tabposmergetable where fromtableid in (select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + KTC[0].TableNo + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + KTC[0].TableNo + ") and totableid::int in(select tableid from tabpostablesandcoversmst where status='M') union  select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid in( select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + KTC[0].TableNo + ") order by tableid);";
        //string Kotscount = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, KotUpdate));


        //if (Convert.ToInt32(Kotscount) == 0)
        //{
        //    // NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update  tabpostablesandcoversmst SET status='A' where tableid in  (select  tableid from tabpostablesandcoversmst where tableid=" + KTC[0].TableNo + " union all select distinct  cast( totableid as bigint) from tabposmergetable where cast( fromtableid as bigint) = " + KTC[0].TableNo + ")");
        //    //  NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update  tabpostablesandcoversmst SET status='A' where tableid in  (select  tableid from tabpostablesandcoversmst where tableid=" + KTC[0].TableNo + " union  select distinct  cast( totableid as bigint) from tabposmergetable where  fromtableid  = " + KTC[0].TableNo + " union  select distinct  mergetableid from tabposkotmergedtables where tableid = " + KTC[0].TableNo + ")");

        //    //Madhu
        //    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update  tabpostablesandcoversmst SET status='A' where tableid in  ( select tableid from tabpostablesandcoversmst where tableid=" + KTC[0].TableNo + " union select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + KTC[0].TableNo + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + KTC[0].TableNo + " union select totableid::int from tabposmergetable where fromtableid in (select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + KTC[0].TableNo + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + KTC[0].TableNo + ") and totableid::int in(select tableid from tabpostablesandcoversmst where status='M') union  select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid in( select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + KTC[0].TableNo + ") order by tableid)");
        //}

        #endregion

        private void CloseCon(NpgsqlConnection con)
        {
            //if (con.State == ConnectionState.Open)
            //{
            con.Close();
            con.Dispose();
            con.ClearPool();
            //}
        }

        #region BILL-GENERATION
        public List<BillGeneration> ShowkotDetails(Core.BillGeneration objBillGeneration)
        {
            List<BillGeneration> lstBillGeneration = new List<BillGeneration>();
            DataTable dt = new DataTable();
            string Query = string.Empty;

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();

                    Query = "select to_char('now'::text::date::timestamp with time zone, 'dd/MM/yyyy'::text)::character varying AS billdate, ((split_part(split_part(now()::text, ' '::text, 2), ':'::text, 1) || ':'::text) || split_part(split_part(now()::text, ' '::text, 2), ':'::text, 2))::character varying AS billtime,trkd.kotid,trkd.vchkotid,trkd.itemname,itemid,trkd.itemqty,trkd.itemcode,trkd.itemrate as numrate,(trkd.itemqty*trkd.itemrate)as numamount,sessionid,sectionid,vchhostname,'Y'::character varying AS chkselect,'N'::character varying AS chknoncharge from tabrmskotdetails trkd JOIN tabrmskot trk  ON  trk.vchkotid=trkd.vchkotid where vchitemstatus='N' and tableid=" + objBillGeneration.tableid + " order by kotid";
                    string tablename = NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, " select tablesname from tabpostablesandcoversmst  where  tableid  =" + objBillGeneration.tableid + ";").ToString();
                    objBillGeneration.tableid = objBillGeneration.tableid;
                    objBillGeneration.tablesname = tablename;
                    using (NpgsqlDataReader npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, Query))
                    {

                        if (npgdr != null)
                        {
                            decimal totalamount = 0;
                            while (npgdr.Read())
                            {
                                Core.BillGeneration objBillGeneration1 = new Core.BillGeneration();
                                objBillGeneration1.kotid = Convert.ToInt64(npgdr["kotid"]);
                                objBillGeneration1.vchkotid = npgdr["vchkotid"].ToString();
                                objBillGeneration1.sessionid = Convert.ToInt64(npgdr["sessionid"]);
                                objBillGeneration1.sectionid = Convert.ToInt64(npgdr["sectionid"]);
                                objBillGeneration1.vchhostname = npgdr["vchhostname"].ToString();
                                objBillGeneration1.itemid = Convert.ToInt64(npgdr["itemid"]);
                                objBillGeneration1.itemname = npgdr["itemname"].ToString();
                                objBillGeneration1.itemcode = npgdr["itemcode"].ToString();
                                objBillGeneration1.itemqty = Convert.ToDecimal(npgdr["itemqty"]);
                                objBillGeneration1.itemrate = Convert.ToDecimal(npgdr["numrate"]);
                                objBillGeneration1.numamount = Convert.ToDecimal(npgdr["numamount"]);
                                objBillGeneration1.chkselect = npgdr["chkselect"].ToString();
                                objBillGeneration1.noncharge = npgdr["chknoncharge"].ToString();
                                objBillGeneration1.select = npgdr["chkselect"].ToString();
                                objBillGeneration1.chknoncharge = npgdr["chknoncharge"].ToString();
                                lstBillGeneration.Add(objBillGeneration1);

                                objBillGeneration.billtime = npgdr["billtime"].ToString();
                                objBillGeneration.billdate = npgdr["billdate"].ToString();
                                totalamount = totalamount += Convert.ToDecimal(npgdr["numamount"]); ;
                            }

                            objBillGeneration.totalamount = totalamount;
                            objBillGeneration.gross = totalamount;
                            objBillGeneration.netamount = totalamount;

                        }
                    }
                    con.Close();
                }
            }
            catch
            {
                throw;
            }

            return lstBillGeneration;
        }

        public List<BillGeneration> ShowBillVouchers()
        {
            List<BillGeneration> lstVouchers = new List<BillGeneration>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    string strInsert = "SELECT VOUCHERID,VOUCHERNAME FROM TABPOSVOUCHERMST WHERE datvalidupto >= current_date and  STATUSID=1 and vchvoucherstatus='I' ORDER BY VOUCHERNAME;";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();

                        while (npgdr.Read())
                        {
                            BillGeneration objVouchers = new BillGeneration();
                            objVouchers.VoucherId = Convert.ToInt32(npgdr["VOUCHERID"]);
                            objVouchers.VoucherName = npgdr["VOUCHERNAME"].ToString();
                            lstVouchers.Add(objVouchers);
                        }


                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillEdit");
                throw ex;

            }
            finally
            {
                if (npgdr != null)
                {
                    npgdr.Dispose();
                }
            }
            return lstVouchers;

        }

        public List<BillGeneration> ShowServicetax()
        {
            List<BillGeneration> lstBillGeneration = new List<BillGeneration>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    string strInsert = "select numpercentage from tabpostaxmst where taxname='SERVICE TAX';";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {

                        npgdr = cmd.ExecuteReader();

                        while (npgdr.Read())
                        {
                            BillGeneration objBillGeneration = new BillGeneration();

                            objBillGeneration.servicetax = Convert.ToDecimal(npgdr["numpercentage"]);

                            lstBillGeneration.Add(objBillGeneration);
                        }

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillGeneration");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstBillGeneration;

        }
        public List<BillGeneration> ShowServiceCharge()
        {
            List<BillGeneration> lstBillGeneration = new List<BillGeneration>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    string strInsert = "select numpercentage from tabpostaxmst where taxname='SERVICE CHARGE';";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();

                        while (npgdr.Read())
                        {
                            BillGeneration objBillGeneration = new BillGeneration();

                            objBillGeneration.servicecharges = Convert.ToDecimal(npgdr["numpercentage"]);

                            lstBillGeneration.Add(objBillGeneration);
                        }

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillGeneration");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstBillGeneration;

        }

        public bool SaveBillGeneration(BillGeneration objBillGeneration, List<BillGeneration> lstKotDetails)
        {
            DataTable dt = new DataTable();
            bool isSaved = false;
            string strBillEdit = string.Empty;
            string strKotDetails = string.Empty;
            string strDiscount = string.Empty;
            string strVoucher = string.Empty;
            string strChargableornot = string.Empty;
            int countoftotables = 0;
            string KotString = "";
            string KOTNEWSTRING = "";

            int strDetailid = 0;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                strBillEdit = "select coalesce(MAX(CAST(COALESCE((substring(vchbillno FROM '[0-9]+')),'0')AS INTEGER)),0) as materialid  from (select vchbillno from tabposbillgeneration union select vchbillno from tabposbillcancel ) bill";
                // strBillEdit = "select coalesce(MAX(CAST((substring(vchbillno FROM '[0-9]+')),'0')AS INTEGER)),'0') as materialid  from (select vchbillno from tabposbillgeneration union select vchbillno from tabposbillcancel ) bill";
                Int64 nextid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strBillEdit));
                objBillGeneration.billno = "B" + (nextid + 1);

                strBillEdit = "INSERT INTO tabposbillgeneration(vchbillno,datkotdate,kottime , tableid,numtotalamount, numdiscount, numvoucherdiscount, numgross, numservicetax, numservicetaxvale,numservicecharges,numservicechargevale, numnet, vchauthorisedby, statusid, createdby,createddate,numservicetax15onservicecharge,vattax,vattaxvalue) VALUES ('" + ManageQuote(objBillGeneration.billno) + "','" + FormatDate(objBillGeneration.billdate) + "','" + ManageQuote(objBillGeneration.billtime) + "','" + (objBillGeneration.tableid) + "','" + Convert.ToDecimal(objBillGeneration.totalamount) + "','" + Convert.ToDecimal(objBillGeneration.discountamount) + "','" + Convert.ToDecimal(objBillGeneration.VDiscountAmount) + "','" + Convert.ToDecimal(objBillGeneration.gross) + "'," + Convert.ToDecimal(objBillGeneration.servicetax) + "," + Convert.ToDecimal(objBillGeneration.servicetaxvalue) + "," + Convert.ToDecimal(objBillGeneration.servicecharges) + "," + Convert.ToDecimal(objBillGeneration.servicechargesvalue) + ",'" + Convert.ToDecimal(objBillGeneration.netamount) + "','" + ManageQuote(objBillGeneration.vchauthorisedby) + "'," + objBillGeneration.statusid + "," + objBillGeneration.createdby + ",current_timestamp," + objBillGeneration.ServiceTaxOnServiceCharge + "," + objBillGeneration.VATtax + "," + objBillGeneration.VAtaxvalue + ")RETURNING recordid;";
                strDetailid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strBillEdit));

                for (int i = 0; i < lstKotDetails.Count; i++)
                {
                    if (lstKotDetails[i].chkselect == "Y")
                    {
                        strKotDetails = "INSERT INTO tabposbillgenerationdetails(detailsid, vchbillno, kotid, vchkotid, itemid, itemname,itemcode, itemqty, itemrate,numamount, vchitemchargableornot, reason, statusid,createdby, createddate,vchdiscounttype,vchdiscount) VALUES (" + strDetailid + ",'" + ManageQuote(objBillGeneration.billno) + "'," + (lstKotDetails[i].kotid) + ",'" + ManageQuote(lstKotDetails[i].vchkotid) + "','" + (lstKotDetails[i].itemid) + "','" + ManageQuote(lstKotDetails[i].itemname) + "','" + ManageQuote(lstKotDetails[i].itemcode) + "'," + lstKotDetails[i].itemqty + "," + lstKotDetails[i].itemrate + "," + lstKotDetails[i].numamount + ",'" + ManageQuote(lstKotDetails[i].chknoncharge) + "','" + lstKotDetails[i].ReasonName + "'," + objBillGeneration.statusid + "," + objBillGeneration.createdby + ",current_timestamp,'" + lstKotDetails[i].IndDiscountType + "'," + lstKotDetails[i].IndiviItemQty + ");";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strKotDetails);

                        strKotDetails = "update tabrmskotdetails set vchitemstatus ='Y',modifiedby=" + objBillGeneration.createdby + ",modifieddate=current_timestamp where vchkotid='" + ManageQuote(lstKotDetails[i].vchkotid) + "' and itemcode = '" + ManageQuote(lstKotDetails[i].itemcode) + "';";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strKotDetails);

                    }

                }

                if (objBillGeneration.discount > 0)
                {
                    strDiscount = "INSERT INTO tabposbillgenerationdiscountdetails(detailsid, vchbillno, vchdiscounttype, vchdiscount,reason, statusid, createdby, createddate) VALUES ('" + strDetailid + "','" + ManageQuote(objBillGeneration.billno) + "','" + ManageQuote(objBillGeneration.discounttype) + "','" + objBillGeneration.discount + "','" + ManageQuote(objBillGeneration.ReasonName) + "'," + objBillGeneration.statusid + "," + objBillGeneration.createdby + ",current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strDiscount);
                }
                if (objBillGeneration.VDiscountAmount > 0)
                {
                    strDiscount = "INSERT INTO tabposbillgenerationvoucherdetails(detailsid, vchbillno, vchvoucherno,numvoucherdiscountpercentage, numvoucherdiscountamount,statusid, createdby, createddate) VALUES ('" + strDetailid + "','" + ManageQuote(objBillGeneration.billno) + "','" + (objBillGeneration.VoucherName) + "','" + objBillGeneration.VDiscountPercentage + "','" + objBillGeneration.VDiscountAmount + "'," + objBillGeneration.statusid + "," + objBillGeneration.createdby + ",current_timestamp); ";
                    strDiscount = strDiscount + "update tabposvouchermst set vchvoucherstatus='Y',modifiedby=" + objBillGeneration.createdby + ",modifieddate=current_timestamp  where voucherid= " + objBillGeneration.VoucherId + ";";

                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strDiscount);
                }


                string GETsTATUS = "select array_to_string(array_agg(trkd.vchitemstatus), ',') from tabrmskotdetails trkd join tabrmskot trk on trkd.vchkotid=trk.vchkotid where tableid=" + objBillGeneration.tableid + "";
                string sTATUSq = (NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, GETsTATUS)).ToString();

                if (sTATUSq.Contains("N") == true)
                {
                    strKotDetails = "update tabpostablesandcoversmst set status ='R',modifiedby=" + objBillGeneration.createdby + ",modifieddate=current_timestamp  where tableid=" + (objBillGeneration.tableid) + ";";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strKotDetails);
                }
                else
                {
                    strKotDetails = "update tabpostablesandcoversmst set status ='B',modifiedby=" + objBillGeneration.createdby + ",modifieddate=current_timestamp  where tableid=" + (objBillGeneration.tableid) + ";";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strKotDetails);

                }
                trans.Commit();
                isSaved = true;

            }
            catch (Exception ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "BillEdit");
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                    trans.Dispose();
                }

            }

            return isSaved; ;
        }

        #endregion

        #region BillCancel

        public DataTable GetBillComboData(string TableId)
        {
            DataTable dt = new DataTable();

            try
            {

                string strData = "select vchbillno as recordid ,vchbillno from tabposbillgeneration  where statusid=1  and tableid=" + TableId + " and vchbillno not in (select vchbillno from tabposbillCancel);";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public DataTable getBillDetals(string TableId)
        {
            DataTable dt = new DataTable();
            int GettableId = 0;

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();

                    string strgettableid = "select tableid from tabpostablesandcoversmst where tablesname='" + TableId + "'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(strgettableid, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            KOTTakingDTO objKot = new KOTTakingDTO();

                            GettableId = Convert.ToInt32(npgdr["tableid"].ToString());


                        }
                    }

                    if (GettableId != 0)
                    {

                        string strData = " select vchbillno ,  numnet,to_char(datkotdate,'dd-mm-yyyy') as datkotdate from tabposbillgeneration where tableid=" + GettableId + " and vchbillno not in (select vchbillno from tabposbillsettlement )";
                        dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public bool SaveBillSDeleted(string Jsons, string createby)
        {

            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(Jsons);

            int GettableId = 0;
            string tablename = JsonData["TableId"].ToString();

            DataTable dt = new DataTable();
            bool isSaved = false;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }



                using (con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();

                    string strgettableid = "select tableid from tabpostablesandcoversmst where tablesname='" + tablename + "'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(strgettableid, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            KOTTakingDTO objKot = new KOTTakingDTO();

                            GettableId = Convert.ToInt32(npgdr["tableid"].ToString());


                        }
                    }

                    if (GettableId != 0)
                    {






                        // string strQuery = "INSERT INTO tabposbillcancel(vchbillno, datkotdate, tableid, numtotalamount, statusid,createdby, createddate)  VALUES ('" + ManageQuote(JsonData["vchbillno"].ToString()) + "','" + ManageQuote(JsonData["datkotdate"].ToString()) + "'," + ManageQuote(JsonData["TableId"].ToString()) + "," + ManageQuote(JsonData["numnet"].ToString()) + ",1," + createby + ",current_timestamp);;";



                        string strQuery = "INSERT INTO tabposbillcancel(vchbillno, datkotdate, tableid, numtotalamount, statusid,createdby, createddate)  VALUES ('" + ManageQuote(JsonData["vchbillno"].ToString()) + "','" + ManageQuote(JsonData["datkotdate"].ToString()) + "'," + GettableId + "," + ManageQuote(JsonData["numnet"].ToString()) + ",1," + createby + ",current_timestamp);;";


                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, strQuery);
                        isSaved = true;
                        string get = "select  vchkotid,itemname from tabposbillgenerationdetails where vchbillno='" + ManageQuote(JsonData["vchbillno"].ToString()) + "'";
                        dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, get).Tables[0];
                        int len = dt.Rows.Count;

                        for (int i = 0; i < len; i++)
                        {
                            string updateKotdetails = "update tabrmskotdetails  set  vchitemstatus='N' where itemname='" + dt.Rows[i]["itemname"].ToString() + "' and vchkotid='" + dt.Rows[i]["vchkotid"].ToString() + "'";
                            NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, updateKotdetails);
                        }

                        // string updatetables = "update tabpostablesandcoversmst set status='R' where tableid='" + (JsonData["TableId"].ToString()) + "'";


                        string updatetables = "update tabpostablesandcoversmst set status='R' where tableid=" + GettableId + "";



                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, updatetables);



                        string deleteBillgenDiscount = "delete from TABPOSBILLGENERATIONDISCOUNTDETAILS where vchbillno='" + ManageQuote(JsonData["vchbillno"].ToString()) + "'";
                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, deleteBillgenDiscount);
                        string deleteBillgendetailsVoucher = "delete from TABPOSBILLGENERATIONVOUCHERDETAILS where vchbillno='" + ManageQuote(JsonData["vchbillno"].ToString()) + "'";
                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, deleteBillgendetailsVoucher);
                        string deleteBillgendetails = "delete from tabposbillgenerationdetails where vchbillno='" + ManageQuote(JsonData["vchbillno"].ToString()) + "'";
                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, deleteBillgendetails);
                        string deleteBillgen = "delete from tabposbillgeneration where vchbillno='" + ManageQuote(JsonData["vchbillno"].ToString()) + "'";
                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, deleteBillgen);



                        string deleteBillEditDiscount = "delete from TABPOSBILLGENERATIONDISCOUNTDETAILS where vchbillno='" + ManageQuote(JsonData["vchbillno"].ToString()) + "'";
                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, deleteBillEditDiscount);
                        string deleteBillEditdetailsVoucher = "delete from TABPOSBILLGENERATIONVOUCHERDETAILS where vchbillno='" + ManageQuote(JsonData["vchbillno"].ToString()) + "'";
                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, deleteBillEditdetailsVoucher);
                        string deleteBillEditdetails = "delete from TABPOSBILLEDITDETAILS where vchbillno='" + ManageQuote(JsonData["vchbillno"].ToString()) + "'";
                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, deleteBillEditdetails);
                        string deleteBillEdit = "delete from TABPOSBILLEDIT where vchbillno='" + ManageQuote(JsonData["vchbillno"].ToString()) + "'";
                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, deleteBillEdit);
                    }
                }


            }
            catch (Exception ex)
            {
                isSaved = false;
                throw ex;
            }
            finally
            {

                con.Close();
                con.Dispose();


            }
            return isSaved;

        }

        #endregion


        #region DASHBOARD...


        //public List<DashboardDTO> ShowAlltables(int Userid)
        //{
        //    List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();
        //    try
        //    {
        //        using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
        //        {
        //            con.Open();
        //            string strInsert = " SELECT TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST  WHERE STATUS IN ('A')  and statusid=1 UNION ALL SELECT TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B') AND  TABLEID IN(select TABLEID from tabrmskot where kotid in(select max(kotid) from tabrmskot group by TABLEID ) and createdby=" + Userid + " )   and statusid=1 order by TABLEID";
        //            using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
        //            {
        //                npgdr = cmd.ExecuteReader();

        //                while (npgdr.Read())
        //                {
        //                    DashboardDTO objKOTTakingDTO = new DashboardDTO();
        //                    objKOTTakingDTO.Tableid = Convert.ToInt32(npgdr["TABLEID"]);
        //                    objKOTTakingDTO.TableName = Convert.ToString(npgdr["TABLESNAME"]);
        //                    objKOTTakingDTO.TableStatus = Convert.ToString(npgdr["STATUS"]);

        //                    lstKOTTakingDTO.Add(objKOTTakingDTO);
        //                }

        //            }
        //            con.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "ShowAvailabletables");
        //        throw ex;

        //    }
        //    finally
        //    {
        //        npgdr.Dispose();
        //    }
        //    return lstKOTTakingDTO;
        //}

        public DashboardDTO ShowKOtsCount(int userid)
        {
            DashboardDTO objKOTTakingDTO = new DashboardDTO();
            string strInsert = "";
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    if (userid != 1)
                    {
                        strInsert = "SELECT (SELECT COALESCE(COUNT(VCHKOTID),0) FROM TABrmsKOT WHERE datkotdate::DATE=CURRENT_DATE and createdby=" + userid + ") AS TOTALKOTS ,(SELECT COALESCE(COUNT(distinct VCHKOTID),0) FROM TABrmsKOTDETAILS WHERE VCHKOTID not in(select vchkotid from TABrmsKOTDETAILS where VCHITEMSTATUS='N') AND CREATEDDATE::DATE=CURRENT_DATE and createdby=" + userid + ") AS COMPLETEDKOTS,(SELECT COALESCE(COUNT(distinct VCHKOTID),0) FROM TABrmsKOTDETAILS WHERE CREATEDDATE::DATE=CURRENT_DATE and createdby=" + userid + ")-(SELECT COALESCE(COUNT(distinct VCHKOTID),0) FROM TABrmsKOTDETAILS WHERE vchkotid not in(select distinct vchkotid from TABrmsKOTDETAILS where VCHITEMSTATUS='N') and  CREATEDDATE::DATE=CURRENT_DATE and createdby=" + userid + ") RUNNINGKOTS, (select  coalesce(count(distinct vchkotid),0)  from tabrmskotcancel where createddate::date=current_date and createdby=" + userid + ")as CancelKots;";
                    }
                    else
                    {
                        strInsert = "SELECT (SELECT COALESCE(COUNT(VCHKOTID),0) FROM TABrmsKOT WHERE datkotdate::DATE=CURRENT_DATE ) AS TOTALKOTS ,(SELECT COALESCE(COUNT(distinct VCHKOTID),0) FROM TABrmsKOTDETAILS WHERE VCHKOTID not in(select vchkotid from TABrmsKOTDETAILS where VCHITEMSTATUS='N') AND CREATEDDATE::DATE=CURRENT_DATE ) AS COMPLETEDKOTS,(SELECT COALESCE(COUNT(distinct VCHKOTID),0) FROM TABrmsKOTDETAILS WHERE CREATEDDATE::DATE=CURRENT_DATE )-(SELECT COALESCE(COUNT(distinct VCHKOTID),0) FROM TABrmsKOTDETAILS WHERE vchkotid not in(select distinct vchkotid from TABrmsKOTDETAILS where VCHITEMSTATUS='N') and  CREATEDDATE::DATE=CURRENT_DATE ) RUNNINGKOTS, (select  coalesce(count(distinct vchkotid),0)  from tabrmskotcancel where createddate::date=current_date )as CancelKots;";

                    }
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {

                            objKOTTakingDTO.Totalkots = Convert.ToInt32(npgdr["TOTALKOTS"]);
                            objKOTTakingDTO.CompletedKots = Convert.ToInt32(npgdr["COMPLETEDKOTS"]);
                            objKOTTakingDTO.RunningKots = Convert.ToInt32(npgdr["RUNNINGKOTS"]);
                            objKOTTakingDTO.CancelKots = Convert.ToInt32(npgdr["CancelKots"]);


                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowKOtsCount");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return objKOTTakingDTO;
        }

        public string CheckBillStatus(string tableid)
        {
            string billstatus = string.Empty;
            string billstatusB = string.Empty;
            string billstatusR = string.Empty;
            int countR = -1;
            try
            {

                string KotCount = "select count(*) from tabrmskot tpk join   tabrmskotdetails tpkd on tpk.vchkotid=tpkd.vchkotid where tableid=" + tableid + " and vchitemstatus='N'";
                int kotC = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, KotCount).ToString());

                string Count = "select count(*) from tabpostablesandcoversmst where status='R';";
                countR = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, Count).ToString());




                if (kotC == 0)
                {

                    string strInsert = "select status from tabpostablesandcoversmst  where tableid  =" + tableid + ";";
                    billstatus = NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert).ToString();

                }
                else
                {
                    billstatus = "P";
                }

                string KotCountB = "select count(x.*) from (SELECT DISTINCT T1.VCHKOTID,TABLEID FROM TABRMSKOTDETAILS T1 JOIN TABRMSKOT T2 ON T1.VCHKOTID=T2.VCHKOTID WHERE T1.VCHITEMSTATUS='Y' )x join (SELECT DISTINCT VCHKOTID,TABLEID FROM TABPOSBILLGENERATIONDETAILS   T1 JOIN TABPOSBILLGENERATION T2 ON T1.VCHBILLNO=T2.VCHBILLNO WHERE T2.VCHBILLNO NOT IN(SELECT VCHBILLNO FROM TABPOSBILLSETTLEMENT ))y on x.vchkotid=y.vchkotid where x.tableid=" + tableid + "";
                int kotCB = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, KotCountB).ToString());



                if (kotCB == 0)
                {

                    billstatusB = "No";

                }
                else
                {
                    billstatusB = "Yes";
                }
                string KotCountR = "select count(status) from tabpostablesandcoversmst  where status in('R','B')";
                int kotCR = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, KotCountR).ToString());



                if (kotCR == 1)
                {

                    billstatusR = "S";

                }
                else
                {
                    billstatusR = "D";
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowKOtsCount");
                throw ex;

            }
            finally
            {

            }
            return billstatus + ':' + countR + ':' + billstatusB + ':' + billstatusR;
        }



        #region Charts

        public List<DashboardDTO> ChartListForCategorywiseToday(int Userid)
        {
            string strInsert = "";
            List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();

                    if (Userid == 1)
                    {

                        //strInsert = "SELECT ITEMCATEGORYNAME,sum(round(PAIDAMOUNT,2)) as PAIDAMOUNT  FROM vwbilldatecategorywiseamount  group by ITEMCATEGORYNAME";
                        strInsert = "select productcategoryname as ITEMCATEGORYNAME,sum(tsd.numsaleamount) as PAIDAMOUNT from tabmmssaledetails tsd join tabmmsproductmst  tp on CAST (tsd.vchproductid AS bigint) = CAST (tp.productid AS bigint) join tabmmsproductcategorymst tpc on coalesce(tp.productcategoryid, 1)=tpc.productcategoryid join tabmmssale ts on tsd.detailsid=ts.recordid where datsaledate=current_date group by productcategoryname  ";

                    }
                    else
                    {

                        //strInsert = "SELECT ITEMCATEGORYNAME,round(PAIDAMOUNT,2) as PAIDAMOUNT FROM  vwbilldatecategorywiseamount where createdby=" + Userid + "; ";
                        strInsert = "SELECT ITEMCATEGORYNAME,round(PAIDAMOUNT,2) as PAIDAMOUNT FROM  vwbilldatecategorywiseamount where billdate=current_date and  createdby=" + Userid + "; ";
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            DashboardDTO objKOTTakingDTO = new DashboardDTO();
                            objKOTTakingDTO.PlanName = Convert.ToString(npgdr["ITEMCATEGORYNAME"]);
                            objKOTTakingDTO.PaymentAmount = Convert.ToDecimal(npgdr["PAIDAMOUNT"]);

                            lstKOTTakingDTO.Add(objKOTTakingDTO);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ChartList");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstKOTTakingDTO;
        }

        public List<DashboardDTO> ChartListForCategorywiseWeek(int Userid)
        {
            List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();
            string strInsert = "";
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();

                    if (Userid == 1)
                    {

                        strInsert = "select productcategoryname as sessionname,sum(tsd.numsaleamount) as PAIDAMOUNT from tabmmssaledetails tsd join tabmmsproductmst  tp on CAST (tsd.vchproductid AS bigint) = CAST (tp.productid AS bigint) join tabmmsproductcategorymst tpc on coalesce(tp.productcategoryid, 1)=tpc.productcategoryid join tabmmssale ts on tsd.detailsid=ts.recordid where datsaledate between current_date-7  and current_date group by productcategoryname  ";

                    }
                    else
                    {

                        strInsert = "select sessionname,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid WHERE vw.createdby=" + Userid + " and billdate = 'now'::text::date group by vw.sessionid,sessionname,vw.createdby; ";  

                    }



                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            DashboardDTO objKOTTakingDTO = new DashboardDTO();
                            objKOTTakingDTO.PlanName = Convert.ToString(npgdr["sessionname"]);
                            objKOTTakingDTO.PaymentAmount = Convert.ToDecimal(npgdr["PAIDAMOUNT"]);

                            lstKOTTakingDTO.Add(objKOTTakingDTO);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ChartList");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstKOTTakingDTO;
        }

        public List<DashboardDTO> ChartListForCategorywiseMonth(int Userid)
        {
            List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();
            string strInsert = "";
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();


                    if (Userid == 1)
                    {

                        strInsert = "select productcategoryname as SECTIONNAME,sum(tsd.numsaleamount) as PAIDAMOUNT from tabmmssaledetails tsd join tabmmsproductmst  tp on CAST (tsd.vchproductid AS bigint) = CAST (tp.productid AS bigint) join tabmmsproductcategorymst tpc on coalesce(tp.productcategoryid, 1)=tpc.productcategoryid join tabmmssale ts on tsd.detailsid=ts.recordid where datsaledate between current_date-30  and current_date group by productcategoryname  ";

                    }
                    else
                    {

                        strInsert = "SELECT  SECTIONNAME,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) AS PAIDAMOUNT FROM VWITEMDISCOUNTDETAILS VW LEFT JOIN TABRMSKOT TK ON  VW.VCHKOTID=TK.VCHKOTID LEFT JOIN TABPOSSECTIONMST TS ON TK.SECTIONID=TS.SECTIONID   WHERE BILLDATE=CURRENT_DATE  AND VW.CREATEDBY=" + Userid + "   GROUP BY SECTIONNAME;";

                    }




                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            DashboardDTO objKOTTakingDTO = new DashboardDTO();
                            objKOTTakingDTO.PlanName = Convert.ToString(npgdr["SECTIONNAME"]);
                            objKOTTakingDTO.PaymentAmount = Convert.ToDecimal(npgdr["PAIDAMOUNT"]);

                            lstKOTTakingDTO.Add(objKOTTakingDTO);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ChartList");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstKOTTakingDTO;
        }

        #endregion

        #endregion

        #region KOT TABLE CHANGE

        // table change From details
        public KOTTablesDTO ShowFromTableNoDetails(string tableId)
        {
            KOTTablesDTO objTables = new KOTTablesDTO();
            try
            {
                if (tableId != null)
                {

                    string strInsert = "select distinct kt.sectionid,tablesname,kt.tableid,kt.numcovers,kt.numadults,numkids,ktd.vchitemstatus,( select string_agg(y.vchkotid, ',') as m from (select x.vchkotid from (select distinct kt.sectionid,tablesname,kt.tableid,kt.numcovers,kt.numadults,numkids, kt.vchkotid,kt.kotid,ktd.vchitemstatus  from tabrmskot kt join  tabrmskotdetails ktd on kt.kotid=ktd.kotid  join tabpostablesandcoversmst tpc on kt.tableid=tpc.tableid  where   vchitemstatus ='N'  and kt.tableid=" + tableId + " )x)y  ) as vchkotid  from tabrmskot kt join  tabrmskotdetails ktd on kt.kotid=ktd.kotid  join tabpostablesandcoversmst tpc on kt.tableid=tpc.tableid  where   vchitemstatus ='N'  and kt.tableid=" + tableId + "";
                    npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);

                    while (npgdr.Read())
                    {

                        objTables.fromtableid = npgdr["tableid"].ToString();
                        objTables.fromTableName = npgdr["tablesname"].ToString();
                        objTables.numfromcovers = Convert.ToInt32(npgdr["numcovers"].ToString());
                        objTables.numfromadults = Convert.ToInt32(npgdr["numadults"].ToString());
                        objTables.numfromkids = Convert.ToInt32(npgdr["numkids"].ToString());
                        objTables.SectionId = Convert.ToInt32(npgdr["sectionid"].ToString());

                        objTables.numtocovers = Convert.ToInt32(npgdr["numcovers"].ToString());
                        objTables.numtoadults = Convert.ToInt32(npgdr["numadults"].ToString());
                        objTables.numtokids = Convert.ToInt32(npgdr["numkids"].ToString());


                        objTables.kotid = npgdr["vchkotid"].ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowTableandCovers");
                throw ex;
                // ErrorMessage(ex);
            }
            return objTables;
        }

        // table change To details
        public List<KOTTablesDTO> ShowToTableNoDetails(string tab)
        {
            List<KOTTablesDTO> lstPrinterdetails = new List<KOTTablesDTO>();
            try
            {
                if (tab != null)
                {
                    string strInsert = "select cast (coalesce(regexp_replace(tablesname,'[abcdefghijklmnopqrstuvwxyz]', '', 'gi'),'0') as numeric) as tablename,tableid,numcovers from tabpostablesandcoversmst where status ='A' and tableid <>18  and statusid <> 2 and tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and statusid=1) order by cast (coalesce(regexp_replace(tablesname,'[abcdefghijklmnopqrstuvwxyz]', '', 'gi'),'0') as numeric);";
                    //  string strInsert = "select cast (coalesce(regexp_replace(tablesname,'[abcdefghijklmnopqrstuvwxyz]', '', 'gi'),'0') as numeric) as tablename,tableid,numcovers from tabpostablesandcoversmst where status in ('A') and tableid <>" + tab + "  and statusid <> 2 order by cast (coalesce(regexp_replace(tablesname,'[abcdefghijklmnopqrstuvwxyz]', '', 'gi'),'0') as numeric);";
                    npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    while (npgdr.Read())
                    {
                        KOTTablesDTO objTables = new KOTTablesDTO();
                        objTables.totableid = npgdr["tablename"].ToString();
                        objTables.TOID = npgdr["tableid"].ToString();
                        objTables.numtocovers = Convert.ToInt32(npgdr["numcovers"]);
                        lstPrinterdetails.Add(objTables);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowToTableNoDetails");
                throw ex;
                //ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstPrinterdetails;
        }


        public List<KOTTablesDTO> ShowToTableNoDetailsRunning(string tab)
        {
            List<KOTTablesDTO> lstPrinterdetails = new List<KOTTablesDTO>();
            try
            {
                if (tab != null)
                {
                    string strInsert = "select tableid,numcovers from tabpostablesandcoversmst where status in ('R','B') and statusid=1 and tableid <> " + tab + " order by tableid;";
                    npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    while (npgdr.Read())
                    {
                        KOTTablesDTO objTables = new KOTTablesDTO();





                        objTables.totableid = npgdr["tableid"].ToString();
                        objTables.numtocovers = Convert.ToInt32(npgdr["numcovers"]);
                        lstPrinterdetails.Add(objTables);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowToTableNoDetailsRunning");
                throw ex;
                //ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstPrinterdetails;
        }


        //Binding Merge Tables
        public List<KOTTablesDTO> ShowToMergeTableNoDetails(string tab)
        {
            List<KOTTablesDTO> lstPrinterdetails = new List<KOTTablesDTO>();
            try
            {
                if (tab != null)
                {
                    // string strInsert = "select tablesname from tabpostablesandcoversmst where tableid=" + tab + " union select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + tab + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + tab + " union select totableid::int from tabposmergetable where fromtableid in (select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + tab + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + tab + ") and totableid::int in(select tableid from tabpostablesandcoversmst where status='M') union  select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid in( select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + tab + ") order by tableid ;";
                    string strInsert = "select tableid from tabpostablesandcoversmst where tableid=" + tab + " union select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + tab + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + tab + " union select totableid::int from tabposmergetable where fromtableid in (select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + tab + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + tab + ") and totableid::int in(select tableid from tabpostablesandcoversmst where status='M') union  select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid in( select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + tab + ") order by tableid ;";
                    npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    while (npgdr.Read())
                    {
                        KOTTablesDTO objTables = new KOTTablesDTO();


                        objTables.totableid = npgdr["tableid"].ToString();
                        objTables.numtocovers = 0;
                        lstPrinterdetails.Add(objTables);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowToTableNoDetails");
                throw ex;
                //ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstPrinterdetails;
        }






        public List<KOTTablesDTO> ShowToTableNoDetailsRunningKotTransfer(string tab)
        {
            List<KOTTablesDTO> lstPrinterdetails = new List<KOTTablesDTO>();

            int GetTableid = 0;


            try
            {
                if (tab != null)
                {
                    using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                    {
                        con.Open();

                        string strgettableid = "select tableid from tabpostablesandcoversmst where tablesname='" + tab + "'";

                        using (NpgsqlCommand cmd = new NpgsqlCommand(strgettableid, con))
                        {
                            npgdr = cmd.ExecuteReader();
                            while (npgdr.Read())
                            {
                                KOTTakingDTO objKot = new KOTTakingDTO();

                                GetTableid = Convert.ToInt32(npgdr["tableid"].ToString());


                            }
                        }
                    }



                    if (GetTableid != 0)
                    {

                        string strInsert = "select tablesname,tableid,numcovers from tabpostablesandcoversmst where status in ('R','B') and statusid=1 and tableid <> " + GetTableid + " order by tableid;";
                        npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                        while (npgdr.Read())
                        {
                            KOTTablesDTO objTables = new KOTTablesDTO();



                            objTables.TableName = (npgdr["tablesname"].ToString());

                            objTables.totableid = npgdr["tableid"].ToString();
                            objTables.numtocovers = Convert.ToInt32(npgdr["numcovers"]);
                            lstPrinterdetails.Add(objTables);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowToTableNoDetailsRunning");
                throw ex;
                //ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstPrinterdetails;
        }


        //Binding Merge Tables
        public List<KOTTablesDTO> ShowToMergeTableNoDetailsKotTransfer(string tab)
        {
            List<KOTTablesDTO> lstPrinterdetails = new List<KOTTablesDTO>();
            int GetTableid = 0;
            try
            {
                if (tab != null)
                {
                    using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                    {
                        con.Open();

                        string strgettableid = "select tableid from tabpostablesandcoversmst where tablesname='" + tab + "'";

                        using (NpgsqlCommand cmd = new NpgsqlCommand(strgettableid, con))
                        {
                            npgdr = cmd.ExecuteReader();
                            while (npgdr.Read())
                            {
                                KOTTakingDTO objKot = new KOTTakingDTO();

                                GetTableid = Convert.ToInt32(npgdr["tableid"].ToString());


                            }
                        }
                    }
                    if (GetTableid != 0)
                    {

                        string strInsert = " select tablesname,tableid from tabpostablesandcoversmst where tableid in(select tableid from tabpostablesandcoversmst where tableid=" + GetTableid + " union select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + GetTableid + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + GetTableid + " union select totableid::int from tabposmergetable where fromtableid in (select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + GetTableid + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + GetTableid + ") and totableid::int in(select tableid from tabpostablesandcoversmst where status='M') union  select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid in( select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + GetTableid + ") order by tableid);";
                        npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                        while (npgdr.Read())
                        {
                            KOTTablesDTO objTables = new KOTTablesDTO();

                            objTables.TableName = (npgdr["tablesname"].ToString());

                            objTables.totableid = npgdr["tableid"].ToString();
                            objTables.numtocovers = 0;
                            lstPrinterdetails.Add(objTables);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowToTableNoDetails");
                throw ex;
                //ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstPrinterdetails;
        }








        //saving fromtotablechange
        //        public int SaveFromToTableDetails(KOTTablesDTO tab, long Createdby, string tableclick)
        //        {
        //            int Count = 0;
        //            try
        //            {
        //                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        //                if (con.State != ConnectionState.Open)
        //                {
        //                    con.Open();
        //                }
        //                trans = con.BeginTransaction();

        //                string Mergestatus = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select status from tabpostablesandcoversmst  where tableid=" + tab.fromtableid + "").ToString();
        //                if (Mergestatus == "M")
        //                {

        //                    string strInCreatedbysert = @"insert into tabpostablechange(fromtableid,numfromcovers,numfromadults,numfromkids, totableid,numtocovers,numtoadults,numtokids,kotid,statusid,createdby,createddate)
        //                                    values(" + tab.fromtableid + "," + tab.numfromcovers + "," + tab.numfromadults + "," + tab.numfromkids + ", " + tab.totableid + "," + tab.numtocovers + "," + tab.numtoadults + "," + tab.numtokids + ",'" + ManageQuote(tab.kotid) + "',1," + Createdby + ",current_timestamp);";
        //                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInCreatedbysert);
        //                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabposmergetable set tablestatus='GM' where totableid='" + tab.fromtableid + "';");
        //                    string kots = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select kotid from tabposmergetable  where fromtableid='" + tableclick + "'").ToString();
        //                    string strQuery = "INSERT INTO tabposmergetable(fromtableid, totableid, kotid, statusid, createdby, createddate,tablestatus)   VALUES (" + tableclick + "," + tab.totableid + ",'" + kots + "',1," + Createdby + ",current_timestamp,'MG');";
        //                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strQuery);
        //                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabpostablesandcoversmst  set  status='A' where tableid=" + tab.fromtableid + "");
        //                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabpostablesandcoversmst  set  status='M' where tableid=" + tab.totableid + "");

        //                }
        //                else
        //                {
        //                    string strInCreatedbysert = @"insert into tabpostablechange(fromtableid,numfromcovers,numfromadults,numfromkids, totableid,numtocovers,numtoadults,numtokids,kotid,statusid,createdby,createddate)
        //                                    values(" + tab.fromtableid + "," + tab.numfromcovers + "," + tab.numfromadults + "," + tab.numfromkids + ", " + tab.totableid + "," + tab.numtocovers + "," + tab.numtoadults + "," + tab.numtokids + ",'" + ManageQuote(tab.kotid) + "',1," + Createdby + ",current_timestamp);";
        //                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInCreatedbysert);

        //                    string updateStatusINrmskot = "update tabrmskot set tableid=" + tab.totableid + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp  where tableid='" + tab.fromtableid + "'";
        //                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, updateStatusINrmskot);
        //                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabpostablesandcoversmst set status='A' where tableid=" + tab.fromtableid + ";");
        //                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabpostablesandcoversmst set status='R' where tableid=" + tab.totableid + ";");
        //                    int countvocher = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from tabposbillgeneration where tableid='" + tab.fromtableid + "'"));
        //                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabposmergetable set fromtableid=" + tab.totableid + " where fromtableid=" + tab.fromtableid + " and tablestatus='MG';");

        //                    //if (countvocher > 0)
        //                    //{
        //                    //    string BillNo = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select vchbillno from tabposbillgeneration where tableid='" + tab.fromtableid + "'").ToString();
        //                    //    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabposbillgeneration set tableid=" + tab.totableid + " where tableid=" + tab.fromtableid + " and vchbillno='" + BillNo + "' ;");

        //                    //}
        //                }

        //                trans.Commit();
        //                Count = Count + 1;
        //            }
        //            catch (Exception ex)
        //            {
        //                trans.Rollback();
        //                EventLogger.WriteToErrorLog(ex, "SaveTableandCovers");
        //            }
        //            finally
        //            {
        //                if (con.State == ConnectionState.Open)
        //                {
        //                    con.Close();
        //                    con.Dispose();
        //                    con.ClearPool();
        //                    trans.Dispose();
        //                }

        //            }
        //            return Count;
        //        }
        public int SaveFromToTableDetails(KOTTablesDTO tab, long Createdby, string tableclick)
        {
            int Count = 0;
            int GettableId = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }



                using (con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();

                    string strgettableid = "select tableid from tabpostablesandcoversmst where tablesname='" + tab.fromtableid + "'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(strgettableid, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            KOTTakingDTO objKot = new KOTTakingDTO();

                            GettableId = Convert.ToInt32(npgdr["tableid"].ToString());


                        }
                    }



                    if (GettableId != 0)
                    {
                        trans = con.BeginTransaction();
                        string Mergestatus = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select status from tabpostablesandcoversmst  where tableid=" + GettableId + "").ToString();
                        // string Mergestatus = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select status from tabpostablesandcoversmst  where tableid=" + tab.fromtableid + "").ToString();
                        if (Mergestatus == "M")
                        {

                            string strInCreatedbysert = @"insert into tabpostablechange(fromtableid,numfromcovers,numfromadults,numfromkids, totableid,numtocovers,numtoadults,numtokids,kotid,statusid,createdby,createddate)
                                    values(" + GettableId + "," + tab.numfromcovers + "," + tab.numfromadults + "," + tab.numfromkids + ", " + tab.TOID + "," + tab.numtocovers + "," + tab.numtoadults + "," + tab.numtokids + ",'" + ManageQuote(tab.kotid) + "',1," + Createdby + ",current_timestamp);";
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInCreatedbysert);
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabposmergetable set tablestatus='GM' where totableid='" + GettableId + "';");
                            string kots = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select kotid from tabposmergetable  where fromtableid='" + tableclick + "'").ToString();
                            string strQuery = "INSERT INTO tabposmergetable(fromtableid, totableid, kotid, statusid, createdby, createddate,tablestatus)   VALUES (" + tableclick + "," + tab.TOID + ",'" + kots + "',1," + Createdby + ",current_timestamp,'MG');";
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strQuery);
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabpostablesandcoversmst  set  status='A' where tableid=" + GettableId + "");
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabpostablesandcoversmst  set  status='M' where tableid=" + tab.TOID + "");





                            //                        string strInCreatedbysert = @"insert into tabpostablechange(fromtableid,numfromcovers,numfromadults,numfromkids, totableid,numtocovers,numtoadults,numtokids,kotid,statusid,createdby,createddate)
                            //                                    values(" + tab.fromtableid + "," + tab.numfromcovers + "," + tab.numfromadults + "," + tab.numfromkids + ", " + tab.TOID + "," + tab.numtocovers + "," + tab.numtoadults + "," + tab.numtokids + ",'" + ManageQuote(tab.kotid) + "',1," + Createdby + ",current_timestamp);";
                            //                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInCreatedbysert);
                            //                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabposmergetable set tablestatus='GM' where totableid='" + tab.fromtableid + "';");
                            //                        string kots = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select kotid from tabposmergetable  where fromtableid='" + tableclick + "'").ToString();
                            //                        string strQuery = "INSERT INTO tabposmergetable(fromtableid, totableid, kotid, statusid, createdby, createddate,tablestatus)   VALUES (" + tableclick + "," + tab.TOID + ",'" + kots + "',1," + Createdby + ",current_timestamp,'MG');";
                            //                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strQuery);
                            //                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabpostablesandcoversmst  set  status='A' where tableid=" + tab.fromtableid + "");
                            //                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabpostablesandcoversmst  set  status='M' where tableid=" + tab.TOID + "");

                        }
                        else
                        {




                            string strInCreatedbysert = @"insert into tabpostablechange(fromtableid,numfromcovers,numfromadults,numfromkids, totableid,numtocovers,numtoadults,numtokids,kotid,statusid,createdby,createddate)
                                    values(" + GettableId + "," + tab.numfromcovers + "," + tab.numfromadults + "," + tab.numfromkids + ", " + tab.TOID + "," + tab.numtocovers + "," + tab.numtoadults + "," + tab.numtokids + ",'" + ManageQuote(tab.kotid) + "',1," + Createdby + ",current_timestamp);";
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInCreatedbysert);

                            string updateStatusINrmskot = "update tabrmskot set tableid=" + tab.TOID + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp  where tableid='" + GettableId + "'";
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, updateStatusINrmskot);
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabpostablesandcoversmst set status='A' where tableid=" + GettableId + ";");
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabpostablesandcoversmst set status='R' where tableid=" + tab.TOID + ";");
                            int countvocher = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from tabposbillgeneration where tableid='" + GettableId + "'"));
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabposmergetable set fromtableid=" + tab.TOID + " where fromtableid=" + GettableId + " and tablestatus='MG';");












                            //                        string strInCreatedbysert = @"insert into tabpostablechange(fromtableid,numfromcovers,numfromadults,numfromkids, totableid,numtocovers,numtoadults,numtokids,kotid,statusid,createdby,createddate)
                            //                                    values(" + tab.fromtableid + "," + tab.numfromcovers + "," + tab.numfromadults + "," + tab.numfromkids + ", " + tab.TOID + "," + tab.numtocovers + "," + tab.numtoadults + "," + tab.numtokids + ",'" + ManageQuote(tab.kotid) + "',1," + Createdby + ",current_timestamp);";
                            //                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInCreatedbysert);

                            //                        string updateStatusINrmskot = "update tabrmskot set tableid=" + tab.TOID + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp  where tableid='" + tab.fromtableid + "'";
                            //                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, updateStatusINrmskot);
                            //                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabpostablesandcoversmst set status='A' where tableid=" + tab.fromtableid + ";");
                            //                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabpostablesandcoversmst set status='R' where tableid=" + tab.TOID + ";");
                            //                        int countvocher = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from tabposbillgeneration where tableid='" + tab.fromtableid + "'"));
                            //                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabposmergetable set fromtableid=" + tab.TOID + " where fromtableid=" + tab.fromtableid + " and tablestatus='MG';");

                            //if (countvocher > 0)
                            //{
                            //    string BillNo = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select vchbillno from tabposbillgeneration where tableid='" + tab.fromtableid + "'").ToString();
                            //    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabposbillgeneration set tableid=" + tab.totableid + " where tableid=" + tab.fromtableid + " and vchbillno='" + BillNo + "' ;");

                            //}
                        }

                        trans.Commit();
                        Count = Count + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "SaveTableandCovers");
            }
            finally
            {
                //if (con.State == ConnectionState.Open)
                //{
                con.Close();
                con.Dispose();
                con.ClearPool();
                trans.Dispose();
                //}

            }
            return Count;
        }
        #endregion

        #region MERGE TABLE

        //public DataTable GetTables(string TableId)
        //{
        //    DataTable dt = new DataTable();
        //    string strData = string.Empty;
        //    try
        //    {

        //        strData = "select tableid as value ,tablesname as text from tabpostablesandcoversmst where tableid<> " + TableId + " and status in ('R','A');";
        //        dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
        //    }
        //    catch (Exception ex)
        //    {
        //        //EventLogger.WriteToErrorLog(ex, "Designation");
        //    }

        //    return dt;
        //}
        public DataTable GetMergedTables(string TableId)
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "SELECT DISTINCT TOTABLEID::INT as value,TC.tablesname as text FROM TABPOSMERGETABLE TM JOIN TABPOSTABLESANDCOVERSMST TC ON TM.TOTABLEID::INT=TC.TABLEID  WHERE FROMTABLEID='" + TableId + "' AND TC.STATUS='M'";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public DataTable GetkotNo(string ID)
        {

            string[] Id = ID.Split(',');


            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {


                strData = "select kt.tableid||'-'|| kt.vchkotid as kotId,kt.vchkotid from tabposkot kt join   tabposkotdetails   ktd on kt.kotid=ktd.kotid  where  tableid in(" + ID + ") and vchitemstatus='N' and kt.vchkotid not in (select vchkotid from tabposkotcancel) ";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public List<MergingTables> GetTables(string TableId, int Userid)
        {
            int GettableId = 0;
            List<MergingTables> lstKOTTakingDTO = new List<MergingTables>();
            string strData = string.Empty;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }



                using (con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();

                    string strgettableid = "select tableid from tabpostablesandcoversmst where tablesname='" + TableId + "'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(strgettableid, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            KOTTakingDTO objKot = new KOTTakingDTO();

                            GettableId = Convert.ToInt32(npgdr["tableid"].ToString());


                        }
                    }



                    if (GettableId != 0)
                    {

                        string strSectionid = "SELECT SECTIONID from TABPOSTABLESANDCOVERSMST WHERE tableid=" + GettableId + ";";
                        int SectionId = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strSectionid));

                        strData = "select tpc.tableid||'-'||kots as value ,tablesname as text from tabpostablesandcoversmst tpc  join(select tableid,array_to_string(array_agg(distinct vchkotid),',')as kots from tabrmskot where vchkotid in(select vchkotid from tabrmskotdetails where vchitemstatus='N' ) group by  tableid )x on   tpc.tableid=x.tableid   where tpc.tableid<> " + GettableId + " and status in ('R') and tpc.tableid not in (select tableid from tabposbillgeneration where  tableid not in (select tableid from tabposbillsettlement ))  order by tpc.tableid;";

                        //string strSectionid = "SELECT SECTIONID from TABPOSTABLESANDCOVERSMST WHERE tableid=" + TableId + ";";
                        //int SectionId = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strSectionid));

                        //strData = "select tpc.tableid||'-'||kots as value ,tablesname as text from tabpostablesandcoversmst tpc  join(select tableid,array_to_string(array_agg(distinct vchkotid),',')as kots from tabrmskot where vchkotid in(select vchkotid from tabrmskotdetails where vchitemstatus='N' and createdby=" + Userid + ") group by  tableid )x on   tpc.tableid=x.tableid   where tpc.tableid<> " + TableId + " and status in ('R') and tpc.tableid not in (select tableid from tabposbillgeneration where  tableid not in (select tableid from tabposbillsettlement ))  order by tpc.tableid;";
                        npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strData);
                        while (npgdr.Read())
                        {
                            MergingTables objKOTTakingDTO = new MergingTables();
                            objKOTTakingDTO.label = Convert.ToString(npgdr["text"]);
                            objKOTTakingDTO.id = Convert.ToString(npgdr["value"]);

                            lstKOTTakingDTO.Add(objKOTTakingDTO);
                        }
                    }
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

        public bool SaveMerge(string ID, List<Mergetables> myDeserializedObjList)
        {
            bool isSaved = false;
            int GettableId = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                using (con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();

                    string strgettableid = "select tableid from tabpostablesandcoversmst where tablesname='" + ID + "'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(strgettableid, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            KOTTakingDTO objKot = new KOTTakingDTO();

                            GettableId = Convert.ToInt32(npgdr["tableid"].ToString());


                        }
                    }



                    if (GettableId != 0)
                    {
                        for (int i = 0; i < myDeserializedObjList.Count; i++)
                        {
                            string strQuery = "INSERT INTO tabposmergetable(fromtableid, totableid, kotid, statusid, createdby, createddate,tablestatus)   VALUES (" + GettableId + "," + myDeserializedObjList[i].id + ",'" + myDeserializedObjList[i].Kotid + "',1," + myDeserializedObjList[0].createdby + ",current_timestamp,'MG');";
                            // string strQuery = "INSERT INTO tabposmergetable(fromtableid, totableid, kotid, statusid, createdby, createddate,tablestatus)   VALUES (" + ID + "," + myDeserializedObjList[i].id + ",'" + myDeserializedObjList[i].Kotid + "',1," + myDeserializedObjList[0].createdby + ",current_timestamp,'MG');";
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strQuery);

                            string updateStatus = "update tabpostablesandcoversmst set status='M',modifiedby=" + myDeserializedObjList[0].createdby + ",modifieddate=current_timestamp  where tableid='" + myDeserializedObjList[i].id + "'";
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, updateStatus);
                            string updateStatusINrmskot = "update tabrmskot set tableid=" + GettableId + ",modifiedby=" + myDeserializedObjList[0].createdby + ",modifieddate=current_timestamp  where tableid='" + myDeserializedObjList[i].id + "'";
                            // string updateStatusINrmskot = "update tabrmskot set tableid=" + ID + ",modifiedby=" + myDeserializedObjList[0].createdby + ",modifieddate=current_timestamp  where tableid='" + myDeserializedObjList[i].id + "'";
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, updateStatusINrmskot);
                        }
                    }
                }
                isSaved = true;
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                isSaved = false;
                throw ex;
            }
            finally
            {
                CloseCon(con);

            }
            return isSaved;

        }



        #endregion

        #region KOT REPRINT

        public List<KOTTakingDTO> ShowReprintKot(string tablid)
        {
            List<KOTTakingDTO> lstKot = new List<KOTTakingDTO>();
            try
            {
                string strInsert = "select vchkotid,datkotdate from (select vchkotid,kotid,tableid,datkotdate from tabposkot where    vchkotid not in(select vchkotid from tabposkotcancel) union select vchkotid,kotid,tableid,datkotdate from tabposkot where   vchkotid not in(select distinct vchkotid from tabposbillgeneration tg join  tabposbillgenerationdetails tgd on tg.recordid=tgd.detailsid ))x where  x.tableid=" + tablid + " order by kotid";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                while (npgdr.Read())
                {
                    KOTTakingDTO objkot = new KOTTakingDTO();
                    objkot.vchKotId = npgdr["vchkotid"].ToString();
                    objkot.KotDate = npgdr["datkotdate"].ToString();
                    lstKot.Add(objkot);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Kot Reprint");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstKot;

        }


        public bool SaveKotReprint(KOTTakingDTO objKotReprint)
        {
            bool isSaved = false;
            string strBillEdit = string.Empty;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                strBillEdit = "INSERT INTO TABPOSKOTPRINT(VCHKOTID,DATKOTDATE,VCHSTATUS) VALUES('" + objKotReprint.vchKotId + "',current_date,'N');";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strBillEdit);


                trans.Commit();
                isSaved = true;

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "KotReprint");
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                    trans.Dispose();
                }
            }
            return isSaved;
        }


        #endregion

        #region TRANSACTION...

        #region BILL EDIT

        public List<BillEditDTO> ShowReasons()
        {
            List<BillEditDTO> lstReasons = new List<BillEditDTO>();

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    string strInsert = "SELECT REASONID,REASONNAME FROM TABPOSREASONMST WHERE STATUSID=1 ORDER BY REASONNAME;";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            BillEditDTO objReasons = new BillEditDTO();
                            objReasons.ReasonId = Convert.ToInt32(npgdr["REASONID"]);
                            objReasons.ReasonName = npgdr["REASONNAME"].ToString();
                            lstReasons.Add(objReasons);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillEdit");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstReasons;

        }

        public List<BillEditDTO> ShowVouchers(string Billid)
        {
            List<BillEditDTO> lstVouchers = new List<BillEditDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    //string strInsert = "SELECT VOUCHERID,VOUCHERNAME FROM TABPOSVOUCHERMST WHERE STATUSID=1 ORDER BY VOUCHERNAME;";
                    string strInsert = "SELECT VOUCHERID,VOUCHERNAME FROM TABPOSVOUCHERMST WHERE  datvalidupto >= current_date and  STATUSID=1 and vchvoucherstatus='I' union all SELECT VOUCHERID,VOUCHERNAME FROM TABPOSVOUCHERMST tv join tabposbillgenerationvoucherdetails tvd on tv.vouchername=tvd.vchvoucherno WHERE  VCHBILLNO='" + Billid + "' ORDER BY VOUCHERNAME";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            BillEditDTO objVouchers = new BillEditDTO();
                            objVouchers.VoucherId = Convert.ToInt32(npgdr["VOUCHERID"]);
                            objVouchers.VoucherName = npgdr["VOUCHERNAME"].ToString();
                            lstVouchers.Add(objVouchers);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillEdit");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstVouchers;

        }

        public BillEditDTO ShowVoucherDetails(string voucherid)
        {
            BillEditDTO objVoucherDetails = new BillEditDTO();

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    string strInsert = "SELECT NUMPERCENTAGE,NUMAMOUNT FROM TABPOSVOUCHERMST WHERE VOUCHERID='" + voucherid + "' AND STATUSID=1 ORDER BY VOUCHERNAME;";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {

                            objVoucherDetails.VDiscountPercentage = Convert.ToDecimal(npgdr["NUMPERCENTAGE"]);
                            objVoucherDetails.VDiscountAmount = Convert.ToDecimal(npgdr["NUMAMOUNT"]);

                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillEdit");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return objVoucherDetails;

        }

        public List<BillEditDTO> ShowBillNosBillEdit()
        {
            List<BillEditDTO> lstBillNos = new List<BillEditDTO>();

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    string strInsert = "SELECT RECORDID,VCHBILLNO FROM TABPOSBILLGENERATION WHERE VCHBILLNO NOT IN(SELECT VCHBILLNO FROM TABPOSBILLSETTLEMENT) and VCHBILLNO !~* 'HB*|.*TB*' ORDER BY VCHBILLNO";
                    //string strInsert = "SELECT RECORDID,VCHBILLNO FROM TABPOSBILLGENERATION WHERE VCHBILLNO NOT IN(SELECT VCHBILLNO FROM TABPOSBILLSETTLEMENT) ORDER BY VCHBILLNO;";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            BillEditDTO objBillNos = new BillEditDTO();
                            objBillNos.RecordId = Convert.ToInt32(npgdr["RECORDID"]);
                            objBillNos.BillNo = npgdr["VCHBILLNO"].ToString();
                            lstBillNos.Add(objBillNos);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillEdit");
                throw ex;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstBillNos;

        }

        public List<BillEditDTO> ShowBillEditServicetax()
        {
            List<BillEditDTO> lstBillGeneration = new List<BillEditDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    string strInsert = "select numpercentage from tabpostaxmst where taxname='SERVICE TAX';";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            BillEditDTO objBillGeneration = new BillEditDTO();

                            objBillGeneration.ServiceTax = Convert.ToDecimal(npgdr["numpercentage"]);

                            lstBillGeneration.Add(objBillGeneration);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillGeneration");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstBillGeneration;

        }

        public List<BillEditDTO> ShowBillEditServiceCharge()
        {
            List<BillEditDTO> lstBillGeneration = new List<BillEditDTO>();

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    string strInsert = "select numpercentage from tabpostaxmst where taxname='SERVICE CHARGE';";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            BillEditDTO objBillGeneration = new BillEditDTO();

                            objBillGeneration.ServiceCharges = Convert.ToDecimal(npgdr["numpercentage"]);

                            lstBillGeneration.Add(objBillGeneration);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillGeneration");
                throw ex;

            }
            finally
            {

                npgdr.Dispose();
            }
            return lstBillGeneration;

        }

        public List<BillEditDTO> ShowBillDetails(string Billid)
        {
            List<BillEditDTO> lstBillDetails = new List<BillEditDTO>();
            string strInsert = string.Empty;

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    //if (Billid != null && Billid != "")
                    //{
                    //strInsert = "SELECT NUMTOTALAMOUNT,NUMDISCOUNT,NUMVOUCHERDISCOUNT,NUMGROSS,NUMSERVICETAX,NUMSERVICECHARGES,NUMNET,KOTID,VCHKOTID,ITEMID,ITEMNAME,ITEMCODE,ITEMQTY,ITEMRATE,ITEMAMOUNT,VCHITEMCHARGABLEORNOT,VCHDISCOUNTTYPE,VCHDISCOUNT,REASON,VCHVOUCHERNO,NUMVOUCHERDISCOUNTPERCENTAGE,NUMVOUCHERDISCOUNTAMOUNT FROM VWBILLDETAILS WHERE RECORDID='" + Billid + "';";
                    strInsert = "SELECT numServiceTax15OnServiceCharge,VatTax,VatTaxvalue,IndiviItemQty ,IndDiscountType,TABLEID,TABLESNAME,VCHBILLNO,NUMTOTALAMOUNT,NUMDISCOUNT,NUMVOUCHERDISCOUNT,NUMGROSS,NUMSERVICETAX,COALESCE(NUMSERVICETAXVALE,0) AS NUMSERVICETAXVALE,NUMSERVICECHARGES,COALESCE(NUMSERVICECHARGEVALE,0) AS NUMSERVICECHARGEVALE,NUMNET,VCHAUTHORISEDBY,KOTID,VCHKOTID,ITEMID,ITEMNAME,ITEMCODE,ITEMQTY,ITEMRATE,ITEMAMOUNT,VCHITEMCHARGABLEORNOT,NONCHARGABLEREASON,VCHDISCOUNTTYPE,COALESCE(VCHDISCOUNT,0) as VCHDISCOUNT,REASON,VCHVOUCHERNO,COALESCE(NUMVOUCHERDISCOUNTPERCENTAGE,0) as NUMVOUCHERDISCOUNTPERCENTAGE,COALESCE(NUMVOUCHERDISCOUNTAMOUNT,0)as NUMVOUCHERDISCOUNTAMOUNT FROM VWBILLEDITDETAILS WHERE VCHBILLNO='" + Billid + "' ;";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {

                        npgdr = cmd.ExecuteReader();

                        while (npgdr.Read())
                        {
                            BillEditDTO objBillDetails = new BillEditDTO();


                            objBillDetails.IndiviItemQty = Convert.ToDecimal(npgdr["IndiviItemQty"].ToString());
                            objBillDetails.IndDiscountType = npgdr["IndDiscountType"].ToString();
                            objBillDetails.TableId = npgdr["TABLEID"].ToString();
                            objBillDetails.TableName = npgdr["TABLESNAME"].ToString();
                            objBillDetails.TotalAmount = Convert.ToDecimal(npgdr["NUMTOTALAMOUNT"]);
                            objBillDetails.TotalDiscount = Convert.ToDecimal(npgdr["NUMDISCOUNT"]);
                            objBillDetails.TotalVoucherDiscount = Convert.ToDecimal(npgdr["NUMVOUCHERDISCOUNT"]);
                            objBillDetails.Gross = Convert.ToDecimal(npgdr["NUMGROSS"]);
                            objBillDetails.ServiceTax = Convert.ToDecimal(npgdr["NUMSERVICETAX"]);
                            objBillDetails.ServiceTaxValue = Convert.ToDecimal(npgdr["NUMSERVICETAXVALE"]);
                            objBillDetails.ServiceCharges = Convert.ToDecimal(npgdr["NUMSERVICECHARGES"]);
                            objBillDetails.ServiceChargesValue = Convert.ToDecimal(npgdr["NUMSERVICECHARGEVALE"]);
                            objBillDetails.NetAmount = Convert.ToDecimal(npgdr["NUMNET"]);
                            objBillDetails.AuthorisedBy = npgdr["VCHAUTHORISEDBY"].ToString();
                            objBillDetails.KotId = npgdr["KOTID"].ToString();
                            objBillDetails.VchKotId = npgdr["VCHKOTID"].ToString();
                            objBillDetails.ItemId = npgdr["ITEMID"].ToString();
                            objBillDetails.ItemName = npgdr["ITEMNAME"].ToString();
                            objBillDetails.ItemCode = npgdr["ITEMCODE"].ToString();
                            objBillDetails.ItemQty = Convert.ToDecimal(npgdr["ITEMQTY"]);
                            objBillDetails.ItemRate = Convert.ToDecimal(npgdr["ITEMRATE"]);
                            objBillDetails.Amount = Convert.ToDecimal(npgdr["ITEMAMOUNT"]);
                            objBillDetails.NonchargableReason = npgdr["NONCHARGABLEREASON"].ToString();
                            objBillDetails.ItemChangableornot = npgdr["VCHITEMCHARGABLEORNOT"].ToString();
                            objBillDetails.Discounttype = npgdr["VCHDISCOUNTTYPE"].ToString();
                            objBillDetails.Dicount = Convert.ToDecimal(npgdr["VCHDISCOUNT"]);
                            objBillDetails.ReasonName = npgdr["REASON"].ToString();
                            objBillDetails.VoucherName = npgdr["VCHVOUCHERNO"].ToString();
                            objBillDetails.VDiscountPercentage = Convert.ToDecimal(npgdr["NUMVOUCHERDISCOUNTPERCENTAGE"]);
                            objBillDetails.VDiscountAmount = Convert.ToDecimal(npgdr["NUMVOUCHERDISCOUNTAMOUNT"]);

                            objBillDetails.VatTax = (npgdr["VatTax"]).ToString();
                            objBillDetails.VAtaxvalue = (npgdr["VatTaxvalue"]).ToString();
                            objBillDetails.ServiceTaxOnServiceCharge = (npgdr["numServiceTax15OnServiceCharge"]).ToString();

                            lstBillDetails.Add(objBillDetails);
                        }
                    }
                    con.Close();
                }
                // }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillEdit");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstBillDetails;

        }

        public bool SaveBillEdit(BillEditDTO objBillEdit, List<BillEditDTO> lstKotDetails)
        {
            bool isSaved = false;
            string strBillEdit = string.Empty;
            string strKotDetails = string.Empty;
            string strDiscount = string.Empty;
            string strVoucher = string.Empty;
            string strChargableornot = string.Empty;
            int count = 0;
            string strDelete = string.Empty;

            int strDetailid = 0;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from tabposbilledit where vchbillno='" + objBillEdit.BillNo + "'"));

                if (count > 0)
                {
                    strDelete = "delete from tabposbilleditdetails where vchbillno='" + ManageQuote(objBillEdit.BillNo) + "';delete from tabposbilleditdiscountdetails where vchbillno='" + ManageQuote(objBillEdit.BillNo) + "'; delete from tabposbilleditvoucherdetails where vchbillno='" + ManageQuote(objBillEdit.BillNo) + "';delete from tabposbilledit where vchbillno='" + ManageQuote(objBillEdit.BillNo) + "';";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strDelete);
                }

                strBillEdit = "INSERT INTO tabposbilledit select * from tabposbillgeneration where vchbillno='" + ManageQuote(objBillEdit.BillNo) + "';";
                strDetailid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strBillEdit));

                strKotDetails = "INSERT INTO tabposbilleditdetails select * from tabposbillgenerationdetails where vchbillno='" + ManageQuote(objBillEdit.BillNo) + "';";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strKotDetails);


                int countvocher = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from TABPOSBILLGENERATIONVOUCHERDETAILS where vchbillno='" + objBillEdit.BillNo + "'"));
                int countdiscount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from TABPOSBILLGENERATIONDISCOUNTDETAILS where vchbillno='" + objBillEdit.BillNo + "'"));

                if (countdiscount > 0)
                {
                    strDiscount = "INSERT INTO tabposbilleditdiscountdetails  select * from TABPOSBILLGENERATIONDISCOUNTDETAILS where vchbillno='" + ManageQuote(objBillEdit.BillNo) + "';";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strDiscount);
                }

                if (countvocher > 0)
                {
                    strDiscount = "INSERT INTO tabposbilleditvoucherdetails  select * from TABPOSBILLGENERATIONVOUCHERDETAILS where vchbillno='" + ManageQuote(objBillEdit.BillNo) + "';";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strDiscount);
                }


                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "delete from TABPOSBILLGENERATIONDISCOUNTDETAILS where vchbillno='" + ManageQuote(objBillEdit.BillNo) + "'");
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "delete from TABPOSBILLGENERATIONVOUCHERDETAILS where vchbillno='" + ManageQuote(objBillEdit.BillNo) + "'");
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "delete from tabposbillgenerationdetails where vchbillno='" + ManageQuote(objBillEdit.BillNo) + "'");
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "delete from TABPOSBILLGENERATION where vchbillno='" + ManageQuote(objBillEdit.BillNo) + "'");
                if (objBillEdit.VAtaxvalue == null)
                {
                    objBillEdit.VAtaxvalue = "0";
                }

                strBillEdit = "INSERT INTO TABPOSBILLGENERATION(vchbillno, tableid,datkotdate,kottime,numtotalamount, numdiscount, numvoucherdiscount, numgross, numservicetax,NUMSERVICETAXVALE,numservicecharges,NUMSERVICECHARGEVALE, numnet, vchauthorisedby, statusid, createdby,createddate,numservicetax15onservicecharge,vattax,vattaxvalue) VALUES ('" + ManageQuote(objBillEdit.BillNo) + "','" + ManageQuote(objBillEdit.TableId) + "', current_date, to_char(current_timestamp, 'HH24:MI')," + Convert.ToDecimal(objBillEdit.TotalAmount) + "," + Convert.ToDecimal(objBillEdit.TotalDiscount) + "," + Convert.ToDecimal(objBillEdit.VDiscountAmount) + "," + Convert.ToDecimal(objBillEdit.Gross) + "," + Convert.ToDecimal(objBillEdit.ServiceTax) + "," + Convert.ToDecimal(objBillEdit.ServiceTaxValue) + "," + Convert.ToDecimal(objBillEdit.ServiceCharges) + "," + Convert.ToDecimal(objBillEdit.ServiceChargesValue) + "," + Convert.ToDecimal(objBillEdit.NetAmount) + ",'" + ManageQuote(objBillEdit.AuthorisedBy) + "',1," + objBillEdit.createdby + ",current_timestamp,'" + objBillEdit.ServiceTaxOnServiceCharge + "','" + objBillEdit.VatTax + "','" + objBillEdit.VAtaxvalue + "')RETURNING recordid;";
                strDetailid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strBillEdit));

                for (int i = 0; i < lstKotDetails.Count; i++)
                {

                    strKotDetails = "INSERT INTO TABPOSBILLGENERATIONDETAILS(detailsid, vchbillno, kotid, vchkotid, itemid, itemname,itemcode, itemqty, itemrate,numamount, vchitemchargableornot, reason, statusid,createdby, createddate,vchdiscounttype,vchdiscount) VALUES ('" + strDetailid + "','" + ManageQuote(objBillEdit.BillNo) + "','" + ManageQuote(lstKotDetails[i].KotId) + "','" + ManageQuote(lstKotDetails[i].VchKotId) + "','" + ManageQuote(lstKotDetails[i].ItemId) + "','" + ManageQuote(lstKotDetails[i].ItemName) + "','" + ManageQuote(lstKotDetails[i].ItemCode) + "'," + lstKotDetails[i].ItemQty + "," + lstKotDetails[i].ItemRate + "," + lstKotDetails[i].Amount + ",'" + ManageQuote(lstKotDetails[i].ItemChangableornot) + "','" + ManageQuote(lstKotDetails[i].NonchargableReason) + "',1," + objBillEdit.createdby + ",current_timestamp,'" + lstKotDetails[i].IndDiscountType + "'," + lstKotDetails[i].IndiviItemQty + ");";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strKotDetails);
                }

                if (objBillEdit.Dicount > 0)
                {
                    strDiscount = "INSERT INTO TABPOSBILLGENERATIONDISCOUNTDETAILS(detailsid, vchbillno, vchdiscounttype, vchdiscount,reason, statusid, createdby, createddate) VALUES ('" + strDetailid + "','" + ManageQuote(objBillEdit.BillNo) + "','" + ManageQuote(objBillEdit.Discounttype) + "'," + objBillEdit.Dicount + ",'" + ManageQuote(objBillEdit.ReasonName) + "',1," + objBillEdit.createdby + ",current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strDiscount);
                }

                if (objBillEdit.VDiscountPercentage > 0 && objBillEdit.VDiscountAmount > 0)
                {
                    strDiscount = "INSERT INTO TABPOSBILLGENERATIONVOUCHERDETAILS(detailsid, vchbillno, vchvoucherno,numvoucherdiscountpercentage, numvoucherdiscountamount,statusid, createdby, createddate) VALUES ('" + strDetailid + "','" + ManageQuote(objBillEdit.BillNo) + "','" + ManageQuote(objBillEdit.VoucherName) + "'," + objBillEdit.VDiscountPercentage + "," + objBillEdit.VDiscountAmount + ",1," + objBillEdit.createdby + ",current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strDiscount);
                }


                trans.Commit();
                isSaved = true;

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "BillEdit");
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                    trans.Dispose();
                }

            }

            return isSaved;
        }

        #endregion

        #region BILL-SETTLEMENT...

        public List<BillSettlementDTO> ShowBillNos()
        {
            List<BillSettlementDTO> lstBillSettlement = new List<BillSettlementDTO>();
            try
            {

                string strInsert = "SELECT  vchdeliverycharges,x.numServiceTax15OnServiceCharge,x.VatTax,x.VatTaxvalue,X.vchhomedeliverystatus,X.RECORDID,X.VCHBILLNO,X.SESSIONID,X.DATKOTDATE,X. KOTTIME, TC.TABLESNAME, X.TABLEID,NUMTOTALAMOUNT,case when upper(th.vchdiscounttype)='PER' then( NUMTOTALAMOUNT* x.NUMDISCOUNT)/100 else x.NUMDISCOUNT end as NUMDISCOUNT ,NUMVOUCHERDISCOUNT,NUMGROSS,X.NUMSERVICETAX,X.NUMSERVICECHARGES, NUMNET, VCHAUTHORISEDBY, X.STATUSID,NUMSERVICETAXVALE,NUMSERVICECHARGEVALE FROM TABPOSBILLGENERATIONDETAILS S join TABPOSBILLGENERATION X on s.VCHBILLNO=X.VCHBILLNO JOIN TABPOSTABLESANDCOVERSMST TC ON X.TABLEID=TC.TABLEID left join tabposhdkot th on substring(X.vchhomedeliverystatus,3)=th.vchorderno WHERE X.VCHBILLNO NOT IN (SELECT VCHBILLNO FROM TABPOSBILLSETTLEMENT) and  S.vchkotid not in(select vchorderno from tabposhdorderreturn) group by X.RECORDID,X.VCHBILLNO,X.SESSIONID,X.DATKOTDATE,X. KOTTIME, TC.TABLESNAME, X.TABLEID,NUMTOTALAMOUNT,x.NUMDISCOUNT,NUMVOUCHERDISCOUNT,NUMGROSS,x.NUMSERVICETAX,x.NUMSERVICECHARGES, NUMNET, VCHAUTHORISEDBY, X.STATUSID,NUMSERVICETAXVALE,NUMSERVICECHARGEVALE,th.vchdiscounttype,vchdeliverycharges,x.numServiceTax15OnServiceCharge,x.VatTax,x.VatTaxvalue,X.vchhomedeliverystatus order by vchbillno  ";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                while (npgdr.Read())
                {
                    BillSettlementDTO objBillSettlement = new BillSettlementDTO();
                    objBillSettlement.RecordId = Convert.ToInt32(npgdr["recordid"]);
                    objBillSettlement.BillNo = Convert.ToString(npgdr["VCHBILLNO"]);
                    objBillSettlement.SessionId = Convert.ToString(npgdr["SESSIONID"]);
                    objBillSettlement.KotDate = Convert.ToString(npgdr["DATKOTDATE"]);
                    objBillSettlement.KotTime = Convert.ToString(npgdr["KOTTIME"]);

                    objBillSettlement.TableName = Convert.ToString(npgdr["TABLESNAME"]);
                    objBillSettlement.TableId = Convert.ToString(npgdr["TABLEID"]);
                    objBillSettlement.TotalAmount = Convert.ToDecimal(npgdr["NUMTOTALAMOUNT"]);
                    objBillSettlement.Dicount = Convert.ToDecimal(npgdr["NUMDISCOUNT"]);
                    objBillSettlement.VoucherDiscount = Convert.ToDecimal(npgdr["NUMVOUCHERDISCOUNT"]);

                    objBillSettlement.Gross = Convert.ToDecimal(npgdr["NUMGROSS"]);
                    objBillSettlement.ServiceTax = Convert.ToDecimal(npgdr["NUMSERVICETAX"]);
                    objBillSettlement.ServiceTaxValue = Convert.ToDecimal(npgdr["NUMSERVICETAXVALE"]);
                    objBillSettlement.ServiceCharges = Convert.ToDecimal(npgdr["NUMSERVICECHARGES"]);
                    objBillSettlement.ServiceChargesValue = Convert.ToDecimal(npgdr["NUMSERVICECHARGEVALE"]);
                    objBillSettlement.NetAmount = Convert.ToDecimal(npgdr["NUMNET"]);
                    objBillSettlement.Authorisedby = Convert.ToString(npgdr["VCHAUTHORISEDBY"]);
                    objBillSettlement.Status = Convert.ToString(npgdr["STATUSID"]);
                    objBillSettlement.HDStatus = Convert.ToString(npgdr["vchhomedeliverystatus"]);
                    objBillSettlement.Deliverycharges = Convert.ToString(npgdr["vchdeliverycharges"]);
                    if (Convert.ToString(npgdr["VatTax"]) == "") { objBillSettlement.VatTax = "0"; }
                    else { objBillSettlement.VatTax = Convert.ToString(npgdr["VatTax"]); }
                    if (Convert.ToString(npgdr["VatTax"]) == "") { objBillSettlement.VatTaxvalue = "0"; }
                    else
                    {
                        objBillSettlement.VatTaxvalue = Convert.ToString(npgdr["VatTaxvalue"]);
                    }
                    if (Convert.ToString(npgdr["numServiceTax15OnServiceCharge"]) == "") { objBillSettlement.ServiceTaxOnServiceCharge = "0"; }
                    else
                    {
                        objBillSettlement.ServiceTaxOnServiceCharge = Convert.ToString(npgdr["numServiceTax15OnServiceCharge"]);
                    }
                    lstBillSettlement.Add(objBillSettlement);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowBillNos");
                throw ex;
            }
            return lstBillSettlement;

        }
        public bool SaveBillSettlement(BillSettlementDTO BillSettlement, CashDetails CashDetails, List<CardDetails> lstCardDetails)
        {
            bool IsValid = false;
            Int32 DetailsId = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                decimal cardfullAmount = 0;
                if (lstCardDetails != null)
                {
                    foreach (var CardDetails in lstCardDetails)
                    {

                        cardfullAmount += (CardDetails.CardAmount);

                    }
                }
                if (BillSettlement.Deliverycharges == null || BillSettlement.Deliverycharges == "")
                {
                    BillSettlement.Deliverycharges = "0.00";

                }
                string billstatus = "";
                if (BillSettlement.BilltoCompany == "True")
                {

                    BillSettlement.PaidAmount = 0;
                    billstatus = "BC";
                }
                else
                {

                    billstatus = "B";
                }
                DetailsId = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "INSERT INTO tabposbillsettlement(vchbillno,  datdate, billtime, tableid, numtotalrate,numdiscount, numvoucherdiscount, numgross, numservicetax, numservicetaxpercentage,numservicecharges, numservicechargespercentage, numnet, numcashpaid,numcardpaid, numpaidamount,numbalanceamount,statusid,createdby,createddate,ordercancelstatus,numservicetax15onservicecharge,vattax,vattaxvalue,vchbillstatustoknow,vchcompanyname,vchdeliverycharges)  VALUES ('" + ManageQuote(BillSettlement.BillNo) + "',  current_date, to_char(current_timestamp, 'HH24:MI')," + BillSettlement.TableId + ", '" + (BillSettlement.TotalAmount) + "', '" + (BillSettlement.Dicount) + "', '" + (BillSettlement.VoucherDiscount) + "',  '" + (BillSettlement.Gross) + "', '" + (BillSettlement.ServiceTaxValue) + "','" + (BillSettlement.ServiceTax) + "', '" + (BillSettlement.ServiceChargesValue) + "', '" + (BillSettlement.ServiceCharges) + "', '" + (BillSettlement.NetAmount) + "', '" + (BillSettlement.CashAmount) + "', '" + (cardfullAmount) + "', '" + (BillSettlement.PaidAmount) + "', '" + (BillSettlement.BalanceAmount) + "', 1," + BillSettlement.createdby + ",current_timestamp, '" + (BillSettlement.ReasonForOrdercancel) + "'," + (BillSettlement.ServiceTaxOnServiceCharge) + "," + (BillSettlement.VatTax) + "," + (BillSettlement.VatTaxvalue) + ",'" + billstatus + "','" + BillSettlement.CompanyName + "'," + BillSettlement.Deliverycharges + ")RETURNING recordid"));


                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "INSERT INTO TABPOSBILLSETTLEDISCOUNTDETAILS( DETAILSID, VCHBILLNO, VCHDISCOUNTTYPE, VCHDISCOUNT,REASON, STATUSID, CREATEDBY, CREATEDDATE)    ( select  " + DetailsId + ",VCHBILLNO, VCHDISCOUNTTYPE, VCHDISCOUNT,REASON, 1," + BillSettlement.createdby + ",current_timestamp from ( SELECT VCHBILLNO, VCHDISCOUNTTYPE, VCHDISCOUNT,REASON FROM TABPOSBILLEDITDISCOUNTDETAILS  UNION  SELECT VCHBILLNO, VCHDISCOUNTTYPE, VCHDISCOUNT,REASON FROM TABPOSBILLGENERATIONDISCOUNTDETAILS WHERE VCHBILLNO NOT IN (SELECT VCHBILLNO FROM TABPOSBILLEDITDISCOUNTDETAILS) )x where VCHBILLNO='" + ManageQuote(BillSettlement.BillNo) + "') ");


                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "INSERT INTO TABPOSBILLSETTLEVOUCHERDETAILS( DETAILSID,VCHBILLNO,VCHVOUCHERTYPE,VCHVOUCHERNO,NUMVOUCHERDISCOUNTPERCENTAGE, NUMVOUCHERDISCOUNTAMOUNT, REASON, STATUSID, CREATEDBY, CREATEDDATE) ( select  " + DetailsId + ",VCHBILLNO,VCHVOUCHERTYPE, VCHVOUCHERNO,NUMVOUCHERDISCOUNTPERCENTAGE, NUMVOUCHERDISCOUNTAMOUNT,REASON, 1," + BillSettlement.createdby + ",current_timestamp from ( SELECT VCHBILLNO,VCHVOUCHERTYPE, VCHVOUCHERNO,NUMVOUCHERDISCOUNTPERCENTAGE, NUMVOUCHERDISCOUNTAMOUNT,REASON FROM TABPOSBILLEDITVOUCHERDETAILS  UNION  SELECT VCHBILLNO,VCHVOUCHERTYPE, VCHVOUCHERNO,NUMVOUCHERDISCOUNTPERCENTAGE, NUMVOUCHERDISCOUNTAMOUNT,REASON FROM TABPOSBILLGENERATIONVOUCHERDETAILS WHERE VCHBILLNO NOT IN (SELECT VCHBILLNO FROM TABPOSBILLEDITVOUCHERDETAILS) )x where VCHBILLNO='" + ManageQuote(BillSettlement.BillNo) + "') ");

                if (lstCardDetails != null)
                {
                    foreach (var CardDetails in lstCardDetails)
                    {

                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "INSERT INTO TABPOSBILLSETTLECARDDETAILS( DETAILSID,VCHBILLNO,VCHCARDNO,VCHCARDHOLDERNAME,VCHTYPEOFCARD,VCHCARDEXPDATE,NUMAMOUNT,STATUSID,CREATEDBY,CREATEDDATE,vchTransactionno) VALUES ('" + DetailsId + "', '" + ManageQuote(BillSettlement.BillNo) + "', '" + (CardDetails.CardNo) + "', '" + ManageQuote(CardDetails.CardName) + "', '" + ManageQuote(CardDetails.CardType) + "',  '" + ManageQuote(CardDetails.ExpireDate) + "', " + CardDetails.CardAmount + ", 1," + BillSettlement.createdby + ",current_timestamp, '" + ManageQuote(CardDetails.TransactionNo) + "');");
                    }
                }

                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "INSERT INTO TABPOSBILLSETTLECASHDETAILS(DETAILSID, VCHBILLNO, NUM1000, NUM500, NUM100, NUM50,NUM20, NUM10, NUM5, NUM2, NUM1, NUM50PAISA, NUMAMOUNT,statusid,createdby,createddate ) VALUES ('" + DetailsId + "', '" + ManageQuote(BillSettlement.BillNo) + "', " + CashDetails.Thousand + ", " + CashDetails.fiveHundred + ", " + CashDetails.Hundred + ", " + CashDetails.Fifty + ", " + CashDetails.Twenty + ", " + CashDetails.Ten + ", " + CashDetails.Five + ", " + CashDetails.Two + ", " + CashDetails.One + ", " + CashDetails.Half + ", " + CashDetails.CashAmount + ",1," + BillSettlement.createdby + ",current_timestamp);");
                if (BillSettlement.HDStatus != null)
                {
                    if (BillSettlement.HDStatus.Split('-')[0] == "H")
                    {
                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "update tabposhdkot set vchstatus='ORP' where vchorderno='" + BillSettlement.HDStatus.Split('-')[1] + "';");
                    }
                    else if (BillSettlement.HDStatus.Split('-')[0] == "T")
                    {
                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "update tabposhdkot set vchstatus='TKP' where vchorderno='" + BillSettlement.HDStatus.Split('-')[1] + "';");
                    }
                }
                string status = (NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select status from tabpostablesandcoversmst  where tableid=" + BillSettlement.TableId + "").ToString());

                string S = "R".ToString();
                if (status != S)
                {
                    int kotCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "(select count(vchbillno) from tabposbillgeneration  where tableid=" + BillSettlement.TableId + " and vchbillno not in (select vchbillno from tabposbillsettlement where  tableid=" + BillSettlement.TableId + "))"));

                    if (kotCount == 0)
                    {
                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "update tabpostablesandcoversmst set status ='A',modifiedby=" + BillSettlement.createdby + ",modifieddate=current_timestamp  where  tableid in(select tableid from tabpostablesandcoversmst where tableid=" + BillSettlement.TableId + " union select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + BillSettlement.TableId + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + BillSettlement.TableId + " union select totableid::int from tabposmergetable where fromtableid in (select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + BillSettlement.TableId + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + BillSettlement.TableId + ") and totableid::int in(select tableid from tabpostablesandcoversmst where status='M') union  select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid in( select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + BillSettlement.TableId + ") order by tableid) ;");
                    }
                }


                trans.Commit();
                string InsertFuntion = "SELECT fntabbillsettlement('" + ManageQuote(BillSettlement.BillNo) + "')";
                NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, InsertFuntion);


                IsValid = true;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveKOTChangeDetails");
                IsValid = false;
                trans.Rollback();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                    trans.Dispose();
                }
            }

            return IsValid;
        }


        public string getStusForBillsPending(string TableId)
        {

            string Bills = "";
            try
            {



                // string strData = "select array_to_string(array_agg(vchItemstatus), ', ') from tabposkotdetails tkd join tabposkot tk on tkd.vchkotid=tk.vchkotid where tableid= " + TableId + "";
                //string strData = "select array_to_string(array_agg(vchItemstatus), ', ') from tabposkotdetails tkd join tabposkot tk on tkd.vchkotid=tk.vchkotid where tableid=" + TableId + " or tableid in(select cast (totableid as bigint) from tabposmergetable where fromtableid=" + TableId + ")";
                //string strData = "select array_to_string(array_agg(vchItemstatus), ', ') from tabposkotdetails tkd join tabposkot tk on tkd.vchkotid=tk.vchkotid where  tk.tableid in(select " + TableId + " union select cast (totableid as bigint) from tabposmergetable where fromtableid=" + TableId + ") and tk.vchkotid not in(select vchkotid from  tabposkotcancel)";

                string strData = "select array_to_string(array_agg(vchItemstatus), ', ') from tabrmskotdetails tkd join tabrmskot tk on tkd.vchkotid=tk.vchkotid where  tk.tableid in(select " + TableId + " union select cast (totableid as bigint) from tabposmergetable where fromtableid=" + TableId + ") ";
                Bills = NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strData).ToString(); ;
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return Bills;
        }

        public bool SaveCompanyDetails(BillSettlementDTO C)
        {
            bool IsValid = false;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "INSERT INTO tabposbilltocompanycompanydetails(vchcontactpersonname, vchmobileno, vchcompanyname,vchphoneno, vchaddress, statusid, createdby, createddate)    VALUES ('" + ManageQuote(C.ContactPersonName) + "','" + ManageQuote(C.MobileNo) + "','" + ManageQuote(C.CompanyName) + "','" + ManageQuote(C.PhoneNo) + "','" + (C.Address) + "'," + C.statusid + "," + C.createdby + ",current_timestamp);");

                trans.Commit();



                IsValid = true;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveCompanyDetails");
                IsValid = false;
                trans.Rollback();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                    trans.Dispose();
                }
            }

            return IsValid;
        }

        public DataTable getComapnynamesOfBilltocomapny()
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "select vchcompanyname from tabposbilltocompanycompanydetails";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }


        #endregion

        #region ISSUE VOUCHER...
        public List<CutomerMaster> ShowCutomerNames()
        {
            List<CutomerMaster> lstCutomers = new List<CutomerMaster>();

            try
            {

                string strInsert = "select vchcustomername,vchcustid,vchcustomercontactno from tabposcustomers ORDER BY vchcustomername;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                while (npgdr.Read())
                {
                    CutomerMaster objCutomer = new CutomerMaster();
                    objCutomer.CustomerName = npgdr["vchcustomername"].ToString();
                    objCutomer.CustomerId = npgdr["vchcustid"].ToString();
                    objCutomer.CustomerContact = npgdr["vchcustomercontactno"].ToString();
                    lstCutomers.Add(objCutomer);
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowCutomerNames");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstCutomers;
        }
        public List<UserMaster> ShowUserNames()
        {
            List<UserMaster> lstUsers = new List<UserMaster>();
            try
            {
                string strInsert = "select recordid,vchname from tabhrmsemployeepersonaldetails ORDER BY vchname;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                while (npgdr.Read())
                {
                    UserMaster objUser = new UserMaster();
                    objUser.UserName = npgdr["vchname"].ToString();
                    objUser.UserId = npgdr["recordid"].ToString();
                    lstUsers.Add(objUser);
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowUserNames");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstUsers;
        }
        public List<Vouchertype> ShowVoucherTypes()
        {
            List<Vouchertype> lstVoucher = new List<Vouchertype>();

            try
            {
                string strInsert = "select vouchertypename,chargeapplicable,vouchertypeid from tabposvouchertypemst where statusid=1 order by vouchertypename;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                while (npgdr.Read())
                {
                    Vouchertype objVoucher = new Vouchertype();
                    objVoucher.VoucherTypeName = npgdr["vouchertypename"].ToString();
                    objVoucher.VoucherTypeId = npgdr["vouchertypeid"].ToString();
                    objVoucher.ChargeApplicable = npgdr["chargeapplicable"].ToString();
                    lstVoucher.Add(objVoucher);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowVoucher");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstVoucher;
        }
        public List<Vouchertype> ShowVouchers(string VouchName, string VouchID)
        {
            List<Vouchertype> lstVoucher = new List<Vouchertype>();
            try
            {
                if (VouchName != null)
                {
                    string strInsert = "select distinct vouchername,vouchercode,voucherid,description,numamount from tabposvouchermst where vouchertype='" + VouchName + "' and  statusid=1  and vchvoucherstatus='N' order by vouchername;";
                    //strInsert = "select distinct vouchername,vouchercode,voucherid,description, case when numpercentage=0 then  numamount::text else numpercentage::text  end as numamount,case when numpercentage=0 then  'AMOUNT' else 'PERCENTAGE' end as Category from tabposvouchermst where vouchertype='" + VouchName + "' order by vouchername;";
                    npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    while (npgdr.Read())
                    {
                        Vouchertype objVoucher = new Vouchertype();
                        objVoucher.VoucherTypeId = npgdr["voucherid"].ToString();
                        objVoucher.VoucherTypeName = npgdr["vouchername"].ToString();
                        objVoucher.VoucherDescription = npgdr["description"].ToString();
                        objVoucher.VoucherValue = npgdr["numamount"].ToString();
                        lstVoucher.Add(objVoucher);
                    }
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowVoucher");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstVoucher;
        }
        //public bool SaveIssueVoucher(IssueVoucher ISV)
        //{
        //    bool IsValid = false;
        //    try
        //    {
        //        con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        //        if (con.State != ConnectionState.Open)
        //        {
        //            con.Open();
        //        }
        //        trans = con.BeginTransaction();
        //        int Custlst;
        //        string CustID = string.Empty;
        //        string strInsert = "";
        //        string Voucherid;
        //        string strcust = "select count(*) from tabposcustomers  where vchcustomername='" + ISV.CustomerName + "' and vchcustomercontactno='" + ISV.ContactNO + "';";
        //        int count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strcust));
        //        if (count == 0)
        //        {
        //            strcust = "";
        //            strcust = "select coalesce(max(customerid),0) from tabposcustomers;";
        //            Custlst = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strcust));
        //            strInsert = "insert into tabposcustomers(vchcustid,vchcustomername,vchcustomercontactno) values ('C" + Custlst + "','" + ISV.CustomerName + "','" + ISV.ContactNO + "') returning customerid;";
        //            CustID = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strInsert).ToString();
        //        }
        //        else
        //        {
        //            CustID = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select customerid from tabposcustomers  where vchcustomername='" + ISV.CustomerName + "' and vchcustomercontactno='" + ISV.ContactNO + "';").ToString();
        //        }
        //        Voucherid = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select coalesce(max(voucherid),0) from tabposissuevoucher;").ToString();
        //        strInsert = "INSERT INTO tabposissuevoucher(vchvouchercode,numvouchertype,numvoucherno,issuedby,authorizedby,vchremarks,numcustomerid,vchcustomername,numvouchervalue,chargeapplicable,numnetpayable,numvouchertaxtotal,createddate,createdby) ";
        //        strInsert += " VALUES('ISV" + Voucherid + "','" + ISV.VoucherType + "','" + ISV.VoucherNO + "','" + ISV.IssuedBy + "','" + ISV.AuthorizePerson + "','" + ISV.Remarks + "','" + CustID + "','" + ISV.CustomerName + "','" + ISV.VoucherValue + "','" + ISV.Charge + "','" + ISV.NetPayable + "'," + ISV.NetPayable + "-" + ISV.VoucherValue + ",Current_timestamp," + ISV.IssuedBy + ") returning voucherid;";
        //        string voucherid = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strInsert).ToString();
        //        if (ISV.ServiceTax != "0" || ISV.ServiceCharge != "0" || ISV.Vat != "0")
        //        {
        //            strInsert = "insert into tabposissuevouchertaxdetails(detailsid,numvouchervalue,numvatpercentage,numvat,numservicetaxpercentage,numservicetax,numservicechargepercentage,numservicecharge,numnetpayable,createdby,createddate) values ('" + voucherid + "'," + ISV.VoucherValue + "," + ISV.Vat + ",(" + ISV.VoucherValue + "*" + ISV.Vat + "::decimal)/100," + ISV.ServiceTax + ",(" + ISV.VoucherValue + "*" + ISV.ServiceTax + "::decimal)/100," + ISV.ServiceCharge + ",(" + ISV.VoucherValue + "*" + ISV.ServiceCharge + "::decimal)/100," + ISV.NetPayable + "," + ISV.IssuedBy + ",Current_timestamp);";
        //            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
        //        }
        //        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabposvouchermst set vchvoucherstatus='I',modifiedby=" + ISV.IssuedBy + ",modifieddate=current_timestamp  where voucherid=" + ISV.VoucherNO + " and vouchertype='" + ISV.VoucherType + "';");
        //        if (ISV.Charge != null)
        //        {
        //            strInsert = string.Empty;
        //            if (ISV.PaymentType == "Cash")
        //            {
        //                strInsert = "insert into tabposissuevouchercashdetails(detailsid, numnetpayable, numnoofthousands, numnooffivehundreds, numnoofhundreds, numnooffiftys, numnooftwentys, numnooftens,numnooffives, numnooftwos, numnoofones, numnooffiftypaisa, createdby, createddate) values (" + voucherid + "," + ISV.NetPayable + "," + ISV.Thousand + "," + ISV.FiveHundred + "," + ISV.Hundred + "," + ISV.Fifty + "," + ISV.Twenty + "," + ISV.Ten + "," + ISV.Five + "," + ISV.Two + "," + ISV.One + "," + ISV.FiftyPaisa + "," + ISV.IssuedBy + ",Current_timestamp);";
        //            }
        //            else if (ISV.PaymentType == "Card")
        //            {
        //                strInsert = "insert into tabposissuevouchercarddetails(detailsid,numnetpayable,vchcardnumber,vchcardtype,vchnameoncard,vchcardexpdate,createdby,createddate) values(" + voucherid + "," + ISV.NetPayable + ",'" + ISV.CardNo + "','" + ISV.CardType + "','" + ISV.NameOnCard + "','" + ISV.CardExpDate + "'," + ISV.IssuedBy + ",Current_timestamp);";
        //            }
        //            else if (ISV.PaymentType == "Voucher")
        //            {
        //                strInsert = "insert into tabposvoucherissuepaymentwithvoucher(detailsid, numnetpayable, vchpaymentvoucherno, vchpaymentvouchertype, vchpaymentvouchername, vchpaymentvoucherexpdate, createdby, createddate) values (" + voucherid + "," + ISV.NetPayable + ",'" + ISV.CouponNo + "','" + ISV.TypeofCoupon + "','" + ISV.CouponName + "','" + ISV.CouponExpDate + "'," + ISV.IssuedBy + ",Current_timestamp);";
        //            }
        //            else if (ISV.PaymentType == "BOTH")
        //            {
        //                strInsert = "insert into tabposvoucherissuepaymentwithvoucher(detailsid, numnetpayable, vchpaymentvoucherno, vchpaymentvouchertype, vchpaymentvouchername, vchpaymentvoucherexpdate, createdby, createddate) values (" + voucherid + "," + ISV.NetPayable + ",'" + ISV.CouponNo + "','" + ISV.TypeofCoupon + "','" + ISV.CouponName + "','" + ISV.CouponExpDate + "'," + ISV.IssuedBy + ",Current_timestamp);";
        //                strInsert += "insert into tabposissuevouchercarddetails(detailsid,numnetpayable,vchcardnumber,vchcardtype,vchnameoncard,vchcardexpdate,createdby,createddate) values(" + voucherid + "," + ISV.NetPayable + ",'" + ISV.CardNo + "','" + ISV.CardType + "','" + ISV.NameOnCard + "','" + ISV.CardExpDate + "'," + ISV.IssuedBy + ",Current_timestamp);";
        //                strInsert += "insert into tabposissuevouchercashdetails(detailsid, numnetpayable, numnoofthousands, numnooffivehundreds, numnoofhundreds, numnooffiftys, numnooftwentys, numnooftens,numnooffives, numnooftwos, numnoofones, numnooffiftypaisa, createdby, createddate) values (" + voucherid + "," + ISV.NetPayable + "," + ISV.Thousand + "," + ISV.FiveHundred + "," + ISV.Hundred + "," + ISV.Fifty + "," + ISV.Twenty + "," + ISV.Ten + "," + ISV.Five + "," + ISV.Two + "," + ISV.One + "," + ISV.FiftyPaisa + "," + ISV.IssuedBy + ",Current_timestamp);";
        //            }
        //            if (strInsert != string.Empty)
        //            {
        //                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
        //            }
        //        }
        //        trans.Commit();
        //        IsValid = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "SaveIssueVoucher");
        //        trans.Rollback();
        //        IsValid = false;
        //    }
        //    return IsValid;
        //}

        public bool SaveIssueVoucher(IssueVoucher ISV)
        {
            bool IsValid = false;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                int Custlst;
                string CustID = string.Empty;
                string strInsert = "";
                string Voucherid;
                string strcust = "select count(*) from tabposcustomers  where vchcustomername='" + ISV.CustomerName + "' and vchcustomercontactno='" + ISV.ContactNO + "';";
                int count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strcust));
                if (count == 0)
                {
                    strcust = "";
                    strcust = "select coalesce(max(customerid),0) from tabposcustomers;";
                    Custlst = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strcust));
                    strInsert = "insert into tabposcustomers(vchcustid,vchcustomername,vchcustomercontactno) values ('C" + Custlst + "','" + ISV.CustomerName + "','" + ISV.ContactNO + "') returning customerid;";
                    CustID = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strInsert).ToString();
                }
                else
                {
                    CustID = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select customerid from tabposcustomers  where vchcustomername='" + ISV.CustomerName + "' and vchcustomercontactno='" + ISV.ContactNO + "';").ToString();
                }
                Voucherid = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select coalesce(max(voucherid),0) from tabposissuevoucher;").ToString();
                strInsert = "INSERT INTO tabposissuevoucher(vchvouchercode,numvouchertype,numvoucherno,issuedby,authorizedby,vchremarks,numcustomerid,vchcustomername,numvouchervalue,chargeapplicable,numnetpayable,numvouchertaxtotal,createddate,createdby) ";
                strInsert += " VALUES('ISV" + Voucherid + "','" + ISV.VoucherType + "','" + ISV.VoucherNO + "','" + ISV.IssuedBy + "','" + ISV.AuthorizePerson + "','" + ISV.Remarks + "','" + CustID + "','" + ISV.CustomerName + "','" + ISV.VoucherValue + "','" + ISV.Charge + "','" + ISV.NetPayable + "'," + ISV.NetPayable + "-" + ISV.VoucherValue + ",Current_timestamp," + ISV.IssuedBy + ") returning voucherid;";
                string voucherid = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strInsert).ToString();
                if (ISV.ServiceTax != "0" || ISV.ServiceCharge != "0" || ISV.Vat != "0")
                {
                    strInsert = "insert into tabposissuevouchertaxdetails(detailsid,numvouchervalue,numvatpercentage,numvat,numservicetaxpercentage,numservicetax,numservicechargepercentage,numservicecharge,numnetpayable,createdby,createddate) values ('" + voucherid + "'," + ISV.VoucherValue + "," + ISV.Vat + ",(" + ISV.VoucherValue + "*" + ISV.Vat + "::decimal)/100," + ISV.ServiceTax + ",(" + ISV.VoucherValue + "*" + ISV.ServiceTax + "::decimal)/100," + ISV.ServiceCharge + ",(" + ISV.VoucherValue + "*" + ISV.ServiceCharge + "::decimal)/100," + ISV.NetPayable + "," + ISV.IssuedBy + ",Current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                }
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tabposvouchermst set vchvoucherstatus='I',modifiedby=" + ISV.IssuedBy + ",modifieddate=current_timestamp  where voucherid=" + ISV.VoucherNO + " and vouchertype='" + ISV.VoucherType + "';");
                if (ISV.Charge != null)
                {
                    strInsert = string.Empty;
                    if (Convert.ToInt32(ISV.CashTotal) > 0)
                    {
                        strInsert = "insert into tabposissuevouchercashdetails(detailsid, numnetpayable, numnoofthousands, numnooffivehundreds, numnoofhundreds, numnooffiftys, numnooftwentys, numnooftens,numnooffives, numnooftwos, numnoofones, numnooffiftypaisa, createdby, createddate) values (" + voucherid + "," + ISV.NetPayable + "," + ISV.Thousand + "," + ISV.FiveHundred + "," + ISV.Hundred + "," + ISV.Fifty + "," + ISV.Twenty + "," + ISV.Ten + "," + ISV.Five + "," + ISV.Two + "," + ISV.One + "," + ISV.FiftyPaisa + "," + ISV.IssuedBy + ",Current_timestamp);";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                    }
                    if (Convert.ToInt32(ISV.CardAmount) > 0)
                    {
                        strInsert = "insert into tabposissuevouchercarddetails(detailsid,numnetpayable,vchcardnumber,vchcardtype,vchnameoncard,vchcardexpdate,createdby,createddate) values(" + voucherid + "," + ISV.NetPayable + ",'" + ISV.CardNo + "','" + ISV.CardType + "','" + ISV.NameOnCard + "','" + ISV.CardExpDate + "'," + ISV.IssuedBy + ",Current_timestamp);";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                    }
                    if (Convert.ToInt32(ISV.VoucherValue) > 0)
                    {
                        strInsert = "insert into tabposvoucherissuepaymentwithvoucher(detailsid, numnetpayable, vchpaymentvoucherno, vchpaymentvouchertype, vchpaymentvouchername, vchpaymentvoucherexpdate, createdby, createddate) values (" + voucherid + "," + ISV.NetPayable + ",'" + ISV.CouponNo + "','" + ISV.TypeofCoupon + "','" + ISV.CouponName + "','" + ISV.CouponExpDate + "'," + ISV.IssuedBy + ",Current_timestamp);";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                    }
                }
                trans.Commit();
                IsValid = true;

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveIssueVoucher");
                trans.Rollback();
                IsValid = false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                    trans.Dispose();
                }
            }
            return IsValid;
        }
        #endregion



        #endregion

        #region KOT TRANSFER

        public List<KOTTakingDTO> ShowKotTransferDetails(string TableId)
        {
            List<KOTTakingDTO> lstKOT = new List<KOTTakingDTO>();

            int GettableId = 0;
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();

                    string strgettableid = "select tableid from tabpostablesandcoversmst where tablesname='" + TableId + "'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(strgettableid, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            KOTTakingDTO objKot = new KOTTakingDTO();

                            GettableId = Convert.ToInt32(npgdr["tableid"].ToString());


                        }
                    }

                    if (GettableId != 0)
                    {

                        string strInsert = "select ktd.VCHKOTID ,ktd.ITEMNAME,ktd.ITEMQTY,ktd.itemrate as numamount  from tabrmskot kt join  tabrmskotdetails ktd on kt.kotid=ktd.kotid where tableid=" + GettableId + " and ktd.vchitemstatus='N' and ktd.vchkotid not in (select  vchkotid from tabposbillgenerationdetails ) order by ktd.VCHKOTID ";
                        using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                        {
                            npgdr = cmd.ExecuteReader();
                            while (npgdr.Read())
                            {
                                KOTTakingDTO objKot = new KOTTakingDTO();

                                objKot.KotName = npgdr["VCHKOTID"].ToString();

                                objKot.ItemName = npgdr["ITEMNAME"].ToString();


                                objKot.Quantity = Convert.ToInt32(npgdr["ITEMQTY"]);

                                //  objKot.Rate = Convert.ToInt32(npgdr["itemrate"]);
                                objKot.Price = Convert.ToInt32(npgdr["numamount"]);
                                // objKot.Price = Convert.ToInt32(npgdr["ITEMQTY"]) * Convert.ToInt32(npgdr["itemrate"]);
                                lstKOT.Add(objKot);
                            }
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowKotTransferDetails");
                throw ex;
            }
            return lstKOT;
        }
        public List<KOTTakingDTO> ShowKotDetailsBytableid(string TableId)
        {
            List<KOTTakingDTO> lstKOT = new List<KOTTakingDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    string strInsert = "select distinct kt.vchhostname  from tabrmskot kt join  tabrmskotdetails ktd on kt.kotid=ktd.kotid where tableid=" + TableId + " and ktd.vchitemstatus='N' and datkotdate=current_date order by ktd.VCHKOTID ";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            KOTTakingDTO objKot = new KOTTakingDTO();

                            objKot.Host = npgdr["vchhostname"].ToString();



                            lstKOT.Add(objKot);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowKotTransferDetails");
                throw ex;
            }
            return lstKOT;
        }
        public bool KOTTransfereDetails(string KotTransferDto, string TablesfId, string date, string time, string hostname, int ttablesfid)
        {
            bool isKottransfer = false;
            string strKottransfer = string.Empty;
            int createdby = 0;
            string strkottransinsert = string.Empty;


            int TablesId = 0;

            int ttablesid = 0;




            try
            {


                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();



                string strgettableid = "select tableid from tabpostablesandcoversmst where tablesname='" + TablesfId + "'";

                using (NpgsqlCommand cmd = new NpgsqlCommand(strgettableid, con))
                {
                    npgdr = cmd.ExecuteReader();
                    while (npgdr.Read())
                    {
                        KOTTakingDTO objKot = new KOTTakingDTO();

                        TablesId = Convert.ToInt32(npgdr["tableid"].ToString());


                    }
                }


                string strgettabletid = "select tableid from tabpostablesandcoversmst where tablesname='" + ttablesfid + "'";

                using (NpgsqlCommand cmd = new NpgsqlCommand(strgettabletid, con))
                {
                    npgdr = cmd.ExecuteReader();
                    while (npgdr.Read())
                    {
                        KOTTakingDTO objKot = new KOTTakingDTO();

                        ttablesid = Convert.ToInt32(npgdr["tableid"].ToString());


                    }
                }










                if (TablesId != 0 && ttablesid != 0)
                {


                    string strselect = "select userid from tabuserinfo where username='" + hostname + "'";



                    using (NpgsqlCommand cmd = new NpgsqlCommand(strselect, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {

                            createdby = Convert.ToInt32(npgdr["userid"]);


                        }
                    }

                    //1 TablesId
                    strKottransfer = " update tabrmskot set tableid=" + TablesId + " ,createdby=" + createdby + " ,vchhostname='" + hostname + "' where vchkotid in(" + KotTransferDto + ")";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strKottransfer);

                    //2 TablesId
                    string strcovertmstR = "update tabpostablesandcoversmst set status ='R',modifieddate=current_timestamp  where  tableid in(" + TablesId + ") ;";
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, strcovertmstR);


                    string[] Kotids = KotTransferDto.Split(',');

                    for (int i = 0; i < Kotids.Length; i++)
                    {
                        //3 TablesId
                        strkottransinsert = "insert into tabrmskottransfer(vchtranskotid,dattranskotdate,kottranstime,vchtranshostname,tableid,transtotableid) values(" + Kotids[i] + ",'" + FormatDate(date) + "','" + ManageQuote(time) + "','" + ManageQuote(hostname) + "'," + ttablesid + "," + TablesId + ")";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strkottransinsert);
                    }
                    trans.Commit();
                    string strcount = "select count(vchkotid) from vwrmskotdetails where tableid=" + ttablesid + " and  vchitemstatus='N'";
                    int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, strcount));

                    if (count == 0)
                    {
                        int countOfBillsIntable = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select count(vchbillno) from tabposbillgeneration where tableid=" + ttablesid + ""));
                        if (countOfBillsIntable != 0)
                        {
                            string strcounttableid = "select count(vchbillno) from tabposbillgeneration  where vchbillno NOT in(select vchbillno from tabposbillsettlement where tableid=" + ttablesid + ") and tableid=" + ttablesid + "";
                            int iscounttableid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, strcounttableid));
                            if (iscounttableid > 0)
                            {
                                string strcovertmst = "update tabpostablesandcoversmst set status ='B',modifieddate=current_timestamp  where  tableid in(" + ttablesid + ")";
                                NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, strcovertmst);
                            }
                            else
                            {
                                string strcovertmst = "update tabpostablesandcoversmst set status ='A',modifieddate=current_timestamp  where  tableid in(select tableid from tabpostablesandcoversmst where tableid=" + ttablesid + " union select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + ttablesid + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + ttablesid + " union select totableid::int from tabposmergetable where fromtableid in (select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + ttablesid + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + ttablesid + ") and totableid::int in(select tableid from tabpostablesandcoversmst where status='M') union  select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid in( select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + ttablesid + ") order by tableid) ;";
                                NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, strcovertmst);
                            }
                        }
                        else
                        {
                            string strcovertmst = "update tabpostablesandcoversmst set status ='A',modifieddate=current_timestamp  where  tableid in(select tableid from tabpostablesandcoversmst where tableid=" + ttablesid + " union select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + ttablesid + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + ttablesid + " union select totableid::int from tabposmergetable where fromtableid in (select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + ttablesid + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + ttablesid + ") and totableid::int in(select tableid from tabpostablesandcoversmst where status='M') union  select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid in( select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + ttablesid + ") order by tableid) ;";
                            NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, strcovertmst);
                        }

                    }
                }
                //else
                //{
                //    string strcovertmst = "update tabpostablesandcoversmst set status ='A',modifieddate=current_timestamp  where  tableid in(select tableid from tabpostablesandcoversmst where tableid=" + ttablesid + " union select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + ttablesid + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + ttablesid + " union select totableid::int from tabposmergetable where fromtableid in (select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + ttablesid + " union select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid=" + ttablesid + ") and totableid::int in(select tableid from tabpostablesandcoversmst where status='M') union  select mergetableid::int from tabposkotmergedtables where recordid in(select max(recordid) from tabposkotmergedtables where mergetableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by mergetableid ) and  tableid in( select totableid::int from tabposmergetable where recordid in(select max(recordid) from tabposmergetable where totableid::int in(select tableid from tabpostablesandcoversmst where status='M')  group by totableid ) and   fromtableid=" + ttablesid + ") order by tableid) ;";
                //    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, strcovertmst);
                //}
                isKottransfer = true;

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "KOTTransfereDetails");
                throw ex;


            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                    trans.Dispose();
                }

            }

            return isKottransfer;
        }

        //public DataTable GetCashierComboData(int Userid)
        //{
        //    DataTable dt = new DataTable();
        //    string strData = string.Empty;
        //    try
        //    {

        //        strData = "select userid,username from tabuserinfo where userid not in(" + Userid + ");";
        //        dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
        //    }
        //    catch (Exception ex)
        //    {
        //        //EventLogger.WriteToErrorLog(ex, "Designation");
        //    }

        //    return dt;
        //}



        public DataTable GetCashierComboDataBind(string tableid)
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            int GettableId = 0;
            try
            {


                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();

                    string strgettableid = "select tableid from tabpostablesandcoversmst where tablesname='" + tableid + "'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(strgettableid, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            KOTTakingDTO objKot = new KOTTakingDTO();

                            GettableId = Convert.ToInt32(npgdr["tableid"].ToString());


                        }
                    }
                }

                if (GettableId != 0)
                {
                    //strData = "select distinct kt.vchhostname  from tabrmskot kt join  tabrmskotdetails ktd on kt.kotid=ktd.kotid where tableid=" + tableid + " and ktd.vchitemstatus='N' and datkotdate=current_date order by ktd.VCHKOTID ";
                    // strData = "select distinct vchhostname from vwrmskotdetails where tableid=" + GettableId + " ;";
                    strData = "select distinct vchhostname,kottime::time,datkotdate from vwrmskotdetails where tableid=" + GettableId + " and datkotdate=current_date order by kottime::time desc limit 1";
                    dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "KOTTransfereDetails");
            }

            return dt;
        }



        public DataTable GetCashierComboData()
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "select userid,username from tabuserinfo;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "KOTTransfereDetails");
            }

            return dt;
        }







        #endregion






        #region DashBoardRegion




        public bool SaveTableDetails(string TableName, int Createdby)
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
                // string date = FormatDate(System.DateTime.Now.ToString());
                for (int i = 0; i < listStrtablename.Count; i++)
                {
                    string savedetailsTable = "insert into tabpostablesandcoversmstmaster(tablesname,hostid,datfromdate,dattodate,createddate) values(" + listStrtablename[i] + "," + Createdby + ",'" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "',current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, savedetailsTable);
                    strsaveDetails = "update tabpostablesandcoversmst set modifieddate=current_timestamp  where tablesname in(" + listStrtablename[i] + ")";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strsaveDetails);
                    IssaveDetails = true;
                }


            }
            catch (Exception ex)
            {
                throw ex;

                //EventLogger.WriteToErrorLog(ex, "DeliveryBoyDetails");
            }
            finally
            {

                con.Close();

            }
            return IssaveDetails;
        }




        public bool updateTableDetails(int Createdby)
        {
            bool IsUpdateDetails = false;
            string strupdateDetails = string.Empty;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }


                string strcount = "select count(*) from tabpostablesandcoversmst where status in('R','M') and hostid=" + Createdby + "";

                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, strcount));

                if (count == 0)
                {

                    strupdateDetails = "update tabpostablesandcoversmst set vchassignedstatus='N' where hostid=" + Createdby + "";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strupdateDetails);
                    IsUpdateDetails = true;
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
            return IsUpdateDetails;
        }


        public List<DashboardDTO> ShowAlltablesformodel()
        {
            List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {

                    con.Open();

                    string strInsert = "SELECT sectionname,array_to_string(array_agg(distinct tablesname),',') AS tablesname,array_to_string(array_agg(distinct tableid),',') AS tableid FROM tabpostablesandcoversmst WHERE  statusid=1 and modifieddate::date!=CURRENT_DATE GROUP BY sectionname";
                    // string strInsert = "SELECT sectionname,array_to_string(array_agg(distinct tablesname),',') AS tablesname,array_to_string(array_agg(distinct tableid),',') AS tableid FROM tabpostablesandcoversmst WHERE vchassignedstatus IN ('N')  and statusid=1 GROUP BY sectionname;";
                    // string strInsert = "SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST  WHERE vchassignedstatus IN ('N')  and statusid=1 order by TABLEID;";
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














        public List<DashboardDTO> ShowAlltables(int Userid)
        {
            List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    //--old  string strInsert = "SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST  WHERE STATUS IN ('A')  and statusid=1 and tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) UNION ALL SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B') AND statusid=1 and  tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) order by TABLEID";

                    string strInsert = "select tbm.categoryname,tbm.productname as pluname,tbm.uomname ,tbm.numsalerate as unitprice, coalesce(vw.availableqty,'0')as availableqty from vwgetstockdetails vw right join tabmmsproductmst tbm on vw.productid=tbm.productid where coalesce(vw.storagelocation,'MAIN STORAGE')='MAIN STORAGE'  order by pluname;";


                    //  string strInsert = "SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST  WHERE STATUS IN ('A')  and statusid=1 and tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) UNION ALL SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B') AND statusid=1 and  tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) order by TABLEID";
                    // string strInsert = "SELECT STATUS,sectionname,array_to_string(array_agg(distinct tablesname),',') AS tablesname,array_to_string(array_agg(distinct tableid),',')AS tableid FROM TABPOSTABLESANDCOVERSMST  WHERE STATUS IN ('A')  and statusid=1 and vchassignedstatus='Y' and hostid=" + Userid + "group by tablesname,sectionname,tableid,STATUS  UNION ALL  SELECT STATUS,sectionname,array_to_string(array_agg(distinct tablesname),',') AS tablesname,array_to_string(array_agg(distinct tableid),',') AS tableid FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B') AND  TABLEID IN(select TABLEID from tabrmskot where kotid in(select max(kotid) from tabrmskot group by TABLEID ) and createdby=" + Userid + "  )   and statusid=1 and vchassignedstatus='Y' and  hostid=" + Userid + " group by tablesname,sectionname,tableid,STATUS order by TABLEID;";
                    // string strInsert = "SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST  WHERE STATUS IN ('A')  and statusid=1 and tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) UNION ALL SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B') AND  TABLEID IN(select TABLEID from tabrmskot where kotid in(select max(kotid) from tabrmskot group by TABLEID ) and createdby=" + Userid + ")   and statusid=1 and  tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) order by TABLEID";
                    // string strInsert = "SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST  WHERE STATUS IN ('A')  and statusid=1 and vchassignedstatus='Y' and hostid=" + Userid + " UNION ALL SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B') AND  TABLEID IN(select TABLEID from tabrmskot where kotid in(select max(kotid) from tabrmskot group by TABLEID ) and createdby=" + Userid + " )   and statusid=1 and vchassignedstatus='Y' and hostid=" + Userid + " order by TABLEID";
                    // string strInsert = "SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST  WHERE STATUS IN ('A')  and statusid=1 and tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) UNION ALL SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B') AND  TABLEID IN(select TABLEID from tabrmskot where kotid in(select max(kotid) from tabrmskot group by TABLEID ) and createdby=" + Userid + ")   and statusid=1 and  tablesname in(select tablesname from tabpostablesassignedmst where dattodate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + " and statusid=1) order by TABLEID";
                    // string strInsert = "SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST  WHERE STATUS IN ('A')  and statusid=1 and tablesname in(select tablesname from tabpostablesassignedmst where dattodate='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + ") UNION ALL SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B') AND  TABLEID IN(select TABLEID from tabrmskot where kotid in(select max(kotid) from tabrmskot group by TABLEID ) and createdby=" + Userid + ")   and statusid=1 and  tablesname in(select tablesname from tabpostablesassignedmst where dattodate='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and hostid=" + Userid + ") order by TABLEID";

                    // string strInsert = " SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST  WHERE STATUS IN ('A')  and statusid=1 UNION ALL SELECT sectionname,TABLEID,TABLESNAME,STATUS FROM TABPOSTABLESANDCOVERSMST WHERE  STATUS IN ('R','B') AND  TABLEID IN(select TABLEID from tabrmskot where kotid in(select max(kotid) from tabrmskot group by TABLEID ) and createdby=" + Userid + " )   and statusid=1 order by TABLEID";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();

                        while (npgdr.Read())
                        {
                            DashboardDTO objKOTTakingDTO = new DashboardDTO();


                            objKOTTakingDTO.Categoryname = npgdr["categoryname"].ToString();
                            objKOTTakingDTO.Uomname = Convert.ToString(npgdr["uomname"]);
                            objKOTTakingDTO.Pluname = Convert.ToString(npgdr["pluname"]);
                            objKOTTakingDTO.Unitprice = Convert.ToString(npgdr["unitprice"]);
                            objKOTTakingDTO.Availableqty = Convert.ToString(npgdr["availableqty"]);

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











        #endregion


        #region Old Code Indent Details By Vikram

        //public List<MaterialIndentDTO> ShowIndentProducts()
        //{
        //    List<MaterialIndentDTO> lstIndent = new List<MaterialIndentDTO>();
        //    try
        //    {
        //        string strInsert = "select  productid,productname,productcode,productcategoryid,categoryname,subcategoryname,productsubcategoryid,producttype,uomname,vchuomid,coalesce(minqty,0) as minqty,coalesce(maxqty,0) as maxqty,coalesce( maxqty-minqty ,0)as availaibleqnty from tabmmsproductmst  where statusid=1 ;";
        //        npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
        //        while (npgdr.Read())
        //        {
        //            MaterialIndentDTO objIndentGeneration = new MaterialIndentDTO();

        //            objIndentGeneration.productname = Convert.ToString(npgdr["productname"]);
        //            objIndentGeneration.productid = Convert.ToInt16(npgdr["productid"]);
        //            objIndentGeneration.productcategoryid = Convert.ToInt16(npgdr["productcategoryid"]);
        //            objIndentGeneration.productsubcategoryid = Convert.ToInt16(npgdr["productsubcategoryid"]);
        //            objIndentGeneration.productcode = Convert.ToString(npgdr["productcode"]);
        //            objIndentGeneration.vchuomid = Convert.ToString(npgdr["vchuomid"]);
        //            objIndentGeneration.categoryname = Convert.ToString(npgdr["categoryname"]);
        //            objIndentGeneration.subcategoryname = Convert.ToString(npgdr["subcategoryname"]);
        //            objIndentGeneration.producttype = Convert.ToString(npgdr["producttype"]);
        //            objIndentGeneration.uomname = Convert.ToString(npgdr["uomname"]);
        //            // objIndentGeneration.minqty = Convert.ToString(npgdr["minqty"]) == null ? 0 : Convert.ToDecimal(npgdr["minqty"]);
        //            objIndentGeneration.minqty = Convert.ToDecimal(npgdr["minqty"]);
        //            objIndentGeneration.maxqty = Convert.ToDecimal(npgdr["maxqty"]);
        //            objIndentGeneration.AvailableQty = Convert.ToDecimal(npgdr["availaibleqnty"]);

        //            lstIndent.Add(objIndentGeneration);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "ShowProducts");
        //        throw ex;
        //    }
        //    return lstIndent;

        //}

        //public List<MaterialIndentDTO> ShowStorage()
        //{
        //    List<MaterialIndentDTO> lstStorages = new List<MaterialIndentDTO>();
        //    string strstaorage = string.Empty;
        //    try
        //    {
        //        strstaorage = "select storagelocationname,storagelocationcode from tabmmsstoragelocationmst  where statusid=1";
        //        npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strstaorage);
        //        while (npgdr.Read())
        //        {
        //            MaterialIndentDTO objIndentStorages = new MaterialIndentDTO();

        //            objIndentStorages.storagelocationname = Convert.ToString(npgdr["storagelocationname"]);
        //            objIndentStorages.storagelocationcode = Convert.ToString(npgdr["storagelocationcode"]);
        //            lstStorages.Add(objIndentStorages);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return lstStorages;
        //}

        //public List<MaterialIndentDTO> GetSelfdetails(string showroomselected)
        //{
        //    string strselfdata = string.Empty;
        //    //   showroomselected = showroomselected.Split(':')[1].ToString();
        //    // string:SL27 'SL11'
        //    // showroomselected = "SL11";
        //    List<MaterialIndentDTO> lstSelfdata = new List<MaterialIndentDTO>();
        //    try
        //    {
        //        strselfdata = "select shelfname,storagearea,storagecode from tabmmsshelfmst where storagecode='" + showroomselected + "' ;";
        //        npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strselfdata);
        //        while (npgdr.Read())
        //        {
        //            MaterialIndentDTO objIndentSelfnames = new MaterialIndentDTO();
        //            objIndentSelfnames.shelfname = Convert.ToString(npgdr["shelfname"]);
        //            objIndentSelfnames.storagelocationname = Convert.ToString(npgdr["storagearea"]);
        //            objIndentSelfnames.storagelocationcode = Convert.ToString(npgdr["storagecode"]);
        //            lstSelfdata.Add(objIndentSelfnames);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return lstSelfdata;
        //}

        //public List<MaterialIndentDTO> showrequestedby()
        //{
        //    string strrequestedby = string.Empty;
        //    List<MaterialIndentDTO> lstrequesteby = new List<MaterialIndentDTO>();
        //    try
        //    {
        //        // strrequestedby = "select vchindentno,vchrequestedby from tabmmsmaterialindent where statusid=1";
        //        strrequestedby = "SELECT vchemployeeid,vchname||' '||vchsurname as employeefullname FROM tabhrmsemployeepersonaldetails   ;";

        //        npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strrequestedby);
        //        while (npgdr.Read())
        //        {
        //            MaterialIndentDTO objIndentRequesteby = new MaterialIndentDTO();
        //            objIndentRequesteby.vchindentno = Convert.ToString(npgdr["vchemployeeid"]);
        //            objIndentRequesteby.RequestedBy = Convert.ToString(npgdr["employeefullname"]);
        //            lstrequesteby.Add(objIndentRequesteby);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return lstrequesteby;
        //}

        ////public List<MaterialIndentDTO> ShowUOM1()
        ////{
        ////    List<MaterialIndentDTO> lstUOM = new List<MaterialIndentDTO>();
        ////    try
        ////    {
        ////        using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
        ////        {
        ////            con.Open();
        ////            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT VCHUOMID,VCHUOMDESCRIPTION FROM TABINVUOMMST  WHERE INTSTATUS=1 ORDER BY VCHUOMDESCRIPTION;", con))
        ////            {
        ////                npgdr = cmd.ExecuteReader();
        ////                while (npgdr.Read())
        ////                {
        ////                    MaterialIndentDTO objUOM = new MaterialIndentDTO();
        ////                    objUOM.uomid = Convert.ToString(npgdr["VCHUOMID"]);
        ////                    objUOM.uom = npgdr["VCHUOMDESCRIPTION"].ToString();
        ////                    // objUOM.uomabbreviation = npgdr["uomabbreviation"].ToString();

        ////                    lstUOM.Add(objUOM);
        ////                }
        ////            }
        ////            con.Close();
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        EventLogger.WriteToErrorLog(ex, "ShowUOM");
        ////        throw ex;
        ////    }
        ////    finally
        ////    {
        ////        npgdr.Dispose();
        ////    }
        ////    return lstUOM;
        ////}


        //public bool SaveIndent(List<MaterialIndentDTO> griddata, MaterialIndentDTO BE)
        //{

        //    bool issaved = false;
        //    string strsave = string.Empty;
        //    string strsavedetails = string.Empty;
        //    try
        //    {
        //        con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        //        if (con.State != ConnectionState.Open)
        //        {
        //            con.Open();
        //        }
        //        trans = con.BeginTransaction();
        //        int count = 0;

        //        string strIndentEdit = "select substring(vchindentno,4)::int +1 from tabmmsmaterialindent order by recordid desc limit 1;";
        //        int vchKOTID = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strIndentEdit));
        //        BE.vchindentno = "IND" + vchKOTID;

        //        strsave = "INSERT INTO tabmmsmaterialindent(vchindentno,datindentdate,vchindenttype,vchrequestedby, vchapprovalby, statusid, createdby, createddate,vchdepartment)VALUES('" + BE.vchindentno + "', '" + BE.vchdate.ToString("dd-MM-yyyy") + "', ";
        //        if (BE.Indenttypenew != string.Empty)
        //        {
        //            strsave = strsave + " '" + BE.Indenttypenew + "', '" + BE.RequestedBy + "', '" + BE.ApprovedBy + "', '1', '" + BE.createdby + "', CURRENT_TIMESTAMP,'" + BE.DeparmentName + "' ) returning recordid;";
        //        }
        //        if (BE.Indenttypeexisting != string.Empty && BE.Indenttypeexisting != null)
        //        {
        //            strsave = strsave + " '" + BE.Indenttypeexisting + "', '" + BE.RequestedBy + "', '" + BE.ApprovedBy + "', '1', '" + BE.createdby + "', CURRENT_TIMESTAMP,'" + BE.DeparmentName + "') returning recordid;";
        //        }

        //        count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strsave));

        //        int j = griddata.Count;
        //        for (int i = 0; i < griddata.Count; i++)
        //        {
        //            if (count != 0)
        //            {
        //                string AvailableQty = Convert.ToString(griddata[i].AvailableQty);
        //                if (AvailableQty == "")
        //                {
        //                    griddata[i].AvailableQty = 0;
        //                }
        //                strsavedetails = "INSERT INTO tabmmsmaterialindentdetails(indentno, vchindentno, productid, productcode, productcategoryid, categoryname, productsubcategoryid, subcategoryname, vchuom,  numindentqty, numindentconvertionqty, statusid, createdby, createddate, productname,storagelocationname,storagelocationcode,shelfname,AvailableQty)VALUES ('" + count + "', '" + BE.vchindentno + "', '" + griddata[i].productid + "', '" + griddata[i].productcode + "', '" + griddata[i].productcategoryid + "', '" + griddata[i].categoryname.ToString() + "', '" + griddata[i].productsubcategoryid + "', '" + griddata[i].subcategoryname.ToString() + "', '" + griddata[i].uomname.ToString() + "',  '" + griddata[i].indentqty + "', '22', '1', '" + BE.createdby + "',CURRENT_TIMESTAMP, '" + griddata[i].productname + "','" + griddata[i].storagetext + "','" + griddata[i].storagelocationname + "','" + griddata[i].shelfname + "','" + griddata[i].AvailableQty + "');";
        //                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strsavedetails);
        //                issaved = true;
        //            }
        //            else
        //            {
        //                issaved = false;
        //            }
        //        }
        //        if (issaved == true)
        //        {
        //            trans.Commit();
        //        }
        //        return issaved;
        //    }
        //    catch (Exception ex)
        //    {
        //        trans.Rollback();
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Close();
        //            con.Dispose();
        //            con.ClearPool();
        //            trans.Dispose();
        //        }
        //    }
        //    return issaved;
        //}


        //public List<MaterialIndentDTO> GetExistingIndentNo(string IndentType)
        //{
        //    List<MaterialIndentDTO> lstIndentDetails = new List<MaterialIndentDTO>();

        //    try
        //    {
        //        using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
        //        {
        //            con.Open();
        //            string sdfsd = IndentType.ToString();
        //            using (NpgsqlCommand cmd = new NpgsqlCommand("select TA.recordid,TA.vchindentno,TA.datindentdate,TA.vchindenttype,TA.vchrequestedby,TA.vchapprovalby,TA.statusid,TA.createdby,TA.createddate,TA.modifiedby,TA.modifieddate,TB.recordid as recordid2,TB.indentno as indentno2,TB.vchindentno as vchindentno2,TB.productid as productid2,TB.productcode as productcode2,TB.productcategoryid as productcategoryid2,TB.categoryname as categoryname2,TB.productsubcategoryid as productsubcategoryid2,TB.subcategoryname as subcategoryname2,TB.vchuom as vchuom2,TB.vchindentuom as vchindentuom2,TB.numindentqty as numindentqty2,TB.numindentconvertionqty as numindentconvertionqty2,TB.statusid as statusid2,TB.createdby as createdby2,TB.AvailableQty as AvailableQty2,TB.createddate as createddate2,TB.modifiedby as modifiedby2,TB.modifieddate as modifieddate2,TB.productname as productname2  from tabmmsmaterialindent TA   join  tabmmsmaterialindentdetails TB on TA.recordid= TB.indentno where TB.vchindentno='" + IndentType + "'   ;", con))
        //            {
        //                npgdr = cmd.ExecuteReader();
        //                while (npgdr.Read())
        //                {
        //                    MaterialIndentDTO objIndentDetails = new MaterialIndentDTO();
        //                    objIndentDetails.RequestedBy = Convert.ToString(npgdr["vchrequestedby"]);
        //                    objIndentDetails.ApprovedBy = Convert.ToString(npgdr["vchapprovalby"]);

        //                    objIndentDetails.recordid = Convert.ToInt16(npgdr["indentno2"]);
        //                    objIndentDetails.IndentNo = Convert.ToString(npgdr["vchindentno2"]);
        //                    objIndentDetails.productid = Convert.ToInt16(npgdr["productid2"]);
        //                    objIndentDetails.productcode = Convert.ToString(npgdr["productcode2"]);
        //                    //productcategoryid2,categoryname2,productsubcategoryid2,subcategoryname2,vchuom2,vchindentuom2
        //                    //numindentqty2,numindentconvertionqty2,productname2
        //                    objIndentDetails.productcategoryid = Convert.ToInt16(npgdr["productcategoryid2"]);
        //                    objIndentDetails.categoryname = Convert.ToString(npgdr["categoryname2"]);
        //                    objIndentDetails.productsubcategoryid = Convert.ToInt16(npgdr["productsubcategoryid2"]);
        //                    objIndentDetails.subcategoryname = Convert.ToString(npgdr["subcategoryname2"]);
        //                    objIndentDetails.uomname = Convert.ToString(npgdr["vchuom2"]);
        //                    objIndentDetails.vchuomid = Convert.ToString(npgdr["vchindentuom2"]);
        //                    objIndentDetails.indentqty = Convert.ToDecimal(npgdr["numindentqty2"]);
        //                    objIndentDetails.maxqty = Convert.ToDecimal(npgdr["numindentconvertionqty2"]);
        //                    objIndentDetails.productname = Convert.ToString(npgdr["productname2"]);
        //                    if (npgdr["AvailableQty2"] != DBNull.Value)
        //                    {
        //                        objIndentDetails.AvailableQty = Convert.ToDecimal(npgdr["AvailableQty2"]);
        //                    }

        //                    //  missing this AvailableQty
        //                    lstIndentDetails.Add(objIndentDetails);

        //                }
        //            }
        //            con.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "ShowIndentDetails");
        //        throw ex;
        //    }
        //    finally
        //    {
        //        npgdr.Dispose();
        //    }
        //    return lstIndentDetails;
        //}

        //public List<MaterialIndentDTO> GetExistingIndents(string Indents)
        //{
        //    List<MaterialIndentDTO> lstIndents = new List<MaterialIndentDTO>();
        //    try
        //    {
        //        using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
        //        {
        //            con.Open();
        //            //  using (NpgsqlCommand cmd = new NpgsqlCommand("select TA.recordid,TA.vchindentno,TA.statusid  from tabmmsmaterialindent TA join tabmmsmaterialissue TB on TB.vchindentno!=TA.vchindentno ;", con))
        //            using (NpgsqlCommand cmd = new NpgsqlCommand("select recordid,vchindentno from tabmmsmaterialindent where statusid=1 and vchindentno not in (select vchindentno from tabmmsmaterialissue );", con))
        //            {
        //                npgdr = cmd.ExecuteReader();
        //                while (npgdr.Read())
        //                {
        //                    MaterialIndentDTO objIndents = new MaterialIndentDTO();
        //                    objIndents.recordid = Convert.ToInt16(npgdr["recordid"]);
        //                    objIndents.IndentNo = Convert.ToString(npgdr["vchindentno"]);
        //                    lstIndents.Add(objIndents);
        //                }
        //            }
        //            con.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "ShowIndentDetails");
        //        throw ex;
        //    }
        //    finally
        //    {
        //        npgdr.Dispose();
        //    }
        //    return lstIndents;

        //}

        //public List<MaterialIndentDTO> GetProductAvailability(string productid, string productname)
        //{
        //    List<MaterialIndentDTO> lstIndents = new List<MaterialIndentDTO>();
        //    try
        //    {
        //        using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
        //        {
        //            con.Open();

        //            using (NpgsqlCommand cmd = new NpgsqlCommand("select productid,productname,categoryname,subcategoryname,uomname,storagelocation,shelfname,availableqty from vwgetstockdetails where productid='" + productid + "' and productname='" + productname + "';", con))
        //            {
        //                npgdr = cmd.ExecuteReader();
        //                while (npgdr.Read())
        //                {
        //                    MaterialIndentDTO objprdctavilable = new MaterialIndentDTO();
        //                    objprdctavilable.AvailableQty = Convert.ToInt16(npgdr["availableqty"]);
        //                    if (objprdctavilable.AvailableQty == null)
        //                    {
        //                        objprdctavilable.AvailableQty = 0;
        //                    }
        //                    // objprdctavilable.IndentNo = Convert.ToString(npgdr["vchindentno"]);
        //                    lstIndents.Add(objprdctavilable);
        //                }
        //            }
        //            con.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "ShowIndentDetails");
        //        throw ex;
        //    }
        //    finally
        //    {
        //        npgdr.Dispose();
        //    }
        //    return lstIndents;
        //}

        //public List<MaterialIndentDTO> ShowDepartment()
        //{
        //    List<MaterialIndentDTO> lstDepartmentdetails = new List<MaterialIndentDTO>();
        //    try
        //    {
        //        string strInsert = "SELECT DEPARTMENTID,DEPARTMENTNAME,DEPARTMENTCODE FROM TABPOSDEPARTMENTMST WHERE STATUSID=1 ORDER BY DEPARTMENTID desc;";
        //        npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
        //        while (npgdr.Read())
        //        {
        //            MaterialIndentDTO objDepartmentDTO = new MaterialIndentDTO();
        //            objDepartmentDTO.deptid = Convert.ToInt32(npgdr["DEPARTMENTID"]);
        //            objDepartmentDTO.DeparmentName = npgdr["DEPARTMENTNAME"].ToString();
        //            objDepartmentDTO.DeparmentCode = Convert.ToString(npgdr["DEPARTMENTCODE"]);

        //            lstDepartmentdetails.Add(objDepartmentDTO);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "ShowDepartment");
        //        throw ex;
        //        // ErrorMessage(ex);
        //    }
        //    finally
        //    {
        //        npgdr.Dispose();
        //    }
        //    return lstDepartmentdetails;

        //}


        #endregion




        #region srinath
        //public List<DashboardDTO> ChartListForCategorywisePeriod(int Userid, string Period)
        //{
        //    string strInsert = "";
        //    List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();
        //    try
        //    {
        //        using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
        //        {
        //            con.Open();
        //            if (Userid == 1)
        //            {

        //                //strInsert = "SELECT ITEMCATEGORYNAME,sum(round(PAIDAMOUNT,2)) as PAIDAMOUNT  FROM vwbilldatecategorywiseamount  group by ITEMCATEGORYNAME";
        //                strInsert = "SELECT ITEMCATEGORYNAME,sum(round(PAIDAMOUNT,2)) as PAIDAMOUNT  FROM vwbilldatecategorywiseamount where billdate=current_date group by ITEMCATEGORYNAME ";

        //            }
        //            else
        //            {

        //                //strInsert = "SELECT ITEMCATEGORYNAME,round(PAIDAMOUNT,2) as PAIDAMOUNT FROM  vwbilldatecategorywiseamount where createdby=" + Userid + "; ";
        //                strInsert = "SELECT ITEMCATEGORYNAME,round(PAIDAMOUNT,2) as PAIDAMOUNT FROM  vwbilldatecategorywiseamount where billdate=current_date and  createdby=" + Userid + "; ";
        //            }

        //            using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
        //            {
        //                npgdr = cmd.ExecuteReader();
        //                while (npgdr.Read())
        //                {
        //                    DashboardDTO objKOTTakingDTO = new DashboardDTO();
        //                    objKOTTakingDTO.PlanName = Convert.ToString(npgdr["ITEMCATEGORYNAME"]);
        //                    objKOTTakingDTO.PaymentAmount = Convert.ToDecimal(npgdr["PAIDAMOUNT"]);

        //                    lstKOTTakingDTO.Add(objKOTTakingDTO);
        //                }
        //            }
        //            con.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "ChartList");
        //        throw ex;

        //    }
        //    finally
        //    {
        //        npgdr.Dispose();
        //    }
        //    return lstKOTTakingDTO;
        //}


        //public List<DashboardDTO> ChartListForSessionwisePeriod(int Userid, string Period)
        //{
        //    List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();
        //    string strInsert = "";
        //    try
        //    {
        //        using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
        //        {
        //            con.Open();

        //            if (Userid == 1)
        //            {

        //                strInsert = "select  (sessionname),(round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2)) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid  where  billdate = 'now'::text::date group by sessionname";

        //            }
        //            else
        //            {

        //                strInsert = "select sessionname,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid WHERE vw.createdby=" + Userid + " and billdate = 'now'::text::date group by vw.sessionid,sessionname,vw.createdby; ";

        //            }



        //            using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
        //            {
        //                npgdr = cmd.ExecuteReader();
        //                while (npgdr.Read())
        //                {
        //                    DashboardDTO objKOTTakingDTO = new DashboardDTO();
        //                    objKOTTakingDTO.PlanName = Convert.ToString(npgdr["sessionname"]);
        //                    objKOTTakingDTO.PaymentAmount = Convert.ToDecimal(npgdr["PAIDAMOUNT"]);

        //                    lstKOTTakingDTO.Add(objKOTTakingDTO);
        //                }
        //            }
        //            con.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "ChartList");
        //        throw ex;

        //    }
        //    finally
        //    {
        //        npgdr.Dispose();
        //    }
        //    return lstKOTTakingDTO;
        //}

        //public List<DashboardDTO> ChartListForSectionwisePeriod(int Userid, string Period)
        //{
        //    List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();
        //    string strInsert = "";
        //    try
        //    {
        //        using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
        //        {
        //            con.Open();


        //            if (Userid == 1)
        //            {

        //                strInsert = "SELECT  SECTIONNAME,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) AS PAIDAMOUNT FROM VWITEMDISCOUNTDETAILS VW LEFT JOIN TABRMSKOT TK ON  VW.VCHKOTID=TK.VCHKOTID LEFT JOIN TABPOSSECTIONMST TS ON TK.SECTIONID=TS.SECTIONID   WHERE BILLDATE=CURRENT_DATE  GROUP BY SECTIONNAME;";

        //            }
        //            else
        //            {

        //                strInsert = "SELECT  SECTIONNAME,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) AS PAIDAMOUNT FROM VWITEMDISCOUNTDETAILS VW LEFT JOIN TABRMSKOT TK ON  VW.VCHKOTID=TK.VCHKOTID LEFT JOIN TABPOSSECTIONMST TS ON TK.SECTIONID=TS.SECTIONID   WHERE BILLDATE=CURRENT_DATE  AND VW.CREATEDBY=" + Userid + "   GROUP BY SECTIONNAME;";

        //            }




        //            using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
        //            {
        //                npgdr = cmd.ExecuteReader();
        //                while (npgdr.Read())
        //                {
        //                    DashboardDTO objKOTTakingDTO = new DashboardDTO();
        //                    objKOTTakingDTO.PlanName = Convert.ToString(npgdr["SECTIONNAME"]);
        //                    objKOTTakingDTO.PaymentAmount = Convert.ToDecimal(npgdr["PAIDAMOUNT"]);

        //                    lstKOTTakingDTO.Add(objKOTTakingDTO);
        //                }
        //            }
        //            con.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "ChartList");
        //        throw ex;

        //    }
        //    finally
        //    {
        //        npgdr.Dispose();
        //    }
        //    return lstKOTTakingDTO;
        //}

        #endregion

        #region srinath
        public List<DashboardDTO> ChartListForCategorywisePeriod(int Userid, string Period)
        {
            string strInsert = "";
            List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();

            string ResultPeriod = Period;
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    if (Userid == 1)
                    {

                        //strInsert = "SELECT ITEMCATEGORYNAME,sum(round(PAIDAMOUNT,2)) as PAIDAMOUNT  FROM vwbilldatecategorywiseamount  group by ITEMCATEGORYNAME";
                        if (ResultPeriod == "Today")
                        {
                            strInsert = "SELECT ITEMCATEGORYNAME,sum(round(PAIDAMOUNT,2)) as PAIDAMOUNT  FROM vwbilldatecategorywiseamount where billdate=current_date group by ITEMCATEGORYNAME ";
                        }
                        if (ResultPeriod == "Weekly")
                        {
                            strInsert = "SELECT ITEMCATEGORYNAME,sum(round(PAIDAMOUNT,2)) as PAIDAMOUNT  FROM vwbilldatecategorywiseamount where billdate between current_date-7 and current_date group by ITEMCATEGORYNAME";
                            //  strInsert = "SELECT ITEMCATEGORYNAME,sum(round(PAIDAMOUNT,2)) as PAIDAMOUNT  FROM vwbilldatecategorywiseamount where billdate =date_trunc('week',current_date) group by ITEMCATEGORYNAME ";
                        }

                        if (ResultPeriod == "Monthly")
                        {
                            strInsert = "SELECT ITEMCATEGORYNAME,sum(round(PAIDAMOUNT,2)) as PAIDAMOUNT  FROM vwbilldatecategorywiseamount where billdate between current_date-30 and  current_date group by ITEMCATEGORYNAME";
                            //strInsert = "SELECT ITEMCATEGORYNAME,sum(round(PAIDAMOUNT,2)) as PAIDAMOUNT  FROM vwbilldatecategorywiseamount where billdate=date_trunc('month',current_date) group by ITEMCATEGORYNAME ";
                        }
                        if (ResultPeriod == "Yearly")
                        {

                            strInsert = "SELECT ITEMCATEGORYNAME,sum(round(PAIDAMOUNT,2)) as PAIDAMOUNT  FROM vwbilldatecategorywiseamount where billdate between current_date-365 and current_date group by ITEMCATEGORYNAME";

                            // strInsert = "SELECT ITEMCATEGORYNAME,sum(round(PAIDAMOUNT,2)) as PAIDAMOUNT  FROM vwbilldatecategorywiseamount where billdate=date_trunc('year',current_date) group by ITEMCATEGORYNAME ";
                        }

                    }
                    else
                    {

                        if (ResultPeriod == "Today")
                        {
                            strInsert = "SELECT ITEMCATEGORYNAME,round(PAIDAMOUNT,2) as PAIDAMOUNT FROM  vwbilldatecategorywiseamount where billdate=current_date and  createdby=" + Userid + "; ";
                        }

                        if (ResultPeriod == "Weekly")
                        {
                            //  strInsert = "SELECT ITEMCATEGORYNAME,round(PAIDAMOUNT,2) as PAIDAMOUNT FROM  vwbilldatecategorywiseamount where billdate=date_trunc('week',current_date) and  createdby=" + Userid + "; ";

                            strInsert = "SELECT ITEMCATEGORYNAME,round(PAIDAMOUNT,2) as PAIDAMOUNT FROM  vwbilldatecategorywiseamount where billdate between current_date-7 and current_date and  createdby=" + Userid + "; ";

                        }

                        if (ResultPeriod == "Monthly")
                        {
                            //  strInsert = "SELECT ITEMCATEGORYNAME,round(PAIDAMOUNT,2) as PAIDAMOUNT FROM  vwbilldatecategorywiseamount where billdate=date_trunc('month',current_date) and  createdby=" + Userid + "; ";


                            strInsert = "SELECT ITEMCATEGORYNAME,round(PAIDAMOUNT,2) as PAIDAMOUNT FROM  vwbilldatecategorywiseamount where billdate between current_date-30 and current_date and  createdby=" + Userid + "; ";

                        }
                        if (ResultPeriod == "Yearly")
                        {
                            // strInsert = "SELECT ITEMCATEGORYNAME,round(PAIDAMOUNT,2) as PAIDAMOUNT FROM  vwbilldatecategorywiseamount where billdate=date_trunc('year',current_date) and  createdby=" + Userid + "; ";


                            strInsert = "SELECT ITEMCATEGORYNAME,round(PAIDAMOUNT,2) as PAIDAMOUNT FROM  vwbilldatecategorywiseamount where billdate between current_date-365 and current_date and  createdby=" + Userid + "; ";

                        }








                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            DashboardDTO objKOTTakingDTO = new DashboardDTO();
                            objKOTTakingDTO.PlanName = Convert.ToString(npgdr["ITEMCATEGORYNAME"]);
                            objKOTTakingDTO.PaymentAmount = Convert.ToDecimal(npgdr["PAIDAMOUNT"]);

                            lstKOTTakingDTO.Add(objKOTTakingDTO);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ChartList");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstKOTTakingDTO;
        }


        public List<DashboardDTO> ChartListForSessionwisePeriod(int Userid, string Period)
        {
            List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();
            string strInsert = "";

            string Resultsessionperiod = Period;


            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();

                    if (Userid == 1)
                    {
                        if (Resultsessionperiod == "Today")
                        {
                            strInsert = "select  (sessionname),(round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2)) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid  where  billdate =current_date group by sessionname";
                        }
                        if (Resultsessionperiod == "Weekly")
                        {
                            // strInsert = "select  (sessionname),(round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2)) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid  where  billdate = date_trunc('week',current_date) group by sessionname";


                            strInsert = "select  (sessionname),(round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2)) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid  where  billdate between current_date-7 and current_date group by sessionname";
                        }

                        if (Resultsessionperiod == "Monthly")
                        {
                            // strInsert = "select  (sessionname),(round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2)) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid  where  billdate = date_trunc('month',current_date) group by sessionname";

                            strInsert = "select  (sessionname),(round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2)) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid  where  billdate between current_date-30 and current_date group by sessionname";
                        }

                        if (Resultsessionperiod == "Yearly")
                        {
                            // strInsert = "select  (sessionname),(round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2)) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid  where  billdate = date_trunc('year',current_date) group by sessionname";

                            strInsert = "select  (sessionname),(round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2)) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid  where  billdate between current_date-365 and current_date group by sessionname";
                        }
                    }
                    else
                    {
                        if (Resultsessionperiod == "Today")
                        {
                            strInsert = "select sessionname,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid WHERE vw.createdby=" + Userid + " and billdate =current_date group by vw.sessionid,sessionname,vw.createdby; ";
                        }
                        if (Resultsessionperiod == "Weekly")
                        {
                            // strInsert = "select sessionname,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid WHERE vw.createdby=" + Userid + " and billdate = date_trunc('week',current_date) group by vw.sessionid,sessionname,vw.createdby; ";

                            strInsert = "select sessionname,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid WHERE vw.createdby=" + Userid + " and billdate between current_date-7 and current_date group by vw.sessionid,sessionname,vw.createdby; ";
                        }
                        if (Resultsessionperiod == "Monthly")
                        {
                            // strInsert = "select sessionname,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid WHERE vw.createdby=" + Userid + " and billdate = date_trunc('month',current_date) group by vw.sessionid,sessionname,vw.createdby; ";


                            strInsert = "select sessionname,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid WHERE vw.createdby=" + Userid + " and billdate between current_date-30 and current_date group by vw.sessionid,sessionname,vw.createdby; ";
                        }
                        if (Resultsessionperiod == "Yearly")
                        {
                            // strInsert = "select sessionname,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid WHERE vw.createdby=" + Userid + " and billdate = date_trunc('year',current_date) group by vw.sessionid,sessionname,vw.createdby; ";

                            strInsert = "select sessionname,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) as paidamount from vwitemdiscountdetails vw left join tabpossessionmst ts on vw.sessionid=ts.sessionid WHERE vw.createdby=" + Userid + " and billdate between current_date-365 and current_date group by vw.sessionid,sessionname,vw.createdby; ";
                        }
                    }



                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            DashboardDTO objKOTTakingDTO = new DashboardDTO();
                            objKOTTakingDTO.PlanName = Convert.ToString(npgdr["sessionname"]);
                            objKOTTakingDTO.PaymentAmount = Convert.ToDecimal(npgdr["PAIDAMOUNT"]);

                            lstKOTTakingDTO.Add(objKOTTakingDTO);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ChartList");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstKOTTakingDTO;
        }

        public List<DashboardDTO> ChartListForSectionwisePeriod(int Userid, string Period)
        {
            List<DashboardDTO> lstKOTTakingDTO = new List<DashboardDTO>();
            string strInsert = "";

            string strsectionresult = Period;

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();


                    if (Userid == 1)
                    {
                        if (strsectionresult == "Today")
                        {
                            strInsert = "SELECT  SECTIONNAME,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) AS PAIDAMOUNT FROM VWITEMDISCOUNTDETAILS VW LEFT JOIN TABRMSKOT TK ON  VW.VCHKOTID=TK.VCHKOTID LEFT JOIN TABPOSSECTIONMST TS ON TK.SECTIONID=TS.SECTIONID   WHERE BILLDATE=current_date  GROUP BY SECTIONNAME;";
                        }

                        if (strsectionresult == "Weekly")
                        {

                            strInsert = "SELECT  SECTIONNAME,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) AS PAIDAMOUNT FROM VWITEMDISCOUNTDETAILS VW LEFT JOIN TABRMSKOT TK ON  VW.VCHKOTID=TK.VCHKOTID LEFT JOIN TABPOSSECTIONMST TS ON TK.SECTIONID=TS.SECTIONID   WHERE BILLDATE between current_date-7 and current_date  GROUP BY SECTIONNAME;";
                        }
                        if (strsectionresult == "Monthly")
                        {

                            strInsert = "SELECT  SECTIONNAME,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) AS PAIDAMOUNT FROM VWITEMDISCOUNTDETAILS VW LEFT JOIN TABRMSKOT TK ON  VW.VCHKOTID=TK.VCHKOTID LEFT JOIN TABPOSSECTIONMST TS ON TK.SECTIONID=TS.SECTIONID   WHERE BILLDATE between current_date-30 and current_date  GROUP BY SECTIONNAME;";
                        }
                        if (strsectionresult == "Yearly")
                        {

                            strInsert = "SELECT  SECTIONNAME,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) AS PAIDAMOUNT FROM VWITEMDISCOUNTDETAILS VW LEFT JOIN TABRMSKOT TK ON  VW.VCHKOTID=TK.VCHKOTID LEFT JOIN TABPOSSECTIONMST TS ON TK.SECTIONID=TS.SECTIONID   WHERE BILLDATE between current_date-365 and current_date  GROUP BY SECTIONNAME;";
                        }
                    }
                    else
                    {
                        if (strsectionresult == "Today")
                        {
                            strInsert = "SELECT  SECTIONNAME,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) AS PAIDAMOUNT FROM VWITEMDISCOUNTDETAILS VW LEFT JOIN TABRMSKOT TK ON  VW.VCHKOTID=TK.VCHKOTID LEFT JOIN TABPOSSECTIONMST TS ON TK.SECTIONID=TS.SECTIONID   WHERE BILLDATE=current_date  AND VW.CREATEDBY=" + Userid + "   GROUP BY SECTIONNAME;";
                        }
                        if (strsectionresult == "Weekly")
                        {
                            strInsert = "SELECT  SECTIONNAME,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) AS PAIDAMOUNT FROM VWITEMDISCOUNTDETAILS VW LEFT JOIN TABRMSKOT TK ON  VW.VCHKOTID=TK.VCHKOTID LEFT JOIN TABPOSSECTIONMST TS ON TK.SECTIONID=TS.SECTIONID   WHERE BILLDATE between current_date-7 and   current_date  AND VW.CREATEDBY=" + Userid + "   GROUP BY SECTIONNAME;";
                        }
                        if (strsectionresult == "Monthly")
                        {
                            strInsert = "SELECT  SECTIONNAME,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) AS PAIDAMOUNT FROM VWITEMDISCOUNTDETAILS VW LEFT JOIN TABRMSKOT TK ON  VW.VCHKOTID=TK.VCHKOTID LEFT JOIN TABPOSSECTIONMST TS ON TK.SECTIONID=TS.SECTIONID   WHERE BILLDATE between  current_date-30 and current_date AND VW.CREATEDBY=" + Userid + "   GROUP BY SECTIONNAME;";
                        }
                        if (strsectionresult == "Yearly")
                        {
                            strInsert = "SELECT  SECTIONNAME,round(sum(amount)-sum(itemdicount)+sum(numservicetaxvale)+sum(numservicechargevale),2) AS PAIDAMOUNT FROM VWITEMDISCOUNTDETAILS VW LEFT JOIN TABRMSKOT TK ON  VW.VCHKOTID=TK.VCHKOTID LEFT JOIN TABPOSSECTIONMST TS ON TK.SECTIONID=TS.SECTIONID   WHERE BILLDATE between current_date-365 and current_date  AND VW.CREATEDBY=" + Userid + "   GROUP BY SECTIONNAME;";
                        }
                    }




                    using (NpgsqlCommand cmd = new NpgsqlCommand(strInsert, con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            DashboardDTO objKOTTakingDTO = new DashboardDTO();
                            objKOTTakingDTO.PlanName = Convert.ToString(npgdr["SECTIONNAME"]);
                            objKOTTakingDTO.PaymentAmount = Convert.ToDecimal(npgdr["PAIDAMOUNT"]);

                            lstKOTTakingDTO.Add(objKOTTakingDTO);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ChartList");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstKOTTakingDTO;
        }

        #endregion

    }

}


































































































