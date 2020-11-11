using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Room 
{
    private List<Cell> _cells = new List<Cell>();
    private Color color;

    public Room()
    {
        color = Random.ColorHSV();
    }

    public void AddCell(Cell cell)
    {
        _cells.Add(cell);
        cell.Room = this;
    }

    public Cell GetFirstCell() => _cells.Count == 0 ? throw new InvalidDataException() : _cells[0];
    public Color Color => color;
    public List<Cell> GetCellsList() => _cells;
}
