using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogCardAnwser
{
    public string anwserString;   // This is what the customer says
    public int nextDialogIndex;  // If this is a higher number then the tree lenght it will end the dialog
    public float scorePoints;
    public int moodMod;
}
