using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCubes : PersistableObject
{

    public PersistableObject prefab;
    public KeyCode createKey = KeyCode.C;
    public KeyCode resetKey = KeyCode.R;
    public KeyCode saveKey = KeyCode.F5;
    public KeyCode loadKey = KeyCode.F9;

    public float spawnRadius = 5.0f;

    private PersistableObject tempObj;
    private Transform tempTrans;
    private List<PersistableObject> objects;
    private void Awake()
    {
        objects = new List<PersistableObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(createKey))
        {
            CreateObject();
        }
        else if (Input.GetKeyDown(resetKey))
        {
            ResetLevel();
        }
        else if (Input.GetKeyDown(saveKey))
        {
            PersistableStorage.Instance.Save(this);   
        }
        else if (Input.GetKeyDown(loadKey))
        {
            ResetLevel();
            PersistableStorage.Instance.Load(this);
        }
    }

    private void CreateObject()
    {
        tempObj = Instantiate(prefab);
        tempTrans = tempObj.transform;
        tempTrans.localPosition = Random.insideUnitSphere * spawnRadius;
        tempTrans.localRotation = Random.rotation;
        tempTrans.localScale = Vector3.one * Random.Range(0.3f, 1.5f);

        objects.Add(tempObj);
    }

    private void ResetLevel()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i].gameObject);
        }
        objects.Clear();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(objects.Count);
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].Save(writer);
        }
    }
    public override void Load(GameDataReader reader)
    {
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            PersistableObject o = Instantiate(prefab);
            o.Load(reader);
            objects.Add(o);
        }
    }
}
