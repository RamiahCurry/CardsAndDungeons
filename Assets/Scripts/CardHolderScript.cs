using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardHolderScript : MonoBehaviour, IDropHandler
{
    public CardScript selectedCard;
    public bool canDrop = true;
    public bool discardWhenDrop = false;
    GameObject selectedCardObject;
    CardScript hoveredCard;

    public int maxCardsHeld;
    public List <GameObject> cards;

    public GameObject tempCard;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (selectedCard != null)
        {
            selectedCardObject = selectedCard.gameObject;
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            RemoveCard(selectedCardObject);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddCard(tempCard);
        }
        if (discardWhenDrop)
        {
            // Create a copy of the cards list to iterate over
            List<GameObject> cardsCopy = new List<GameObject>(cards);
            foreach (GameObject card in cardsCopy)
            {
                RemoveCard(card);
            }
        }

        updateCardList();
    }

    public void updateCardList()
    {
        cards.Clear();
        foreach (Transform child in transform)
        {
            if (child.tag == "CardSlot")
            {
                cards.Add(child.gameObject);
            }
        }
    }

    public void BeginDrag(CardScript card)
    {
        selectedCard = card;
        // Debug.Log("Selected card is:" + selectedCard.name);
    }

    public void EndDrag(CardScript card)
    {
        selectedCard = null;
    }

    public void CardPointEnter(CardScript card)
    {
        hoveredCard = card;
    }

    public void CardPointExit(CardScript card)
    {
        hoveredCard = null;
    }

    public void AddCard(GameObject cardSlot)
    {
        if (cardSlot != null)
        {
            Instantiate(cardSlot, this.gameObject.transform);
            cards.Add(cardSlot);
            Debug.Log("Added Card");
        }
    }

    public void RemoveCard(GameObject cardSlot)
    {
        if (cardSlot != null)
        {
            cards.Remove(cardSlot);
            Destroy(cardSlot);
            Debug.Log("Deleted Card");
        }
        /// Debug.Log("Delete button pressed");
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (canDrop && maxCardsHeld > cards.Count)
        {
            GameObject dropped = eventData.pointerDrag;
            CardScript cardScript = dropped.GetComponent<CardScript>();
            cardScript.parentAfterDrag = transform;
        }
    }
}
