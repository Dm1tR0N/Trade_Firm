using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Rendering;
using DynamicData;
using Npgsql;
using Trader_Firm_Windows.DataBase.Tables;
using static Trader_Firm_Windows.DataBase.Connection;

namespace Trader_Firm_Windows.Views;

public partial class MainWindow : Window
{
    public static bool isAuth = false; // false - Сотрдник не авторизирован, true - сотрудник авторизирован
    public static int number_attempts = 0; // Хранит количество не удачных входов в аккаунт
    public static int idUser = 0;
    public MainWindow()
    {
        InitializeComponent();
    }

    // Функция авторизации сотрудника
    private void LoginButton_Click(object? sender, RoutedEventArgs e)
    {
        Context _context = new Context();
        var user = _context.Users.SingleOrDefault(u => u.Name == LoginTextBox.Text && u.Password == PasswordBox.Text);

        if (user != null)
        {
            StatusBar.Text = $"Добро пожаловать {user.Name}!";
            idUser = user.Id;
            number_attempts = 0; // Обнуление попыток входа.
            isAuth = true;
        }
        else
        {
            if ( number_attempts >= 3)
            {
                StatusBar.Text = $"Вы не авторезированный уже {number_attempts} раза,\n" +
                                 $"Обратитесь к Администратору для востонавления доступа к аккаунту!";
            }
            else
            {
                StatusBar.Text = $"Не авторизирован!";
            }
            
            number_attempts++;
            PasswordBox.Text = "";
        }
    }

    // Добавления аккаунта
    public void AddUser(Context context, string UserName, string Passord, int Role)
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

    // Функциядля того что бы скрыть окно авторизации
    private void HideBorder_Click(object? sender, RoutedEventArgs e)
    {
        if (isAuth == true)
        {
            Auth.IsVisible = false; // Скрыть окно авторизации
            Menu.IsVisible = true; // Показать меню
            LoginTextBox.Text = "";
            PasswordBox.Text = "";
        }
        else
        {
            StatusBar.Text = "Для выхода нужно Авторизироватся!";
        }
    }


    private void AP_AddUser(object? sender, RoutedEventArgs e)
    {
        Context _context = new Context();
        var checkUser = _context.Users.SingleOrDefault(x => x.Id == idUser);
        var role = _context.Roles.SingleOrDefault(x => x.IdRoles == 1); // Роль администратора
        
        if (checkUser.Role == role) // Создавать аккаунты может только администратор
        {
            Menu.IsVisible = false;
            ApWinAddUser.IsVisible = true;
        }
        else
        {
            InfoBoxMenu.Text = $"К этой функции есть доступ только у Администрации!";
        }
    }

    private void AP_AddUser_Cancel(object? sender, RoutedEventArgs e)
    {
        ApWinAddUser.IsVisible = false;
        Menu.IsVisible = true;
    }

    private void AP_AddUser_Create(object? sender, RoutedEventArgs e)
    {
        Context _context = new Context();
        var checkUser = _context.Users.SingleOrDefault(x => x.Name == UserLogin.Text && x.Password == UserPass.Text);
        if (checkUser == null)
        {
            AddUser(_context, UserLogin.Text, UserPass.Text, Convert.ToInt32(UserRole.Text));
            InfoBox.Text = $"Аккаунт создан\nLogin: {UserLogin.Text}\nPassword: {UserPass.Text}\nДоя выхода нажмите 'Отмена'\n\n";

            UserLogin.Text = "";
            UserPass.Text = "";
            UserRole.Text = "";
        }
        else
        {
            InfoBox.Text = $"Создатьаккаунт не удалось!\nВозможно логин ({UserLogin.Text}) уже жанят.\n\n";
        }
        
    }

    private void LeaveAcc(object? sender, RoutedEventArgs e)
    {
        isAuth = false;
        Menu.IsVisible = false;
        Auth.IsVisible = true;
        InfoBoxMenu.Text = "";
    }
}