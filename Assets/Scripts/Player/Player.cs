using CameraLogic;
using Fusion;
using Spawn;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(NetworkCharacterControllerPrototype))]
    public class Player : NetworkBehaviour
    {
        private const float BallSpawnDelay = 0.5f;
        [SerializeField] private float _movementSpeed = 5f;
        [SerializeField] private Ball _prefabBall;
        [SerializeField] private PhysxBall _prefabPhysxBall;
        [SerializeField] private float _physxBallVelocity = 10f;

        [Networked] 
        private TickTimer Delay { get; set; }
        [Networked(OnChanged = nameof(OnBallSpawned))]
        private NetworkBool BallSpawnTrigger { get; set; }
    
        private PlayerHud _playerHud;
        private CameraController _cameraController;
        private NetworkCharacterControllerPrototype _characterController;
        private Vector3 _forward;
        private Material _material;

        private Material Material
        {
            get
            {
                if (_material == null)
                    _material = GetComponentInChildren<MeshRenderer>().material;
                return _material;
            }
        }

        private void Awake()
        {
            _characterController = GetComponent<NetworkCharacterControllerPrototype>();
            Material.color = Color.blue;
        }
    
        private void Update()
        {
            if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.R))
            {
                RPC_ShowPopup();
            }
        }

        public void SetCamera()
        {
            if (_cameraController == null)
                _cameraController = FindObjectOfType<CameraController>();
            _cameraController.SetTarget(this);
        }

        public override void Render()
        {
            Material.color = Color.Lerp(Material.color, Color.blue, Time.deltaTime);
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                data.Direction.Normalize();
                if (data.Direction.sqrMagnitude > 0)
                {
                    _forward = data.Direction;
                    _characterController.Move(_forward * _movementSpeed * Runner.DeltaTime);
                    
                }
                
                if (Delay.ExpiredOrNotRunning(Runner))
                {
                    if ((data.Buttons & NetworkInputData.MOUSEBUTTON1) != 0) 
                        SpawnBall();
                    else if ((data.Buttons & NetworkInputData.MOUSEBUTTON2) != 0) 
                        SpawnPhysxBall();
                }
            }
        }

        public static void OnBallSpawned(Changed<Player> changed)
        {
            changed.Behaviour.Material.color = Color.white;
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
            BallSpawnTrigger = !BallSpawnTrigger;
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
            BallSpawnTrigger = !BallSpawnTrigger;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void RPC_ShowPopup(RpcInfo info = default)
        {
            if (_playerHud == null)
                _playerHud = FindObjectOfType<PlayerHud>();
            _playerHud.ShowMessage((info.IsInvokeLocal ? "You said: " : "Player " + info.Source.PlayerId + " said: ") + "Hello!");
        }
    }
}