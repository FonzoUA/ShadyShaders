using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShapeFactory : ScriptableObject
{
    [SerializeField] Shape[] prefabs;
    [SerializeField] Material[] materials;
    [SerializeField] bool recycle;

    List<Shape>[] pools;

    private void CreatePools()
    {
        pools = new List<Shape>[prefabs.Length];
        for (int i = 0; i < pools.Length; i++)
            pools[i] = new List<Shape>();
    }

    public Shape Get (int shapeId = 0, int materialId = 0)
    {
        if (recycle)
        {
            if (pools == null)
                CreatePools();
            List<Shape> pool = pools[shapeId];
            int lastIndex = pool.Count - 1;
            if (lastIndex >= 0)
            {
                Shape instance = pool[lastIndex];
                instance.gameObject.SetActive(true);
                pool.RemoveAt(lastIndex);
                return instance;
            }
            else
                return InstanciateNewShape(shapeId, materialId);
        }
        else
            return InstanciateNewShape(shapeId, materialId);
    }

    public void ReturnShape(Shape recycledShape)
    {
        if (recycle)
        {
            if (pools == null)
                CreatePools();
            pools[recycledShape.ShapeID].Add(recycledShape);
            recycledShape.gameObject.SetActive(false);
        }
        else
            Destroy(recycledShape.gameObject);
    }

    private Shape InstanciateNewShape(int shapeId, int materialId)
    {
        Shape instance = Instantiate(prefabs[shapeId]);
        instance.ShapeID = shapeId;
        instance.SetMaterial(materials[materialId], materialId);
        return instance;
    }

    public Shape GetRandom()
    {
        return Get(Random.Range(0, prefabs.Length), Random.Range(0, materials.Length));
    }


}
