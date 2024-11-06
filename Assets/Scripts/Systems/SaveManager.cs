using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public bool[] g_clearFlag = new bool[30];
    public int[] g_starCount = new int[30];
}

public class SaveManager : MonoBehaviour
{
    static public SaveData g_saveData;
    private const string _saveFileName = "save.dat";

    private static bool _hasInitialized = false;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (!_hasInitialized)
        {
            _hasInitialized = true;
            LoadData();
        }
        //Debug.Log("saveData.MyInt: " + saveData.g_clearFlag);
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void Update()
    {
        //if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.RightShift))
        //{
        //    ResetData();
        //}

        if ((Input.GetKey(KeyCode.LeftShift)) && Input.GetKey(KeyCode.C))
        {
            CompleteData();
        }
    }

    //public void ResetData()
    //{
    //    SaveData data = new SaveData();
    //    for (int i = 0; i < 30; ++i)
    //    {
    //        data.g_clearFlag[i] = false;
    //        data.g_starCount[i] = 0;
    //    }
    //    g_saveData = data;

    //    Debug.Log("reset");
    //}

    public void CompleteData()
    {
        SaveData data = new SaveData();
        for (int i = 0; i < 30; ++i)
        {
            data.g_clearFlag[i] = true;
            data.g_starCount[i] = 3;
        }
        g_saveData = data;

        Debug.Log("complete");
    }

    public void SaveData()
    {
        //Debug.Log("Button clicked. Saving " + saveData.g_clearFlag + " to file.");
        SaveDataToFile(g_saveData, _saveFileName);
    }

    public void LoadData()
    {
        g_saveData = LoadDataFromFile(_saveFileName);
        //Debug.Log("saveData.MyInt: " + saveData.g_clearFlag);
    }

    private void SaveDataToFile(SaveData data, string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName;
        FileStream fileStream = new FileStream(filePath, FileMode.Create);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fileStream, data);
        fileStream.Close();
        //  Debug.Log("Save data saved to " + filePath);
    }

    private SaveData LoadDataFromFile(string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(filePath))
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            SaveData data = (SaveData)bf.Deserialize(fileStream);
            fileStream.Close();
            // Debug.Log("Save data loaded from " + filePath);
            return data;
        }
        else
        {
            //Debug.Log("Save file not found.");
            SaveData data = new SaveData();

            for (int i = 0; i < 30; ++i)
            {
                data.g_clearFlag[i] = false;
                data.g_starCount[i] = 0;
            }

            return data;
        }
    }
}