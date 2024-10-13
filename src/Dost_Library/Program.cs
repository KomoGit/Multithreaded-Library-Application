using Dost_Library.Application;
using Dost_Library.DAL;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

Stopwatch sw = Stopwatch.StartNew();
#region Simulation Initializing
int studentCount = 0;
int seatCount = 0;
#region Take Seat Count
do
{
    Console.WriteLine("Insert number of seats for simulation: (3 to 5)");
    _ = int.TryParse(Console.ReadLine(), out seatCount);
    if (seatCount < 3 || seatCount > 5)
        Console.WriteLine("Value cannot be more than 100 or less than 1.\n");
}
while (seatCount < 3 || seatCount > 5);
#endregion
#region Take Student Cound
do
{
    Console.WriteLine("Insert number of students for simulation: (1 to 100)");
    _ = int.TryParse(Console.ReadLine(), out studentCount);
    if (studentCount > 100 || studentCount < 1)
        Console.WriteLine("Value cannot be more than 100 or less than 1.\n");
}
while (studentCount is 0 || studentCount > 100);
#endregion
Console.WriteLine($"Number of students set to {studentCount}\n");
Console.WriteLine($"Number of seats set to {seatCount}\n");
#endregion

using (var context = new AppDbContext())
{
    var data = context.Students
        .Take(studentCount)
        .Include(x => x.Book)
        .ToList()
        .OrderBy(x => x.BookId) //Was in reverse previously.
        .ThenBy(x => x.TimeItTakesToRead);

    var sim = new Simulation(maxSeats: seatCount);
    sim.StartSimulation(data);
    sim.WaitForAll();
}
sw.Stop();
Console.WriteLine($"{sw} seconds.");