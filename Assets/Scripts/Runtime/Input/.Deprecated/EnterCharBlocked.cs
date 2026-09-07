using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KW.Core;
using KW.UI;
using KW.Managers;
using KW.Gameplay;

namespace KW.Inputs
{
    [System.Obsolete("This class is not used anymore. Use KoreanInputHandler instead.", true)]
    public class EnterCharBlocked : EnterCharacter
    {
        // CharBlockedStage charEnteredForBlocked;
        // CellManagerBlocked cellManager;
        // void Start()
        // {
        //     charEnteredForBlocked = GamePlayManager.Instance.GetComponent<CharBlockedStage>();
        //     cellManager = GamePlayManager.Instance.GetComponent<CellManagerBlocked>();
        //     //string answer = System.Convert.ToString(inputValue.codedAnswer / 2147483647);
        //     //print("decoded answer : " + answer);
        // }

        // public override void SetCellText(bool bOnlyFirst, int curCell)
        // {
        //     string[] firstLetters = charEnteredForBlocked.firstLetters;
        //     bool[] isFirst = charEnteredForBlocked.isFirst, isMiddle = charEnteredForBlocked.isMiddle, isLast = charEnteredForBlocked.isLast;
        //     int[] first_idx = charEnteredForBlocked.first_idx, mid_idx = charEnteredForBlocked.mid_idx, last_idx = charEnteredForBlocked.last_idx;
        //     int[] firstLetterDiff = charEnteredForBlocked.firstLetterDiff, lastLetterDiff = charEnteredForBlocked.lastLetterDiff;
        //     if (bOnlyFirst)
        //     {
        //         cellManager.SetText(firstLetters[first_idx[curCell]]);
        //     }
        //     else
        //     {
        //         cellManager.SetText(System.Convert.ToChar((0xAC00 + 28 * 21 * first_idx[curCell] + 28 * mid_idx[curCell] + lastLetterDiff[last_idx[curCell]])).ToString());
        //     }
        // }

        // public override void EnterChar()
        // {
        //     if (GamePlayManager.Instance.isEnd) return;
        //     /*
        //     charEnteredForDeblocked = GameSetting.Instance.GetComponent<CharBlockedStage>();
        //     cellManager = GameSetting.Instance.GetComponent<CellManagerBlocked>();
        //     GameSetting gameManager = GameSetting.Instance;*/
        //     string[] firstLetters = charEnteredForBlocked.firstLetters;
        //     bool[] isFirst = charEnteredForBlocked.isFirst, isMiddle = charEnteredForBlocked.isMiddle, isLast = charEnteredForBlocked.isLast;
        //     int[] first_idx = charEnteredForBlocked.first_idx, mid_idx = charEnteredForBlocked.mid_idx, last_idx = charEnteredForBlocked.last_idx;
        //     int[] firstLetterDiff = charEnteredForBlocked.firstLetterDiff, lastLetterDiff = charEnteredForBlocked.lastLetterDiff;
        //     int wordLen = charEnteredForBlocked.wordLen;
        //     int curCell = cellManager.curCellPos;
        //     enteredChar = this.name;
        //     int intWord = System.Convert.ToInt32(System.Convert.ToChar(enteredChar));

