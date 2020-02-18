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
        GCost = parent.GCost + Vector2.Distance(pos, parent.pos);
       // float dx = Mathf.Abs(pos.x - parent.pos.x);
       // float dy = Mathf.Abs(pos.y - parent.pos.y);
       // GCost = parent.GCost + (dx + dy) + (1 - 2) * Mathf.Min(dx, dy);
    }

    public void Calc_HCost(Vector2  final)                        //........................change to grid end...........................//
    {
        HCost = Vector2.Distance(pos, final);
        //float dx = Mathf.Abs(pos.x - final.x);
        //float dy = Mathf.Abs(pos.y - final.y);
       // HCost = (dx + dy) + (1 - 2) * Mathf.Min(dx, dy);
      // HCost = (dx + dy) + (1 - 2) * Mathf.Min(dx, dy);
    }

    public void Calc_FCost()
    {
        FCost = GCost + HCost;
    }
}
