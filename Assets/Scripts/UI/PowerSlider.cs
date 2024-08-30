using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerSlider : MonoBehaviour
{
    [SerializeField] private Slider powerSlider;
    [SerializeField] private TMP_Text powerText;
    private PlayerController _playerController;
    private float _powerOldValue;

    private bool _powerSet;

    // Start is called before the first frame update
    private void Start()
    {
        _playerController = PlayerController.Instance;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!Mathf.Approximately(powerSlider.value, _powerOldValue))
        {
            powerText.text = "Power: " + powerSlider.value.ToString("00");
            _powerOldValue = powerSlider.value;
            // Update power if it was not set by controller
            if (!_powerSet)
            {
                Debug.Log("Power will be updated for player");
                var temp = _playerController.SetPower(_powerOldValue);
                if (!Mathf.Approximately(temp, -1))
                    SetPower(temp);
            }

            _powerSet = false;
        }
    }

    public void SetPower(float power)
    {
        _powerSet = true;
        powerSlider.value = Mathf.Clamp(powerSlider.value, 0, 100);
    }

    public float GetPower()
    {
        return powerSlider.value;
    }
}