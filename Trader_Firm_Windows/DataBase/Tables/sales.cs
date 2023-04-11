using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Trader_Firm_Windows.DataBase.Tables;

public class sales
{
    [Key]
    public int SaleId { get; set; }

    public stores? StoreId { get; set; }

    public users? UserId { get; set; }
    public DateTime SaleDate { get; set; }
}