using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SearchService;
using UnityEngine.Serialization;

public class Node
{
    public float GCost;  // distance from start Node
    public float HCost;  // distance from end Node
    public float FCost;  // H + G cost

    public bool Visited = false, Option = false;

    public Vector2Int Pos;

    public Node Parent=null;   //ref

    public bool IsWall = false;

    public void Reset()
    {
        Parent = null;
        Visited = false;
        Option = false;
    }
    public void Set(Node other)
    {
      GCost =other.GCost ;  
      HCost =other.HCost ;  
      FCost =other.FCost ;

      Visited =other.Visited  ; 
      Option = other.Option ;

     Pos = other.Pos;

     Parent = other.Parent;

     IsWall = other.IsWall;
    }
    public void Calc_GCost()
    {
        GCost = Parent.GCost + Vector2Int.Distance(Pos, Parent.Pos);
       // float dx = Mathf.Abs(pos.x - parent.pos.x);
       // float dy = Mathf.Abs(pos.y - parent.pos.y);
       // GCost = parent.GCost + (dx + dy) + (1 - 2) * Mathf.Min(dx, dy);
    }

    public void Calc_HCost(Vector2Int  final) //........................change to grid end...........................//
    {
        HCost = Vector2Int.Distance(Pos, final);
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
