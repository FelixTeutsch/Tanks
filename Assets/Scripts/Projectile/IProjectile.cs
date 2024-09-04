using Player;
using UnityEngine;

namespace Projectile
{
    /// <summary>
    ///     Interface for projectiles in the game.
    /// </summary>
    public interface IProjectile
    {
        /// <summary>
        ///     Gets the damage dealt by the projectile.
        /// </summary>
        /// <returns>The amount of damage.</returns>
        float GetDamage();

        /// <summary>
        ///     Gets the explosion radius of the projectile.
        /// </summary>
        /// <returns>The radius of the explosion.</returns>
        float GetExplosionRadius();

        /// <summary>
        ///     Calculates the damage based on the distance of the player from the explosion center.
        /// </summary>
        /// <param name="playerPosition">The position of the player.</param>
        /// <returns>The calculated damage.</returns>
        float CalculateDamage(Vector2 playerPosition);


        /// <summary>
        ///     Sets the owner of the bullet.
        /// </summary>
        /// <param name="owner">The player who fired the bullet.</param>
        public void SetOwner(PlayerObject owner);

        /// <summary>
        ///     Gets the owner of the bullet.
        /// </summary>
        /// <returns>The player who fired the bullet.</returns>
        public PlayerObject GetOwner();
    }
}