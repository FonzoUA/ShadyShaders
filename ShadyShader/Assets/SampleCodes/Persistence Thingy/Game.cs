using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : PersistableObject
{
    public ShapeFactory shapeFactory;
    
    public KeyCode createKey = KeyCode.C;
    public KeyCode destroyKey = KeyCode.X;
    public KeyCode resetKey = KeyCode.R;
    public KeyCode saveKey = KeyCode.F5;
    public KeyCode loadKey = KeyCode.F9;

    public float spawnRadius = 5.0f;
    public float creationSpeed { get; set; }
    public float destructionSpeed { get; set; }

    const int saveVersion = 1;

    private Shape tempObj;
    private Transform tempTrans;
    private List<Shape> shapes;
    private float creationProcess;
    private float destructionProcess;
    private void Awake()
    {
        shapes = new List<Shape>();
    }

    private void Update()
    {
        creationProcess += Time.deltaTime * creationSpeed;
        destructionProcess += Time.deltaTime * destructionSpeed;
        while (creationProcess >= 1f)
        {
            creationProcess -= 1f;
            CreateObject();
        }
        while (destructionProcess >= 1f)
        {
            destructionProcess -= 1f;
            DestroyShape();
        }

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
            PersistableStorage.Instance.Save(this, saveVersion);   
        }
        else if (Input.GetKeyDown(loadKey))
        {
            ResetLevel();
            PersistableStorage.Instance.Load(this);
        }
        else if (Input.GetKeyDown(destroyKey))
        {
            DestroyShape();
        }
    }

    private void CreateObject()
    {
        tempObj = shapeFactory.GetRandom();
        tempTrans = tempObj.transform;
        tempTrans.localPosition = Random.insideUnitSphere * spawnRadius;
        tempTrans.localRotation = Random.rotation;
        tempTrans.localScale = Vector3.one * Random.Range(0.3f, 1.5f);
        tempObj.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));
        shapes.Add(tempObj);
    }

    private void ResetLevel()
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            shapeFactory.ReturnShape(shapes[i]);
        }
        shapes.Clear();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(shapes.Count);
        for (int i = 0; i < shapes.Count; i++)
        {
            writer.Write(shapes[i].ShapeID);
            writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }
    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
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
            int matId = version > 0 ? reader.ReadInt() : 0;
            Shape o = shapeFactory.Get(shapeId, matId);
            o.Load(reader);
            shapes.Add(o);
        }
    }

    private void DestroyShape()
    {
        if (shapes.Count <= 0)
            return;
        int index = Random.Range(0, shapes.Count);
        shapeFactory.ReturnShape(shapes[index]);
        int lastIndex = shapes.Count - 1;
        shapes[index] = shapes[lastIndex];
        shapes.RemoveAt(lastIndex);
    }
}
