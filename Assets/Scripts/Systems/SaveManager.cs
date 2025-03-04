using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public bool[] g_clearFlag = new bool[30];
    public bool[] g_arriveFlag = new bool[30];
    public int[] g_starCount = new int[30];
}

public class SaveManager : MonoBehaviour
{
    private static SaveData _saveData = null;
    static public SaveData SaveDataInstance
    {
        set { _saveData = value; }
        get
        {
            if(_saveData == null)
            {
                LoadData();
            }
            return _saveData; 
        }
    }
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

    private void OnDisable()
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
            data.g_arriveFlag[i] = true;
            data.g_starCount[i] = 3;
        }
        _saveData = data;

        Debug.Log("complete");
    }

    public static void SaveData()
    {
        //Debug.Log("Button clicked. Saving " + saveData.g_clearFlag + " to file.");
        SaveDataToFile(_saveData, _saveFileName);
    }

    public static void LoadData()
    {
        _saveData = LoadDataFromFile(_saveFileName);
        //Debug.Log("saveData.MyInt: " + saveData.g_clearFlag);
    }

    private static void SaveDataToFile(SaveData data, string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName;
        FileStream fileStream = new FileStream(filePath, FileMode.Create);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fileStream, data);
        fileStream.Close();
        //  Debug.Log("Save data saved to " + filePath);
    }

    private static SaveData LoadDataFromFile(string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(filePath))
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    if(fileStream.Length == 0)
                    {
                        //セーブデータが空の場合
                        return CreateDefaultSaveData();
                    }

                    BinaryFormatter bf = new BinaryFormatter();
                    return (SaveData)bf.Deserialize(fileStream);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to load save data: " + ex.Message);
                return CreateDefaultSaveData();
            }
        }
        else
        {
            //新しいセーブデータ作成
            return CreateDefaultSaveData();
        }
    }

    private static SaveData CreateDefaultSaveData()
    {
        //Debug.Log("Save file not found.");
        SaveData data = new SaveData();

        for (int i = 0; i < 30; ++i)
        {
            data.g_clearFlag[i] = false;
            data.g_arriveFlag[i] = false;
            data.g_starCount[i] = 0;
        }

        return data;
    }
}