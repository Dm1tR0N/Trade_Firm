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
    decimal totalCost = 0; // Хранит сумму чека при добовлении товара на склад магазина
    
    public void updateListStores()
    {
        Context _context = new Context();
        var stores = _context.Stores.ToList();
        StoreMenu.Items = stores;
    }

    public MainWindow()
    {
        InitializeComponent();
        updateListStores();
        #if DEBUG
        this.AttachDevTools();
        #endif
        this.WindowState = WindowState.Maximized;
    }

    // 1 Функция авторизации сотрудника
    private void LoginButton_Click(object? sender, RoutedEventArgs e)
    {
        Context _context = new Context();
        var user = _context.Users.Include(x => x.Role).SingleOrDefault(u => u.Name == LoginTextBox.Text && u.Password == PasswordBox.Text);

        if (user != null)
        {
            StatusBar.Text = $"Добро пожаловать {user.Name}!";
            InfoRole.Text = $"Ваша должность: {user.Role.Title}";
            idUser = user.Id;
            number_attempts = 0; // Обнуление попыток входа.
            isAuth = true;
            Auth.IsVisible = false; // Скрыть окно авторизации
            Menu.IsVisible = true; // Показать меню
            StoreInfo.IsVisible = true;
            LoginTextBox.Text = "";
            PasswordBox.Text = "";
        }
        else
        {
            if (number_attempts >= 3)
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
            InfoBoxMenu.Text = $"Отказано в доступе!";
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
            InfoBox.Text =
                $"Аккаунт создан\nLogin: {UserLogin.Text}\nPassword: {UserPass.Text}\nДоя выхода нажмите 'Отмена'\n\n";

            UserLogin.Text = "";
            UserPass.Text = "";
            UserRole.Text = "";
        }
        else
        {
            InfoBox.Text = $"Создать аккаунт не удалось!\nВозможно логин ({UserLogin.Text}) уже жанят.\n\n";
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
            InfoBoxMenu.Text = $"Отказано в доступе!";
        }
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
            AddFunctions.AddProduct(ProductName.Text, Convert.ToDecimal(ProductPrice.Text), ProductDiscription.Text);
            ProductInfoBox.Text =
                $"Продукт: {ProductName.Text} Стоимостью {ProductPrice.Text}\nДля выхода нажмите 'Отмена'\n\n";

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
            InfoBoxMenu.Text = $"Отказано в доступе!";
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
                updateListStores();
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
        Context _context = new Context();
        var checkUser = _context.Users.SingleOrDefault(x => x.Id == idUser);
        var role = _context.Roles.SingleOrDefault(x => x.IdRoles == 1); // Роль администратора

        if (checkUser.Role == role) // Создавать аккаунты может только администратор
        {
            Menu.IsVisible = false;
            UpdateWinAddUser.IsVisible = true;
            var users = _context.Users.ToList();
            var roles = _context.Roles.ToList();

            UpdateUser.Items = users;
            UpUserRole.Items = roles;
            StoreInfo.IsVisible = false;
        }
        else
        {
            InfoBoxMenu.Text = $"К этой функции есть доступ только у Администрации!";
        }
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

            if (selectedUser != null && selectedRole != null && UserLogin.Text != "" && UserPass.Text != "")
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
        Context _context = new Context();
        var checkUser = _context.Users.SingleOrDefault(x => x.Id == idUser);
        var role = _context.Roles.SingleOrDefault(x => x.IdRoles == 1); // Роль администратора

        if (checkUser.Role == role) // Создавать аккаунты может только администратор
        {
            UpdateStore.IsVisible = true;
            Menu.IsVisible = false;
            StoreInfo.IsVisible = false;
            var store = _context.Stores.ToList();
            var menegers = _context.Users.Where(x => x.Role.IdRoles == 2).ToList();

            UpStoreId.Items = store;
            UpStoreMeneger.Items = menegers;
        }
        else
        {
            InfoBoxMenu.Text = $"Отказано в доступе!";
        }
    }

    private void AP_UpStore_Cancel(object? sender, RoutedEventArgs e)
    {
        Menu.IsVisible = true;
        StoreInfo.IsVisible = true;
        UpdateStore.IsVisible = false;
    }

    private void AP_UpStore_Update(object? sender, RoutedEventArgs e)
    {
        try
        {
            var storeId = (UpStoreId.SelectedItem as stores).StoreId;
            var storeMeneger = (UpStoreMeneger.SelectedItem as users).Id;

            var newStoreName = UpStoreName.Text;
            var newStoreLocation = UpStoreLocation.Text;

            if (storeId != null && storeMeneger != null && newStoreName != "" && newStoreLocation != "")
            {
                AddFunctions.UpdateStore(storeId, storeMeneger, newStoreName, newStoreLocation);
                updateListStores();
                UpStoreInfoBox.Text = $"Данные обновлены!";
                UpStoreName.Text = "";
                UpStoreLocation.Text = "";
            }
            else
            {
                UpStoreInfoBox.Text = "Ошибка! Проверьте все ли поля заолнены.";
                UpStoreName.Text = "";
                UpStoreLocation.Text = "";
            }
        }
        catch (Exception exception)
        {
            UpStoreInfoBox.Text = "Критическая ошибка! Проверьте все ли поля заолнены.";
            UpStoreName.Text = "";
        }
    }
    
    private void Add_Product_Store(object? sender, RoutedEventArgs e)
    {
        Context _context = new Context();
        if (StoreMenu.SelectedItem != null)
        {
            var selectedStore = (StoreMenu.SelectedItem as stores).StoreId;
            var menegerStore = _context.Users.SingleOrDefault(x => x.Id == idUser);
            var storeInfo = _context.Stores.SingleOrDefault(x => x.StoreId == selectedStore && x.ManagerId == menegerStore);
            if (storeInfo == null)
            {
                InfoBoxMenu.Text = "Отказано в доступе!";
            }
            else
            {
                var products = _context.Products.ToList();
                AddProductStore_Product.Items = products;
                AddProductStore.IsVisible = true;
                Menu.IsVisible = false;
                StoreInfo.IsVisible = false;
            }
        }
        else
        {
            InfoBoxMenu.Text = "Нужно выбрать магазин!";
        }
    }

    private void Menu_AddProductStore_Cancel(object? sender, RoutedEventArgs e)
    {
        AddProductStore.IsVisible = false;
        Menu.IsVisible = true;
        StoreInfo.IsVisible = true;
        AddProductStore_InfoBox.Text = "";
        AddProductStore_TotalCost.Text = "";
    }

    private void Menu_AddProductStore_Add(object? sender, RoutedEventArgs e)
    {
        int idProduct = (AddProductStore_Product.SelectedItem as products).ProductId;
        Context _context = new Context();
        var selectedProduct = _context.Products.SingleOrDefault(x => x.ProductId == idProduct);
        var selectedStore = _context.Stores.SingleOrDefault(x => x.StoreId == (StoreMenu.SelectedItem as stores).StoreId);

        AddProductStore_InfoBox.Text += $"Продукт: {selectedProduct.ProductName}, Количество: {AddProductStore_CuounProduct.Text}\n";

        int count = _context.StoreProducts.Count();
        Debug.WriteLine($"Количество записей StoreProducts: {count}");

        if (selectedStore != null && selectedProduct != null && !string.IsNullOrEmpty(AddProductStore_CuounProduct.Text))
        {
            AddFunctions.AddProductStore(selectedStore.StoreId, selectedProduct.ProductId, Convert.ToInt32(AddProductStore_CuounProduct.Text));
        }

        totalCost += Convert.ToDecimal(AddProductStore_CuounProduct.Text) * selectedProduct.Price;
        AddProductStore_TotalCost.Text = $"Итого: {totalCost}₽";
    }

    private void Delete_Product_Store(object? sender, RoutedEventArgs e)
    {
        Context _context = new Context();
        var menegerStore = _context.Users.SingleOrDefault(x => x.Id == idUser);
        var selectedStoreCheck = (StoreMenu.SelectedItem as stores).StoreId;
        var storeInfoCheck = _context.Stores.SingleOrDefault(x => x.StoreId == selectedStoreCheck && x.ManagerId == menegerStore);
        
        if (storeInfoCheck == null)
        {
            InfoBoxMenu.Text = "Отказано в доступе!";
        }
        else if (StoreMenu.SelectedItem != null && StoreMenu.SelectedItem != null)
        {
            var storeInfo = _context.Stores.SingleOrDefault(x => x.StoreId == (StoreMenu.SelectedItem as stores).StoreId && x.ManagerId == menegerStore);
            var selectedStore = _context.Stores.SingleOrDefault(x => x.StoreId == (StoreMenu.SelectedItem as stores).StoreId);
            var products = _context.StoreProducts.Where(x => x.StoreId == selectedStore).Select(x => x.ProductId).ToList();
            var list = _context.StoreProducts.Include(x => x.ProductId).Where(x => x.StoreId == selectedStore).ToList();
            Delete_Product_Store_Product.Items = products;

            Delete_Product_Store_InfoSklad.Text = $"Склад магазина: {selectedStore.StoreName}\n";
            foreach (var item in list)
            {
                Delete_Product_Store_InfoSklad.Text += $"{item.ProductId.ProductName} в количестве {item.Quantity}\n";
            }
            
            Delete_Product_Store_StakPanel.IsVisible = true;
            Menu.IsVisible = false;
            InfoBox.IsVisible = false;
        }
        else
        {
            InfoBoxMenu.Text = "Выберите магазин!";
        }
    }

    private void Delete_Product_Store_StakPanel_Cancel(object? sender, RoutedEventArgs e)
    {
        Delete_Product_Store_StakPanel.IsVisible = false;
        Menu.IsVisible = true;
        InfoBox.IsVisible = true;
    }

    private void Delete_Product_Store_StakPanel_Delete(object? sender, RoutedEventArgs e)
    {
        Context _context = new Context();
        var del = _context.StoreProducts.Include(x => x.ProductId).SingleOrDefault(x => x.StoreId == (StoreMenu.SelectedItem as stores) && x.ProductId == (Delete_Product_Store_Product.SelectedItem as products));
        
        if (del != null)
        {
            _context.StoreProducts.Remove(del);
            _context.SaveChanges();
            Delete_Product_Store_InfoBox.Text = $"{del.ProductId.ProductName} была удалено со склада";
            
            var selectedStore = _context.Stores.SingleOrDefault(x => x.StoreId == (StoreMenu.SelectedItem as stores).StoreId);
            var list = _context.StoreProducts.Include(x => x.ProductId).Where(x => x.StoreId == selectedStore).ToList();
            Delete_Product_Store_InfoSklad.Text = $"Склад магазина: {selectedStore.StoreName}\n";
            foreach (var item in list)
            {
                Delete_Product_Store_InfoSklad.Text += $"{item.ProductId.ProductName} в количестве {item.Quantity}\n";
            }
        }
        else
        {
            Delete_Product_Store_InfoBox.Text = $"{del.ProductId.ProductName} не удалось удалить.";
        }
    }
    private async void Sold_Product_Store(object? sender, RoutedEventArgs e)
    {
        Context _context = new Context();
        
        var store = (StoreMenu.SelectedItem as stores);
        
            if (store == null)
            {
                InfoBoxMenu.Text = "Нужно выбрать магазин!";
                return;
            }
        
            var products = await _context.StoreProducts
                .Include(x => x.ProductId)
                .Include(x => x.StoreId)
                .Where(x => x.StoreId == store)
                .Select(x => new { x.ProductId, x.ProductId.ProductName, x.ProductId.Price, x.Quantity, x.StoreId, x.IdStoreProducts })
                .ToListAsync();
        
            Sold_Product_Store_StakPanel_List.DataContext = products;
            Sold_Product_Store_StakPanel.IsVisible = true;
            Menu.IsVisible = false;
            InfoBox.IsVisible = false;
    }

    private void Sold_Product_Store_StakPanel_List_Add(object? sender, RoutedEventArgs e)
    {
        try
        {
            Context _context = new Context();
            var selectedProduct = Sold_Product_Store_StakPanel_List.SelectedItem as dynamic;
            int productId = selectedProduct.ProductId.ProductId;


            
            var selectProdect2 = _context.Products.SingleOrDefault(x =>
                x.ProductId == productId);
            
            var storeSelect = StoreMenu.SelectedItem as stores;
            
            var product = _context.Products.SingleOrDefault(x =>
                x.ProductName == selectProdect2.ProductName).ProductId;
            
            var store = _context.Stores.SingleOrDefault(x => x.StoreName == storeSelect.StoreName).StoreId;
            
            AddFunctions.SellProduct(
                product, 
                Convert.ToInt32(Sold_Product_Store_StakPanel_CuounProduct.Text), 
                store,
                idUser);

            Sold_Product_Store_StakPanel_InfoBox.Text += $"{selectProdect2.ProductName} - {Sold_Product_Store_StakPanel_CuounProduct.Text} * {selectProdect2.Price}\n";
            totalCost += Convert.ToDecimal(Sold_Product_Store_StakPanel_CuounProduct.Text) * selectProdect2.Price;
            Sold_Product_Store_StakPanel_TotalCost.Text = $"Итого: {totalCost}₽";
        }
        catch (Exception exception)
        {
            Sold_Product_Store_StakPanel_InfoBox.Text = $"{exception.Message}";
        }
    }

    private void Sold_Product_Store_StakPanel_Cancel(object? sender, RoutedEventArgs e)
    {
        Sold_Product_Store_StakPanel.IsVisible = false;
        Menu.IsVisible = true;
        InfoBox.IsVisible = true;

        Context _context = new Context();
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

    private void Return_Products_To_Store(object? sender, RoutedEventArgs e)
    {
        // Пока что не работает, в будешум это должна быть кнопка для открытия формы возврата
        Context _context = new Context();
        var storeP = StoreMenu.SelectedItem as stores;
        var user = _context.Users.SingleOrDefault(x => x.Id == idUser);
        var soldproduct = _context.SoldProducts.Include(x => x.ProductId).SingleOrDefault(x => x.IdSoldProducts == 1);
        var sale = _context.Sales.SingleOrDefault(x => x.SaleId == 1);
        
        
        // AddFunctions.ReturnProduct(soldproduct.ProductId.ProductId, 1, storeP.StoreId, idUser);
        ReturnProduct.IsVisible = true;
        Menu.IsVisible = false;
        InfoBox.IsVisible = false;

       var listSales = _context.Sales
           .Where(x => x.UserId == _context.Users.SingleOrDefault(u => u.Id == idUser) && x.StoreId == storeP)
           .ToList();
       
       //var listProducts = _context.SoldProducts
           // .Include(x => x.SaleId)
           //     .ThenInclude(s => s.StoreId)
           // .Include(x => x.ProductId)
           // .Where(x => listSales.Any(s => s.SaleId == x.SaleId.SaleId))
           // .ToList();

       var products = _context.SoldProducts
           .Include(x => x.ProductId)
           .Where(x => x.SaleId.UserId == _context.Users.SingleOrDefault(x => x.Id == idUser) &&
                       DateTime.Compare(x.SaleId.SaleDate.AddDays(14), DateTime.Now.ToUniversalTime()) >= 0 &&
                       x.Quantity > 0)
           .ToList();
       
       ReturnProduct_StakPanel_List.Items = products;


    }

    private void ReturnProduct_Cancel(object? sender, RoutedEventArgs e)
    {
        ReturnProduct.IsVisible = false;
        Menu.IsVisible = true;
        InfoBox.IsVisible = true;

        SelectStore(null, null);
    }

    private void ReturnProduct_Add(object? sender, RoutedEventArgs e)
    {
        var productId = (ReturnProduct_StakPanel_List.SelectedItem as dynamic);
        var storeP = StoreMenu.SelectedItem as stores;
        try
        {
            AddFunctions.ReturnProduct(productId.ProductId.ProductId, productId.Quantity, storeP.StoreId, idUser);
            ReturnProduct_InfoBox.Text = "Товар возвращён.";
            ReturnProduct_StakPanel_List.SelectedItem = null;
        }
        catch (Exception exception)
        {
            ReturnProduct_InfoBox.Text = $"{exception.Message}";
        }
    }
}