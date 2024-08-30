
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;


public class VFX_Helper : MonoBehaviour
{
    public List<VFXTargetComponent> targetComponents;
    public List<VFXTransition> transitions;
    private bool isTransitioning = false; // Flag to track if a transition is in progress

    // Method to trigger a transition by name
    public void PlayTransition(string transitionName)
    {   
    
        if (isTransitioning)
        {
            Debug.LogWarning($"Transition request for '{transitionName}' blocked because another transition is already in progress.");
            return; // Block the request if a transition is already running
        }

        VFXTransition transition = transitions.Find(t => t.name == transitionName);
        if (transition != null)
        {   
            Debug.Log($"start transition '{transitionName}' ");
            List<ProcessingItem> processingQueue = BuildProcessingQueue(transition);
            StartCoroutine(PerformTransition(processingQueue, transition));
        }
        else
        {
            Debug.LogWarning($"Transition with name {transitionName} not found!");
        }
    }

    // Method to build the processing queue
    private List<ProcessingItem> BuildProcessingQueue(VFXTransition transition)
    {
        List<ProcessingItem> processingQueue = new List<ProcessingItem>();

        // Traverse the target components
        foreach (var target in targetComponents)
        {
            // Iterate through each RendererMaterialsPair
            foreach (var rendererMaterialsPair in target.rendererMaterialsPairs)
            {
                Renderer renderer = rendererMaterialsPair.targetRenderer;
                List<Material> materials = rendererMaterialsPair.targetMaterials;

                //Debug.Log($"Processing Renderer: {renderer.name}, Materials Count: {materials.Count}");

                // Iterate through each material in the pair
                foreach (var material in materials)
                {
                    //Debug.Log($"Processing Material: {material.name}");

                    if (transition.fromState != null && transition.toState != null)
                    {
                        // Traverse through all float parameters in fromState and toState
                        foreach (var fromFloat in transition.fromState.materialState.FloatSets)
                        {
                            foreach (var toFloat in transition.toState.materialState.FloatSets)
                            {
                                if (fromFloat.para_name == toFloat.para_name)
                                {
                                    //Debug.Log($"Adding Float Parameter: {fromFloat.para_name} to Queue for Material: {material.name}");
                                    processingQueue.Add(new ProcessingItem(fromFloat.para_name, ParameterType.Float, renderer, material));
                                }
                            }
                        }

                        // Traverse through all color parameters in fromState and toState
                        foreach (var fromColor in transition.fromState.materialState.ColorSets)
                        {
                            foreach (var toColor in transition.toState.materialState.ColorSets)
                            {
                                if (fromColor.para_name == toColor.para_name)
                                {
                                    //Debug.Log($"Adding Color Parameter: {fromColor.para_name} to Queue for Material: {material.name}");
                                    processingQueue.Add(new ProcessingItem(fromColor.para_name, ParameterType.Color, renderer, material));
                                }
                            }
                        }

                        // Traverse through all bool parameters in fromState and toState
                        foreach (var fromBool in transition.fromState.materialState.BoolSets)
                        {
                            foreach (var toBool in transition.toState.materialState.BoolSets)
                            {
                                if (fromBool.para_name == toBool.para_name)
                                {
                                    //Debug.Log($"Adding Bool Parameter: {fromBool.para_name} to Queue for Material: {material.name}");
                                    processingQueue.Add(new ProcessingItem(fromBool.para_name, ParameterType.Bool, renderer, material));
                                }
                            }
                        }
                    }
                }
            }
            // Handle Light Components
           
            foreach (var light in target.targetLights)
            {
                if (light != null && transition.fromState != null && transition.toState != null)
                {
                    // Process intensity (float)
                    processingQueue.Add(new ProcessingItem("intensity", ParameterType.Float, light));

                    // Process color (Color)
                    processingQueue.Add(new ProcessingItem("color", ParameterType.Color, light));

                    // Process range (float)
                    processingQueue.Add(new ProcessingItem("range", ParameterType.Float, light));
                }
            }
        }

        Debug.Log($"Total Processing Items in Queue: {processingQueue.Count}");
        return processingQueue;
    }


    
    // Coroutine to perform the transition over time
    private IEnumerator PerformTransition(List<ProcessingItem> processingQueue, VFXTransition transition)
    {
        //Debug.Log("PerformTransition started."); // Log when the transition starts

        isTransitioning = true; // Set the flag to indicate a transition is in progress
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch(); // Stopwatch to measure actual time
        stopwatch.Start();

        // Log the number of items in the processing queue
        //Debug.Log($"Processing Queue Count: {processingQueue.Count}");

        // Start processing coroutines for different types of items
        List<Coroutine> processingCoroutines = new List<Coroutine>();

        foreach (var item in processingQueue)
        {
            //Debug.Log($"Processing item: {item.parameterName} on Material: {item.targetMaterial.name} using Component: {item.targetComponent?.GetType().Name}");

            if (item.targetMaterial != null && item.targetComponent is Renderer)
            {
                //Debug.Log($"Starting ProcessMaterialItem coroutine for {item.parameterName} on {item.targetMaterial.name}");
                processingCoroutines.Add(StartCoroutine(ProcessMaterialItem(item, transition)));
            }
            else if (item.targetComponent != null)
            {
                if (item.targetComponent is Light)
                {
                    Debug.Log($"Starting ProcessLightItem coroutine for {item.parameterName}");
                    processingCoroutines.Add(StartCoroutine(ProcessLightItem(item, transition)));
                }
                else if (item.targetComponent is Transform)
                {
                    Debug.Log($"Starting ProcessTransformItem coroutine for {item.parameterName}");
                    processingCoroutines.Add(StartCoroutine(ProcessTransformItem(item, transition)));
                }
            }
        }

        // Wait until all coroutines are finished
        Debug.Log("Waiting for all processing coroutines to complete...");
        foreach (var coroutine in processingCoroutines)
        {
            yield return coroutine;
        }

        stopwatch.Stop();
        Debug.Log($"VFX Transition Complete. Actual time spent: {stopwatch.Elapsed.TotalSeconds} seconds. Expected duration: {transition.processingStateAsset.processingState.duration} seconds.");

        isTransitioning = false; // Reset the flag once the transition is complete
        Debug.Log("isTransitioning flag reset. Transition process complete.");
    }