        //     bool bOnlyEnterFirst = false;
        //     if (GamePlayManager.Instance.isEnd) return;
        //     if (curCell >= wordLen)
        //     {
        //         curCell = wordLen - 1;
        //         return;
        //     }
        //     if (curCell < 0)
        //     {
        //         curCell = 0;
        //         return;
        //     }
        //     if (!isFirst[curCell] && !isMiddle[curCell] && !isLast[curCell] && (0x3131 <= intWord && intWord <= 0x314E))
        //     {
        //         first_idx[curCell] = firstLetterDiff[intWord - 0x3131];
        //         isFirst[curCell] = true;
        //         bOnlyEnterFirst = true;
        //         SetCellText(bOnlyEnterFirst, curCell);
        //     }
        //     //초성 있을 시
        //     else if (isFirst[curCell])
        //     {
        //         //중성 있을시
        //         if (isMiddle[curCell])
        //         {
        //             //종성 있을 시
        //             if (isLast[curCell])
        //             {
        //                 //모음 입력했을 때
        //                 if (0x314F <= intWord && intWord <= 0x3163)
        //                 {
        //                     //곁받침이면 곁받침 중 하나 지우고 지운 거랑 모음 합친거 다음 셀에 입력
        //                     if (last_idx[curCell] == 3) // ㄳ
        //                     {
        //                         last_idx[curCell] = 1;
        //                         SetCellText(false, curCell);
        //                         //cellManager.SetText(System.Convert.ToChar((0xAC00 + 28 * 21 * first_idx[curCell] + 28 * mid_idx[curCell] + lastLetterDiff[last_idx[curCell]])).ToString());
        //                         curCell = ++cellManager.curCellPos;
        //                         first_idx[curCell] = 9;
        //                         isFirst[curCell] = true;
        //                         isMiddle[curCell] = true;
        //                     }
        //                     else if (last_idx[curCell] == 5 || last_idx[curCell] == 6) //ㄵ, ㄶ
        //                     {
        //                         if (last_idx[curCell] == 5) // ㄵ
        //                             first_idx[curCell + 1] = 12; // ㅈ
        //                         else // ㄶ
        //                             first_idx[curCell + 1] = 18; //ㅎ
        //                         last_idx[curCell] = 4;

        //                         SetCellText(false, curCell);
        //                         //cellManager.SetText(System.Convert.ToChar((0xAC00 + 28 * 21 * first_idx[curCell] + 28 * mid_idx[curCell] + lastLetterDiff[last_idx[curCell]])).ToString());

        //                         curCell = ++cellManager.curCellPos;
        //                         isFirst[curCell] = true;
        //                         isMiddle[curCell] = true;
        //                     }
        //                     else if (10 <= last_idx[curCell] && last_idx[curCell] <= 16) //ㄺ, ㄻ, ㄼ, ㄽ, ㄾ, ㄿ, ㅀ
        //                     {
        //                         if (last_idx[curCell] == 10)
        //                             first_idx[curCell + 1] = 0;
        //                         else if (last_idx[curCell] == 11)
        //                             first_idx[curCell + 1] = 6;
        //                         else if (last_idx[curCell] == 12)
        //                             first_idx[curCell + 1] = 7;
        //                         else if (last_idx[curCell] == 13)
        //                             first_idx[curCell + 1] = 9;
        //                         else if (last_idx[curCell] == 14)
        //                             first_idx[curCell + 1] = 16;
        //                         else if (last_idx[curCell] == 15)
        //                             first_idx[curCell + 1] = 17;
        //                         else
        //                             first_idx[curCell + 1] = 18;

        //                         last_idx[curCell] = 9; //곁받침에서 홑받침으로 바꾸기
        //                         cellManager.SetText(System.Convert.ToChar((0xAC00 + 28 * 21 * first_idx[curCell] + 28 * mid_idx[curCell] + lastLetterDiff[last_idx[curCell]])).ToString());

        //                         curCell = ++cellManager.curCellPos;
        //                         isFirst[curCell] = true;
        //                         isMiddle[curCell] = true;
        //                     }
        //                     else if (last_idx[curCell] == 20) // ㅄ
        //                     {
        //                         last_idx[curCell] = 18;
        //                         SetCellText(false, curCell);
        //                         //cellManager.SetText(System.Convert.ToChar((0xAC00 + 28 * 21 * first_idx[curCell] + 28 * mid_idx[curCell] + lastLetterDiff[last_idx[curCell]])).ToString());
        //                         curCell = ++cellManager.curCellPos;
        //                         first_idx[curCell] = 9;
        //                         mid_idx[curCell] = intWord - 0x314F;
        //                         isFirst[curCell] = true;
        //                         isMiddle[curCell] = true;
        //                     }
        //                     else //곁받침 아닌 경우 종성이랑 모음 합친거 다음 셀에 입력
        //                     {
        //                         first_idx[curCell + 1] = firstLetterDiff[last_idx[curCell] - 1];
        //                         last_idx[curCell] = 0;
        //                         isLast[curCell] = false;
        //                         SetCellText(false, curCell);
        //                         //cellManager.SetText(System.Convert.ToChar((0xAC00 + 28 * 21 * first_idx[curCell] + 28 * mid_idx[curCell] + lastLetterDiff[last_idx[curCell]])).ToString());
        //                         curCell = ++cellManager.curCellPos;
        //                         isFirst[curCell] = true;
        //                         isMiddle[curCell] = true;
        //                     }
        //                     mid_idx[curCell] = intWord - 0x314F;
        //                 }

