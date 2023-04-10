namespace Trader_Firm_Windows.DataBase.Tables;

public class sold_products
{
    public sales SaleId { get; set; }
    public products ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}