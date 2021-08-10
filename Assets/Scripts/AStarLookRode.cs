using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarLookRode
{
    public GridMeshCreate meshMap;
    public Grid startGrid;
    public Grid endGrid;

    public List<Grid> openGrids;
    public List<Grid> closeGrids;
    public Stack<Grid> rodes;

    public void Init(GridMeshCreate meshMap, Grid startGrid, Grid endGrid)
    {
        this.meshMap = meshMap;
        this.startGrid = startGrid;
        this.endGrid = endGrid;
        openGrids = new List<Grid>();
        closeGrids = new List<Grid>();
        rodes = new Stack<Grid>();
    }

    
    public IEnumerator OnStart()
    {

        //Item itemRoot = Map.bolls[0].item;
        rodes.Push(startGrid);
        closeGrids.Add(startGrid);

        TraverseItem(startGrid.posX, startGrid.posY);
        yield return new WaitForSeconds(1);
        Traverse();

        //Ϊ�˱����޷����Ѱ·��������ѭ���������ʹ��For���涨Ѱ·�������
        for (int i = 0; i < 6000; i++)
        {
            if (rodes.Peek().posX == endGrid.posX && rodes.Peek().posY == endGrid.posY)
            {
                ChangeRodeColor();
                break;
            }

            TraverseItem(rodes.Peek().posX, rodes.Peek().posY);
            yield return new WaitForSeconds(1f);
            Traverse();

        }

    }
    


    /// <summary>
    /// Ѱ���ߵ���ǰ��λʱ����һ�����п����ߵ�����λ�ò����뵽�����б�
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public void TraverseItem(int i, int j)
    {
        int xMin = Mathf.Max(i - 1, 0);
        int xMax = Mathf.Min(i + 1, meshMap.meshRange.horizontal - 1);
        int yMin = Mathf.Max(j - 1, 0);
        int yMax = Mathf.Min(j + 1, meshMap.meshRange.vertical - 1);
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                Grid grid = meshMap.grids[x, y].GetComponent<Grid>();
                if ((y == j && i == x) || closeGrids.Contains(grid)|| openGrids.Contains(grid))
                {
                    continue;
                }
                if (openGrids.Contains(grid))
                {
                    grid.parentGrid = meshMap.grids[i, j].GetComponent<Grid>();
                    continue;
                }
                    
                if (!grid.isHinder)
                {
                    openGrids.Add(grid);
                    grid.ChangeColor(Color.blue);
                    grid.parentGrid= meshMap.grids[i, j].GetComponent<Grid>();
                }
               
            }
        }



    }

    /// <summary>
    /// �ڿ����б�ѡ��·����̵ĵ�����·��ջ��ͬʱ��·������뵽�պ��б���
    /// </summary>
    public void Traverse()
    {
        if (openGrids.Count == 0)
        {
            return;
        }
        Grid minLenthGrid = openGrids[0];
        int minLength = SetNoteData(minLenthGrid);
        for (int i = 0; i < openGrids.Count; i++)
        {
            if (minLength > SetNoteData(openGrids[i]))
            {
                minLenthGrid = openGrids[i];
                minLength = SetNoteData(openGrids[i]);
            }
        }
        minLenthGrid.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        Debug.Log("����Ѱ�������ķ���" + minLenthGrid.posX + "::::" + minLenthGrid.posY);

        closeGrids.Add(minLenthGrid);
        openGrids.Remove(minLenthGrid);
        while (rodes.Count != 0)
        {
            if (minLenthGrid.parentGrid == rodes.Peek())
            {
                break;
            }
            rodes.Pop();
        }
        
        rodes.Push(minLenthGrid);
    }


    void ChangeRodeColor()
    {
        List<Grid> grids = new List<Grid>();
        
        
        while (rodes.Count != 0)
        {
            rodes.Pop().ChangeColor(Color.black);
            grids.Insert(0, rodes.Pop());

        }

        
        
    }
    public void AddGridToRode(Grid grid)
    {
        while (true)
        {
            if (rodes.Count==0 || rodes.Peek() == grid.parentGrid)
            {
                break;
            }
            rodes.Pop();
        }
        rodes.Push(grid);
    }


     /// <summary>
     /// ��������ĳһ��λ��Ԥ��·���ܳ���
     /// </summary>
     /// <param name="grid"></param>
     /// <returns></returns>
    public int SetNoteData(Grid grid)
    {

        Grid itemParent = rodes.Count == 0 ? startGrid : grid.parentGrid;
        int numG = Mathf.Abs(itemParent.posX - grid.posX) + Mathf.Abs(itemParent.posY - grid.posY);
        int n = numG == 1 ? 10 : 14;
        grid.G = itemParent.G + n;

        int numH = Mathf.Abs(endGrid.posX - grid.posX) + Mathf.Abs(endGrid.posY - grid.posY);
        grid.H = numH * 10;
        grid.All = grid.H + grid.G;
        return grid.All;
    }











}
