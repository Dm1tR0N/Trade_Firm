using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Trader_Firm_Windows.DataBase.Tables;

public class users
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public roles Role { get; set; }
}