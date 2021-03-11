using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enum;
using UnityEngine;
using UnityEngine.Serialization;

public class Cell : MonoBehaviour
{
   

    [FormerlySerializedAs("_colorByStates")] [SerializeField] private List<ColorByState> colorByStates;
    
    [SerializeField] private CellStateType currentState;

    public CellStateType CurrentState => currentState;
    
    private readonly Dictionary<CellStateType, ColorByState> _colorByStatesDictionary =
        new Dictionary<CellStateType, ColorByState>();

    private MeshRenderer _meshRenderer;

    
    
    [Serializable]
    private struct ColorByState
    {
        [FormerlySerializedAs("StateType")] public CellStateType stateType;
        [FormerlySerializedAs("Color")] public Color color;
    }

    private void Awake()
    {
        InitColorsByStateDictionary();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void SetColorByState(CellStateType state)
    {
        var material = _meshRenderer.materials.First();
        material.SetColor("_Color", _colorByStatesDictionary[state].color);
    }

    public void SetState(CellStateType state)
    {
        SetColorByState(state);
        currentState = state;
    }
    
    private void InitColorsByStateDictionary()
    {
        foreach (var colorByState in colorByStates)
        {
            _colorByStatesDictionary.Add(colorByState.stateType, colorByState);
        }
    }
}