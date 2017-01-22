using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager {
	public int x;
	public int y;
	private int[,] roomPos;

	public WorldManager(int x, int y) {
		this.x = x;
		this.y = y;
		roomPos = new int[x, y];
	}

	// 座標が利用可能か調べる
	public bool used(int x, int y){
		return roomPos [x, y] == 1 ? true : false;
	}

	// 座標を利用する
	public void checkin(int x, int y){
		if (this.used (x, y)) {
			throw new System.Exception ("Already Userd Pos x: " + x + " y:" + y);
		}
		roomPos [x, y] = 1;
	}
}
