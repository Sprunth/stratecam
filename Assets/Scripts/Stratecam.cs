using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Stratecam : MonoBehaviour {
    // -------------------------- Configuration --------------------------
    public Terrain terrain;

    public float panSpeed = 15.0f;
    public float zoomSpeed = 100.0f;
    public float rotationSpeed = 50.0f;

    public float mousePanMultiplier = 0.1f;
    public float mouseRotationMultiplier = 0.2f;
    public float mouseZoomMultiplier = 5.0f;

    public float minZoomDistance = 20.0f;
    public float maxZoomDistance = 200.0f;
    public float smoothingFactor = 0.1f;
    public float goToSpeed = 0.1f;

    public bool useKeyboardInput = true;
    public bool useMouseInput = true;
    public bool adaptToTerrainHeight = true;
    public bool increaseSpeedWhenZoomedOut = true;
    public bool correctZoomingOutRatio = true;
    public bool smoothing = true;
    public bool allowDoubleClickMovement = false;

    public GameObject objectToFollow;
    public Vector3 cameraTarget;

    // -------------------------- Private Attributes --------------------------
    private float currentCameraDistance;
    private Vector3 lastMousePos;
    private Vector3 lastPanSpeed = Vector3.zero;
    private Vector3 goingToCameraTarget = Vector3.zero;
    private bool doingAutoMovement = false;
    private DoubleClickDetector doubleClickDetector;

    // -------------------------- Public Methods --------------------------
	void  Start (){
		currentCameraDistance = minZoomDistance + ((maxZoomDistance - minZoomDistance) / 2.0f);
		lastMousePos = Vector3.zero;
		doubleClickDetector = new DoubleClickDetector();
	}

	void  Update (){
		if (allowDoubleClickMovement) {
			doubleClickDetector.Update();
			UpdateDoubleClick();
		}
		UpdatePanning();
		UpdateRotation();
		UpdateZooming();
		UpdatePosition();
		UpdateAutoMovement();
		lastMousePos = Input.mousePosition;
	}

	public void  GoTo (Vector3 position){
		doingAutoMovement = true;
		goingToCameraTarget = position;
		objectToFollow = null;
	}

	public void Follow(GameObject gameObjectToFollow){
		objectToFollow = gameObjectToFollow;
	}

	// -------------------------- Private Methods --------------------------
	private void  UpdateDoubleClick (){
		if (doubleClickDetector.IsDoubleClick() && terrain && terrain.GetComponent<Collider>()) {
			float cameraTargetY= cameraTarget.y;
			
			Collider collider = terrain.GetComponent<Collider>();
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			Vector3 pos;
			
			if (collider.Raycast(ray, out hit, Mathf.Infinity)) {
				pos = hit.point;
				pos.y = cameraTargetY;
				GoTo(pos);
			}
		}
	}

	private void  UpdatePanning (){
		Vector3 moveVector = new Vector3(0, 0, 0);
		if (useKeyboardInput) {
			if (Input.GetKey(KeyCode.A)) {
				moveVector += new Vector3(-1, 0, 0);
			} 
			if (Input.GetKey(KeyCode.S)) {
				moveVector += new Vector3(0, 0, -1);
			} 
			if (Input.GetKey(KeyCode.D)) {
				moveVector += new Vector3(1, 0, 0);
			} 
			if (Input.GetKey(KeyCode.W)) {
				moveVector += new Vector3(0, 0, 1);
			}
		}
		
		if (useMouseInput) {
			if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftShift)) {
				Vector3 deltaMousePos = (Input.mousePosition - lastMousePos);
				moveVector += new Vector3(-deltaMousePos.x, 0, -deltaMousePos.y) * mousePanMultiplier;
			}	
		}
		
		if (moveVector != Vector3.zero) {
			objectToFollow = null;
			doingAutoMovement = false;
		}
		
		var effectivePanSpeed = moveVector;
		if (smoothing) {
			effectivePanSpeed = Vector3.Lerp(lastPanSpeed, moveVector, smoothingFactor);
			lastPanSpeed = effectivePanSpeed;
		}
		
		float oldRotationX = transform.localEulerAngles.x;
        transform.localEulerAngles = new Vector3(0.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
		float panMultiplier = increaseSpeedWhenZoomedOut ? (Mathf.Sqrt(currentCameraDistance)) : 1.0f;
		cameraTarget = cameraTarget + transform.TransformDirection(effectivePanSpeed) * panSpeed * panMultiplier * Time.deltaTime;
        transform.localEulerAngles = new Vector3(oldRotationX, transform.localEulerAngles.y, transform.localEulerAngles.z);
	}

	private void  UpdateRotation (){
		float deltaAngleH = 0.0f;
		float deltaAngleV = 0.0f;
	
		if (useKeyboardInput) {
			if (Input.GetKey(KeyCode.Q)) {
				deltaAngleH = 1.0f;
			} 
			if (Input.GetKey(KeyCode.E)) {
				deltaAngleH = -1.0f;
			} 
		}

		if (useMouseInput) {
			if (Input.GetMouseButton(2) && !Input.GetKey(KeyCode.LeftShift)) {
				Vector3 deltaMousePos = (Input.mousePosition - lastMousePos);
				deltaAngleH += deltaMousePos.x * mouseRotationMultiplier;
				deltaAngleV -= deltaMousePos.y * mouseRotationMultiplier;
			}
		}

	    var newX = Mathf.Min(80.0f,
	        Mathf.Max(5.0f, transform.localEulerAngles.x + deltaAngleV*Time.deltaTime*rotationSpeed));
	    var newY = transform.localEulerAngles.y + deltaAngleH*Time.deltaTime*rotationSpeed;

        transform.localEulerAngles = new Vector3(newX, newY, transform.localEulerAngles.z);
	}

	private void  UpdateZooming (){
		float deltaZoom = 0.0f;
		if (useKeyboardInput) {
			if (Input.GetKey(KeyCode.F)) {
				deltaZoom = 1.0f;
			} 
			if (Input.GetKey(KeyCode.R)) {
				deltaZoom = -1.0f;
			} 
		}
		if (useMouseInput) {
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			deltaZoom -= scroll * mouseZoomMultiplier;
		}
		float zoomedOutRatio = correctZoomingOutRatio ? (currentCameraDistance - minZoomDistance) / (maxZoomDistance - minZoomDistance) : 0.0f;
		currentCameraDistance = Mathf.Max(minZoomDistance, Mathf.Min(maxZoomDistance, currentCameraDistance + deltaZoom * Time.deltaTime * zoomSpeed * (zoomedOutRatio * 2.0f + 1.0f)));
	}

	private void  UpdatePosition (){
		if (objectToFollow != null) {
			cameraTarget = Vector3.Lerp(cameraTarget, objectToFollow.transform.position, goToSpeed);
		}
		transform.position = cameraTarget;
		transform.Translate(Vector3.back * currentCameraDistance);
		
		if (adaptToTerrainHeight && terrain != null) {
            transform.position = new Vector3(transform.position.x,
                Mathf.Max(terrain.SampleHeight(transform.position) + terrain.transform.position.y + 10.0f, transform.position.y),
                transform.position.z);
        }
    }

	private void  UpdateAutoMovement (){
		if (doingAutoMovement) {
			cameraTarget = Vector3.Lerp(cameraTarget, goingToCameraTarget, goToSpeed);
			if (Vector3.Distance(goingToCameraTarget, cameraTarget) < 1.0f) {
				doingAutoMovement = false;
			}
		}
	}
}