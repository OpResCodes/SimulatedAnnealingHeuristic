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
            double[,] matFlow = new double[5, 5];
            matFlow[0, 0] = 0; matFlow[0, 1] = 3; matFlow[0, 2] = 6; matFlow[0, 3] = 9; matFlow[0, 4] = 8;
            matFlow[1, 0] = 5; matFlow[1, 1] = 0; matFlow[1, 2] = 2; matFlow[1, 3] = 1; matFlow[1, 4] = 3;
            matFlow[2, 0] = 2; matFlow[2, 1] = 7; matFlow[2, 2] = 0; matFlow[2, 3] = 4; matFlow[2, 4] = 0;
            matFlow[3, 0] = 2; matFlow[3, 1] = 4; matFlow[3, 2] = 0; matFlow[3, 3] = 0; matFlow[3, 4] = 3;
            matFlow[4, 0] = 4; matFlow[4, 1] = 9; matFlow[4, 2] = 6; matFlow[4, 3] = 0; matFlow[4, 4] = 0;

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
