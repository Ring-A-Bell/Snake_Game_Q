using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
    private Text ScoreText;
    private void Awake()
    {
        ScoreText = transform.Find("ScoreText").GetComponent<Text>();
    }
    private void Update()
    {
        ScoreText.text = GameHandler.GetScore().ToString();
    }
}
