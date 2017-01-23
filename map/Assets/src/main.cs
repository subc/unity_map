using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class main : MonoBehaviour {
	public GameObject myCube;
	public Vector3 pos;
	public WorldManager m;
	public const string FLOOR = "FLOOR";
	public const string WALL = "WALL";
	public const string ROAD = "ROAD";
	public bool refresh = true;
	bool isRunning = false;

	// Use this for initialization
	void Start () {
		// 背景に色
		setColor(GameObject.Find("bg"), Color.black);

		// カメラ用
		myCube = GameObject.Find("target1");
		setColor(myCube, Color.gray);

	}
	
	// Update is called once per frame
	void Update () {
		if (refresh && !isRunning) {
			genMap ();
			refresh = false;

			//3.5秒後に実行する
			StartCoroutine(DelayMethod(3.5f, () =>
				{
					Debug.Log("Delay call");
					refresh = true;
				}));
		}
	}

	/// <summary>
	/// 渡された処理を指定時間後に実行する
	/// </summary>
	/// <param name="waitTime">遅延時間[ミリ秒]</param>
	/// <param name="action">実行したい処理</param>
	/// <returns></returns>
	private IEnumerator DelayMethod(float waitTime, System.Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}

	void genMap(){
		// 消す
		crearCube ();

		// 部屋生成のmanager を作成
		m = new WorldManager (1000, 1000);
		RoomManager r1 = createRoom (new Vector3 (0, 15, 0), 5);
		RoomManager r2 = createRoom (new Vector3 (Random.Range (10, 15), Random.Range (5, 30), 0), 6);
		List<Vector2> roads = Road.create (r1, r2);
		StartCoroutine(createBlock (roads));
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
		int yMax = Random.Range (3, size + 2) + 2;

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
					cube.tag = WALL;
					setColor (cube, Color.white);
					room.setWall ((int)posBase.x + x, (int)posBase.y + y);
				} else {
					// floor
					cube.tag = FLOOR;
					setColor (cube, Color.blue);
					room.setFloor ((int)posBase.x + x, (int)posBase.y + y);
				}
			}
		}
		return room;
	}

	IEnumerator createBlock(List<Vector2> tiles){
		isRunning = true;
		GameObject prefab = (GameObject)Resources.Load ("Prefabs/Tile");
		while (true) {
			foreach (Vector2 pos in tiles) {
				Debug.Log ("createBlock x: " + pos.x + " y: " + pos.y);
				GameObject cube = Instantiate (prefab, new Vector3 (pos.x, pos.y, -0.01F), Quaternion.identity);
				cube.tag = ROAD;
				setColor (cube, Color.green);
				yield return new WaitForSeconds (0.1f);
			}
			isRunning = false;

			//0.5秒後に実行して壁を消す
			StartCoroutine(DelayMethod(0.5f, () =>
				{
					crearWall();
				}));

			break;
		}
	}

	void crearWall(){
		GameObject[] walls = GameObject.FindGameObjectsWithTag (WALL);
		foreach (GameObject o in walls) {Destroy (o);}
	}

	void crearCube(){
		GameObject[] walls = GameObject.FindGameObjectsWithTag (WALL);
		GameObject[] floors = GameObject.FindGameObjectsWithTag (FLOOR);
		GameObject[] roads = GameObject.FindGameObjectsWithTag (ROAD);
		foreach (GameObject o in walls) {Destroy (o);}
		foreach (GameObject o in floors) {Destroy (o);}
		foreach (GameObject o in roads) {Destroy (o);}
	}
}
