using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomItem : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] Sprite[] sprites;
    [SerializeField] Image image;
    [SerializeField] float fps = 10;
    [Space]
    [SerializeField] Sprite sprQBlock;

    [Header("Booleans")]
    [SerializeField] bool isStopped;
    [SerializeField] bool canInput;
    [SerializeField] bool isRunning;

    [Header("Times")]
    [SerializeField] float timeMax;
    [SerializeField] float timeCooldown;
    int index;

    [Header("Items")]
    [SerializeField] PowerUp[] powerUps;

    CanvasGroup group;
    Animator anim;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        anim = GetComponent<Animator>();
        canInput = true;
    }

    void Update()
    {
        if (GameManager.instance.coins >= 20)
        {
            group.alpha = 1;
            if (Input.GetButtonDown("Item") && canInput && isStopped)
                Play();
        }
        else if (!isRunning)
            group.alpha = 0.5f;

        if (Input.GetButtonDown("Item") && canInput && isRunning)
            Stop();
    }

    public void Play()
    {
        isRunning = true;
        isStopped = false;
        canInput = false;
        GameManager.instance.PayCoins(20);
        StopAllCoroutines();
        StartCoroutine(AnimSequence());
        StartCoroutine(CountDown());
        AudioManager.instance.Play("RuletteStart");
    }

    public void Stop()
    {
        isStopped = true;
        canInput = false;
        anim.SetTrigger("End");
        StopAllCoroutines();
        ShowFrame();
        SelectItem();
        Invoke("CooldDownCanInput", timeCooldown);
        AudioManager.instance.Stop("RuletteMove");
        AudioManager.instance.Play("RuletteStop");
    }

    void CooldDownCanInput()
    {
        canInput = true;
        isRunning = false;
        image.sprite = sprQBlock;
    }

    void SelectItem()
    {
        Player player = FindObjectOfType<Player>();

        switch (index)
        {
            case 0:
                Debug.Log("SuperMushroom");
                GameManager.instance.Heal(1);
                AudioManager.instance.Play("PowerUp");
                break;

            case 1:
                Debug.Log("FireFlower");
                player.SetPowerUpWithTransformation(powerUps[0]);
                AudioManager.instance.Play("PowerUp");
                break;

            case 2:
                Debug.Log("SuperLeaf");
                player.SetPowerUpWithTransformation(powerUps[1]);
                AudioManager.instance.Play("PowerUp");
                break;

            case 3:
                Debug.Log("FrogSuit");
                player.SetPowerUpWithTransformation(powerUps[2]);
                AudioManager.instance.Play("PowerUp");
                break;

            case 4:
                Debug.Log("HammerSuit");
                player.SetPowerUpWithTransformation(powerUps[3]);
                AudioManager.instance.Play("PowerUp");
                break;

            case 5:
                Debug.Log("SuperStar");
                player.StartStarEffect();
                break;

            case 6:
                Debug.Log("LifeMushroom");
                GameManager.instance.LifeMushroom();
                AudioManager.instance.Play("ExtraLife");
                break;
        }
    }

    IEnumerator AnimSequence()
    {
        anim.SetTrigger("Start");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1);
        var delay = new WaitForSeconds(1 / fps);
        while (true)
        {
            index++;
            if (index >= sprites.Length)
                index = 0;
            ShowFrame();
            yield return delay; 
            AudioManager.instance.Play("RuletteMove");
            canInput = true;
        }
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(timeMax);
        Stop();
    }

    void ShowFrame()
    {
        image.sprite = sprites[index];
    }

    
}