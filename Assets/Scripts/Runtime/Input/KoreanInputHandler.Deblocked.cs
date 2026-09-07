using UnityEngine;

using KW.UI;
using System.Collections.Generic;
using KW.Core.Constants;
using KW.Gameplay;
using KW.Core.Settings;
using KW.Utility;

namespace KW.Input
{
    public class KoreanInputHandlerDeblocked : KoreanInputHandler
    {
        private GameplayCellManagerDeblocked cellManagerDeblocked;
        private List<string> answerCharactersBuffer = new List<string>();

        #region Initialization
        protected override void InitializeMode()
        {
            cellManagerDeblocked = gameplayManager.GameplayCellManager as GameplayCellManagerDeblocked;

            if (cellManagerDeblocked == null)
                KWDebug.LogError("[KoreanInputHandlerDeblocked] CellManagerDeblocked not found!");

            answer = wordSelector.GetAnswer();
            answerCharactersBuffer.Clear();

            for (int i = 0; i < answer.Length; i++)
            {
                HangulComposer.DecomposeHangul(answer[i], out int answerFirstIdx, out int answerMidIdx, out int answerLastIdx);
                answerCharactersBuffer.Add(KoreanInputConstants.ChosungLetters[answerFirstIdx]);
                answerCharactersBuffer.Add(KoreanInputConstants.JungsungLetters[answerMidIdx]);
                if (answerLastIdx > 0)
                {
                    if (answerLastIdx >= 8) answerLastIdx++;
                    if (answerLastIdx >= 19) answerLastIdx++;
                    answerCharactersBuffer.Add(KoreanInputConstants.JongsungLetters[answerLastIdx]);
                }
            }
        }
        #endregion

        #region Character Input
        public override void EnterChar(string hangulJamo)
        {
            if (gameplayManager.IsGameEnd) return;

            int wordLen = wordSelector.DecomposedWordLength;
            int curCell = cellManagerDeblocked.CurCellPos;

            if (curCell >= wordLen || curCell < 0)
                return;

            int intWord = System.Convert.ToInt32(System.Convert.ToChar(hangulJamo));

            if (!HangulComposer.IsKoreanChar(intWord)) return;

            cellManagerDeblocked.SetCurrentCellText(hangulJamo);
            cellManagerDeblocked.CurCellPos++;

            if (gameplayUIManager.IsCaps)
                CapsWord();

            if (cellManagerDeblocked.CurCellPos == wordLen)
                return;
        }
        #endregion

        #region Backspace
        public override void Backspace()
        {
            if (gameplayManager.IsGameEnd) return;

            if (wordSelector == null)
            {
                KWDebug.LogError("[KoreanInputHandlerDeblocked] WordSelector not found!");
                return;
            }
            if (cellManagerDeblocked == null)
            {
                KWDebug.LogError("[KoreanInputHandlerDeblocked] CellManagerDeblocked not found!");
                return;
            }

            int curCell = cellManagerDeblocked.CurCellPos;
            if (curCell < 0) return;
            if (curCell != 0) cellManagerDeblocked.CurCellPos--;
            cellManagerDeblocked.SetCurrentCellText(" ");
        }
        #endregion

        #region Word Submission
        public override void EnterWord()
        {
            int wordLength = wordSelector.DecomposedWordLength;

            if (cellManagerDeblocked.CurCellPos != wordLength)
            {
                gameplayManager.ShowStatusText("유효하지 않는 입력입니다.");
                return;
            }

            HashSet<string> candidates = GenerateCandidateWords(cellManagerDeblocked.NumberOfTry, wordLength);

            string combinedWord = null;
            foreach (string candidate in candidates)
            {
                if (gameManager.DefaultList.Contains(candidate))
                {
                    combinedWord = candidate;
                    if (candidate == wordSelector.GetAnswer()) break;
                }
            }

            if (combinedWord == null)
            {
                gameplayManager.ShowStatusText("사전에 없습니다.");
                return;
            }

            int correctCount = 0;
            for (int i = 0; i < wordLength; i++)
            {
                string enteredChar = cellManagerDeblocked.GameplayCells[cellManagerDeblocked.NumberOfTry, i].Text;

                GameplayEnums.CellState cellState;
                if (enteredChar == answerCharactersBuffer[i])
                {
                    cellState = GameplayEnums.CellState.Correct;
                    correctCount++;
                }
                else if (answerCharactersBuffer.Contains(enteredChar)) cellState = GameplayEnums.CellState.WrongPosition;
                else cellState = GameplayEnums.CellState.NotInWord;

                GameplayCell gameplayCell = cellManagerDeblocked.GameplayCells[cellManagerDeblocked.NumberOfTry, i];
                cellManagerDeblocked.SetCellState(gameplayCell, cellState);
                keyboardView.SetKeyCellState(enteredChar, cellState);
            }

            cellManagerDeblocked.CurCellPos = 0;
            cellManagerDeblocked.NumberOfTry++;

            if (candidates.Contains(answer) && correctCount == wordLength)
                StartCoroutine(gameplayManager.WinGame(answer));
            else if (cellManagerDeblocked.NumberOfTry == gameplaySettings.MaxAttempts)
                StartCoroutine(gameplayManager.LoseGame(answer));
        }
        #endregion

