public class SnookerBall
{
    //constants for the balls
    public static readonly float mass = 0.2f;
    public static readonly float frictionCoeff = 0.04f;
    public static readonly float radius = 0.1f;
    public static readonly float gravityConst = 9.81f;

    public static float NormalForce
    {
        get { return gravityConst * mass; }
    }
}
