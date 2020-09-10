namespace SimnOpt.Heuristics.Genetic
{
    public interface IGaSolution
    {
        double Fitness { get; set; }

        IGaSolution DeepClone();

    }
}
