using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float GCost;  // distance from start Node
    public float HCost;  // distance from end Node
    public float FCost;  // H + G cost

    public bool visited = false, option = false;

    public Vector2 pos;

    public Node parent;   //ref

    public bool isWall = false;

    public void Calc_GCost()
    {
        GCost = parent.GCost + Vector2.Distance(pos, parent.pos) ; 
    }

    public void Calc_HCost(Vector2  final)                        //........................change to grid end...........................//
    {
        GCost =  Vector2.Distance(pos, final);
    }

    public void Calc_FCost()
    {
        FCost = GCost + HCost;
    }
}
