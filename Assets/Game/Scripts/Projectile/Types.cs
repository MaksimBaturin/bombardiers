public class LightProjectile : Projectile
{
    protected override void Awake()
    {
        base.Awake();
        mass = 0.5f;
        airResistance = 0.2f; 
        gravityScale = 0.8f; 
    }
}
public class CommonProjectile : Projectile
{
    protected override void Awake()
    {
        base.Awake();
        mass = 1.5f;
        airResistance = 0.3f; 
        gravityScale = 1f; 
    }
}

public class HeavyProjectile : Projectile
{
    protected override void Awake()
    {
        base.Awake();
        mass = 3f;
        airResistance = 0.05f; 
        gravityScale = 1.2f; 
    }
}