using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class TaskTwo : MonoBehaviour
{
    private void Start()
    {
        Product iPhone12 = new Product("IPhone 12");
        Product iPhone11 = new Product("IPhone 11");

        Warehouse warehouse = new Warehouse();

        Shop shop = new Shop(warehouse);

        warehouse.Delive(iPhone12, 10);
        warehouse.Delive(iPhone11, 1);
        warehouse.ShowAllProducts();

        Cart cart = shop.Cart();

        cart.AddProduct(iPhone12, 4);
        cart.AddProduct(iPhone11, 1); 
        // cart.AddProduct(iPhone11, 3);
        
        cart.ShowAllProducts();
        cart.DoOrder();
        
        Debug.Log(cart.Order.PayLink);
        
        cart.AddProduct(iPhone12, 5);
        // cart.AddProduct(iPhone12, 9);
        warehouse.ShowAllProducts();
    }

    public class Warehouse
    {
        private List<Cell> _cells = new List<Cell>();

        public void ShowAllProducts()
        {
            foreach (var cell in _cells)
                DataOutput.ShowCellData(cell);
        }

        public void Delive(Product product, int amount)
        {
            if (product == null)
                throw new ArgumentNullException();

            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount) + " " + amount);

            Cell newCell = new Cell(product, amount);
            int indexOfCell = _cells.FindIndex(cell => cell.Product == product);

            if (indexOfCell == -1)
                _cells.Add(newCell);
            else
                _cells[indexOfCell] = _cells[indexOfCell].Merge(newCell);
        }

        public bool IsEnoughProducts(Product product, int amount)
        {
            if (product == null)
                throw new ArgumentNullException();

            int indexOfCell = _cells.FindIndex(cell => cell.Product == product);

            if (indexOfCell == -1)
                throw new ArgumentNullException();

            return amount <= _cells[indexOfCell].Amount;
        }

        public void Remove(Product product, int amount)
        {
            if (product == null)
                throw new ArgumentNullException();

            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount) + " " + amount);

            int indexOfCell = _cells.FindIndex(cell => cell.Product == product);

            if (indexOfCell == -1)
                throw new ArgumentException();

            if (_cells[indexOfCell].Amount < amount)
                throw new ArgumentOutOfRangeException(nameof(amount) + " " + amount);

            _cells[indexOfCell] = _cells[indexOfCell].DecreaseAmount(amount);
        }
    }

    public struct Cell
    {
        public Product Product { get; }
        public int Amount { get; private set; }

        public Cell(Product product, int amount)
        {
            if (product == null)
                throw new ArgumentNullException();

            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            Product = product;
            Amount = amount;
        }

        public Cell Merge(Cell newCell)
        {
            if (newCell.Product != Product)
                throw new ArgumentException();

            return new Cell(Product, Amount + newCell.Amount);
        }

        public Cell DecreaseAmount(int amount)
        {
            return new Cell(Product, Amount - amount);
        }
    }
    
    public class Product
    {
        public string Name { get; }

        public Product(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException();

            Name = name;
        }
    }

    public class Shop
    {
        public Warehouse Warehouse { get; }

        public Shop(Warehouse warehouse)
        {
            Warehouse = warehouse ?? throw new ArgumentNullException();
        }

        public Cart Cart()
        {
            Cart cart = new Cart(this);
            return cart;
        }
    }

    public class Cart
    {
        private readonly List<Cell> _cells = new List<Cell>();
        private Shop _shop;

        public IReadOnlyList<Cell> Cells => _cells;
        public Order Order { get; private set; }

        public Cart(Shop shop)
        {
            _shop = shop;
        }

        public void AddProduct(Product product, int amount)
        {
            if (product == null)
                throw new ArgumentNullException();

            if (amount <= 0)
                throw new ArgumentException();

            if (_shop.Warehouse.IsEnoughProducts(product, amount) == false)
                throw new ArgumentOutOfRangeException(nameof(amount) + " " + amount);
            
            Cell newCell = new Cell(product, amount);
            int indexOfCell = _cells.FindIndex(cell => cell.Product == product);

            if (indexOfCell == -1)
                _cells.Add(newCell);
            else
                _cells[indexOfCell] = _cells[indexOfCell].Merge(newCell);
        }

        public void ShowAllProducts()
        {
            foreach (var cell in _cells)
                DataOutput.ShowCellData(cell);
        }

        public void DoOrder()
        {
            Order = new Order(Randomizer.GetRandomString(20), this);
            foreach (var cell in _cells)
                _shop.Warehouse.Remove(cell.Product, cell.Amount);
            Clear();
        }

        private void Clear()
        {
            _cells.Clear();
        }
    }

    public class Order
    {
        public string PayLink { get; }
        private IReadOnlyList<Cell> _cells;

        public Order(string payLink, Cart cart)
        {
            PayLink = payLink; 
            _cells = cart.Cells;
        }
    }

    public static class DataOutput
    {
        public static void ShowCellData(Cell cell)
        {
            Debug.Log("Product: " + cell.Product.Name + " - " + cell.Amount + " piece(/s)");
        }
    }

    public static class Randomizer
    {
        public static string GetRandomString(int length)
        {
            var rnd = new Random();
            var randomString = string.Empty;

            while (length-- > 0)
            {
                randomString += (char) rnd.Next(65, 91);
            }

            return randomString;
        }       
    }
}