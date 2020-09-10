using System;
using System.Collections.Generic;

namespace SimnOpt.Heuristics.SimAn
{

    /// <summary>
    /// Simulated Annealing Heuristic
    /// </summary>
    public class SimAnHeuristic
    {
        private readonly Random _rnd;
        private ISimAnSolution _bestSolution;
        private readonly ISimAnSolution _startSolution;
        private readonly SimAnHeurParams _params;

        public SimAnHeuristic(SimAnHeurParams p, ISimAnSolution startSol, int randomSeed = 1)
        {
            if (p == null)
                throw new ArgumentNullException(nameof(p));
            if (p.CoolDown == null)
                throw new ArgumentNullException("CoolDown function");
            if (p.CoolDownStepSize == null)
                throw new ArgumentNullException("CoolDown step size");
            if (p.GenerateNeighborSolution == null)
                throw new ArgumentNullException("GenerateNeighborSolution");

            _params = p;
            _startSolution = startSol;
            _rnd = new Random(randomSeed);
        }

        public SimAnOutput Minimize()
        {
            bool stop = false;
            _bestSolution = _startSolution;
            ISimAnSolution currentSolution = _startSolution;
            double deltaFitness, curTemp = _params.StartTemp;
            int iter = 0, iterCoolDown;
            iterCoolDown = iter + _params.CoolDownStepSize(curTemp);
            int accepted = 0;
            List<double> temperatureFitnessLevels = new List<double>();
            List<double> iterationFitnessLevels = new List<double>() { currentSolution.Fitness };
            while (!stop)
            {
                //generate neighbor solution
                ISimAnSolution neighbor = _params.GenerateNeighborSolution(currentSolution);
                deltaFitness = neighbor.Fitness - currentSolution.Fitness;
                //acceptance test
                if (AcceptNeighbor(deltaFitness, curTemp))
                {
                    currentSolution = neighbor;
                    accepted += 1;
                }
                //improvement test
                if (currentSolution.Fitness < _bestSolution.Fitness)
                {
                    _bestSolution = currentSolution;
                }
                iterationFitnessLevels.Add(currentSolution.Fitness);
                iter++;
                //cooldown by annealing schedule
                if (iter == iterCoolDown)
                {
                    //record statistics for this temp-level (plateau)
                    temperatureFitnessLevels.Add(_bestSolution.Fitness);
                    //update temperature and set next cooldown iteration
                    curTemp = _params.CoolDown(curTemp);
                    iterCoolDown += _params.CoolDownStepSize(curTemp);
                    //test abort criterion 
                    stop = StopSearchAfterPlateau(accepted, temperatureFitnessLevels);
                    //reset number of accepted solutions within plateau
                    accepted = 0;
                }
                stop = stop | iter > _params.MaxIter;
            }
            return new SimAnOutput(_bestSolution, temperatureFitnessLevels, iterationFitnessLevels, _startSolution.Fitness);
        }

        private bool StopSearchAfterPlateau(int acceptedSolutions, List<double> fitnessRecord)
        {
            //number of accepted solutions
            if (acceptedSolutions < _params.MinAcceptCountPerTempLevel)
                return true;

            //Test if there was improvement in the last iterations
            int n = _params.MaxCoolDownsWithoutImprovement;
            int m = fitnessRecord.Count;
            if (n <= m)
            {
                bool isDifferent = false;
                double v = fitnessRecord[m - n];
                for (int i = m - n; i < m; i++)
                {
                    if (Math.Abs(fitnessRecord[i] - v) > _params.AbsTolerance)
                    {
                        isDifferent = true;
                        break;
                    }
                    else
                    {
                        v = fitnessRecord[i];
                    }
                }
                if (!isDifferent)
                    return true;
            }
            return false;
        }

        private bool AcceptNeighbor(double deltaFit, double temperature)
        {
            if (deltaFit < 0)
            {
                return true;
            }
            else if (_rnd.NextDouble() < Math.Exp(-1 * deltaFit / temperature))
            {
                return true;
            }
            //reject solution
            return false;
        }
    }
}