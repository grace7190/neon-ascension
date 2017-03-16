using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFacingBlockDetector : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Block") {
			var script = other.GetComponent<Block> ();
			if (gameObject.name == "Player1Child") 
			{
				script.ChangeColor (Color.yellow, Block.ChangeColorDuration);
			}
			else
			{
				script.ChangeColor (Color.green, Block.ChangeColorDuration);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Block") {
			var script = other.GetComponent<Block> ();
			script.ChangeColor (script.BaseColor, Block.ChangeColorDuration);
		}
	}
}
