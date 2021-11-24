using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskOne : MonoBehaviour
{
    private class Weapon
    {
        private readonly int _damage;
        private int _bullets;
        
        public Weapon(int damage, int bullets)
        {
            if (damage <= 0)
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
        public Weapon Weapon { get; }
        public Bot(Weapon weapon)
        {
            if (weapon == null)
                throw new ArgumentOutOfRangeException(nameof(weapon));
            
            Weapon = weapon;
        }

        public void OnSeePlayer(Player player)
        {
            if (Weapon.IsBulletsEnough() && player.IsAlive())
                Weapon.Shoot(player);
        }
    }

    // -- TEST -- //
    
    private Player _petya;
    private Weapon _pistol;
    private Bot _bot1;
    private void Start()
    {
        InitData(100, 150, 10);
        _bot1.OnSeePlayer(_petya);
    }

    private void InitData(int playerHealth, int weaponDamage, int bullets)
    {
        _petya = new Player(playerHealth);
        _pistol = new Weapon(weaponDamage, bullets);
        _bot1 = new Bot(_pistol);        
    }
}
