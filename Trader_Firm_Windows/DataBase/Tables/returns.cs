using System;

namespace Trader_Firm_Windows.DataBase.Tables;

public class returns
{
    public int ReturnId { get; set; }
    public stores StoreId { get; set; }
    public users UserId { get; set; }
    public DateTime ReturnDate { get; set; }
}