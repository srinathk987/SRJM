using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Core
{
   public class RMSMasters
    {


    }
   #region Rms


   public class RmsDTO
   {

       public string ProductName { set; get; }

       public string UOM { set; get; }

       public string ReceipeUOM { set; get; }

       public int Qty { set; get; }

       public string ConversionUom { set; get; }

       public string ProductCode { set; get; }

       public string GroupName { set; get; }

       public string ItemId { set; get; }

       public string ItemName { set; get; }

       public string PreparationSteps { set; get; }


       public string RMSNO { set;get;}

       public int Detailsid { set; get; }

       // public string Productid { set; get; }



   }






   #endregion
}
