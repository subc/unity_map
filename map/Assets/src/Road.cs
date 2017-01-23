using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road  {
	public static List<Vector2> create(RoomManager room1, RoomManager room2){
		// wall同士でもっとも近いpairを生成いていく
		List<Vector2> nears = getNearPos (room1.getRoomExitTile(), room2.getRoomExitTile());
		return nears;
	}

	public static List<Vector2> getNearPos(List<Vector2> walls1, List<Vector2> walls2) {
		Vector2 near1 = new Vector2(0, 0);
		Vector2 near2 = new Vector2(0, 0);
		float nearLength = 0;
		foreach(Vector2 pos1 in walls1)
		{
			foreach(Vector2 pos2 in walls2)
			{
				// init
				if (nearLength == 0) {
					nearLength = Vector2.Distance(pos1, pos2);
					near1 = pos1;
					near2 = pos2;
				}

				// 距離計算
				float l = Vector2.Distance(pos1, pos2);
				if (l < nearLength) {
					// 記録更新
					nearLength = l;
					near1 = pos1;
					near2 = pos2;
				}
			}
		}

		// near1, near2を起点に線を引く
		Vector2 tmp;
		if (near1.x > near2.x) {
			tmp = near1;
			near1 = near2;
			near2 = tmp;
		}
		List<Vector2> result = new List<Vector2>();
		result.Add (near1);
		result.Add (near2);

		// 真横に引けるとき
		if (near1.y == near2.y) {
			foreach (int i in System.Linq.Enumerable.Range((int)(near1.x + 1), (int)near2.x - (int)(near1.x + 1))) {
				result.Add (new Vector2 (i, near1.y));
			}
		} else {
			// ズレてる時
			float edgeX1 = near1.x;
			float edgeX2 = near2.x;
			while (true) {
				if (edgeX1 >= edgeX2) {
					break;
				}
				// しゃくとり方式
				edgeX1 += 1;
				result.Add (new Vector2 (edgeX1, near1.y));

				if (edgeX1 >= edgeX2) {
					break;
				}
				edgeX2 += -1;
				result.Add (new Vector2 (edgeX2, near2.y));
			}

			// 上下を埋める
			foreach (int i in System.Linq.Enumerable.Range((int)System.Math.Min(near1.y, near2.y), (int)System.Math.Max(near1.y, near2.y) - (int)System.Math.Min(near1.y, near2.y))) {
				result.Add (new Vector2 (edgeX2, i));
			}
		}


		return result;
	}

}
