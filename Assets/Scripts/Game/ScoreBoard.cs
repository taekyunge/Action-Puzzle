using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField]
    private Text _ScoreText = null;

    private int _Point = 0;

    public void Clear()
    {
        _Point = 0;
        _ScoreText.text = "0";
    }

    public void AddScore(int point)
    {
        _Point += point;

        _ScoreText.text = _Point.ToString("N0");
    }
}
