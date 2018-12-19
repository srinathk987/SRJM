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
    public class MMSMastersRepository : IMMSMasters
    {
        NpgsqlConnection con = null;
        NpgsqlDataReader npgdr = null;

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
        public int CheckCount(string strCheckValue, string strTableName, string strColumnName)
        {
            string strcount = "SELECT COUNT(*)  FROM " + strTableName + " WHERE upper(" + strColumnName + ")='" + ManageQuote(strCheckValue.Trim().ToUpper()) + "'and statusid=1;";
            int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
            return res;
        }
        public List<ProductMasterDTO> ShowShelf1()
        {
            List<ProductMasterDTO> lstShelf = new List<ProductMasterDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand("select shelfid,shelfname,shelfcode from tabmmsshelfmst where statusid=1 order by shelfname;", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ProductMasterDTO objShelf = new ProductMasterDTO();
                            objShelf.shelfid = npgdr["shelfid"].ToString();
                            objShelf.shelfname = npgdr["shelfname"].ToString();
                            objShelf.shelfcode = npgdr["shelfcode"].ToString();
                            lstShelf.Add(objShelf);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowShelf");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstShelf;


        }

        public string GenerateNextID(string Table, string Column, string FormName)
        {

            string prefix = Convert.ToString(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select trim(upper(vchprefix)) from tabposcode  where upper(vchmastername)='" + FormName.ToUpper().Trim() + "'"));
            string res = Convert.ToString(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT (coalesce(MAX(coalesce(cast(LTRIM(" + Column + ",'" + prefix.Trim() + "')as numeric),0)),0))+1 as nextno FROM  " + Table + " where  " + Column + " like '%" + prefix.Trim() + "%';"));
            return prefix + res;
        }

        #region Product Master...

        public List<ProductMasterDTO> ShowProductdetails()
        {
            List<ProductMasterDTO> lstProductMaster = new List<ProductMasterDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT t2.productid,t2.uomname, productname, t2.productcode, productcategoryid, categoryname, productsubcategoryid, subcategoryname, producttypeid, producttype,COALESCE(numsalerate, 0::numeric) AS numsalerate, storagelocationid, storagelocation, "
                        + "shelfid, shelfname, t2.vchuomid,uomname, minqty, maxqty, vchmeasureduomid, nummeasureduomqty, vchpurchaseuomid, numpurchaseuomconvertion, vchsaleuomid, numsaleuomconvertion,tm.vchuomdescription as PurchaseuomName,tmt.vchuomdescription as SalesuomName FROM tabmmsproductmst t2 left join tabmmsproductuomconversion t1 on  t2.productid=t1.productid left join TABINVUOMMST tm on tm.vchuomid=t1.vchpurchaseuomid  left join TABINVUOMMST tmt on  tmt.vchuomid=t1.vchsaleuomid  where t2.statusid=1 order by productname;", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ProductMasterDTO objProductMaster = new ProductMasterDTO();

                            objProductMaster.productid = Convert.ToInt32(npgdr["productid"]);
                            objProductMaster.productname = npgdr["productname"].ToString();
                            objProductMaster.productcode = npgdr["productcode"].ToString();
                            objProductMaster.CategoryId = npgdr["productcategoryid"].ToString();
                            objProductMaster.SubCategoryId = npgdr["productsubcategoryid"].ToString();
                            objProductMaster.CategoryName = npgdr["categoryname"].ToString();
                            objProductMaster.Subcategory = npgdr["subcategoryname"].ToString();
                            objProductMaster.producttypeid = Convert.ToInt32(npgdr["producttypeid"]);
                            objProductMaster.storagelocationname = Convert.ToString(npgdr["storagelocation"]);
                            objProductMaster.storagelocationid = Convert.ToString(npgdr["storagelocationid"]);
                            objProductMaster.producttypename = npgdr["producttype"].ToString();
                            objProductMaster.shelfname = npgdr["shelfname"].ToString();
                            objProductMaster.numsalerate = Convert.ToDecimal(npgdr["numsalerate"]);
                            string dd = Convert.ToString(npgdr["shelfid"]);

                            if (string.IsNullOrEmpty(dd))
                            {
                                objProductMaster.shelfid = null;
                            }
                            else
                            {
                                objProductMaster.shelfid = npgdr["shelfid"].ToString();
                            }

                            objProductMaster.uomid = npgdr["vchuomid"].ToString();
                            objProductMaster.shelfname = npgdr["shelfname"].ToString();
                            objProductMaster.Minqty = Convert.ToDecimal(npgdr["minqty"]);
                            objProductMaster.Maxqty = Convert.ToDecimal(npgdr["maxqty"]);
                            //objProductMaster.Measureduom = npgdr["vchmeasureduomid"].ToString();


                            //objProductMaster.PurchaseUOMTo = Convert.ToInt32(npgdr["nummeasureduomqty"]);
                            //objProductMaster.SalesuomTo = Convert.ToInt32(npgdr["nummeasureduomqty"]);
                            //objProductMaster.Purchaseuom = npgdr["vchpurchaseuomid"].ToString();
                            //objProductMaster.PurchaseUOMfrom = Convert.ToInt32(npgdr["numpurchaseuomconvertion"]);
                            //objProductMaster.Salesuom = npgdr["vchsaleuomid"].ToString();
                            //objProductMaster.SalesuomFrom = Convert.ToInt32(npgdr["numsaleuomconvertion"]);
                            objProductMaster.MeasuredUOMName = npgdr["uomname"].ToString();

                            //objProductMaster.SalesuomName = npgdr["SalesuomName"].ToString();
                            //objProductMaster.PurchaseuomName = npgdr["PurchaseuomName"].ToString();


                            lstProductMaster.Add(objProductMaster);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowProductMaster");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstProductMaster;

        }
        public List<ProductMasterDTO> ShowProductType1()
        {
            List<ProductMasterDTO> lstProductType = new List<ProductMasterDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand("select producttypeid,producttypename from tabmmsproducttypemst where statusid=1 order by producttypename;", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ProductMasterDTO objProductType = new ProductMasterDTO();
                            objProductType.producttypeid = Convert.ToInt32(npgdr["producttypeid"]);
                            objProductType.producttypename = npgdr["producttypename"].ToString();
                            lstProductType.Add(objProductType);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowProductType");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstProductType;

        }
        public List<ProductMasterDTO> ShowUOM1()
        {
            List<ProductMasterDTO> lstUOM = new List<ProductMasterDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT VCHUOMID,VCHUOMDESCRIPTION FROM TABINVUOMMST  WHERE INTSTATUS=1 ORDER BY VCHUOMDESCRIPTION;", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ProductMasterDTO objUOM = new ProductMasterDTO();
                            objUOM.uomid = Convert.ToString(npgdr["VCHUOMID"]);
                            objUOM.uom = npgdr["VCHUOMDESCRIPTION"].ToString();
                            // objUOM.uomabbreviation = npgdr["uomabbreviation"].ToString();

                            lstUOM.Add(objUOM);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowUOM");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstUOM;

        }
        public List<ProductMasterDTO> ShowStorageLocation1()
        {
            List<ProductMasterDTO> lstStorageLocation = new List<ProductMasterDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand("select storagelocationid,storagelocationname from tabmmsstoragelocationmst where statusid=1 order by storagelocationname;", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ProductMasterDTO objStorageLoca = new ProductMasterDTO();
                            objStorageLoca.storagelocationid = Convert.ToString(npgdr["storagelocationid"]);
                            objStorageLoca.storagelocationname = npgdr["storagelocationname"].ToString();

                            lstStorageLocation.Add(objStorageLoca);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowStorageLocation");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstStorageLocation;

        }
        public List<ProductMasterDTO> ShowShelf1(string Storageid)
        {
            List<ProductMasterDTO> lstShelf = new List<ProductMasterDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    // using (NpgsqlCommand cmd = new NpgsqlCommand(" select shelfid,shelfname,shelfcode from tabmmsshelfmst t1 left join tabmmsstoragelocationmst t2 on t1.storagecode=t2.storagelocationcode where t1.statusid=1 and storagelocationid=" + Storageid + " order by shelfname;", con))
                    // using (NpgsqlCommand cmd = new NpgsqlCommand(" select shelfid,shelfname,shelfcode from tabmmsshelfmst t1 left join tabmmsstoragelocationmst t2 on t1.storagecode=t2.storagelocationcode where t1.statusid=1 and storagelocationid in(" + Storageid + ") order by shelfname;", con))
                    //using (NpgsqlCommand cmd = new NpgsqlCommand("select shelfid,shelfname,shelfcode from tabmmsshelfmst t1 left join tabmmsstoragelocationmst t2 on t1.storagecode=t2.storagelocationcode where t1.statusid=1 and storagelocationid='" + Storageid + "' and shelfid not in (select coalesce(shelfid::int,'0') from tabmmsproductmst  where storagelocationid='" + Storageid + "') order by shelfname;", con))
                    using (NpgsqlCommand cmd = new NpgsqlCommand("select shelfid,shelfname,shelfcode from tabmmsshelfmst t1 left join tabmmsstoragelocationmst t2 on t1.storagecode=t2.storagelocationcode where t1.statusid=1 and storagelocationid='" + Storageid + "' and shelfid::text not in (select distinct coalesce(shelfid::text,'0') from tabmmsproductmst  where storagelocationid='" + Storageid + "') order by shelfname;", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ProductMasterDTO objShelf = new ProductMasterDTO();
                            objShelf.shelfid = npgdr["shelfid"].ToString();
                            objShelf.shelfname = npgdr["shelfname"].ToString();
                            objShelf.shelfcode = npgdr["shelfcode"].ToString();


                            lstShelf.Add(objShelf);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowShelf");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstShelf;


        }

        public int SaveProductMaster(ProductMasterDTO Prodmaster, out string productcode)
        {
            int Count = 0;
            int Exist = 0;
            int productid = 0;
            int sequenceno = 0;
            productcode = string.Empty;
            if (string.IsNullOrEmpty(Prodmaster.SubCategoryId) == true || Prodmaster.SubCategoryId == "0")
            {
                Prodmaster.SubCategoryId = "null";
            }
            if (Prodmaster.shelfid == null)
            {
                Prodmaster.shelfid = "null";
            }
            if (Prodmaster.Minqty == null)
            {
                Prodmaster.Minqty = 0;
            }
            if (Prodmaster.Maxqty == null)
            {
                Prodmaster.Maxqty = 0;
            }
            if (Prodmaster.numsalerate == null)
            {
                Prodmaster.numsalerate = 0;
            }
            try
            {
                string check_exist = "SELECT COUNT(productname) FROM tabmmsproductmst  WHERE  UPPER(productname)='" + Prodmaster.productname.Trim().ToUpper() + "' and statusid=1";
                Exist = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Exist == 0)
                {
                    string naxnumber = "select coalesce(max(productsequence),0) from tabmmsproductmst";



                    sequenceno = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, naxnumber));



                    if (sequenceno == null || sequenceno == 0)
                    {
                        sequenceno = 100001;
                    }
                    else
                    {
                        sequenceno = sequenceno + 1;
                    }

                    //string strCode = string.Empty;
                    //strCode = GenerateNextID("tabmmsproductmst", "productcode", "PRODUCT MASTER");
                    Prodmaster.productcode = GenerateNextID("tabmmsproductmst", "productcode", "PRODUCT MASTER");
                    string strInsert = "INSERT INTO tabmmsproductmst( productname, productcode, productcategoryid, categoryname,productsubcategoryid, subcategoryname, producttypeid, producttype, storagelocationid, storagelocation, shelfid, shelfname, vchuomid, uomname, minqty, maxqty, statusid, createdby, createddate,numsalerate,productsequence)"
                      + "VALUES ('" + ManageQuote(Prodmaster.productname.ToUpper().Trim()) + "','" + Prodmaster.productcode + "','" + Prodmaster.CategoryId + "','" + Prodmaster.CategoryName + "'," + Prodmaster.SubCategoryId + ",'" + Prodmaster.Subcategory + "','" + Prodmaster.producttypeid + "','" + Prodmaster.producttypename + "','1','MAIN STORAGE'," + Prodmaster.shelfid + ",'" + Prodmaster.shelfname + "','" + Prodmaster.uomid + "','" + Prodmaster.uom + "'," + Prodmaster.Minqty + "," + Prodmaster.Maxqty + ",1," + Prodmaster.createdby + ",current_timestamp," + Prodmaster.numsalerate + "," + sequenceno + ") returning productid";
                    productid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert));
                    //string strInsertUOM = "INSERT INTO tabmmsproductuomconversion(productid, productcode, vchmeasureduomid, nummeasureduomqty,vchpurchaseuomid, numpurchaseuomconvertion, vchsaleuomid, numsaleuomconvertion,statusid, createdby, createddate)VALUES ("
                    //    + "" + productid + ",'" + Prodmaster.productcode + "','" + Prodmaster.uomid + "'," + Prodmaster.PurchaseUOMTo + ",'" + Prodmaster.Purchaseuom + "'," + Prodmaster.PurchaseUOMfrom + ",'" + Prodmaster.Salesuom + "'," + Prodmaster.SalesuomFrom + ",1," + Prodmaster.createdby + ",current_timestamp)";
                    //NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsertUOM);
                    Count = 1;
                    productcode = Prodmaster.productcode;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveProductMaster");
            }
            return productid;
        }

        public int UpdateProductdetails(ProductMasterDTO Prodmaster)
        {

            int Exist = 0;
            int Count = 0;
            if (Prodmaster.Minqty == null)
            {
                Prodmaster.Minqty = 0;
            }
            if (Prodmaster.Maxqty == null)
            {
                Prodmaster.Maxqty = 0;
            }
            if (Prodmaster.SubCategoryId == "0")
            {
                Prodmaster.SubCategoryId = null;
            }
            string subcatry = Prodmaster.SubCategoryId;

            try
            {
                string strInsert = "";
                string check_exist = "SELECT COUNT(productname) FROM tabmmsproductmst  WHERE  UPPER(productname)='" + Prodmaster.productname.Trim().ToUpper() + "' and statusid=1 and productid!=" + Prodmaster.productid + "";
                Exist = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Exist == 0)
                {

                    //strCode = GenerateNextID("tabmmsproductmst", "productcode", "PRODUCT MASTER");


                    if (Prodmaster.SubCategoryId == null || Prodmaster.SubCategoryId == "")
                    {
                        strInsert = "UPDATE tabmmsproductmst SET productname='" + ManageQuote(Prodmaster.productname) + "',productcategoryid='" + Prodmaster.CategoryId + "',categoryname='" + Prodmaster.CategoryName + "', productsubcategoryid=null, "
                       + " subcategoryname='" + Prodmaster.Subcategory + "', producttypeid='" + Prodmaster.producttypeid + "',producttype='" + Prodmaster.producttypename + "', storagelocationid='1', storagelocation='MAIN STORAGE', shelfid='" + Prodmaster.shelfid + "', shelfname='" + ManageQuote(Prodmaster.shelfname) + "', vchuomid='" + Prodmaster.uomid + "', uomname='" + Prodmaster.uom + "', minqty=" + Prodmaster.Minqty + ", "
                       + " maxqty=" + Prodmaster.Maxqty + ",numsalerate=" + Prodmaster.numsalerate + ", modifiedby=1, modifieddate=current_timestamp  WHERE productcode='" + Prodmaster.productcode + "';";
                    }
                    else
                    {

                        strInsert = "UPDATE tabmmsproductmst SET productname='" + ManageQuote(Prodmaster.productname) + "',productcategoryid='" + Prodmaster.CategoryId + "',categoryname='" + Prodmaster.CategoryName + "', productsubcategoryid='" + Prodmaster.SubCategoryId + "', "
                        + " subcategoryname='" + Prodmaster.Subcategory + "', producttypeid='" + Prodmaster.producttypeid + "',producttype='" + Prodmaster.producttypename + "', storagelocationid='1', storagelocation='MAIN STORAGE', shelfid='" + Prodmaster.shelfid + "', shelfname='" + ManageQuote(Prodmaster.shelfname) + "', vchuomid='" + Prodmaster.uomid + "', uomname='" + Prodmaster.uom + "', minqty=" + Prodmaster.Minqty + ", "
                        + " maxqty=" + Prodmaster.Maxqty + ",numsalerate=" + Prodmaster.numsalerate + " , modifiedby=1, modifieddate=current_timestamp  WHERE productcode='" + Prodmaster.productcode + "';";
                    }

                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);


                    //string strInsertUOM = "UPDATE tabmmsproductuomconversion SET vchmeasureduomid='" + Prodmaster.uomid + "', nummeasureduomqty=" + Prodmaster.PurchaseUOMTo + ",vchpurchaseuomid='" + Prodmaster.Purchaseuom + "', numpurchaseuomconvertion=" + Prodmaster.PurchaseUOMfrom + ", " +
                    //    " vchsaleuomid='" + Prodmaster.Salesuom + "',numsaleuomconvertion=" + Prodmaster.SalesuomFrom + ", modifiedby=1, modifieddate=current_timestamp WHERE productcode='" + Prodmaster.productcode + "';";
                    //NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsertUOM);
                    Count = 1;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveProductMaster");
            }
            return Count;
        }

        public List<ProductMasterDTO> EditProductMasterdetails(int Productid)
        {
            List<ProductMasterDTO> lstProductMaster = new List<ProductMasterDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT t2.productid, productname, t2.productcode, productcategoryid, categoryname, productsubcategoryid, " +
                       "subcategoryname, producttypeid, producttype, storagelocationid, storagelocation, shelfid, shelfname, vchuomid,uomname, minqty, maxqty,numsalerate, " +
                       "vchmeasureduomid, nummeasureduomqty,vchpurchaseuomid, numpurchaseuomconvertion, vchsaleuomid, numsaleuomconvertion FROM tabmmsproductmst t2 " +
                       "join tabmmsproductuomconversion t1 on  t2.productid=t1.productid where t2.productid=" + Productid + " ", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ProductMasterDTO objProductMaster = new ProductMasterDTO();
                            objProductMaster.productid = Convert.ToInt32(npgdr["productid"]);
                            objProductMaster.productname = npgdr["productname"].ToString();
                            objProductMaster.productcode = npgdr["productcode"].ToString();
                            objProductMaster.CategoryId = npgdr["productcategoryid"].ToString();
                            objProductMaster.SubCategoryId = npgdr["productsubcategoryid"].ToString();
                            objProductMaster.producttypeid = Convert.ToInt32(npgdr["producttypeid"]);
                            objProductMaster.storagelocationid = Convert.ToString(npgdr["storagelocationid"]);
                            objProductMaster.shelfid = npgdr["shelfid"].ToString();
                            objProductMaster.uomid = npgdr["vchuomid"].ToString();
                            objProductMaster.Minqty = Convert.ToDecimal(npgdr["minqty"]);
                            objProductMaster.Maxqty = Convert.ToDecimal(npgdr["maxqty"]);
                            //objProductMaster.Measureduom = npgdr["vchmeasureduomid"].ToString();
                            objProductMaster.Measureduom = npgdr["nummeasureduomqty"].ToString();
                            objProductMaster.productcode = npgdr["productcode"].ToString();
                            objProductMaster.productname = npgdr["productname"].ToString();
                            objProductMaster.productcode = npgdr["productcode"].ToString();
                            objProductMaster.productname = npgdr["productname"].ToString();
                            objProductMaster.productcode = npgdr["productcode"].ToString();
                            objProductMaster.numsalerate = Convert.ToDecimal(npgdr["numsalerate"]);


                            lstProductMaster.Add(objProductMaster);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowProductMaster");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstProductMaster;

        }

        public int DeleteProductMaster(string Productcode)
        {

            int Count = 0;
            int Exist = 0;
            try
            {

                string check_exist = "select count(*) from tabmmsproductvendors where statusid=1 and productcode='" + Productcode + "'";
                Exist = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Exist == 0)
                {

                    string strDelete = @"UPDATE tabmmsproductmst SET STATUSID=2 WHERE productcode='" + Productcode + "';UPDATE tabmmsproductuomconversion SET STATUSID=2 WHERE productcode='" + Productcode + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);
                    Count = 1;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteProductMaster");
                throw;
            }
            finally
            {

            }
            return Count;
        }

        public int CheckGRNCount(string Product)
        {

            int Count = 0;           
            try
            {
                string check_exist = "select count(*) from tabmmsgoodsreceivednotedetails where statusid=1 and UPPER(productcode)='" + Product + "'";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
               
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "CheckGRNCount");
                throw;
            }
            finally
            {

            }
            return Count;
        }








        public List<ProductMasterDTO> vendorProductsMaster(string productname)
        {
            List<ProductMasterDTO> lstVendorProductDTO = new List<ProductMasterDTO>();
            try
            {

                // // productcode = "PM2";
                // npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, " select tv.vendorid,tv.recordid,tv.vchvendorid,tp.productname,tv.numproductcost ,tvv.vchvendorname from tabmmsproductmst tp join tabmmsproductvendors tv on " +
                //" tp.productid=tv.productid left join tabmmsvendormst tvv on tvv.vchvendorcode=tv.vchvendorid  where tv.statusid=1 and tp.productcode='" + productcode + "' order by tvv.vchvendorname ;");
                // // productcode = "PM2";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, " select tv.vchmeasureduomid,tv.vendorid,tv.recordid,tv.vchvendorid,tp.productname,tv.numproductcost ,tvv.vchvendorname from tabmmsproductmst tp join tabmmsproductvendors tv on " +
               " tp.productid=tv.productid left join tabmmsvendormst tvv on tvv.vendorid=tv.vendorid  where tv.statusid=1 and tp.productname='" + productname + "' and tp.statusid=1 order by tvv.vchvendorname ;");
                while (npgdr.Read())
                {
                    ProductMasterDTO objVendorProductDTO = new ProductMasterDTO();
                    objVendorProductDTO.recordid = Convert.ToInt32(npgdr["recordid"]);
                    objVendorProductDTO.vendorid = Convert.ToInt32(npgdr["vendorid"]);
                    objVendorProductDTO.vendorcode = Convert.ToString(npgdr["vchvendorid"]);
                    objVendorProductDTO.productcost = Convert.ToDecimal(npgdr["numproductcost"]);
                    objVendorProductDTO.vendorname = Convert.ToString(npgdr["vchvendorname"]);
                    objVendorProductDTO.productname = Convert.ToString(npgdr["productname"]);
                    objVendorProductDTO.uom = Convert.ToString(npgdr["vchmeasureduomid"]);
                    lstVendorProductDTO.Add(objVendorProductDTO);
                }


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ProductsMaster");
            }
            finally
            {
                npgdr.Dispose();

            }
            return lstVendorProductDTO;
        }

        public bool UpdatevendorProductsinform(VendorDetailsDTO VP)
        {
            Boolean IsSaved = false;
            int Count;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string strInsert = string.Empty;
                string check_exist = "SELECT count(*) FROM tabmmsproductvendors  WHERE recordid='" + VP.recordid + "' and statusid=1";
                Count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));
                if (Count > 0)
                {
                    string vchvendorid = "SELECT vchvendorcode FROM tabmmsvendormst  WHERE vendorid=" + VP.vendorid + "";
                    vchvendorid = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, vchvendorid));

                    strInsert = "update tabmmsproductvendors set vendorid=" + VP.vendorid + ",vchmeasureduomid='" + VP.productuom + "',vchvendorid='" + vchvendorid + "',numproductcost=" + VP.productcost + ",datcostupto=current_date,statusid=1,modifiedby=1,modifieddate=CURRENT_TIMESTAMP where recordid=" + VP.recordid + ";";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                    IsSaved = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Product Master");
                IsSaved = false;
            }
            finally
            {
                if (IsSaved)
                {
                    trans.Commit();
                }
                else
                {
                    trans.Rollback();
                }
            }
            return IsSaved;


        }

        public List<ProductMasterDTO> ShowOrderdetails(string productname)
        {
            List<ProductMasterDTO> lstProductMaster = new List<ProductMasterDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT orderid,vchpurchaseorderno,VCHVENDORNAME,PRODUCTNAME,NUMORDEREDQTY,NUMRATE FROM vwmmsgrnorder where productname= '" + productname + "' and statusid=1 ORDER BY orderid;", con))
                    //using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT orderid,vchpurchaseorderno,VCHVENDORNAME,PRODUCTNAME,NUMORDEREDQTY,NUMRATE FROM VWMMSPURCHASEORDER where productname= '" + productname + "' and vchpurchaseorderstatus='Y' ORDER BY orderid;", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ProductMasterDTO objProductMaster = new ProductMasterDTO();
                            objProductMaster.OrderNo = Convert.ToString(npgdr["vchpurchaseorderno"]);
                            objProductMaster.vendorname = npgdr["VCHVENDORNAME"].ToString();
                            objProductMaster.productname = npgdr["PRODUCTNAME"].ToString();
                            objProductMaster.orderqty = Convert.ToDecimal(npgdr["NUMORDEREDQTY"]);
                            objProductMaster.Rate = Convert.ToDecimal(npgdr["NUMRATE"]);
                            lstProductMaster.Add(objProductMaster);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowProductMaster");
                throw ex;

            }
            finally
            {
                npgdr.Dispose();
            }
            return lstProductMaster;

        }

        public List<ProductSubCategoryDTO> ShowProductSubCategory(string Categoryid)
        {
            List<ProductSubCategoryDTO> lstSubCategory = new List<ProductSubCategoryDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(" SELECT productsubcategoryid,upper(productsubcategoryname) as productsubcategoryname, productsubcategorycode, TPC.productcategoryid,upper(TPC.productcategoryname) as productcategoryname, TPC.productcategorycode FROM tabmmsproductcategorymst TPC  JOIN tabmmsproductsubcategorymst TPSC ON TPC.productcategoryid =TPSC.productcategoryid  WHERE TPSC.STATUSID=1 and TPC.productcategoryid= " + Categoryid + "ORDER BY productsubcategoryname;", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ProductSubCategoryDTO objSubCategory = new ProductSubCategoryDTO();
                            objSubCategory.SubCategoryId = Convert.ToInt32(npgdr["productsubcategoryid"]);
                            objSubCategory.Subcategory = npgdr["productsubcategoryname"].ToString();
                            objSubCategory.SubcategoryCode = Convert.ToString(npgdr["productsubcategorycode"]);
                            objSubCategory.Category = Convert.ToString(npgdr["productcategoryname"]);
                            objSubCategory.CategoryCode = Convert.ToString(npgdr["productcategorycode"]);

                            lstSubCategory.Add(objSubCategory);
                        }
                    }
                    con.Close();
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
            return lstSubCategory;

        }


        public bool SavevendorProductsMaster(VendorProductDTO VP)
        {
            Boolean IsSaved = false;
            string Count = string.Empty;
            int Count_exist = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string strInsert = string.Empty;
                string check_exist = "SELECT VCHVENDORCODE FROM TABMMSVENDORMST  WHERE  vendorid='" + VP.vendorid + "' and statusid=1";
                Count = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));

                string check = "SELECT count(*) FROM tabmmsproductvendors  WHERE  vendorid='" + VP.vendorid + "' and statusid=1 and productid=" + VP.productid + "";
                Count_exist = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check));

                if (Count_exist == 0)
                {
                    strInsert = "INSERT INTO tabmmsproductvendors(VENDORID,VCHVENDORID,productid,productcode,vchmeasureduomid,numproductcost,datcostupto,statusid,CREATEDBY,CREATEDDATE)VALUES(" + VP.vendorid + ",'" + ManageQuote(Count) + "','" + VP.productid + "','" + ManageQuote(VP.productcode) + "','" + VP.productuom + "'," + VP.productcost + ",current_date,1,1,CURRENT_TIMESTAMP)";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                    trans.Commit();
                    con.Close();
                    con.ClearPool();
                    con.Dispose();
                    IsSaved = true;
                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "SavevendorProducts");
                IsSaved = false;
            }
            finally
            {

            }
            return IsSaved;
        }
        #endregion



        #region Product Category...

        public List<ProductCategoryDTO> ShowProductCategory()
        {
            List<ProductCategoryDTO> lstProductCategory = new List<ProductCategoryDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand("select productcategoryid, upper(productcategoryname) as productcategoryname, productcategorycode from tabmmsproductcategorymst where statusid=1 order by productcategoryname;", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ProductCategoryDTO objProductCategory = new ProductCategoryDTO();
                            objProductCategory.CategoryId = Convert.ToInt32(npgdr["productcategoryid"]);
                            objProductCategory.CategoryName = npgdr["productcategoryname"].ToString();
                            objProductCategory.CategoryCode = Convert.ToString(npgdr["productcategorycode"]);
                            lstProductCategory.Add(objProductCategory);
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
            return lstProductCategory;

        }
        public int SaveProductCategory(ProductCategoryDTO ProductCategory)
        {
            int Count = 0;
            try
            {

                string strCode = GenerateNextID("tabmmsproductcategorymst", "productcategorycode", "ItemCategory");
                string check_exist = "SELECT COUNT(productcategoryname) FROM tabmmsproductcategorymst  WHERE  UPPER(productcategoryname)='" + ManageQuote(ProductCategory.CategoryName.Trim().ToUpper()) + "'   and  statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));

                if (Count == 0)
                {
                    string strInsert = "INSERT INTO tabmmsproductcategorymst(productcategoryname, productcategorycode,statusid, createdby, createddate) VALUES ('" + ManageQuote(ProductCategory.CategoryName.ToUpper()) + "','" + strCode + "'," + 1 + ", '" + ProductCategory.createdby + "', Current_timestamp)";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveProductCategory");
                throw;

            }

            return Count;
        }
        public int UpdateProductCategory(ProductCategoryDTO Ic)
        {
            int Count = 0;
            try
            {
                // Count = ProductCategoryCheck(Ic.CategoryId, "tabmmsproductsubcategorymst", "productcategoryid");
                string strcount = "select count(*) from tabmmsproductcategorymst where upper(productcategoryname)='" + ManageQuote(Ic.CategoryName.ToUpper()) + "' and statusid=1 and  productcategoryid <>" + Ic.CategoryId + ";";

                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
                if (Count == 0)
                {
                    string strInsert = "UPDATE tabmmsproductcategorymst   SET  productcategoryname='" + ManageQuote(Ic.CategoryName.ToUpper()) + "', productcategorycode='" + ManageQuote(Ic.CategoryCode) + "', modifieddate=current_timestamp,modifiedby=1 WHERE productcategoryid='" + Ic.CategoryId + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateProductCategory");
                throw;
            }


            return Count;
        }
        public int ProductCategoryCheck(int strCheckValue, string strTableName, string strColumnName)
        {
            string strcount = "SELECT COUNT(*)  FROM " + strTableName + " WHERE " + strColumnName + "=" + strCheckValue + " and statusid=1;";
            int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
            return res;
        }
        public bool DeleteProductCategory(int categoryid)
        {
            bool IsValid = false;
            try
            {
                string strcount = "select count(productcategoryid) from tabmmsproductsubcategorymst where productcategoryid=" + categoryid + " and statusid=1;";
                Int32 Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
                if (Count == 0)
                {
                    string strQuery = "UPDATE tabmmsproductcategorymst SET STATUSID=2 WHERE productcategoryid='" + categoryid + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery);
                    IsValid = true;
                }
                else
                {

                    IsValid = false;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteProductCategory");
                throw;
            }

            return IsValid;
        }

        public int CheckProductCategory(ProductCategoryDTO Ic)
        {
            int Count = 0;
            int Count1 = 0;
            try
            {
                string strcount = "SELECT COUNT(*)  FROM tabmmsproductsubcategorymst WHERE productcategoryid='" + Ic.CategoryId + "' and statusid=1;";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
                string strcount1 = "SELECT COUNT(*)  FROM tabmmsproductmst WHERE productcategoryid='" + Ic.CategoryId + "' and statusid=1;";
                Count1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount1));
                if (Count == 0 && Count1 == 0)
                {
                    Count = 1;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateProductCategory");
                throw;
            }


            return Count;
        }

        #endregion

        #region Product Subcategory...

        public List<ProductSubCategoryDTO> ShowProductSubCategory()
        {
            List<ProductSubCategoryDTO> lstSubCategory = new List<ProductSubCategoryDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(" SELECT productsubcategoryid,upper(productsubcategoryname) as productsubcategoryname, productsubcategorycode, TPC.productcategoryid,upper(TPC.productcategoryname) as productcategoryname, TPC.productcategorycode FROM tabmmsproductcategorymst TPC  JOIN tabmmsproductsubcategorymst TPSC ON TPC.productcategoryid =TPSC.productcategoryid  WHERE TPSC.STATUSID=1 ORDER BY productsubcategoryname;", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ProductSubCategoryDTO objSubCategory = new ProductSubCategoryDTO();
                            objSubCategory.SubCategoryId = Convert.ToInt32(npgdr["productsubcategoryid"]);
                            objSubCategory.Subcategory = npgdr["productsubcategoryname"].ToString();
                            objSubCategory.SubcategoryCode = Convert.ToString(npgdr["productsubcategorycode"]);
                            objSubCategory.Category = Convert.ToString(npgdr["productcategoryname"]);
                            objSubCategory.CategoryCode = Convert.ToString(npgdr["productcategorycode"]);

                            lstSubCategory.Add(objSubCategory);
                        }
                    }
                    con.Close();
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
            return lstSubCategory;

        }
        public int SaveProductSubCategory(ProductSubCategoryDTO Isc)
        {

            int Count = 0;
            try
            {
                string strCode = GenerateNextID("tabmmsproductsubcategorymst", "productsubcategorycode", "ItemSubCategory");
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT COUNT(productsubcategoryname) FROM tabmmsproductsubcategorymst  WHERE  UPPER(productsubcategoryname)='" + ManageQuote(Isc.Subcategory.Trim().ToUpper()) + "' and statusid=1"));
                if (Count == 0)
                {
                    string strInsert = "INSERT INTO tabmmsproductsubcategorymst(productsubcategoryname, productsubcategorycode,productcategoryid, statusid, createdby, createddate)  VALUES ('" + ManageQuote(Isc.Subcategory.ToUpper()) + "','" + strCode + "'," + Isc.CategoryId + "," + 1 + ", '" + Isc.createdby + "', Current_timestamp)";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveSubCategory");
                throw;

            }

            return Count;
        }
        public int UpdateProductSubCategory(ProductSubCategoryDTO Isc)
        {
            int Count = 0;
            try
            {
                string strcount = "select count(*) from tabmmsproductsubcategorymst where upper(productsubcategoryname)='" + ManageQuote(Isc.Subcategory.ToUpper()) + "' and statusid=1 and  productsubcategoryid <>" + Isc.SubCategoryId + ";";

                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
                if (Count == 0)
                {
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "UPDATE tabmmsproductsubcategorymst   SET productcategoryid=" + Isc.CategoryId + ",  productsubcategoryname='" + ManageQuote(Isc.Subcategory) + "', productsubcategorycode='" + ManageQuote(Isc.SubcategoryCode) + "', modifieddate=current_timestamp,modifiedby=1 WHERE productsubcategoryid='" + Isc.SubCategoryId + "';");
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateSubCategory");
                throw;
            }

            return Count;
        }
        public bool DeleteSubCategory(int SubCategoryId, string Subcategory)
        {
            bool IsValid = false;
            try
            {
                Int32 exists = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(subcategoryname) from tabmmsproductmst where subcategoryname='" + Subcategory + "' and statusid=1"));
                //int res = ProductCategoryCheck(SubCategoryId, "tabpositemmst", "subcategoryid");
                if (exists == 0)
                {
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "UPDATE tabmmsproductsubcategorymst SET STATUSID=2 WHERE productsubcategoryid='" + SubCategoryId + "';");

                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteSubCategory");
                throw;
            }

            return IsValid;
        }



        //public int CheckProductSubCategory(ProductSubCategoryDTO Isc)
        //{
        //    int Count = 0;
        //    try
        //    {
        //        string strcount = "SELECT COUNT(*)  FROM tabmmsproductsubcategorymst WHERE productcategoryid='" + Isc.CategoryId + "' and statusid=1;";
        //        Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "UpdateProductCategory");
        //        throw;
        //    }


        //    return Count;
        //}

        #endregion

        #region   TAX DETAILS...

        public List<ProductTaxDTO> ShowTaxDetails()
        {
            List<ProductTaxDTO> lstTaxDetails = new List<ProductTaxDTO>();
            try
            {
                string strlocation = "SELECT TAXID,upper(TAXNAME) as TAXNAME,NUMPERCENTAGE,STATUSID FROM tabmmstaxmst WHERE STATUSID=1 ORDER BY TAXNAME;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strlocation);
                while (npgdr.Read())
                {
                    ProductTaxDTO objTaxDTO = new ProductTaxDTO();
                    objTaxDTO.TaxId = Convert.ToInt32(npgdr["TAXID"]);
                    objTaxDTO.TaxName = npgdr["TAXNAME"].ToString();
                    objTaxDTO.Percentage = npgdr["NUMPERCENTAGE"].ToString();
                    lstTaxDetails.Add(objTaxDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Tax");
                throw ex;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstTaxDetails;

        }
        public int SaveTax(ProductTaxDTO tx)
        {
            int Count = 0;
            try
            {

                string check_exist = "SELECT COUNT(TAXNAME) FROM tabmmstaxmst  WHERE  upper(TAXNAME)='" + ManageQuote(tx.TaxName.Trim().ToUpper()) + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strInsert = "INSERT INTO tabmmstaxmst(TAXNAME, NUMPERCENTAGE, STATUSID, CREATEDBY, CREATEDDATE)  VALUES ('" + ManageQuote(tx.TaxName.ToUpper()) + "', '" + tx.Percentage + "',  1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);

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
            return Count;
        }
        public int CountUpdateTax(string name)
        {

            int Count = 0;
            try
            {

                //Count = CheckCount(name, "tabmmstaxmst", "taxtype");

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
        public int UpdateTax(ProductTaxDTO tx)
        {
            int Count = 0;
            try
            {

                //Count = CheckCount(tx.TaxName, "tabmmstaxmst", "taxtype");
                //if (Count == 0)
                //{
                string strInsert = "UPDATE tabmmstaxmst SET  taxname='" + ManageQuote(tx.TaxName) + "', numpercentage='" + ManageQuote(tx.Percentage) + "',  modifieddate=current_timestamp,modifiedby=1 WHERE taxid='" + tx.TaxId + "';";
                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                // }
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
                //int res = CheckCount(name, "tabmmstaxmst", "taxtype");
                //if (res == 0)
                //{
                string strDelete = "UPDATE tabmmstaxmst SET STATUSID=2 WHERE TAXID='" + id + "';";
                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);
                IsValid = true;
                //}


            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }
            return IsValid;
        }
        #endregion

        #region Country...
        public bool SaveCountry(CountryDTO objCountry)
        {
            Boolean isvalid = false;
            try
            {

                int count = check_Existed(objCountry);
                if (count == 0)
                {
                    string strSave = "INSERT INTO tabcountry(countryname, statusid, createdby, createddate) VALUES ('" + objCountry.CountryName.Trim().ToUpper() + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strSave);
                    isvalid = true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return isvalid;
        }
        private int check_Existed(CountryDTO objCountry)
        {
            try
            {
                string check_exist = "SELECT COUNT(countryname) FROM tabcountry  WHERE   statusid=1 and countryname= '" + objCountry.CountryName.Trim().ToUpper() + "'";
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CountryDTO> ShowCountryDetails()
        {
            List<CountryDTO> lstCountry = new List<CountryDTO>();
            DataSet ds = new DataSet();
            try
            {
                string strGetCountry = "select countryid,upper(countryname) as countryname from tabcountry  where statusid=1 order by countryname;";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strGetCountry);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        CountryDTO objC = new CountryDTO();
                        objC.CountryId = Convert.ToInt32(ds.Tables[0].Rows[i]["countryid"]);
                        objC.CountryName = ds.Tables[0].Rows[i]["countryname"].ToString();
                        lstCountry.Add(objC);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                ds.Dispose();
            }
            return lstCountry;
        }
        public bool UpdateCountry(CountryDTO objCountry)
        {
            bool isUpdate = false;
            try
            {
                string strcount = "select count(*) from tabcountry where  upper(countryname)='" + ManageQuote(objCountry.CountryName.ToUpper()) + "' and statusid=1 and  countryid <>" + objCountry.CountryId + ";";
                int count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));

                if (count == 0)
                {
                    string update = "UPDATE tabcountry SET countryname='" + objCountry.CountryName.Trim().ToUpper() + "', MODIFIEDBY=1, MODIFIEDDATE=CURRENT_TIMESTAMP WHERE countryid=" + objCountry.CountryId + "";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, update);
                    isUpdate = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return isUpdate;
        }
        public bool DeleteCountry(int countryid)
        {
            bool isDelete = false;
            try
            {

                string delete = "UPDATE tabcountry SET STATUSID=2, MODIFIEDBY=1, MODIFIEDDATE=CURRENT_TIMESTAMP WHERE countryid=" + countryid + "";
                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, delete);
                isDelete = true;

            }
            catch (Exception)
            {

                throw;
            }
            return isDelete;
        }
        private int check_Existedcountry(int countryid)
        {
            try
            {
                string check_exist = "select count(statename) from tabstate where countryid=" + countryid + " and statusid=1;";
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int CheckCountry(CountryDTO objCountry)
        {
            int isUpdate = 0;
            try
            {
                int numcount1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabcity where countryid=" + objCountry.CountryId + " and statusid=1; "));
                int numcount2 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabstate where countryid=" + objCountry.CountryId + " and statusid=1;"));
                int numcount3 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabmmsvendoraddressmst where countryid=" + objCountry.CountryId + " and statusid=1;"));
                //int numcount4 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabstate where countryid=" + objCountry.CountryId + " and statusid=1;"));
                if (numcount1 == 0 && numcount2 == 0 && numcount3 == 0)
                {
                    isUpdate = 1;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return isUpdate;
        }

        #endregion

        #region State

        public bool SaveState(StateDTO objState)
        {
            Boolean isvalid = false;
            try
            {

                int count = check_Existed(objState);
                if (count == 0)
                {
                    string strSave = "INSERT INTO tabstate(statename, countryid,statusid, createdby, createddate) VALUES ('" + objState.state.Trim().ToUpper() + "','" + objState.CountryId + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strSave);
                    isvalid = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "State");
            }
            return isvalid;
        }
        private int check_Existed(StateDTO objState)
        {
            try
            {
                string check_exist = "SELECT COUNT(statename) FROM tabstate  WHERE statusid=1 and statename= '" + objState.state.Trim().ToUpper() + "' and countryid=" + objState.CountryId + " and stateid<>" + objState.StateId + "";
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<StateDTO> BindState()
        {
            List<StateDTO> lstState = new List<StateDTO>();
            try
            {
                NpgsqlDataAdapter da = new NpgsqlDataAdapter("SELECT STATENAME,ST.STATEID,ST.STATUSID,CO.COUNTRYID,CO.COUNTRYNAME FROM TABSTATE ST LEFT JOIN TABCOUNTRY CO  ON ST.COUNTRYID=CO.COUNTRYID WHERE ST.STATUSID=1 order by CO.COUNTRYNAME,STATENAME;", NPGSqlHelper.SQLConnString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    StateDTO objState = new StateDTO();

                    objState.StateId = Convert.ToInt32(dr["STATEID"].ToString());
                    objState.Country = dr["countryname"].ToString();
                    objState.state = dr["statename"].ToString();
                    objState.CountryId = (dr["countryid"].ToString());
                    lstState.Add(objState);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "State");
            }
            return lstState;
        }
        public bool UpdateState(StateDTO objState)
        {
            Boolean IsValid = false;
            try
            {
                int count = check_Existed(objState);
                if (count == 0)
                {
                    string strUpdate = "update tabstate set statename='" + ManageQuote(objState.state.ToUpper()) + "',countryid=" + objState.CountryId + ",modifiedby='1',modifieddate=current_timestamp where stateid=" + objState.StateId + " ";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strUpdate);
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "State");
                throw;
            }
            finally
            {

            }
            return IsValid;

        }
        public bool DeleteState(int stateid, string countryid)
        {
            bool IsValid = false;
            try
            {
                int count = check_Existedstate(stateid, countryid);
                if (count == 0)
                {
                    string strDelete = "update tabstate  set statusid=2 where stateid=" + stateid + "";
                    int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);
                    IsValid = true;
                }


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "State");
                throw;
            }

            return IsValid;
        }
        private int check_Existedstate(int stateid, string countryid)
        {
            try
            {
                string check_exist = "select count(cityname) from tabcity where statusid=1 and stateid=" + stateid + " and countryid=" + countryid + ";";
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int CheckState(StateDTO objState)
        {
            int IsValid = 0;
            try
            {
                int numcount1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsemployeeaddressdetails where vchstate='" + ManageQuote(objState.state) + "' and statusid=1;"));
                int numcount2 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabcity where stateid=" + objState.StateId + " and statusid=1;"));
                if (numcount1 == 0 && numcount2 == 0)
                {
                    IsValid = 1;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "State");
                throw;
            }
            finally
            {

            }
            return IsValid;

        }

        #endregion

        #region City Details

        public bool CreateCity(City City)
        {
            bool IsValid = false;
            try
            {
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabcity where countryid=" + City.CountryId + " and stateid=" + City.StateId + " and upper(cityname)='" + ManageQuote(City.CityName.ToUpper()) + "' and statusid=1;"));
                if (count == 0)
                {
                    string strCity = "insert into tabcity(cityname,stateid,countryid,statusid,createdby,createddate) values('" + ManageQuote(City.CityName.ToUpper()) + "','" + City.StateId + "','" + City.CountryId + "',1,1,current_timestamp) ";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strCity);
                    IsValid = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw ex;

            }
            finally
            {

            }
            return IsValid;
        }

        public List<City> ShowCountry()
        {
            List<City> lstcountry = new List<City>();
            DataSet ds = new DataSet();
            try
            {

                string strcountry = "select countryid,countryname from tabcountry where statusid=1 order by countryname ;";

                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        City objcountry = new City();

                        objcountry.CountryId = ds.Tables[0].Rows[i]["countryid"].ToString();
                        objcountry.Country = ds.Tables[0].Rows[i]["countryname"].ToString();

                        lstcountry.Add(objcountry);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");

                throw;
            }
            finally
            {

                ds.Dispose();
            }
            return lstcountry;
        }

        public List<City> ShowStates(string strcountryid)
        {
            List<City> lstStates = new List<City>();
            DataSet ds = new DataSet();
            try
            {

                string strcountry = "select stateid,statename from tabstate where countryid='" + strcountryid + "'  and statusid=1 order by statename;";

                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        City objstate = new City();

                        objstate.StateId = ds.Tables[0].Rows[i]["stateid"].ToString();
                        objstate.State = ds.Tables[0].Rows[i]["statename"].ToString();

                        lstStates.Add(objstate);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "State");
                throw;
            }
            finally
            {
                ds.Dispose();
            }
            return lstStates;
        }

        public List<City> BindCityDetails()
        {
            List<City> lstcitydetails = new List<City>();
            DataSet ds = new DataSet();
            try
            {

                // ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select tsc.countryid,countryname,tsc.stateid,statename,cityid,cityname from tabcity tc join (SELECT tco.countryid,stateid,countryname,statename from tabstate ts join tabcountry tco on ts.countryid=tco.countryid)tsc on tc.countryid=tsc.countryid and tc.stateid=tsc.stateid where statusid=1 order by cityid desc;");
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select tsc.countryid,countryname,tsc.stateid,statename,cityid,cityname from tabcity tc join (SELECT tco.countryid,stateid,countryname,statename from tabstate ts join tabcountry tco on ts.countryid=tco.countryid)tsc on tc.countryid=tsc.countryid and tc.stateid=tsc.stateid where statusid=1 order by countryname,statename,cityname;");
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        City objcityDTO = new City();

                        objcityDTO.Country = Convert.ToString(ds.Tables[0].Rows[i]["countryname"]);
                        objcityDTO.CountryId = Convert.ToString(ds.Tables[0].Rows[i]["countryid"]);
                        objcityDTO.State = Convert.ToString(ds.Tables[0].Rows[i]["statename"]);
                        objcityDTO.StateId = Convert.ToString(ds.Tables[0].Rows[i]["stateid"]);
                        objcityDTO.CityName = Convert.ToString(ds.Tables[0].Rows[i]["cityname"]);
                        objcityDTO.CityId = Convert.ToString(ds.Tables[0].Rows[i]["cityid"]);

                        lstcitydetails.Add(objcityDTO);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }

            finally
            {
                ds.Dispose();
            }
            return lstcitydetails;
        }

        public bool UpdateCity(City objcity)
        {
            bool IsValid = false;
            try
            {
                //if (CheckCity(objcity))
                //{
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabcity where countryid=" + objcity.CountryId + " and stateid=" + objcity.StateId + " and upper(cityname)='" + ManageQuote(objcity.CityName.ToUpper()) + "' and statusid=1 and cityid<>" + objcity.CityId + ";"));
                if (count == 0)
                {
                    string strupdate = "update tabcity set cityname='" + ManageQuote(objcity.CityName.ToUpper()) + "',stateid=" + objcity.StateId + ",countryid=" + objcity.CountryId + " ,statusid=1,modifiedby=1,modifieddate=current_timestamp where cityid=" + objcity.CityId + ";";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strupdate);
                    IsValid = true;
                }
                //}
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }
            finally
            {

            }
            return IsValid;

        }

        public bool DeleteCity(City citydto)
        {
            bool IsValid = false;
            try
            {
                // if (CheckCity(citydto))
                //{
                string strDelete = "update tabcity set statusid=2,modifiedby=1,modifieddate=current_timestamp where cityid=" + citydto.CityId + ";";
                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);
                if (res == 1)
                {
                    IsValid = true;
                }

                //}
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        public int CheckCity(City objcity)
        {
            int IsValid = 0;
            try
            {
                int numcount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabmmsvendoraddressmst where cityid='" + ManageQuote(objcity.CityId) + "' and statusid=1;"));
                if (numcount == 0)
                {
                    IsValid = 1;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }
            finally
            {

            }
            return IsValid;

        }


        #endregion

        #region PRODUCTTYPE...
        public List<ProductTypeDTO> ShowProductType()
        {
            List<ProductTypeDTO> lstProductTypedetails = new List<ProductTypeDTO>();
            try
            {

                string strInsert = "SELECT PRODUCTTYPEID,PRODUCTTYPENAME,PRODUCTTYPECODE FROM TABMMSPRODUCTTYPEMST WHERE STATUSID=1 ORDER BY PRODUCTTYPEID desc;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                while (npgdr.Read())
                {
                    ProductTypeDTO objProductTypeDTO = new ProductTypeDTO();
                    objProductTypeDTO.RecordId = Convert.ToInt32(npgdr["PRODUCTTYPEID"]);
                    objProductTypeDTO.ProductTypeName = npgdr["PRODUCTTYPENAME"].ToString();
                    objProductTypeDTO.ProductTypeCode = Convert.ToString(npgdr["PRODUCTTYPECODE"]);
                    lstProductTypedetails.Add(objProductTypeDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowProductType");
                throw ex;
                // ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstProductTypedetails;

        }
        public int SaveProductType(ProductTypeDTO PT)
        {

            int Count = 0;
            try
            {

                string check_exist = "SELECT COUNT(PRODUCTTYPENAME) FROM TABMMSPRODUCTTYPEMST  WHERE  UPPER(PRODUCTTYPENAME)='" + ManageQuote(PT.ProductTypeName.ToUpper()) + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strCode = string.Empty;
                    strCode = GenerateNextID("TABMMSPRODUCTTYPEMST", "PRODUCTTYPECODE", "PRODUCT TYPE");
                    //if (Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "Select Count(*) from TABMMSPRODUCTTYPEMST;")) > 0)
                    //{
                    //    strCode = Convert.ToString(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select 'PC'||MAX(coalesce((replace(PRODUCTTYPECODE,'PC','')::INTEGER),0))::INTEGER+1 from TABMMSPRODUCTTYPEMST Order by 1;"));
                    //}
                    //else
                    //{
                    //    strCode = "PC1";
                    //}
                    string strInsert = @"INSERT INTO TABMMSPRODUCTTYPEMST(PRODUCTTYPENAME,PRODUCTTYPECODE,STATUSID,createdby,createddate)VALUES('" + ManageQuote(PT.ProductTypeName.ToUpper()) + "','" + strCode + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveProductType");
            }
            return Count;
        }
        public int UpdateProductType(ProductTypeDTO PT)
        {
            int Count = 0;
            try
            {
                //string check_exist = "SELECT COUNT(PRODUCTTYPENAME) FROM TABMMSPRODUCTTYPEMST  WHERE  UPPER(PRODUCTTYPENAME)='" + PT.ProductTypeName.Trim().ToUpper() + "' and   UPPER(PRODUCTTYPECODE)='" + PT.ProductTypeCode.Trim().ToUpper() + "' and PRODUCTTYPEID=" + PT.RecordId + " and statusid=1";
                //string check_exist = "SELECT COUNT(PRODUCTTYPENAME) FROM TABMMSPRODUCTTYPEMST  WHERE PRODUCTTYPEID=" + PT.RecordId + " and statusid=1";
                string check_exist = "select count(*) from TABMMSPRODUCTTYPEMST where upper(PRODUCTTYPENAME)='" + ManageQuote(PT.ProductTypeName.ToUpper()) + "' and statusid=1 and  PRODUCTTYPEID <>" + PT.RecordId + ";";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strInsert = @"UPDATE TABMMSPRODUCTTYPEMST SET PRODUCTTYPECODE='" + ManageQuote(PT.ProductTypeCode) + "',PRODUCTTYPENAME='" + ManageQuote(PT.ProductTypeName.ToUpper()) + "',modifieddate=current_timestamp,modifiedby=1 WHERE PRODUCTTYPEID='" + PT.RecordId + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    Count = 0;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateProductType");
                throw;
            }

            return Count;
        }
        public bool DeleteProductType(int RecordId)
        {
            bool IsValid = false;
            int Count = 0;
            try
            {
                //string check_exist = "SELECT COUNT(PRODUCTTYPENAME) FROM TABMMSPRODUCTTYPEMST  WHERE PRODUCTTYPEID=" + RecordId + " and statusid=1";
                //Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                //if (Count > 0)
                //{
                string strDelete = @"UPDATE TABMMSPRODUCTTYPEMST SET STATUSID=2 WHERE PRODUCTTYPEID='" + RecordId + "';";
                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);
                IsValid = true;
                // }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteProductType");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        public int CheckProducttype(ProductTypeDTO PT)
        {

            int Count = 0;
            int Count1 = 0;
            try
            {

                string strcount1 = "SELECT COUNT(*)  FROM tabmmsproductmst WHERE producttypeid='" + PT.RecordId + "' and statusid=1;";
                Count1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount1));
                if (Count1 == 0)
                {
                    Count = 1;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "CheckProducttype");
                throw;
            }


            return Count;
        }
        #endregion

        #region StorageLocation...
        public List<StorageLocationDTO> ShowStorageLocation()
        {
            List<StorageLocationDTO> lstStorageLocationdetails = new List<StorageLocationDTO>();
            try
            {

                string strInsert = "SELECT STORAGELOCATIONID,STORAGELOCATIONNAME,STORAGELOCATIONCODE FROM TABMMSSTORAGELOCATIONMST WHERE STATUSID=1 ORDER BY STORAGELOCATIONNAME;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                while (npgdr.Read())
                {
                    StorageLocationDTO objStorageLocationDTO = new StorageLocationDTO();
                    objStorageLocationDTO.RecordId = Convert.ToInt32(npgdr["StorageLocationID"]);
                    objStorageLocationDTO.StorageLocationName = npgdr["StorageLocationNAME"].ToString();
                    objStorageLocationDTO.StorageLocationCode = Convert.ToString(npgdr["StorageLocationCODE"]);
                    lstStorageLocationdetails.Add(objStorageLocationDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowStorageLocation");
                throw ex;
                // ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstStorageLocationdetails;

        }
        public int SaveStorageLocation(StorageLocationDTO SL)
        {

            int Count = 0;
            try
            {

                string check_exist = "SELECT COUNT(STORAGELOCATIONNAME) FROM TABMMSSTORAGELOCATIONMST  WHERE  UPPER(STORAGELOCATIONNAME)='" + ManageQuote(SL.StorageLocationName.ToUpper()) + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strCode = string.Empty;
                    strCode = GenerateNextID("TABMMSSTORAGELOCATIONMST", "STORAGELOCATIONCODE", "STORAGE LOCATION");
                    //if (Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "Select Count(*) from TABMMSSTORAGELOCATIONMST;")) > 0)
                    //{
                    //    strCode = Convert.ToString(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select 'SL'||MAX(coalesce((replace(STORAGELOCATIONCODE,'SL','')::INTEGER),0))::INTEGER+1 from TABMMSSTORAGELOCATIONMST Order by 1;"));
                    //}
                    //else
                    //{
                    //    strCode = "SL1";
                    //}
                    string strInsert = @"INSERT INTO TABMMSSTORAGELOCATIONMST(STORAGELOCATIONNAME,STORAGELOCATIONCODE,STATUSID,createdby,createddate)VALUES('" + ManageQuote(SL.StorageLocationName.ToUpper()) + "','" + strCode + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveStorageLocation");
            }
            return Count;
        }
        public int UpdateStorageLocation(StorageLocationDTO SL)
        {
            int Count = 0;
            try
            {
                //string check_exist = "SELECT COUNT(StorageLocationNAME) FROM TABMMSStorageLocationMST  WHERE  UPPER(StorageLocationNAME)='" + PT.StorageLocationName.Trim().ToUpper() + "' and   UPPER(StorageLocationCODE)='" + PT.StorageLocationCode.Trim().ToUpper() + "' and StorageLocationID=" + PT.RecordId + " and statusid=1";
                // string check_exist = "SELECT COUNT(StorageLocationNAME) FROM TABMMSStorageLocationMST  WHERE StorageLocationID=" + SL.RecordId + " and statusid=1";
                string check_exist = "select count(*) from TABMMSSTORAGELOCATIONMST where upper(StorageLocationNAME)='" + ManageQuote(SL.StorageLocationName.ToUpper()) + "' and statusid=1 and  StorageLocationID <>" + SL.RecordId + ";";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strInsert = @"UPDATE TABMMSSTORAGELOCATIONMST SET STORAGELOCATIONCODE='" + ManageQuote(SL.StorageLocationCode) + "',STORAGELOCATIONNAME='" + ManageQuote(SL.StorageLocationName.ToUpper()) + "',modifieddate=current_timestamp,modifiedby=1 WHERE STORAGELOCATIONID='" + SL.RecordId + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    Count = 0;
                }


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateStorageLocation");
                throw;
            }

            return Count;
        }
        public bool DeleteStorageLocation(int RecordId)
        {
            bool IsValid = false;
            // int Count = 0;
            try
            {
                //string check_exist = "SELECT COUNT(STORAGELOCATIONNAME) FROM TABMMSSTORAGELOCATIONMST  WHERE STORAGELOCATIONID=" + RecordId + " and statusid=1";
                //Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                //if (Count > 0)
                //{
                string strDelete = @"UPDATE TABMMSSTORAGELOCATIONMST SET STATUSID=2 WHERE STORAGELOCATIONID='" + RecordId + "';";
                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);
                IsValid = true;
                //}
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteStorageLocation");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        public int CheckStorageLocation(StorageLocationDTO SL)
        {
            int Count = 0;
            int Count1 = 0;
            int Count2 = 0;
            try
            {
                string strcount = "select count(*) from tabmmsshelfmst where storagecode='" + ManageQuote(SL.StorageLocationCode) + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
                string strcount1 = "select count(*) from tabmmsproductmst where storagelocationid='" + SL.RecordId + "' and statusid=1";
                Count1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount1));
                string strcount2 = "select count(*) from tabmmsgoodsreceivednotedetails   where storagelocationid='" + SL.RecordId + "' and statusid=1";
                Count2 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount2));
                if (Count == 0 && Count1 == 0 && Count2 == 0)
                {
                    Count = 1;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateProductCategory");
                throw;
            }


            return Count;
        }
        #endregion

        #region Vendor Details
        public DataTable ShowVendordetails()
        {
            DataTable dt = new DataTable();
            try
            {

                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select vendorid,vchvendorcode, vchvendorname from tabmmsvendormst where status=1;").Tables[0];
                //while (npgdr.Read())
                //{
                //    VendorDetailsDTO objVendorDetailsDTO = new VendorDetailsDTO();
                //    objVendorDetailsDTO.VendorId = Convert.ToInt32(npgdr["vendorid"]);
                //    objVendorDetailsDTO.VendorName = npgdr["vchvendorname"].ToString();
                //    objVendorDetailsDTO.VendorCode = npgdr["vchvendorcode"].ToString();
                //    lstVendorDetailsDTO.Add(objVendorDetailsDTO);
                //}
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowCountry");
            }
            return dt;
        }
        public List<VendorDetailsDTO> GetvendornamesComboData()
        {
            List<VendorDetailsDTO> lstVendorDetailsDTO = new List<VendorDetailsDTO>();
            try
            {
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "select vendorid, vchvendorname from tabmmsvendormst where statusid=1 order by vchvendorname;");
                while (npgdr.Read())
                {
                    VendorDetailsDTO objVendorDetailsDTO = new VendorDetailsDTO();
                    objVendorDetailsDTO.vendorid = Convert.ToInt32(npgdr["vendorid"]);
                    objVendorDetailsDTO.vendorname = npgdr["vchvendorname"].ToString();
                    lstVendorDetailsDTO.Add(objVendorDetailsDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "GetvendornamesComboData");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstVendorDetailsDTO;
        }
        public List<CountryVendorDTO> ShowVendorCountry()
        {
            List<CountryVendorDTO> lstCountryDTO = new List<CountryVendorDTO>();
            try
            {
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "select countryname,countryid from tabcountry where statusid=1 order by countryname;");
                while (npgdr.Read())
                {
                    CountryVendorDTO objCountryDTO = new CountryVendorDTO();
                    objCountryDTO.countryid = Convert.ToInt32(npgdr["countryid"]);
                    objCountryDTO.countryname = npgdr["countryname"].ToString();
                    lstCountryDTO.Add(objCountryDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowCountry");
            }
            finally
            {
                npgdr.Dispose();
            }

            return lstCountryDTO;
        }
        public List<StateVendorDTO> ShowState(string Countryid)
        {
            List<StateVendorDTO> lstStateDTO = new List<StateVendorDTO>();
            try
            {
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "select statename,stateid from tabstate where countryid=" + Countryid + " and statusid=1 order by statename;");
                while (npgdr.Read())
                {
                    StateVendorDTO objStateDTO = new StateVendorDTO();
                    objStateDTO.stateid = Convert.ToInt32(npgdr["stateid"]);
                    objStateDTO.statename = npgdr["statename"].ToString();
                    lstStateDTO.Add(objStateDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowState(string Countryid)");
            }
            finally
            {
                npgdr.Dispose();
            }

            return lstStateDTO;
        }
        public List<CityDTO> Showcities(string Countryid, string stateid)
        {
            List<CityDTO> lstCityDTO = new List<CityDTO>();
            try
            {
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "select cityname,cityid from tabcity where countryid=" + Countryid + " and stateid=" + stateid + " and statusid=1 order by cityname;");
                while (npgdr.Read())
                {
                    CityDTO objCityDTO = new CityDTO();
                    objCityDTO.cityid = Convert.ToInt32(npgdr["cityid"]);
                    objCityDTO.cityname = npgdr["cityname"].ToString();
                    lstCityDTO.Add(objCityDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Showcities(string Countryid, string stateid)");
            }
            finally
            {
                npgdr.Dispose();
            }

            return lstCityDTO;
        }
        public bool SaveVendordetails(VendorDetailsDTO VD, out string vendorid, out string vendorcode)
        {
            Boolean IsSaved = false;
            int Count = 0;
            vendorid = string.Empty;
            vendorcode = string.Empty;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string strID = string.Empty;
                string strCode = string.Empty;
                string strInsert = string.Empty;

                string check_exist = "SELECT COUNT(VCHVENDORNAME) FROM TABMMSVENDORMST  WHERE  UPPER(VCHVENDORNAME)='" + ManageQuote(VD.vendorname.ToUpper()) + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    //if (Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "Select Count(*) from TABMMSVENDORMST;")) > 0)
                    //{
                    //    strCode = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select 'VC'||MAX(coalesce((replace(VCHVENDORCODE,'VC','')::INTEGER),0))::INTEGER+1 from TABMMSVENDORMST Order by 1;"));
                    //}
                    //else
                    //{
                    //    strCode = "VC1";
                    //}
                    strCode = GenerateNextID("TABMMSVENDORMST", "VCHVENDORCODE", "VCHVENDOR DETAILS");
                    strInsert = @"INSERT INTO TABMMSVENDORMST(VCHVENDORNAME,VCHVENDORCODE,statusid,createdby,createddate,vchbankaccountnumber,vchbankname,vchbanklocation,vchifsc,vchtaxtype,vchvatorcst,vchpanno,vchtin,vchservicetax,vchexcise,vchremarks)VALUES('" + ManageQuote(VD.vendorname.ToUpper()) + "','" + strCode + "',1,1,current_timestamp,'" + ManageQuote(VD.bankacno) + "','" + ManageQuote(VD.bankname) + "','" + ManageQuote(VD.bankaddress) + "','" + ManageQuote(VD.bankifsc) + "','" + ManageQuote(VD.taxtype) + "','" + ManageQuote(VD.vatorcst) + "','" + ManageQuote(VD.panno) + "','" + ManageQuote(VD.tinno) + "','" + ManageQuote(VD.servicetax) + "','" + ManageQuote(VD.excise) + "','" + ManageQuote(VD.remarks) + "') returning vendorid;";
                    strID = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strInsert));
                    vendorid = strID;
                    vendorcode = strCode;
                    if (strID != null && strID != "" && strID != string.Empty)
                    {
                        strInsert = @"INSERT INTO TABMMSVENDORCONTACTMST(VENDORID,VCHVENDORCODE,VCHCONTACTPERSON,VCHFAX,VCHEMAIL,VCHMOBILENUMBER,VCHOFFICEPHONE,VCHWEBSITE,statusid,CREATEDBY,CREATEDDATE)VALUES(" + strID + ",'" + strCode + "','" + ManageQuote(VD.contactperson) + "','" + ManageQuote(VD.fax) + "','" + ManageQuote(VD.email) + "','" + ManageQuote(VD.mobileno) + "','" + ManageQuote(VD.landlineno) + "','" + ManageQuote(VD.website) + "',1,1,CURRENT_TIMESTAMP);";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                        strInsert = @"INSERT INTO tabmmsvendoraddressmst(VENDORID,VCHVENDORCODE,vchaddress1,vchaddress2,cityid,stateid,countryid,pincode,statusid,CREATEDBY,CREATEDDATE)VALUES(" + strID + ",'" + strCode + "','" + ManageQuote(VD.line1) + "','" + ManageQuote(VD.line2) + "','" + VD.cityid.ToString() + "','" + VD.stateid.ToString() + "','" + VD.countryid.ToString() + "','" + ManageQuote(VD.pincode) + "',1,1,CURRENT_TIMESTAMP);";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);

                    }
                    IsSaved = true;
                }

                trans.Commit();
                con.Close();
                con.ClearPool();
                con.Dispose();


            }
            catch (Exception ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "SaveVendordetails");
                IsSaved = false;
            }
            finally
            {

            }
            return IsSaved;
        }
        public List<VendorDetailsDTO> getVendorData(string ID)
        {
            List<VendorDetailsDTO> lstVendorDetailsDTO = new List<VendorDetailsDTO>();
            try
            {
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "select ta.vchvendorname,ta.vendorid,ta.vchvendorcode as vendorcode,tc.contactid as contactid,tc.vchcontactperson as contactperson,tc.vchfax as fax,tc.vchemail as email,tc.vchmobilenumber as mobilenumber,tc.vchofficephone as landline,tc.vchwebsite as website,ta.vchbankaccountnumber as bankacno,ta.vchbankname as bankname,ta.vchbanklocation as bankaddress,ta.vchifsc as bankifsc,ta.vchtaxtype as taxtype,ta.vchvatorcst as vatorcst,ta.vchpanno as panno,ta.vchtin  as tinno,ta.vchservicetax  as servicetax,ta.vchexcise as  excise,ta.vchremarks as remarks from tabmmsvendormst ta left join tabmmsvendorcontactmst tc on  ta.vendorid=tc.vendorid where  ta.statusid=1 and ta.vendorid=" + ID + ";");
                //string strQuery = "select * from tabmmsvendormst ta left join tabmmsvendoraddressmst tb on ta.vendorid=tb.intvendorid left join tabmmsvendorcontactmst tc on  ta.vendorid=tc.intvendorid";
                while (npgdr.Read())
                {
                    //select ta.vchvendorname,ta.vendorid,tc.contactid as contactid,tc.vchcontactperson as contactperson,tc.vchfax as fax,tc.vchemail as email,
                    //tc.vchmobilenumber as mobilenumber,tc.vchofficephone as landline,tc.vchwebsite as website
                    //ta.vchbankaccountnumber as bankacno,ta.vchbankname as bankname,ta.vchbanklocation as bankaddress,ta.vchifsc as bankifsc,ta.vchtaxtype as taxtype,
                    //ta.vchvatorcst as vatorcst,ta.vchpanno as panno,ta.vchtin  as tinno,ta.vchservicetax  as servicetax,ta.vchexcise as  excise,ta.vchremarks as remarks
                    VendorDetailsDTO objVendorDetailsDTO = new VendorDetailsDTO();
                    objVendorDetailsDTO.vendorid = Convert.ToInt32(npgdr["vendorid"]);
                    objVendorDetailsDTO.vendorcode = Convert.ToString(npgdr["vendorcode"]);
                    objVendorDetailsDTO.vendorname = Convert.ToString(npgdr["vchvendorname"]);
                    objVendorDetailsDTO.contactid = Convert.ToInt32(npgdr["contactid"]);
                    objVendorDetailsDTO.contactperson = Convert.ToString(npgdr["contactperson"]);
                    objVendorDetailsDTO.fax = Convert.ToString(npgdr["fax"]);
                    objVendorDetailsDTO.email = Convert.ToString(npgdr["email"]);
                    objVendorDetailsDTO.mobileno = Convert.ToString(npgdr["mobilenumber"]);
                    objVendorDetailsDTO.landlineno = Convert.ToString(npgdr["landline"]);
                    objVendorDetailsDTO.website = Convert.ToString(npgdr["website"]);
                    objVendorDetailsDTO.bankacno = Convert.ToString(npgdr["bankacno"]);
                    objVendorDetailsDTO.bankname = Convert.ToString(npgdr["bankname"]);
                    objVendorDetailsDTO.bankaddress = Convert.ToString(npgdr["bankaddress"]);
                    objVendorDetailsDTO.bankifsc = Convert.ToString(npgdr["bankifsc"]);
                    objVendorDetailsDTO.taxtype = Convert.ToString(npgdr["taxtype"]);
                    objVendorDetailsDTO.vatorcst = Convert.ToString(npgdr["vatorcst"]);
                    objVendorDetailsDTO.panno = Convert.ToString(npgdr["panno"]);
                    objVendorDetailsDTO.tinno = Convert.ToString(npgdr["tinno"]);
                    objVendorDetailsDTO.servicetax = Convert.ToString(npgdr["servicetax"]);
                    objVendorDetailsDTO.excise = Convert.ToString(npgdr["excise"]);
                    objVendorDetailsDTO.remarks = Convert.ToString(npgdr["remarks"]);
                    lstVendorDetailsDTO.Add(objVendorDetailsDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "getVendorData");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstVendorDetailsDTO;
        }

        public List<PurchaseOrderDTO> vendorDetails(string ID)
        {
            List<PurchaseOrderDTO> lstPurchaseOrderDTO = new List<PurchaseOrderDTO>();
            try
            {

                // string strqry = "select orderid,vchpurchaseorderno,datpurchaseorderdate::date,vchvendorname,coalesce(totalamount,0)  as orderamount from vwmmspurchaseorder  where vendorid=" + ID + " and vchpurchaseorderstatus='Y' group by orderid,vchpurchaseorderno,datpurchaseorderdate,vchvendorname,totalamount order by datpurchaseorderdate,orderid;";
                //string strqry = "select vchpurchaseorderno,datpurchaseorderdate,vchvendorname,to_char(sum(round(COALESCE(numorderedqty*numrate,0),2)), '99,99,999D99') as orderamount from vwmmspurchaseorder group by vchpurchaseorderno,datpurchaseorderdate,vchvendorname order by datpurchaseorderdate;";

                string strqry = "select orderid,vchpurchaseorderno,datgrndate::date as datpurchaseorderdate,vchvendorname,coalesce(totalamount,0)  as orderamount from vwmmsgrnorder  where vendorid=" + ID + " group by orderid,vchpurchaseorderno,datpurchaseorderdate,vchvendorname,totalamount order by datpurchaseorderdate,orderid;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strqry);

                while (npgdr.Read())
                {

                    PurchaseOrderDTO objPurchaseOrderDTO = new PurchaseOrderDTO();
                    objPurchaseOrderDTO.PurchaseOrderId = Convert.ToString(npgdr["vchpurchaseorderno"]);

                    //objPurchaseOrderDTO.PurchseOrderDate = Convert.ToString(npgdr["datpurchaseorderdate"]);
                    objPurchaseOrderDTO.PurchseOrderDate = Convert.ToDateTime(npgdr["datpurchaseorderdate"]).ToString("dd-MM-yyyy");
                    objPurchaseOrderDTO.VendorName = Convert.ToString(npgdr["vchvendorname"]);
                    if (npgdr["orderamount"] == null)
                    {
                        objPurchaseOrderDTO.EstimateRate = 0;
                    }
                    objPurchaseOrderDTO.EstimateRate = Convert.ToDecimal(npgdr["orderamount"]);


                    lstPurchaseOrderDTO.Add(objPurchaseOrderDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "vendorDetails");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstPurchaseOrderDTO;
        }


        public bool DeleteVendorData(string ID)
        {
            Boolean IsDeleted = false;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string strID = string.Empty;
                string strCode = string.Empty;
                string strInsert = string.Empty;
                int Count;
                string check_exist = "SELECT COUNT(*) FROM TABMMSVENDORMST  WHERE VENDORID=" + ID + " and statusid=1;";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));
                if (Count > 0)
                {
                    check_exist = "SELECT COUNT(vendorid) FROM tabmmsproductvendors  WHERE vendorid='" + ID + "' and statusid=1";
                    Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));
                    if (Count == 0)
                    {
                        check_exist = "SELECT COUNT(vendorid) FROM tabmmspurchaseorder  WHERE vendorid='" + ID + "' and statusid=1";
                        Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));
                        if (Count == 0)
                        {
                            strInsert = @"Update tabmmsvendoraddressmst  set statusid=2  where vendorid=" + ID + ";";
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                            //strInsert = "Delete from tabmmsvendorcontactmst  where vendorid=" + ID + ";";
                            strInsert = @"Update tabmmsvendorcontactmst set statusid=2  where vendorid=" + ID + ";";
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                            //strInsert = "Delete from TABMMSVENDORMST  where vendorid=" + ID + ";";
                            strInsert = @"Update TABMMSVENDORMST set statusid=2 where vendorid=" + ID + ";";
                            NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strInsert);
                            IsDeleted = true;
                            trans.Commit();
                            con.Close();
                            con.ClearPool();
                            con.Dispose();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "DeleteVendorData");
                IsDeleted = false;
            }
            finally
            {

            }
            return IsDeleted;
        }
        public bool UpdateVendordetails(VendorDetailsDTO VD)
        {
            Boolean IsUpdated = false;
            int Count = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string strID = string.Empty;
                string strCode = string.Empty;
                string strInsert = string.Empty;
                string check_exist = "SELECT COUNT(VCHvendorname) FROM TABMMSVENDORMST  WHERE  VENDORID='" + VD.vendorid + "' and UPPER(VCHVENDORCODE)='" + VD.vendorcode.Trim().ToUpper() + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));
                if (Count > 0)
                {
                    check_exist = "SELECT COUNT(vchvendorname) FROM TABMMSVENDORMST  WHERE UPPER(vchvendorname)='" + ManageQuote(VD.vendorname).Trim().ToUpper() + "' and statusid=1";
                    Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));
                    if (Count == 0)
                    {
                        check_exist = "SELECT COUNT(vendorid) FROM tabmmsproductvendors  WHERE vendorid='" + VD.vendorid + "' and statusid=1";
                        Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));
                        if (Count == 0)
                        {
                            check_exist = "SELECT COUNT(vendorid) FROM tabmmspurchaseorder  WHERE vendorid='" + VD.vendorid + "' and statusid=1";
                            Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));
                            if (Count == 0)
                            {
                                //strInsert = "Update TABMMSVENDORMST set VCHvendorname='" + VD.vendorname.Trim().ToUpper() + "',modifiedby=1,modifieddate=current_timestamp where vendorid=" + VD.vendorid + " and  UPPER(VCHVENDORCODE)='" + VD.vendorcode.Trim().ToUpper() + "';";
                                strInsert = @"Update tabmmsvendormst set vchvendorname='" + ManageQuote(VD.vendorname) + "', modifiedby=1, modifieddate=current_timestamp, vchbankaccountnumber='" + ManageQuote(VD.bankacno) + "',vchbankname='" + ManageQuote(VD.bankname) + "', vchbanklocation='" + ManageQuote(VD.bankaddress) + "', vchifsc='" + ManageQuote(VD.bankifsc) + "', vchtaxtype='" + ManageQuote(VD.taxtype) + "',vchvatorcst='" + ManageQuote(VD.vatorcst) + "',vchpanno='" + ManageQuote(VD.panno) + "', vchtin='" + ManageQuote(VD.tinno) + "', vchservicetax='" + ManageQuote(VD.servicetax) + "', vchexcise='" + ManageQuote(VD.excise) + "', vchremarks='" + ManageQuote(VD.remarks) + "' where vendorid=" + VD.vendorid + " and  UPPER(VCHVENDORCODE)='" + ManageQuote(VD.vendorcode) + "';";
                                strID = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strInsert));
                            }
                        }
                    }
                }
                strInsert = @"Update tabmmsvendormst set  modifiedby=1, modifieddate=current_timestamp, vchbankaccountnumber='" + ManageQuote(VD.bankacno) + "',vchbankname='" + ManageQuote(VD.bankname) + "', vchbanklocation='" + ManageQuote(VD.bankaddress) + "', vchifsc='" + ManageQuote(VD.bankifsc) + "', vchtaxtype='" + ManageQuote(VD.taxtype) + "',vchvatorcst='" + ManageQuote(VD.vatorcst) + "',vchpanno='" + ManageQuote(VD.panno) + "', vchtin='" + ManageQuote(VD.tinno) + "', vchservicetax='" + ManageQuote(VD.servicetax) + "', vchexcise='" + ManageQuote(VD.excise) + "', vchremarks='" + ManageQuote(VD.remarks) + "' where vendorid=" + VD.vendorid + " and  UPPER(VCHVENDORCODE)='" + ManageQuote(VD.vendorcode) + "';";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                strInsert = @"UPDATE TABMMSVENDORCONTACTMST SET VCHCONTACTPERSON='" + ManageQuote(VD.contactperson) + "',VCHFAX='" + ManageQuote(VD.fax) + "',VCHEMAIL='" + ManageQuote(VD.email) + "',VCHMOBILENUMBER='" + ManageQuote(VD.mobileno) + "',VCHOFFICEPHONE='" + ManageQuote(VD.landlineno) + "',VCHWEBSITE='" + ManageQuote(VD.website) + "',modifiedby=1,modifieddate=current_timestamp WHERE VENDORID=" + VD.vendorid + " AND CONTACTID=" + VD.contactid + " AND statusid=1;";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                trans.Commit();
                con.Close();
                con.ClearPool();
                con.Dispose();

                IsUpdated = true;

            }
            catch (Exception ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "UpdateVendordetails");
                IsUpdated = false;
            }
            finally
            {

            }
            return IsUpdated;
        }

        //ADDRESS
        public List<VendorDetailsDTO> getVendorAddressData(string ID)
        {
            List<VendorDetailsDTO> lstVendorDetailsDTO = new List<VendorDetailsDTO>();
            try
            {
                //npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "select tb.VENDORID as vendorid,tb.addressid,tb.vchaddress1 as line1,tb.vchaddress2 as line2,tb.cityid as cityid,tb.stateid as stateid,tb.countryid as countryid,tb.pincode as pincode  from tabmmsvendoraddressmst tb where tb.statusid=1 and  tb.VENDORID=" + ID + ";");
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "select tb.VENDORID as vendorid,tb.addressid,upper(tb.vchaddress1) as line1,upper(tb.vchaddress2) as line2,tb.cityid as cityid,cityname,statename,countryname,tb.stateid as stateid,tb.countryid as countryid,tb.pincode as pincode  from tabmmsvendoraddressmst tb join tabcity tcy on tb.cityid=tcy.cityid join tabcountry tcr on tb.countryid=tcr.countryid join tabstate ts on tb.stateid=ts.stateid where tb.statusid=1 and  tb.VENDORID=" + ID + ";");
                //string strQuery = "select ta.vchvendorcode,ta.vchvendorname,ta.vendorid,tb.intaddressid,tb.vchaddress1 as line1,tb.vchaddress2 as line2,tb.intcity as cityid,tb.intstate as stateid,tb.intcountry as countryid,tb.intpincode as pincode,tc.intcontactid as contactid,tc.vchcontactperson as contactperson,tc.vchfax as fax,tc.vchemail as email,tc.vchmobilenumber as mobilenumber,tc.vchofficephone as landline,tc.vchwebsite as website from tabmmsvendormst ta left join tabmmsvendoraddressmst tb on ta.vendorid=tb.VENDORID left join tabmmsvendorcontactmst tc on  ta.vendorid=tc.VENDORID where  ta.statusid=1 and ta.vendorid=" + ID + ";";
                while (npgdr.Read())
                {
                    VendorDetailsDTO objVendorDetailsDTO = new VendorDetailsDTO();
                    objVendorDetailsDTO.vendorid = Convert.ToInt32(npgdr["vendorid"]);
                    objVendorDetailsDTO.addressid = Convert.ToInt32(npgdr["addressid"]);
                    objVendorDetailsDTO.line1 = Convert.ToString(npgdr["line1"]);
                    objVendorDetailsDTO.line2 = Convert.ToString(npgdr["line2"]);
                    objVendorDetailsDTO.cityid = Convert.ToInt32(npgdr["cityid"]);
                    objVendorDetailsDTO.stateid = Convert.ToInt32(npgdr["stateid"]);
                    objVendorDetailsDTO.countryid = Convert.ToInt32(npgdr["countryid"]);
                    objVendorDetailsDTO.pincode = Convert.ToString(npgdr["pincode"]);
                    objVendorDetailsDTO.cityname = Convert.ToString(npgdr["cityname"]);
                    objVendorDetailsDTO.statename = Convert.ToString(npgdr["statename"]);
                    objVendorDetailsDTO.countryname = Convert.ToString(npgdr["countryname"]);
                    lstVendorDetailsDTO.Add(objVendorDetailsDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "getVendorData");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstVendorDetailsDTO;
        }
        public bool SaveAddressdetails(VendorDetailsDTO VD)
        {
            Boolean IsSaved = false;
            string Count = string.Empty;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string strInsert = string.Empty;
                string check_exist = "SELECT VCHVENDORCODE FROM TABMMSVENDORMST  WHERE  vendorid='" + VD.vendorid + "' and statusid=1";
                Count = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));
                if (Count != null && Count.ToString() != "" && Count != string.Empty)
                {
                    strInsert = @"INSERT INTO tabmmsvendoraddressmst(VENDORID,VCHVENDORCODE,vchaddress1,vchaddress2,cityid,stateid,countryid,pincode,statusid,CREATEDBY,CREATEDDATE)VALUES(" + VD.vendorid + ",'" + ManageQuote(Count) + "','" + ManageQuote(VD.line1) + "','" + ManageQuote(VD.line2) + "','" + VD.cityid.ToString() + "','" + VD.stateid.ToString() + "','" + VD.countryid.ToString() + "','" + ManageQuote(VD.pincode) + "',1,1,CURRENT_TIMESTAMP);";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                }
                trans.Commit();
                con.Close();
                con.ClearPool();
                con.Dispose();
                IsSaved = true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "SaveAddressdetails");
                IsSaved = false;
            }
            finally
            {

            }
            return IsSaved;
        }
        public bool UpdateVendorAddress(VendorDetailsDTO VD)
        {
            Boolean IsUpdated = false;
            int Count = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string strInsert = string.Empty;
                string check_exist = "SELECT count(*) FROM tabmmsvendoraddressmst  WHERE  addressid='" + VD.addressid + "' and statusid=1";
                Count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));
                if (Count > 0)
                {
                    strInsert = @"UPDATE tabmmsvendoraddressmst SET vchaddress1='" + ManageQuote(VD.line1) + "',vchaddress2='" + ManageQuote(VD.line2) + "',cityid=" + VD.cityid + ",stateid=" + VD.stateid + ",countryid=" + VD.countryid + ",pincode=" + ManageQuote(VD.pincode) + ",modifiedby=1,modifieddate=current_timestamp WHERE VENDORID=" + VD.vendorid + " AND addressid=" + VD.addressid + " AND statusid=1;";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                    trans.Commit();
                    con.Close();
                    con.ClearPool();
                    con.Dispose();
                    IsUpdated = true;
                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "UpdateVendorAddress");
                IsUpdated = false;
            }
            finally
            {

            }
            return IsUpdated;
        }
        public bool DeleteVendorAddress(string ID)
        {
            Boolean IsUpdated = false;
            int Count = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string strInsert = string.Empty;
                string check_exist = "SELECT count(*) FROM tabmmsvendoraddressmst  WHERE  addressid='" + ID + "' and statusid=1";
                Count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));
                if (Count > 0)
                {
                    check_exist = "SELECT vendorid FROM tabmmsvendoraddressmst  WHERE  addressid='" + ID + "' and statusid=1";
                    string strcheck = string.Empty;
                    strcheck = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));

                    check_exist = "SELECT count(*) FROM tabmmsvendoraddressmst  WHERE  vendorid='" + strcheck + "' and statusid=1";
                    Count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));
                    if (Count > 1)
                    {
                        strInsert = @"UPDATE tabmmsvendoraddressmst SET statusid=2 WHERE  addressid='" + ID + "' and statusid=1";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                        trans.Commit();
                        con.Close();
                        con.ClearPool();
                        con.Dispose();
                        IsUpdated = true;
                    }
                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "DeleteVendorAddress");
                IsUpdated = false;
            }
            finally
            {

            }
            return IsUpdated;
        }



        //Vendor Products List 
        public List<VendorProductDTO> vendorProducts(string ID)
        {
            List<VendorProductDTO> lstVendorProductDTO = new List<VendorProductDTO>();
            try
            {
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "select tp.productname,tv.recordid as recordid ,tp.productid,tp.productcode,tp.categoryname as productcategory,tp.subcategoryname as productsubcategory,tp.uomname as productuom,tv.numproductcost as productcost,tv.datcostupto  as productuptodate from tabmmsproductmst tp join tabmmsproductvendors tv on  tp.productid=tv.productid where tv.statusid=1 and vendorid=" + ID + ";");
                while (npgdr.Read())
                {
                    VendorProductDTO objVendorProductDTO = new VendorProductDTO();
                    //select tv.recordid as recordid ,tp.productid,tp.productcode,tp.categoryname as productcategory,tp.subcategoryname as productsubcategory,
                    //tp.uomname as productuom,tv.numproductcost as productcost,tv.datcostupto  as productuptodate
                    objVendorProductDTO.recordid = Convert.ToInt16(npgdr["recordid"]);
                    objVendorProductDTO.productid = Convert.ToInt16(npgdr["productid"]);
                    objVendorProductDTO.productcode = Convert.ToString(npgdr["productcode"]);
                    objVendorProductDTO.productcategory = Convert.ToString(npgdr["productcategory"]);
                    objVendorProductDTO.productsubcategory = Convert.ToString(npgdr["productsubcategory"]);
                    objVendorProductDTO.productcost = Convert.ToString(npgdr["productcost"]);
                    objVendorProductDTO.productuom = Convert.ToString(npgdr["productuom"]);
                    objVendorProductDTO.productname = Convert.ToString(npgdr["productname"]);
                    lstVendorProductDTO.Add(objVendorProductDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "vendorProducts");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstVendorProductDTO;
        }
        public List<ProductlistDTO> productlist()
        {
            List<ProductlistDTO> lstobjProductlistDTO = new List<ProductlistDTO>();
            try
            {
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "select productid,productname from tabmmsproductmst where statusid=1 order by productname;");
                while (npgdr.Read())
                {
                    ProductlistDTO objProductlistDTO = new ProductlistDTO();
                    objProductlistDTO.productid = Convert.ToInt16(npgdr["productid"]);
                    objProductlistDTO.productname = Convert.ToString(npgdr["productname"]);
                    lstobjProductlistDTO.Add(objProductlistDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "productlist");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstobjProductlistDTO;
        }
        public bool SavevendorProducts(VendorProductDTO VP)
        {
            Boolean IsSaved = false;
            string Count = string.Empty;
            int Count_exist = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string strInsert = string.Empty;
                string check_exist = "SELECT VCHVENDORCODE FROM TABMMSVENDORMST  WHERE  vendorid='" + VP.vendorid + "' and statusid=1";
                Count = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));

                if ((Count != null && Count.ToString() != "" && Count != string.Empty))
                {
                    string check_product = "SELECT count(*) FROM tabmmsproductvendors  WHERE  statusid=1 and productid='" + VP.productid + "' and vendorid='" + VP.vendorid + "'";
                    int Count1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_product));
                    if (Count1 == 0)
                    {
                        strInsert = "INSERT INTO tabmmsproductvendors(VENDORID,VCHVENDORID,productid,productcode,vchmeasureduomid,numproductcost,datcostupto,statusid,CREATEDBY,CREATEDDATE)VALUES(" + VP.vendorid + ",'" + ManageQuote(Count) + "','" + VP.productid + "','" + ManageQuote(VP.productcode) + "','" + ManageQuote(VP.productuom) + "'," + ManageQuote(VP.productcost) + ",current_date,1,1,CURRENT_TIMESTAMP)";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                        trans.Commit();
                        con.Close();
                        con.ClearPool();
                        con.Dispose();
                        IsSaved = true;
                    }

                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "SavevendorProducts");
                IsSaved = false;
            }
            finally
            {

            }
            return IsSaved;
        }
        public VendorProductDTO ShowProductDetails(string Productid)
        {
            VendorProductDTO objVendorProductDTO = new VendorProductDTO();
            try
            {
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "select productcode ,productname,categoryname as productcategory,subcategoryname as productsubcategory, uomname as productuom from tabmmsproductmst where statusid=1 and productid=" + Productid + ";");
                while (npgdr.Read())
                {
                    objVendorProductDTO.productcode = Convert.ToString(npgdr["productcode"]);
                    objVendorProductDTO.productname = Convert.ToString(npgdr["productname"]);
                    objVendorProductDTO.productcategory = Convert.ToString(npgdr["productcategory"]);
                    objVendorProductDTO.productsubcategory = Convert.ToString(npgdr["productsubcategory"]);
                    objVendorProductDTO.productuom = Convert.ToString(npgdr["productuom"]);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowProductDetails");
            }
            finally
            {
                npgdr.Dispose();
            }
            return objVendorProductDTO;
        }

        public bool UpdatevendorProducts(VendorProductDTO VP)
        {
            Boolean IsSaved = false;
            int Count;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string strInsert = string.Empty;
                //SELECT count(*) FROM tabmmsproductvendors  WHERE recordid<>16 and statusid=1 and productid=12 and vchvendorid='VD4'
                // string check_exist = "SELECT count(*) FROM tabmmsproductvendors  WHERE recordid<>'" + VP.recordid + "' and statusid=1 and productid='" + VP.productid + "' and vendorid='" + VP.vendorid + "'";
                // Count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, check_exist));
                //if (Count > 0)
                //{
                strInsert = "update tabmmsproductvendors set numproductcost=" + VP.productcost + ",datcostupto=current_date,statusid=1,modifiedby=1,modifieddate=CURRENT_TIMESTAMP where recordid=" + VP.recordid + ";";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                trans.Commit();
                con.Close();
                con.ClearPool();
                con.Dispose();
                IsSaved = true;
                //}
            }
            catch (Exception ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "SavevendorProducts");
                IsSaved = false;
            }
            finally
            {

            }
            return IsSaved;
        }


        public bool Deletevendorproducts(VendorProductDTO VP)
        {
            Boolean IsDeleted = false;
            string check_exist = string.Empty;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                string strInsert = string.Empty;
                int Count;
                string chk = "SELECT COUNT(*) FROM tabmmsproductvendors  WHERE recordid=" + VP.recordid + " and statusid=1;";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, chk));
                if (Count > 0)
                {
                    strInsert = @"Update tabmmsproductvendors set statusid=2 WHERE recordid=" + VP.recordid + " and statusid=1;";
                    NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strInsert);
                    trans.Commit();
                    con.Close();
                    con.ClearPool();
                    con.Dispose();
                }
                IsDeleted = true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "Deletevendorproducts");
                IsDeleted = false;
            }
            finally
            {

            }
            return IsDeleted;
        }
        #endregion

        #region Restaurant Location

        public List<RestaurantLocationDTO> ShowRestaurantLocation()
        {
            List<RestaurantLocationDTO> lstRestaurantLocationdetails = new List<RestaurantLocationDTO>();
            try
            {

                string strInsert = "  SELECT restaurantlocationid,restaurantlocationname, line1, line2, area,tabstate.stateid,tabcity.cityid, statename, cityname, intpincode FROM tabmmsrestaurantlocationmst left join tabstate on "
                                     + "tabmmsrestaurantlocationmst.stateid=tabstate.stateid left join tabcity on tabmmsrestaurantlocationmst.cityid=tabcity.cityid  WHERE tabmmsrestaurantlocationmst.STATUSID=1 ORDER BY restaurantlocationid desc;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                while (npgdr.Read())
                {
                    RestaurantLocationDTO objRestaurantLocationDTO = new RestaurantLocationDTO();
                    objRestaurantLocationDTO.RecordId = Convert.ToInt32(npgdr["restaurantlocationid"]);
                    objRestaurantLocationDTO.RestaurantName = npgdr["restaurantlocationname"].ToString();
                    objRestaurantLocationDTO.Line1 = npgdr["line1"].ToString();
                    objRestaurantLocationDTO.Line2 = npgdr["line2"].ToString();
                    objRestaurantLocationDTO.Area = npgdr["area"].ToString();
                    objRestaurantLocationDTO.StateName = npgdr["statename"].ToString();
                    objRestaurantLocationDTO.CityName = npgdr["cityname"].ToString();

                    objRestaurantLocationDTO.Stateid = Convert.ToInt32(npgdr["stateid"]);
                    objRestaurantLocationDTO.Cityid = Convert.ToInt32(npgdr["cityid"]);

                    objRestaurantLocationDTO.Pincode = Convert.ToInt32(npgdr["intpincode"]);
                    lstRestaurantLocationdetails.Add(objRestaurantLocationDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowRestaurantLocation");
                throw ex;
                // ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstRestaurantLocationdetails;

        }

        public List<RestaurantLocationDTO> ShowState()
        {
            List<RestaurantLocationDTO> lstRestaurantLocationdetails = new List<RestaurantLocationDTO>();
            try
            {

                string strInsert = "select stateid,statename from tabstate  where statusid=1  order by statename";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                while (npgdr.Read())
                {
                    RestaurantLocationDTO objRestaurantLocationDTO = new RestaurantLocationDTO();
                    objRestaurantLocationDTO.Stateid = Convert.ToInt32(npgdr["stateid"]);
                    objRestaurantLocationDTO.StateName = Convert.ToString(npgdr["statename"]);
                    lstRestaurantLocationdetails.Add(objRestaurantLocationDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowRestaurantLocation");
                throw ex;
                // ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstRestaurantLocationdetails;

        }
        public List<RestaurantLocationDTO> ShowCity()
        {
            List<RestaurantLocationDTO> lstRestaurantLocationdetails = new List<RestaurantLocationDTO>();
            try
            {

                string strInsert = "select cityid,cityname,stateid from tabcity  where statusid=1  order by cityname   ";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                while (npgdr.Read())
                {
                    RestaurantLocationDTO objRestaurantLocationDTO = new RestaurantLocationDTO();
                    objRestaurantLocationDTO.Cityid = Convert.ToInt32(npgdr["cityid"]);
                    objRestaurantLocationDTO.Stateid = Convert.ToInt32(npgdr["stateid"]);
                    objRestaurantLocationDTO.CityName = Convert.ToString(npgdr["cityname"]);

                    lstRestaurantLocationdetails.Add(objRestaurantLocationDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowRestaurantLocation");
                throw ex;
                // ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstRestaurantLocationdetails;

        }

        public int SaveRestaurantLocation(RestaurantLocationDTO RestaurantLoc)
        {

            int Count = 0;
            try
            {

                string check_exist = "SELECT COUNT(restaurantlocationname) FROM tabmmsrestaurantlocationmst  WHERE  UPPER(restaurantlocationname)='" + RestaurantLoc.RestaurantName.Trim().ToUpper() + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    //string strCode = string.Empty;
                    //strCode = GenerateNextID("tabmmsrestaurantlocationmst", "restaurantlocationCode", "restaurantlocation");
                    string strInsert = "INSERT INTO tabmmsrestaurantlocationmst(restaurantlocationname, line1, line2, area, stateid, cityid, intpincode, statusid, createdby, createddate) "
                   + " VALUES ('" + RestaurantLoc.RestaurantName.Trim().ToUpper() + "','" + RestaurantLoc.Line1.Trim() + "','" + RestaurantLoc.Line2.Trim() + "','" + RestaurantLoc.Area.Trim() + "','" + RestaurantLoc.Stateid + "','" + RestaurantLoc.Cityid + "'," + RestaurantLoc.Pincode + ",1,1,CURRENT_DATE);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveRestaurantLocation");
            }
            return Count;
        }
        public int UpdateRestaurantLocation(RestaurantLocationDTO RestaurantLoc)
        {
            int Count = 0;
            try
            {

                string check_exist = "SELECT COUNT(restaurantlocationname) FROM tabmmsrestaurantlocationmst  WHERE restaurantlocationid=" + RestaurantLoc.RecordId + " and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count > 0)
                {
                    string strInsert = "UPDATE tabmmsrestaurantlocationmst SET restaurantlocationname='" + RestaurantLoc.RestaurantName + "',line1='" + RestaurantLoc.Line1 + "',line2='" + RestaurantLoc.Line2 + "',area='" + RestaurantLoc.Area + "',stateid='" + RestaurantLoc.Stateid + "',cityid='" + RestaurantLoc.Cityid + "',intpincode=" + RestaurantLoc.Pincode + ",modifieddate=current_timestamp,modifiedby=1 WHERE restaurantlocationid=" + RestaurantLoc.RecordId + ";";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateRestaurantLocation");
                throw;
            }

            return Count;
        }
        public bool DeleteRestaurantLocation(int RecordId)
        {
            bool IsValid = false;
            int Count = 0;
            try
            {
                string check_exist = "SELECT COUNT(restaurantlocationname) FROM tabmmsrestaurantlocationmst  WHERE restaurantlocationid=" + RecordId + " and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count > 0)
                {
                    string strDelete = "UPDATE tabmmsrestaurantlocationmst SET STATUSID=2 WHERE restaurantlocationid='" + RecordId + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);
                    IsValid = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteRestaurantLocation");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }
        #endregion



        #region UOM



        public List<UOMDTO> ShowUOM()
        {
            List<UOMDTO> lstProductCategory = new List<UOMDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand("select uomid, upper(name)as name, nameabbreviation, quantity, uom, uomabbreviation from tabmmsuommst where statusid=1 order by name;", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            UOMDTO objProductCategory = new UOMDTO();
                            objProductCategory.UOMid = Convert.ToInt32(npgdr["uomid"]);
                            objProductCategory.Name = npgdr["name"].ToString();
                            objProductCategory.NameAbbreviation = Convert.ToString(npgdr["nameabbreviation"]);
                            objProductCategory.Quantity = Convert.ToDecimal(npgdr["quantity"]);
                            objProductCategory.UOM = Convert.ToString(npgdr["uom"]);
                            objProductCategory.UOMAbbreviation = Convert.ToString(npgdr["uomabbreviation"]);
                            lstProductCategory.Add(objProductCategory);
                        }
                    }
                    con.Close();
                    con.ClearPool();
                    con.Dispose();

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
            return lstProductCategory;

        }
        public int SaveUOM(UOMDTO ProductCategory)
        {
            int Count = 0;
            try
            {

                string check_exist = "SELECT COUNT(name) FROM tabmmsuommst  WHERE  UPPER(name)='" + ManageQuote(ProductCategory.Name.Trim().ToUpper()) + "'   and  statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));

                if (Count == 0)
                {
                    string strInsert = "INSERT INTO tabmmsuommst(name,nameabbreviation, quantity, uom, uomabbreviation,statusid, createdby, createddate) VALUES ('" + ManageQuote(ProductCategory.Name) + "','" + ManageQuote(ProductCategory.NameAbbreviation) + "'," + ProductCategory.Quantity + ", '" + ProductCategory.UOM + "', '" + ProductCategory.UOMAbbreviation + "',1,1, Current_timestamp)";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveUOM");
                throw;

            }

            return Count;
        }
        public int UpdateUOM(UOMDTO Ic)
        {
            int Count = 0;
            try
            {
                Count = ProductCategoryCheck(Ic.UOMid, "tabmmsuommst", "uomid");
                if (Count == 0)
                {
                    string strInsert = "UPDATE tabmmsuommst   SET  name='" + ManageQuote(Ic.Name) + "', nameabbreviation='" + ManageQuote(Ic.NameAbbreviation) + "', quantity=" + Ic.Quantity + ",uom='" + ManageQuote(Ic.UOM) + "',uomabbreviation='" + ManageQuote(Ic.UOMAbbreviation) + "',modifiedby=1 from tabmmsuommst WHERE productcategoryid='" + Ic.UOMid + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateUOM");
                throw;
            }


            return Count;
        }

        public bool DeleteUOM(int categoryid)
        {
            bool IsValid = false;
            try
            {

                // int res = ProductCategoryCheck(categoryid, "tabmmsuommst", "uomid");
                int res = 0;
                if (res == 0)
                {
                    string strQuery = "UPDATE tabmmsuommst SET STATUSID=2 WHERE uomid='" + categoryid + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery);
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteUOM");
                throw;
            }

            return IsValid;
        }

        #endregion




        #region ShelfMaster

        public List<ShelfMasterDTO> ShowShelfMaster()
        {
            List<ShelfMasterDTO> lstProductCategory = new List<ShelfMasterDTO>();
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT shelfid, shelfname, shelfcode, storagearea,storagecode FROM tabmmsshelfmst where statusid=1 order by shelfname;", con))
                    {
                        npgdr = cmd.ExecuteReader();
                        while (npgdr.Read())
                        {
                            ShelfMasterDTO objProductCategory = new ShelfMasterDTO();
                            objProductCategory.Shelfid = Convert.ToInt32(npgdr["shelfid"]);
                            objProductCategory.ShelfName = npgdr["shelfname"].ToString();
                            objProductCategory.ShelfCode = Convert.ToString(npgdr["shelfcode"]);
                            objProductCategory.StorageLocationName = Convert.ToString(npgdr["storagearea"]);
                            objProductCategory.StorageLocationCode = Convert.ToString(npgdr["storagecode"]);


                            lstProductCategory.Add(objProductCategory);
                        }
                    }
                    con.Close();
                    con.ClearPool();
                    con.Dispose();
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
            return lstProductCategory;

        }
        public int SaveShelfMaster(ShelfMasterDTO ProductCategory)
        {
            int Count = 0;
            try
            {

                string check_exist = "SELECT COUNT(shelfname) FROM tabmmsshelfmst  WHERE  UPPER(shelfname)='" + ManageQuote(ProductCategory.ShelfName.Trim().ToUpper()) + "'   and  statusid=1";
                //string check_exist = "select count(*) from tabmmsshelfmst where  upper(shelfname)='" + ManageQuote(ProductCategory.ShelfName.Trim().ToUpper()) + "' and statusid=1 and  shelfid <>" + ProductCategory.Shelfid+ ";";

                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));

                if (Count == 0)
                {
                    ProductCategory.ShelfCode = GenerateNextID("tabmmsshelfmst", "shelfcode", "SHELF MASTER");
                    string strInsert = "INSERT INTO tabmmsshelfmst(shelfname, shelfcode, storagearea,storagecode,statusid, createdby, createddate) VALUES ('" + ManageQuote(ProductCategory.ShelfName.Trim().ToUpper()) + "','" + ManageQuote(ProductCategory.ShelfCode.Trim().ToUpper()) + "','" + ManageQuote(ProductCategory.StorageLocationName) + "','" + ManageQuote(ProductCategory.StorageLocationCode) + "',1,1, Current_timestamp)";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveShelfMaster");
                throw;

            }

            return Count;
        }
        public int UpdateShelfMaster(ShelfMasterDTO Ic)
        {
            int Count = 0;
            try
            {
                //string check_exist = "SELECT COUNT(shelfname) FROM tabmmsshelfmst  WHERE shelfid=" + Ic.Shelfid + " and statusid=1";
                string check_exist = "select count(*) from tabmmsshelfmst where  upper(shelfname)='" + ManageQuote(Ic.ShelfName.Trim().ToUpper()) + "' and statusid=1 and  shelfid <>" + Ic.Shelfid + ";";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                //Count = ProductCategoryCheck(Ic.ShelfName, "tabmmsshelfmst", "shelfname");
                if (Count == 0)
                {
                    string strInsert = "UPDATE tabmmsshelfmst   SET  shelfname='" + ManageQuote(Ic.ShelfName.Trim().ToUpper()) + "', shelfcode='" + ManageQuote(Ic.ShelfCode.Trim().ToUpper()) + "', storagearea='" + ManageQuote(Ic.StorageLocationName) + "',storagecode='" + ManageQuote(Ic.StorageLocationCode) + "',modifiedby=1  WHERE Shelfid='" + Ic.Shelfid + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UpdateShelfMaster");
                throw;
            }


            return Count;
        }
        public bool DeleteShelfMaster(int categoryid)
        {
            bool IsValid = false;
            try
            {
                string strQuery = "UPDATE tabmmsshelfmst SET STATUSID=2 WHERE Shelfid='" + categoryid + "';";
                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery);
                IsValid = true;

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteShelfMaster");
                throw;
            }

            return IsValid;
        }
        #endregion


        #region Purchase Order
        public DataTable GetVendorProducts(int VID)
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;

            try
            {
                strData = "select distinct  tp.productid,tp.productname from tabmmsproductvendors  tv join tabmmsproductmst tp on tv.productid=tp.productid where  tv.statusid=1 and tp.statusid=1 and tv.vendorid=" + VID + " order by productname;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "PurchaseOrder");
            }

            return dt;
        }
        public DataTable getProductStoragelocations(string prdid)
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {
                strData = "select * from (select regexp_split_to_table(storagelocationid, ',') AS storagelocationids,productid from tabmmsproductmst where productid in (" + prdid + ")) x  join (select storagelocationid,storagelocationname, shelfid,shelfname from tabmmsstoragelocationmst tsa join tabmmsshelfmst tss on tsa.storagelocationname=tss.storagearea)y on x.storagelocationids=y.storagelocationid::text";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "PurchaseOrder");
            }

            return dt;
        }
        public DataTable GetVendorNames()
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "select vendorid,vchvendorname from tabmmsvendormst where statusid=1 order by vchvendorname;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "PurchaseOrder");
            }

            return dt;
        }
        public DataTable BindUOMNames()
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "select uomid,uom from tabmmsuommst where statusid=1;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "PurchaseOrder");
            }

            return dt;
        }
        public DataTable GetPlaceofDeliveryData()
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "select restaurantlocationname as PlaceofDelivery,restaurantlocationname as PlaceofDelivery from tabmmsrestaurantlocationmst where statusid=1;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "PurchaseOrder");
            }

            return dt;
        }
        public DataTable GetVendorContactPerson(int Name)
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;

            try
            {
                strData = "select vchcontactperson,vchmobilenumber from tabmmsvendorcontactmst where vendorid=" + Name + " and statusid=1";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "PurchaseOrder");
            }

            return dt;
        }
        public DataTable GetProductuombyid(int prdid, int vendorid)
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;

            try
            {
                //strData = "select uomname,categoryname||'+'||productcategoryid as categoryname,subcategoryname||'+'||productsubcategoryid as subcategoryname,productcode,storagelocationid,storagelocation,shelfname,shelfid from tabmmsproductmst  where productid=" + prdid + " and statusid=1;";
                strData = "select uomname,categoryname||'+'||productcategoryid as categoryname,subcategoryname||'+'||productsubcategoryid as subcategoryname,tp.productcode,storagelocationid,storagelocation,shelfname,shelfid,tv.numproductcost as EstimateRate  from tabmmsproductmst tp join tabmmsproductvendors tv on tp.productid=tv.productid where vendorid=" + vendorid + " and tp.productid=" + prdid + " and tp.statusid=1 and tv.statusid=1;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "PurchaseOrder");
            }

            return dt;
        }
        public DataTable Getvendorsbyproduct(int prdid)
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;

            try
            {
                strData = "select distinct tb.vchvendorcode ||'_'|| tb.vchvendorname as vchvendorname,tb.vendorid  from tabmmsproductvendors ta left join tabmmsvendormst tb on ta.vendorid=tb.vendorid where ta.productid=" + prdid + " and ta.statusid=1 and tb.statusid=1;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "PurchaseOrder");
            }

            return dt;
        }
        public bool SavePurchaseOrder(PurchaseOrderDTO PurchaseOrderDTO, List<PurchaseOrderDTO> listPurchaseOrderDTO, PurchaseOrderTAXDTO TAX, out string pono)
        {
            bool IsSave = false;
            pono = string.Empty;
            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();
            try
            {
                string strinsertpurchseorder = string.Empty;
                //string Maxpurchaseorderid = "select substring(vchPurchaseorderid,4)::int +1 from tabrmsPurchaseorder order by Recordid desc limit 1  ;";
                //int PurchaseID = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, Maxpurchaseorderid));
                //PurchaseOrderDTO.PurchaseOrderId = "POI" + PurchaseID;
                if (TAX.DiscountType != null && TAX.DiscountValue != string.Empty)
                {
                    if (TAX.DiscountValue == null || TAX.DiscountValue == string.Empty)
                    {
                        TAX.DiscountValue = "0";
                    }
                }
                else
                {
                    TAX.DiscountValue = "0";
                    TAX.DiscountFlatPercentage = "0";
                }
                if (TAX.taxvatcst == null || TAX.taxvatcst == string.Empty || TAX.taxvatcst == "")
                {
                    TAX.TaxvatorcstAmount = "0";
                }
                if (TAX.TransportCharges == null || TAX.TransportCharges == string.Empty)
                {
                    TAX.TransportCharges = "0";
                }
                if (TAX.TaxExcisePercentage == null || TAX.taxtype == string.Empty || TAX.taxtype == "")
                {
                    TAX.TaxSHCESSPercentage = "0";
                    TAX.TaxSHCESSAmount = "0";
                    TAX.TaxCESSPercentage = "0";
                    TAX.TaxCESSAmount = "0";
                    TAX.TaxExciseAmount = "0";
                }
                if (PurchaseOrderDTO.PoType == "NEW" || PurchaseOrderDTO.PoType == "REORDER")
                {
                    PurchaseOrderDTO.PurchaseOrderId = GenerateNextID("tabmmsPurchaseorder", "vchpurchaseorderno", "PURCHASE ORDER");
                }
                else if (PurchaseOrderDTO.PoType == "MODIFY")
                {
                    PurchaseOrderDTO.PurchaseOrderId = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select distinct  vchpurchaseorderno from tabmmspurchaseorder where statusid =1 and recordid=" + PurchaseOrderDTO.poid + ";"));
                    strinsertpurchseorder = "INSERT INTO  tabmmstempmodifypurchaseorder(vchpurchaseorderno, vchpurchaserequestno, vchpurchasequoteno, vchpurchaseordertype, datpurchaseorderdate, vchrequestedby, vchapprovalby, vendorid, vchvendorname, vchcontactperson, vchcontactno, vchplaceofdelivery, vchterms, vchpurchaseorderstatus, statusid, createdby, createddate, modifiedby, modifieddate, pothourh, numtransportcharges, discountamount, excisetaxpercentage, excisetaxamount, cesstaxpercentage, cesstaxamount, shcesstaxpercentage, shcesstaxamount, taxtype, vatorcsttaxpercentage, vatorcsttaxamount, basicamount, totalamount,itemdiscountamount)";
                    strinsertpurchseorder = strinsertpurchseorder + "SELECT vchpurchaseorderno, vchpurchaserequestno, vchpurchasequoteno, vchpurchaseordertype, datpurchaseorderdate, vchrequestedby, vchapprovalby, vendorid, vchvendorname, vchcontactperson, vchcontactno, vchplaceofdelivery, vchterms, vchpurchaseorderstatus, statusid," + PurchaseOrderDTO.createdby + " as  createdby,current_date as createddate,null as modifiedby,now()::timestamp as modifieddate, pothourh, numtransportcharges, discountamount, excisetaxpercentage, excisetaxamount, cesstaxpercentage, cesstaxamount, shcesstaxpercentage, shcesstaxamount, taxtype, vatorcsttaxpercentage, vatorcsttaxamount, basicamount, totalamount,itemdiscountamount FROM tabmmspurchaseorder where recordid=" + PurchaseOrderDTO.poid + ";";
                    strinsertpurchseorder = strinsertpurchseorder + "INSERT INTO tabmmstempmodifypurchaseorderdetails (orderid, vchpurchaseorderno, productid, productcode, productcategoryid, categoryname, productsubcategoryid, subcategoryname, vchuom, vchorderuom, numorderedqty, datdeliverybefore, numrate, statusid,createdby , createddate, modifiedby, modifieddate, productname, numorderconvertionqty, vchvendorname, vendorid, vchpovendorno,taxtype, vatorcsttaxpercentage, vatorcsttaxamount, amount, excisetaxpercentage, excisetaxamount, cesstaxpercentage, cesstaxamount, shcesstaxpercentage,shcesstaxamount,vchdiscounttype,numdiscountvalue,discountamount)";
                    strinsertpurchseorder = strinsertpurchseorder + "SELECT orderid, vchpurchaseorderno, productid, productcode, productcategoryid, categoryname, productsubcategoryid, subcategoryname, vchuom, vchorderuom, numorderedqty, datdeliverybefore, numrate, statusid, " + PurchaseOrderDTO.createdby + " as createdby, current_date as createddate,null as modifiedby, now()::timestamp as modifieddate, productname, numorderconvertionqty, vchvendorname, vendorid, vchpovendorno,taxtype, vatorcsttaxpercentage, vatorcsttaxamount, amount, excisetaxpercentage, excisetaxamount, cesstaxpercentage, cesstaxamount, shcesstaxpercentage,shcesstaxamount,vchdiscounttype,numdiscountvalue,discountamount FROM tabmmspurchaseorderdetails where orderid=" + PurchaseOrderDTO.poid + ";";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strinsertpurchseorder);
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "delete from tabmmspurchaseorderdetails where orderid=" + PurchaseOrderDTO.poid + "");
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "delete from tabmmspurchaseorder where recordid=" + PurchaseOrderDTO.poid + "");
                }
                //strinsertpurchseorder = "INSERT INTO tabmmsPurchaseorder(vchpurchaseorderno,vchpurchaserequestno,vchpurchasequoteno,vchpurchaseordertype,POThourh,datpurchaseorderdate,vchrequestedby,vchapprovalby,vchcontactperson,vchcontactno,vchplaceofdelivery,vchterms,vchpurchaseorderstatus,statusid,CREATEDBY,CREATEDDATE,vchvendorname,vendorid,discountamount, numtransportcharges, excisetaxpercentage, excisetaxamount, cesstaxpercentage, cesstaxamount, shcesstaxpercentage, shcesstaxamount, taxtype, vatorcsttaxpercentage, vatorcsttaxamount, basicamount, totalamount,numdueamount,vchdiscounttype,numdiscountvalue,vchtaxtype)VALUES ('" + ManageQuote(PurchaseOrderDTO.PurchaseOrderId) + "','" + ManageQuote(PurchaseOrderDTO.PRNO) + "','" + ManageQuote(PurchaseOrderDTO.PQNO) + "','" + ManageQuote(PurchaseOrderDTO.PoType) + "','" + ManageQuote(PurchaseOrderDTO.PoThrough) + "','" + FormatDate(PurchaseOrderDTO.PurchseOrderDate) + "','" + ManageQuote(PurchaseOrderDTO.RequestedBy) + "','" + ManageQuote(PurchaseOrderDTO.ApprovalBy) + "','" + ManageQuote(PurchaseOrderDTO.ContactPerson) + "','" + ManageQuote(PurchaseOrderDTO.Contactno) + "','" + ManageQuote(PurchaseOrderDTO.PlaceofDelivery) + "','" + ManageQuote(PurchaseOrderDTO.Terms) + "','Y',1," + PurchaseOrderDTO.createdby + ",current_date,'" + ManageQuote(PurchaseOrderDTO.VendorName) + "'," + PurchaseOrderDTO.VendorID + "," + TAX.DiscountValue + ", " + TAX.TransportCharges + ", " + TAX.TaxExcisePercentage + ", " + TAX.TaxExciseAmount + ", " + TAX.TaxCESSPercentage + ", " + TAX.TaxCESSAmount + ", " + TAX.TaxSHCESSPercentage + ", " + TAX.TaxSHCESSAmount + ", '" + TAX.vatorcst + "', " + TAX.taxvatcst + ", " + TAX.TaxvatorcstAmount + ", " + TAX.BasicAmount + ", " + TAX.TotalAmount + "," + TAX.TotalAmount + ",'" + TAX.DiscountType + "'," + TAX.DiscountFlatPercentage + ",'" + TAX.taxtype + "') returning recordid;";
                strinsertpurchseorder = "INSERT INTO tabmmsPurchaseorder(vchpurchaseorderno,vchpurchaserequestno,vchpurchasequoteno,vchpurchaseordertype,POThourh,datpurchaseorderdate,vchrequestedby,vchapprovalby,vchcontactperson,vchcontactno,vchplaceofdelivery,vchterms,vchpurchaseorderstatus,statusid,CREATEDBY,CREATEDDATE,vchvendorname,vendorid,discountamount, numtransportcharges, excisetaxamount, cesstaxamount, shcesstaxamount, vatorcsttaxamount, basicamount, totalamount,numdueamount,vchdiscounttype,numdiscountvalue,itemdiscountamount) VALUES ('" + ManageQuote(PurchaseOrderDTO.PurchaseOrderId) + "','" + ManageQuote(PurchaseOrderDTO.PRNO) + "','" + ManageQuote(PurchaseOrderDTO.PQNO) + "','" + ManageQuote(PurchaseOrderDTO.PoType) + "','" + ManageQuote(PurchaseOrderDTO.PoThrough) + "','" + FormatDate(PurchaseOrderDTO.PurchseOrderDate) + "','" + ManageQuote(PurchaseOrderDTO.RequestedBy) + "','" + ManageQuote(PurchaseOrderDTO.ApprovalBy) + "','" + ManageQuote(PurchaseOrderDTO.ContactPerson) + "','" + ManageQuote(PurchaseOrderDTO.Contactno) + "','" + ManageQuote(PurchaseOrderDTO.PlaceofDelivery) + "','" + ManageQuote(PurchaseOrderDTO.Terms) + "','Y',1," + PurchaseOrderDTO.createdby + ",current_date,'" + ManageQuote(PurchaseOrderDTO.VendorName) + "'," + PurchaseOrderDTO.VendorID + "," + TAX.DiscountValue + ", " + TAX.TransportCharges + ", " + TAX.totalexcise + ", " + TAX.totalcess + "," + TAX.totalshcess + ", " + TAX.totalvatorcst + ", " + TAX.BasicAmount + ", " + TAX.TotalAmount + "," + TAX.TotalAmount + ",'" + TAX.DiscountType + "'," + TAX.DiscountFlatPercentage + "," + TAX.totalproductdiscount + ") returning recordid;";
                PurchaseOrderDTO.OrderId = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strinsertpurchseorder));
                pono = PurchaseOrderDTO.PurchaseOrderId;
                string Items = string.Empty;
                for (int i = 0; i < listPurchaseOrderDTO.Count; i++)
                {
                    Items = string.Empty;
                    if (Convert.ToString(listPurchaseOrderDTO[i].ProductSubcategoryID) == "" || Convert.ToString(listPurchaseOrderDTO[i].ProductSubcategoryID) == string.Empty || listPurchaseOrderDTO[i].ProductSubcategoryID == null || Convert.ToInt64(listPurchaseOrderDTO[i].ProductSubcategoryID) == 0)
                    {
                        listPurchaseOrderDTO[i].ProductSubcategoryID = "null";
                        listPurchaseOrderDTO[i].ProductSubCategoryName = "";
                    }
                    if (Convert.ToString(listPurchaseOrderDTO[i].ProductSubCategoryName) == "" || Convert.ToString(listPurchaseOrderDTO[i].ProductSubCategoryName) == string.Empty || listPurchaseOrderDTO[i].ProductSubCategoryName == null || listPurchaseOrderDTO[i].ProductSubCategoryName == "SELECT")
                    {
                        listPurchaseOrderDTO[i].ProductSubCategoryName = "";
                        listPurchaseOrderDTO[i].ProductSubcategoryID = "null";
                    }
                    //if (listPurchaseOrderDTO[i].ProductSubcategoryID == null || listPurchaseOrderDTO[i].ProductSubcategoryID == 0 || listPurchaseOrderDTO[i].ProductSubCategoryName == "" || listPurchaseOrderDTO[i].ProductSubCategoryName == null)
                    //{
                    //    listPurchaseOrderDTO[i].ProductSubcategoryID = 0;
                    //}
                    if (listPurchaseOrderDTO[i].DeliveredBefore != null && listPurchaseOrderDTO[i].DeliveredBefore != "" && listPurchaseOrderDTO[i].DeliveredBefore != string.Empty)
                    {
                        //Items = "INSERT INTO tabmmsPurchaseorderdetails(orderid,vchpurchaseorderno,productid,productname,productcategoryid,categoryname,productsubcategoryid,subcategoryname,vchuom,vchorderuom,numorderedqty,datdeliverybefore,numrate,statusid, createdby, createddate,productcode)";
                        //Items = Items + " VALUES (" + PurchaseOrderDTO.OrderId + ",'" + ManageQuote(PurchaseOrderDTO.PurchaseOrderId) + "'," + listPurchaseOrderDTO[i].ProductID + ",'" + listPurchaseOrderDTO[i].ProductName + "'," + listPurchaseOrderDTO[i].ProductCategoryID + ",'" + listPurchaseOrderDTO[i].ProductCategoryName + "'," + listPurchaseOrderDTO[i].ProductSubcategoryID + ",'" + listPurchaseOrderDTO[i].ProductSubCategoryName + "','" + listPurchaseOrderDTO[i].Uom + "','" + listPurchaseOrderDTO[i].Uom + "'," + listPurchaseOrderDTO[i].Quantity + ",'" + FormatDate(listPurchaseOrderDTO[i].DeliveredBefore) + "'," + listPurchaseOrderDTO[i].EstimateRate + ",1," + PurchaseOrderDTO.createdby + ",current_timestamp,'" + listPurchaseOrderDTO[i].Productcode + "');";

                        Items = "INSERT INTO tabmmsPurchaseorderdetails(orderid,vchpurchaseorderno,productid,productname,productcategoryid,categoryname,productsubcategoryid,subcategoryname,vchuom,vchorderuom,numorderedqty,datdeliverybefore,numrate,statusid, createdby, createddate,productcode,taxtype, vatorcsttaxpercentage, vatorcsttaxamount, amount, excisetaxpercentage, excisetaxamount, cesstaxpercentage, cesstaxamount, shcesstaxpercentage,shcesstaxamount,vchdiscounttype,numdiscountvalue,discountamount,vchtaxtype)";
                        Items = Items + " VALUES (" + PurchaseOrderDTO.OrderId + ",'" + ManageQuote(PurchaseOrderDTO.PurchaseOrderId) + "'," + listPurchaseOrderDTO[i].ProductID + ",'" + listPurchaseOrderDTO[i].ProductName + "'," + listPurchaseOrderDTO[i].ProductCategoryID + ",'" + listPurchaseOrderDTO[i].ProductCategoryName + "'," + listPurchaseOrderDTO[i].ProductSubcategoryID + ",'" + listPurchaseOrderDTO[i].ProductSubCategoryName + "','" + listPurchaseOrderDTO[i].Uom + "','" + listPurchaseOrderDTO[i].Uom + "'," + listPurchaseOrderDTO[i].Quantity + ",'" + FormatDate(listPurchaseOrderDTO[i].DeliveredBefore) + "'," + listPurchaseOrderDTO[i].EstimateRate + ",1," + PurchaseOrderDTO.createdby + ",current_timestamp,'" + listPurchaseOrderDTO[i].Productcode + "','" + listPurchaseOrderDTO[i].vatorcst + "','" + listPurchaseOrderDTO[i].taxvatcst + "','" + listPurchaseOrderDTO[i].TaxvatorcstAmount + "','" + listPurchaseOrderDTO[i].Amount + "','" + listPurchaseOrderDTO[i].TaxExcisePercentage + "','" + listPurchaseOrderDTO[i].TaxExciseAmount + "','" + listPurchaseOrderDTO[i].TaxCESSPercentage + "','" + listPurchaseOrderDTO[i].TaxCESSAmount + "','" + listPurchaseOrderDTO[i].TaxSHCESSPercentage + "','" + listPurchaseOrderDTO[i].TaxSHCESSAmount + "','" + listPurchaseOrderDTO[i].ProductDiscountType + "','" + listPurchaseOrderDTO[i].ProductDiscountFlatPercentage + "','" + listPurchaseOrderDTO[i].ProductDiscountValue + "','" + listPurchaseOrderDTO[i].taxtype + "');";

                    }
                    else
                    {
                        // Items = "INSERT INTO tabmmsPurchaseorderdetails(orderid,vchpurchaseorderno,productid,productname,productcategoryid,categoryname,productsubcategoryid,subcategoryname,vchuom,vchorderuom,numorderedqty,numrate,statusid, createdby, createddate,productcode)";
                        // Items = Items + " VALUES (" + PurchaseOrderDTO.OrderId + ",'" + ManageQuote(PurchaseOrderDTO.PurchaseOrderId) + "'," + listPurchaseOrderDTO[i].ProductID + ",'" + listPurchaseOrderDTO[i].ProductName + "'," + listPurchaseOrderDTO[i].ProductCategoryID + ",'" + listPurchaseOrderDTO[i].ProductCategoryName + "'," + listPurchaseOrderDTO[i].ProductSubcategoryID + ",'" + listPurchaseOrderDTO[i].ProductSubCategoryName + "','" + listPurchaseOrderDTO[i].Uom + "','" + listPurchaseOrderDTO[i].Uom + "'," + listPurchaseOrderDTO[i].Quantity + "," + listPurchaseOrderDTO[i].EstimateRate + ",1," + PurchaseOrderDTO.createdby + ",current_timestamp,'" + listPurchaseOrderDTO[i].Productcode + "');";

                        Items = "INSERT INTO tabmmsPurchaseorderdetails(orderid,vchpurchaseorderno,productid,productname,productcategoryid,categoryname,productsubcategoryid,subcategoryname,vchuom,vchorderuom,numorderedqty,numrate,statusid, createdby, createddate,productcode,taxtype, vatorcsttaxpercentage, vatorcsttaxamount, amount, excisetaxpercentage, excisetaxamount, cesstaxpercentage, cesstaxamount, shcesstaxpercentage,shcesstaxamount,vchdiscounttype,numdiscountvalue,discountamount,vchtaxtype)";
                        Items = Items + " VALUES (" + PurchaseOrderDTO.OrderId + ",'" + ManageQuote(PurchaseOrderDTO.PurchaseOrderId) + "'," + listPurchaseOrderDTO[i].ProductID + ",'" + listPurchaseOrderDTO[i].ProductName + "'," + listPurchaseOrderDTO[i].ProductCategoryID + ",'" + listPurchaseOrderDTO[i].ProductCategoryName + "'," + listPurchaseOrderDTO[i].ProductSubcategoryID + ",'" + listPurchaseOrderDTO[i].ProductSubCategoryName + "','" + listPurchaseOrderDTO[i].Uom + "','" + listPurchaseOrderDTO[i].Uom + "'," + listPurchaseOrderDTO[i].Quantity + "," + listPurchaseOrderDTO[i].EstimateRate + ",1," + PurchaseOrderDTO.createdby + ",current_timestamp,'" + listPurchaseOrderDTO[i].Productcode + "','" + listPurchaseOrderDTO[i].vatorcst + "','" + listPurchaseOrderDTO[i].taxvatcst + "','" + listPurchaseOrderDTO[i].TaxvatorcstAmount + "','" + listPurchaseOrderDTO[i].Amount + "','" + listPurchaseOrderDTO[i].TaxExcisePercentage + "','" + listPurchaseOrderDTO[i].TaxExciseAmount + "','" + listPurchaseOrderDTO[i].TaxCESSPercentage + "','" + listPurchaseOrderDTO[i].TaxCESSAmount + "','" + listPurchaseOrderDTO[i].TaxSHCESSPercentage + "','" + listPurchaseOrderDTO[i].TaxSHCESSAmount + "','" + listPurchaseOrderDTO[i].ProductDiscountType + "','" + listPurchaseOrderDTO[i].ProductDiscountFlatPercentage + "','" + listPurchaseOrderDTO[i].ProductDiscountValue + "','" + listPurchaseOrderDTO[i].taxtype + "');";

                    }
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Items);
                }
                trans.Commit();
                IsSave = true;
            }
            catch (Exception ex)
            {

                EventLogger.WriteToErrorLog(ex, "PurchaseOrder");
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
            return IsSave;
        }
        #endregion




        #region ComboCreation

        public DataTable GetProductnames()
        {
            DataTable dt = new DataTable();
            string strGetData = string.Empty;
            try
            {
                strGetData = "select productid,productname,productcode,numsalerate,uomname from tabmmsproductmst where statusid=1;";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strGetData).Tables[0];
            }
            catch (Exception)
            {

                throw;
            }
            return dt;
        }

        public DataTable GetComboProductDetails(string pid)
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {
                strData = "select productid,productname,vchuomid,productcode,numsalerate,uomname from tabmmsproductmst where productid=" + Convert.ToInt32(pid) + " and statusid=1;";

                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];

            }
            catch (Exception)
            {

                throw;
            }
            return dt;
        }

        public string SaveComboDetails(List<ComboDetails> objmmsComboSaveDTO, string NOP, string AAMount, string CCost, string Cname, string Cdate)
        {
            string combocode = string.Empty;
            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();
            try
            {
                string GetComboName = string.Empty;
                GetComboName = "select count(*) from tabmmscombomst where comboname='" + Cname.ToUpper() + "' and statusid=1;";
                int nn = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, GetComboName));
                if (nn > 0)
                {
                    return "CMBB";
                }
                else
                {
                    string str = "select max(combocode) from tabmmscombomst";
                    string comboid = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, str).ToString();
                    if (comboid == string.Empty || comboid == "" || comboid == null)
                    {
                        combocode = "CB" + 1;
                    }
                    else
                    {
                        string strrecordidd = "select max(substring(combocode,3)::int)+1 from tabmmscombomst";
                        long id = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strrecordidd));
                        combocode = "CB" + id;
                    }
                    string strSaveCombo = "insert into tabmmscombomst(comboname,combocode,actualcost,combocost,noofproducts,statusid,createdby,createddate,datcombodate) values('" + Cname + "','" + combocode + "'," + Convert.ToDecimal(AAMount) + "," + Convert.ToDecimal(CCost) + "," + Convert.ToInt32(NOP) + ",1,1,current_timestamp,'" + Cdate + "') returning combocode";
                    string CC = (NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strSaveCombo)).ToString();
                    int count = 0;
                    for (int i = 0; i < objmmsComboSaveDTO.Count; i++)
                    {
                        string vchuom = string.Empty;
                        if (objmmsComboSaveDTO[i].uom == "KG")
                        {
                            vchuom = "KILOGRAMS";
                        }
                        else if (objmmsComboSaveDTO[i].uom == "NO.")
                        {
                            vchuom = "NUMBERS";
                        }

                        string strComboDetails = string.Empty;
                        strComboDetails = "insert into tabmmscombomstdetails(numunitrate,numamount,vchquantity,combocode,productid,productname,productcode,vchuom,statusid,createdby,createddate) values(" + Convert.ToDecimal(objmmsComboSaveDTO[i].rate) + ", " + Convert.ToDecimal(objmmsComboSaveDTO[i].amount) + ", " + Convert.ToDecimal(objmmsComboSaveDTO[i].qafc) + ",'" + CC + "', '" + objmmsComboSaveDTO[i].productid + "','" + objmmsComboSaveDTO[i].productname + "','" + objmmsComboSaveDTO[i].productcode + "','" + vchuom + "',1,1,current_timestamp)";
                        int DetailsSave = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strComboDetails);
                        if (DetailsSave != 0 && DetailsSave > 0)
                        {
                            count++;
                        }
                    }
                }
                trans.Commit();
            }
            catch (Exception Ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(Ex, "ComboSave");
                throw Ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return Cname;
        }

        public List<ComboDetails> GetComboDetails()
        {
            List<ComboDetails> objLBSD = new List<ComboDetails>();
            try
            {
                string strGetDetails = "select recordid,comboname,combocode,actualcost,combocost,noofproducts from tabmmscombomst where statusid=1;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strGetDetails);
                if (npgdr != null)
                {
                    while (npgdr.Read())
                    {
                        ComboDetails objBSD = new ComboDetails();
                        objBSD.combocode = npgdr["combocode"].ToString();
                        objBSD.comboname = npgdr["comboname"].ToString();
                        objBSD.actualcost = npgdr["actualcost"].ToString();
                        objBSD.combocost = npgdr["combocost"].ToString();
                        objBSD.noofproducts = npgdr["noofproducts"].ToString();
                        objLBSD.Add(objBSD);
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return objLBSD;
        }

        public List<ComboDetails> GetComboDetailsforUpdate(string CID)
        {
            List<ComboDetails> objLBSD = new List<ComboDetails>();
            try
            {
                string strGetDetails = "select comboname,combocost, productid,productname,productcode,case when vchuom='KILOGRAMS' then 'KG' when vchuom='NUMBERS' then 'NO.' end as vchuom,vchquantity,numunitrate,numamount from tabmmscombomstdetails td join tabmmscombomst tc on td.combocode=tc.combocode where tc.combocode='" + CID + "' and tc.statusid=1 and td.statusid=1;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strGetDetails);
                if (npgdr != null)
                {
                    while (npgdr.Read())
                    {
                        ComboDetails objBSD = new ComboDetails();
                        objBSD.comboname = npgdr["comboname"].ToString();
                        objBSD.productid = npgdr["productid"].ToString();
                        objBSD.productname = npgdr["productname"].ToString();
                        objBSD.productcode = npgdr["productcode"].ToString();
                        objBSD.uom = npgdr["vchuom"].ToString();
                        objBSD.qafc = npgdr["vchquantity"].ToString();
                        objBSD.rate = npgdr["numunitrate"].ToString();
                        objBSD.amount = npgdr["numamount"].ToString();
                        objBSD.combocost = npgdr["combocost"].ToString();
                        objLBSD.Add(objBSD);
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return objLBSD;
        }

        public string UpdateComboDetails(List<ComboDetails> objmmsComboUpdateDTO, string NOP, string AAMount, string CCost, string Cname, string Cdate, long Modifiedby, string ComboCode)
        {
            string combocode = string.Empty;
            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();
            try
            {
                string UpdateCombo = string.Empty;
                UpdateCombo = "UPDATE TABMMSCOMBOMST SET ACTUALCOST=" + Convert.ToDecimal(AAMount) + ",COMBOCOST=" + Convert.ToDecimal(CCost) + ",NOOFPRODUCTS=" + Convert.ToInt32(NOP) + ",MODIFIEDBY=" + Modifiedby + ",MODIFIEDDATE=CURRENT_TIMESTAMP WHERE COMBOCODE='" + ComboCode + "' AND STATUSID=1";
                int UpdateCount = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, UpdateCombo);
                int ProductCount = 0;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < objmmsComboUpdateDTO.Count; i++)
                {
                    if (i < objmmsComboUpdateDTO.Count - 1)
                    {
                        sb.Append("'" + objmmsComboUpdateDTO[i].productname.ToString() + "'" + ",");
                    }
                    else if (i == objmmsComboUpdateDTO.Count - 1)
                    {
                        sb.Append("'" + objmmsComboUpdateDTO[i].productname.ToString() + "'").ToString();
                    }
                }
                string strUpdateProducts = "update tabmmscombomstdetails set statusid=2 where combocode='" + ComboCode + "' and productname not in(" + sb + ") and statusid=1";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strUpdateProducts);
                for (int i = 0; i < objmmsComboUpdateDTO.Count; i++)
                {
                    string vchuom = string.Empty;
                    if (objmmsComboUpdateDTO[i].uom == "KG")
                    {
                        vchuom = "KILOGRAMS";
                    }
                    else if (objmmsComboUpdateDTO[i].uom == "NO.")
                    {
                        vchuom = "NUMBERS";
                    }
                    string GetCount = string.Empty;
                    GetCount = "Select count(*) from tabmmscombomstdetails where combocode='" + ComboCode + "' and productname='" + objmmsComboUpdateDTO[i].productname + "' and statusid=1";
                    ProductCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, GetCount));
                    if (ProductCount == 0 || ProductCount < 0)
                    {
                        string strComboDetails = string.Empty;
                        strComboDetails = "insert into tabmmscombomstdetails(numunitrate,numamount,vchquantity,combocode,productid,productname,productcode,vchuom,statusid,createdby,createddate,modifiedby,modifieddate) values(" + Convert.ToDecimal(objmmsComboUpdateDTO[i].rate) + ", " + Convert.ToDecimal(objmmsComboUpdateDTO[i].amount) + ", " + Convert.ToDecimal(objmmsComboUpdateDTO[i].qafc) + ",'" + ComboCode + "', '" + objmmsComboUpdateDTO[i].productid + "','" + objmmsComboUpdateDTO[i].productname + "','" + objmmsComboUpdateDTO[i].productcode + "','" + vchuom + "',1,1,current_timestamp," + Modifiedby + ",current_timestamp)";
                        int DetailsSave = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strComboDetails);
                    }
                    else if (ProductCount > 0)
                    {
                        string Ucount = string.Empty;
                        Ucount = "update tabmmscombomstdetails set vchquantity=" + objmmsComboUpdateDTO[i].qafc + ", numamount=" + objmmsComboUpdateDTO[i].amount + ", modifiedby=" + Modifiedby + ",modifieddate=current_timestamp where combocode='" + ComboCode + "'and statusid=1 and productname='" + objmmsComboUpdateDTO[i].productname + "' and productcode='" + objmmsComboUpdateDTO[i].productcode + "';";
                        int cnt = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Ucount);
                    }
                }
                trans.Commit();
            }
            catch (Exception Ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(Ex, "Update Combo Details");
                throw Ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return Cname;
        }


        public bool DeleteComboPackDetails(string Code)
        {
            bool IsValid = false;
            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();
            try
            {
                string strDelete = "update tabmmscombomst set statusid=2 where combocode='" + Code + "' and statusid=1";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strDelete);
                string strDetailsUpdate = "update tabmmscombomstdetails set statusid=2 where combocode='" + Code + "' and statusid=1";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strDetailsUpdate);
                IsValid = true;
                trans.Commit();

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "DeleteComboPackDetails");
                trans.Rollback();
                throw;
            }
            finally
            {

            }
            return IsValid;
        }


        #endregion

    }
}
