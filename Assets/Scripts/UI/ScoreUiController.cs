using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utility;

namespace UI
{
    public class ScoreUiController : MonoBehaviour
    {
        private const int ContainerHeight = 16;
        private const int IncreaseDirection = -1;
        private const int IncreaseSize = 16;
        private const int InitialPosition = -16;
        private const int Padding = 4;
        private const int ScoreUiWidth = 150;
        [SerializeField] private GameObject scoreUiPrefab;
        [SerializeField] private GameObject scoreUiParent;
        [SerializeField] public bool updateScore;
        private int _activeTanks;
        private bool _initialised;

        private List<PlayerScore> _playerScores;
        public static ScoreUiController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (updateScore)
            {
                UpdateScore();
                updateScore = false;
            }
        }

        public void Init()
        {
            Debug.Log("ScoreUiController - Init START");
            var numberOfPlayers = GameManager.Instance.playerCount;
            var containerHeight = ContainerHeight + IncreaseSize * numberOfPlayers + Padding * (numberOfPlayers - 1);
            GetComponent<RectTransform>().sizeDelta = new Vector2(ScoreUiWidth, containerHeight);

            _playerScores = new List<PlayerScore>();

            for (var i = 0; i < GameManager.Instance.players.Count; i++)
            {
                var player = GameManager.Instance.players[i];
                var scoreUi = Instantiate(scoreUiPrefab, scoreUiParent.transform);
                var rectTransform = scoreUi.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0,
                    InitialPosition + (i * IncreaseSize + Padding * i) * IncreaseDirection);

                var nameText = scoreUi.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                var scoreText = scoreUi.transform.Find("Score").GetComponent<TextMeshProUGUI>();
                nameText.SetText(player.name);
                scoreText.SetText(player.score.ToString());
                scoreUi.name = player.name;


                _playerScores.Add(new PlayerScore(scoreUi, player));
            }

            _initialised = true;
            Debug.Log("ScoreUiController - Init END");
        }

        public void UpdateScore()
        {
            Debug.Log("ScoreUiController - UpdateScore START");

            if (!_initialised) Init();

            foreach (var playerScore in _playerScores)
            {
                var scoreText = playerScore.ScoreUi.transform.Find("Score").GetComponent<TextMeshProUGUI>();
                scoreText.SetText(playerScore.Player.score.ToString());
            }

            _playerScores.Sort((a, b) => b.Player.score.CompareTo(a.Player.score));

            for (var i = 0; i < _playerScores.Count; i++)
            {
                var rectTransform = _playerScores[i].ScoreUi.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0,
                    InitialPosition + (i * IncreaseSize + Padding * i) * IncreaseDirection);
            }

            Debug.Log("ScoreUiController - UpdateScore END");
        }

        public void TankActive()
        {
            if (++_activeTanks == GameManager.Instance.players.Count)
                SetPlayerColors();
        }

        private void SetPlayerColors()
        {
            foreach (var playerScore in _playerScores)
            {
                var nameText = playerScore.ScoreUi.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                var scoreText = playerScore.ScoreUi.transform.Find("Score").GetComponent<TextMeshProUGUI>();

                var colorHex = GetColorHex(playerScore.Player.playerColor);
                nameText.color = colorHex;
                scoreText.color = colorHex;
            }
        }

        private Color GetColorHex(EColour color)
        {
            switch (color)
            {
                case EColour.Red:
                    return Color.red;
                case EColour.Blue:
                    return Color.blue;
                case EColour.Green:
                    return Color.green;
                case EColour.Orange:
                    return new Color(1f, 0.65f, 0f); // Orange color
                case EColour.Cyan:
                    return Color.cyan;
                case EColour.Purple:
                    return new Color(0.5f, 0f, 0.5f); // Purple color
                case EColour.Magenta:
                    return Color.magenta;
                default:
                    return Color.white;
            }
        }
    }

    internal class PlayerScore
    {
        public PlayerScore(GameObject scoreUi, Player.Player player)
        {
            ScoreUi = scoreUi;
            Player = player;
        }

        public Player.Player Player { get; }
        public GameObject ScoreUi { get; }
    }
}