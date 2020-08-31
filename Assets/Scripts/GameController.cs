using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private const int SPRITES_NUM = 4;

    [SerializeField]
    private GameObject card;

    [SerializeField]
    private Transform field;

    [SerializeField]
    private List<Button> cardsList;

    [SerializeField]
    private Sprite background;

    [SerializeField]
    private Sprite[] pictures;

    [SerializeField]
    private Sprite[] puzzleCards;

    [SerializeField]
    List<Sprite> chosenSprites;

    [SerializeField]
    private bool firstClick, secondClick;

    [SerializeField]
    private string firstName, secondName;

    private int firstId, secondId;

    [SerializeField]
    public int correctNum = 0;

    void Awake()
    {
        cardsList = new List<Button>();
        chosenSprites = new List<Sprite>();
        puzzleCards = new Sprite[2 * SPRITES_NUM];
        AttachedCardToField();

        getPictures();
        setCardSprite();
    }

    void Start()
    {
        firstClick = true;
        secondClick = false;
        MakeListOfButton();

    }

    private void AttachedCardToField()
    {
        for (int i = 0; i < 2 * SPRITES_NUM; i++)
        {
            var obj = Instantiate(card);
            obj.name = $"{i}";
            obj.transform.SetParent(field, false);
        }
    }

    private void getPictures()
    {
        pictures = Resources.LoadAll<Sprite>("Sprites/Candy");
        while (chosenSprites.Count < SPRITES_NUM)
        {
            var rand = Random.Range(0, 15);
            if (!chosenSprites.Contains(pictures[rand]))
            {
                chosenSprites.Add(pictures[rand]);
            }
        }
    }

    void setCardSprite()
    {
        for (int i = 0; i < chosenSprites.Count; i++)
        {
            puzzleCards[i] = chosenSprites[i];
            puzzleCards[i + SPRITES_NUM] = chosenSprites[i];
        }
    }

    private void MakeListOfButton()
    {
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("Card");

        for (int i = 0; i < tmp.Length; i++)
        {
            cardsList.Add(tmp[i].GetComponent<Button>());
            cardsList[i].image.sprite = background;
            cardsList[i].onClick.AddListener(() => clickOnCard());
        }
    }

    void clickOnCard()
    {
        if (firstClick)
        {
            string name = EventSystem.current.currentSelectedGameObject.name.Trim();

            int id = int.Parse(name);
            firstId = id;
            cardsList[id].image.sprite = puzzleCards[id];
            firstName = puzzleCards[id].name;

            secondClick = true;
            firstClick = false;
        }
        else if (secondClick)
        {
            string name = EventSystem.current.currentSelectedGameObject.name.Trim();
            int id = int.Parse(name);
            secondId = id;
            cardsList[id].image.sprite = puzzleCards[id];
            secondName = puzzleCards[id].name;

            secondClick = false;

            if (secondName == firstName)
            {
                correctNum++;
                cardsList[firstId].onClick.RemoveAllListeners();
                cardsList[secondId].onClick.RemoveAllListeners();
                firstClick = true;
            }
            else
            {
                firstClick = false;
                StartCoroutine(wait());
            }
        }
    }

    private void backToDefaults(int firstId, int secondId)
    {
        cardsList[firstId].image.sprite = background;
        cardsList[secondId].image.sprite = background;
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.5f);
        backToDefaults(firstId, secondId);
        firstClick = true;
    }


    void Update()
    {
        if (correctNum == SPRITES_NUM)
        {
            Debug.Log("You Can Win ....");
            firstClick = secondClick = false;
            Destroy(gameObject);
        }
    }
}
