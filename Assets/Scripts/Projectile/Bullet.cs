using Player;
using UnityEngine;

namespace Projectile
{
    public class Bullet : Projectile
    {
        private bool _leftCannon;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Avoid player hitting themselves (bullets are trigger) (player still takes damage from explosion)
            if (collision.gameObject.name == Owner.gameObject.name) return;

            if (collision.gameObject.CompareTag("Tank"))
            {
                var tank = collision.gameObject.GetComponent<Tank>();
                if (tank != null) tank.TakeDamage(GetDamage());
            }

            Destroy(gameObject);
        }

        public override float CalculateDamage(Vector2 playerPosition)
        {
            var explosionCenter = transform.position;
            var distance = Vector2.Distance(explosionCenter, playerPosition);

            if (distance > explosionRadius + DistanceThreshold) return 0f;

            var damageFactor = 1 - distance / (explosionRadius + DistanceThreshold);
            return damage * damageFactor;
        }
    }
}