using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MainGrid : MonoBehaviour
{
    /// <summary>
    /// distance to move
    /// </summary>
    /// 
    [Range(0, 2)]
    public float distance; 
    public int size;
    public float scale = 1;
    private float Xoffset;
    private float Yoffset;

    private Node Enemy;
    private Node Target;
    private Node current;

    Stack<Node> Path;

    List<Node> visited = new List<Node>();
    List<Node> options = new List<Node>();

    public GameObject cube;
    public GameObject wall;     /// <summary>
    /// //hazf
    /// </summary>

    public GameObject plane;
    public Transform player;
    public Transform enemy;

    private bool[,] walls;
    private bool Found = false;

    Node[,] Grid;

    public Vector2Int WorldPos_To_GridIndex(Transform pos)
    {
        return new Vector2Int((int)((pos.position.x + Xoffset * 10) / 10), ((int)(pos.position.z + Yoffset * 10) / 10));
    }

    public Vector3 GridIndex_To_WorldPos(int x, int y)
    {
        return new Vector3((x - Xoffset) * 10 +5, 0, (y - Yoffset) * 10 + 5);
    }

    public Node getMinCost()
    {
        if (options==null)
            return null;
        Node temp = options[0];

        foreach (var item in options)
        {
            if (temp.FCost > item.FCost)
            {
                temp = item;
            }
           else if (temp.FCost == item.FCost && temp.HCost > item.HCost)
            {
                temp = item;
            }
        }
       // options.Remove(options[0]);
        return temp;
    }

    private void Start()
    {
        Grid = new Node[size, size];
        walls = new bool[size, size];
        GenerateGrid();

        Path = new Stack<Node>();

        Target = Grid[0, 0];
        Target.pos = new Vector2(0, 0);
        Target.HCost = 0;
        Grid[0, 0] = Target;

        Enemy = Grid[19, 19];
        Enemy.pos = new Vector2(19, 19);
        Enemy.parent = null;
        Enemy.visited = true;
        Enemy.GCost = 0;
        Enemy.Calc_HCost(Target.pos);
        Enemy.Calc_FCost();
        Grid[19, 19] = Enemy;
        visited.Add(Enemy);

        CalcNeighbour(Enemy);
       // options= options.OrderBy(x => x.FCost).ToList();
        Grid[4, 4].isWall = true;
        Instantiate(wall, GridIndex_To_WorldPos(4, 4), Quaternion.identity);


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
        Grid[9, 4].isWall = true;
        Instantiate(wall, new Vector3((9 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);
                                                                  
        Grid[10,4].isWall = true;                                 
        Instantiate(wall, new Vector3((10 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);
        Grid[11,4].isWall = true;                                 
        Instantiate(wall, new Vector3((11 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);
        Grid[12,4].isWall = true;                                 
        Instantiate(wall, new Vector3((12 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);
        Grid[13,4].isWall = true;                                 
        Instantiate(wall, new Vector3((13 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);
        Grid[14,4].isWall = true;                                 
        Instantiate(wall, new Vector3((14 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);
        Grid[15,4].isWall = true;                                 
        Instantiate(wall, new Vector3((15 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);
        Grid[16,4].isWall = true;                                 
        Instantiate(wall, new Vector3((16 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);
        Grid[17,4].isWall = true;                                 
        Instantiate(wall, new Vector3((17 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);
        Grid[18,4].isWall = true;                                 
        Instantiate(wall, new Vector3((18 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);
        Grid[19,4].isWall = true;                                 
        Instantiate(wall, new Vector3((19 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);
        Instantiate(cube, new Vector3((9 - Xoffset) * 10 + 5, 0, (9- Yoffset) * 10 + 5), Quaternion.identity);
        Grid[7, 4].isWall = true;
        Instantiate(wall, new Vector3((7 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);

        Grid[8, 4].isWall = true;
        Instantiate(wall, new Vector3((8 - Xoffset) * 10 + 5, 0, (4 - Yoffset) * 10 + 5), Quaternion.identity);

    }

    private void Update()
    {
       if (!Found)
        {

            current = getMinCost();
            

            current.visited = true;

           int index =  options.FindIndex(x => x.visited);
        options.RemoveAt(index);
            visited.Add(current);
            Grid[(int)current.pos.x, (int)current.pos.y] = current;
            Instantiate(cube, new Vector3((current.pos.x - Xoffset) * 10 + 5, 0, (current.pos.y - Yoffset) * 10 + 5), Quaternion.identity);
            if (current.pos == Target.pos)
            {
                Instantiate(cube, new Vector3((current.pos.x - Xoffset) * 10 + 5, 0, (current.pos.y - Yoffset) * 10 + 5), Quaternion.identity);
                Found = true;

                Node pointer = current;
                while (pointer.pos != Enemy.pos)
                {
                    Path.Push(pointer);
                    pointer = pointer.parent;
                }
                Path.Push(Enemy);


                //while (Path.Count>0)
                //{
                //    Debug.Log(GridIndex_To_WorldPos((int)Path.Peek().pos.x, (int)Path.Peek().pos.y));
                //    Path.Pop();
                //}


                Debug.Log("FOUND");
            }
            CalcNeighbour(current);
            // options = options.OrderBy(x => x.HCost).ToList();
            // options = options.OrderBy(x => x.FCost).ToList();
            
            for (int i = 0; i < options.Count; i++)
            {
                //Debug.Log(options[i].pos.ToString() + " F = " + options[i].FCost.ToString()+ " H = "+ options[i].HCost.ToString()) ;
            }
        }
        else
        {
            //distance enemy pos,path.peek.pos 
           Debug.Log(Vector2.Distance((Vector2)WorldPos_To_GridIndex(enemy), Path.Peek().pos));
            if (Path.Count>0 && Vector2.Distance((Vector2)WorldPos_To_GridIndex(enemy),Path.Peek().pos) > distance)
            {
                float t=0.97f;
               // Debug.Log(GridIndex_To_WorldPos((int)Path.Peek().pos.x, (int)Path.Peek().pos.y));
                enemy.position =enemy.position*t +  (1-t)*GridIndex_To_WorldPos((int)Path.Peek().pos.x,(int) Path.Peek().pos.y);
            }
            else if(Path.Count > 0 && Vector2.Distance((Vector2)WorldPos_To_GridIndex(enemy), Path.Peek().pos) < distance)
            {
                Path.Pop();
            }
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
                        neig.option = true;
                        neig.parent = curr;
                        options.Add(neig);
                    }
                    else if (neig.option)
                    {
                        Node tmp = neig;
                        tmp.parent = curr;
                        tmp.Calc_GCost();
                        tmp.Calc_FCost();
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
            //Debug.Log( ((int)((player.position.x + Xoffset*10 )/ 10) ).ToString() + " " + ((int)(player.position.z + Yoffset*10) / 10).ToString());
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
