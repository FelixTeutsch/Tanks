using System;
using System.Collections.Generic;
using Player;
using UI;
using UnityEngine;

namespace Utility
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] public int playerCount = 7;
        [SerializeField] private bool printPlayers;
        private List<Player.Player> players { get; set; }
        public static GameManager instance { get; private set; }

        public Player.Player currentPlayer { get; private set; }
        public int activePlayers { get; private set; }

        public List<Player.Player> getPlayers => players;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Debug.Log("GameManager - Start START");
            if (SceneManager.GetCurrentScene() == EScene.Game && (players == null || players.Count < 1))
                LoadRandomPlayers();
            Debug.Log("GameManager - Start END");
        }

        private void Update()
        {
            if (printPlayers)
            {
                printPlayers = false;
                LogData();
            }
        }

        public void LogData()
        {
            Debug.Log("GameManager - LogData START");
            Debug.Log("Player Count: " + playerCount);
            Debug.Log("Players: " + players.Count);
            Debug.Log("Player names: " + string.Join(", ", players.ConvertAll(p => p.name).ToArray()));
            Debug.Log("GameManager - LogData END");
        }

        public void StartGame()
        {
            Debug.Log("GameManager - InitializeGame START");
            try
            {
                activePlayers = players.Count;
                currentPlayer = players[^1];
                SwitchPlayer();
            }
            catch (Exception e)
            {
                Debug.LogError("GameManager - InitializeGame - Exception: " + e.Message);
                throw;
            }

            Debug.Log("GameManager - InitializeGame END");
        }

        private void LoadRandomPlayers()
        {
            // Here for debug purposes
            players = new List<Player.Player>();

            for (var i = 0; i < playerCount; i++)
            {
                var playerObject = Instantiate(playerPrefab);
                var player = playerObject.GetComponent<Player.Player>();
                player.name = "Player " + (i + 1);
                player.SetColour((EColour)(i % Enum.GetValues(typeof(EColour)).Length));
                players.Add(player);
            }
        }

        public void CreatePlayerObjects(List<string> playerNames, List<EColour> playerColours)
        {
            players = new List<Player.Player>();

            Debug.Log("Starting Game with " + playerNames.Count + " players");
            Debug.Log("Names: " + string.Join(", ", playerNames));
            Debug.Log("Colors: " + string.Join(", ", playerColours));

            for (var i = 0; i < playerNames.Count; i++)
            {
                var playerObject = Instantiate(playerPrefab);
                var player = playerObject.GetComponent<Player.Player>();
                Debug.Log("Player Name: " + playerNames[i]);
                Debug.Log("Player Colour: " + playerColours[i]);
                Debug.Log("Player: " + player);
                player.name = playerNames[i];
                player.SetColour(playerColours[i]);
                players.Add(player);
            }

            playerCount = players.Count;
        }

        public void SwitchPlayer()
        {
            Debug.Log("GameManager - SwitchPlayer START");
            if (activePlayers <= 1)
            {
                RoundOver();
                return;
            }

            var newPlayerIndex = (players.IndexOf(currentPlayer) + 1) % players.Count;
            currentPlayer = players[newPlayerIndex];

            if (currentPlayer.tank == null)
            {
                activePlayers--;
                SwitchPlayer();
            }
            else
            {
                WindEngine.instance.UpdateWind();
                PlayerController.Instance.SwitchPlayer(currentPlayer);
                ScoreUiController.instance.UpdateScore();
            }

            Debug.Log("GameManager - SwitchPlayer END");
        }

        private void RoundOver()
        {
            // Handle round over logic
        }
    }
}