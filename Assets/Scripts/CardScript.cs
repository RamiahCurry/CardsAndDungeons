using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CardScript : MonoBehaviour,
    IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int cardValue = 0;

    public enum CardSuit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    public CardSuit cardSuit;

    private Text valueText;

    [HideInInspector]
    public bool isDragging;

    // Define UnityEvents for drag start and end
    public static UnityEvent<CardScript> BeginDragEvent = new UnityEvent<CardScript>();
    public static UnityEvent<CardScript> EndDragEvent = new UnityEvent<CardScript>();

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 dragOffset;

    private GameObject cardHolder;
    [HideInInspector]
    public Transform parentAfterDrag;

    // Prefab code
    public GameObject prefabToInstantiate; // The prefab to instantiate and follow the first object
    private GameObject instantiatedPrefab; // Reference to the instantiated prefab
    private VisualCardScript visualCardScript;
    private Canvas instantiatedCanvas; // Reference to the Canvas component of the instantiated prefab
    private int initialSortingOrder; // Initial sorting order value

    private RectTransform canvasRect; // Reference to the canvas RectTransform

    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        // Get the RectTransform of the canvas
        canvasRect = canvas.GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
                Debug.LogError("GameManager not found in the scene!");
        }

        cardHolder = transform.parent.parent.gameObject;

        // Prefab code
        // Calculate the position for instantiation with the z offset
        Vector3 spawnPosition = transform.position + new Vector3(0f, 0f, -1f);
        // Instantiate the prefab
        instantiatedPrefab = Instantiate(prefabToInstantiate, spawnPosition, Quaternion.identity, canvasRect);
        // Get the VisualCardScript component of the instantiated prefab
        visualCardScript = instantiatedPrefab.GetComponent<VisualCardScript>();

        // Set the instantiating object of the prefab
        visualCardScript.SetInstantiatingObject(this.transform);

        // Ensure the instantiated prefab is always on top
        instantiatedCanvas = instantiatedPrefab.GetComponent<Canvas>();
        if (instantiatedCanvas == null)
        {
            instantiatedCanvas = instantiatedPrefab.AddComponent<Canvas>();
        }

        instantiatedCanvas.overrideSorting = true;
        instantiatedCanvas.sortingOrder = 2;
        initialSortingOrder = instantiatedCanvas.sortingOrder;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out localPoint);
            rectTransform.localPosition = new Vector3(localPoint.x - dragOffset.x, localPoint.y - dragOffset.y, rectTransform.localPosition.z);

            // Adjust sorting order to keep it on top
            if (instantiatedCanvas != null)
            {
                instantiatedCanvas.sortingOrder = 10; // Ensure it's always on top
            }
            visualCardScript.idleRotate3d = false;
        }
        else
        {
            // Restore the sorting order to its initial value
            instantiatedCanvas.sortingOrder = initialSortingOrder;

            visualCardScript.idleRotate3d = true;
        }

        if(valueText != null)
        {
            valueText.text = "Value: " + cardValue;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        visualCardScript.hoverRotate3d = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        visualCardScript.hoverRotate3d = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Calculate the offset between the card's position and the mouse position
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out localPoint);
        dragOffset = localPoint - (Vector2)rectTransform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // This method is called when the user begins dragging the UI element
        BeginDragEvent.Invoke(this);
        isDragging = true;

        // Optional: Make the card semi-transparent while dragging
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;

        cardHolder.GetComponent<CardHolderScript>().BeginDrag(this);
        cardHolder.GetComponent<CardHolderScript>().updateCardList();

        // Call the shake method on the instantiated prefab
        visualCardScript.StartShake();
        visualCardScript.StartScaleUp();

        // gameManager.mouseSelectObject = instantiatedPrefab;

        parentAfterDrag = transform.parent.parent;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out localPoint);
            rectTransform.localPosition = new Vector3(localPoint.x - dragOffset.x, localPoint.y - dragOffset.y, rectTransform.localPosition.z);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragEvent.Invoke(this);
        isDragging = false;

        // Restore the card's transparency
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // Set the card's local position to (0, 0)
        rectTransform.localPosition = Vector3.zero;

        cardHolder.GetComponent<CardHolderScript>().EndDrag(this);
        cardHolder.GetComponent<CardHolderScript>().updateCardList();

        // Stop shaking and scale down the visual card
        visualCardScript.StartScaleDown();

        // gameManager.mouseSelectObject = null;

        transform.parent.SetParent(parentAfterDrag);
    }
}
