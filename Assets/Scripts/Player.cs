using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkCharacterControllerPrototype))]
public class Player : NetworkBehaviour
{
    private const float BallSpawnDelay = 0.5f;
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private PhysxBall _prefabPhysxBall;
    [SerializeField] private float _physxBallVelocity = 10f;
    
    [Networked] private TickTimer Delay { get; set; }
    private NetworkCharacterControllerPrototype _characterController;
    private Vector3 _forward;

    private void Awake()
    {
        _characterController = GetComponent<NetworkCharacterControllerPrototype>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.Direction.Normalize();
            if (data.Direction.sqrMagnitude > 0)
                _forward = data.Direction;
            
            _characterController.Move(data.Direction * _movementSpeed * Runner.DeltaTime);

            if (Delay.ExpiredOrNotRunning(Runner))
            {
                if ((data.Buttons & NetworkInputData.MOUSEBUTTON1) != 0) 
                    SpawnBall();
                else if ((data.Buttons & NetworkInputData.MOUSEBUTTON2) != 0) 
                    SpawnPhysxBall();
            }
        }
    }

    private void SpawnBall()
    {
        Delay = TickTimer.CreateFromSeconds(Runner, BallSpawnDelay);
        Runner.Spawn(_prefabBall,
            transform.position + _forward, Quaternion.LookRotation(_forward),
            Object.InputAuthority,
            (_, ballObject) =>
            {
                ballObject.GetComponent<Ball>().Init();
            });
    }

    private void SpawnPhysxBall()
    {
        Delay = TickTimer.CreateFromSeconds(Runner, BallSpawnDelay);
        Runner.Spawn(_prefabPhysxBall,
            transform.position + _forward,
            Quaternion.LookRotation(_forward),
            Object.InputAuthority,
            (_, ballObject) =>
            {
                ballObject.GetComponent<PhysxBall>().Init(_physxBallVelocity * _forward);
            });
    }
}