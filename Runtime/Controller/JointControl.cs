using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp;

public class JointControl : MonoBehaviour
{
    RosSharp.Control.Controller controller;

    public RosSharp.Control.RotationDirection direction;
    public RosSharp.Control.ControlType controltype;
    public float speed ;
    public float torque ;
    public float acceleration;
    public ArticulationBody joint;


    void Start()
    {
        direction = 0;
        controller = (RosSharp.Control.Controller)this.GetComponentInParent(typeof(RosSharp.Control.Controller));
        joint = this.GetComponent<ArticulationBody>();
        controller.UpdateControlType(this);
        speed = controller.speed;
        torque = controller.torque;
        acceleration = controller.acceleration;
    }

    void FixedUpdate(){

        speed = controller.speed;
        torque = controller.torque;
        acceleration = controller.acceleration;


        if(joint.jointType != ArticulationJointType.FixedJoint)
        {
            if (controltype == RosSharp.Control.ControlType.PositionControl)
            {
                ArticulationDrive currentDrive = joint.xDrive;
                if((int)direction != 0)
                {
                    float newTargetDelta = (int)direction * Time.fixedDeltaTime * speed;
                    if(currentDrive.upperLimit == 0 && currentDrive.lowerLimit == 0)//Motion is free
                    {
                        currentDrive.target += newTargetDelta;
                    }
                    else//Motion is limits
                    {
                        if (newTargetDelta + currentDrive.target <= currentDrive.upperLimit && newTargetDelta + currentDrive.target >= currentDrive.lowerLimit){
                            currentDrive.target += newTargetDelta;
                        }
                    }
                    joint.xDrive = currentDrive;
                }
            }
            
        }
    }
}
