using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class CreateInputController : MonoBehaviour
    {
        private const int Padding = 8;
        [SerializeField] private TMP_InputField playerNameInput;
        [SerializeField] private Button colorButtonPrefab;
        [SerializeField] private GameObject colorArea;
        [SerializeField] private int colorsPerRow = 5;
        [SerializeField] private GameObject playerCard;
        [SerializeField] private GameObject playerCardArea;
        [SerializeField] private Button addPlayerButton;
        [SerializeField] private TMP_Text playerCountText;
        private readonly int _maxPlayers = 6;

        private readonly List<PlayerInfo> _players = new();
        private Button _activeButton;
        private int _playerCount;
        private EColour _selectedColor;
        public static CreateInputController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            PopulateColorArea();
        }

        private void Update()
        {
            addPlayerButton.interactable = !string.IsNullOrEmpty(playerNameInput.text);
        }

        private void PopulateColorArea()
        {
            var colors = Enum.GetValues(typeof(EColour));
            var colorAreaWidth = colorArea.GetComponent<RectTransform>().rect.width;
            var buttonWidth = colorButtonPrefab.GetComponent<RectTransform>().sizeDelta.x;
            var totalButtonWidth = colorsPerRow * buttonWidth;
            var totalGapWidth = colorAreaWidth - totalButtonWidth - 2 * Padding;
            var gap = totalGapWidth / (colorsPerRow - 1);

            var row = 0;
            var col = 0;
            var isFirstButton = true;

            foreach (EColour color in colors)
            {
                var colorButton = Instantiate(colorButtonPrefab, colorArea.transform);
                var rectTransform = colorButton.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(
                    Padding + col * (rectTransform.sizeDelta.x + gap),
                    -(Padding + row * (rectTransform.sizeDelta.y + gap))
                );

                var buttonImage = colorButton.GetComponent<Image>();
                buttonImage.color = color.GetColour();
                buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b,
                    isFirstButton ? 1.0f : 0.5f);

                var border = colorButton.transform.Find("Border").GetComponent<Image>();
                border.color = new Color(0, 0, 0, isFirstButton ? 1.0f : 0.25f);

                colorButton.GetComponent<ColorButtonClickHandler>().colour = color;

                if (isFirstButton)
                {
                    _activeButton = colorButton;
                    _selectedColor = color;
                    isFirstButton = false;
                }

                col++;
                if (col >= colorsPerRow)
                {
                    col = 0;
                    row++;
                }
            }

            // Set the first color button as the default selected color
            SelectColor(_activeButton, _selectedColor);
        }

        public void SelectColor(Button button, EColour color)
        {
            if (_activeButton != null)
            {
                var previousBorder = _activeButton.transform.Find("Border").GetComponent<Image>();
                previousBorder.color = new Color(0, 0, 0, 0.25f);

                var previousImage = _activeButton.GetComponent<Image>();
                previousImage.color =
                    new Color(previousImage.color.r, previousImage.color.g, previousImage.color.b, 0.5f);
            }

            _activeButton = button;
            _selectedColor = color;

            var currentBorder = _activeButton.transform.Find("Border").GetComponent<Image>();
            currentBorder.color = new Color(0.854f, 0.906f, 0.878f, 1.0f);

            var currentImage = _activeButton.GetComponent<Image>();
            currentImage.color = new Color(currentImage.color.r, currentImage.color.g, currentImage.color.b, 1.0f);
        }

        public void CreatePlayer()
        {
            if (_playerCount >= _maxPlayers) return;

            AddPlayerCard(playerNameInput.text, _selectedColor);

            playerNameInput.text = "";
            SelectColor(colorArea.transform.GetChild(0).GetComponent<Button>(),
                (EColour)Enum.GetValues(typeof(EColour)).GetValue(0));
        }

        private void AddPlayerCard(string playerName, EColour playerColor)
        {
            var newPlayerCard = Instantiate(playerCard, playerCardArea.transform);
            newPlayerCard.name = playerName;

            var playerNameText = newPlayerCard.GetComponentInChildren<TMP_Text>();
            playerNameText.text = playerName;

            var playerColorImage = newPlayerCard.GetComponentsInChildren<Image>()
                .FirstOrDefault(image => image.name == "Color");
            if (playerColorImage != null)
                playerColorImage.color = playerColor.GetColour();

            var rectTransform = newPlayerCard.GetComponent<RectTransform>();
            var cardHeight = rectTransform.sizeDelta.y;
            var newYPosition = -_playerCount++ * (cardHeight + Padding) - Padding;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, newYPosition);

            playerCountText.text = $"{_playerCount}/{_maxPlayers}";
            if (_playerCount == _maxPlayers) addPlayerButton.enabled = false;
            var playerCardController = newPlayerCard.GetComponent<PlayerCardController>();
            playerCardController.playerName = playerName;
            playerCardController.playerColour = playerColor;

            _players.Add(new PlayerInfo(playerName, playerColor, newPlayerCard));
        }

        public void RemovePlayer(string playerName)
        {
            Debug.Log("Removing Player " + playerName);
            var playerInfo = _players.FirstOrDefault(p => p.Name == playerName);
            if (playerInfo != null)
            {
                Destroy(playerInfo.Card);
                _players.Remove(playerInfo);
                _playerCount--;

                UpdatePlayerCardPositions();

                playerCountText.text = $"{_playerCount}/{_maxPlayers}";
                if (_playerCount < _maxPlayers) addPlayerButton.enabled = true;
            }
        }

        private void UpdatePlayerCardPositions()
        {
            var cardHeight = playerCard.GetComponent<RectTransform>().sizeDelta.y;
            for (var i = 0; i < _players.Count; i++)
            {
                var rectTransform = _players[i].Card.GetComponent<RectTransform>();
                var newYPosition = -i * (cardHeight + Padding) - Padding;
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, newYPosition);
            }
        }

        public void StartGame()
        {
            // TODO: Implement start game logic (e.g. load game scene, pass player info to GameManager, etc.)
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene(EScene.MainMenu);
        }
    }

    public class PlayerInfo
    {
        public PlayerInfo(string name, EColour color, GameObject card)
        {
            Name = name;
            Color = color;
            Card = card;
        }

        public string Name { get; set; }
        public EColour Color { get; set; }
        public GameObject Card { get; set; }
    }
}