using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class ColorButtonClickHandler : MonoBehaviour
    {
        public EColour colour;
        private CreateInputController _createInputController;

        private void Start()
        {
            _createInputController = CreateInputController.Instance;
        }

        private void Update()
        {
        }

        public void OnClick()
        {
            _createInputController.SelectColor(GetComponent<Button>(), colour);
        }
    }
}