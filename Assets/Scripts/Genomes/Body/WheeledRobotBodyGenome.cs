public class WheeledRobotBodyGenome : BodyGenome
{
    public static WheeledRobotBodyGenome CreateWheeledRobotTestGenome()
    {
        return new();
    }

    public override BodyGenome Clone()
    {
        WheeledRobotBodyGenome clone = this;
        return clone;
    }

    public override (BodyGenome bodygenome1, BodyGenome bodygenome2) Reproduce(BodyGenome body_genome)
    {
        WheeledRobotBodyGenome clone = this;
        WheeledRobotBodyGenome clone2 = this;
        return (clone, clone2);
    }
}
