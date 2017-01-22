using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class main : MonoBehaviour {
	public GameObject myCube;
	public Vector3 pos;
	public WorldManager m;

	// Use this for initialization
	void Start () {
		// 背景に色
		setColor(GameObject.Find("bg"), Color.black);

		// 部屋生成のmanager を作成
		m = new WorldManager(1000, 1000);

		RoomManager r1 = createRoom(new Vector3(0, 10, 0), 4);
		RoomManager r2 = createRoom(new Vector3(Random.Range(12, 18), Random.Range (5, 15), 0), 6);

		//gameObject取得 
		myCube = GameObject.Find("target1");

		//今の色コンソールに出力
		Debug.Log(myCube.GetComponent<Renderer>().material.color);

		//青色に変更
		setColor(myCube, Color.blue);

		//変更後の色コンソールに出力
		Debug.Log(myCube.GetComponent<Renderer>().material.color);		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//
	void setColor(GameObject obj, Color color){
		obj.GetComponent<Renderer>().material.color = color;
	}
		
	RoomManager createRoom(Vector3 posBase, int size){
		Debug.Log ("Start create room");
		RoomManager room = new RoomManager ();
		pos = new Vector3(2, 2, 2);
		// プレハブを取得
		GameObject prefab = (GameObject)Resources.Load ("Prefabs/Tile");

		// ルームの外枠のために2マス膨らませる
		int xMax = size + 2;
		int yMax = Random.Range (1, size + 2) + 2;

		// WorldManageに問い合わせて作成できるか調べる
		for (int x = 0; x <= xMax; ++x) {
			for (int y = 0; y <= yMax; ++y) {
				if (m.used((int)posBase.x + x, (int)posBase.y + y)){
					GameObject cube = Instantiate (prefab, posBase + new Vector3(x, y, 0), Quaternion.identity);
					setColor (cube, Color.red);
					throw new System.Exception("Already Created");
				}
			}
		}

		// 部屋作成
		for(int x=0; x<=xMax; ++x)
		{
			for(int y=0; y<=yMax; ++y)
			{
				// xy の値を、幅をそろえて表示
				// プレハブからインスタンスを生成
				GameObject cube = Instantiate (prefab, posBase + new Vector3(x, y, 0), Quaternion.identity);
				m.checkin ((int)posBase.x + x, (int)posBase.y + y); // WorldManagerに登録
				if (x == 0 || x == xMax || y == 0 || y == yMax)
				{
					// wall
					setColor (cube, Color.white);
					room.setWall (x, y);
				} else {
					// floor
					setColor (cube, Color.blue);
					room.setFloor (x, y);
				}
			}
		}
		room.setExit ();
		return room;
	}

}
