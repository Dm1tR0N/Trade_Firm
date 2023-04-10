namespace Trader_Firm_Windows.DataBase.Tables;

public class stores
{
    public int StoreId { get; set; }
    public string StoreName { get; set; }
    public string StoreLocation { get; set; }
    public users ManagerId { get; set; }
}