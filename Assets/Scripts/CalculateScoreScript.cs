using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculateScoreScript : MonoBehaviour
{
    public Text text;

    [HideInInspector]
    public int score = 0;
    public bool hasAce = false;

    private CardHolderScript cardHolderScript;

    // Start is called before the first frame update
    void Start()
    {
        cardHolderScript = GetComponent<CardHolderScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // Reset the score and ace flag at the start of each frame
        score = 0;
        hasAce = false;

        // Create a copy of the cards list to iterate over
        List<GameObject> cardsCopy = new List<GameObject>(cardHolderScript.cards);
        foreach (GameObject cardSlot in cardsCopy)
        {
            // Check if the cardSlot is null or has been destroyed
            if (cardSlot == null)
            {
                continue; // Skip to the next iteration if the cardSlot is null
            }

            // Ensure the cardSlot has a child
            if (cardSlot.transform.childCount > 0)
            {
                // Get the child game object
                GameObject card = cardSlot.transform.GetChild(0).gameObject;

                // Check if the card is null or has been destroyed
                if (card == null)
                {
                    continue; // Skip to the next iteration if the card is null
                }

                // Get the CardScript component
                CardScript cardScript = card.GetComponent<CardScript>();

                // Check if the cardScript is null or has been destroyed
                if (cardScript != null)
                {
                    // Add the card value to the score
                    score += cardScript.cardValue;

                    // Check if the card has a value of 11
                    if (cardScript.cardValue == 1)
                    {
                        hasAce = true; // Set the ace flag to true
                    }
                }
            }
        }

        // If the hand has an ace and adding 11 would not bust the score, add 10
        if (hasAce && score + 10 <= 21)
        {
            score += 10;
        }

        // Update the score text
        if (score < 21)
        {
            text.text = "Score: " + score.ToString();
        }
        else if (score == 21)
        {
            text.text = "Score: " + score.ToString() + "!!";
        }
        else
        {
            text.text = "Score: " + score.ToString() + " BUST!";
        }
    }
}
