using System;

public class Edge
{
    private Cell _cell1, _cell2;
    private EdgeTypes _edgeType;
    public EdgeTypes EdgeType
    {
        get => _edgeType;
        set => _edgeType = value;
    }

    public Edge(Cell cell1, Cell cell2)
    {
        _cell1 = cell1;
        _cell2 = cell2;
    }
    
    

    public Cell Cell1 => _cell1;

    public Cell Cell2 => _cell2;
    
    public Cell GetOther(Cell thisCell)
    {
        return thisCell == _cell1 ? _cell2 : _cell1;
    }
    
    public enum EdgeTypes
    {
        Wall,
        Passage,
        Door
    }
}