    public bool CheckIsTransitioning(){
        return isTransitioning;
    }


    // processing each kinds

    private IEnumerator ProcessMaterialItem(ProcessingItem item, VFXTransition transition)
    {
        float elapsedTime = 0f;
        VFXProcessingState processingState = transition.processingStateAsset.processingState;

        Debug.Log($"Checking duration for transition. Duration: {processingState.duration} seconds");

        Renderer targetRenderer = item.targetComponent as Renderer;

        if (processingState.duration <= 0)
        {
            Debug.LogError($"Invalid duration: {processingState.duration}. Transition cannot proceed.");
            yield break;
        }

        if (targetRenderer != null)
        {
            Material material = item.targetMaterial;

            Debug.Log($"Item Target Material: {material.name} (Instance ID: {material.GetInstanceID()})");

            // Log all shared materials in the renderer
            Debug.Log($"Shared Materials in {targetRenderer.name}:");
            foreach (var sharedMat in targetRenderer.sharedMaterials)
            {
                Debug.Log($" - {sharedMat.name} (Instance ID: {sharedMat.GetInstanceID()})");
            }

            if (material != null && System.Array.Exists(targetRenderer.sharedMaterials, sharedMat => sharedMat == material))
            {
                Debug.Log($"Starting processing on material: {material.name} for parameter: {item.parameterName}");

                while (elapsedTime < processingState.duration)
                {
                    elapsedTime += Time.deltaTime;
                    float normalizedTime = Mathf.Clamp01(elapsedTime / processingState.duration);
                    float curveValue = processingState.transitionCurve.Evaluate(normalizedTime);

                    Debug.Log($"Processing {item.parameterName} on {material.name}. Elapsed Time: {elapsedTime}, Normalized Time: {normalizedTime}, Curve Value: {curveValue}");

                    if (item.parameterType == ParameterType.Float)
                    {
                        float fromValue = transition.fromState.materialState.FloatSets.Find(p => p.para_name == item.parameterName).float_para;
                        float toValue = transition.toState.materialState.FloatSets.Find(p => p.para_name == item.parameterName).float_para;
                        float newValue = Mathf.Lerp(fromValue, toValue, curveValue);
                        Debug.Log($"Setting float parameter '{item.parameterName}' from {fromValue} to {newValue}");
                        material.SetFloat(item.parameterName, newValue);
                    }
                    else if (item.parameterType == ParameterType.Color)
                    {
                        Color fromValue = transition.fromState.materialState.ColorSets.Find(p => p.para_name == item.parameterName).color_para;
                        Color toValue = transition.toState.materialState.ColorSets.Find(p => p.para_name == item.parameterName).color_para;
                        Color newValue = Color.Lerp(fromValue, toValue, curveValue);
                        Debug.Log($"Setting color parameter '{item.parameterName}' from {fromValue} to {newValue}");
                        material.SetColor(item.parameterName, newValue);
                    }
                    else if (item.parameterType == ParameterType.Bool)
                    {
                        bool fromValue = transition.fromState.materialState.BoolSets.Find(p => p.para_name == item.parameterName).bool_para;
                        bool toValue = transition.toState.materialState.BoolSets.Find(p => p.para_name == item.parameterName).bool_para;
                        bool newValue = curveValue > 0.5f ? toValue : fromValue;
                        Debug.Log($"Setting bool parameter '{item.parameterName}' to {newValue}");
                        material.SetFloat(item.parameterName, newValue ? 1.0f : 0.0f);
                    }

                    yield return null; // Continue on the next frame
                }

                Debug.Log($"Finished processing on material: {material.name} for parameter: {item.parameterName}");
            }
            else
            {
                Debug.LogWarning($"Material {material.name} not found in renderer {targetRenderer.name}");
            }
        }
        else
        {
            Debug.LogWarning($"Target component is not a Renderer or is null for parameter: {item.parameterName}");
        }
    }






