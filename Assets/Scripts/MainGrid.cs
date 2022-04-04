using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;


public class MainGrid : MonoBehaviour
{
    [Range(0, 2)] [SerializeField] private float distance;
    [SerializeField] private int size;
    [SerializeField] private Text toggleButtonText;
    [SerializeField] private Button toggleButton;

    private bool _startProcess = false;
    private Camera _camera;
    private float _xOffset;
    private float _yOffset;

    private Node _source = null;
    private Node _destination = null;
    private Node _current;

    private Stack<Node> _path;

    List<Node> _visited = new List<Node>();
    List<Node> _options = new List<Node>();


    private bool _found = false;
    private CellType _cellDrawType = CellType.Wall;
    Dictionary<(int, int), (Node, Cell)> _grid;

    private void Start()
    {
        _camera = Camera.main;
        _grid = new Dictionary<(int, int), (Node, Cell)>();
        GenerateGrid();
        // Reset();
        SetDestination(19, 19);
        SetSource(0, 0);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            hit = CastRay();
            if (!hit.collider)
                _cellDrawType = CellType.Wall;
            else if (hit.collider.GetComponent<Cell>().cellType == CellType.Wall)
                _cellDrawType = CellType.Path;
        }

        if (Input.GetMouseButton(0))
            DrawWalls(_cellDrawType);

        if ((Input.GetKey(KeyCode.LeftControl) ||
             Input.GetKey(KeyCode.RightControl)) &&
            Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            hit = CastRay();
            if (hit.collider)
            {
                var cell = hit.collider.GetComponent<Cell>();
                if (cell)
                    SetSource(cell.i, cell.j);
            }
        }
        if ((Input.GetKey(KeyCode.LeftShift) ||
             Input.GetKey(KeyCode.RightShift)) &&
            Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            hit = CastRay();
            if (hit.collider)
            {
                var cell = hit.collider.GetComponent<Cell>();
                if (cell)
                    SetDestination(cell.i, cell.j);
            }
        }


