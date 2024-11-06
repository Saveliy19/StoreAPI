﻿using DAL.Infrastructure;

namespace DAL.Entities
{
    internal class Store: IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public List<Product> Products { get; set; }
    }
}