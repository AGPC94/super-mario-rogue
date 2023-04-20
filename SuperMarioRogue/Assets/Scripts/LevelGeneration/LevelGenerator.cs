using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public enum SectionType { BEGIN, MID, STAIRS, FLAG, BOSS, BONUS }
public enum LevelType { GROUND, UNDERGROUND, CASTLE, ANY, ISLAND }
enum CoinLayout { ROW, ARC, WAVE }
enum SpecialStat { NORMAL, LAKITU, BULLETBILLS }

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] ColorToPrefab[] colorMappings;

    Tilemap tilemap;

    [Header("Sections")]
    List<Section> allSections = new List<Section>();
    List<Section> sectionsInLevel = new List<Section>();
    //[Space]
    List<Section> sectionsBegin = new List<Section>();
    List<Section> sectionsMid = new List<Section>();
    List<Section> sectionsStairs = new List<Section>();
    List<Section> sectionsFlag = new List<Section>();
    List<Section> sectionsBoss = new List<Section>();
    List<Section> sectionsBonus = new List<Section>();

    [Header("Layers")]
    [SerializeField] LayerMask whatIsEnemy;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] LayerMask whatIsSafeZone;

    [Header("Procedural objects")]
    [SerializeField] GameObject[] items;

    [Header("Backgrounds")]
    [SerializeField] GameObject bgColor;
    [SerializeField] Color blue;
    [SerializeField] Color black;
    [SerializeField] GameObject bgMountains;
    [SerializeField] GameObject bgClouds;
    [SerializeField] GameObject bgLava;


    const float MIN_LENGTH = 2204;

    public readonly float MAX_LENGTH = 6144;

    public readonly float MULTIPLE = 256;
    
    const float DISTANCE_BETWEEN_SECTIONS = 16;
    
    float width_units;

    float posX;

    public static LevelGenerator instance;

    float width_pixels;

    const float MIN_DIFFICULTY = 1;

    const float MAX_DIFFICULTY = 9;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        GenerateLevel();
    }


    void GenerateLevel()
    {
        ReadSectionsFromResources();
        
        GenerateLevelType();

        SetWidth();

        GenerateSectionLists();

        GenerateSections();

        GenerateMap();

        AssignCameraToPlayer();

        GenerateBackgrounds();

        GenerateRandomEnemies();

        CreateSafeZones();

        PutItemsInQBLocks();

        PlayMusic();
    }

    void ReadSectionsFromResources()
    {
        Section[] sections1 = Resources.LoadAll("Sections/Begin", typeof(Section)).Cast<Section>().ToArray();
        Section[] sections2 = Resources.LoadAll("Sections/Boss", typeof(Section)).Cast<Section>().ToArray();
        Section[] sections3 = Resources.LoadAll("Sections/Flag", typeof(Section)).Cast<Section>().ToArray();
        Section[] sections4 = Resources.LoadAll("Sections/Mid", typeof(Section)).Cast<Section>().ToArray();
        Section[] sections5 = Resources.LoadAll("Sections/Stairs", typeof(Section)).Cast<Section>().ToArray();

        allSections = sections1.Concat(sections2).Concat(sections3).Concat(sections4).Concat(sections5).ToList();

        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
    }

    void GenerateLevelType()
    {
        int rnd = Random.Range(1, 3);

        switch (GameManager.instance.level.level)
        {
            case 1:
                GameManager.instance.levelType = LevelType.GROUND;
                break;
            case 2:
                if (rnd == 1)
                    GameManager.instance.levelType = LevelType.UNDERGROUND;
                else
                    GameManager.instance.levelType = LevelType.GROUND;
                break;
            case 3:
                if (rnd == 1)
                    GameManager.instance.levelType = LevelType.ISLAND;
                else
                    GameManager.instance.levelType = LevelType.GROUND;
                break;
            case 4:
                GameManager.instance.levelType = LevelType.CASTLE;
                break;
        }
    }

    void SetWidth()
    {
        float w = GameManager.instance.maxLengthCurrent;
        if (w != MIN_LENGTH)
        {
            do
            {
                w = Random.Range(MIN_LENGTH, MAX_LENGTH);
            } while (!IsMultiple(w, MULTIPLE));
        }

        width_pixels = w;
        width_units = w / MULTIPLE;

        bool IsMultiple(float number, float multiple)
        {
            return number % multiple == 0;
        }
    }

    void GenerateSectionLists()
    {
        GenerateSectionList(SectionType.BEGIN, sectionsBegin);
        GenerateSectionList(SectionType.MID, sectionsMid);
        GenerateSectionList(SectionType.STAIRS, sectionsStairs);
        GenerateSectionList(SectionType.FLAG, sectionsFlag);

        GenerateSectionList(SectionType.BONUS, sectionsBonus);

        GenerateSectionList(SectionType.BOSS, sectionsBoss);
    }

    void GenerateSectionList(SectionType section, List<Section> list)
    {
        foreach (var item in allSections)
            if (item.section == section && ((item.level == GameManager.instance.levelType && item.difficulty <= GameManager.instance.difficulty) || item.level == LevelType.ANY))
                list.Add(item);
    }

    void GenerateSections()
    {
        RandomSection(sectionsBegin);

        for (int i = 0; i < width_units - 3; i++)
            RandomSection(sectionsMid);

        if (GameManager.instance.levelType == LevelType.CASTLE)
            RandomSection(sectionsBoss);
        else
        {
            RandomSection(sectionsStairs);
            RandomSection(sectionsFlag);
        }
    }

    void RandomSection(List<Section> sections)
    {
        int rnd;
        do
        {
            rnd = Random.Range(0, sections.Count);
        } while (sectionsInLevel.Contains(sections[rnd]));

        sectionsInLevel.Add(sections[rnd]);
    }

    void GenerateMap()
    {
        foreach (Section section in sectionsInLevel)
        {
            for (int x = 0; x < section.map.width; x++)
                for (int y = 0; y < section.map.height; y++)
                    GenerateTile(x, y, section.map);
            posX += DISTANCE_BETWEEN_SECTIONS;
        }
    }

    void GenerateTile(int x, int y, Texture2D map)
    {
        Color pixelColor = map.GetPixel(x, y);

        if (pixelColor.a == 0)
            return;

        foreach (ColorToPrefab colorMapping in colorMappings)
        {
            if (IsTheSameColor(colorMapping.color, pixelColor))
            {
                Vector2 position = new Vector2(x + posX, y);
                if (colorMapping.prefab != null)
                {
                    Instantiate(colorMapping.prefab, position, Quaternion.identity, transform);
                }
                else if (colorMapping.tile != null)
                {
                    Vector3Int posInt = new Vector3Int((int)position.x, (int)position.y, 0);
                    if (colorMapping.tile.m_DefaultGameObject != null)
                        Instantiate(colorMapping.tile.m_DefaultGameObject, position, Quaternion.identity, transform);
                    tilemap.SetTile(posInt, colorMapping.tile);
                }

            }
        }

        bool IsTheSameColor(Color c1, Color c2)
        {
            return (int)(c1.r * 1000) == (int)(c2.r * 1000) && (int)(c1.g * 1000) == (int)(c2.g * 1000) && (int)(c1.b * 1000) == (int)(c2.b * 1000);
        }
    }

    void AssignCameraToPlayer()
    {
        CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();
        Player player = FindObjectOfType<Player>();
        vcam.Follow = player.transform;
    }

    void GenerateBackgrounds()
    {
        //Color
        if (GameManager.instance.levelType == LevelType.UNDERGROUND || GameManager.instance.levelType == LevelType.CASTLE)
            bgColor.GetComponent<SpriteRenderer>().color = black;
        else
            bgColor.GetComponent<SpriteRenderer>().color = blue;

        bgColor.transform.position = new Vector2((posX / 2) - 0.5f, 15 / 2);
        bgColor.transform.localScale = new Vector2(posX, 15);

        //Backgrounds
        bgMountains.SetActive(false);
        bgClouds.SetActive(false);
        bgLava.SetActive(false);

        switch (GameManager.instance.levelType)
        {
            case LevelType.GROUND:
                bgMountains.SetActive(true);
                break;

            case LevelType.CASTLE:
                bgLava.SetActive(true);
                bgLava.transform.position = new Vector2((posX / 2) - 0.5f, 0);
                bgLava.GetComponent<SpriteRenderer>().size = new Vector2(posX, 2);
                break;

            case LevelType.ISLAND:
                bgClouds.SetActive(true);
                break;
        }
    }

    void GenerateRandomEnemies()
    {
        for (int i = 0; i < width_units; i++)
        {
            int noEnemyProb = Random.Range(1, 101);
            if (noEnemyProb <= 20)
                continue;

            //N de enemigos al azar
            int nEnemies = Random.Range(1, (int)GameManager.instance.enemiesBySection + 1);

            //Posicion en el centro de la sección
            float insPosX = posX;
            float insPosY = 0;// / 2;
            Vector2 insPos = new Vector2(insPosX, insPosY);

            //Tamaño de la sección
            Vector2 insSize = new Vector2(16, 12);

            //Instanciar cada enemigo hasta el nº máx
            for (int j = 0; j < nEnemies; j++)
            {
                //Variables
                float rndPosX;
                float rndPosY;
                Vector2 rndPos;
                Vector2 objSize;
                Vector2 snapPos;
                RaycastHit2D hit;
                RaycastHit2D hitEnemy;

                do
                {
                    //Random position until hit with ground
                    do
                    {
                        //Random position
                        rndPosX = Random.Range(0 + 1, insPos.x + insSize.x - 1);
                        rndPosY = Random.Range(0 + 1, insPos.y + insSize.y - 1);
                        rndPos = new Vector2(rndPosX, rndPosY);

                        //Size
                        objSize = new Vector2(0.9f, 0.9f);

                        //Snap to grid
                        snapPos = new Vector2(Mathf.Floor(rndPos.x), Mathf.Floor(rndPos.y));

                        //Raycast
                        hit = Physics2D.BoxCast(snapPos, objSize, 0, Vector2.zero, 0, whatIsGround);

                    } while (!hit);

                    //Sube hasta que se posicione arriba del todo del suelo
                    float offset = 1;
                    RaycastHit2D hitUp;
                    do
                    {
                        snapPos.y += offset;
                        hitUp = Physics2D.BoxCast(snapPos, objSize, 0, Vector2.zero, 0, whatIsGround);
                    } while (hitUp);

                    //Comprobar si hay enemigos
                    Vector2 enemySize = new Vector2(2f, 1f);
                    //Mismo lugar
                    hitEnemy = Physics2D.BoxCast(snapPos, enemySize, 0, Vector2.zero, 0, whatIsEnemy);

                } while (hitEnemy);

                #region Instanciar enemigo

                int rndEnemy = Random.Range(1, 101);

                GameObject cloneEnemy = null;

                foreach (SpawnEnemy enemy in GameManager.instance.enemies)
                {
                    if (rndEnemy > enemy.probMin && rndEnemy <= enemy.probMax)
                    {
                        cloneEnemy = enemy.prefab;
                    }
                    //Debug.Log($"ENEMY PROB: {enemy.prefab.name}, {enemy.probMin}-{enemy.probMax}");
                }

                if (cloneEnemy != null)
                    Instantiate(cloneEnemy, snapPos, Quaternion.identity, transform);
                #endregion
            }
        }
    }

    void CreateSafeZones()
    {
        Vector2 insSize = new Vector2(16, 16);
        DestroyObjectsInArea(new Vector2(8, 7));
        DestroyObjectsInArea(new Vector2(posX - 8, 7));

        void DestroyObjectsInArea(Vector2 position)
        {
            RaycastHit2D[] hitSafeZone = Physics2D.BoxCastAll(position, insSize, 0, Vector2.zero, 0, whatIsEnemy);
            foreach (var item in hitSafeZone)
            {
                if (item.collider != null)
                    Destroy(item.collider.gameObject);
            }
        }
    }

    void PutItemsInQBLocks()
    {
        BlockQuestion[] blocksQ = FindObjectsOfType<BlockQuestion>();
        System.Array.Reverse(blocksQ);

        //int nItem = 3;        for (int i = 0; i < nItem; i++){        }
        for (int j = 0; j < blocksQ.Length; j++)
        {
            if (j % 4 == 0)
            {
                blocksQ[j].item = items[Random.Range(0, items.Length)];
            }
        }
    }

    void PlayMusic()
    {
        switch (GameManager.instance.levelType)
        {
            case LevelType.GROUND:
                AudioManager.instance.PlayMusic("Overworld");
                break;
            case LevelType.UNDERGROUND:
                AudioManager.instance.PlayMusic("Underground");
                break;
            case LevelType.CASTLE:
                AudioManager.instance.PlayMusic("Castle");
                break;
            case LevelType.ISLAND:
                AudioManager.instance.PlayMusic("Overworld");
                break;
        }
    }


    /*Descartad
    void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(new Vector2(8, 7), new Vector2(16, 16));
        //Gizmos.DrawWireCube(new Vector2(posX - 8, 7), new Vector2(16, 16));
    }
    void GenerateRandomCoins()
    {
        /*
         for secciones
            si en la seccion no hay bloques
                cantidad al azar de monedas de 0 a 6
                layout al azar: fila, arco o w
              
                
                recorro la sección en una posicion al azar entre 3 y 6 encima de la tierra. Si ahí hay hueco suficiente, las instancio


            si en la seccion hay una isla/puente/champiñón
                
         
        for (int i = 2; i < width_units - 1; i++)
        {
            //Posicion en el centro de la sección
            float insPosX = posX;
            float insPosY = 0;
            Vector2 insPos = new Vector2(insPosX, insPosY);

            //Tamaño de la sección
            Vector2 insSize = new Vector2(16, 16);

            //Variables
            float rndPosX;
            float rndPosY;
            Vector2 rndPos = Vector2.zero;
            Vector2 objSize;
            Vector2 snapPos;
            RaycastHit2D hitBlock = Physics2D.BoxCast(insPos, insSize, 0, Vector2.zero, 0, whatIsBlock);

            //Comprobar que no hay ni bloques ni ladrillos
            if (!hitBlock)
            {
                int nConins = Random.Range(0, 7);
                CoinLayout coinLayout = (CoinLayout)Random.Range(0, 3);

                RaycastHit2D hitGround = Physics2D.BoxCast(insPos, insSize, 0, Vector2.zero, 0, whatIsTerrain);

                if (hitGround)
                {
                    //Buscar suelo
                    do
                    {
                        //Random position
                        rndPosX = Random.Range(0 + 1, insPos.x + insSize.x - 1);
                        rndPosY = Random.Range(0 + 1, insPos.y + insSize.y - 1);
                        rndPos = new Vector2(rndPosX, rndPosY);

                        //Size
                        objSize = new Vector2(1, 1);

                        if (coinLayout == CoinLayout.ROW)
                            objSize.y = 1;
                        else
                            objSize.y = 2;

                        //Snap to grid
                        snapPos = new Vector2(Mathf.Floor(rndPos.x), Mathf.Floor(rndPos.y));

                        //Raycast
                        hitGround = Physics2D.BoxCast(snapPos, objSize, 0, Vector2.zero, 0, whatIsTerrain);

                    } while (!hitGround);
                }

                //Posicion encima del suelo
                objSize = new Vector2(nConins, 0);
                rndPos.y += Random.Range(3, 7);
                snapPos = new Vector2(Mathf.Floor(rndPos.x), Mathf.Floor(rndPos.y));

                RaycastHit2D hitEmpty = Physics2D.BoxCast(snapPos, objSize, 0, Vector2.zero, 0); ;

                if (!hitEmpty)
                {
                    GameObject cloneCoin = prefabCoin;

                    float originalPosY = rndPos.y;

                    switch (coinLayout)
                    {
                        case CoinLayout.ROW:
                            for (int k = 0; k < nConins; k++)
                            {
                                rndPos.x += 1;
                                snapPos = new Vector2(Mathf.Floor(rndPos.x), Mathf.Floor(rndPos.y));
                                Instantiate(cloneCoin, snapPos, Quaternion.identity, transform);
                            }
                            break;
                        case CoinLayout.ARC:
                            for (int k = 0; k < nConins; k++)
                            {
                                rndPos.x += 1;

                                if (k == 0 || k == nConins - 1)
                                    rndPos.y = originalPosY;
                                else
                                    rndPos.y = originalPosY + 1;

                                snapPos = new Vector2(Mathf.Floor(rndPos.x), Mathf.Floor(rndPos.y));
                                Instantiate(cloneCoin, snapPos, Quaternion.identity, transform);
                            }
                            break;
                        case CoinLayout.WAVE:
                            for (int k = 0; k < nConins; k++)
                            {
                                rndPos.x += 1;

                                if ((k % 2) == 0)
                                    rndPos.y = originalPosY + 1;
                                else
                                    rndPos.y = originalPosY;

                                snapPos = new Vector2(Mathf.Floor(rndPos.x), Mathf.Floor(rndPos.y));
                                Instantiate(cloneCoin, snapPos, Quaternion.identity, transform);
                            }
                            break;
                    }

                }
            }
        }

    }
    void PutItemsInBricks()
    {
        BlockBrick[] bricks = FindObjectsOfType<BlockBrick>();
        System.Array.Reverse(bricks);

        //int nItem = 3;        for (int i = 0; i < nItem; i++){        }
        for (int j = 0; j < bricks.Length; j++)
        {
            if (j % 10 == 0)
            {
                bricks[j].item = items[Random.Range(0, items.Length)];
            }
        }
    }
    */

}
