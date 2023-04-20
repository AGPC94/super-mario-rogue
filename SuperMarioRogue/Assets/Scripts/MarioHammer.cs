using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioHammer : MonoBehaviour
{
    Projectile projectile;
    Player player;

    void Awake()
    {
        projectile = GetComponent<Projectile>();

        player = transform.parent.Find("Character").GetComponent<Player>();

        projectile.InheritSpeed(player.Velocity.x, player.Direction);
    }
}