    private IEnumerator ProcessLightItem(ProcessingItem item, VFXTransition transition)
    {
        float elapsedTime = 0f;
        VFXProcessingState processingState = transition.processingStateAsset.processingState;

        Light targetLight = item.targetComponent as Light;

        if (targetLight != null)
        {
            Debug.Log($"Starting processing on Light for parameter: {item.parameterName}");

            while (elapsedTime < processingState.duration)
            {
                elapsedTime += Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(elapsedTime / processingState.duration);
                float curveValue = processingState.transitionCurve.Evaluate(normalizedTime);

                if (item.parameterType == ParameterType.Float && item.parameterName == "intensity")
                {
                    float fromValue = transition.fromState.lightState.intensity;
                    float toValue = transition.toState.lightState.intensity;
                    float newValue = Mathf.Lerp(fromValue, toValue, curveValue);
                    Debug.Log($"Setting light intensity from {fromValue} to {newValue}");
                    targetLight.intensity = newValue;
                }
                else if (item.parameterType == ParameterType.Color && item.parameterName == "color")
                {
                    Color fromValue = transition.fromState.lightState.color;
                    Color toValue = transition.toState.lightState.color;
                    Color newValue = Color.Lerp(fromValue, toValue, curveValue);
                    Debug.Log($"Setting light color from {fromValue} to {newValue}");
                    targetLight.color = newValue;
                }
                else if (item.parameterType == ParameterType.Float && item.parameterName == "range")
                {
                    float fromValue = transition.fromState.lightState.range;
                    float toValue = transition.toState.lightState.range;
                    float newValue = Mathf.Lerp(fromValue, toValue, curveValue);
                    Debug.Log($"Setting light range from {fromValue} to {newValue}");
                    targetLight.range = newValue;
                }

                yield return null;
            }

            Debug.Log($"Finished processing on Light for parameter: {item.parameterName}");
        }
    }




    private IEnumerator ProcessTransformItem(ProcessingItem item, VFXTransition transition)
    {
        float elapsedTime = 0f;
        VFXProcessingState processingState = transition.processingStateAsset.processingState;
        Transform transformComponent = item.targetComponent as Transform;

        while (elapsedTime < processingState.duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / processingState.duration);
            float curveValue = processingState.transitionCurve.Evaluate(normalizedTime);

            // Process transform parameter changes (example for position)
            if (item.parameterType == ParameterType.Float)
            {
                // Handle float-specific processing if needed for Transform
            }
            else if (item.parameterType == ParameterType.Vector3)
            {
                Vector3 fromValue = transition.fromState.transformState.Vector3Sets.Find(p => p.para_name == item.parameterName).vector3_para;
                Vector3 toValue = transition.toState.transformState.Vector3Sets.Find(p => p.para_name == item.parameterName).vector3_para;
                Vector3 newValue = Vector3.Lerp(fromValue, toValue, curveValue);
                if (item.parameterName == "position")
                {
                    transformComponent.position = newValue;
                }
                else if (item.parameterName == "rotation")
                {
                    transformComponent.rotation = Quaternion.Euler(newValue);
                }
                else if (item.parameterName == "scale")
                {
                    transformComponent.localScale = newValue;
                }
            }

            yield return null; // Continue on the next frame
        }
    }



    // Method to process each item in the queue
    
}



public enum ParameterType
{
    Float,
    Color,
    Vector3,
    Bool
}

public class ProcessingItem
{
    public Component targetComponent; // The component to modify (e.g., Renderer, Light, etc.)
    public Material targetMaterial; // The material to modify (if applicable)
    public string parameterName; // The name of the parameter (e.g., "_Color", "Intensity", etc.)
    public ParameterType parameterType; // The type of the parameter (float, color, or bool)

    // Constructor for both Component and Material being optional
    public ProcessingItem( string parameterName = null, ParameterType parameterType = ParameterType.Float,Component targetComponent = null, Material targetMaterial = null)
    {
        this.targetComponent = targetComponent;
        this.targetMaterial = targetMaterial;
        this.parameterName = parameterName;
        this.parameterType = parameterType;
    }
}
