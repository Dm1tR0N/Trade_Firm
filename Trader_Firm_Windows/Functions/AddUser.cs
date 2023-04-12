using Trader_Firm_Windows.DataBase.Tables;
using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Trader_Firm_Windows.Functions;

public class AddFunctions
{
    // Создание аккаунта
    public static void AddUser(Context context, string UserName, string Passord, int Role)
    {
        Context _context = new Context();
        _context = context;
        // Роли:
        // Администратор 1,
        // Менеджер по снабжению 2,
        // Аудитор 3,
        // Сотрудник продаж 4,
        // Менеджер для каждого магазина 5
        
        _context = context;
        var _roles = _context.Roles.ToList();
        var _users = _context.Users.ToList();

        var newRole = new roles
        {
            Title = "Admin"
        };
        _roles.Add(newRole);

        _context.Add(newRole);
        _context.SaveChanges();

        var roleId = _context.Roles.SingleOrDefault(x => x.IdRoles == Role);

        var newUser = new users
        {
            Name = UserName,
            Password = Passord,
            Role = roleId
        };
        _users.Add(newUser);

        _context.Add(newUser);
        _context.SaveChanges();
    }

    // Создание магазина
    public static void AddStore(string StoreName, string StoreLocation, int userId)
    {
        Context _context = new Context();
        var storesList = _context.Stores.ToList();
       
        var meneger = _context.Users.SingleOrDefault(x => x.Id == userId);

        if (userId != null && StoreName != "" && StoreLocation != "")
        {
            var newStore = new stores
            {
                StoreName = StoreName,
                StoreLocation = StoreLocation,
                ManagerId = meneger
            };
            
            storesList.Add(newStore);
            _context.Add(newStore);
            _context.SaveChanges();
        }
        else
        {
            Debug.WriteLine($"Ошибка! Проверь правильность заполнения полей.");
        }
    }

    // Обновление данных пользователя
    public static void UpdateUser(int userId, int roleId, string userLogin, string userPass)
    {
        Context _context = new Context();
        var user = _context.Users.ToList();
        
        var selectUser = _context.Users.SingleOrDefault(x => x.Id == userId);
        
        var selectRole = _context.Roles.SingleOrDefault(x => x.IdRoles == roleId);

        if (selectUser != null && selectRole != null)
        {
            selectUser.Name = userLogin;
            selectUser.Password = userPass;
            selectUser.Role = selectRole;
            Debug.WriteLine($"Данные пользователя: {selectUser.Name} были изменены.");
        }
        else
        {
            Debug.WriteLine($"Ошибка! Что то не так, проверьте заполненны ли все поля!");
        }
        _context.SaveChanges();
    }
}