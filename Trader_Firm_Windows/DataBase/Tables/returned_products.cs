namespace Trader_Firm_Windows.DataBase.Tables;

public class returned_products
{
    public returns ReturnId { get; set; }
    public products ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}