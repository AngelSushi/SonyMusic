// This code automatically generated by TableCodeGen
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SampleTable
{
	public class Row
	{
		public string Year;
		public string Make;
		public string Model;
		public string Description;
		public string Price;

	}

	List<Row> rowList = new List<Row>();
	bool isLoaded = false;

	public bool IsLoaded()
	{
		return isLoaded;
	}

	public List<Row> GetRowList()
	{
		return rowList;
	}

	public void Load(TextAsset csv)
	{
		rowList.Clear();
		string[][] grid = CsvParser2.Parse(csv.text);
		for(int i = 1 ; i < grid.Length ; i++)
		{
			Row row = new Row();
			Debug.Log(grid[i][0]);
			row.Year = grid[i][0];
			row.Make = grid[i][1];
			row.Model = grid[i][2];
			row.Description = grid[i][3];
			row.Price = grid[i][4];

			rowList.Add(row);
		}
		isLoaded = true;
	}

	public int NumRows()
	{
		return rowList.Count;
	}

	public Row GetAt(int i)
	{
		if(rowList.Count <= i)
			return null;
		return rowList[i];
	}

	public Row Find_Year(string find)
	{
		return rowList.Find(x => x.Year == find);
	}
	public List<Row> FindAll_Year(string find)
	{
		return rowList.FindAll(x => x.Year == find);
	}
	public Row Find_Make(string find)
	{
		return rowList.Find(x => x.Make == find);
	}
	public List<Row> FindAll_Make(string find)
	{
		return rowList.FindAll(x => x.Make == find);
	}
	public Row Find_Model(string find)
	{
		return rowList.Find(x => x.Model == find);
	}
	public List<Row> FindAll_Model(string find)
	{
		return rowList.FindAll(x => x.Model == find);
	}
	public Row Find_Description(string find)
	{
		return rowList.Find(x => x.Description == find);
	}
	public List<Row> FindAll_Description(string find)
	{
		return rowList.FindAll(x => x.Description == find);
	}
	public Row Find_Price(string find)
	{
		return rowList.Find(x => x.Price == find);
	}
	public List<Row> FindAll_Price(string find)
	{
		return rowList.FindAll(x => x.Price == find);
	}

}