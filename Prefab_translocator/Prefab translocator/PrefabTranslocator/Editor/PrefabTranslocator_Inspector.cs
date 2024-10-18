using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static PrefabTranslocater;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(PrefabTranslocater))]
public class PrefabTranslocator_Inspector : Editor
{
    PrefabTranslocater m_PrefabTranslocater;
    VisualElement myInspector;
    [SerializeField] VisualTreeAsset visualTree;
    
    public override VisualElement CreateInspectorGUI()
    {
        m_PrefabTranslocater = target as PrefabTranslocater;
        myInspector = new VisualElement();
         
        visualTree.CloneTree(myInspector);  
             
        HandleButtons(myInspector);      
        HandlePropperties(myInspector);      

        return myInspector;               
    }        
               
    private void OnEnable()            
    {  
        m_PrefabTranslocater = target as PrefabTranslocater;
         
        m_PrefabTranslocater.SelectedObjectClearEvent += OnSelectedObjectCleared;
    }

    private void OnDisable()
    {
        m_PrefabTranslocater = target as PrefabTranslocater;

        m_PrefabTranslocater.SelectedObjectClearEvent -= OnSelectedObjectCleared;   
    }

    void OnSelectedObjectCleared()
    {
        serializedObject.SetIsDifferentCacheDirty();
        myInspector.Q<PropertyField>("AllObjects").bindingPath = null;
        myInspector.Q<PropertyField>("AllObjects").bindingPath = "objectsToTransfer";
    }
     
    void HandleButtons(VisualElement myInspector)
    {
        Button findBut = myInspector.Q<Button>("FindPrefabs");
        findBut.clicked += m_PrefabTranslocater.FindObjects;

        Button translocateBut = myInspector.Q<Button>("TransLocateButton");
        translocateBut.clicked += m_PrefabTranslocater.TransLocate;
    }
     
    void HandlePropperties(VisualElement myInspector)
    {
        m_PrefabTranslocater = target as PrefabTranslocater;

        EnumField searchModeField = myInspector.Q<EnumField>("SearchMode");
        EnumField outputModeField = myInspector.Q<EnumField>("OutputMode");

        searchModeField.value = m_PrefabTranslocater.searchMode;
        outputModeField.value = m_PrefabTranslocater.outputMode;

        CheckSearchMode();
        CheckOutputMode();
         
        searchModeField.RegisterValueChangedCallback((x) =>
        {
            CheckSearchMode();
        });
        outputModeField.RegisterValueChangedCallback((x) =>
        {
            CheckOutputMode();
        });

        void CheckSearchMode()
        {
            if (searchModeField.value.ToString() == SearchModeEnum.PrefabSearch.ToString())
            {
                myInspector.Q<VisualElement>("PrefabSearchVisEl").style.display = DisplayStyle.Flex;
                myInspector.Q<VisualElement>("TextSearchVisEl").style.display = DisplayStyle.None;
            }
            else
            { 
                myInspector.Q<VisualElement>("PrefabSearchVisEl").style.display = DisplayStyle.None;
                myInspector.Q<VisualElement>("TextSearchVisEl").style.display = DisplayStyle.Flex;
            }
        } 

        void CheckOutputMode()
        {
            if(outputModeField.value.ToString() == OutputModeEnum.Both.ToString())
            {  
                myInspector.Q<VisualElement>("OutputTranslocateVisEl").style.display = DisplayStyle.Flex;
                myInspector.Q<VisualElement>("OutputReplaceVisEl").style.display = DisplayStyle.Flex;
                myInspector.Q<Button>("TransLocateButton").text = "Translocate and Replace objects!";
            }
            else if(outputModeField.value.ToString() == OutputModeEnum.Translocate.ToString())
            {  
                myInspector.Q<VisualElement>("OutputTranslocateVisEl").style.display = DisplayStyle.Flex;
                myInspector.Q<VisualElement>("OutputReplaceVisEl").style.display = DisplayStyle.None;
                myInspector.Q<Button>("TransLocateButton").text = "Translocate objects!";
            }
            else
            {
                myInspector.Q<VisualElement>("OutputTranslocateVisEl").style.display = DisplayStyle.None;
                myInspector.Q<VisualElement>("OutputReplaceVisEl").style.display = DisplayStyle.Flex;
                myInspector.Q<Button>("TransLocateButton").text = "Replace objects!";
            }
        }


    } 
}
  