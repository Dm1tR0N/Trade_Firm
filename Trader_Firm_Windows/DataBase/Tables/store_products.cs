﻿namespace Trader_Firm_Windows.DataBase.Tables;

public class store_products
{
    public stores StoreId { get; set; }
    public products ProductId { get; set; }
    public int Quantity { get; set; }
}