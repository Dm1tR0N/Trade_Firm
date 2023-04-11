using System.ComponentModel.DataAnnotations;

namespace Trader_Firm_Windows.DataBase.Tables;

public class roles
{
    [Key]
    public int IdRoles { get; set; }
    public string Title { get; set; }
}