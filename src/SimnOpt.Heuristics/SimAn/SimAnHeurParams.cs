using System;

namespace SimnOpt.Heuristics.SimAn
{

    /// <summary>
    /// Parameters to control the optimization process
    /// </summary>
    public class SimAnHeurParams
    {

        /// <summary>
        /// Create a new set of Parameters. Note: Cooldown and step size can be specified as functions, too.
        /// See <seealso cref="CoolDownStepSize"/> and <seealso cref="CoolDown"/>
        /// </summary>
        /// <param name="defaultStepSize">Default number of iterations in which the temperature is kept constant</param>
        /// <param name="defaultCoolDown">Default percentage of temperature decrease when cooling is applied 
        /// (example values: 0.8 - 0.99)</param>
        /// <param name="startTemp">The initial temperature</param>
        public SimAnHeurParams(int defaultStepSize, double defaultCoolDown, double startTemp)
        {
            if (defaultStepSize <= 0)
                throw new ArgumentException("The value must be bigger than 0.", nameof(defaultStepSize));
            if (defaultCoolDown > 0.999 || defaultCoolDown < 0.0001)
                throw new ArgumentException("The value is outside the range ]0.0001, 0.999[", nameof(defaultCoolDown));
            if (startTemp < 0)
                throw new ArgumentException("The value cannot be negative" ,nameof(startTemp));

            //set simple default functions
            StartTemp = startTemp;
            CoolDownStepSize = new CoolDownStepSize((temp) => defaultStepSize);
            CoolDown = new CoolDownFunction((temp) => defaultCoolDown * temp);
            MaxIter = 10000;
            MinAcceptCountPerTempLevel = 0;
            MaxCoolDownsWithoutImprovement = int.MaxValue;
        }

        /// <summary>
        /// Number of iterations in which the temperature is kept constant
        /// </summary>
        public CoolDownStepSize CoolDownStepSize { get; set; }

        /// <summary>
        /// The cooldown function
        /// </summary>
        public CoolDownFunction CoolDown { get; set; }

        public GenerateSolution GenerateNeighborSolution { get; set; }

        /// <summary>
        /// The initial temperature
        /// </summary>
        public double StartTemp { get; set; }

        /// <summary>
        /// Absolute tolerance for fitness value comparison (default: 1e-8)
        /// </summary>
        public double AbsTolerance { get; set; } = 1e-8;

        private int _maxIter;

        /// <summary>
        /// Maximum iterations (stop criterion)
        /// </summary>
        public int MaxIter
        {
            get { return _maxIter; }
            set
            {
                if (value <= 0 || value == int.MaxValue)
                    throw new ArgumentException("Positive number of iterations required.");
                _maxIter = value;
            }
        }

        private int _minAccept;
        /// <summary>
        /// Minimum number of accepted solutions on each level (stop criterion)
        /// </summary>
        public int MinAcceptCountPerTempLevel
        {
            get { return _minAccept; }
            set
            {
                if (value < 0 || value == int.MaxValue)
                    throw new ArgumentException("Positive feasible number expected.");
                _minAccept = value;
            }
        }

        private int _maxEmptyCoolDowns;

        /// <summary>
        /// Maximum number of cooldowns with equal fitness values (stop criterion)
        /// </summary>
        public int MaxCoolDownsWithoutImprovement
        {
            get { return _maxEmptyCoolDowns; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Feasible value expected.");
                _maxEmptyCoolDowns = value;
            }
        }
    }
}