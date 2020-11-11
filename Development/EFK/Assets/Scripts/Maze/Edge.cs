public class Edge
{
    private Cell _cell1, _cell2;

    public Edge(Cell cell1, Cell cell2)
    {
        _cell1 = cell1;
        _cell2 = cell2;
    }

    public Cell Cell1 => _cell1;

    public Cell Cell2 => _cell2;
}