using UnityEngine;
using System.Collections;

public class camera : MonoBehaviour {

	[SerializeField] Transform target1 = null, target2 = null;
	[SerializeField] Vector2 offset = new Vector2(1, 1);

	private float screenAspect = 0; 
	private Camera _camera = null;
	public GameObject cube1;
	public GameObject cube2;

	void Awake()
	{
		screenAspect = (float)Screen.height / Screen.width;
		_camera = GetComponent<Camera> ();
		cube1 = GameObject.Find("target1");
		cube2 = GameObject.Find("target2");
	}

	void Update () 
	{
		UpdateCameraPosition ();
		UpdateOrthographicSize ();
	}

	void UpdateCameraPosition()
	{
		// 2点間の中心点からカメラの位置を更新
		Vector3 center = Vector3.Lerp (cube1.transform.position, cube2.transform.position, 0.5f);
		transform.position = center + Vector3.forward * -10;
	}

	void UpdateOrthographicSize()
	{
		// ２点間のベクトルを取得
		Vector3 targetsVector = AbsPositionDiff (cube1.transform, cube2.transform) + (Vector3)offset;

		// アスペクト比が縦長ならyの半分、横長ならxとアスペクト比でカメラのサイズを更新
		float targetsAspect = targetsVector.y / targetsVector.x;
		float targetOrthographicSize = 0;
		if ( screenAspect < targetsAspect) {
			targetOrthographicSize = targetsVector.y * 0.5f + 10f;
		} else {
			targetOrthographicSize = targetsVector.x * (1/_camera.aspect) * 0.5f + 10f;
		}
		_camera.orthographicSize =  targetOrthographicSize;
	}

	Vector3 AbsPositionDiff(Transform target1, Transform target2)
	{
		Vector3 targetsDiff = target1.position - target2.position;
		return new Vector3(Mathf.Abs(targetsDiff.x), Mathf.Abs(targetsDiff.y));
	}
}