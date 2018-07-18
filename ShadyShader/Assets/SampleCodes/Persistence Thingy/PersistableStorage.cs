using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PersistableStorage : MonoBehaviour
{
    private static PersistableStorage _instance;
    public static PersistableStorage Instance { get { return _instance; } }

    private string savePath;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        DontDestroyOnLoad(this);


        savePath = Path.Combine(Application.persistentDataPath, "SampleSaveFile");
        ShowSaveInfo();
    }

    public void Save(PersistableObject obj)
    {
        using (
            var writer = new BinaryWriter(File.Open(savePath, FileMode.Create))
            )
        {
            obj.Save(new GameDataWriter(writer));
        }

        Debug.Log("Saved!");
    }

    public void Load(PersistableObject obj)
    {
        using (
            var reader = new BinaryReader(File.Open(savePath, FileMode.Open))
            )
        {
            obj.Load(new GameDataReader(reader));
        }

        Debug.Log("Loaded!");
    }


    public void ShowSaveInfo()
    {
        Debug.Log("Save file path: " + savePath);
    }

}
