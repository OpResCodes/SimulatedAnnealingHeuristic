using System;

namespace SimnOpt.Heuristics.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This is an example of a simulated annealing optimization run.");
            Console.WriteLine("We solve the quadratic assignment problem.");

            //setup a quadratic assignment problem:
            double[,] matFlow = new double[5, 5]
            {
                {0,3,6,9,8 },
                {5,0,2,1,3 },
                {2,7,0,4,0 },
                {2,4,0,0,3 },
                {4,9,6,0,0 }
            };
            
            double[,] distances = new double[5, 5] 
            {
                {0,4,3,4,8 },
                {4,0,5,8,4 },
                {3,5,0,3,5 },
                {4,8,3,0,4 },
                {8,4,5,4,0 }
            };

            QuadAssignmentGenerator g = new QuadAssignmentGenerator();
            g.OptimizeAssignment(matFlow, distances);

            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
        }
    }
}
