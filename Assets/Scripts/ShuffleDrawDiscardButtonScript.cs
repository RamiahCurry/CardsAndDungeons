using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleDrawDiscardButtonScript : MonoBehaviour
{
    public CardHolderScript[] drawTake; // Reference to the CardHolderScript from which cards are drawn
    public CardHolderScript[] drawPlace; // Reference to the CardHolderScript to which cards are placed
    public List<int> drawIndices; // List of indices to draw

    public int minRandIndex;
    public int maxRandIndex;
    public int countRandIndex;

    public bool automaticDeal = false;
    public bool automaticTake = false;
    private float drawCooldown = 1.0f; // 1 second cooldown
    private float lastDrawTime = 0f;

    void Update()
    {
        int cardNumsinHolder = 0;

        foreach (CardHolderScript cardHolder in drawTake)
        {
            cardNumsinHolder += cardHolder.cards.Count;
        }

        if (automaticDeal && cardNumsinHolder > 0)
        {
            OnDrawCardsBySuit(10);
        }

        if (automaticTake && cardNumsinHolder > 0 && Time.time > lastDrawTime + drawCooldown)
        {
            lastDrawTime = Time.time;
            OnDrawRandomCard();
        }
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
        foreach (CardHolderScript cardHolder in drawTake)
        {
            drawCards(cardHolder, drawPlace[0], drawIndices);
            shuffleCards(cardHolder);
        }
    }

    public void drawCards(CardHolderScript drawTake, CardHolderScript drawPlace, List<int> drawIndices)
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

        shuffleCards(drawPlace);
    }

    // Wrapper method for button click
    public void OnDrawCardsBySuit(int count)
    {
        GenerateRandomIndices(minRandIndex, maxRandIndex, count);
        foreach (CardHolderScript cardHolderTake in drawTake)
        {
            foreach (CardHolderScript cardHolderPlace in drawPlace)
            {
                drawCards(cardHolderTake, cardHolderPlace, drawIndices);
            }
        }
    }

    public void SortCardsBySuit(CardHolderScript drawTake, List<CardHolderScript> drawPlaces, List<int> drawIndices)
    {
        // Ensure drawPlaces has exactly 4 holders
        if (drawPlaces.Count != 4)
        {
            Debug.LogError("drawPlaces must have exactly 4 elements for Hearts, Diamonds, Clubs, and Spades.");
            return;
        }

        // Sort indices in descending order to avoid shifting issues while removing items
        drawIndices.Sort((a, b) => b.CompareTo(a));

        foreach (int i in drawIndices)
        {
            if (i < drawTake.cards.Count)
            {
                // Get the card from drawTake.cards at index i
                GameObject card = drawTake.cards[i];
                CardScript cardScript = card.GetComponent<CardScript>();

                // Determine the target index based on cardSuit
                int targetIndex = -1;
                switch (cardScript.cardSuit)
                {
                    case CardScript.CardSuit.Hearts:
                        targetIndex = 0;
                        break;
                    case CardScript.CardSuit.Diamonds:
                        targetIndex = 1;
                        break;
                    case CardScript.CardSuit.Clubs:
                        targetIndex = 2;
                        break;
                    case CardScript.CardSuit.Spades:
                        targetIndex = 3;
                        break;
                }

                if (targetIndex != -1)
                {
                    CardHolderScript drawPlace = drawPlaces[targetIndex];

                    // Check if there is room in the target drawPlace
                    if (drawPlace.maxCardsHeld - drawPlace.cards.Count > 0)
                    {
                        // Add the card to drawPlace.cards
                        drawPlace.AddCard(card);

                        // Remove the card from drawTake.cards
                        drawTake.RemoveCard(card);
                    }
                }
            }
        }

        maxRandIndex = drawTake.cards.Count;
    }

    public void OnDrawRandomCard()
    {
        // Ensure drawTake has elements to avoid errors
        if (drawTake.Length == 0)
        {
            Debug.LogError("drawTake is empty. Cannot draw a random card.");
            return;
        }

        // Ensure drawPlace has at least one element
        if (drawPlace.Length == 0)
        {
            Debug.LogError("drawPlace is empty. Cannot place drawn card.");
            return;
        }

        // Select a random CardHolderScript from drawTake
        int randomIndex = Random.Range(0, drawTake.Length);

        // Move a random card from the selected drawTake to the first drawPlace
        MoveRandomCardToDrawPlace(drawTake[randomIndex], drawPlace[0]);
    }

    // New function to move a random card to the 0 index of drawPlace
    public void MoveRandomCardToDrawPlace(CardHolderScript drawTake, CardHolderScript drawPlace)
    {
        if (drawTake.cards.Count == 0)
        {
            Debug.LogError("No cards to draw from.");
            return;
        }

        if (drawPlace.cards.Count >= drawPlace.maxCardsHeld)
        {
            Debug.LogError("No space in drawPlace to add a new card.");
            return;
        }

        // Select a random index from drawTake
        int randomIndex = Random.Range(0, drawTake.cards.Count);

        // Get the card at the random index
        GameObject card = drawTake.cards[randomIndex];

        // Remove the card from drawTake
        drawTake.RemoveCard(card);

        // Insert the card at the 0 index of drawPlace
        drawPlace.cards.Insert(0, card);

        // Adjust the card's parent to match drawPlace for UI purposes
        card.transform.SetParent(drawPlace.transform, false);
        card.transform.SetSiblingIndex(0); // Place it at the top of the hierarchy

        // Log the move for debugging purposes
        Debug.Log($"Moved card {card.name} from drawTake to index 0 of drawPlace.");
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
