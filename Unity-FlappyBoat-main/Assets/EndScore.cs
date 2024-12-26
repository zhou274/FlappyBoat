using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScore : MonoBehaviour
{
    public Text text;

    public void setScore(int score) {
        text.text = "×îÖÕµÃ·Ö: "+score;
    }
}
