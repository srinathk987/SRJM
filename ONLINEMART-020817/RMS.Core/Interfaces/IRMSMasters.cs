using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Core.Interfaces
{
   public interface IRMSMasters
    {

        #region RMS

        DataTable GetItemNames();
        DataTable GetCategoryAndSubCategory(string ItemName);

        DataTable GetProducts();

        List<RmsDTO> ShowProductCategory(string ProductNames);
        bool SaveRMSDetails(RmsDTO RmsDTOdetails, List<RmsDTO> lstRmsDTO);
        DataTable GetItemsFromRms();
        List<RmsDTO> ShowItmCategory(string ItmNames);
        #endregion
    }
}
