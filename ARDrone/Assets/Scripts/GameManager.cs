﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GameManager : MonoBehaviour
{

    public DroneController _DroneController;

    public Button _FlyButton;
    public Button _LandButton;

    public GameObject _Controls;

    public ARRaycastManager _RayCastManager;
    public ARPlaneManager _PlaneManager;
    List<ARRaycastHit> _HitResult = new List<ARRaycastHit>();
    public GameObject _Drone;

    struct DroneAnimationControls
    {
        public bool _moving;
        public bool _interpolatingAsc;
        public bool _interpolatingDesc;
        public float _axis;
        public float _direction;
    }

    DroneAnimationControls _MovingLeft;
    DroneAnimationControls _MovingBack;

    // Start is called before the first frame update
    void Start()
    {
        _FlyButton.onClick.AddListener(EventOnClickFlyButton);
        _LandButton.onClick.AddListener(EventOnClickLandButton);
    }

    // Update is called once per frame
    void Update()
    {
        //float speedX = Input.GetAxis("Horizontal");
        //float speedZ = Input.GetAxis("Vertical");

        UpdateControls(ref _MovingLeft);
        UpdateControls(ref _MovingBack);

        _DroneController.Move(_MovingLeft._axis * _MovingLeft._direction, _MovingBack._axis * _MovingBack._direction);
        if (_DroneController.IsIdle())
        {
            UpdateAR();
        }
    }

    void UpdateAR()
    {
        Vector3 positionScreenSpace = Camera.current.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        _RayCastManager.Raycast(positionScreenSpace, _HitResult, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds);

        if(_HitResult.Count > 0)
        {
            if (_PlaneManager.GetPlane(_HitResult[0].trackableId).alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp)
            {
                Pose pose = _HitResult[0].pose;
                _Drone.transform.position = pose.position;
                _Drone.SetActive(true);
            }
        }
    }

    void UpdateControls(ref DroneAnimationControls _controls)
    {
        if(_controls._moving || _controls._interpolatingAsc || _controls._interpolatingDesc)
        {
            if (_controls._interpolatingAsc)
            {
                _controls._axis += .05f;
                if (_controls._axis >= 1.0f)
                {
                    _controls._axis = 1.0f;
                    _controls._interpolatingAsc = false;
                    _controls._interpolatingDesc = true;
                }
            }else if (!_controls._moving)
            {
                _controls._axis -= .05f;
                if(_controls._axis <= 0.0f)
                {
                    _controls._axis = 0.0f;
                    _controls._interpolatingDesc = false;
                }
            }
        }
    }

    void EventOnClickFlyButton()
    {
        if (_DroneController.IsIdle())
        {
            _DroneController.TakeOff();
            _FlyButton.gameObject.SetActive(false);
            _Controls.SetActive(true);
        }
    }

    void EventOnClickLandButton()
    {
        if (_DroneController.isFlying())
        {
            _DroneController.Land();
            _FlyButton.gameObject.SetActive(true);
            _Controls.SetActive(false);
        }
    }

    public void EventOnClickLeftButtonPressed()
    {
        _MovingLeft._moving = true;
        _MovingLeft._interpolatingAsc = true;
        _MovingLeft._direction = -1.0f;
    }
    public void EventOnClickLeftButtonReleased()
    {
        _MovingLeft._moving = false;
    }
    public void EventOnClickRightButtonPressed()
    {
        _MovingLeft._moving = true;
        _MovingLeft._interpolatingAsc = true;
        _MovingLeft._direction = 1.0f;
    }
    public void EventOnClickRightButtonReleased()
    {
        _MovingLeft._moving = false;    
    }
    public void EventOnClickBackButtonPressed()
    {
        _MovingBack._moving = true;
        _MovingBack._interpolatingAsc = true;
        _MovingBack._direction = -1.0f;
    }
    public void EventOnClickBackButtonReleased()
    {
        _MovingBack._moving = false;
    }
    public void EventOnClickForwardButtonPressed()
    {
        _MovingBack._moving = true;
        _MovingBack._interpolatingAsc = true;
        _MovingBack._direction = 1.0f;
    }
    public void EventOnClickForwardButtonReleased()
    {
        _MovingBack._moving = false;
    }

}