        //                 //곁받침 가능한 경우에
        //                 else if (enteredChar == "ㄱ" && last_idx[curCell] == 9) //ㄹ에 ㄱ
        //                     last_idx[curCell] = 10;
        //                 else if (enteredChar == "ㅁ" && last_idx[curCell] == 9) //ㄹ에 ㅁ
        //                     last_idx[curCell] = 11;
        //                 else if (enteredChar == "ㅂ" && last_idx[curCell] == 9) //ㄹ에 ㅂ
        //                     last_idx[curCell] = 12;
        //                 else if (enteredChar == "ㅅ" && last_idx[curCell] == 1) //ㄱ에 ㅅ
        //                     last_idx[curCell] = 3;
        //                 else if (enteredChar == "ㅅ" && last_idx[curCell] == 9) //ㄹ에 ㅅ
        //                     last_idx[curCell] = 13;
        //                 else if (enteredChar == "ㅅ" && last_idx[curCell] == 18) //ㅂ에 ㅅ
        //                     last_idx[curCell] = 20;
        //                 else if (enteredChar == "ㅈ" && last_idx[curCell] == 4) //ㄴ에 ㅈ
        //                     last_idx[curCell] = 5;
        //                 else if (enteredChar == "ㅌ" && last_idx[curCell] == 9) //ㄹ에 ㅌ
        //                     last_idx[curCell] = 14;
        //                 else if (enteredChar == "ㅍ" && last_idx[curCell] == 9) //ㄹ에 ㅍ
        //                     last_idx[curCell] = 15;
        //                 else if (enteredChar == "ㅎ" && last_idx[curCell] == 4) //ㄴ에 ㅎ
        //                     last_idx[curCell] = 6;
        //                 else if (enteredChar == "ㅎ" && last_idx[curCell] == 9) //ㄹ에 ㅎ
        //                     last_idx[curCell] = 16;

        //                 //모음도 아니고 곁받침 자음도 아니고 그냥 자음일 때
        //                 else if (0x3131 <= intWord && intWord <= 0x314E)
        //                 {
        //                     //다음 셀 이동 및 EnterCharacter 다시 실행
        //                     if (cellManager.curCellPos < wordLen - 1)
        //                     {
        //                         cellManager.curCellPos++;
        //                         EnterChar();
        //                         return;
        //                     }
        //                 }
        //             }
        //             //종성 없는데 모음 입력이면
        //             else if (0x314F <= intWord && intWord <= 0x3163)
        //             {
        //                 // ㅗ이고 ㅘ,ㅙ,ㅚ 가능한 경우
        //                 if (mid_idx[curCell] == 8)
        //                 {
        //                     if (enteredChar == "ㅏ")
        //                         mid_idx[curCell] = 9;
        //                     else if (enteredChar == "ㅐ")
        //                         mid_idx[curCell] = 10;
        //                     else if (enteredChar == "ㅣ")
        //                         mid_idx[curCell] = 11;
        //                 }
        //                 //ㅜ 이고 ㅝ,ㅞ,ㅟ 가능한 경우
        //                 else if (mid_idx[curCell] == 13)
        //                 {
        //                     if (enteredChar == "ㅓ")
        //                         mid_idx[curCell] = 14;
        //                     else if (enteredChar == "ㅔ")
        //                         mid_idx[curCell] = 15;
        //                     else if (enteredChar == "ㅣ")
        //                         mid_idx[curCell] = 16;
        //                 }
        //                 //ㅡ 이고 ㅢ 가능한 경우
        //                 else if (mid_idx[curCell] == 18)
        //                 {
        //                     if (enteredChar == "ㅣ")
        //                         mid_idx[curCell] = 19;
        //                 }
        //             }
        //             //종성 없을 시 자음 입력이면
        //             else if (0x3131 <= intWord && intWord <= 0x314E)
        //             {
        //                 // ㄸ,ㅃ,ㅉ(종성 글자 x) 고 다음 셀이 있다면
        //                 if ((intWord == 0x3138 || intWord == 0x3143 || intWord == 0x3149) && curCell != wordLen - 1)
        //                 {
        //                     //다음 셀에 입력
        //                     curCell = ++cellManager.curCellPos;
        //                     first_idx[curCell] = firstLetterDiff[intWord - 0x3131];
        //                     isFirst[curCell] = true;
        //                     bOnlyEnterFirst = true;
        //                     //print("ㄸ ㅃ ㅉ 다음 셀에 입력");
        //                 }
        //                 else if (!(intWord == 0x3138 || intWord == 0x3143 || intWord == 0x3149)) // 종성 글자면 입력
        //                 {
        //                     last_idx[curCell] = (intWord - 0x3131) + 1;
        //                     isLast[curCell] = true;
        //                 }
        //             }
        //         }
        //         //중성 없을시 모음 입력이면
        //         else if (0x314F <= intWord && intWord <= 0x3163)
        //         {
        //             mid_idx[curCell] = intWord - 0x314F;
        //             isMiddle[curCell] = true;
        //         }
        //         //중성 입력 차례에 자음이 입력되면 무시
        //         else
        //             return;
        //         //결과 반영
        //         SetCellText(bOnlyEnterFirst, curCell);
        //     }

