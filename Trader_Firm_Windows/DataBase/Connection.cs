using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Trader_Firm_Windows.DataBase.Tables;

namespace Trader_Firm_Windows.DataBase;

public static class Connection
{
    public static DbContextOptions Connect()
    {
        var connString = "Server=localhost;" +
                         "Port=5432;" +
                         "Database=TradeFirm1;" +
                         "Username=postgres;" +
                         "Password=1234;";

        return null;
    }
}