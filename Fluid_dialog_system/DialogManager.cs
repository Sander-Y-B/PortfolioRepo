using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [HideInInspector]
    public DialogTree currentDialogTree;
    private int cardIndex;

    public TMP_Text customerText;
    public TMP_Text[] anwserTexts;
    public GameObject convoUi;

    public enum Moods { neutral, positive, negative }
    public Moods curMood;
    public int moodValue = 15;
    int tempMoodValue = 0;
    //0-9 negative| 10-19 neutral| 20-29 positive|

    void LoadNextDialogCard(int nextCardIndex)
    {
        PickMood();

        if (nextCardIndex >= currentDialogTree.treeLength)
        {
            EndDialog();
        }
        else
        {
            customerText.text = currentDialogTree.allCards[nextCardIndex, (int)curMood].customerString;
            for (int i = 0; i < anwserTexts.Length; i++)
            {
                anwserTexts[i].text = currentDialogTree.allCards[nextCardIndex, (int)curMood].anwsers[i].anwserString;
            }
            cardIndex = nextCardIndex;
        }
    }

    public void StartDialog()
    {
        convoUi.SetActive(true);
        LoadNextDialogCard(0);
    }

    void EndDialog()
    {
        convoUi.SetActive(false);
        Manager.self.customerDeletePoint.MoveAwayCurCustomer();
    }

    // Gets called by the awnser UI buttons 
    public void AnwserClicked(int buttonAnwserNumber)
    {
        DialogCardAnwser curAwnser = currentDialogTree.allCards[cardIndex, (int)curMood].anwsers[buttonAnwserNumber];
        Manager.self.scoreManager.curScore += curAwnser.scorePoints;
        LoadNextDialogCard(curAwnser.nextDialogIndex);
        moodValue += curAwnser.moodMod;
    }

    void PickMood()
    {
        // I use 2 random ranges here instead of 1 so that it is more likely to fall in the middle of -20 , 20 instead of having a 1/20 chance for each number
        tempMoodValue = moodValue + Random.Range(-10, 11) + Random.Range(-10, 11); 
        curMood = tempMoodValue < 10 ? Moods.negative : tempMoodValue < 20 ? Moods.neutral : Moods.positive;
    }
}
