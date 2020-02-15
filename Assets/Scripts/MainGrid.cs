using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGrid : MonoBehaviour
{
    public int size;
    public float scale = 1;
    private float Xoffset;
    private float Yoffset;

    public Node begin;
    public Node end;

    public GameObject plane;
    public Transform player;

    private bool[,] walls;

    List<Node> visited;
    List<Node> option;

    Node[,] matrix;

    private void Start()
    {
        matrix = new Node[size, size];
        walls = new bool[size, size];
        GenerateGrid();
    }

    private void Update()
    {
        PlayerPos();
    }

    private void GenerateGrid()
    {
        GameObject p = Instantiate(plane, Vector3.zero, Quaternion.identity);

        p.transform.localScale = new Vector3(size, size, size);

        Xoffset = p.transform.localScale.x / 2 ;
        Yoffset = p.transform.localScale.z / 2;
        
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                matrix[i,j] = InitializeNode(i, j);
            }
        }
    player.position = new Vector3(p.transform.position.x - Xoffset, 1,p.transform.position.z - Yoffset) * 10;
    }
    private Vector2 PlayerPos()
    {

        return new Vector2((((int)player.position.x / size + (size / 2))),(((int)player.position.z / size + (size / 2))));

       /* if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log((((int)player.position.x/size + (size/2))).ToString() + "  " + (((int)player.position.z / size + (size / 2))).ToString());
        }*/
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Gizmos.DrawWireCube(new Vector3(i*(- Xoffset *10)+size/2, 0,j*(  -Yoffset *10) + size / 2) , new Vector3(size, 0.1f, size));
            }
        }
    }
}
