﻿namespace Contracts
{
    
    public struct ShoppingCartItem
    {
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public int Amount { get; set; }
        public double LineTotal => Amount * UnitPrice;
    }
}
