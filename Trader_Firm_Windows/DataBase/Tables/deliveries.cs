using System;

namespace Trader_Firm_Windows.DataBase.Tables;

public class deliveries
{
    public int DeliveryId { get; set; }
    public stores StoreId { get; set; }
    public users UserId { get; set; }
    public DateTime DeliveryDate { get; set; }
}