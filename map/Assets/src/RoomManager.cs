using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager {
	private int[,] roomPos; 
	public Dictionary<string, Vector2> wallTile; 
	public Dictionary<string, Vector2> floorTile; 
	public RoomManager(){
		roomPos = new int[1000, 1000];
		wallTile = new Dictionary<string, Vector2>();
		floorTile = new Dictionary<string, Vector2>();
	}

	public void setWall(int x, int y){
		roomPos[x,y] = 1;
		wallTile.Add (getKey (x, y), new Vector2(x, y));
	}

	public void setFloor(int x, int y){
		roomPos[x,y] = 1;
		floorTile.Add (getKey (x, y), new Vector2(x, y));
	}

	private string getKey(int x, int y){
		return x + ":" + y;
	}

	public List<Vector2> getRoomExitTile(){
		float xMin = 0, xMax = 0, yMin = 0, yMax = 0;

		// 部屋の角の座標
		foreach (Vector2 pos in new List<Vector2>(wallTile.Values)) {
			if (xMin == 0 || xMax == 0 || yMin == 0 || yMax == 0) {
				xMin = pos.x;
				xMax = pos.x;
				yMin = pos.y;
				yMax = pos.y;
			}
			if (pos.x < xMin) {xMin = pos.x;};
			if (pos.x > xMax) {xMax = pos.x;};
			if (pos.y < xMin) {yMin = pos.y;};
			if (pos.y > xMax) {yMin = pos.y;};
		}

		// 部屋の角の除いた座標
		List<Vector2> results = new List<Vector2> ();
		foreach (Vector2 pos in new List<Vector2>(wallTile.Values)) {
			if (pos.x == xMin && pos.y == yMin) {
				continue;
			}
			if (pos.x == xMax && pos.y == yMax) {
				continue;
			}
			if (pos.x == xMin && pos.y == yMax) {
				continue;
			}
			if (pos.x == xMax && pos.y == yMin) {
				continue;
			}
			results.Add (pos);
		}
		return results;
	}

	public Vector2 getCenter(){
		float x = 0F;
		float y = 0F;
		foreach (Vector2 pos in new List<Vector2>(floorTile.Values)) {
			x += pos.x;
			y += pos.y;
		}
		return new Vector2 (x / floorTile.Count, y / floorTile.Count);
	}

}
