using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public Text scoreUI;

    void Update()
    {
        scoreUI.text = ScoreKeeper.score.ToString("D6");
    }
}
