using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollWithArrows : MonoBehaviour
{
    ScrollRect scrollrect;
    [SerializeField] float scrollSpeed;

    void Awake()
    {
        scrollrect = GetComponent<ScrollRect>();
    }
    // Update is called once per frame
    void Update()
    {
        //Up
        if (scrollrect.verticalNormalizedPosition <= 1)
        {
            if (Input.GetAxisRaw("Vertical") == 1)
                scrollrect.verticalNormalizedPosition += scrollSpeed;
        }

        //Down
        if (scrollrect.verticalNormalizedPosition >= 0)
        {
            if (Input.GetAxisRaw("Vertical") == -1)
                scrollrect.verticalNormalizedPosition -= scrollSpeed;
        }

        //scrollrect.verticalNormalizedPosition += scrollSpeed * Input.GetAxisRaw("Vertical");
        //scrollrect.verticalNormalizedPosition = Mathf.Clamp(scrollrect.verticalNormalizedPosition, 0, 1);
    }
}
