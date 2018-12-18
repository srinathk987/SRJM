using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace RMS.Core.Interfaces
{
    public interface ITakeAway
    {
        List<ItemsDTO> ShowItemsForSession();

        int SaveHomeDelivery(string Session, CustomerDetails CustomerDetails, List<ItemsDTO> ItemDetails);

        DataTable DeliveryItemsForCustomer();

        string UpdateeliveryStatus(string OderNo, string Status, string DeliveryBoydetails, string createdby);

        int SaveHomeDeliveryCharges(DeliveryChargesDTO DeliveryCharges);

        DataTable DeliveryChargesDetails();

        bool UpdateDeliveryCharges(DeliveryChargesDTO country);

        bool DeleteDeliveryCharges(DeliveryChargesDTO DeliveryCharges);

        bool UpdateReturnStatus(string OrderReason, string Nameofcust, string OrderNo, string contactno, decimal netamount);

        int SaveTakeAwayAthotel(string session, CustomerDetails CustomerDetails, List<ItemsDTO> ItemDetails, out string billNo, out string take);

        string getdeliveryCharges(string c);



        DataTable getCustomerInformation(string MobileNo);

        DataTable GetDeliveryBoyDetails();

        DataTable GetItemsOfSpecicOrder(string orderNo);

        DataTable DeliveryItemsForCustomerInTakeAway();

        #region ThirdParty

        bool SaveThirdPartyVendor(ThirdParty ThirdDetails);

        #endregion

        DataTable getImagesOfThirParty();

        int SaveThirdPartyNewOrderDetails(string session, CustomerDetails CustomerDetails, List<ItemsDTO> ItemDetails, out string billNo, out string TAKEAWAYID);

        DataTable GetThirdPartyOders(string ThirdpartyRecorId, string ThirdpartyId);

        bool UpdateDliveryStatusInThirdPartyOrders(string OderNo, string Status, string statusid, string createdby);

        DataTable GetItemsOfThirdPartySpecicOrder(string orderNo);

        DataTable GetThirdPartyOdersWeekWiseAndMonthWise(string ThirdpartyRecorId, string ThirdpartyId, string Wise);

        DataTable GetThirdpartyNames();

        DataTable GetThirdpartyBills(string Date, string ThirdpartyId);



        bool saveThirdPartyBillsettlement(Thirpartysettlement ThirdT, List<Thirpartysettlement> ThirdDetailsTD);

        DataTable getBillToCompanyBills(string CompanyName);

        bool SaveBillTocomanyBills(Thirpartysettlement ThirdT, List<Thirpartysettlement> ThirdDetailsTD);

        



        DataTable GetBillToCompanynames();
    }
}