        if (!_startProcess) return;
        FindPath();
        if (_found)
        {
            toggleButtonText.text = "Restart";
        }
    }

    private void DrawWalls(CellType cellType)
    {
        RaycastHit hit = CastRay();
        if (hit.collider)
        {
            var cell = hit.collider.GetComponent<Cell>();
            if (!cell) return;
            if (cell.cellType == CellType.End || cell.cellType == CellType.Start) return;
            _grid[(cell.i, cell.j)].Item1.IsWall = true;
            cell.SetCellType(cellType);
        }
    }

    private RaycastHit CastRay()
    {
        var mousePos = Input.mousePosition;
        var rayOrigin = _camera.ScreenToWorldPoint(mousePos);
        var rayEnd = rayOrigin;
        rayEnd.y = 500;
        Physics.Raycast(rayOrigin, rayOrigin - rayEnd, out var hit);
        return hit;
    }

    public void Reset()
    {
        _path = new Stack<Node>();
        _visited = new List<Node>();
        _options = new List<Node>();
        _found = false;
        foreach (var nodeCell in _grid)
        {
            if(nodeCell.Value.Item2.cellType == CellType.Start ||
               nodeCell.Value.Item2.cellType == CellType.End )
             continue;
                
            var index = nodeCell.Key;
            var pos = nodeCell.Value.Item1.Pos;
            nodeCell.Value.Item1.Reset();
            if (nodeCell.Value.Item2.cellType != CellType.Wall)
                nodeCell.Value.Item2.SetCellType(CellType.Path);
        }
        SetDestination(_destination.Pos.x,_destination.Pos.y);
        SetSource(_source.Pos.x,_source.Pos.y);
        CalcNeighbour(_source);
    }

    private void SetSource(int i, int j)
    {
        if (_source == null)
            _source = _grid[(i, j)].Item1;
        else
            _grid[(_source.Pos.x, _source.Pos.y)].Item2.SetCellType(CellType.Path);
        _source = _grid[(i, j)].Item1;
        _source.Parent = null;
        _source.Visited = true;
        _source.GCost = 0;
        _source.Calc_HCost(_destination.Pos);
        _source.Calc_FCost();
        _grid[(i, j)].Item1.Set(_source);
        _visited.Add(_source);
        _grid[(i, j)].Item2.SetCellType(CellType.Start);
    }

    private void SetDestination(int i, int j)
    {
        if (_destination == null)
            _destination = _grid[(i, j)].Item1;
        else
            _grid[(_destination.Pos.x, _destination.Pos.y)].Item2.SetCellType(CellType.Path);
        _destination = _grid[(i, j)].Item1;
        _destination.HCost = 0;
        _grid[(i, j)].Item1.Set(_destination);
        _grid[(i, j)].Item2.SetCellType(CellType.End);
    }

    public void ToggleProcess()
    {
        Reset();
        _startProcess = true;
        toggleButton.interactable = false;
    }

    private Vector2Int WorldPos_To_GridIndex(Vector3 pos)
    {
        return new Vector2Int((int) ((pos.x + _xOffset * 10) / 10), ((int) (pos.z + _yOffset * 10) / 10));
    }

    private Vector3 GridIndex_To_WorldPos(int x, int y)
    {
        return new Vector3((x - _xOffset) * 10 + 5, 0, (y - _yOffset) * 10 + 5);
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
            _grid[(_current.Pos.x, _current.Pos.y)].Item1.Set(_current);

            var cell = _grid[(_current.Pos.x, _current.Pos.y)].Item2;
            if (cell)
            {
                cell.SetCellType(CellType.Visited);
            }

            Debug.Log(_current.Pos.ToString() + "  " + _destination.Pos.ToString());
            if (_current.Pos == _destination.Pos)
            {
                //FillCell(_current, 2);
                _found = true;

                Node pointer = _current;
                while (pointer.Pos != _source.Pos)
                {
                    _path.Push(pointer);
                    pointer = pointer.Parent;
                }

                _path.Push(_source);


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

            /*for (int i = 0; i < _options.Count; i++)
            {
                //Debug.Log(options[i].pos.ToString() + " F = " + options[i].FCost.ToString()+ " H = "+ options[i].HCost.ToString()) ;
            }*/
        }
        else
        {
            if (_path.Count > 0)
            {
                var node = _path.Pop();
                _grid[(node.Pos.x, node.Pos.y)].Item2.SetCellType(CellType.None);
            }
            else
            {
                _startProcess = false;
                toggleButton.interactable = true;
            }
        }
    }


    private Cell CreateCell(int i, int j)
    {
        var cell = FilledCellPool.Instance.Get();
        cell.SetCellType(CellType.Path);
        cell.transform.position =
            new Vector3((i - _xOffset) * 10 + 5, 0, (j - _yOffset) * 10 + 5);
        cell.i = i;
        cell.j = j;
        return cell;
    }

    private void CalcNeighbour(Node curr)
    {
        int[] dir = new int[16]
        {
            -1, 1, 0, 1, 1, 1,
            -1, 0, 1, 0,
            -1, -1, 0, -1, 1, -1
        };
        int Px, Py, x, y;
        Px = (int) curr.Pos.x;
        Py = (int) curr.Pos.y;

        for (int i = 0; i < 16; i += 2)
        {
            x = Px;
            y = Py;
            x += dir[i];
            y += dir[i + 1];
            if (x >= 0 && x < size && y >= 0 && y < size)
            {
                var neig = _grid[(x, y)].Item1;

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
                        if (tmp.GCost < neig.GCost)
                        {
                            neig = tmp;
                        }
                    }

                    neig.Calc_GCost();
                    neig.Calc_HCost(_destination.Pos);
                    neig.Calc_FCost();

                    _grid[(x, y)].Item1.Set(neig);
                }
            }
        }
    }

    private void GenerateGrid()
    {
        var s = size / 2;
        _xOffset = s;
        _yOffset = s;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                _grid.Add((i, j), (InitializeNode(i, j), CreateCell(i, j)));
            }
        }
    }

    private Node InitializeNode(int x, int y)
    {
        Node temp = new Node();
        temp.Pos.x = x;
        temp.Pos.y = y;
        return temp;
    }
}