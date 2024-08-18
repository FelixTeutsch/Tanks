using UnityEngine;
using Utility;

namespace Projectile
{
    public abstract class Projectile : MonoBehaviour, IProjectile
    {
        protected const float DistanceThreshold = 0.1f;

        [SerializeField] protected float damage = 10;
        [SerializeField] protected float explosionRadius = 0.1f;
        protected GameObject Owner;
        protected WindEngine WindEngine;
        private Rigidbody2D Rigidbody => GetComponent<Rigidbody2D>();

        public void Start()
        {
            WindEngine = FindObjectOfType<WindEngine>();
        }

        public void FixedUpdate()
        {
            // Apply a smaller, continuous force to simulate wind effect
            Rigidbody.AddForce(Vector3.right * (WindEngine.GetWindSpeed() * Time.fixedDeltaTime), ForceMode2D.Force);
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