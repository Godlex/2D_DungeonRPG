using Enum;
using UnityEngine;

[RequireComponent(typeof(Cell))]
public class CellController : MonoBehaviour
{
    private Cell _cell;

    private void Awake()
    {
        _cell = GetComponent<Cell>();
    }

    private void OnMouseEnter()
    {
        if (_cell.CurrentState == CellStateType.SELECTED)
        {
            return;
        }
        _cell.SetState(CellStateType.HOVER);
    }

    private void OnMouseExit()
    {
        if (_cell.CurrentState == CellStateType.SELECTED)
        {
            return;
        }
        _cell.SetState(CellStateType.STANDART);
    }

    private void OnMouseDown()
    {
        if (_cell.CurrentState == CellStateType.SELECTED)
        {
            _cell.SetState(CellStateType.HOVER);
            return;
        }
        _cell.SetState(CellStateType.SELECTED);
    }
}
