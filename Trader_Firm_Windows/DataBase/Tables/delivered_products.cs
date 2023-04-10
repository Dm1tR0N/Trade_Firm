namespace Trader_Firm_Windows.DataBase.Tables;

public class delivered_products
{
    public deliveries DeliveryId { get; set; }
    public products ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}