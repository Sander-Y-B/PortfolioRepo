using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTree : MonoBehaviour
{
    // !! Each array needs to be the same length as the others !!
    [SerializeField] DialogCard[] cards_neutral, cards_positive, cards_negative;
    public DialogCard[,] allCards;
    [HideInInspector]public int treeLength;

    // Here it puts the 3 arrays in 1 2D array. I've done this so you can easily assing all the cards in the inspector whilst also accessing it easily in code
    private void Awake()
    {
        treeLength = cards_neutral.Length;
        allCards = new DialogCard[treeLength, 3];

        for (int i = 0; i < cards_neutral.Length; i++)
        {
            allCards[i, 0] = cards_neutral[i];
            allCards[i, 1] = cards_positive[i];
            allCards[i, 2] = cards_negative[i];
        }
    }
}
