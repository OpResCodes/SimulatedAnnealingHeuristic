using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SimnOpt.Heuristics.SimAn
{
    /// <summary>
    /// Represents the output of a run
    /// </summary>
    [DebuggerDisplay("Iter: {Iterations}, Fitness: {FinalObjective}, Initial: {InitialObjective}")]
    public class SimAnOutput
    {

        internal SimAnOutput(ISimAnSolution finalSol, 
            List<double> temperatureFitness,
            List<double> iterFitness, 
            double initSolVal)
        {
            BestSolution = finalSol;
            TemperatureFitnessLevels = temperatureFitness;
            IterationFitnessLevels = iterFitness;
            Iterations = iterFitness.Count;
            InitialObjective = initSolVal;
        }

        /// <summary>
        /// The best found Solution
        /// </summary>
        public ISimAnSolution BestSolution { get; }

        /// <summary>
        /// The sequence of fitness levels where each value represents the best found value of a temperature
        /// </summary>
        public List<double> TemperatureFitnessLevels { get; }

        /// <summary>
        /// The sequence of fitness levels where each value represents the best found value of the iteration
        /// </summary>
        public List<double> IterationFitnessLevels { get; set; }
        
        /// <summary>
        /// The number of iterations
        /// </summary>
        public int Iterations { get; }

        /// <summary>
        /// The objective value of the starting solution
        /// </summary>
        public double InitialObjective { get; }

        /// <summary>
        /// The objective value of the best found solution
        /// </summary>
        public double FinalObjective => BestSolution.Fitness;

        /// <summary>
        /// FinalObjective / InitialObjective
        /// </summary>
        public double Improvement => 1.0 - (FinalObjective / InitialObjective);

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Date: {DateTime.Now}");
            sb.AppendLine($"Initial Objective: {InitialObjective:N4}");
            sb.AppendLine($"Final Objective: {FinalObjective:N4}");
            sb.AppendLine($"Improvement: {Improvement:N4}");
            sb.AppendLine($"Iterations: {Iterations:N0}");
            return sb.ToString();
        }

        /// <summary>
        /// Iter;Fitness
        /// </summary>
        public string IterFitCsv
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Iter;Fitness");
                int c = 0;
                foreach (double i in IterationFitnessLevels)
                {
                    c++;
                    sb.AppendLine($"{c};{i}");
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Temp-Level;Fitness
        /// </summary>
        public string TempFitCsv
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Temp-Level;Fitness");
                int c = 0;
                foreach (double i in TemperatureFitnessLevels)
                {
                    c++;
                    sb.AppendLine($"{c};{i}");
                }
                return sb.ToString();
            }
        }

    }
}
