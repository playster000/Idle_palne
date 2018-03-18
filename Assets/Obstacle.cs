using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Obstacle : MonoBehaviour {

	private RectTransform rTransform;
	private Vector2 startPos;
	float totalMoved;
	private BgMover bgmover;
	private float width=258;
	[HideInInspector]
	public float spotToMove = 0;
	public Fighter fighter;
	public static bool canMoveWithMouse= true;
	bool exitMouseMove;

	// Use this for initialization
	void Start () {
		bgmover = transform.parent.parent.parent.GetComponentInChildren<BgMover> ();
		rTransform = GetComponent<RectTransform> ();
		startPos = rTransform.localPosition;
		spotToMove = Random.Range (-80, 80);
		rTransform.localPosition = new Vector2 (rTransform.localPosition.x,rTransform.localPosition.y+spotToMove);
	}
	
	// Update is called once per frame
	void Update () {
		totalMoved += ((Time.deltaTime*9)*bgmover.speed);
		rTransform.localPosition = new Vector2 (rTransform.localPosition.x - ((Time.deltaTime*9)*bgmover.speed),rTransform.localPosition.y);
		if(totalMoved>width+rTransform.rect.width){
			totalMoved = 0;
			rTransform.localPosition = startPos;
			spotToMove = Random.Range (-80, 80);
			rTransform.localPosition = new Vector2 (rTransform.localPosition.x,rTransform.localPosition.y+spotToMove);
			canMoveWithMouse = true;
		}
	}

	public void startMoveWithMouse(){
		exitMouseMove = false;
		StartCoroutine (moveWithMouse());
	}
	public void stopMoveWithMouse(){
		exitMouseMove = true;
	}

	IEnumerator moveWithMouse(){
		while (!exitMouseMove) {
			yield return new WaitForSeconds (0.05f);
			if (canMoveWithMouse) {
				Vector2 tempToMove = new Vector2 (Input.mousePosition.x * (640 / (float)(Screen.width))
				, Input.mousePosition.y * (360 / (float)(Screen.height)));
				//Debug.Log (tempToMove.y+" "+Input.mousePosition.y);
				Vector2 tempToMove2 = rTransform.localPosition;
				fighter.moveTo (tempToMove, true);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "fighter") {
			bgmover.speed = 1;
			fighter.StartCoroutine (fighter.imageFLicker (0.2f, 6));
		} else {//sensor
			fighter.moveTo (rTransform.localPosition,false);
			canMoveWithMouse = false;
		}
	}

}
