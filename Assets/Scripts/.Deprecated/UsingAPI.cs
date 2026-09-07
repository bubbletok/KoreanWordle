using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;

public class UsingAPI : MonoBehaviour
{
    string apiKey = "D6D37AE6052C90769F084CDE8807D501";
    //string targetCodeMethodType = "&type_search=view&req_type=json&method=TARGET_CODE&q=";
    string wordMethodType = "&type_search=search&req_type=json&q=";
    string[] strs;
    public string[] wordDefinition;

    private void Awake()
    {
        wordDefinition = new string[10];
    }

    private void Start()
    {
        //StartCoroutine(Validate("https://stdict.korean.go.kr/main/main.do"));
        //StartCoroutine(LoadDataWithTargetCode());
        //StartCoroutine(LoadDataWithWord("나무"));
    }

    public void GetDefintion(string word)
    {
        StartCoroutine(LoadDataWithWord(word));
    }

    public void GetDefinitionFromWord(ref string[] texts, string word)
    {
        for (int i = 0, defCnt = 0; i < texts.Length && defCnt < 10; i++)
        {
            if (texts[i] == "definition")
            {
                if (texts[i + 2].Contains(word)) continue;
                wordDefinition[defCnt] = "뜻" + (defCnt + 1).ToString() + ": " + texts[i + 2] + "\n";
                defCnt++;
            }
        }
    }
    IEnumerator LoadDataWithWord(string word)
    {
        string GetDataUrl = "https://stdict.korean.go.kr/api/search.do?certkey_no=4017&key=" + apiKey + wordMethodType + word;
        using (UnityWebRequest www = UnityWebRequest.Get(GetDataUrl))
        {
            www.certificateHandler = new AcceptCeritificates();
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                StartCoroutine(LoadDataWithWord(word));
            }
            else
            {
                if (www.isDone)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    strs = jsonResult.Split('"');
                    GetDefinitionFromWord(ref strs, word);
                }
            }
        }
    }
    /*    void GetDataFromTargetCode(string[] texts)
        {
            string tempWord = "";
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i] == "word" && texts[i + 2].Length < 3)
                {
                    tempWord = texts[i + 2];
                    i += 2;
                }
                if (texts[i] == "pos")
                {
                    if (texts[i + 2] != "명사")
                    {
                        tempWord = "";
                        break;
                    }
                }
                else if (texts[i] == "type")
                {
                    if (texts[i + 2] != "일반어" && texts[i + 2] != "한자어")
                    {
                        tempWord = "";
                        break;
                    }
                }
            }
            if(tempWord == "")
            {
                StartCoroutine(LoadDataWithTargetCode());
                return;
            }
            if (tempWord != "" && !completeList.Contains(tempWord))
            {
                currentWord = tempWord;
                //Debug.Log(currentWord);

                *//*byte[] bytesForEncoding = Encoding.Unicode.GetBytes(currentWord);
                string encodedString = System.Convert.ToBase64String(bytesForEncoding);
                byte[] decodedBytes = System.Convert.FromBase64String(encodedString);
                string decodedString = Encoding.Unicode.GetString(decodedBytes);
                Debug.Log(encodedString);
                Debug.Log(decodedString);*//*

                char[] values = currentWord.ToCharArray();
                foreach(char letter in values)
                {
                    int value = System.Convert.ToInt32(letter);
                    int firstLetter, middleLetter, lastLetter;
                    int uniChar = letter - 0xAC00;
                    firstLetter = 0x1100 + uniChar / 588;
                    middleLetter = 0x1161 + (uniChar % 588 ) / 28;
                    lastLetter = 0x11A7 + uniChar % 28;

                    givenList.Add(System.Convert.ToChar(firstLetter));
                    givenList.Add(System.Convert.ToChar(middleLetter));
                    if(lastLetter != 0x11A7)
                        givenList.Add(System.Convert.ToChar(lastLetter));
                }
                if (givenList.Count != 5)
                {
                    givenList.Clear();
                    StartCoroutine(LoadDataWithTargetCode());
                    return;
                }
            }
        }*/

    /*IEnumerator LoadDataWithTargetCode()
    {
        randN = UnityEngine.Random.Range(1, 550000);
        string GetDataUrl = "https://stdict.korean.go.kr/api/view.do?certkey_no=4017&key=" + apiKey + targetCodeMethodType + randN.ToString();
        using (UnityWebRequest www = UnityWebRequest.Get(GetDataUrl))
        {
            www.certificateHandler = new AcceptCeritificates();
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                StartCoroutine(LoadDataWithTargetCode());
            }
            else
            {
                if (www.isDone)
                {
                    jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    strs = jsonResult.Split('"');
                    GetDataFromTargetCode(strs);
                }
            }
        }
    }*/

    IEnumerator Validate(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.certificateHandler = new AcceptCeritificates();
        yield return www.SendWebRequest();
    }
}
