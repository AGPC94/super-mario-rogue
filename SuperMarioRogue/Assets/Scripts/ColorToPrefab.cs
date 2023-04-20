using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum tileType { NONE, BACKGROUND, GROUND, PLATFORM}


[CreateAssetMenu(fileName = "NewElement", menuName = "Element")]
public class ColorToPrefab : ScriptableObject
{
    public Color color;
    public GameObject prefab;
    public RuleTile tile;
}