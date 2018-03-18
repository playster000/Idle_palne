using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fighter : MonoBehaviour {

	[HideInInspector]
	public float sideSpeed;
	private Vector2 goTo;
	public RectTransform rTransform;
	private Vector2 startPos;
	float totalMoved;
	private Image fighter;
	private ParticleSystem fighterTrail;
	private BgMover bgMover;
	private bool goGlobal;

	// Use this for initialization
	void Start () {
		sideSpeed = 10;
		rTransform = GetComponent<RectTransform> ();
		fighter = GetComponent<Image> ();
		startPos = rTransform.localPosition;
		fighterTrail = GetComponentInChildren<ParticleSystem> ();
		bgMover = transform.parent.parent.parent.GetComponentInChildren<BgMover> ();
	}
	
	// Update is called once per frame
	void Update () {
		goToValue ();
		fighterTrail.startSpeed = bgMover.speed * 10f>50 ? bgMover.speed * 10f : 50f;
	}

	public void moveTo(Vector2 value,bool isGlobalPos){
		goTo = value;
		if (isGlobalPos)
			goGlobal = true;
		else
			goGlobal = false;
	}

	void goToValue(){
		//rTransform.localPosition = new Vector2 (rTransform.localPosition.x,
		//	rTransform.localPosition.y + speed);

		if (!goGlobal) {
			float speed = ((Time.deltaTime*6f) * sideSpeed);
			if(Lab.getEnergy(0)>=400)
				speed=((Time.deltaTime*7.5f) * sideSpeed);
			rTransform.localPosition = Vector3.MoveTowards (rTransform.localPosition, new Vector2 (startPos.x, goTo.y), speed);
			totalMoved += speed;
		} else {
			float speed = ((Time.deltaTime*12f) * sideSpeed);
			transform.position = Vector3.MoveTowards (transform.position, new Vector2 (transform.position.x, goTo.y), speed);
			rTransform.localPosition = new Vector3 (rTransform.localPosition.x,rTransform.localPosition.y,0);
		}
		
	}

	void returnToStart(){
		goTo = startPos;
	}

	public IEnumerator imageFLicker(float delayBetweenFlicker,float timesToFlicker){
		float count = 0;
		while(true){
			fighter.enabled = !fighter.enabled;
			fighterTrail.enableEmission = !fighterTrail.enableEmission;
			count += 0.5f;
			yield return new WaitForSeconds (delayBetweenFlicker);
			if (count > timesToFlicker)
				break;
		}
		fighter.enabled = true;
		fighterTrail.enableEmission = true;
	}

	public void setSideSpeed(float value){
		sideSpeed = value;
	}

}
