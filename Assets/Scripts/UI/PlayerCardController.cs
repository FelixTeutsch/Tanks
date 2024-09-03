using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    internal class PlayerCardController : MonoBehaviour
    {
        public string playerName;
        public EColour playerColour;

        private Button _deleteButton;

        private void Start()
        {
            _deleteButton = GetComponentInChildren<Button>();
            _deleteButton.onClick.AddListener(OnDeleteButtonClick);
        }

        private void OnDeleteButtonClick()
        {
            var createInputController = CreateInputController.Instance;
            createInputController.RemovePlayer(playerName);
        }
    }
}