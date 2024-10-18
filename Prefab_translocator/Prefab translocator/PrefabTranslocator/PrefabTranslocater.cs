#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using static Unity.VisualScripting.Metadata;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEditor.Progress;
 
public class PrefabTranslocater : MonoBehaviour
{
    public enum SearchModeEnum{PrefabSearch,TextSearch };
    public SearchModeEnum searchMode = SearchModeEnum.PrefabSearch;

    public enum OutputModeEnum { Translocate,Replace,Both };
    public OutputModeEnum outputMode = OutputModeEnum.Translocate;

    [SerializeField] GameObject parentGameObject;
    [SerializeField] GameObject prefab;
    [SerializeField] string searchName;

    public List<GameObject> objectsToTransfer = new List<GameObject>();
    GameObject[] allObjects;
      
    [SerializeField] GameObject targetObject;
    [SerializeField] GameObject replacePrefab;

    [SerializeField] string helpText;

    public event Action SelectedObjectClearEvent;

    public void FindObjects()
    {
        helpText = @"";
        objectsToTransfer.Clear();

        if (searchMode == SearchModeEnum.PrefabSearch)
        {
            if(prefab != null)
            {
                allObjects = PrefabUtility.FindAllInstancesOfPrefab(prefab);

                if (parentGameObject == null)
                {
                    foreach (GameObject obj in allObjects)
                    {
                        objectsToTransfer.Add(obj);
                    }
                }
                else
                {
                    foreach (GameObject obj in allObjects)
                    {
                        if (obj.transform.IsChildOf(parentGameObject.transform))
                        {
                            objectsToTransfer.Add(obj);
                        }
                    }
                }
            }
            else
            {
                helpText = @"Please add a prefab to search for!";
            }
        }
        else 
        {
            if(searchName == null || searchName == "")
            {
                helpText = @"Add some text to search for!";
            }
            else
            {
                if (searchName != null)
                {
                    allObjects = FindObjectsOfType<GameObject>();

                    if (parentGameObject == null)
                    {
                        foreach (GameObject obj in allObjects)
                        {
                            if (obj.name.Contains(searchName))
                            { 
                                objectsToTransfer.Add(obj);
                            } 
                        }
                    }
                    else 
                    {
                        foreach (GameObject obj in allObjects)
                        {
                            if (obj.transform.IsChildOf(parentGameObject.transform))
                            {
                                if (obj.name.Contains(searchName))
                                {
                                    objectsToTransfer.Add(obj);
                                }
                            }
                        }
                    }
                }
                else
                {
                    helpText = @"Please input text into the text field.";
                }
            }
        }
    }

    public void TransLocate()
    {
        if (objectsToTransfer.Count <= 0)
        {
            helpText = "Please search for some objects first";
        }
        else if (outputMode == OutputModeEnum.Translocate && targetObject == null || outputMode == OutputModeEnum.Both && targetObject == null)
        {
            helpText = @"Please select a target object to translocate to";
        }
        else if (outputMode == OutputModeEnum.Replace && replacePrefab == null || outputMode == OutputModeEnum.Both && replacePrefab == null)
        {
            helpText = @"Please select a prefab to replace the objects with";
        }
        else
        {



            Undo.IncrementCurrentGroup();
            int undoGroupIndex = Undo.GetCurrentGroup();

            if (outputMode == OutputModeEnum.Translocate)
            {
                foreach (GameObject obj in objectsToTransfer)
                {
                    Undo.SetTransformParent(obj.transform, targetObject.transform, "Translocate object");
                }

                objectsToTransfer.Clear();
                SelectedObjectClearEvent?.Invoke();
            }
            else
            {
                for (int i = 0; i < objectsToTransfer.Count; i++)
                {
                    GameObject obj = objectsToTransfer[i];

                    GameObject temp = PrefabUtility.InstantiatePrefab(replacePrefab) as GameObject;
                    Undo.RegisterCreatedObjectUndo(temp, "Replace Objects");

                    if (outputMode == OutputModeEnum.Both)
                    {
                        Undo.SetTransformParent(temp.transform, targetObject.transform, "Translocate object");
                    }
                    else
                    {
                        Undo.SetTransformParent(temp.transform, obj.transform.parent, "Translocate object");
                    }

                    temp.transform.position = obj.transform.position;
                    temp.transform.rotation = obj.transform.rotation;
                    temp.transform.localScale = obj.transform.lossyScale;

                    Undo.DestroyObjectImmediate(obj);
                }

                objectsToTransfer.Clear();
                SelectedObjectClearEvent?.Invoke();
            }

            Undo.CollapseUndoOperations(undoGroupIndex);
        }
    }
}

#endif