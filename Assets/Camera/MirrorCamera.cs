using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference: http://wiki.unity3d.com/index.php?title=InvertCamera
public class MirrorCamera : MonoBehaviour {

	private Camera cam;

	void Start() {
		cam = GetComponent<Camera> ();
	}

	void OnPreCull () {
		cam.ResetWorldToCameraMatrix ();
		cam.ResetProjectionMatrix ();
		Vector3 v = new Vector3 (-1, 1, 1);
		Matrix4x4 mat= Matrix4x4.Scale (v);
		cam.projectionMatrix = cam.projectionMatrix * mat;
	}

	void OnPreRender () {
		GL.invertCulling = true;
	}

	void OnPostRender () {
		GL.invertCulling = false;
	}
}
