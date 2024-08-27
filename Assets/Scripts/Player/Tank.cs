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

        [SerializeField] private Slider powerSlider;
        [SerializeField] private Slider tankHealthBar;

        private float _currentSpeedCannon;
        private float _currentSpeedTank;

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

        public void Move(float movementInput)
        {
            var move = new Vector3(movementInput, 0, 0);
            transform.position += move * (_currentSpeedTank * Time.deltaTime);
            _currentSpeedTank = Mathf.Min(tankMaxSpeed, _currentSpeedTank * 1.01f);
        }

        public void RotateCannon(float cannonInput)
        {
            var newAngle = cannon.localEulerAngles.z + cannonInput * _currentSpeedCannon * Time.deltaTime;
            newAngle = Mathf.Clamp(newAngle, 0, 180);
            cannon.localEulerAngles = new Vector3(cannon.localEulerAngles.x, cannon.localEulerAngles.y, newAngle);
            _currentSpeedCannon = Mathf.Min(cannonMaxSpeed, _currentSpeedCannon * 1.01f);
        }

        public void Fire()
        {
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

        public void UpdatePower(float newPower)
        {
            power = Mathf.Clamp(newPower, 0, 50f);
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
    }
}