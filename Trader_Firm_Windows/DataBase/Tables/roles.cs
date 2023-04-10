using System.ComponentModel.DataAnnotations;

namespace Trader_Firm_Windows.DataBase.Tables;

public class roles
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }
}