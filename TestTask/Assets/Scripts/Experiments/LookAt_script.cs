using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt_script : MonoBehaviour
{
	[SerializeField] private Transform objToRotate;
    	void Update()
    	{
        	objToRotate.LookAt(Camera.main.transform.position);
    	}    
}