        #region Jamo Combination
        private HashSet<string> GenerateCandidateWords(int row, int wordLength)
        {
            List<string> jamos = new List<string>(wordLength);
            for (int i = 0; i < wordLength; i++)
                jamos.Add(cellManagerDeblocked.GameplayCells[row, i].Text);

            var results = new HashSet<string>();
            ParseJamosRecursive(jamos, 0, -1, -1, -1, new System.Text.StringBuilder(), results);
            return results;
        }

        private void ParseJamosRecursive(
            List<string> jamos, int pos,
            int choIdx, int jungIdx, int jongIdx,
            System.Text.StringBuilder sb, HashSet<string> results)
        {
            if (pos == jamos.Count)
            {
                if (choIdx >= 0 && jungIdx >= 0)
                {
                    char last = jongIdx >= 0
                        ? HangulComposer.ComposeHangul(choIdx, jungIdx, jongIdx)
                        : HangulComposer.ComposeHangulWithoutJongsung(choIdx, jungIdx);
                    sb.Append(last);
                    results.Add(sb.ToString());
                    sb.Length--;
                }
                else if (choIdx < 0)
                {
                    results.Add(sb.ToString());
                }
                return;
            }

            string jamo = jamos[pos];
            int code = System.Convert.ToInt32(System.Convert.ToChar(jamo));
            bool isCons = HangulComposer.IsConsonant(code);
            bool isVow = HangulComposer.IsVowel(code);

            if (choIdx < 0)
            {
                if (isCons)
                    ParseJamosRecursive(jamos, pos + 1, HangulComposer.GetConsonantIndex(code), -1, -1, sb, results);
                return;
            }

            if (jungIdx < 0)
            {
                if (isVow)
                    ParseJamosRecursive(jamos, pos + 1, choIdx, HangulComposer.GetVowelIndex(code), -1, sb, results);
                return;
            }

            if (jongIdx < 0)
            {
                if (isCons)
                {
                    if (HangulComposer.CanBeFinalConsonant(code))
                    {
                        int jIdx = HangulComposer.ConsonantCodeToJongsungIndex(code);
                        ParseJamosRecursive(jamos, pos + 1, choIdx, jungIdx, jIdx, sb, results);
                    }

                    char sylNoJong = HangulComposer.ComposeHangulWithoutJongsung(choIdx, jungIdx);
                    sb.Append(sylNoJong);
                    ParseJamosRecursive(jamos, pos + 1, HangulComposer.GetConsonantIndex(code), -1, -1, sb, results);
                    sb.Length--;
                }
                else if (isVow && HangulComposer.CanFormDoubleVowel(jungIdx, jamo))
                {
                    ParseJamosRecursive(jamos, pos + 1, choIdx, HangulComposer.GetDoubleVowelIndex(jungIdx, jamo), -1, sb, results);
                }
                return;
            }

            if (isVow)
            {
                int nextCho = HangulComposer.GetChosungIndexFromJongsung(jongIdx);
                char sylNoJong = HangulComposer.ComposeHangulWithoutJongsung(choIdx, jungIdx);
                sb.Append(sylNoJong);
                ParseJamosRecursive(jamos, pos + 1, nextCho, HangulComposer.GetVowelIndex(code), -1, sb, results);
                sb.Length--;
            }
            else if (isCons)
            {
                char sylWithJong = HangulComposer.ComposeHangul(choIdx, jungIdx, jongIdx);
                sb.Append(sylWithJong);
                ParseJamosRecursive(jamos, pos + 1, HangulComposer.GetConsonantIndex(code), -1, -1, sb, results);
                sb.Length--;
            }
        }
        #endregion
    }
}
