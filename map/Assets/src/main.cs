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
	public const string DOT = "DOT";
	public enum Dire {Up, Down, Right, Left};  
	public bool refresh = true;
	bool isRunning = false;

	// Use this for initialization
	void Start () {
		// 背景に色
//		setColor(GameObject.Find("bg"), Color.black);

		// 全て壁で埋める
		genWallAll();

		// 掘る
		int ct = 0;
		List<Vector2> results = dig (1, 1, true);
		while (ct < 1000) {
			ct+=1;
			// 道をランダムで選ぶ
			Vector2 startPos = results[ Random.Range(0, results.Count) ];
			results.AddRange(dig ((int)startPos.x, (int)startPos.y, false));
			Debug.Log ("Count :" + results.Count);
			if (results.Count >= 400) {
				break;
			}
		}

		// 実際に掘る
		StartCoroutine ("delBlock", results);  


	}

	public void createExit(List<Vector2> results){
		GameObject prefab = (GameObject)Resources.Load ("Prefabs/Tile");
		// start
		GameObject cube = Instantiate (prefab, new Vector3(1, 1, -1F), Quaternion.identity);
		setColor (cube, Color.red);

		// 一番遠い出口を探す
		Vector2 start = new Vector2(1, 1);
		Vector2 exit = new Vector2(1, 2);
		foreach (Vector2 pos in results) {
			if (Vector2.Distance (start, exit) < Vector2.Distance (start, pos)) {
				exit = pos;
			}
		}
		GameObject exitCube = Instantiate (prefab, new Vector3(exit.x, exit.y, -1F), Quaternion.identity);
		setColor (exitCube, Color.red);
	}

	IEnumerator delBlock(List<Vector2> results){
		int count = 0;
		foreach (Vector2 pos in results) {
			count += 1;
			Destroy(m.getWall((int)pos.x, (int)pos.y));
			if (count <= 500){
				yield return new WaitForSeconds (0.01f);
			} else if(count <= 1000){
				yield return new WaitForSeconds (0.005f);
			} else{
				yield return new WaitForSeconds (0.002f);
			}
		}
		// 出口をprotする
		createExit(results);
		yield return new WaitForSeconds (0.002f);
	}

	List<Vector2> dig(int startX, int startY, bool first){
		// 開始地点を決める
		int x = startX; // 開始地点は偶数でないといけない
		int y = startY; // 開始地点は偶数でないといけない

		// 最初の掘り
		if (first) {
			Destroy (m.destroyWall (x, y));
		}

		// 無限ループ
		bool deleted = true;
		int ct = 0;
		List<Vector2> results = new List<Vector2> ();
		while (deleted) {
			deleted = false;
			int i = Random.Range(1, 5);
			// 上下
			if (i == 1 && m.used (x, y + 2) && m.used (x, y + 1) && m.used (x + 1, y + 1) && m.used (x - 1, y + 1)) {
				y += 1;
				m.checkout (x, y);
//				Destroy(m.destroyWall(x, y));
				deleted = true;
			}
			if (i == 2 && m.used (x, y - 2) && m.used (x, y - 1)&& m.used (x + 1, y - 1) && m.used (x - 1, y - 1)) {
				y += -1;
				m.checkout (x, y);
//				Destroy(m.destroyWall(x, y));
				deleted = true;
			}
			// 左右
			if (i == 3 && m.used (x - 2, y) && m.used (x - 1, y) && m.used (x - 1, y + 1) && m.used (x - 1, y - 1)) {
				x += -1;
				m.checkout (x, y);
//				Destroy(m.destroyWall(x,y));
				deleted = true;
			}
			if (i == 4 && m.used (x + 2, y) && m.used (x + 1, y)&& m.used (x + 1, y + 1) && m.used (x + 1, y - 1)) {
				x += 1;
				m.checkout (x, y);
//				Destroy(m.destroyWall(x,y));
				deleted = true;
			}
			if (!deleted && ct <= 30) {
				ct += 1;
				deleted = true;
			} else {
				// 削除成功してる
				results.Add (new Vector2(x, y));
			}
		}
		return results;
	}

	void genWallAll(){
		m = new WorldManager (1000, 1000);

		// 部屋作成
		int xMax = 32;
		int yMax = 20;

		GameObject prefab = (GameObject)Resources.Load ("Prefabs/Tile");

		for(int x=0; x<=xMax; ++x)
		{
			for(int y=0; y<=yMax; ++y)
			{
				// xy の値を、幅をそろえて表示
				// プレハブからインスタンスを生成
				GameObject cube = Instantiate (prefab, new Vector3(x, y, 0), Quaternion.identity);
				m.checkin (x, y, cube); // WorldManagerに登録

				// wall
				cube.tag = WALL;
//				setColor (cube, Color.black);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (refresh && !isRunning) {
//			genMap ();
			refresh = false;

			//3.5秒後に実行する
//			StartCoroutine(DelayMethod(3.5f, () =>
//				{
//					Debug.Log("Delay call");
//					refresh = true;
//				}));
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
		clearCube ();

		// 部屋生成のmanager を作成
		List<RoomManager> rooms = new List<RoomManager>();
		m = new WorldManager (1000, 1000);
		RoomManager r1 = createRoom (new Vector3 (0, 15, 0), 5);
		RoomManager r2 = createRoom (new Vector3 (10, 25, 0), 6);
		RoomManager r3 = createRoom (new Vector3 (15, 15, 0), 5);
		RoomManager r4 = createRoom (new Vector3 (30, 25, 0), 6);
		RoomManager r5 = createRoom (new Vector3 (10, 42, 0), 6);
		RoomManager r6 = createRoom (new Vector3 (22, 38, 0), 6);
		RoomManager r7 = createRoom (new Vector3 (33, 45, 0), 6);
		RoomManager r8 = createRoom (new Vector3 (33, 35, 0), 4);
		rooms.Add (r1);
		rooms.Add (r2);
		rooms.Add (r3);
		rooms.Add (r4);
		rooms.Add (r5);
		rooms.Add (r6);
		rooms.Add (r7);
		rooms.Add (r8);

		// set dots
		StartCoroutine(DelayMethod(2.0f, () =>
			{
				createDots(rooms);
			}));

		// road
		StartCoroutine(DelayMethod(5.5f, () =>
			{
				StartCoroutine(createBlock (Road.create (r1, r2)));
				StartCoroutine(createBlock (Road.create (r3, r4)));
				StartCoroutine(createBlock (Road.create (r2, r3)));
				StartCoroutine(createBlock (Road.create (r2, r5)));
				StartCoroutine(createBlock (Road.create (r5, r6)));
				StartCoroutine(createBlock (Road.create (r6, r7)));
				StartCoroutine(createBlock (Road.create (r6, r8)));
			}));
		

		//1.5秒後に実行して壁を消す
		StartCoroutine(DelayMethod(11.5f, () =>
			{
				clearWall();
			}));
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
//				m.checkin ((int)posBase.x + x, (int)posBase.y + y); // WorldManagerに登録
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
				setColor (cube, Color.grey);
				yield return new WaitForSeconds (0f);
			}
			isRunning = false;

			//0.5秒後に実行して壁を消す
//			StartCoroutine(DelayMethod(0.5f, () =>
//				{
//					clearWall();
//				}));
//
//			break;
		}
	}

	void clearWall(){
		GameObject[] walls = GameObject.FindGameObjectsWithTag (WALL);
		foreach (GameObject o in walls) {Destroy (o);}
	}

	void clearCube(){
		GameObject[] walls = GameObject.FindGameObjectsWithTag (WALL);
		GameObject[] floors = GameObject.FindGameObjectsWithTag (FLOOR);
		GameObject[] roads = GameObject.FindGameObjectsWithTag (ROAD);
		foreach (GameObject o in walls) {Destroy (o);}
		foreach (GameObject o in floors) {Destroy (o);}
		foreach (GameObject o in roads) {Destroy (o);}
	}

	// 全域木の線を引く
	void createDots (List<RoomManager> rooms){
		// dotを打つ
		GameObject prefab = (GameObject)Resources.Load ("Prefabs/Cube");
		List<GameObject> spherList = new List<GameObject>();
		foreach (RoomManager room in rooms) {
			Vector2 center = room.getCenter ();
			GameObject cube = Instantiate (prefab, new Vector3 (center.x, center.y, -1F), Quaternion.identity);
			cube.tag = DOT;
			setColor (cube, Color.green);
			spherList.Add (cube);
		}

		StartCoroutine(DelayMethod(0.7f, () =>
			{
				// 線を引く
				GameObject c = GameObject.FindGameObjectsWithTag ("MainCamera")[0];
				LineRenderer lineRenderer = c.AddComponent<LineRenderer> ();;
				lineRenderer.material.color = Color.green;
				lineRenderer.SetWidth (0.3f, 0.3f);
				lineRenderer.SetVertexCount (14);
				lineRenderer.SetPosition (0, spherList[0].transform.position);
				lineRenderer.SetPosition (1, spherList[1].transform.position);
				lineRenderer.SetPosition (2, spherList[2].transform.position);
				lineRenderer.SetPosition (3, spherList[3].transform.position);
				lineRenderer.SetPosition (4, spherList[2].transform.position);
				lineRenderer.SetPosition (5, spherList[1].transform.position);
				lineRenderer.SetPosition (5, spherList[2].transform.position);
				lineRenderer.SetPosition (6, spherList[1].transform.position);
				lineRenderer.SetPosition (7, spherList[4].transform.position);
				lineRenderer.SetPosition (8, spherList[4].transform.position);
				lineRenderer.SetPosition (9, spherList[5].transform.position);
				lineRenderer.SetPosition (10, spherList[5].transform.position);
				lineRenderer.SetPosition (11, spherList[6].transform.position);
				lineRenderer.SetPosition (12, spherList[5].transform.position);
				lineRenderer.SetPosition (13, spherList[7].transform.position);

				// 線を消す
				StartCoroutine(DelayMethod(4.3f, () =>
					{
						Destroy(lineRenderer);
					}));
				// dotを消す
				StartCoroutine(DelayMethod(7.3f, () =>
					{
						GameObject[] dots = GameObject.FindGameObjectsWithTag (DOT);
						foreach (GameObject o in dots) {Destroy (o);}
					}));

			}));

	}
}
