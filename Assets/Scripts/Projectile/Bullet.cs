using UnityEngine;

namespace Projectile
{
    public class Bullet : MonoBehaviour, IProjectile
    {
        private const float DistanceThreshold = 0.1f;

        [SerializeField] private float damage = 10;
        [SerializeField] private float explosionRadius = 0.1f;
        private bool _leftCannon;
        private GameObject _owner;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log(collision);
            Debug.Log(_owner);
            Debug.Log("Bullet collided with " + collision.gameObject.name);
            // Avoid player hitting themselves (bullets are trigger) (player still takes damage from explosion)
            if (collision.gameObject.name == _owner.gameObject.name) return;

            Debug.Log("Bullet collided with " + collision.gameObject.name);
            if (collision.gameObject.CompareTag("Tank"))
            {
                var tank = collision.gameObject.GetComponent<Tank.Tank>();
                if (tank != null) tank.TakeDamage(GetDamage());
            }

            Destroy(gameObject);
        }

        public float GetDamage()
        {
            return damage;
        }

        public float GetExplosionRadius()
        {
            return explosionRadius;
        }

        public float CalculateDamage(Vector2 playerPosition)
        {
            var explosionCenter = transform.position;
            var distance = Vector2.Distance(explosionCenter, playerPosition);

            if (distance > explosionRadius + DistanceThreshold) return 0f;

            var damageFactor = 1 - distance / (explosionRadius + DistanceThreshold);
            return damage * damageFactor;
        }

        public void SetOwner(GameObject owner)
        {
            _owner = owner;
        }

        public GameObject GetOwner()
        {
            return _owner;
        }
    }
}