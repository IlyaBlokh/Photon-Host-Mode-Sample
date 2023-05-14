using Fusion;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    private const float LifeTime = 5.0f;
    [SerializeField] private float _movementSpeed = 5f;
    [Networked] private TickTimer Life { get; set; }

    public void Init()
    {
        Life = TickTimer.CreateFromSeconds(Runner, LifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        if(Life.Expired(Runner))
            Runner.Despawn(Object);
        else
            transform.position += _movementSpeed * transform.forward * Runner.DeltaTime;
    }
}