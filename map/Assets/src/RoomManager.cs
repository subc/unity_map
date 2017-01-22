using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager {
	private int[,] roomPos; 
	public Dictionary<string, int[,]> wallTile; 
	public Dictionary<string, int[,]> floorTile; 
	public RoomManager(){
		roomPos = new int[100, 100];
		wallTile = new Dictionary<string, int[,]>();
		floorTile = new Dictionary<string, int[,]>();
	}

	public void setWall(int x, int y){
		roomTile.Add (getKey (x, y), new int[x, y]);
		wallTile.Add (getKey (x, y), new int[x, y]);
	}

	public void setFloor(int x, int y){
		roomTile.Add (getKey (x, y), new int[x, y]);
		floorTile.Add (getKey (x, y), new int[x, y]);
	}

	private string getKey(int x, int y){
		return x + ":" + y;
	}

	public void setExit(){
		
	}
}
