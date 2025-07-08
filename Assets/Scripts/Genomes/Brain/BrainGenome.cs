public abstract class BrainGenome
{
    public abstract (BrainGenome, BrainGenome) Reproduce(BrainGenome parent2);

    public abstract BrainGenome Clone();

    public abstract void Mutate();

    public abstract float CalculateHammingDistance(BrainGenome other_genome);
}