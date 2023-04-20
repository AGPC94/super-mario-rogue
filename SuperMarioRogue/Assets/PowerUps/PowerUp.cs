using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName ="New PowerUp", menuName = "PowerUp")]
public class PowerUp : ScriptableObject
{
    [Header("Aspect")]
    public Sprite sprite;
    public RuntimeAnimatorController aniPowerUp;
    public RuntimeAnimatorController aniMario;

    [Header("Movement Item")]
    public float delay;
    public float upSpeed;
    public float timeMoveUp;
    public float hspeed;
    public bool bounces;

    [Header("Actions")]
    public bool hasAttack;
    public bool hasDoubleJump;
    public float frogJump;

    [Header("Projectile")]
    public GameObject projectile;
    

}
