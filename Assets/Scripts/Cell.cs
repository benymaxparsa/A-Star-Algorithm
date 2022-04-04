using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int i,j;
    public CellType cellType = CellType.Path;
    public Material wall;
    public Material visited;
    public Material start;
    public Material end;
    public Material path;
    public Material none;

    public void SetCellType(CellType cellType)
    {
        this.cellType=cellType;
        Material tmp = none;
        switch (cellType)
        {
            case CellType.Start:
                tmp = start;
                break;
            case CellType.End:
                tmp = end;
                break;
            case CellType.Visited:
                tmp = visited;
                break;
            case CellType.Wall:
                tmp = wall;
                break;
            case CellType.Path:
                tmp = path;
                break;

        }
        GetComponent<Renderer>().material = tmp;
    }
}