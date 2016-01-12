using UnityEngine;

[System.Serializable]
public class Cell
{
    public int CellType;            // type cell with 4 : red; 3 : blue; 2 : gray; 1 : transparent.

    public Vector2 CellPosition;    // position vector 2 of cell

    public int CellEffect;          // 4 : link, 5 : ice

	//These vars are used only for cells that hide items under it

	public bool IsUnlocked = false;         // If the cell has item under it unlocked

	public Cell ChainedCell_next;         // Cell that chained with this cell

	public Cell ChainedCell_last;         // Cell that chained with this cell
}
