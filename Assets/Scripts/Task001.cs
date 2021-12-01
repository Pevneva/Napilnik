using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task001 : MonoBehaviour
{
    private class Weapon
    {
        private readonly int _damage;
        private int _bullets;
        
        public Weapon(int damage, int bullets)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage));

            if (bullets < 0)
                throw new ArgumentOutOfRangeException(nameof(bullets));
            
            _damage = damage;
            _bullets = bullets;
        }

        public void Shoot(Player player)
        {
            if (player == null)
                throw new ArgumentOutOfRangeException(nameof(player));
            
            if (IsBulletsEnough() == false)
                throw new ArgumentOutOfRangeException(nameof(player));
            
            player.ApplyDamage(_damage);
            _bullets -= 1;
        }

        public bool IsBulletsEnough()
        {
            return _bullets > 0;
        }
    }

    private class Player
    {
        private int _health;

        public Player(int health)
        {
            if (health <= 0)
                throw new ArgumentOutOfRangeException(nameof(health));
                
            _health = health;
        }

        public void ApplyDamage(int damage)
        {
            if (_health >= damage)
                _health -= damage;
            else
                _health = 0;
        }

        public bool IsAlive()
        {
            return _health > 0;
        }
    }

    private class Bot
    {
        private Weapon _weapon;
        public Bot(Weapon weapon)
        {
            if (weapon == null)
                throw new ArgumentOutOfRangeException(nameof(weapon));
            
            _weapon = weapon;
        }

        public void OnSeePlayer(Player player)
        {
            if (_weapon.IsBulletsEnough() && player.IsAlive())
                _weapon.Shoot(player);
        }
    }
}
