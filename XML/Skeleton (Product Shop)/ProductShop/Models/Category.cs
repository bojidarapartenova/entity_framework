﻿namespace ProductShop.Models
{
    using System.Collections.Generic;

    public class Category
    {
        public Category()
        {
            this.CategoryProducts = new List<CategoryProduct>();
        }

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<CategoryProduct> CategoryProducts { get; set; }=new HashSet<CategoryProduct>();
    }
}
