using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager {
	public int x;
	public int y;
	public Dictionary<string, GameObject> walls;
	private int[,] roomPos;

	public WorldManager(int x, int y) {
		this.x = x;
		this.y = y;
		this.walls = new Dictionary<string, GameObject> ();
		roomPos = new int[x, y];
	}

	public GameObject getWall(int x, int y){
		return this.walls[getKey (x, y)];
	}

	// 壁を消す
	public GameObject destroyWall(int x, int y){
		GameObject wall = this.walls[getKey (x, y)];
		this.walls.Remove (getKey (x, y));
		roomPos [x, y] = 0;
		return wall;
	}

	// 座標が利用可能か調べる
	public bool used(int x, int y){
		if (x < 0) {return false;}
		if (y < 0) {return false;}
		return roomPos [x, y] == 1 ? true : false;
	}

	// 座標を利用する
	public void checkin(int x, int y, GameObject wall){
		if (this.used (x, y)) {
			throw new System.Exception ("Already Userd Pos x: " + x + " y:" + y);
		}
		roomPos [x, y] = 1;
		// 壁の登録
		this.walls.Add (getKey (x, y), wall);
	}

	public void checkout(int x, int y){
		roomPos [x, y] = 0;
	}

	private string getKey(int x, int y){
		return x + ":" + y;
	}
}
