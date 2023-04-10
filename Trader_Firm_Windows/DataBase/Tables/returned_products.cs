using System.ComponentModel.DataAnnotations;

namespace Trader_Firm_Windows.DataBase.Tables;

public class returned_products
{
    [Key]
    public returns ReturnId { get; set; }
    [Key]
    public products ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}