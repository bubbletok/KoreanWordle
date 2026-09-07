using UnityEngine;
using KW.UI;
using KW.Managers;
using KW.Core;
using KW.Core.Constants;
using KW.Utility;

using static KW.Core.Settings.GameplayEnums;
using KW.Gameplay;
using System;

namespace KW.Input
{
    public class KoreanInputHandlerBlocked : KoreanInputHandler
    {
        private GameplayCellManagerBlocked cellManagerBlocked;
        private KoreanCompositionState compositionState;

        #region Initialization
        protected override void InitializeMode()
        {
            cellManagerBlocked = gameplayManager.GameplayCellManager as GameplayCellManagerBlocked;
            compositionState = new KoreanCompositionState(gameplaySettings.MaxWordLength);

            if (cellManagerBlocked == null)
                KWDebug.LogError("[KoreanInputHandlerBlocked] CellManagerBlocked not found!");
        }
        #endregion

        #region Character Input
        public override void EnterChar(string hangulJamo)
        {
            if (gameplayManager.IsGameEnd) return;

            if (wordSelector == null)
            {
                KWDebug.LogError("[KoreanInputHandlerBlocked] WordSelector not found!");
                return;
            }
            if (cellManagerBlocked == null)
            {
                KWDebug.LogError("[KoreanInputHandlerBlocked] CellManagerBlocked not found!");
                return;
            }

            int wordLen = wordSelector.WordLength;
            int curCell = cellManagerBlocked.CurCellPos;
            int intWord = System.Convert.ToInt32(System.Convert.ToChar(hangulJamo));

            bool bOnlyEnterFirst = false;

            if (curCell < 0 || curCell >= wordLen)
                return;

            if (!compositionState.HasAnyComposition(curCell))
            {
                if (!HangulComposer.IsConsonant(intWord))
                    return;

                try
                {
                    int consonantIdx = HangulComposer.GetConsonantIndex(intWord);
                    compositionState.SetFirstConsonant(curCell, consonantIdx);
                    bOnlyEnterFirst = true;
                }
                catch (Exception e)
                {
                    KWDebug.LogError($"[KoreanInputHandlerBlocked] Error occurred when Enter First Consonant. {e}");
                    compositionState.ClearFirstConsonant(curCell);
                    bOnlyEnterFirst = false;
                    return;
                }
            }
            else if (compositionState.IsFirstConsonant[curCell])
            {
                if (compositionState.IsMiddleVowel[curCell])
                {
                    if (compositionState.IsLastConsonant[curCell])
                    {
                        HandleFullSyllableInput(curCell, intWord, hangulJamo, ref bOnlyEnterFirst);
                    }
                    else
                    {
                        HandleNoLastConsonantInput(curCell, intWord, hangulJamo, ref bOnlyEnterFirst);
                    }
                }
                else if (HangulComposer.IsVowel(intWord))
                {
                    try
                    {
                        compositionState.SetMiddleVowel(curCell, HangulComposer.GetVowelIndex(intWord));
                    }
                    catch (Exception e)
                    {
                        KWDebug.LogError($"[KoreanInputHandlerBlocked] Error occurred when Enter Middle Vowel. {e}");
                        compositionState.ClearMiddleVowel(curCell);
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            SetCellTextBlocked(bOnlyEnterFirst, cellManagerBlocked.CurCellPos);

            if (gameplayUIManager.IsCaps)
                CapsWord();

            if (cellManagerBlocked.CurCellPos == wordLen)
                return;
        }

        private void HandleFullSyllableInput(int curCell, int intWord, string hangulJamo, ref bool bOnlyEnterFirst)
        {
            int wordLen = wordSelector.WordLength;
            int lastIdx = compositionState.LastConsonantIndex[curCell];

            if (HangulComposer.IsVowel(intWord))
            {
                if (HangulComposer.IsDoubleConsonant(lastIdx))
                {
                    var (firstPart, secondPart) = HangulComposer.SplitDoubleConsonant(lastIdx);
                    compositionState.LastConsonantIndex[curCell] = firstPart;
                    SetCellTextBlocked(false, curCell);

                    cellManagerBlocked.CurCellPos++;
                    int newCell = cellManagerBlocked.CurCellPos;
                    compositionState.SetFirstConsonant(newCell, secondPart);
                    compositionState.SetMiddleVowel(newCell, HangulComposer.GetVowelIndex(intWord));
                    SetCellTextBlocked(false, newCell);
                    bOnlyEnterFirst = false;
                }
                else
                {
                    int consonantIdx = HangulComposer.GetChosungIndexFromJongsung(lastIdx);
                    compositionState.ClearLastConsonant(curCell);
                    SetCellTextBlocked(false, curCell);

                    cellManagerBlocked.CurCellPos++;
                    int newCell = cellManagerBlocked.CurCellPos;
                    compositionState.SetFirstConsonant(newCell, consonantIdx);
                    compositionState.SetMiddleVowel(newCell, HangulComposer.GetVowelIndex(intWord));
                    SetCellTextBlocked(false, newCell);
                    bOnlyEnterFirst = false;
                }
            }
            else if (HangulComposer.IsConsonant(intWord))
            {
                int newJongsung = HangulComposer.TryFormDoubleConsonant(lastIdx, hangulJamo);
                if (newJongsung != -1)
                {
                    compositionState.LastConsonantIndex[curCell] = newJongsung;
                }
                else if (cellManagerBlocked.CurCellPos < wordLen - 1)
                {
                    cellManagerBlocked.CurCellPos++;
                    EnterChar(hangulJamo);
                    return;
                }
            }
        }

        private void HandleNoLastConsonantInput(int curCell, int intWord, string character, ref bool bOnlyEnterFirst)
        {
            int wordLen = wordSelector.WordLength;
            int midIdx = compositionState.MiddleVowelIndex[curCell];

            if (HangulComposer.IsVowel(intWord))
            {
                if (HangulComposer.CanFormDoubleVowel(midIdx, character))
                    compositionState.MiddleVowelIndex[curCell] = HangulComposer.GetDoubleVowelIndex(midIdx, character);
            }
            else if (HangulComposer.IsConsonant(intWord))
            {
                if (!HangulComposer.CanBeFinalConsonant(intWord) && curCell != wordLen - 1)
                {
                    cellManagerBlocked.CurCellPos++;
                    int newCell = cellManagerBlocked.CurCellPos;
                    int consonantIdx = HangulComposer.GetConsonantIndex(intWord);
                    compositionState.SetFirstConsonant(newCell, consonantIdx);
                    bOnlyEnterFirst = true;
                }
                else if (HangulComposer.CanBeFinalConsonant(intWord))
                {
                    compositionState.SetLastConsonant(curCell, HangulComposer.ConsonantCodeToJongsungIndex(intWord));
                }
            }
        }

        private void SetCellTextBlocked(bool bOnlyFirst, int curCell)
        {
            int firstIdx = compositionState.FirstConsonantIndex[curCell];
            int midIdx = compositionState.MiddleVowelIndex[curCell];
            int lastIdx = compositionState.LastConsonantIndex[curCell];

            if (bOnlyFirst || !compositionState.IsMiddleVowel[curCell])
            {
                cellManagerBlocked.SetCurrentCellText(KoreanInputConstants.ChosungLetters[firstIdx]);
            }
            else
            {
                char composed = HangulComposer.ComposeHangul(firstIdx, midIdx, lastIdx);
                cellManagerBlocked.SetCurrentCellText(composed.ToString());
            }
        }
        #endregion

        #region Backspace
        public override void Backspace()
        {
            if (gameplayManager.IsGameEnd) return;

            if (wordSelector == null)
            {
                KWDebug.LogError("[KoreanInputHandlerBlocked] WordSelector not found!");
                return;
            }
            if (cellManagerBlocked == null)
            {
                KWDebug.LogError("[KoreanInputHandlerBlocked] CellManagerBlocked not found!");
                return;
            }

            int curCell = cellManagerBlocked.CurCellPos;

            if (compositionState.IsLastConsonant[curCell])
            {
                int lastIdx = compositionState.LastConsonantIndex[curCell];

                if (HangulComposer.IsDoubleConsonant(lastIdx))
                    compositionState.LastConsonantIndex[curCell] = HangulComposer.SimplifyDoubleConsonant(lastIdx);
                else
                    compositionState.ClearLastConsonant(curCell);
            }
            else if (compositionState.IsMiddleVowel[curCell])
            {
                int midIdx = compositionState.MiddleVowelIndex[curCell];

                if (HangulComposer.IsDoubleVowel(midIdx))
                {
                    compositionState.MiddleVowelIndex[curCell] = HangulComposer.SimplifyDoubleVowel(midIdx);
                }
                else
                {
                    compositionState.ClearMiddleVowel(curCell);
                    cellManagerBlocked.SetCurrentCellText(KoreanInputConstants.ChosungLetters[compositionState.FirstConsonantIndex[curCell]]);
                    return;
                }
            }
            else if (compositionState.IsFirstConsonant[curCell])
            {
                compositionState.ClearFirstConsonant(curCell);
                cellManagerBlocked.SetCurrentCellText(" ");
                if (cellManagerBlocked.CurCellPos != 0)
                    cellManagerBlocked.CurCellPos--;
                return;
            }
            else
            {
                return;
            }

            char composed = HangulComposer.ComposeHangul(
                compositionState.FirstConsonantIndex[curCell],
                compositionState.MiddleVowelIndex[curCell],
                compositionState.LastConsonantIndex[curCell]
            );
            cellManagerBlocked.SetCurrentCellText(composed.ToString());
        }
        #endregion

        #region Word Submission
        public override void EnterWord()
        {
            if (gameplayManager.IsGameEnd) return;

            if (wordSelector == null)
            {
                KWDebug.LogError("[KoreanInputHandlerBlocked] WordSelector not found!");
                return;
            }
            if (cellManagerBlocked == null)
            {
                KWDebug.LogError("[KoreanInputHandlerBlocked] CellManagerBlocked not found!");
                return;
            }

            int wordLength = wordSelector.WordLength;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < wordLength; i++)
                sb.Append(cellManagerBlocked.GameplayCells[cellManagerBlocked.NumberOfTry, i].Text);
            string combinedWord = sb.ToString();

            if (combinedWord.Length != wordLength)
            {
                gameplayManager.ShowStatusText("올바르지 않은 입력입니다.");
                return;
            }
            if (!gameManager.DefaultList.Contains(combinedWord))
            {
                gameplayManager.ShowStatusText("사전에 없습니다.");
                return;
            }

            answer = wordSelector.GetAnswer();
            int[] answerFirstIdx = new int[wordLength];
            int[] answerMidIdx = new int[wordLength];
            int[] answerLastIdx = new int[wordLength];

            for (int i = 0; i < answer.Length; i++)
                HangulComposer.DecomposeHangul(answer[i], out answerFirstIdx[i], out answerMidIdx[i], out answerLastIdx[i]);

            int[] answerChoFreq = new int[KoreanInputConstants.ChosungLetters.Length];
            int[] answerJungFreq = new int[KoreanInputConstants.JungsungLetters.Length];
            int[] answerJongFreq = new int[KoreanInputConstants.JongsungLetters.Length];

            for (int i = 0; i < wordLength; i++)
            {
                answerChoFreq[answerFirstIdx[i]]++;
                answerJungFreq[answerMidIdx[i]]++;
                if (answerLastIdx[i] != 0) answerJongFreq[answerLastIdx[i]]++;
            }

            int correctCount = 0;

            for (int i = 0; i < wordLength; i++)
            {
                int firstIdx = compositionState.FirstConsonantIndex[i];
                int midIdx = compositionState.MiddleVowelIndex[i];
                int lastIdx = compositionState.LastConsonantIndex[i];
                int lastIdxAdjusted = HangulComposer.GetJongsungMappedIndex(lastIdx);

                bool isChoCorrect = firstIdx == answerFirstIdx[i];
                bool isJungCorrect = midIdx == answerMidIdx[i];
                bool isJongCorrect = (lastIdxAdjusted == answerLastIdx[i]) ||
                                     (answerLastIdx[i] == 0 && lastIdxAdjusted == 0);

                bool isChoExist = answerChoFreq[firstIdx] > 0;
                bool isJungExist = answerJungFreq[midIdx] > 0;
                bool isJongExist = (lastIdxAdjusted != 0) && (answerJongFreq[lastIdxAdjusted] > 0);

                if (isChoCorrect)       cellManagerBlocked.ChosungStates[firstIdx] = CellState.Correct;
                else if (isChoExist)    cellManagerBlocked.ChosungStates[firstIdx] = CellState.WrongPosition;
                else                    cellManagerBlocked.ChosungStates[firstIdx] = CellState.NotInWord;

                if (isJungCorrect)      cellManagerBlocked.JungsungStates[midIdx] = CellState.Correct;
                else if (isJungExist)   cellManagerBlocked.JungsungStates[midIdx] = CellState.WrongPosition;
                else                    cellManagerBlocked.JungsungStates[midIdx] = CellState.NotInWord;

                if (answerLastIdx[i] == 0) cellManagerBlocked.JongsungStates[lastIdxAdjusted] = CellState.Empty;
                else if (lastIdxAdjusted != 0)
                {
                    if (isJongCorrect)      cellManagerBlocked.JongsungStates[lastIdxAdjusted] = CellState.Correct;
                    else if (isJongExist)   cellManagerBlocked.JongsungStates[lastIdxAdjusted] = CellState.WrongPosition;
                    else                    cellManagerBlocked.JongsungStates[lastIdxAdjusted] = CellState.NotInWord;
                }
                else cellManagerBlocked.JongsungStates[lastIdxAdjusted] = CellState.NotInWord;

                GameplayCell gameplayCell = cellManagerBlocked.GameplayCells[cellManagerBlocked.NumberOfTry, i];
                cellManagerBlocked.SetCellState(gameplayCell, cellManagerBlocked.ChosungStates[firstIdx], HangulComponentPosition.Chosung);
                cellManagerBlocked.SetCellState(gameplayCell, cellManagerBlocked.JungsungStates[midIdx], HangulComponentPosition.Jungsung);
                cellManagerBlocked.SetCellState(gameplayCell, cellManagerBlocked.JongsungStates[lastIdxAdjusted], HangulComponentPosition.Jongsung);

                if (isChoCorrect && isJungCorrect && isJongCorrect) correctCount++;
            }

            cellManagerBlocked.CurCellPos = 0;
            cellManagerBlocked.NumberOfTry++;
            compositionState.Reset();

            if (answer == combinedWord && correctCount == wordLength)
                StartCoroutine(gameplayManager.WinGame(answer));
            else if (cellManagerBlocked.NumberOfTry == gameplaySettings.MaxAttempts)
                StartCoroutine(gameplayManager.LoseGame(answer));
        }
        #endregion
    }
}
