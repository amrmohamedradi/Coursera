using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Common.Models
{
    public class UpdateCategoryRequest
    {
        public string Name { get; set; } = default!;
        public string ImagePath { get; set; } = default!;
    }
}
