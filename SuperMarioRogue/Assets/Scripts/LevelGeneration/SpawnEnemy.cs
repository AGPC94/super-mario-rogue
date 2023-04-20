using UnityEngine;

[System.Serializable]
public class SpawnEnemy
{
    public GameObject prefab;

    public float probMin;
    public float probMax;
    public float increase;

    public void IncreaseProbability()
    {
        if (probMin > 0)
            probMin += increase;
        if (probMax < 100)
            probMax += increase;
    }
}
