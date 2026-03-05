using Coursera.Domain.Common;
using Coursera.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Domain.Entities
{
    public class Instructor : BaseEntity
    {
        public string Name { get; protected set; }
        public string Bio { get; protected set; }
        public JobTitle JobTitle { get; protected set; }
        public string ImagePath { get; protected set; } = default!;

        public ICollection<Course> Courses { get; private set; } = new List<Course>();

        private Instructor() { }

        public Instructor(string name, string bio, JobTitle jobTitle, string imagePath)
        {
            Name = name;
            Bio = bio;
            JobTitle = jobTitle;
            ImagePath = imagePath;
        }

        public void Update(string name, string bio, JobTitle jobTitle, string imagePath)
        {
            Name = name;
            Bio = bio;
            JobTitle = jobTitle;
            ImagePath = imagePath;
        }

    }
}