        //     //Caps 상태면 해제
        //     if (cellManager.bIsCaps)
        //     {
        //         CapsWord();
        //     }
        //     //셀 꽉차면 무시
        //     if (cellManager.curCellPos == wordLen)
        //         return;
        // }

        // public override void Backspace()
        // {
        //     if (GamePlayManager.Instance.isEnd) return;
        //     /*charEnteredForDeblocked = GameSetting.instance.GetComponent<CharBlockedStage>();
        //     cellManager = GameSetting.instance.GetComponent<CellManagerBlocked>();*/
        //     string[] firstLetters = charEnteredForBlocked.firstLetters;
        //     bool[] isFirst = charEnteredForBlocked.isFirst, isMiddle = charEnteredForBlocked.isMiddle, isLast = charEnteredForBlocked.isLast;
        //     int[] first_idx = charEnteredForBlocked.first_idx, mid_idx = charEnteredForBlocked.mid_idx, last_idx = charEnteredForBlocked.last_idx;
        //     int[] firstLetterDiff = charEnteredForBlocked.firstLetterDiff, lastLetterDiff = charEnteredForBlocked.lastLetterDiff;
        //     int wordLen = charEnteredForBlocked.wordLen;
        //     int curCell = cellManager.curCellPos;
        //     if (isLast[curCell]) //종성 있으면
        //     {
        //         //이중자음이면 자음 하나 지우기
        //         if (last_idx[curCell] == 3) // ㄳ
        //             last_idx[curCell] = 1;
        //         else if (last_idx[curCell] == 5 || last_idx[curCell] == 6) // ㄵ ㄶ
        //             last_idx[curCell] = 4;
        //         else if (10 <= last_idx[curCell] && last_idx[curCell] <= 16)
        //             last_idx[curCell] = 9;
        //         else if (last_idx[curCell] == 20) // ㅄ
        //             last_idx[curCell] = 18;
        //         else
        //         {
        //             //이중자음 아니면 그냥 종성 지우기
        //             last_idx[curCell] = 0;
        //             isLast[curCell] = false;
        //         }
        //     }
        //     else if (isMiddle[curCell]) //중성 있으면 지우기
        //     {

        //         //중성이 ㅘ,ㅙ,ㅚ,ㅝ,ㅞ,ㅟ,ㅢ 인 경우 모음 하나 지우기
        //         if (mid_idx[curCell] == 9 || mid_idx[curCell] == 10 || mid_idx[curCell] == 11)
        //             mid_idx[curCell] = 8;
        //         else if (mid_idx[curCell] == 14 || mid_idx[curCell] == 15 || mid_idx[curCell] == 16)
        //             mid_idx[curCell] = 13;
        //         else if (mid_idx[curCell] == 19)
        //             mid_idx[curCell] = 18;
        //         else //이외의 경우 모음 지우기
        //         {
        //             mid_idx[curCell] = 0;
        //             isMiddle[curCell] = false;
        //             cellManager.SetText(firstLetters[first_idx[curCell]]);
        //             return;
        //         }
        //     }
        //     else if (isFirst[curCell]) //초성 있으면 지우고 빈칸 및 이전 셀로 이동
        //     {
        //         first_idx[curCell] = -1;
        //         cellManager.SetText(" ");
        //         isFirst[curCell] = false;
        //         if (cellManager.curCellPos != 0)
        //             cellManager.curCellPos--;
        //         return;
        //     }
        //     else
        //     {
        //         return;
        //     }
        //     //바뀐 결과 반영
        //     cellManager.SetText(System.Convert.ToChar((0xAC00 + 28 * 21 * first_idx[curCell] + 28 * mid_idx[curCell] + lastLetterDiff[last_idx[curCell]])).ToString());
        // }

