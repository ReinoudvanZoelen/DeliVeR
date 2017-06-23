using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Basic movement class for use with the game bikes.
/// </summary>
public class BikeMovement : MonoBehaviour
{
    public CharacterController Controller;
    public int ControllerForPlayerNumber = 0; //This is used to determine wich gets then next arduino that connects (ensures that arduino 1 will always be linked to player 1)
    string _ArduinoName;

    public float _TargetSpeed = 0f;        //The speed that the bike is aiming to move at. The actual speed lerps towards this speed, this is to prevent shocking accelerations from paddling.
    public float _ActualSpeed = 0f;        //The speed that moves the bike.
    public float _SpeedMultiplier = 1f;    //this is used to scale the speed with the gameworld.
    public float _SteeringSpeed = 30f;
    public int _Steering = 0;
    public float _Dragg = 1f;              //effectively works as dragg, facters into the speed at wich the bike slows down when not paddling and also determines the maximum speed.
    public float _Transversale = 1f;       //controles the speed at wich the bike's speed becomes the target speed.

    public float _VerticalDrag = 1f;
    public float gravity = 9.8f;

    void Start()
    {
        TryReservingArduino();

    }

    void TryReservingArduino()
    {
        _ArduinoName = ArduinoControl.ReserveArduino();
        if (_ArduinoName == null)
        {
            ArduinoControl.RegisterForNewArduino(ControllerForPlayerNumber, TryReservingArduino);
        }
        else
        {
            ArduinoControl.RegisterForEvents(_ArduinoName, OnNewData, OnDisconnect);
        }
    }

    /// <summary>
    /// Data format is pedals,buttons so 1,0 = completed rotation, no buttons pressed. 
    /// for the buttons 
    /// 0 = none 
    /// 1 = left
    /// 2 = middel
    /// 3 = right
    /// </summary>
    /// <param name="spdc"></param>
    void OnNewData(SerialPortDataContainer spdc)
    {
        string[] temp = spdc.LastValue.Split(',');
        _TargetSpeed += int.Parse(temp[0]) * _SpeedMultiplier;
        if (temp[1].Equals("1"))
        {
            _Steering = -1;
        }
        else if (temp[1].Equals("2"))
        {
            ThrowPackage player = GameObject.FindGameObjectWithTag("Player").GetComponent<ThrowPackage>();
            if (player.timer >= player.cooldown)
                player.Throw();
        }
        else if (temp[1].Equals("3"))
        {
            _Steering = 1;
        }
        else
        {
            _Steering = 0;
        }
    }
    private void OnDisconnect(string arduinoName)
    {
        if (_ArduinoName.Equals(arduinoName))
        {
            ArduinoControl.RegisterForNewArduino(ControllerForPlayerNumber, TryReservingArduino);
        }
    }

    void Update()
    {
        Player._ActualSpeed = _ActualSpeed;

        if (Input.GetKey(KeyCode.W))
        {
            _TargetSpeed = 0.3f;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            _Steering = -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            _TargetSpeed = 0;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _Steering = 1;
        }
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            _Steering = 0;
        }

        _TargetSpeed = Mathf.Lerp(_TargetSpeed, 0, _Dragg * Time.fixedDeltaTime); //apply dragg to the target speed.
        _ActualSpeed = Mathf.Lerp(_ActualSpeed, _TargetSpeed, _Transversale * Time.fixedDeltaTime); //Set the new _ActualSpeed.

        transform.Rotate(new Vector3(0, _Steering * (_SteeringSpeed * (1 + _ActualSpeed) * Time.fixedDeltaTime), 0)); //apply steering.

        Controller.Move(transform.forward * _ActualSpeed); //this is also where you would add gravitational movement.

        transform.Rotate(0, Input.GetAxis("Horizontal") * _Steering * Time.deltaTime, 0);
        Vector3 vel = transform.forward * Input.GetAxis("Vertical") * _ActualSpeed;
        if (Controller.isGrounded)
        {
            _VerticalDrag = 0; // grounded character has vSpeed = 0...
        }
        // apply gravity acceleration to vertical speed:
        _VerticalDrag -= gravity * Time.deltaTime;
        vel.y = _VerticalDrag; // include vertical speed in vel
        // convert vel to displacement and Move the character:
        Controller.Move(vel * Time.deltaTime);
        //You could add addition rotations for aligning to a sloped ground and for leaning the bike left to right while steering.
        //You could add addition rotations for aligning to a sloped ground and for leaning the bike left to right while steering.
    }
}
