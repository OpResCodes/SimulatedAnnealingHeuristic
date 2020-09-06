namespace SimnOpt.Heuristics.SimAn
{

    /// <summary>
    /// Number of iterations in which the temperature is kept constant
    /// May be dependent on the current temperature level
    /// </summary>
    /// <returns>The plateau length</returns>
    public delegate int CoolDownStepSize(double currentTemp);

    /// <summary>
    /// A function that describes the reduction in temperature based on the current temperature
    /// </summary>
    /// <param name="currentTemp">The current temperature</param>
    /// <returns>The reduced temperature</returns>
    public delegate double CoolDownFunction(double currentTemp);

    /// <summary>
    /// This function should lift the heavy work of generating and evaluating a neighbor solution
    /// </summary>
    /// <param name="baseSolution">The solution that the newly generated solution is based upon</param>
    /// <returns>A new feasible neighbor solution</returns>
    public delegate ISimAnSolution GenerateSolution(ISimAnSolution baseSolution);

}
