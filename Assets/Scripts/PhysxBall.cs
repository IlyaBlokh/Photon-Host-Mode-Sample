using Fusion;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysxBall : NetworkBehaviour
{
    private const float LifeTime = 5.0f;
    [Networked] private TickTimer Life { get; set; }
    
    public void Init(Vector3 forward)
    {
        Life = TickTimer.CreateFromSeconds(Runner, LifeTime);
        GetComponent<Rigidbody>().velocity = forward;
    }

    public override void FixedUpdateNetwork()
    {
        if(Life.Expired(Runner))
            Runner.Despawn(Object);
    }
}