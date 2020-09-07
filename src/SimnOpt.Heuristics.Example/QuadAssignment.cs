using SimnOpt.Heuristics.SimAn;
using System;
using System.Collections.Generic;

namespace SimnOpt.Heuristics.Example
{
    public class QuadAssignment : ISimAnSolution
    {
        /// <summary>
        /// Material flow between two units
        /// </summary>
        public static double[,] MaterialFlow { get; set; }

        /// <summary>
        /// Distance between two locations
        /// </summary>
        public static double[,] Distances { get; set; }

        /// <summary>
        /// Assignment of unit to location
        /// </summary>
        public Dictionary<int,int> AssignmentSolution { get; }

        public QuadAssignment()
        {
            AssignmentSolution = new Dictionary<int, int>();
        }
        
        /// <summary>
        /// Calculates the total transport performance
        /// (expected material flow multiplied with the distances)
        /// </summary>
        public void CalculateFitness()
        {
            Fitness = 0.0;//to be determined
            int locU, locV; //locations
            //iterate all pairs of units
            for (int u = 0; u < MaterialFlow.GetLength(0); u++)
            {
                locU = AssignmentSolution[u];
                for (int v = 0; v < MaterialFlow.GetLength(1); v++)
                {
                    locV = AssignmentSolution[v];
                    Fitness += MaterialFlow[u, v] * Distances[locU, locV];
                }
            }            
        }

        public double Fitness { get; set; }

    }
}
