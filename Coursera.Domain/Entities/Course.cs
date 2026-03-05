using Coursera.Domain.Common;
using Coursera.Domain.Enums;
using System.ComponentModel;

namespace Coursera.Domain.Entities
{
    public class Course :BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string Description { get; private set; } = default!;

        public decimal  Price { get; private set; }
        public decimal Rating { get; private set; }
        public string ImagePath{ get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Level Level { get; private set; }
        public Category Category { get; private set; } = default!;
        public Guid CategoryId{ get; private set; }
        public Instructor Instructor { get; private set; } = default!;
        public Guid InstructorId{ get; private set; }


        private Course() { }

        public Course(string name,
            string description,
            decimal price,
            Level level,
            string imagePath,
            Guid categoryId,
            Guid instructorId)
        {
            Name = name;
            Description = description;
            Price = price;
            Level = level;
            ImagePath = imagePath;
            CategoryId = categoryId;
            InstructorId = instructorId;
            CreatedAt = DateTime.UtcNow;
            Rating = 0;
        }
        public void Update(string name,
            string description,
            decimal price,
            Level level,
            string imagePath,
            Guid categoryId,
            Guid instructorId)
        {
            Name = name;
            Description = description;
            Price = price;
            Level = level;
            ImagePath = imagePath;
            CategoryId = categoryId;
            InstructorId = instructorId;
        }
        public void UpdateRating(decimal rating)
        {
            if (rating < 0 || rating > 5)
                throw new ArgumentException("Rating must be between 0 and 5.");
            Rating = rating;
        }
    }
}