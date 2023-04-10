using System.ComponentModel.DataAnnotations;

namespace Trader_Firm_Windows.DataBase.Tables;

public class store_products
{
    [Key]
    public stores StoreId { get; set; }
    [Key]
    public products ProductId { get; set; }
    public int Quantity { get; set; }
}