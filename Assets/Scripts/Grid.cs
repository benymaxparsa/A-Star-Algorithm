using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int size;
    public Node start;
    public Node end;

    private bool[,] walls;

    List<Node> visited;
    List<Node> option;

    Node[,] matrix;

    private void Start()
    {
        matrix = new Node[size, size];
        walls = new bool[size, size];
        walls[size/2, size/2] = true;
    }

    public void Created() 
    {

    }
}
