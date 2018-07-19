using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
    MeshRenderer meshRenderer;
    static MaterialPropertyBlock shaderPropertyBlock;
    static int colorPropertyId = Shader.PropertyToID("_Color");

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    int shapeId = int.MinValue;

    public int ShapeID
    {
        get { return shapeId; }

        set
        {
            if (shapeId == int.MinValue && value != int.MinValue)
                shapeId = value;
            else
                Debug.LogError("Not allowed to change shapeId.");
        }
    }
    public int MaterialId { get; private set; }

    public void SetMaterial(Material material, int materialId)
    {
        meshRenderer.material = material;
        MaterialId = materialId;
    }

    private Color color;

    public void SetColor(Color color)
    {
        this.color = color;
        if (shaderPropertyBlock == null)
            shaderPropertyBlock = new MaterialPropertyBlock();
        shaderPropertyBlock.SetColor(colorPropertyId, color);
        meshRenderer.SetPropertyBlock(shaderPropertyBlock);
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(color);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
    }

}
