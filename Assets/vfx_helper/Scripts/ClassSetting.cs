using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;


// para set classes
[System.Serializable]
public class ColorParaSet{
    public string para_name;
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

    public MaterialState(List<ColorParaSet> colorSets, List<FloatParaSet> floatSets)
    {
        this.ColorSets = colorSets;
        this.FloatSets = floatSets;
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
    public List<ColorParaSet> ColorSets;
    public List<FloatParaSet> FloatSets;

    public LightState(List<ColorParaSet> colorSets, List<FloatParaSet> floatSets)
    {
        this.ColorSets = colorSets;
        this.FloatSets = floatSets;
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

    public VFXProcessingState(float duration, AnimationCurve curve, VFXState fromState, VFXState toState)
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
            curve: AnimationCurve.Linear(0, 0, 1, 1),
        );
    }
}


[System.Serializable]
public class VFXTransition
{
    public VFXProcessingStateAsset processingStateAsset; // The processing state asset
    public VFXStateAsset fromStateAsset; // The "from" state asset
    public VFXStateAsset toStateAsset;   // The "to" state asset

    public VFXTransition(VFXProcessingStateAsset processingStateAsset, VFXStateAsset fromStateAsset, VFXStateAsset toStateAsset)
    {
        this.processingStateAsset = processingStateAsset;
        this.fromStateAsset = fromStateAsset;
        this.toStateAsset = toStateAsset;
    }
}
