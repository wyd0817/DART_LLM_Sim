//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Control
{
    [Serializable]
    public class PointHeadGoal : Message
    {
        public const string k_RosMessageName = "control_msgs/PointHead";
        public override string RosMessageName => k_RosMessageName;

        public Geometry.PointStampedMsg target;
        public Geometry.Vector3Msg pointing_axis;
        public string pointing_frame;
        public BuiltinInterfaces.DurationMsg min_duration;
        public double max_velocity;

        public PointHeadGoal()
        {
            this.target = new Geometry.PointStampedMsg();
            this.pointing_axis = new Geometry.Vector3Msg();
            this.pointing_frame = "";
            this.min_duration = new BuiltinInterfaces.DurationMsg();
            this.max_velocity = 0.0;
        }

        public PointHeadGoal(Geometry.PointStampedMsg target, Geometry.Vector3Msg pointing_axis, string pointing_frame, BuiltinInterfaces.DurationMsg min_duration, double max_velocity)
        {
            this.target = target;
            this.pointing_axis = pointing_axis;
            this.pointing_frame = pointing_frame;
            this.min_duration = min_duration;
            this.max_velocity = max_velocity;
        }

        public static PointHeadGoal Deserialize(MessageDeserializer deserializer) => new PointHeadGoal(deserializer);

        private PointHeadGoal(MessageDeserializer deserializer)
        {
            this.target = Geometry.PointStampedMsg.Deserialize(deserializer);
            this.pointing_axis = Geometry.Vector3Msg.Deserialize(deserializer);
            deserializer.Read(out this.pointing_frame);
            this.min_duration = BuiltinInterfaces.DurationMsg.Deserialize(deserializer);
            deserializer.Read(out this.max_velocity);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.target);
            serializer.Write(this.pointing_axis);
            serializer.Write(this.pointing_frame);
            serializer.Write(this.min_duration);
            serializer.Write(this.max_velocity);
        }

        public override string ToString()
        {
            return "PointHeadGoal: " +
            "\ntarget: " + target.ToString() +
            "\npointing_axis: " + pointing_axis.ToString() +
            "\npointing_frame: " + pointing_frame.ToString() +
            "\nmin_duration: " + min_duration.ToString() +
            "\nmax_velocity: " + max_velocity.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize, MessageSubtopic.Goal);
        }
    }
}