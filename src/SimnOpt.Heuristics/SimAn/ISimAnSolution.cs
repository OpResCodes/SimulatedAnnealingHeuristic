namespace SimnOpt.Heuristics.SimAn
{

    /// <summary>
    /// A solution to the optimization problem
    /// </summary>
    public interface ISimAnSolution
    {
        /// <summary>
        /// The objective value of the solution
        /// </summary>
        double Fitness { get; set; }
    }

}
