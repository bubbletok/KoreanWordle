using System;
using System.Collections.Generic;
using UnityEngine;
using KW.Core;
using static KW.Core.Settings.GameplayEnums;

namespace KW.Gameplay
{
    [System.Obsolete("This class is not used anymore. Use GameWordSelector instead.", true)]
    public class CharBlockedStage : MonoBehaviour
    {
        //public Text text;
        public string[] firstLetters = { "ㄱ", "ㄲ", "ㄴ", "ㄷ", "ㄸ", "ㄹ", "ㅁ", "ㅂ", "ㅃ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅉ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ" };
        public bool[] isFirst, isMiddle, isLast; //초성 중성 종성 입력 체크
        //public bool isCaps;
        public int[] first_idx, mid_idx, last_idx; //초성 중성 종성 순서
        public int[] firstLetterDiff = new int[] { 0, 1, 0, 2, 0, 0, 3, 4, 5, 0, 0, 0, 0, 0, 0, 0, 6, 7, 8, 0, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
        public int[] lastLetterDiff = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 0, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 0, 18, 19, 20, 21, 22, 0, 23, 24, 25, 26, 27 };
        public int wordLen = 3;
        public int maxWordLen = 5;
        public int wordPart;
        public int curIndex;
        public List<int> answerWord = new List<int>();
        public GameObject completedTodayWord;

        [SerializeField] private GameStageType gameStageType;
        string originalAnswer = " ";

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
        //             GameManager.Instance.GameStageType = (GameStageType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(GameStageType)).Length);
        //             originalAnswer = GetWord();
        //         }
        //     }
        //     //print("answer : " + originalAnswer);
        //     byte[] answerBytes = System.Text.Encoding.UTF8.GetBytes(originalAnswer);
        //     for (int i = 0; i < answerBytes.Length; i++)
        //     {
        //         answerWord.Add(System.Convert.ToInt32(answerBytes[i]) + 214743673);
        //     }
        //     isFirst = new bool[maxWordLen];
        //     isMiddle = new bool[maxWordLen];
        //     isLast = new bool[maxWordLen];

        //     first_idx = new int[maxWordLen];
        //     mid_idx = new int[maxWordLen];
        //     last_idx = new int[maxWordLen];

        //     for (int i = 0; i < maxWordLen; i++)
        //     {
        //         isFirst[i] = false;
        //         isMiddle[i] = false;
        //         isLast[i] = false;

        //         first_idx[i] = 0;
        //         mid_idx[i] = 0;
        //         last_idx[i] = 0;
        //     }
        //     /*
        //     isFirst = new bool[5];
        //     isMiddle = new bool[5];
        //     isLast = new bool[5];

        //     first_idx = new int[5];
        //     mid_idx = new int[5];
        //     last_idx = new int[5];
        //     */

        //     //text.text = originalAnswer;

        // }
        // public string GetWord()
        // {
        //     gameStageType = GameManager.Instance.GameStageType;
        //     List<string> wordlist = new List<string>();
        //     //wordlist = GameManager.Instance.defaultList;
        //     switch (gameStageType)
        //     {
        //         case GameStageType.Noun:
        //             wordlist = GameManager.Instance.NounList;
        //             break;
        //         case GameStageType.Pronoun:
        //             wordlist = GameManager.Instance.PronounList;

        //             break;
        //         case GameStageType.Numeral:
        //             wordlist = GameManager.Instance.NumerList;

        //             break;
        //         case GameStageType.Verb:
        //             wordlist = GameManager.Instance.VerbList;

        //             break;
        //         case GameStageType.Adjective:
        //             wordlist = GameManager.Instance.AdjList;

        //             break;
        //         case GameStageType.Adverb:
        //             wordlist = GameManager.Instance.AdvList;
        //             break;
        //         default:
        //             wordlist = GameManager.Instance.defaultList;
        //             break;
        //     }
        //     /*        else if (gameType == 4)
        //             {
        //                 wordlist = GameManager.Instance.DeterList;
        //             }
        //             else if (gameType == 5)
        //             {
        //                 wordlist = GameManager.Instance.InterList;
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
        //     if (GameManager.Instance.data.clearWordIndices[(GameType.Blocked, gameStageType - 1, idx)]) return emptyWord;
        //     word = wordlist[idx];
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