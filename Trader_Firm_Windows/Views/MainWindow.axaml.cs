using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Rendering;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Trader_Firm_Windows.DataBase.Tables;
using Trader_Firm_Windows.Functions;
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
        Context _context = new Context();
        var stores = _context.Stores.ToList();
        StoreMenu.Items = stores;
    }

    // 1 Функция авторизации сотрудника
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

    // 2 Добавления аккаунта
    

    // 3 Функциядля того что бы скрыть окно авторизации
    private void HideBorder_Click(object? sender, RoutedEventArgs e)
    {
        if (isAuth == true)
        {
            Auth.IsVisible = false; // Скрыть окно авторизации
            Menu.IsVisible = true; // Показать меню
            StoreInfo.IsVisible = true;
            LoginTextBox.Text = "";
            PasswordBox.Text = "";
        }
        else
        {
            StatusBar.Text = "Для выхода нужно Авторизироватся!";
        }
    }


    // 4 Кнока в меню для открытия окна создания сотрудника
    private void AP_AddUser(object? sender, RoutedEventArgs e)
    {
        Context _context = new Context();
        var checkUser = _context.Users.SingleOrDefault(x => x.Id == idUser);
        var role = _context.Roles.SingleOrDefault(x => x.IdRoles == 1); // Роль администратора
        
        if (checkUser.Role == role) // Создавать аккаунты может только администратор
        {
            Menu.IsVisible = false;
            StoreInfo.IsVisible = false;
            ApWinAddUser.IsVisible = true;
        }
        else
        {
            InfoBoxMenu.Text = $"К этой функции есть доступ только у Администрации!";
        }
    }

    // 5 Кнопка для закрытия окна создания сотрудника
    private void AP_AddUser_Cancel(object? sender, RoutedEventArgs e)
    {
        ApWinAddUser.IsVisible = false;
        Menu.IsVisible = true;
        StoreInfo.IsVisible = true;
    }

    // 6 Кнопка что бы создать аккаунт сотрудника
    private void AP_AddUser_Create(object? sender, RoutedEventArgs e)
    {
        Context _context = new Context();
        var checkUser = _context.Users.SingleOrDefault(x => x.Name == UserLogin.Text && x.Password == UserPass.Text);
        if (checkUser == null)
        {
            AddFunctions.AddUser(_context, UserLogin.Text, UserPass.Text, Convert.ToInt32(UserRole.Text));
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

    // 6 Кнопка что бы выйти из аккаунта
    private void LeaveAcc(object? sender, RoutedEventArgs e)
    {
        isAuth = false;
        Menu.IsVisible = false;
        Auth.IsVisible = true;
        StoreInfo.IsVisible = false;
        InfoBoxMenu.Text = "";
        StoreInfo.IsVisible = false;
    }

    // 7 Кнопка в меню для открытия окна создания продукта
    private void Add_Product(object? sender, RoutedEventArgs e)
    {
        Context _context = new Context();
        var checkUser = _context.Users.SingleOrDefault(x => x.Id == idUser);
        var role = _context.Roles.SingleOrDefault(x => x.IdRoles == 1); // Роль администратора
        
        if (checkUser.Role == role) // Создавать аккаунты может только администратор
        {
            AddProductWin.IsVisible = true;
            StoreInfo.IsVisible = false;
            Menu.IsVisible = false;
        }
        else
        {
            InfoBoxMenu.Text = $"К этой функции есть доступ только у Администрации!";
        }
    }
    
    // 9 Функиця создания продукта
    public void AddProduct(string Title, decimal Price, string Discription)
    {
        Context _context = new Context();
        var productsList = _context.Products.ToList();

        var newProduct = new products
        {
            ProductName = Title,
            Description = Discription,
            Price = Price
        };

        productsList.Add(newProduct);
        _context.Add(newProduct);
        _context.SaveChanges();
    }

    // 10 Кнопка закрыть окно создания продукта
    private void AddProductWin_Cancel(object? sender, RoutedEventArgs e)
    {
        AddProductWin.IsVisible = false;
        Menu.IsVisible = true;
        StoreInfo.IsVisible = true;
    }

    // 11 Кнопка для создания продукта
    private void AddProductWin_Create(object? sender, RoutedEventArgs e)
    {
        Context _context = new Context();
        var checkUser = _context.Users.SingleOrDefault(x => x.Id == idUser);
        var role = _context.Roles.SingleOrDefault(x => x.IdRoles == 1); // Роль администратора
        
        if (checkUser.Role == role) // Создавать аккаунты может только администратор
        {
            AddProduct(ProductName.Text, Convert.ToDecimal(ProductPrice.Text), ProductDiscription.Text);
            ProductInfoBox.Text = $"Продукт: {ProductName.Text} Стоимостью {ProductPrice.Text}\nДля выхода нажмите 'Отмена'\n\n";

            ProductName.Text = "";
            ProductDiscription.Text = "";
            ProductPrice.Text = "";
        }
        else
        {
            InfoBoxMenu.Text = $"К этой функции есть доступ только у Администрации!";
        }
    }

    // 12 Открть окно для создания магазина
    private void AP_AddStore(object? sender, RoutedEventArgs e)
    {
        Context _context = new Context();
        var checkUser = _context.Users.SingleOrDefault(x => x.Id == idUser);
        var role = _context.Roles.SingleOrDefault(x => x.IdRoles == 1); // Роль администратора
        
        if (checkUser.Role == role) // Создавать аккаунты может только администратор
        {
            Menu.IsVisible = false;
            AddStoreWin.IsVisible = true;
            StoreInfo.IsVisible = false;
            var users = _context.Users.Where(x => x.Role.IdRoles == 2 || x.Role.IdRoles == 5).ToList();
            StoreMeneger.Items = users.ToList();
        }
        else
        {
            InfoBoxMenu.Text = $"К этой функции есть доступ только у Администрации!";
        }
    }

    // 13 Кнопка для закрытия окна Создания магазина
    private void AddStoreWin_Cancel(object? sender, RoutedEventArgs e)
    {
        Menu.IsVisible = true;
        AddStoreWin.IsVisible = false;
        StoreInfo.IsVisible = true;
    }

    // 14 Кнопка для создания магазина
    private void AddStoreWin_Create(object? sender, RoutedEventArgs e)
    {
        try
        {
            var selectedUser = (StoreMeneger.SelectedItem as users).Id;
            if (selectedUser != null)
            {
                AddFunctions.AddStore(StoreName.Text, StoreLocation.Text, selectedUser);
                Context _context = new Context();
                var stores = _context.Stores.ToList();
                StoreMenu.Items = stores;
                AddStoreInfoBox.Text = "Магазин создан!";
                StoreName.Text = "";
                StoreLocation.Text = "";
            }
            else
            {
                AddStoreInfoBox.Text = "Ошибка!";
                StoreName.Text = "";
                StoreLocation.Text = "";
            }
        }
        catch (Exception exception)
        {
            AddStoreInfoBox.Text = "Критическая ошибка!";
            StoreName.Text = "";
            StoreLocation.Text = "";
        }
    }


    private void AP_UpUser(object? sender, RoutedEventArgs e)
    {
        Menu.IsVisible = false;
        UpdateWinAddUser.IsVisible = true;
        Context _context = new Context();
        var users = _context.Users.ToList();
        var roles = _context.Roles.ToList();

        UpdateUser.Items = users;
        UpUserRole.Items = roles;
        StoreInfo.IsVisible = false;
    }

    private void AP_UpUser_Cancel(object? sender, RoutedEventArgs e)
    {
        Menu.IsVisible = true;
        UpdateWinAddUser.IsVisible = false;
        StoreInfo.IsVisible = true;
    }

    private void AP_UpUser_Create(object? sender, RoutedEventArgs e)
    {
        try
        {
            Context _context = new Context();
            var user = _context.Users.ToList();
        
            var selectedUser = (UpdateUser.SelectedItem as users).Id;
            var selectUser = _context.Users.SingleOrDefault(x => x.Id == selectedUser);

            var selectedRole = (UpUserRole.SelectedItem as roles).IdRoles;
            var selectRole = _context.Roles.SingleOrDefault(x => x.IdRoles == selectedRole);

            if (selectedUser != null && selectedRole != null)
            {
                AddFunctions.UpdateUser(selectedUser, selectedRole, UserLogin.Text, UserPass.Text);

                UserLogin.Text = "";
                UserPass.Text = "";
                UpInfoBox.Text = "Успешно!";
            }
            else
            {
                UserLogin.Text = "";
                UserPass.Text = "";
                UpInfoBox.Text = "Ошибка!";
            }
        }
        catch (Exception exception)
        {
            UpInfoBox.Text = "Критическа ошибка!";
        }
    }

    private void SelectStore(object? sender, RoutedEventArgs e)
    {
        Context _context = new Context();

        if (StoreMenu.SelectedItem != null)
        {
            var store = (StoreMenu.SelectedItem as stores).StoreId;
            var storeP = StoreMenu.SelectedItem as stores;

            var storeInfo = _context.Stores.Include(x => x.ManagerId).SingleOrDefault(x => x.StoreId == store);

            TitleBlock.Text = "Склад магазина\n";
            StoreInfo_NameStore.Text = $"Название магазина: {storeInfo.StoreName}";
            StoreInfo_StoreLocation.Text = $"Адрес магазина: {storeInfo.StoreLocation}";
            StoreInfo_Meneger.Text = $"Менеджер магазина: {storeInfo.ManagerId.Name}";
        
            var products = _context.StoreProducts.Include(sp => sp.ProductId).Where(x => x.StoreId == storeP).ToList();
            ProductsListStore.Text = "";
            NameProductList.Text = "Продукты магазина";
            foreach (var item in products)
            {
                ProductsListStore.Text += $"Продукт: {item.ProductId.ProductName} - Количество: {item.Quantity}\n";
            }
        }
        else
        {
            InfoBoxMenu.Text = $"Сначала нужно выбрать магазин.";
        }

    }

    private void AP_UpStore(object? sender, RoutedEventArgs e)
    {
        UpdateStore.IsVisible = true;
        Menu.IsVisible = false;
        StoreInfo.IsVisible = false;
    }

    private void AP_UpStore_Cancel(object? sender, RoutedEventArgs e)
    {
        Menu.IsVisible = true;
        StoreInfo.IsVisible = true;
        UpdateStore.IsVisible = false;
    }
}