using System;
using System.Collections.Generic;

namespace encapsulation_02
{
    internal class Program
    { 
        public static void Main(string[] args)
        {
            Store _store = new Store();
            
            _store.AddGoodsToWarehouse(new Good("IPhone 12"), 10);
            _store.AddGoodsToWarehouse(new Good("IPhone 11"), 1);
            _store.ShowWarehouse();
            _store.AddToCart(new Good("IPhone 12"), 4);
            
            _store.AddToCart(new Good("IPhone 11"), 3);
            _store.ShowCart();
            _store.MakeOrder();
            _store.ShowWarehouse();
            
            _store.AddGoodsToWarehouse(new Good("IPhone 12"), 10);
            _store.ShowWarehouse();
            _store.AddToCart(new Good("IPhone 12"), 9);
            _store.ShowCart();
            
            _store.MakeOrder();
            _store.ShowWarehouse();
        }
    }

    public struct Good
    {
        public string Name { get; private set; }

        public Good(string name)
        {
            Name = name;
        }
    }

    public class Warehouse
    {
        private readonly Dictionary<Good, uint> _goods = new Dictionary<Good, uint>();
        public IReadOnlyDictionary<Good, uint> Goods => _goods;

        public void Deliver(Good good, uint number)
        {
            if (_goods.ContainsKey(good))
            {
               _goods[good] += number;
            }
            else
            {
                _goods.Add(good, number);
            }
        }

        public void Subtract(Good key, uint value)
        {
            _goods[key] -= value;
        }

        public void ShowGoods()
        {
            Console.WriteLine("In warehouse:");
            
            foreach (var good in _goods)
            {
                Console.WriteLine($"Good name: {good.Key.Name}, good volume: {good.Value}");
            }

            if (_goods.Count == 0)
                Console.WriteLine("You don't have any goods in warehouse");
        }
    }

    public class Shop
    {
        private Warehouse _warehouse;
        
        public Shop(Warehouse warehouse)
        {
            _warehouse = warehouse;
        }

        public Cart GetCart()
        {
            return new Cart(_warehouse.Goods);
        }

        public void Subtract(Dictionary<Good, uint> cartGoods)
        {
            foreach (var good in cartGoods)
                _warehouse.Subtract(good.Key, good.Value);
        }
    }

    public class Cart
    {
        private Dictionary<Good, uint> _goodsInCart = new Dictionary<Good, uint>();
        private Dictionary<Good, uint> _currentGoodsInWarehouse;

        public IReadOnlyDictionary<Good, uint> GoodInCart => _goodsInCart;

        public Cart(IReadOnlyDictionary<Good, uint> goods)
        {
            _currentGoodsInWarehouse = (Dictionary<Good, uint>)goods;
        }

        public void ShowGoods()
        {
            Console.WriteLine("Your order in cart:");
            
            foreach (var good in _goodsInCart)
                Console.WriteLine($"Good name: {good.Key.Name}, good volume: {good.Value}");

            if (_goodsInCart.Count == 0)
                Console.WriteLine("You don't have any goods in cart");
        }

        public void Add(Good good, uint number)
        {
            if (!_currentGoodsInWarehouse.ContainsKey(good))
            {
                Console.WriteLine($"Incorrect good. We don't have {good.Name}. " +
                                  $"Please, connect with us and let us know about this issues");
                throw new ArgumentNullException();
            }
            
            if (_currentGoodsInWarehouse[good] < number)
            {
                Console.WriteLine($"You're trying to order {number} units {good.Name}");
                Console.WriteLine($"Now we have {good.Name} only {_currentGoodsInWarehouse[good]} units");
                Console.WriteLine("Please, try again later");
                throw new ArgumentOutOfRangeException();
            }

            _goodsInCart.Add(good, number);
        }

        public void Order()
        {
            Console.WriteLine("Paylink");
        }
    }

    public class Store
    {
        private Warehouse _warehouse;
        private Shop _shop;
        private Cart _cart;

        public Store()
        {
            _warehouse = new Warehouse();
            _shop = new Shop(_warehouse);
            SetCart();
        }

        public void AddGoodsToWarehouse(Good good, uint number)
        {
            _warehouse.Deliver(good, number);
            SetCart();
        }

        public void ShowWarehouse()
        {
            _warehouse.ShowGoods();
        }

        public void AddToCart(Good good, uint number)
        {
            _cart.Add(good, number);
        }

        public void ShowCart()
        {
            _cart.ShowGoods();
        }

        public void MakeOrder()
        {
            var goods = _cart.GoodInCart;
            
            if (goods.Count == 0)
                return;
            
            _shop.Subtract((Dictionary<Good, uint>)goods);
            _cart.Order();
            SetCart();
        }

        private void SetCart()
        {
            _cart = _shop.GetCart();
        }
    }
}