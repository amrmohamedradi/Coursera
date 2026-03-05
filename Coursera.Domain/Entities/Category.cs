using Coursera.Domain.Common;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Coursera.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string ImagePath { get; private set; } = default!;

        public ICollection<Course> Courses { get; private set; } = new List<Course>();
        private Category() { }
        public Category( string name, string imagePath)
        {
            Name = name;
            ImagePath = imagePath;
        }

        public void Update(string name, string imagePath)
        {
            Name = name;
            ImagePath = imagePath;
        }
    }
}
