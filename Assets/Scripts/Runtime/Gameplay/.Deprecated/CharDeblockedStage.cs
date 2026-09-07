using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KW.Core;

namespace KW.Gameplay
{
    [System.Obsolete("This class is not used anymore. Use GameWordSelector instead.", true)]
    public class CharDeblockedStage : MonoBehaviour
    {
        [SerializeField] int gameType;
        string originalAnswer = " ";

        public string[] firstLetters = { "ㄱ", "ㄲ", "ㄴ", "ㄷ", "ㄸ", "ㄹ", "ㅁ", "ㅂ", "ㅃ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅉ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ" };
        public string[] middleLetters = { "ㅏ", "ㅐ", "ㅑ", "ㅒ", "ㅓ", "ㅔ", "ㅕ", "ㅖ", "ㅗ", "ㅘ", "ㅙ", "ㅚ", "ㅛ", "ㅜ", "ㅝ", "ㅞ", "ㅟ", "ㅠ", "ㅡ", "ㅢ", "ㅣ" };
        public string[] lastLetters = { "ㄱ", "ㄲ", "ㄳ", "ㄴ", "ㄵ", "ㄶ", "ㄷ", "ㄸ", "ㄹ", "ㄺ", "ㄻ", "ㄼ", "ㄽ", "ㄾ", "ㄿ", "ㅀ", "ㅁ", "ㅂ", "ㅄ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ" };

        public bool[] bIsEntered;

        public int wordLen = 3;
        public int maxWordLen = 5;
        public int wordPart;
        public int curIndex;

        // private void Awake()
        // {
        //     //GameManager.Instance.isAbleTodayWord();
        //     //print(GameManager.Instance.isTodayCompleted);
        //     /*        if (GameManager.Instance.gameType == 6 && GameManager.Instance.isTodayCompleted)
        //             {
        //                 GameSetting.instance.SetWindow(completedTodayWord);
        //                 return;
        //             }*/
        //     if (GameManager.Instance != null)
        //     {
        //         while (originalAnswer == " ")
        //         {
        //             GameManager.Instance.GameStageType = UnityEngine.Random.Range(1, 6);
        //             originalAnswer = GetWord();
        //         }
        //     }
        //     bIsEntered = new bool[maxWordLen];
        //     for (int i = 0; i < maxWordLen; i++)
        //     {
        //         bIsEntered[i] = false;
        //     }
        // }
        // public string GetWord()
        // {
        //     gameType = GameManager.Instance.GameStageType;
        //     List<string> wordlist = new List<string>();
        //     //wordlist = GameManager.Instance.defaultList;
        //     if (gameType == 1)
        //     {
        //         wordlist = GameManager.Instance.NounList;
        //     }
        //     else if (gameType == 2)
        //     {
        //         wordlist = GameManager.Instance.PronounList;
        //     }
        //     else if (gameType == 3)
        //     {
        //         wordlist = GameManager.Instance.NumerList;
        //     }
        //     /*        else if (gameType == 4)
        //             {
        //                 wordlist = GameManager.Instance.DeterList;
        //             }
        //             else if (gameType == 5)
        //             {
        //                 wordlist = GameManager.Instance.InterList;
        //             }*/
        //     else if (gameType == 4)
        //     {
        //         wordlist = GameManager.Instance.VerbList;
        //     }
        //     else if (gameType == 5)
        //     {
        //         wordlist = GameManager.Instance.AdjList;
        //     }
        //     else if (gameType == 6)
        //     {
        //         wordlist = GameManager.Instance.AdvList;
        //     }
        //     /*        else
        //             {
        //                 wordlist = GameManager.Instance.defaultList;
        //             }*/

        //     int maxNumber = GameManager.Instance.CurrentStage < wordlist.Count - 1 ? GameManager.Instance.CurrentStage : wordlist.Count - 1;
        //     string word = " ";
        //     string emptyWord = " ";
        //     int idx = UnityEngine.Random.Range(0, maxNumber);

        //     /*        print("GameType: " + gameType);
        //             print("Stage: " + GameManager.Instance.stage);
        //             print("Wordlist: " + wordlist.Count);
        //             print("MaxNumer: " + maxNumber);
        //             print("Index: " + idx);*/

        //     if (wordlist[idx] == null) return emptyWord;
        //     if (GameManager.Instance.data.clearBlockedWordIndex[gameType - 1, idx]) return emptyWord;

        //     word = wordlist[idx];
        //     //print("Word: " + word);
        //     wordLen = word.Length;
        //     curIndex = idx;
        //     /*        for (int i=0; i<maxNumber; i++)
        //             {
        //     *//*            if (GameManager.Instance.data.clearBlockedWordIndex[gameType - 1, i])
        //                     continue;*//*
        //                 //print(maxNumber + " / " + randN);
        //                 *//*        if (GameManager.Instance.data.completeWordList.Contains(wordlist[randN]))
        //                 word = " ";*//*
        //                 if (wordlist[i] != null)
        //                     word = wordlist[i];
        //                 wordLen = word.Length;
        //                 curIndex = i;
        //                 break;
        //             }*/
        //     //print("GameType: " + gameType);
        //     //print("Stgae: " + GameManager.Instance.stage);
        //     //print("Word: " + word);
        //     return word;
        // }

        // public string GetAnswer()
        // {
        //     return originalAnswer;
        // }
    }

}