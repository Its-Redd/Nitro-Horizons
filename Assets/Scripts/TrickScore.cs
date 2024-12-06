using TMPro;
using UnityEngine;

public class TrickScore : MonoBehaviour
{
    public TextMeshProUGUI trickScoreText;
    public TextMeshProUGUI playerRating;
    private float trickScoreIncrement = 1f;
    private float trickScoreTimer = 0f;
    private bool isTrickCounterActive = false;

    void Start()
    {
    }

    void Update()
    {
        if (PlayerPrefs.GetFloat("TrickScore") > 0)
        {
            playerRating.text = "Player Rating: " + PlayerPrefs.GetFloat("TrickScore").ToString();
        }

        if (isTrickCounterActive)
        {
            trickScoreTimer += Time.deltaTime;
            if (trickScoreTimer >= 1f)
            {
                trickScoreTimer = 0f;
                PlayerPrefs.SetFloat("TrickScore", PlayerPrefs.GetFloat("TrickScore") + trickScoreIncrement);
                PlayerPrefs.Save();
                trickScoreText.text = "Trick Score: " + trickScoreIncrement.ToString();
                Invoke("hideTrickScore", 3f);
            }
        }
    }

    public void startTrickCounter(float scoreToAdd)
    {
        if (scoreToAdd > 0)
        {
            trickScoreIncrement = scoreToAdd;
            showTrickScore();
            isTrickCounterActive = true;
        }
    }

    public void stopTrickCounter()
    {
        isTrickCounterActive = false;
        trickScoreText.text = null;
    }

    void hideTrickScore()
    {
        trickScoreText.transform.localScale = new Vector3(0f, 0f, 0f);
    }

    void showTrickScore()
    {
        trickScoreText.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
