using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPage : MonoBehaviour
{
    public enum PageType{ SaveAndLoad, Config, Help}
    public PageType pageType;
    private const string OPEN = "Open";
    private const string CLOSE = "Close";
    [SerializeField] private Animator anim;
    public void Open()
    {
        anim.SetTrigger(OPEN);
    }

    public void Close()
    {
        anim.SetTrigger(CLOSE);
    }
}
