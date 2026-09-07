using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KW.Core;
using KW.Managers;

namespace KW.UI
{

    public class UserStats : MonoBehaviour
    {
        // public RectTransform[] tryBars;
        // void Start()
        // {
        //     //tryBars = new RectTransform[6];
        //     int gameType = GameManager.Instance.GameStageType - 1;
        //     int total = 0;
        //     for (int i = 0; i < 6; i++)
        //         total += GameManager.Instance.data.attemptToClear[gameType, i];
        //     if (total == 0)
        //         return;
        //     else
        //     {
        //         for (int i = 0; i < tryBars.Length; i++)
        //         {
        //             int attempNum = GameManager.Instance.data.attemptToClear[gameType, i];
        //             if (attempNum != 0)
        //             {
        //                 tryBars[i].offsetMin = new Vector2(0, tryBars[i].offsetMin.y);
        //                 tryBars[i].offsetMax = new Vector2(-(150 - ((300 * attempNum) / total)), tryBars[i].offsetMax.y);
        //             }
        //         }
        //     }
        // }
    }

}