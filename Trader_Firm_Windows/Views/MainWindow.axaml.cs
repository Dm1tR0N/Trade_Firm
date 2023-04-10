using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Npgsql;
using Trader_Firm_Windows.DataBase.Tables;
using static Trader_Firm_Windows.DataBase.Connection;

namespace Trader_Firm_Windows.Views;

public partial class MainWindow : Window
{
    public static bool isAuth = false; // false - Сотрдник не авторизирован, true - сотрудник авторизирован
    public static int number_attempts = 0; // Хранит количество не удачных входов в аккаунт
    public MainWindow()
    {
        InitializeComponent();
    }

    // Функция авторизации сотрудника
    private void LoginButton_Click(object? sender, RoutedEventArgs e) 
    {
        // try
        // {
        //     var connString = Connect();
        //     var conn = new NpgsqlConnection(connString);
        //     conn.Open();
        //     var cmd = new NpgsqlCommand("SELECT * FROM users WHERE username = @username AND password = @password", conn);
        //     cmd.Parameters.AddWithValue("username", LoginTextBox.Text);
        //     cmd.Parameters.AddWithValue("password", PasswordBox.Text);
        //     var reader = cmd.ExecuteReader();
        //     if (reader.HasRows)
        //     {
        //         StatusBar.Text = "Авторизирован";
        //         number_attempts = 0; // Обнуление попыток входа.
        //         isAuth = true;
        //     }
        //     else
        //     {
        //         if ( number_attempts >= 3)
        //         {
        //             StatusBar.Text = $"Вы не авторезированный уже {number_attempts} раза,\n" +
        //                              $"Обратитесь к Администратору для востонавления доступа к аккаунту!";
        //         }
        //         else
        //         {
        //             StatusBar.Text = $"Не авторизирован!";
        //         }
        //         
        //         number_attempts++;
        //         PasswordBox.Text = "";
        //     }
        //     reader.Close();
        //     conn.Close();
        // }
        // catch (Exception exception)
        // {
        //     StatusBar.Text = $"В чём то ошибка!\nУбедитесь что все поля заполненны.";
        // }
        
        
    }

    // Функциядля того что бы скрыть окно авторизации
    private void HideBorder_Click(object? sender, RoutedEventArgs e)
    {
        if (isAuth == true)
        {
            Auth.IsVisible = false; // Скрыть окно авторизации
            Menu.IsVisible = true; // Показать меню
        }
        else
        {
            StatusBar.Text = "Для выхода нужно Авторизироватся!";
        }
    }
}