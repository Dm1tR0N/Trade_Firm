using System.ComponentModel.DataAnnotations;

namespace Trader_Firm_Windows.DataBase.Tables;

public class sold_products
{
    [Key]
    public sales SaleId { get; set; }
    [Key]
    public products ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}