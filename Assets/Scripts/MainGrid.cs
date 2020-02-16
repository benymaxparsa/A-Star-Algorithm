using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MainGrid : MonoBehaviour
{
    public int size;
    public float scale = 1;
    private float Xoffset;
    private float Yoffset;

    private Node begin;
    private Node end;

    List<Node> visited = new List<Node>();
    List<Node> option = new List<Node>();

    public GameObject plane;
    public Transform player;

    private bool[,] walls;

    Node[,] matrix;

    private void Start()
    {
        matrix = new Node[size, size];
        walls = new bool[size, size];
        GenerateGrid();
        end = new Node();

        begin = new Node()
        {
            pos = new Vector2(9, 9),
            visited = true,
        };

        begin.GCost = 0 ;
        begin.Calc_HCost(new Vector2(0,0));
        begin.Calc_FCost();

        matrix[9, 9] = begin;

        option.Add(begin);
    }

    private void Update()
    {
        Debug.Log(begin.pos);
        Debug.Log(end.pos);

        if (Input.GetKeyDown(KeyCode.E)) 
        {
            end.pos = PlayerPos();
            Debug.Log(option[0]);

            option.OrderBy(x => x.FCost);
            Debug.Log(option[0]);

            FindPath();
        }
    }

    private void FindPath()
    {
        if (option[0].pos == end.pos)
        {
            Debug.Log("FOUND");
            
        }
        else
        {
            Debug.Log(option[0].pos);
            option[0].visited = true;
            CalcNeighbour(option[0]);
           // option.Remove(option[0]);
        }

    }

    private void CalcNeighbour(Node curr)
    {

        int[] dir = new int[16] { -1,1  ,   0,1  ,    1,1   ,
                                  0,-1       ,        0,1   ,
                                 -1,-1  ,   0,-1  ,   1,-1   };
        int Px, Py,x,y;
        Px = (int)curr.pos.x;
        Py = (int)curr.pos.y;

        for (int i = 0; i < 16; i+=2)
        {
            x = Px;
            y = Py;
            x += i;
            y += i + 1;
            if (x>=0 && x<size && y >= 0 && y < size)
            {
                Node temp = matrix[x, y];
                if (!temp.isWall && !temp.visited)
                {
                    temp.parent = curr;
                    temp.Calc_GCost();
                    temp.Calc_HCost(end.pos);
                    temp.Calc_FCost();
                    option.Add(temp);
                }
            }
        }
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
        

       if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log( ((int)((player.position.x + Xoffset*10 )/ 10) ).ToString() + " " + ((int)(player.position.z + Yoffset*10) / 10).ToString());
        }

        return new Vector2((int)((player.position.x + Xoffset * 10) / 10)  ,((int)(player.position.z + Yoffset * 10) / 10));
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

   /* private void OnDrawGizmos()
    {

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(new Vector3((end.pos.x - Xoffset) * 10 + 5, 0, (end.pos.y - Yoffset) * 10 + 5), new Vector3(10, 10, 10));
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Gizmos.DrawWireCube(new Vector3((i - Xoffset)*10 + 5,0, (j - Yoffset)*10 +5) , new Vector3(10, 0.1f, 10));
            }
        }
    }*/
}
