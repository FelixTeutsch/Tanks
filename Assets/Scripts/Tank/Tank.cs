using System;
using Projectile;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utility;

namespace Tank
{
    public class Tank : MonoBehaviour
    {
        //@formatter:off
        [Header("Controls")]
        [SerializeField] private float tankSpeed = 1f;
        [SerializeField] private float tankMaxSpeed = 5f;
        [SerializeField] private float power = 10f;

        [Header("Cannon Settings")]
        [SerializeField] private float cannonRotationSpeed = 2f;
        [SerializeField] private float cannonMaxSpeed = 50f;

        [Header("References")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform cannon;
        [SerializeField] private Transform tankBody;
        [SerializeField] private Slider tankHealthBar;
        [SerializeField] private Slider powerSlider;
        
        [Header("Health")]
        [SerializeField] private float maxHp = 100;
        [Range(0, 100)]
        [SerializeField] private float hp = 100;
        
        private float _cannonInput;
        private float _currentSpeedCannon;
        private float _currentSpeedTank;
        //@formatter:on

        private GameManager _gameManager;
        private float _movementInput;


        // Start is called before the first frame update
        private void Start()
        {
            _gameManager = GameManager.Instance;
            tankHealthBar.maxValue = maxHp;
            tankHealthBar.value = hp;
        }

        // Update is called once per frame
        private void Update()
        {
            if (_gameManager.currentPlayer.name != gameObject.name) return;
            // Apply continuous cannon rotation
            if (_cannonInput != 0)
            {
                var newAngle = cannon.localEulerAngles.z + _cannonInput * _currentSpeedCannon * Time.deltaTime;
                newAngle = Mathf.Clamp(newAngle, 0, 180);
                cannon.localEulerAngles = new Vector3(cannon.localEulerAngles.x, cannon.localEulerAngles.y, newAngle);
                _currentSpeedCannon = Math.Min(cannonMaxSpeed, _currentSpeedCannon * 1.01f);
            }

            // Apply continuous movement
            if (_movementInput != 0)
            {
                var move = new Vector3(_movementInput, 0, 0);
                transform.position += move * (_currentSpeedTank * Time.deltaTime);
                _currentSpeedTank = Math.Min(tankMaxSpeed, _currentSpeedTank * 1.01f);
            }
        }

        public void OnMovement(InputValue value)
        {
            _movementInput = value.Get<float>();
            _currentSpeedTank = tankSpeed;
        }

        public void OnCannonAngle(InputValue value)
        {
            _cannonInput = value.Get<float>();
            _currentSpeedCannon = cannonRotationSpeed;
        }

        public void OnFire()
        {
            if (_gameManager.currentPlayer.name != gameObject.name) return;
            Debug.Log("Firing. Game manager: " + _gameManager.currentPlayer.name + " This player: " + gameObject.name);
            var bulletPosition = cannon.position + cannon.right * 0.1f;
            var bullet = Instantiate(bulletPrefab, bulletPosition, Quaternion.identity, transform);
            var direction = cannon.right; // Adjusted to use the local right axis
            bullet.GetComponent<Rigidbody2D>().AddForce(direction * power, ForceMode2D.Impulse);
            bullet.GetComponent<IProjectile>().SetOwner(gameObject);
            try
            {
                // _gameManager.SwitchPlayer();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void TakeDamage(float damage)
        {
            Debug.Log("Took damage: " + damage);
            hp -= damage;
            tankHealthBar.value = hp;
            if (hp <= 0) Destroy(gameObject);
        }

        public void Heal(float amount)
        {
            hp = Mathf.Clamp(hp + amount, 0, maxHp);
            tankHealthBar.value = hp;
        }

        public void UpdatePower(float newPower)
        {
            power = Mathf.Clamp(newPower, 0, 50f);
        }
    }
}