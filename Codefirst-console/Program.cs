using Codefirst_console;
using System;
using System.Linq;

namespace Codefirst_console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new SchoolContext())
            {
                // Create DB and table if not exists
                context.Database.EnsureCreated();

                // Insert data
                var student = new Student { Name = "Dhoni", Age = 21 };
                context.Students.Add(student);
                context.SaveChanges();
                Console.WriteLine("Student added!");

                // Retrieve data
                var students = context.Students.ToList();
                Console.WriteLine("Students in database:");
                foreach (var s in students)
                {
                    Console.WriteLine($"{s.StudentId} - {s.Name} - {s.Age}");
                }
            }
        }
    }
}
