using System.ComponentModel.DataAnnotations;

namespace Trader_Firm_Windows.DataBase.Tables;

public class products
{
    [Key]
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
}