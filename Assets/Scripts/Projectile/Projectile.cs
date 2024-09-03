using UnityEngine;
using Utility;

namespace Projectile
{
    public abstract class Projectile : MonoBehaviour, IProjectile
    {
        protected const float DistanceThreshold = 0.1f;

        [SerializeField] protected float damage = 10;
        [SerializeField] protected float explosionRadius = 0.1f;

        private float _cameraLeft;
        private float _cameraRight;
        protected GameObject Owner;

        protected int TotalDamageDealt;
        protected WindEngine WindEngine;
        private Rigidbody2D Rigidbody => GetComponent<Rigidbody2D>();

        public void Start()
        {
            WindEngine = FindObjectOfType<WindEngine>();
            _cameraLeft = CameraUtility.GetCameraLeft(Camera.main);
            _cameraRight = CameraUtility.GetCameraRight(Camera.main);
        }

        public void FixedUpdate()
        {
            // Apply a smaller, continuous force to simulate wind effect
            Rigidbody.AddForce(Vector3.right * (WindEngine.GetWindSpeed() * Time.fixedDeltaTime), ForceMode2D.Force);

            // Check if bullet is outside the camera view. If so, destroy it
            if (transform.position.x < _cameraLeft || transform.position.x > _cameraRight)
                Destroy(gameObject);
        }

        public void OnDestroy()
        {
            if (Owner != null)
            {
                var player = Owner.transform.parent.GetComponent<Player.Player>();
                Debug.Log(player + " " + TotalDamageDealt + " " + Owner);
                player.score += TotalDamageDealt;
            }

            GameManager.instance.SwitchPlayer();
        }

        public float GetDamage()
        {
            return damage;
        }

        public float GetExplosionRadius()
        {
            return explosionRadius;
        }

        public void SetOwner(GameObject owner)
        {
            Owner = owner;
        }

        public GameObject GetOwner()
        {
            return Owner;
        }

        public abstract float CalculateDamage(Vector2 playerPosition);
    }
}