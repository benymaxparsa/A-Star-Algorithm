using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MainGrid : MonoBehaviour
{
    [Range(0, 2)]
    [SerializeField]
    private float distance; 
    [SerializeField]
    public int size;
    [SerializeField]
    public float scale = 1;
    
    private float _xOffset;
    private float _yOffset;

    private Node _enemy;
    private Node _target;
    private Node _current;

    private Stack<Node> _path;

    List<Node> _visited = new List<Node>();
    List<Node> _options = new List<Node>();

    [SerializeField] private GameObject cube;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject plane;
    [SerializeField] private Transform player;
    [SerializeField] private Transform enemy;

    private bool[,] _walls;
    private bool _found = false;

    Node[,] _grid;
    private bool _isReady = false;
    private Vector2Int WorldPos_To_GridIndex(Transform pos)
    {
        return new Vector2Int((int)((pos.position.x + _xOffset * 10) / 10), ((int)(pos.position.z + _yOffset * 10) / 10));
    }

    private Vector3 GridIndex_To_WorldPos(int x, int y)
    {
        return new Vector3((x - _xOffset) * 10 +5, 0, (y - _yOffset) * 10 + 5);
    }

    private Node GetMinCost()
    {
        if (_options == null)
            return null;
        Node temp = _options[0];

        foreach (var item in _options)
        {
            if (temp.fCost > item.fCost)
            {
                temp = item;
            }
            else if (temp.fCost == item.fCost && temp.hCost > item.hCost)
            {
                temp = item;
            }
        }

        // options.Remove(options[0]);
        return temp;
    }

    private void Start()
    {
        _grid = new Node[size, size];
        _walls = new bool[size, size];
        GenerateGrid();

        _path = new Stack<Node>();

        _target = _grid[0, 0];
        _target.pos = new Vector2(0, 0);
        _target.hCost = 0;
        _grid[0, 0] = _target;

        _enemy = _grid[19, 19];
        _enemy.pos = new Vector2(19, 19);
        _enemy.parent = null;
        _enemy.visited = true;
        _enemy.gCost = 0;
        _enemy.Calc_HCost(_target.pos);
        _enemy.Calc_FCost();
        _grid[19, 19] = _enemy;
        _visited.Add(_enemy);

        CalcNeighbour(_enemy);
       // options= options.OrderBy(x => x.FCost).ToList();
        _grid[4, 4].isWall = true;
        Instantiate(wall, GridIndex_To_WorldPos(4, 4), Quaternion.identity);


        _grid[4, 5].isWall = true;
        Instantiate(wall, new Vector3((4 - _xOffset) * 10 + 5, 0, (5 - _yOffset) * 10 + 5), Quaternion.identity);

        _grid[4, 6].isWall = true;
        Instantiate(wall, new Vector3((4 - _xOffset) * 10 + 5, 0, (6 - _yOffset) * 10 + 5), Quaternion.identity);

        _grid[4, 7].isWall = true;
        Instantiate(wall, new Vector3((4 - _xOffset) * 10 + 5, 0, (7 - _yOffset) * 10 + 5), Quaternion.identity);

        _grid[4, 8].isWall = true;
        Instantiate(wall, new Vector3((4 - _xOffset) * 10 + 5, 0, (8 - _yOffset) * 10 + 5), Quaternion.identity);

        _grid[5, 4].isWall = true;
        Instantiate(wall, new Vector3((5 - _xOffset) * 10 + 5, 0, (4 - _yOffset) * 10 + 5), Quaternion.identity);

        _grid[6, 4].isWall = true;
        Instantiate(wall, new Vector3((6 - _xOffset) * 10 + 5, 0, (4 - _yOffset) * 10 + 5), Quaternion.identity);
        _grid[9, 4].isWall = true;
        Instantiate(wall, new Vector3((9 - _xOffset) * 10 + 5, 0, (4 - _yOffset) * 10 + 5), Quaternion.identity);
                                                                  
        _grid[10,4].isWall = true;                                 
        Instantiate(wall, new Vector3((10 - _xOffset) * 10 + 5, 0, (4 - _yOffset) * 10 + 5), Quaternion.identity);
        _grid[11,4].isWall = true;                                 
        Instantiate(wall, new Vector3((11 - _xOffset) * 10 + 5, 0, (4 - _yOffset) * 10 + 5), Quaternion.identity);
        _grid[12,4].isWall = true;                                 
        Instantiate(wall, new Vector3((12 - _xOffset) * 10 + 5, 0, (4 - _yOffset) * 10 + 5), Quaternion.identity);
        _grid[13,4].isWall = true;                                 
        Instantiate(wall, new Vector3((13 - _xOffset) * 10 + 5, 0, (4 - _yOffset) * 10 + 5), Quaternion.identity);
        _grid[14,4].isWall = true;                                 
        Instantiate(wall, new Vector3((14 - _xOffset) * 10 + 5, 0, (4 - _yOffset) * 10 + 5), Quaternion.identity);
        _grid[15,4].isWall = true;                                 
        Instantiate(wall, new Vector3((15 - _xOffset) * 10 + 5, 0, (4 - _yOffset) * 10 + 5), Quaternion.identity);
        _grid[16,4].isWall = true;                                 
        Instantiate(wall, new Vector3((16 - _xOffset) * 10 + 5, 0, (4 - _yOffset) * 10 + 5), Quaternion.identity);
        _grid[17,4].isWall = true;                                 
        Instantiate(wall, new Vector3((17 - _xOffset) * 10 + 5, 0, (4 - _yOffset) * 10 + 5), Quaternion.identity);
        _grid[18,4].isWall = true;                                 
        Instantiate(wall, new Vector3((18 - _xOffset) * 10 + 5, 0, (4 - _yOffset) * 10 + 5), Quaternion.identity);
        _grid[19,4].isWall = true;                                 
        Instantiate(wall, new Vector3((19 - _xOffset) * 10 + 5, 0, (4 - _yOffset) * 10 + 5), Quaternion.identity);
        Instantiate(cube, new Vector3((9 - _xOffset) * 10 + 5, 0, (9- _yOffset) * 10 + 5), Quaternion.identity);
        _grid[7, 4].isWall = true;
        Instantiate(wall, new Vector3((7 - _xOffset) * 10 + 5, 0, (4 - _yOffset) * 10 + 5), Quaternion.identity);

        _grid[8, 4].isWall = true;
        Instantiate(wall, new Vector3((8 - _xOffset) * 10 + 5, 0, (4 - _yOffset) * 10 + 5), Quaternion.identity);
        _isReady = true;
    }

    private void Update()
    {
        if (!_found)
        {
            _current = GetMinCost();


            _current.visited = true;

            int index = _options.FindIndex(x => x.visited);
            _options.RemoveAt(index);
            _visited.Add(_current);
            _grid[(int) _current.pos.x, (int) _current.pos.y] = _current;
            Instantiate(cube,
                new Vector3((_current.pos.x - _xOffset) * 10 + 5, 0, (_current.pos.y - _yOffset) * 10 + 5),
                Quaternion.identity);
            if (_current.pos == _target.pos)
            {
                Instantiate(cube,
                    new Vector3((_current.pos.x - _xOffset) * 10 + 5, 0, (_current.pos.y - _yOffset) * 10 + 5),
                    Quaternion.identity);
                _found = true;

                Node pointer = _current;
                while (pointer.pos != _enemy.pos)
                {
                    _path.Push(pointer);
                    pointer = pointer.parent;
                }

                _path.Push(_enemy);


                //while (Path.Count>0)
                //{
                //    Debug.Log(GridIndex_To_WorldPos((int)Path.Peek().pos.x, (int)Path.Peek().pos.y));
                //    Path.Pop();
                //}


                Debug.Log("FOUND");
            }

            CalcNeighbour(_current);
            // options = options.OrderBy(x => x.HCost).ToList();
            // options = options.OrderBy(x => x.FCost).ToList();

            for (int i = 0; i < _options.Count; i++)
            {
                //Debug.Log(options[i].pos.ToString() + " F = " + options[i].FCost.ToString()+ " H = "+ options[i].HCost.ToString()) ;
            }
        }
        else
        {
            if (_path.Count > 0 &&
                Vector2.Distance((Vector2) WorldPos_To_GridIndex(enemy), _path.Peek().pos) > distance)
            {
                float t = 0.97f;
                // Debug.Log(GridIndex_To_WorldPos((int)Path.Peek().pos.x, (int)Path.Peek().pos.y));
                enemy.position = enemy.position * t +
                                 (1 - t) * GridIndex_To_WorldPos((int) _path.Peek().pos.x, (int) _path.Peek().pos.y);
            }
            else if (_path.Count > 0 &&
                     Vector2.Distance((Vector2) WorldPos_To_GridIndex(enemy), _path.Peek().pos) < distance)
            {
                _path.Pop();
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
                Node neig = _grid[x, y];
                if (!neig.isWall && !neig.visited)
                {
                    if (!neig.option)
                    {
                        neig.option = true;
                        neig.parent = curr;
                        _options.Add(neig);
                    }
                    else if (neig.option)
                    {
                        Node tmp = neig;
                        tmp.parent = curr;
                        tmp.Calc_GCost();
                        tmp.Calc_FCost();
                        if (tmp.gCost < neig.gCost )
                        {
                            neig = tmp;
                        } 
                    }
                    neig.Calc_GCost();
                    neig.Calc_HCost(_target.pos);
                    neig.Calc_FCost();

                    _grid[x, y] = neig;
                }
            }
        }
    }

    private void GenerateGrid()
    {
        GameObject p = Instantiate(plane, Vector3.zero, Quaternion.identity);

        p.transform.localScale = new Vector3(size, size, size);

        _xOffset = p.transform.localScale.x / 2 ;
        _yOffset = p.transform.localScale.z / 2;
        
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                _grid[i,j] = InitializeNode(i, j);
            }
        }
    player.position = new Vector3(p.transform.position.x - _xOffset, 1,p.transform.position.z - _yOffset) * 10;
    }
    private Vector2 PlayerPos()
    {
        

       if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log( ((int)((player.position.x + Xoffset*10 )/ 10) ).ToString() + " " + ((int)(player.position.z + Yoffset*10) / 10).ToString());
        }

        return new Vector2((int)((player.position.x + _xOffset * 10) / 10)  ,((int)(player.position.z + _yOffset * 10) / 10));
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
        if(!_isReady) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(new Vector3((_target.pos.x - _xOffset) * 10 + 5, 0, (_target.pos.y - _yOffset) * 10 + 5), new Vector3(10, 10, 10));
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Gizmos.DrawWireCube(new Vector3((i - _xOffset)*10 + 5,0, (j - _yOffset)*10 +5) , new Vector3(10, 0.1f, 10));
            }
        }
    }
}
