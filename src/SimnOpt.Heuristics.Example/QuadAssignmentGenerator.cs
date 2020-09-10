using SimnOpt.Heuristics.SimAn;
using System;

namespace SimnOpt.Heuristics.Example
{
    public class QuadAssignmentGenerator
    {
        private readonly Random _rnd;

        public QuadAssignmentGenerator(int randomSeed = 1)
        {
            _rnd = new Random(randomSeed);
        }

        public QuadAssignment GenerateNeighbor(ISimAnSolution baseSolution)
        {
            if (!(baseSolution is QuadAssignment qaSol))
                return null;
            // generate a copy of the existing assignment
            QuadAssignment neighbor = new QuadAssignment();
            foreach (var item in qaSol.AssignmentSolution)
            {
                neighbor.AssignmentSolution.Add(item.Key, item.Value);
            }
            // exchange one pair of unit-location assignments
            int unit1 = _rnd.Next(0, QuadAssignment.MaterialFlow.GetLength(0));
            int unit2 = _rnd.Next(0, QuadAssignment.MaterialFlow.GetLength(0));
            int assignment1 = qaSol.AssignmentSolution[unit1];
            int assignment2 = qaSol.AssignmentSolution[unit2];
            neighbor.AssignmentSolution[unit1] = assignment2;
            neighbor.AssignmentSolution[unit2] = assignment1;
            // calculate objective value and return neighbor
            neighbor.CalculateFitness();
            return neighbor;
        }

        public QuadAssignment GenerateStartSolution()
        {
            //assign each unit to one location (number of units assumed <= number of locations)
            QuadAssignment startSolution = new QuadAssignment();

            for (int u = 0; u < QuadAssignment.MaterialFlow.GetLength(0); u++)
            {
                startSolution.AssignmentSolution[u] = u;
            }
            startSolution.CalculateFitness();
            return startSolution;
        }

        public void OptimizeAssignment(double[,] materialFlow, double[,] distances)
        {
            QuadAssignment.MaterialFlow = materialFlow;
            QuadAssignment.Distances = distances;
            if (materialFlow.GetLength(0) > distances.GetLength(0))
                throw new ArgumentException($"Not enough locations to assign all units!");

            //setup the heuristic optimization
            SimAnHeurParams saParams = new SimAnHeurParams(defaultStepSize: 2, defaultCoolDown: 0.8, startTemp: 20);
            saParams.GenerateNeighborSolution = this.GenerateNeighbor;
            saParams.MaxIter = (int)Math.Pow(10, 6);
            //optimize
            ISimAnSolution startSolution = GenerateStartSolution();
            SimAnHeuristic heuristic = new SimAnHeuristic(saParams, startSolution);
            SimAnOutput output = heuristic.Minimize();

            Console.WriteLine("**************");
            Console.WriteLine(output.ToString());
            Console.WriteLine("**************");
            foreach (var item in (output.BestSolution as QuadAssignment).AssignmentSolution)
            {
                Console.WriteLine($"  Unit {item.Key} assigned to location {item.Value}.");
            }
            Console.WriteLine("**************");
        }

    }
}
