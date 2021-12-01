using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DialogCard", menuName = "DialogCard", order = 1)]
public class DialogCard : ScriptableObject
{
    public string customerString;

    public DialogCardAnwser[] anwsers;


}
