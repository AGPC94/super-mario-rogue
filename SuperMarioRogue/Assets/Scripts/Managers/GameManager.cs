using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float maxLives;
    public float lives;
    public float coins;
    public Level level;

    public LevelType levelType;

    public PowerUp powerUp;


    public SpawnEnemy[] enemies;

    public float difficulty;
    public float enemiesBySection;
    public float maxLengthCurrent = 2204;

    public static GameManager instance;

    [SerializeField] PowerUp normal;
    void Awake()
    {

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        lives = Mathf.Clamp(lives, 0, maxLives);
        coins = Mathf.Clamp(coins, 0, 999);
    }

    public void Reset()
    {
        maxLives = 3;
        lives = 3;
        coins = 0;
        powerUp = normal;
        level = new Level(1, 1);
        enemiesBySection = 2;
        maxLengthCurrent = 2204;
    }

    public void LifeMushroom()
    {
        AudioManager.instance.Play("ExtraLife");
        SetMaxLives(6);
        Heal(6);

    }

    public void SetMaxLives(float l)
    {
        maxLives = l;
    }

    public void Heal(float l)
    {
        lives += l;
    }

    public void Damage(float l)
    {
        lives -= l;
    }

    public void AddCoin()
    {
        coins++;
    }

    public void PayCoins(float c)
    {
        coins -= c;
    }

    public void NextLevel()
    {
        level.level++;

        if (level.level > 4)
        {
            level.world++;
            level.level = 1;
        }
        //LevelGenerator.instance.IncreaseDifficulty();
        IncreaseDifficulty();
        AudioManager.instance.StopMusic();
        SceneLoader.instance.LoadScene("Game");
    }

    public void IncreaseDifficulty()
    {
        if (level.level == 1 && enemiesBySection < 4)
            enemiesBySection++;

        //foreach (SpawnEnemy enemy in enemies)
            //enemy.IncreaseProbability();

        if (level.level % 2 == 0
            &&
            maxLengthCurrent < LevelGenerator.instance.MAX_LENGTH)
            maxLengthCurrent += LevelGenerator.instance.MULTIPLE;
    }

    public bool IsANewRecord()
    {
        return level.world > GetRecord().world || (level.world == GetRecord().world && level.level > GetRecord().level);
    }

    public Level GetRecord()
    {
        float w = PlayerPrefs.GetFloat("World", 1);
        float l = PlayerPrefs.GetFloat("Level", 1);
        Level lvl = new Level(w, l);
        return lvl;
    }

    public string GetTopName()
    {
        return PlayerPrefs.GetString("Name", "Super Mario");
    }

    public void SetRecord(string name)
    {
        float w = level.world;
        float l = level.level;
        PlayerPrefs.SetFloat("World", w);
        PlayerPrefs.SetFloat("Level", l);
        PlayerPrefs.SetString("Name", name);
    }

    public int ParseLevelToInt()
    {
        //Guardar nivel y subirlo al leaderboard
        int w = (int)level.world;
        int l = (int)level.level;
        string s = w.ToString() + l.ToString();
        int wlInt = int.Parse(s);

        return wlInt;
    }

    public string LoadLevel(int lvl)
    {
        //Cargar nivel de cada usuario para mostrar el leaderboard
        string s2 = lvl.ToString();
        string level = s2[s2.Length - 1].ToString();
        string world = string.Empty;

        for (int i = 0; i < s2.Length - 1; i++)
            world += s2[i];

        return world + "-" + level;
    }
}

[System.Serializable]
public class Level
{
    public float world;
    public float level;
    public override string ToString() => world + "-" + level;

    public Level(float w = 1, float l = 1)
    {
        world = w;
        level = l;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Level))
            return false;

        return level == ((Level)obj).level && world == ((Level)obj).world;
    }
}
