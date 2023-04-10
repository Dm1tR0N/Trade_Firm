using System;

namespace Trader_Firm_Windows.DataBase.Tables;

public class sales
{
    public int SaleId { get; set; }
    public stores StoreId { get; set; }
    public users UserId { get; set; }
    public DateTime SaleDate { get; set; }
}