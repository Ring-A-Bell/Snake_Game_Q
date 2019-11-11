using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        transform.Find("Play").GetComponent<Button_UI>().ClickFunc = () =>
        {
            Loader.Load(Loader.Scene.GameScene);
        };
    }
}
