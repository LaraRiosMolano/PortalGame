﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam1Script : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.position = new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(0, 0, 0);
        transform.Rotate (new Vector3(0, 0, 0));
    }
}