        // public override void EnterWord()
        // {
        //     if (GamePlayManager.Instance.isEnd) return;
        //     /*charEnteredForDeblocked = GameSetting.Instance.GetComponent<CharBlockedStage>();
        //     cellManager = GameSetting.Instance.GetComponent<CellManagerBlocked>();*/
        //     int correctCount = 0;
        //     int wordLength = charEnteredForBlocked.wordLen;
        //     //string gameType = GameManager.Instance.gameType;
        //     string combinedWord = "";

        //     System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //     for (int i = 0; i < wordLength; i++)
        //     {
        //         sb.Append(cellManager.Col[cellManager.tryN, i].GetComponent<StageCell>().cellText.text);
        //     }

        //     combinedWord = sb.ToString();
        //     //print(combinedWord);

        //     //입력한 단어가 사전에 없는 단어거나, 입력 칸을 다 채우지 않으면 무시
        //     if (combinedWord.Length != wordLength)
        //     {
        //         GamePlayManager.Instance.StateText("올바르지 않은 입력입니다.");
        //         return;
        //     }
        //     if (!GameManager.Instance.defaultList.Contains(combinedWord))
        //     {
        //         GamePlayManager.Instance.StateText("사전에 없습니다.");
        //         return;
        //     }

        //     int[] first_idx = charEnteredForBlocked.first_idx, mid_idx = charEnteredForBlocked.mid_idx, last_idx = charEnteredForBlocked.last_idx;
        //     int[] answerFirst_idx = new int[wordLength], answerMid_idx = new int[wordLength], answerLast_idx = new int[wordLength];

        //     //암호화된 단어 복호화
        //     List<byte> codedWord = new List<byte>();
        //     for (int i = 0; i < charEnteredForBlocked.answerWord.Count; i++)
        //     {
        //         codedWord.Add((byte)(charEnteredForBlocked.answerWord[i] - 214743673));
        //     }
        //     answer = System.Text.Encoding.UTF8.GetString(codedWord.ToArray());

        //     for (int i = 0; i < answer.Length; i++)
        //     {
        //         int value = System.Convert.ToInt32(answer[i]);
        //         int order = (answer[i] - 0xAC00);
        //         answerFirst_idx[i] = order / 588;
        //         answerMid_idx[i] = order % 588 / 28;
        //         answerLast_idx[i] = order % 28;
        //     }
        //     bool isFirstExisted, isFirstFound;
        //     bool isMidExisted, isMidFound;
        //     bool isLastExisted, isLastFound;
        //     for (int i = 0; i < wordLength; i++)
        //     {
        //         int[] lastLetterDiff = charEnteredForBlocked.lastLetterDiff;

        //         isFirstExisted = false;
        //         isMidExisted = false;
        //         isLastExisted = false;
        //         isFirstFound = false;
        //         isMidFound = false;
        //         isLastFound = false;

        //         GameObject firstCell = cellManager.checkCol[cellManager.tryN, i * 3 + 0];
        //         GameObject MidCell = cellManager.checkCol[cellManager.tryN, i * 3 + 1];
        //         GameObject LastCell = cellManager.checkCol[cellManager.tryN, i * 3 + 2];

        //         cellManager.SetCellColor(firstCell, 3);
        //         cellManager.SetCellColor(MidCell, 3);
        //         cellManager.SetCellColor(LastCell, 3);

