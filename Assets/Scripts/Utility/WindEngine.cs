using TMPro;
using UnityEngine;

namespace Utility
{
    public class WindEngine : MonoBehaviour
    {
        [SerializeField] private TMP_Text windText;
        [SerializeField] private int maxWindSpeed = 150;
        [SerializeField] private int minWindSpeed = -150;
        [SerializeField] private int windChangeRate = 30;

        private int _windSpeed;
        private int _windTrend = 1; // 1 for increasing, -1 for decreasing
        public static WindEngine instance { get; private set; }

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
            _windTrend = Random.Range(0, 2) == 0 ? 1 : -1;
            UpdateWind();
        }

        public void UpdateWind()
        {
            _windSpeed += _windTrend * Random.Range(0, windChangeRate);
            _windSpeed = Mathf.Clamp(_windSpeed, minWindSpeed, maxWindSpeed);

            if (_windSpeed >= maxWindSpeed || _windSpeed <= minWindSpeed || Random.Range(0, 100) <= 1) _windTrend *= -1;

            windText.SetText($"Wind: {_windSpeed:00}");
        }

        public int GetWindSpeed()
        {
            return _windSpeed;
        }
    }
}