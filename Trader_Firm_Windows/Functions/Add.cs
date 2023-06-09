﻿using Trader_Firm_Windows.DataBase.Tables;
using System;
using System.Collections.Generic;
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

        var _users = _context.Users.ToList();
        
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
    
    // Добовление продукта в общий склад
    public static void AddProduct(string Title, decimal Price, string Discription, int storeId, int userId, int Quantity)
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

    // Обновление информации о магазине
    public static void UpdateStore(int storeId, int storeMeneger, string newStoreName, string newStoreLocation)
    {
        Context _context = new Context();
        var newStoreMeneger = _context.Users.SingleOrDefault(x => x.Id == storeMeneger);
        var updateStore = _context.Stores.SingleOrDefault(x => x.StoreId == storeId);

        updateStore.StoreName = newStoreName;
        updateStore.StoreLocation = newStoreLocation;
        updateStore.ManagerId = newStoreMeneger;

        _context.SaveChanges();
    }

    // Метод для добавления продукта в магазин
    public static void AddProductStore(int selectedStore, int selectedProduct, int count, int userId)
    {
        using (var context = new Context())
        {
            var storeIdCheck = context.Stores.SingleOrDefault(x => x.StoreId == selectedStore);
            var productIdCheck = context.Products.SingleOrDefault(x => x.ProductId == selectedProduct);
            
            // Проверяем наличие записи в таблице store_products для данного магазина и продукта
            var existingProduct = context.StoreProducts.FirstOrDefault(sp => sp.StoreId == storeIdCheck && sp.ProductId == productIdCheck);

            if (existingProduct != null)
            {
                // Если запись существует, то увеличиваем значение поля Quantity
                existingProduct.Quantity += count;
            }
            else
            {
                // Иначе создаем новую запись
                var storeId = context.Stores.SingleOrDefault(x => x.StoreId == selectedStore);
                var productId = context.Products.SingleOrDefault(x => x.ProductId == selectedProduct);

                var newProduct = new store_products
                {
                    StoreId = storeId,
                    ProductId = productId,
                    Quantity = count
                };
                context.StoreProducts.Add(newProduct);
                
                var deliveriesList = context.Deliveries.ToList();
                var newDeliveries = new deliveries
                {
                    StoreId = storeId,
                    UserId = context.Users.SingleOrDefault(x => x.Id == userId),
                    DeliveryDate = DateTime.Now.ToUniversalTime()
                };
                deliveriesList.Add(newDeliveries);
                context.Add(newDeliveries);
                context.SaveChanges();
                
                var deliveriesProductsList = context.DeliveredProducts.ToList();
                var newdeliveriesProducts = new delivered_products
                {
                    DeliveryId = newDeliveries,
                    ProductId = productId,
                    Quantity = count
                };
                deliveriesProductsList.Add(newdeliveriesProducts);
                context.Add(newdeliveriesProducts);
                context.SaveChanges();
            }

            context.SaveChanges();
        }
    }
    
    // Метод для продажи продукта
    public static void SellProduct(int productId, int quantity, int storeId, int userId)
    {
        using (var context = new Context())
        {
            // Проверяем, что магазин с заданным идентификатором существует
            var store = context.Stores.SingleOrDefault(s => s.StoreId == storeId);
            if (store == null)
            {
                throw new ArgumentException("Магазин с таким идентификатором не найден.");
            }

            // Проверяем, что продукт с заданным идентификатором существует
            var product = context.Products.SingleOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                throw new ArgumentException("Продукт с таким идентификатором не найден.");
            }

            // Проверяем, что в магазине есть достаточное количество товара
            var storeProduct = context.StoreProducts
                .SingleOrDefault(sp => sp.StoreId == store && sp.ProductId == product);
            if (storeProduct == null || storeProduct.Quantity < quantity)
            {
                throw new ArgumentException("В магазине нет достаточного количества товара.");
            }

            // Создаем объект продажи и сохраняем его в базу данных
            var sale = new sales
            {
                StoreId = store,
                UserId = context.Users.Find(userId),
                SaleDate = DateTime.Now.ToUniversalTime()
            };
            context.Sales.Add(sale);
            context.SaveChanges();

            // Создаем объекты проданных товаров и сохраняем их в базу данных
            var soldProduct = new sold_products
            {
                SaleId = sale,
                ProductId = product,
                Quantity = quantity,
                Price = product.Price
            };
            context.SoldProducts.Add(soldProduct);
            context.SaveChanges();

            // Уменьшаем количество товара в магазине
            storeProduct.Quantity -= quantity;
            context.SaveChanges();
        }
    }
    
    // Метод для возврата продукта на склад
    
    /*
     * Метод должен создавать запись в таблицах returned_products, returns.
     * Так-же он должен возвращать продукт в таблицу store_products
     * Продукт можно вернуть в течении 14 дней
     */
    public static void ReturnProduct(int productId, int quantity, int storeId, int userId)
    {
        using (var context = new Context())
        {
            var store = context.Stores.SingleOrDefault(s => s.StoreId == storeId);
            if (store == null)
            {
                throw new ArgumentException("Магазин с таким идентификатором не найден.");
            }
    
            var product = context.Products.SingleOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                throw new ArgumentException("Продукт с таким идентификатором не найден.");
            }
    
            var sale = context.Sales.Where(s => s.StoreId.StoreId == storeId && s.UserId.Id == userId);
            if (sale == null)
            {
                throw new ArgumentException("Этот пользователь не покупал товар в этом магазине.");
            }
            
            
            var soldProduct = context.SoldProducts
                .SingleOrDefault(sp => sp.ProductId.ProductId == product.ProductId &&
                                       sp.Quantity == quantity);
            if (soldProduct == null)
            {
                throw new ArgumentException("Этот пользователь не купил этот товар.");
            }
    
            if (quantity > soldProduct.Quantity)
            {
                throw new ArgumentException("Количество возвращаемого товара больше, чем было куплено.");
            }
    
            var returnObj = new returns
            {
                StoreId = store,
                UserId = context.Users.Find(userId),
                ReturnDate = DateTime.Now.ToUniversalTime()
            };
            context.Returns.Add(returnObj);
    
            var returnedProduct = new returned_products
            {
                ReturnId = returnObj,
                ProductId = product,
                Quantity = quantity,
                Price = product.Price
            };
            context.ReturnsProducts.Add(returnedProduct);
    
            var storeProduct = context.StoreProducts.SingleOrDefault(sp => sp.StoreId.StoreId == store.StoreId && sp.ProductId.ProductId == product.ProductId);
            if (storeProduct == null)
            {
                storeProduct = new store_products 
                { 
                    StoreId = store, 
                    ProductId = product, 
                    Quantity = 0 
                };
                context.StoreProducts.Add(storeProduct);
            }
    
            storeProduct.Quantity += quantity;
    
            soldProduct.Quantity -= quantity;
    
            context.SaveChanges();
        }
    }
}