        //         for (int j = 0; j < wordLength; j++)
        //         {
        //             if (!isFirstFound)
        //             {
        //                 if (first_idx[i] == answerFirst_idx[j])
        //                 {
        //                     isFirstExisted = true;
        //                     if (cellManager.cho[first_idx[i]] != 1)
        //                         cellManager.cho[first_idx[i]] = 2;
        //                     cellManager.SetCellColor(firstCell, 2);
        //                     if (i == j)
        //                     {
        //                         cellManager.cho[first_idx[i]] = 1;
        //                         cellManager.SetCellColor(firstCell, 1);
        //                         isFirstFound = true;
        //                     }
        //                 }
        //             }
        //             if (!isMidFound)
        //             {
        //                 if (mid_idx[i] == answerMid_idx[j])
        //                 {
        //                     isMidExisted = true;
        //                     if (cellManager.cho[mid_idx[i]] != 1)
        //                         cellManager.cho[mid_idx[i]] = 2;
        //                     cellManager.SetCellColor(MidCell, 2);
        //                     if (i == j)
        //                     {
        //                         cellManager.jung[mid_idx[i]] = 1;
        //                         cellManager.SetCellColor(MidCell, 1);
        //                         isMidFound = true;
        //                     }
        //                 }
        //             }
        //             if (lastLetterDiff[last_idx[i]] != 0 && !isLastFound)
        //             {
        //                 if (lastLetterDiff[last_idx[i]] == answerLast_idx[j])
        //                 {
        //                     isLastExisted = true;
        //                     if (cellManager.jong[lastLetterDiff[lastLetterDiff[i]]] != 1)
        //                         cellManager.jong[lastLetterDiff[last_idx[i]]] = 2;
        //                     cellManager.SetCellColor(LastCell, 2);
        //                     if (i == j)
        //                     {
        //                         cellManager.jong[lastLetterDiff[last_idx[i]]] = 1;
        //                         cellManager.SetCellColor(LastCell, 1);
        //                         isLastFound = true;
        //                     }
        //                 }
        //             }
        //         }
        //         if (!isFirstExisted)
        //             cellManager.cho[first_idx[i]] = 3;
        //         if (!isMidExisted)
        //             cellManager.jung[mid_idx[i]] = 3;
        //         if (!isLastExisted)
        //             cellManager.jong[lastLetterDiff[last_idx[i]]] = 3;

        //         // cellManager.setChecklistColor(first_idx[i], mid_idx[i], lastLetterDiff[last_idx[i]]);

        //         if (isFirstFound && isMidFound && (isLastFound || answerLast_idx[i] == 0))
        //             correctCount++;
        //     }
        //     //셀 위치 처음칸, 소진한 기회 1 증가
        //     cellManager.curCellPos = 0;
        //     cellManager.tryN++;

        //     //셀 정보 초기화
        //     for (int i = 0; i < wordLength; i++)
        //     {
        //         charEnteredForBlocked.isFirst[i] = false;
        //         charEnteredForBlocked.isMiddle[i] = false;
        //         charEnteredForBlocked.isLast[i] = false;

        //         charEnteredForBlocked.first_idx[i] = 0;
        //         charEnteredForBlocked.mid_idx[i] = 0;
        //         charEnteredForBlocked.last_idx[i] = 0;
        //     }
        //     //다 맞추면 승리, 기회 다 소진하고 못맞추면 패배
        //     if (answer == combinedWord && correctCount == wordLength)
        //     {
        //         //GameManager.Instance.data.completeWordList.Add(combinedWord);
        //         GameManager.Instance.data.clearBlockedWordIndex[GameManager.Instance.GameStageType - 1, charEnteredForBlocked.curIndex] = true;
        //         GameManager.Instance.data.clearBlockedStages[GameManager.Instance.CurrentStage - 1] = true;
        //         GameManager.Instance.data.attemptToClearBlockedStage[GameManager.Instance.CurrentStage - 1] = cellManager.tryN;
        //         GameManager.Instance.data.successBlockedGameNumber++;
        //         GameManager.Instance.data.attemptBlockedGameNumber++;
        //         GameManager.Instance.SaveData(GameManager.Instance.data);
        //         GamePlayManager.Instance.EndGame(answer);
        //     }
        //     else if (cellManager.tryN == 6)
        //     {
        //         StartCoroutine(WaitToEnd());
        //         GameManager.Instance.data.attemptBlockedGameNumber++;
        //         GameManager.Instance.SaveData(GameManager.Instance.data);
        //     }

        // }

        // public override void CapsWord()
        // {
        //     if (GamePlayManager.Instance.isEnd) return;
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

        // IEnumerator WaitToEnd()
        // {
        //     yield return new WaitForSeconds(1);
        //     GamePlayManager.Instance.LoseGame(answer);
        // }
    }
}