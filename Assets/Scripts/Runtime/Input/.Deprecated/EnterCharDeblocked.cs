using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KW.Core;
using KW.Gameplay;
using KW.UI;
using KW.Managers;

namespace KW.Inputs
{
    [System.Obsolete("This class is not used anymore. Use KoreanInputHandler instead.", true)]
    public class EnterCharDeblocked : EnterCharacter
    {
        // CharDeblockedStage charEnteredForDeblocked;
        // CellManagerDeblocked cellManager;
        // void Start()
        // {
        //     charEnteredForDeblocked = GamePlayManager.instance.GetComponent<CharDeblockedStage>();
        //     cellManager = GamePlayManager.instance.GetComponent<CellManagerDeblocked>();
        //     //string answer = System.Convert.ToString(inputValue.codedAnswer / 2147483647);
        //     //print("decoded answer : " + answer);
        // }
        // public override void EnterChar()
        // {
        //     string[] firstLetters = charEnteredForDeblocked.firstLetters;
        //     string[] middleLetters = charEnteredForDeblocked.middleLetters;
        //     string[] lastLetters = charEnteredForDeblocked.lastLetters;

        //     bool[] bIsEntered = charEnteredForDeblocked.bIsEntered;

        //     int wordLen = charEnteredForDeblocked.wordLen;
        //     int curCell = cellManager.curCellPos;
        //     enteredChar = this.name;
        //     int intWord = System.Convert.ToInt32(System.Convert.ToChar(enteredChar));

        //     bool bOnlyEnterFirst = false;
        //     if (curCell >= wordLen || curCell < 0)
        //         return;
        //     if (!bIsEntered[curCell])
        //     {
        //         if (0x3131 <= intWord && intWord <= 0x314E)
        //         {

        //         }
        //     }
        // }

        // public override void Backspace() { }

        // public override void EnterWord() { }

        // public override void CapsWord()
        // {
        //     if (!cellManager.bIsCaps)
        //     {
        //         for (int i = 0; i < 10; i++)
        //         {
        //             cellManager.topBoards[i].GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 8000);
        //             cellManager.capsTopBoards[i].GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -8000);
        //         }
        //         cellManager.bIsCaps = true;
        //     }
        //     else
        //     {
        //         for (int i = 0; i < 10; i++)
        //         {
        //             cellManager.topBoards[i].GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -8000);
        //             cellManager.capsTopBoards[i].GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 8000);
        //         }
        //         cellManager.bIsCaps = false;
        //     }
        // }
    }
}