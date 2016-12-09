using UnityEngine;
using System.Collections;

public class CameraSwitch : MonoBehaviour {
	// Camera array that holds a reference to every camera in the scene
	public Camera[] cameras;

	// Current camera 
	private int currentCameraIndex;


	// Use this for initialization
	void Start () {
		currentCameraIndex = 0;

		//Turn all cameras off, except first default one
		for(int i = 0; i < cameras.Length; i++)
		{
			cameras[i].gameObject.SetActive(false);
		}
		if(cameras.Length > 0)
		{
			cameras[0].gameObject.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.C))
		{
			//Cycle to the next camera
			currentCameraIndex++;

			//If cameraIndex is in bounds, set this camera active and last one inactive
			if(currentCameraIndex<cameras.Length)
			{
				cameras[currentCameraIndex - 1].gameObject.SetActive(false);
				cameras[currentCameraIndex].gameObject.SetActive(true);

			}

			//If last camera, cycle back to first camera
			else
			{
				cameras[currentCameraIndex - 1].gameObject.SetActive(false);
				currentCameraIndex = 0;
				cameras[currentCameraIndex].gameObject.SetActive(true);
			}
		}
	}
}
