using System;
using System.Collections.Generic;
using Player;
using UI;
using UnityEngine;
using Worldgen;

namespace Utility
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private WorldGeneration worldGeneration;
        [SerializeField] private WindEngine windEngine;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] public int playerCount = 7;

        [SerializeField] private ScoreUiController scoreUiController;

        public List<Player.Player> players;

        public int ActivePlayers;
        public Player.Player CurrentPlayer { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }

        private void Start()
        {
            try
            {
                Debug.Log("GameManager - Start START");
                playerController = PlayerController.Instance;
                scoreUiController = ScoreUiController.Instance;
                players = new List<Player.Player>();
                for (var i = 0; i < playerCount; i++)
                {
                    // Create player
                    var playerObject = Instantiate(playerPrefab);
                    var player = playerObject.GetComponent<Player.Player>();
                    player.name = "Player " + (i + 1);

                    // Add player to list
                    players.Add(player);
                }

                Debug.Log("Tank Objects created");

                worldGeneration.InitialiseMap(players);

                Debug.Log("Number of players: " + players.Count);
                ActivePlayers = players.Count;
                CurrentPlayer = players[^1];
                SwitchPlayer();
                Debug.Log("GameManager - Start END");
            }
            catch (Exception e)
            {
                Debug.LogError("GameManager - Start - Exception: " + e.Message);
                throw;
            }
        }

        public void SwitchPlayer()
        {
            Debug.Log("GameManager - SwitchPlayer START");
            if (ActivePlayers <= 1)
            {
                RoundOver();
                Debug.Log("GameManager - SwitchPlayer END");
                return;
            }

            var newPlayerIndex = (players.IndexOf(CurrentPlayer) + 1) % players.Count;
            CurrentPlayer = players[newPlayerIndex];
            if (CurrentPlayer.tank == null)
            {
                ActivePlayers--;
                SwitchPlayer();
            }
            else
            {
                windEngine.UpdateWind();
                Debug.Log("Current player: " + CurrentPlayer.name);
                playerController.SwitchPlayer(CurrentPlayer);
                scoreUiController.UpdateScore();
            }

            Debug.Log("GameManager - SwitchPlayer END");
        }

        public void RoundOver()
        {
            // Handle round over logic
        }
    }
}