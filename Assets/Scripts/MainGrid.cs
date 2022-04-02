using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;


public class MainGrid : MonoBehaviour
{
    
    [Range(0, 2)]
    [SerializeField] private float distance;
    [SerializeField] private int size;
    [SerializeField] private GameObject cube;
    [SerializeField] private GameObject plane;
    [SerializeField] private Transform player;
    [SerializeField] private Transform enemy;
    [SerializeField] private Text toggleButtonText;
    
    private bool _startProcess = false;
    private Camera _camera;
    private float _xOffset;
    private float _yOffset;

    private Node _enemy;
    private Node _target;
    private Node _current;

    private Stack<Node> _path;

    List<Node> _visited = new List<Node>();
    List<Node> _options = new List<Node>();


    private bool[,] _walls;
    private bool _found = false;

    Node[,] _grid;
    private bool _isGridReady = false;
    private void Start()
    {
        _camera = Camera.main;
        _grid = new Node[size, size];
        _walls = new bool[size, size];
        GenerateGrid();

        _path = new Stack<Node>();

        _target = _grid[0, 0];
        _target.Pos = new Vector2(0, 0);
        _target.HCost = 0;
        _grid[0, 0] = _target;

        _enemy = _grid[19, 19];
        _enemy.Pos = new Vector2(19, 19);
        _enemy.Parent = null;
        _enemy.Visited = true;
        _enemy.GCost = 0;
        _enemy.Calc_HCost(_target.Pos);
        _enemy.Calc_FCost();
        _grid[19, 19] = _enemy;
        _visited.Add(_enemy);

        CalcNeighbour(_enemy);
       // options= options.OrderBy(x => x.FCost).ToList();
       
       _isGridReady = true;
    }

    private void Update()
    {
            var mousePos = Input.mousePosition;
            var rayOrigin =_camera.ScreenToWorldPoint(mousePos);
            var rayEnd = rayOrigin;
            rayEnd.y = 500;
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin,rayOrigin-rayEnd, out hit))
            {
                var cell =hit.collider.GetComponent<Cell>();
                _grid[cell.i, cell.j].IsWall = true;
                FillCell(_grid[cell.i, cell.j], 3);
                Debug.Log(cell.gameObject.name);
            }
        }
            Debug.DrawRay(rayOrigin,rayOrigin-rayEnd );
        if (!_startProcess) return;
        FindPath();
        if (_found)
        {
            toggleButtonText.text = "Restart";
            _startProcess = false;
        }

    }
    public void ToggleProcess()
    {
        _startProcess = !_startProcess;
    }

    private Vector2Int WorldPos_To_GridIndex(Vector3 pos)
    {
        return new Vector2Int((int)((pos.x + _xOffset * 10) / 10), ((int)(pos.z + _yOffset * 10) / 10));
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
            if (temp.FCost > item.FCost)
            {
                temp = item;
            }
            else if (Math.Abs(temp.FCost - item.FCost) < 0.001f && temp.HCost > item.HCost)
            {
                temp = item;
            }
        }

        // options.Remove(options[0]);
        return temp;
    }

   
    private void FindPath()
    {
        if (!_found)
        {
            _current = GetMinCost();


            _current.Visited = true;

            int index = _options.FindIndex(x => x.Visited);
            _options.RemoveAt(index);
            _visited.Add(_current);
            _grid[(int) _current.Pos.x, (int) _current.Pos.y] = _current;

            FillCell(_current, 2);
            
            if (_current.Pos == _target.Pos)
            {
                FillCell(_current, 2);
                _found = true;

                Node pointer = _current;
                while (pointer.Pos != _enemy.Pos)
                {
                    _path.Push(pointer);
                    pointer = pointer.Parent;
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
                Vector2.Distance((Vector2) WorldPos_To_GridIndex(enemy.position), _path.Peek().Pos) > distance)
            {
                float t = 0.97f;
                // Debug.Log(GridIndex_To_WorldPos((int)Path.Peek().pos.x, (int)Path.Peek().pos.y));
                enemy.position = enemy.position * t +
                                 (1 - t) * GridIndex_To_WorldPos((int) _path.Peek().Pos.x, (int) _path.Peek().Pos.y);
            }
            else if (_path.Count > 0 &&
                     Vector2.Distance((Vector2) WorldPos_To_GridIndex(enemy.position), _path.Peek().Pos) < distance)
            {
                _path.Pop();
            }
        }
    }

    private Cell FillCell(Node node, int materialOption)
    {
        var cell = FilledCellPool.Instance.Get();
        cell.SetMaterial(materialOption);
        cell.transform.position =
        new Vector3((node.Pos.x - _xOffset) * 10 + 5, 0, (node.Pos.y - _yOffset) * 10 + 5);
        return cell;
    }

    private void CreateCell(int i, int j)
    {
        var cell = FillCell(_grid[i,j],-1);
        cell.transform.localScale += Vector3.down; 
        cell.i = i;
        cell.j = j;
        cell.GetComponent<BoxCollider>().enabled = true;
    }

    private void CalcNeighbour(Node curr)
    {

        int[] dir = new int[16] { -1,1  ,   0,1  ,    1,1   ,
                                  -1,0       ,        1,0   ,
                                 -1,-1  ,   0,-1  ,   1,-1   };
        int Px, Py,x,y;
        Px = (int)curr.Pos.x;
        Py = (int)curr.Pos.y;

        for (int i = 0; i < 16; i+=2)
        {
            x = Px;
            y = Py;
            x += dir[i];
            y += dir[i+1];
            if (x>=0 && x<size && y >= 0 && y < size)
            {
                Node neig = _grid[x, y];
                if (!neig.IsWall && !neig.Visited)
                {
                    if (!neig.Option)
                    {
                        neig.Option = true;
                        neig.Parent = curr;
                        _options.Add(neig);
                    }
                    else if (neig.Option)
                    {
                        Node tmp = neig;
                        tmp.Parent = curr;
                        tmp.Calc_GCost();
                        tmp.Calc_FCost();
                        if (tmp.GCost < neig.GCost )
                        {
                            neig = tmp;
                        } 
                    }
                    neig.Calc_GCost();
                    neig.Calc_HCost(_target.Pos);
                    neig.Calc_FCost();

                    _grid[x, y] = neig;
                }
            }
        }
    }

    private void GenerateGrid()
    {
        _xOffset = size / 2 ;
        _yOffset = size / 2;
        
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                _grid[i,j] = InitializeNode(i, j);
                CreateCell(i,j);
            }
        }
    }

    private Node InitializeNode(int x, int y)
    {
        Node temp = new Node();
        temp.Pos.x = x ;
        temp.Pos.y = y ;
        return temp;
    }

    private void OnDrawGizmos()
    {
        if(!_isGridReady) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(new Vector3((_target.Pos.x - _xOffset) * 10 + 5, 0, (_target.Pos.y - _yOffset) * 10 + 5), new Vector3(10, 10, 10));
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Gizmos.DrawWireCube(new Vector3((i - _xOffset)*10 + 5,0, (j - _yOffset)*10 +5) , new Vector3(10, 0.1f, 10));
            }
        }
    }
}
