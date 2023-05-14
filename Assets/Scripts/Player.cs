using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkCharacterControllerPrototype))]
public class Player : NetworkBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    private NetworkCharacterControllerPrototype _characterController;
    
    private void Awake()
    {
        _characterController = GetComponent<NetworkCharacterControllerPrototype>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.Direction.Normalize();
            _characterController.Move(data.Direction * _movementSpeed * Runner.DeltaTime);
        }
    }
}