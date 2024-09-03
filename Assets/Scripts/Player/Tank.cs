using Projectile;
using TMPro;
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
        [SerializeField] private Transform tankBodyOutline;
        [SerializeField] private Transform tankCannonOutline;
        [SerializeField] private Slider tankHealthBar;
        [SerializeField] private TMP_Text playerNameText;

        [Header("Health")] [SerializeField] private float maxHp = 100;

        [Range(0, 100)] [SerializeField] private float hp = 100;

        [SerializeField] private bool controlsLocked;
        [SerializeField] public bool activePlayer;

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

        private void Update()
        {
            HandleCannonRotation();
            HandleMovement();
            UpdateOutlineVisibility();
        }

        private void HandleCannonRotation()
        {
            if (_cannonInput != 0 && !controlsLocked)
            {
                var newAngle = cannon.localEulerAngles.z + _cannonInput * _currentSpeedCannon * Time.deltaTime;
                newAngle = Mathf.Clamp(newAngle, 0, 180);
                cannon.localEulerAngles = new Vector3(cannon.localEulerAngles.x, cannon.localEulerAngles.y, newAngle);
                _currentSpeedCannon = Mathf.Min(cannonMaxSpeed, _currentSpeedCannon * 1.01f);
            }
            else
            {
                _cannonInput = 0;
            }
        }

        private void HandleMovement()
        {
            if (_movementInput != 0 && !controlsLocked)
            {
                var move = new Vector3(_movementInput, 0, 0);
                transform.position += move * (_currentSpeedTank * Time.deltaTime);
                _currentSpeedTank = Mathf.Min(tankMaxSpeed, _currentSpeedTank * 1.01f);
            }
            else
            {
                _movementInput = 0;
            }
        }

        private void UpdateOutlineVisibility()
        {
            tankBodyOutline.gameObject.SetActive(activePlayer);
            tankCannonOutline.gameObject.SetActive(activePlayer);
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
            power = Mathf.Clamp(newPower, 0, 50f);
            return power;
        }

        public float GetPower()
        {
            return power;
        }

        public void SetColourAndName(EColour playerColour, string playerName)
        {
            var tankObject = _playerController.GetTankSprite(playerColour);
            if (tankObject != null)
            {
                var sprites = tankObject.GetComponentsInChildren<SpriteRenderer>();
                foreach (var sprite in sprites)
                    if (sprite.gameObject.name.ToLower().Contains("tank"))
                    {
                        if (sprite.gameObject.name.ToLower().Contains("outline"))
                            tankBodyOutline.GetComponent<SpriteRenderer>().sprite = sprite.sprite;
                        else
                            tankBody.GetComponent<SpriteRenderer>().sprite = sprite.sprite;
                    }
                    else if (sprite.gameObject.name.ToLower().Contains("cannon"))
                    {
                        if (sprite.gameObject.name.ToLower().Contains("outline"))
                            tankCannonOutline.GetComponent<SpriteRenderer>().sprite = sprite.sprite;
                        else
                            cannon.GetComponent<SpriteRenderer>().sprite = sprite.sprite;
                    }

                // Set Style
                playerNameText.SetText(playerName);
                playerNameText.color = playerColour.GetColour();
            }
            else
            {
                Debug.LogError($"Sprites for colour {playerColour.ToString().ToLower()} not found.");
            }
        }

        public void LockControls()
        {
            controlsLocked = true;
        }

        public void UnlockControls()
        {
            controlsLocked = false;
        }
    }
}