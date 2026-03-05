using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Common.Models
{
    public class CreateInstructorRequest
    {
        public string Name { get; set; } = default!;
        public string JobTitle { get; set; } = default!;
        public string Bio { get; set; } = default!;
        public string ImagePath{ get; set; } = default!;
    }
}
