using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public CardHolderScript drawTake; // Reference to the CardHolderScript from which cards are drawn
    public CardHolderScript drawPlace; // Reference to the CardHolderScript to which cards are placed
    public List<int> drawIndices; // List of indices to draw

    public int minRandIndex;
    public int maxRandIndex;
    public int countRandIndex;

    // Update is called once per frame
    void Update()
    {
        

    }

    // Method to fill drawIndices with random integers
    public void GenerateRandomIndices(int min, int max, int count)
    {
        drawIndices.Clear(); // Clear the list before filling it
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(min, max);
            drawIndices.Add(randomIndex);
        }
    }

    // Wrapper method for button click
    public void OnDrawCardsButtonClick(int count)
    {
        GenerateRandomIndices(minRandIndex, maxRandIndex, count);
        drawCards(drawTake, drawPlace, drawIndices);
        shuffleCards(drawTake);
    }

    public void drawCards(CardHolderScript drawTake, CardHolderScript drawPlace, List <int> drawIndices)
    {
        // Sort indices in descending order to avoid shifting issues while removing items
        drawIndices.Sort((a, b) => b.CompareTo(a));

        foreach (int i in drawIndices)
        {
            if (i < drawTake.cards.Count && (drawPlace.maxCardsHeld - drawPlace.cards.Count > 0))
            {
                // Get the card from drawTake.cards at index i
                GameObject card = drawTake.cards[i];

                // Add the card to drawPlace.cards
                drawPlace.AddCard(card);

                // Remove the card from drawTake.cards
                drawTake.RemoveCard(card);
            }
        }
        maxRandIndex = drawTake.cards.Count;
    }

    public void shuffleCards(CardHolderScript cardHolder)
    {
        // Get all children with the tag "CardSlot"
        List<Transform> cardSlots = new List<Transform>();

        foreach (Transform child in cardHolder.transform)
        {
            if (child.CompareTag("CardSlot"))
            {
                cardSlots.Add(child);
            }
        }

        // Shuffle the list of card slots
        for (int i = 0; i < cardSlots.Count; i++)
        {
            Transform temp = cardSlots[i];
            int randomIndex = Random.Range(0, cardSlots.Count);
            cardSlots[i] = cardSlots[randomIndex];
            cardSlots[randomIndex] = temp;
        }

        // Reassign the shuffled card slots to the parent
        foreach (Transform cardSlot in cardSlots)
        {
            cardSlot.SetSiblingIndex(cardSlots.IndexOf(cardSlot));
        }
    }
}

