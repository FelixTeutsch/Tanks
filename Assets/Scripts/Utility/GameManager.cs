using System.Collections.Generic;
using Player;
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
        [SerializeField] private int playerCount = 2;

        public List<Player.Player> players;
        public Player.Player CurrentPlayer { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }

        private void Start()
        {
            playerController = PlayerController.Instance;
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

            worldGeneration.InitialiseMap(players);

            Debug.Log("Number of players: " + players.Count);

            CurrentPlayer = players[0];
        }

        public void SwitchPlayer()
        {
            var newPlayerIndex = (players.IndexOf(CurrentPlayer) + 1) % players.Count;
            CurrentPlayer = players[newPlayerIndex];
            windEngine.UpdateWind();
        }

        public void RoundOver()
        {
            // Handle round over logic
        }
    }
}