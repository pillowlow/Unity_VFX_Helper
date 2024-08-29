using UnityEngine;

[System.Serializable]
public class MaterialState
{
    public Color color;
    public float floatValue;

    public MaterialState(Color color, float floatValue)
    {
        this.color = color;
        this.floatValue = floatValue;
    }
}


[System.Serializable]
public class VFXGraphState
{
    public Color vfxColor;
    public float vfxIntensity;

    public VFXGraphState(Color vfxColor, float vfxIntensity)
    {
        this.vfxColor = vfxColor;
        this.vfxIntensity = vfxIntensity;
    }
}



[System.Serializable]
public class ParticleSystemState
{
    public float startSize;
    public float startSpeed;
    public float startLifetime;

    public ParticleSystemState(float startSize, float startSpeed, float startLifetime)
    {
        this.startSize = startSize;
        this.startSpeed = startSpeed;
        this.startLifetime = startLifetime;
    }
}

[System.Serializable]
public class TransformState
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public TransformState(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }
}



[System.Serializable]
public class LightState
{
    public Color color;
    public float intensity;

    public LightState(Color color, float intensity)
    {
        this.color = color;
        this.intensity = intensity;
    }
}

