using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AddEventTrigger : MonoBehaviour
{
    EventTrigger eventTrigger;

    void Awake()
    {
        AddEventTriggerByScript();
        AddMove();
        AddSubmit();
        AddClick();
    }

    void AddEventTriggerByScript()
    {
        //Component
        if (GetComponent<EventTrigger>() == null)
            gameObject.AddComponent<EventTrigger>();
        eventTrigger = GetComponent<EventTrigger>();
    }

    void AddMove()
    {
        //Move
        EventTrigger.Entry entryMove = new EventTrigger.Entry();

        entryMove.eventID = EventTriggerType.Move;

        entryMove.callback.AddListener((functionIWant) => { PlaySound("Fireball"); });

        eventTrigger.triggers.Add(entryMove);
    }
    void AddSubmit()
    {
        //Submit
        EventTrigger.Entry entrySubmit = new EventTrigger.Entry();

        entrySubmit.eventID = EventTriggerType.Submit;

        entrySubmit.callback.AddListener((functionIWant2) => { PlaySound("Coin"); });

        eventTrigger.triggers.Add(entrySubmit);
    }
    void AddClick()
    {
        //Submit
        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.PointerClick;

        entry.callback.AddListener((functionIWant3) => { PlaySound("Coin"); });

        eventTrigger.triggers.Add(entry);
    }

    void PlaySound(string sound)
    {
        AudioManager.instance.Play(sound);
    }
}
