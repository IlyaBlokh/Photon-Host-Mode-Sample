using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Spawn
{
    public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkPrefabRef _playerPrefab;
        private readonly Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();
        private NetworkRunner _runner;
        private bool _mouseButton0;
        private bool _mouseButton1;

        private async void StartGame(GameMode gameMode)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;

            await _runner.StartGame(new StartGameArgs
            {
                GameMode = gameMode,
                SessionName = "Test Room",
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }

        private void OnGUI()
        {
            if (_runner != null) 
                return;
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
                StartGame(GameMode.Host);
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
                StartGame(GameMode.Client);
        }
    
        private void Update()
        {
            _mouseButton0 = _mouseButton0 || Input.GetMouseButton(0);
            _mouseButton1 = _mouseButton1 || Input.GetMouseButton(1);
        }
    
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                Vector3 spawnPosition = new Vector3(player.RawEncoded % runner.Config.Simulation.DefaultPlayers * 3, 1, 0);
                NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player,
                    (_, playerObject) =>
                    {
                        playerObject.GetComponent<Player.Player>().SetCamera();
                    });
                _spawnedCharacters.Add(player, networkPlayerObject);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                _spawnedCharacters.Remove(player);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var data = new NetworkInputData();

            if (Input.GetKey(KeyCode.W))
                data.Direction += Vector3.forward;

            if (Input.GetKey(KeyCode.S))
                data.Direction += Vector3.back;

            if (Input.GetKey(KeyCode.A))
                data.Direction += Vector3.left;

            if (Input.GetKey(KeyCode.D))
                data.Direction += Vector3.right;
        
            if (_mouseButton0)
                data.Buttons |= NetworkInputData.MOUSEBUTTON1;
            _mouseButton0 = false;
        
            if (_mouseButton1)
                data.Buttons |= NetworkInputData.MOUSEBUTTON2;
            _mouseButton1 = false;
        
            input.Set(data);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        
        }
        public void OnConnectedToServer(NetworkRunner runner)
        {
        
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
        
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
        
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        
        }
    }
}
