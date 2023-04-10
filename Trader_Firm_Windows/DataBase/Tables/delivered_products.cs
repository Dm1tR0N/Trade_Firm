using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Trader_Firm_Windows.DataBase.Tables;

public class delivered_products
{
    [Key]
    public deliveries DeliveryId { get; set; }
    [Key]
    public products ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}