using TMPro;
using UnityEngine;

namespace Utility
{
    public class WindEngine : MonoBehaviour
    {
        [SerializeField] private TMP_Text windText;
        [SerializeField] private int maxWindSpeed = 150;
        [SerializeField] private int minWindSpeed = -150;
        [SerializeField] private int windChangeRate = 5;

        private int _windSpeed;
        private int _windTrend = 1; // 1 for increasing, -1 for decreasing

        // Start is called before the first frame update
        private void Start()
        {
            UpdateWind();
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void UpdateWind()
        {
            // Update wind speed based on the current trend
            _windSpeed += _windTrend * Random.Range(0, windChangeRate);
            _windSpeed = Mathf.Clamp(_windSpeed, minWindSpeed, maxWindSpeed);

            if (_windSpeed >= maxWindSpeed || _windSpeed <= minWindSpeed || Random.Range(0, 100) <= 1)
            {
                Debug.Log(
                    $"Wind change due to {(_windSpeed >= maxWindSpeed ? "reaching max speed" : _windSpeed <= minWindSpeed ? "reaching min speed" : "random factor")}: {_windSpeed}. Max: {maxWindSpeed}, Min: {minWindSpeed}");
                _windTrend *= -1;
            }

            windText.SetText($"Wind: {_windSpeed.ToString("00")}");
        }

        public int GetWindSpeed()
        {
            return _windSpeed;
        }
    }
}