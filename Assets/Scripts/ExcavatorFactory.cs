using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Excavator info for generation
public struct ExcavatorInfo
{
    public string name;
    public float x;
    public float y;
    public float z;
    public float theta;
}

public class ExcavatorFactory : MonoBehaviour
{

    [SerializeField]
    private GameObject excavator;
    [SerializeField]
    private string excavator_type = "zx120";
    [SerializeField]
    private int excavatorCount = 2; // Number of excavators to spawn

    [SerializeField]
    private int dumpCount = 6; // Number of dump trucks (c30r) to account for

    [SerializeField]
    private float zCoordinate = -20f; // Z coordinate of the excavators

    // Start is called before the first frame update
    void Start()
    {
        // Define Excavator info
        List<ExcavatorInfo> excavatorInfos = new List<ExcavatorInfo>();
        float dist = 8f;
        float startX = -dist * (dumpCount - 1) / 2; // Adjust starting point to center based on dump count

        // Place excavators on the left and right sides of the dump trucks
        for (int i = 0; i < excavatorCount; i++)
        {
            ExcavatorInfo excavatorInfo = new ExcavatorInfo();
            excavatorInfo.name = excavator_type + "_" + i.ToString();
            if (i % 2 == 0)
            {
                // Even index, place on the left
                excavatorInfo.x = startX - dist * (i / 2 + 1);
            }
            else
            {
                // Odd index, place on the right
                excavatorInfo.x = startX + dist * (i / 2 + dumpCount);
            }
            excavatorInfo.y = 0f;
            excavatorInfo.z = zCoordinate;
            excavatorInfo.theta = 0f;
            excavatorInfos.Add(excavatorInfo);
        }

        // Instantiation
        excavator.SetActive(false);
        foreach(var excavatorInfo in excavatorInfos)
        {
            var translation = new Vector3(excavatorInfo.x, excavatorInfo.y, excavatorInfo.z);
            var rotation = Quaternion.AngleAxis(excavatorInfo.theta, Vector3.up);
            var excavator_obj = Instantiate(excavator, translation, rotation);
            Debug.Log("Instantiate excavator:");
            Debug.Log(excavator_obj);

            DiffDriveController diffdrive_script = excavator_obj.GetComponent<DiffDriveController>();
            Debug.Log(diffdrive_script);
            var robotName = excavatorInfo.name;
            diffdrive_script.robotName = robotName;
            diffdrive_script.TwistTopicName = robotName + "/tracks/cmd_vel";
            diffdrive_script.OdomTopicName = robotName + "/odom";
            diffdrive_script.childFrameName = robotName + "/tracks/base_link";
            JointStatePublisher joint_script = excavator_obj.GetComponent<JointStatePublisher>();
            Debug.Log(joint_script);
            joint_script.topicName = robotName + "/joint_states";

            var base_link_obj = excavator_obj.transform.Find("base_link");
            var pose_pub_script = base_link_obj.GetComponent<PoseStampedPublisher>();
            pose_pub_script.robotName = robotName;
            pose_pub_script.topicName = robotName + "/base_link/pose";

            var body_link_obj = base_link_obj.transform.Find("body_link");
            var body_joint_script = body_link_obj.GetComponent<JointPosController>();
            body_joint_script.setpointTopicName = robotName + "/swing/cmd";

            var boom_link_obj = body_link_obj.transform.Find("boom_link");
            var boom_joint_script = boom_link_obj.GetComponent<JointPosController>();
            boom_joint_script.setpointTopicName = robotName + "/boom/cmd";

            var arm_link_obj = boom_link_obj.transform.Find("arm_link");
            var arm_joint_script = arm_link_obj.GetComponent<JointPosController>();
            arm_joint_script.setpointTopicName = robotName + "/arm/cmd";

            var bucket_link_obj = arm_link_obj.transform.Find("bucket_link");
            var bucket_joint_script = bucket_link_obj.GetComponent<JointPosController>();
            bucket_joint_script.setpointTopicName = robotName + "/bucket/cmd";

            // Joint control
            FollowJointTrajectoryAction joint_action_script = excavator_obj.GetComponent<FollowJointTrajectoryAction>();
            Debug.Log(joint_action_script);
            joint_action_script.fakeControllerTopicName = robotName + "/fake_controller_joint_states";
            
            excavator_obj.SetActive(true);
        }
        excavator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
