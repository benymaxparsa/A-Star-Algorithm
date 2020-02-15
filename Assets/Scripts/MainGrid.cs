using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGrid : MonoBehaviour
{
    public int size;
    public Node begin;
    public Node end;
    public float scale = 1;

    public Transform player;

    private bool[,] walls;

    List<Node> visited;
    List<Node> option;

    Node[,] matrix;

    private void Start()
    {
        matrix = new Node[size, size];
        walls = new bool[size, size];
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                matrix[i,j] = InitializeNode(i, j);
            }
        }
    }

    private void PlayerPos()
    {

    }

    private void EnemyPos()
    {

    }

    private Node InitializeNode(int x, int y)
    {
        Node temp = new Node();
        temp.pos.x = x / scale;
        temp.pos.y = y / scale;
        return temp;
    }

}
