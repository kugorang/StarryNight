using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public struct UpgradeInfo
{
    public int index;
    public string name;
    public int[] value;
    public int[] cost;
}

public class UpgradeDictionary : MonoBehaviour {

    enum FILEINFO
    {
        UPGRADETABLE
    }
    
    public Dictionary<int, UpgradeInfo> findUpDic;

    private void Start()
    {
        // Dictionary을 초기화
        findUpDic = new Dictionary<int, UpgradeInfo>();

        ReadDataFile("dataTable/upgradeTable", FILEINFO.UPGRADETABLE);
    }

    private void ReadDataFile(string fileName, FILEINFO fileType)
    {
        TextAsset txtFile = (TextAsset)Resources.Load(fileName) as TextAsset;
        string txt = txtFile.text;
        string[] stringOperators = new string[] { "\r\n" };
        string[] lineList = txt.Split(stringOperators, StringSplitOptions.None);

        int lineListLen = lineList.Length-1;

        for (int i = 0; i < lineListLen; i++)
        {
            string[] wordList = lineList[i].Split(',');

            int index = Convert.ToInt32(wordList[0]);

            UpgradeInfo upInfo;
            upInfo.index = index;
            upInfo.name = wordList[1];

            upInfo.value = new int[20];
            upInfo.cost = new int[20];

            for (int j = 0; j < 20; j++)
            {
                int value = Convert.ToInt32(wordList[2*j+2]);
                int cost = Convert.ToInt32(wordList[2*j+3]);
                upInfo.value[j] = value;
                upInfo.cost[j] = cost;
            }

            findUpDic[index] = upInfo;
        }
    }

    public UpgradeInfo FindUpgrade(int key)
    {
        return findUpDic[key];
    }
}
