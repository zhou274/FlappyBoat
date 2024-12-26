using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text text;
    private int score = 0;
    public static Score instance;
    public void Awake()
    {
        instance = this;
    }

    public int getScore() {
        return score;
    }

    public void setScore(int score) {
        this.score = score;
        text.text = this.score.ToString();
    }

    public void addScore(int score) {
        this.score += score;
        text.text = this.score.ToString();
    }
}
