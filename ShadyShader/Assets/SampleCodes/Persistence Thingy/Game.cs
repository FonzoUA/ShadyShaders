using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : PersistableObject
{
    public ShapeFactory shapeFactory;
    public SpawnParticlesController particles;
    
    public KeyCode createKey = KeyCode.C;
    public KeyCode destroyKey = KeyCode.X;
    public KeyCode resetKey = KeyCode.R;
    public KeyCode saveKey = KeyCode.F5;
    public KeyCode loadKey = KeyCode.F9;

    public float spawnRadius = 5.0f;
    public float creationSpeed { get; set; }
    public float destructionSpeed { get; set; }
    [Range(0, 2)] public int levelCount;

    const int saveVersion = 2;

    private Shape tempObj;
    private Transform tempTrans;
    private List<Shape> shapes;
    private float creationProcess;
    private float destructionProcess;
    private int loadedLevelBuildIndex;
    private void Start()
    {
        shapes = new List<Shape>();
        if (Application.isEditor)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name.Contains("PersistenceExtra"))
                {
                    SceneManager.SetActiveScene(loadedScene);
                    loadedLevelBuildIndex = loadedScene.buildIndex;
                    return;
                }
            }
            
        }
        StartCoroutine(LoadLevel(levelCount));
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
        else
        {
            for (int i = 1; i <= levelCount; i++)
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    ResetLevel();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
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

        particles.PlayParticles(tempTrans.localPosition, tempTrans.localScale.x * 0.5f);
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
        writer.Write(loadedLevelBuildIndex);
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
        StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));
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

    private IEnumerator LoadLevel(int levelBuildIndex)
    {
        enabled = false;
        if (loadedLevelBuildIndex > 0)
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        loadedLevelBuildIndex = levelBuildIndex;
        enabled = true;
    }

}
