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

    private Node Enemy;
    private Node Target;

    List<Node> visited = new List<Node>();
    List<Node> options = new List<Node>();

    public GameObject cube;
    public GameObject wall;     /// <summary>
    /// //hazf
    /// </summary>

    public GameObject plane;
    public Transform player;

    private bool[,] walls;

    Node[,] Grid;

    private void Start()
    {
        Grid = new Node[size, size];
        walls = new bool[size, size];
        GenerateGrid();

        Target = Grid[0, 0];
        Target.pos = new Vector2(0, 0);
        Target.HCost = 0;
        Grid[0, 0] = Target;

        Enemy = Grid[9, 9];
        Enemy.pos = new Vector2(9, 9);
        Enemy.parent = null;
        Enemy.visited = true;
        Enemy.GCost = 0;
        Enemy.Calc_HCost(Target.pos);
        Enemy.Calc_FCost();
        Grid[9, 9] = Enemy;
        visited.Add(Enemy);

        CalcNeighbour(Enemy);
        options= options.OrderBy(x => x.FCost).ToList();
        Grid[4, 4].isWall = true;
        Instantiate(wall, new Vector3((4 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);


        Grid[4, 5].isWall = true;
        Instantiate(wall, new Vector3((4 - Xoffset) * 10 + 5, 0, (5 - Yoffset) * 10 + 5), Quaternion.identity);

        Grid[4, 6].isWall = true;
        Instantiate(wall, new Vector3((4 - Xoffset) * 10 + 5, 0, (6 - Yoffset) * 10 + 5), Quaternion.identity);

        Grid[4, 7].isWall = true;
        Instantiate(wall, new Vector3((4 - Xoffset) * 10 + 5, 0, (7 - Yoffset) * 10 + 5), Quaternion.identity);

        Grid[4, 8].isWall = true;
        Instantiate(wall, new Vector3((4 - Xoffset) * 10 + 5, 0, (8 - Yoffset) * 10 + 5), Quaternion.identity);

        Grid[5, 4].isWall = true;
        Instantiate(wall, new Vector3((5 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);

        Grid[6, 4].isWall = true;
        Instantiate(wall, new Vector3((6 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);


        Instantiate(cube, new Vector3((9 - Xoffset) * 10 + 5, 0, (9- Yoffset) * 10 + 5), Quaternion.identity);


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {

            Node current;
            current = options[0];


            Instantiate(cube, new Vector3((current.pos.x - Xoffset) * 10 + 5, 0, (current.pos.y - Yoffset) * 10 + 5), Quaternion.identity);


            current.visited = true;
            options.Remove(options[0]);
            visited.Add(current);
            Grid[(int)current.pos.x, (int)current.pos.y] = current;
            if (current.pos == Target.pos)
            {
                Instantiate(cube, new Vector3((current.pos.x - Xoffset) * 10 + 5, 0, (current.pos.y - Yoffset) * 10 + 5), Quaternion.identity);
                Debug.Log("FOUND");
                return;
            }
            CalcNeighbour(current);
            options = options.OrderBy(x => x.FCost).ToList();
        }
    }

    private void FindPath()
    {
        
  

    }

    private void CalcNeighbour(Node curr)
    {

        int[] dir = new int[16] { -1,1  ,   0,1  ,    1,1   ,
                                  -1,0       ,        1,0   ,
                                 -1,-1  ,   0,-1  ,   1,-1   };
        int Px, Py,x,y;
        Px = (int)curr.pos.x;
        Py = (int)curr.pos.y;

        for (int i = 0; i < 16; i+=2)
        {
            x = Px;
            y = Py;
            x += dir[i];
            y += dir[i+1];
            if (x>=0 && x<size && y >= 0 && y < size)
            {
                Node neig = Grid[x, y];
                if (!neig.isWall && !neig.visited)
                {
                    if (!neig.option)
                    {
                        options.Add(neig);
                        neig.option = true;
                        neig.parent = curr;
                    }
                    else if (neig.option)
                    {
                        Node tmp = neig;
                        tmp.parent = curr;
                        tmp.Calc_GCost();
                        if (tmp.GCost < neig.GCost )
                        {
                            neig = tmp;
                        } 
                    }
                    neig.Calc_GCost();
                    neig.Calc_HCost(Target.pos);
                    neig.Calc_FCost();

                    Grid[x, y] = neig;
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
                Grid[i,j] = InitializeNode(i, j);
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
        temp.pos.x = x ;
        temp.pos.y = y ;
        return temp;
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(new Vector3((Target.pos.x - Xoffset) * 10 + 5, 0, (Target.pos.y - Yoffset) * 10 + 5), new Vector3(10, 10, 10));
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Gizmos.DrawWireCube(new Vector3((i - Xoffset)*10 + 5,0, (j - Yoffset)*10 +5) , new Vector3(10, 0.1f, 10));
            }
        }
    }
}
