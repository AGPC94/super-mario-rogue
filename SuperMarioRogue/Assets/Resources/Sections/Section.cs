using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Section", menuName = "Section")]
public class Section : ScriptableObject
{
    public Texture2D map;
    public SectionType section;
    public LevelType level;
    public float difficulty;
}
