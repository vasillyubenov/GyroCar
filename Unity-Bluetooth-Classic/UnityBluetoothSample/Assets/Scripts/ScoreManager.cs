using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreLabel;
    public float scoreIncrementor;
    public float incrementDelay = 0.1f; // time delay between each increment

    private float currentScore = 0f;

    private void Start()
    {
        StartCoroutine(ScoreIncrementCoroutine());
    }

    IEnumerator ScoreIncrementCoroutine()
    {
        while (true) // loop forever
        {
            yield return new WaitForSeconds(incrementDelay); // wait for the specified delay
            currentScore += scoreIncrementor; // then increment the score
            scoreLabel.text = currentScore.ToString(); // and update the score label
        }
    }
}
