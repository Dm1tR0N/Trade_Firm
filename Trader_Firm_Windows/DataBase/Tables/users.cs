namespace Trader_Firm_Windows.DataBase.Tables;

public class users
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public roles Role { get; set; }
}