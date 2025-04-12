using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleBoardScript : MonoBehaviour
{
    public Text playerWinText;
    public Text healthText;
    public int playerHealth;

    public CalculateScoreScript riverScoreScript;
    public CalculateScoreScript handScoreScript;

    // Start is called before the first frame update
    void Start()
    {
        playerWinText.text = "";
        healthText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        // Reset Game
        if (riverScoreScript.score == 0 && handScoreScript.score == 0)
        {
            playerWinText.text = "";
        }

        if (riverScoreScript.score > 21)
        {
            playerWinText.text = "Hand Player Wins!!";
        }
        else if (handScoreScript.score > 21)
        {
            playerWinText.text = "River Player Wins!!";
        }

        playerHealth = Mathf.Clamp(playerHealth, 0, 100);

        healthText.text = "Health: " + playerHealth;
    }

    public void findWinnner()
    {
        if (riverScoreScript.score < handScoreScript.score)
        {
            playerWinText.text = "Hand Player Wins!!";
        }
        else if (riverScoreScript.score > handScoreScript.score)
        {
            playerWinText.text = "River Player Wins!!";
        }
        else
        {
            playerWinText.text = "TIE";
        }
    }

    public void enemyTurn()
    {
        playerHealth -= riverScoreScript.score;

        if (playerHealth <= 0)
        {
            findWinnner();
        }
    }
}
