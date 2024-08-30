using System;
using Projectile;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Player
{
    public class Tank : MonoBehaviour
    {
        [Header("Controls")] [SerializeField] private float tankSpeed = 1f;

        [SerializeField] private float tankMaxSpeed = 5f;
        [SerializeField] private float power = 10f;

        [Header("Cannon Settings")] [SerializeField]
        private float cannonRotationSpeed = 2f;

        [SerializeField] private float cannonMaxSpeed = 50f;

        [Header("References")] [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField] private Transform cannon;
        [SerializeField] private Transform tankBody;
        [SerializeField] private GameObject tank;

        [Header("Health")] [SerializeField] private float maxHp = 100;

        [Range(0, 100)] [SerializeField] private float hp = 100;

        [SerializeField] private Slider tankHealthBar;

        [SerializeField] private bool controlsLocked;
        private float _cannonInput;

        private float _currentSpeedCannon;
        private float _currentSpeedTank;

        private float _movementInput;

        private PlayerController _playerController;


        private void Awake()
        {
            _playerController = PlayerController.Instance;
        }

        private void Start()
        {
            tankHealthBar.maxValue = maxHp;
            tankHealthBar.value = hp;
        }

        // Update is called once per frame
        private void Update()
        {
            // Apply continuous cannon rotation
            if (_cannonInput != 0 && !controlsLocked)
            {
                var newAngle = cannon.localEulerAngles.z + _cannonInput * _currentSpeedCannon * Time.deltaTime;
                newAngle = Mathf.Clamp(newAngle, 0, 180);
                cannon.localEulerAngles = new Vector3(cannon.localEulerAngles.x, cannon.localEulerAngles.y, newAngle);
                _currentSpeedCannon = Math.Min(cannonMaxSpeed, _currentSpeedCannon * 1.01f);
            }
            else
            {
                _cannonInput = 0;
            }

            // Apply continuous movement
            if (_movementInput != 0 && !controlsLocked)
            {
                var move = new Vector3(_movementInput, 0, 0);
                transform.position += move * (_currentSpeedTank * Time.deltaTime);
                _currentSpeedTank = Math.Min(tankMaxSpeed, _currentSpeedTank * 1.01f);
            }
            else
            {
                _movementInput = 0;
            }
        }

        public void Move(float movementInput)
        {
            _movementInput = movementInput;
            _currentSpeedTank = tankSpeed;
        }

        public void RotateCannon(float cannonInput)
        {
            _cannonInput = cannonInput;
            _currentSpeedCannon = cannonRotationSpeed;
        }

        public void Fire()
        {
            LockControls();
            var bulletPosition = cannon.position + cannon.right * 0.1f;
            var bullet = Instantiate(bulletPrefab, bulletPosition, Quaternion.identity, transform);
            var direction = cannon.right;
            bullet.GetComponent<Rigidbody2D>().AddForce(direction * power, ForceMode2D.Impulse);
            bullet.GetComponent<IProjectile>().SetOwner(gameObject);
        }

        public void TakeDamage(float damage)
        {
            hp -= damage;
            tankHealthBar.value = hp;
            if (hp <= 0) Destroy(gameObject);
        }

        public void Heal(float amount)
        {
            hp = Mathf.Clamp(hp + amount, 0, maxHp);
            tankHealthBar.value = hp;
        }

        public float UpdatePower(float newPower)
        {
            Debug.Log("Tank - UpdatePower START");
            power = Mathf.Clamp(newPower, 0, 50f); // Limit power to max HP
            Debug.Log("Current Power: " + power);
            Debug.Log("Tank - UpdatePower END");
            return power;
        }

        public float GetPower()
        {
            return power;
        }

        public void SetColour(EColour colour)
        {
            var tankObject = _playerController.GetTankSprite(colour);
            if (tankObject != null)
            {
                var sprites = tankObject.GetComponentsInChildren<SpriteRenderer>();
                foreach (var sprite in sprites)
                    if (sprite.gameObject.name.ToLower().Contains("tank"))
                        tankBody.GetComponent<SpriteRenderer>().sprite = sprite.sprite;
                    else if (sprite.gameObject.name.ToLower().Contains("cannon"))
                        cannon.GetComponent<SpriteRenderer>().sprite = sprite.sprite;
            }
            else
            {
                Debug.LogError($"Sprites for colour {colour.ToString().ToLower()} not found.");
            }
        }

        // locks controls so tank can't take actions anymore
        public void LockControls()
        {
            controlsLocked = true;
        }

        // unlocks controls, so tank can take actions again
        public void UnlockControls()
        {
            controlsLocked = false;
        }
    }
}