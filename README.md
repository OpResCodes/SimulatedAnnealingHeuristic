# Simulated Annealing Heuristic

Flexible C# implementation of a Simulated Annealing Heuristic

## What is Simulated annealing?
It is an iterative local search optimization algorithm. Based on a given starting solution to an optimization problem, simulated annealing tries to find improvements to an objective criterion (for example: costs, revenue, transport effort) by slightly manipulating the given solution in each iteration. A small change to a solution leads to a "neighbor" solution with a different objective value. If the neighbor improves upon the objective value of its "parent", it is accepted as the next best solution. In order to explore the search space, simulated annealing with some probability also accepts solutions that don't improve the objective value. This probability of acceptance decreases in later iterations, i.e. the search converges towards a true local search that only accepts improvements. 

## Example (Quadratic assignment problem)

We consider 5 locations. The distances between the locations are recorded in a distance-matrix

| d(i,j) | A | B | C | D | E |
|---|---|---|---|---|---|
| A | 0 | 4 | 3 | 4 | 8 |
| B | 4 | 0 | 5 | 8 | 4 |
| C | 3 | 5 | 0 | 3 | 5 | 
| D | 4 | 8 | 3 | 0 | 4 |
| E | 8 | 4 | 5 | 4 | 0 |

We want to assign 5 units to the locations (each unit to one location, no location is used twice). The 5 units exchange the following transport volumes:
|t(u,v) | U1 | U2 | U3 | U4 | U5 |
|-------|----|----|----|----|----|
|  U1   | 0  | 3  | 6  | 9  | 8  |
|  U2   | 5  | 0  | 2  | 1  | 3  |
|  U3   | 2  | 7  | 0  | 4  | 0  |
|  U4   | 2  | 4  | 0  | 0  | 3  |
|  U5   | 4  | 9  | 6  | 0  | 0  |

We want to minimize transportation effort, i.e. transport volume * distance. We try to assign the units to good locations, but our first guess will be to assign unit U1 to location A, unit U2 to location B, U3 -> C, U4 -> D, U5 -> E. 
The total transportation effort for this solution is 383.

### Implementation

The problem is implemented in the Console project of the source files.
We need to create a class that represents a solution to the optimization problem. 
Our solution class must implement the 
ISimAnSolution interface that is part of the simulated annealing library:
```C#
public interface ISimAnSolution
{
    double Fitness { get; set; }
}
```
We implement this interface here:
```C#
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
  
```
We also need to implement a function that generates a "neighbor". The function must take and return a ISimAnSolution object.
```C#
public QuadAssignment GenerateNeighbor(ISimAnSolution baseSolution)
{
  QuadAssignment neighbor = new QuadAssignment();
  // take two units randomly and exchange their location assignment
  // (...)
  neighbor.CalculateFitness();
  return neighbor;
}
```

Now we can setup the optimization process using two classes: **SimAnHeurParams** and **SimAnHeuristic**.
SimAnHeurParams is used to control the optimization run. You must provide the function to generate (and evaluate!) a neighbor solution to it.
We provide the function just shown above to it. 

You can additionally define the maximum number of iterations, the cooldown schedule (see below) and the start temperature to define the optimization run.

```C#
SimAnHeurParams saParams = new SimAnHeurParams(defaultStepSize: 2, defaultCoolDown: 0.8, startTemp: 20);
saParams.GenerateNeighborSolution = this.GenerateNeighbor;
saParams.MaxIter = (int)Math.Pow(10,6);
//optimize
ISimAnSolution startSolution = GenerateStartSolution(); //see source files-> simple start solution
SimAnHeuristic heuristic = new SimAnHeuristic(saParams,startSolution);
SimAnOutput output = heuristic.Minimize();
Console.WriteLine("**************");
Console.WriteLine(output.ToString());
Console.WriteLine("**************");
foreach (var item in (output.BestSolution as QuadAssignment).AssignmentSolution)
{
    Console.WriteLine($"  Unit {item.Key} assigned to location {item.Value}.");
}
Console.WriteLine("**************");
```
The optimization is triggered by using the Minimize() function.
It will return a SimAnOutput object, that contains the best found solution and some basic statistics of the run.
The SimAnOutput.ToString() method will provide a brief summary of the optimization.

#### Cooldown-Schedule
With the SimAnHeurParams object, you can define the number of iterations in which the temperature is kept constant. In general, the lower the temperature value in an iteration the lower the probability of accepting solutions that don't improve upon their parent. In the course of the optimization the temperature is lowered multiple times. You could lower it in each iteration or after any other positive number of iterations. The number of iterations of constant temperature is usually called a plateau.
Alternatively, you can dynamically set the plateau length based on the currently prevailing temperature. You would need to provide a function to the delegate:
```C#
    /// <summary>
    /// Number of iterations in which the temperature is kept constant
    /// May be dependent on the current temperature level
    /// </summary>
    /// <returns>The plateau length</returns>
    public delegate int CoolDownStepSize(double currentTemp);
```
For instance, the default constructor of the SimAnHeurParams provides a constant step size to the delegate:
```C#
public class SimAnHeurParams
{
(...)

  public SimAnHeurParams(int defaultStepSize, double defaultCoolDown, double startTemp)
  {
      //set simple default functions
      StartTemp = startTemp;
      CoolDownStepSize = new CoolDownStepSize((temp) => defaultStepSize);
      CoolDown = new CoolDownFunction((temp) => defaultCoolDown * temp);
      MaxIter = 10000;
      MinAcceptCountPerTempLevel = 0;
      MaxCoolDownsWithoutImprovement = int.MaxValue;
  }
  public CoolDownStepSize CoolDownStepSize { get; set; }
  
  public CoolDownFunction CoolDown { get; set; }

  public GenerateSolution GenerateNeighborSolution { get; set; }
  
(...)
}
```
A similar concept is used for the actual cooldown function. You could use any function that reduces the current temperature by providing a function to the delegate:
```C#
  /// <summary>
  /// A function that describes the reduction in temperature based on the current temperature
  /// </summary>
  /// <param name="currentTemp">The current temperature</param>
  /// <returns>The reduced temperature</returns>
  public delegate double CoolDownFunction(double currentTemp);
```
The basic implementation in the constructor of SimAnHeurParams uses a geometric cooldown T(k+1) = a*T(k) where a is a percentage, usually between 0.8 and 0.99.

