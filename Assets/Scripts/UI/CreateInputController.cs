using System;
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
            // TODO: populate maxPlayers with the value from GameManager
        }

        private void Update()
        {
            addPlayerButton.interactable = playerNameInput.text != "";
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
                var buttonColor = buttonImage.color;
                buttonColor.a = isFirstButton ? 1.0f : 0.5f; // Set opacity to 100% for the first button, 50% for others
                buttonImage.color = buttonColor;

                var border = colorButton.transform.Find("Border").GetComponent<Image>();
                ColorUtility.TryParseHtmlString(isFirstButton ? "#DAE7E0" : "#000000", out var borderColor);
                borderColor.a =
                    isFirstButton ? 1.0f : 0.25f; // Set opacity to 100% for the first button, 25% for others
                border.color = borderColor;

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
        }

        public void SelectColor(Button button, EColour color)
        {
            if (_activeButton != null)
            {
                var previousBorder = _activeButton.transform.Find("Border").GetComponent<Image>();
                ColorUtility.TryParseHtmlString("#000000", out var darkColor);
                darkColor.a = 0.25f; // Set opacity to 25%
                previousBorder.color = darkColor;

                var previousImage = _activeButton.GetComponent<Image>();
                var previousColor = previousImage.color;
                previousColor.a = 0.5f; // Set opacity to 50%
                previousImage.color = previousColor;
            }

            _activeButton = button;
            _selectedColor = color;

            var currentBorder = _activeButton.transform.Find("Border").GetComponent<Image>();
            ColorUtility.TryParseHtmlString("#DAE7E0", out var selectedColor);
            currentBorder.color = selectedColor;

            var currentImage = _activeButton.GetComponent<Image>();
            var currentColor = currentImage.color;
            currentColor.a = 1.0f; // Set opacity to 100%
            currentImage.color = currentColor;
        }

        public void CreatePlayer()
        {
            Debug.Log("CreatePlayer START");
            Debug.Log("_playerCount: " + _playerCount);
            if (_playerCount >= _maxPlayers) return;

            AddPlayerCard(playerNameInput.text, _selectedColor);

            // Reset the input field
            playerNameInput.text = "";

            // Reset the color selection to the first button
            SelectColor(colorArea.transform.GetChild(0).GetComponent<Button>(),
                (EColour)Enum.GetValues(typeof(EColour)).GetValue(0));
        }

        private void AddPlayerCard(string playerName, EColour playerColor)
        {
            // Instantiate a new player card from the prefab
            var newPlayerCard = Instantiate(playerCard, playerCardArea.transform);

            newPlayerCard.name = playerName;

            // Set the player name in the TMP_Text component
            var playerNameText = newPlayerCard.GetComponentInChildren<TMP_Text>();
            playerNameText.text = playerName;

            // Set the player color in the Image component
            var playerColorImage = newPlayerCard.GetComponentsInChildren<Image>()
                .FirstOrDefault(image => image.name == "Color");
            if (playerColorImage != null)
                playerColorImage.color = playerColor.GetColour();

            // Position the new player card
            var rectTransform = newPlayerCard.GetComponent<RectTransform>();
            var cardHeight = rectTransform.sizeDelta.y;
            var gap = 8;
            var newYPosition = -_playerCount++ * (cardHeight + gap) - 16;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, newYPosition);
            playerCountText.text = $"{_playerCount}/{_maxPlayers}";
            if (_playerCount == _maxPlayers) addPlayerButton.enabled = false;
        }
    }
}