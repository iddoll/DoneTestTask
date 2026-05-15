using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoatController : MonoBehaviour
{
    [SerializeField] private Rigidbody boat;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float steerPower;
    [SerializeField] private GameObject materialScreen;
    [SerializeField] private Transform pool;
    

    private float steer = 1;
    private float speed;

    private bool isLeftBtnPressed;
    private bool isRightBtnPressed;
    private bool isUpBtnPressed;
    private bool isDownBtnPressed;
    private void FixedUpdate()
    {
        if (isLeftBtnPressed)
        {
            boat.AddTorque(boat.transform.forward * -steerPower * 0.05f);
        }

        if (isRightBtnPressed)
        {
            boat.AddTorque(boat.transform.forward * steerPower * 0.05f);
        }

        if (isUpBtnPressed)
        {
            steer = 1;
        }
        
        if (isDownBtnPressed)
        {
            steer = -1;
        }
        
        boat.AddForce(steer * boat.transform.right * maxSpeed);
        Vector3 newOffset = pool.InverseTransformPoint(boat.transform.position);
        
        float newX = (newOffset.z + 4.16f) * (-0.12f - 0.64f) / (4.16f + 4.16f) + 0.64f;
        float newY = (newOffset.x + 3.68f) * (0.40f + 0.35f) / (3.48f + 3.68f) - 0.35f;
        
        materialScreen.GetComponent<Renderer>().material.SetTextureOffset("_Texture",
            new Vector2(newX, newY));
        
        steer = 0;

    }

    public void leftBtnDown()
    {
        isLeftBtnPressed = true;
    }
    
    public void rightBtnDown()
    {
        isRightBtnPressed = true;
    }
    
    public void upBtnDown()
    {
        isUpBtnPressed = true;
    }
    
    public void downBtnDown()
    {
        isDownBtnPressed = true;
    }
    
    public void leftBtnUp()
    {
        isLeftBtnPressed = false;
    }
    
    public void rightBtnUp()
    {
        isRightBtnPressed = false;
    }
    
    public void upBtnUp()
    {
        isUpBtnPressed = false;
    }
    
    public void downBtnUp()
    {
        isDownBtnPressed = false;
    }
    
}
