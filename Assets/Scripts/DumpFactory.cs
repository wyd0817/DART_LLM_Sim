using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dump info for generation
public struct DumpInfo
{
    public string name;
    public float x;
    public float y;
    public float z;
    public float theta;
}

public class DumpFactory : MonoBehaviour
{

    [SerializeField]
    private GameObject dump;
    [SerializeField]
    private string dump_type = "c30r";

    [SerializeField]
    private int dumpCount = 6; // Number of dump trucks to spawn
    [SerializeField]
    private float zCoordinate = -20f; // Z coordinate of the dump trucks

    // Start is called before the first frame update
    void Start()
    {

        // Define dump info
        // TODO: load from json
        List<DumpInfo> dumpInfos = new List<DumpInfo>();
        float dist = 8f;
        float startX = -dist * (dumpCount - 1) / 2; // Adjust starting point to center

        for (int i = 0; i < dumpCount; i++)
        {
            DumpInfo dumpInfo = new DumpInfo();
            dumpInfo.name = dump_type + "_" + i.ToString();
            dumpInfo.x = startX + dist * i;
            dumpInfo.y = 0;
            dumpInfo.z = zCoordinate;
            dumpInfo.theta = 0;
            dumpInfos.Add(dumpInfo);
        }


        // Instantiation
        dump.SetActive(false);
        foreach (var dumpInfo in dumpInfos)
        {
            var translation = new Vector3(dumpInfo.x, dumpInfo.y, dumpInfo.z);
            var rotation = Quaternion.AngleAxis(dumpInfo.theta, Vector3.up);
            var dump_obj = Instantiate(dump, translation, rotation);
            Debug.Log("Instantiate:");
            Debug.Log(dump_obj);

            DiffDriveController diffdrive_script = dump_obj.GetComponent<DiffDriveController>();
            Debug.Log(diffdrive_script);
            var robotName = dumpInfo.name;
            diffdrive_script.robotName = robotName;
            diffdrive_script.TwistTopicName = robotName + "/tracks/cmd_vel";
            diffdrive_script.OdomTopicName = robotName + "/odom";
            diffdrive_script.childFrameName = robotName + "/tracks/base_link";
            JointStatePublisher joint_script = dump_obj.GetComponent<JointStatePublisher>();
            Debug.Log(joint_script);
            joint_script.topicName = robotName + "/joint_states";


            var base_link_obj = dump_obj.transform.Find("base_link");
            var pose_pub_script = base_link_obj.GetComponent<PoseStampedPublisher>();
            pose_pub_script.robotName = robotName;
            pose_pub_script.topicName = robotName + "/base_link/pose";

            var vessel_link_obj = base_link_obj.transform.Find("vessel_link");
            Debug.Log(vessel_link_obj);
            var vessel_script = vessel_link_obj.GetComponent<VesselController>();
            vessel_script.DumpTopicName = robotName + "/vessel/cmd";
            Debug.Log(vessel_script);
            //
            dump_obj.SetActive(true);
        }
        dump.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
