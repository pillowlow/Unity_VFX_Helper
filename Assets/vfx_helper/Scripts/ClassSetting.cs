using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.VFX; // Required for VFX Graph


// para set classes
[System.Serializable]
public class ColorParaSet{
    public string para_name;
    [ColorUsage(true, true)] 
    public Color color_para;

    public ColorParaSet(string para_name, Color color_para){
        this.para_name = para_name;
        this.color_para = color_para;
    }
}

[System.Serializable]
public class FloatParaSet
{
    public string para_name;
    public float float_para;

    public FloatParaSet(string para_name, float float_para)
    {
        this.para_name = para_name;
        this.float_para = float_para;
    }
}

[System.Serializable]
public class Vector3ParaSet
{
    public string para_name;
    public Vector3 vector3_para;

    public Vector3ParaSet(string para_name, Vector3 vector3_para)
    {
        this.para_name = para_name;
        this.vector3_para = vector3_para;
    }
}
[System.Serializable]
public class BoolParaSet
{
    public string para_name;
    public bool bool_para;

    public BoolParaSet(string para_name, bool bool_para)
    {
        this.para_name = para_name;
        this.bool_para = bool_para;
    }
}
// states

[System.Serializable]
public class MaterialState
{   
    public List<ColorParaSet> ColorSets;
    public List<FloatParaSet> FloatSets;
    public List<BoolParaSet> BoolSets;

    public MaterialState(List<ColorParaSet> colorSets, List<FloatParaSet> floatSets,List<BoolParaSet> boolSets)
    {
        this.ColorSets = colorSets;
        this.FloatSets = floatSets;
        this.BoolSets = boolSets;
    }
}


[System.Serializable]
public class VFXGraphState
{
    public List<ColorParaSet> ColorSets;
    public List<FloatParaSet> FloatSets;
    public List<BoolParaSet> BoolSets;

    public VFXGraphState(List<ColorParaSet> colorSets, List<FloatParaSet> floatSets, List<BoolParaSet> boolSets)
    {
        this.ColorSets = colorSets;
        this.FloatSets = floatSets;
        this.BoolSets = boolSets;
    }
}

[System.Serializable]
public class ParticleSystemState
{
    public List<FloatParaSet> FloatSets;
    public List<BoolParaSet> BoolSets;

    public ParticleSystemState(List<FloatParaSet> floatSets, List<BoolParaSet> boolSets)
    {
        this.FloatSets = floatSets;
        this.BoolSets = boolSets;
    }
}

[System.Serializable]
public class TransformState
{
    public List<Vector3ParaSet> Vector3Sets;
    public List<BoolParaSet> BoolSets;

    public TransformState(List<Vector3ParaSet> vector3Sets, List<BoolParaSet> boolSets)
    {
        this.Vector3Sets = vector3Sets;
        this.BoolSets = boolSets;
    }
}



[System.Serializable]
public class LightState
{
    public Color color;
    public float intensity;
    public float range;  // Add range property

    public LightState(Color color, float intensity, float range)
    {
        this.color = color;
        this.intensity = intensity;
        this.range = range;
    }
}

// showing state classes


[System.Serializable]
public class VFXState
{
    public MaterialState materialState;
    public VFXGraphState vfxGraphState;
    public ParticleSystemState particleSystemState;
    public TransformState transformState;
    public LightState lightState;
   

    public VFXState(
        MaterialState materialState, 
        VFXGraphState vfxGraphState, 
        ParticleSystemState particleSystemState, 
        TransformState transformState, 
        LightState lightState
    )
    {
        this.materialState = materialState;
        this.vfxGraphState = vfxGraphState;
        this.particleSystemState = particleSystemState;
        this.transformState = transformState;
        this.lightState = lightState;
        
    }
}




[CreateAssetMenu(fileName = "NewVFXState", menuName = "VFXHelper/VFXStateAsset")]
public class VFXStateAsset : ScriptableObject
{   
    public MaterialState materialState;
    public VFXGraphState vfxGraphState;
    public ParticleSystemState particleSystemState;
    public TransformState transformState;
    public LightState lightState;
    

}

[System.Serializable]
public class VFXProcessingState
{
    public float duration; // Duration of the transition
    public AnimationCurve transitionCurve; // Curve to control the transition

    public VFXProcessingState(float duration, AnimationCurve curve)
    {
        this.duration = duration;
        this.transitionCurve = curve;
    }
}


[CreateAssetMenu(fileName = "NewVFXProcessingState", menuName = "VFXHelper/VFXProcessingStateAsset")]
public class VFXProcessingStateAsset : ScriptableObject
{
    public VFXProcessingState processingState;

    // You can initialize the processing state in the constructor or through the Inspector
    public VFXProcessingStateAsset()
    {
        // Default values for the processing state
        processingState = new VFXProcessingState(
            duration: 1.0f,
            curve: AnimationCurve.Linear(0, 0, 1, 1)
        );
    }
}


[System.Serializable]
public class VFXTransition
{   
    public string name;
    public VFXProcessingStateAsset processingStateAsset; // The processing state asset
    public VFXStateAsset fromState; // The "from" state asset
    public VFXStateAsset toState;   // The "to" state asset

    public VFXTransition(string name, VFXProcessingStateAsset processingStateAsset, VFXStateAsset fromState, VFXStateAsset toState)
    {   
        this.name = name;
        this.processingStateAsset = processingStateAsset;
        this.fromState = fromState;
        this.toState = toState;
    }
}

[System.Serializable]
public class RendererMaterialsPair
{
    public Renderer targetRenderer; // The Renderer component
    public List<Material> targetMaterials; // The list of materials associated with the Renderer

    public RendererMaterialsPair(Renderer renderer, List<Material> materials)
    {
        this.targetRenderer = renderer;
        this.targetMaterials = materials;
    }
}

[System.Serializable]
public class VFXTargetComponent
{
    public string name; // A name to identify this component reference group

    // List of Renderer and Material pairs
    public List<RendererMaterialsPair> rendererMaterialsPairs;

    public List<Transform> targetTransforms; // List of Transform components
    public List<Light> targetLights; // List of Light components
    public List<ParticleSystem> targetParticleSystems; // List of Particle Systems
    public List<VisualEffect> targetVFXGraphs; // List of VFX Graph components

    public VFXTargetComponent(string name)
    {
        this.name = name;
        this.rendererMaterialsPairs = new List<RendererMaterialsPair>();
        this.targetTransforms = new List<Transform>();
        this.targetLights = new List<Light>();
        this.targetParticleSystems = new List<ParticleSystem>();
        this.targetVFXGraphs = new List<VisualEffect>();
    }
}