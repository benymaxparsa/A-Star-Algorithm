using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int i,j;
    public Material wall;
    public Material visited;
    public Material start;
    public Material end;
    public Material none;

    /// <param name="option">/// <summary>
    /// this will be the tooltip
    /// </summary>
    /// <param name="args">args will be passed when starting this program</param></param>

    public void SetMaterial(int  option)
    {
        Material tmp = none;
        switch (option)
        {
            case 0:
                tmp = start;
                break;
            case 1:
                tmp = end;
                break;
            case 2:
                tmp = visited;
                break;
            case 3:
                tmp = wall;
                break;
        }
        GetComponent<Renderer>().material = tmp;
    }
}