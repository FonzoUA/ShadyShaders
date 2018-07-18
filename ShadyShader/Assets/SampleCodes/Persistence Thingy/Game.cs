using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : PersistableObject
{
    public ShapeFactory shapeFactory;
    
    public KeyCode createKey = KeyCode.C;
    public KeyCode resetKey = KeyCode.R;
    public KeyCode saveKey = KeyCode.F5;
    public KeyCode loadKey = KeyCode.F9;

    public float spawnRadius = 5.0f;

    const int saveVersion = 1;

    private Shape tempObj;
    private Transform tempTrans;
    private List<Shape> shapes;
    private void Awake()
    {
        shapes = new List<Shape>();
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
        tempObj = shapeFactory.GetRandom();
        tempTrans = tempObj.transform;
        tempTrans.localPosition = Random.insideUnitSphere * spawnRadius;
        tempTrans.localRotation = Random.rotation;
        tempTrans.localScale = Vector3.one * Random.Range(0.3f, 1.5f);

        shapes.Add(tempObj);
    }

    private void ResetLevel()
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            Destroy(shapes[i].gameObject);
        }
        shapes.Clear();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(-saveVersion);
        writer.Write(shapes.Count);
        for (int i = 0; i < shapes.Count; i++)
        {
            writer.Write(shapes[i].ShapeID);
            shapes[i].Save(writer);
        }
    }
    public override void Load(GameDataReader reader)
    {
        int version = -reader.ReadInt();
        if (version > saveVersion)
        {
            Debug.LogError("Unsupported future save versions " + version);
            return;
        }
        //          condition    ?  TRUEresult : FALSEresult;
        int count = version <= 0 ? -version : reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            Shape o = shapeFactory.Get(shapeId);
            o.Load(reader);
            shapes.Add(o);
        }
    }
}
