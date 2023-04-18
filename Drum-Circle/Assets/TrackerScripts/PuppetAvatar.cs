using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using System.Text;

public class PuppetAvatar : MonoBehaviour
{
    public TrackerHandler KinectDevice;
    Dictionary<JointId, Quaternion> absoluteOffsetMap1;
    Dictionary<JointId, Quaternion> absoluteOffsetMap2;
    Dictionary<JointId, Quaternion> absoluteOffsetMap3;
    Animator PuppetAnimator1;
    Animator PuppetAnimator2;
    Animator PuppetAnimator3;
    public Transform CharacterRootTransform1;
    public Transform CharacterRootTransform2;
    public Transform CharacterRootTransform3;
    public float OffsetY1;
    public float OffsetZ1;
    public float OffsetY2;
    public float OffsetZ2;
    public float OffsetY3;
    public float OffsetZ3;
    private static HumanBodyBones MapKinectJoint(JointId joint)
    {
        // https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints
        switch (joint)
        {
            case JointId.Pelvis: return HumanBodyBones.Hips;
            case JointId.SpineNavel: return HumanBodyBones.Spine;
            case JointId.SpineChest: return HumanBodyBones.Chest;
            case JointId.Neck: return HumanBodyBones.Neck;
            case JointId.Head: return HumanBodyBones.Head;
            case JointId.HipLeft: return HumanBodyBones.LeftUpperLeg;
            case JointId.KneeLeft: return HumanBodyBones.LeftLowerLeg;
            case JointId.AnkleLeft: return HumanBodyBones.LeftFoot;
            case JointId.FootLeft: return HumanBodyBones.LeftToes;
            case JointId.HipRight: return HumanBodyBones.RightUpperLeg;
            case JointId.KneeRight: return HumanBodyBones.RightLowerLeg;
            case JointId.AnkleRight: return HumanBodyBones.RightFoot;
            case JointId.FootRight: return HumanBodyBones.RightToes;
            case JointId.ClavicleLeft: return HumanBodyBones.LeftShoulder;
            case JointId.ShoulderLeft: return HumanBodyBones.LeftUpperArm;
            case JointId.ElbowLeft: return HumanBodyBones.LeftLowerArm;
            case JointId.WristLeft: return HumanBodyBones.LeftHand;
            case JointId.ClavicleRight: return HumanBodyBones.RightShoulder;
            case JointId.ShoulderRight: return HumanBodyBones.RightUpperArm;
            case JointId.ElbowRight: return HumanBodyBones.RightLowerArm;
            case JointId.WristRight: return HumanBodyBones.RightHand;
            default: return HumanBodyBones.LastBone;
        }
    }
    private void Start()
    {
        PuppetAnimator1 = transform.GetChild(0).GetComponent<Animator>();
        Transform _rootJointTransform1 = CharacterRootTransform1;

        PuppetAnimator2 = transform.GetChild(1).GetComponent<Animator>();
        Transform _rootJointTransform2 = CharacterRootTransform2;
       
        PuppetAnimator3 = transform.GetChild(2).GetComponent<Animator>();
        Transform _rootJointTransform3 = CharacterRootTransform3;

        absoluteOffsetMap1 = new Dictionary<JointId, Quaternion>();
        absoluteOffsetMap2 = new Dictionary<JointId, Quaternion>();
        absoluteOffsetMap3 = new Dictionary<JointId, Quaternion>();
        for (int i = 0; i < (int)JointId.Count; i++)
        {
            HumanBodyBones hbb = MapKinectJoint((JointId)i);
            if (hbb != HumanBodyBones.LastBone)
            {
                Transform transform = PuppetAnimator1.GetBoneTransform(hbb);
                Quaternion absOffset = GetSkeletonBone(PuppetAnimator1, transform.name).rotation;
                // find the absolute offset for the tpose
                while (!ReferenceEquals(transform, _rootJointTransform1))
                {
                    transform = transform.parent;
                    absOffset = GetSkeletonBone(PuppetAnimator1, transform.name).rotation * absOffset;
                }
                absoluteOffsetMap1[(JointId)i] = absOffset;
            }
            
            if (hbb != HumanBodyBones.LastBone)
            {
                Transform transform = PuppetAnimator2.GetBoneTransform(hbb);
                Quaternion absOffset = GetSkeletonBone(PuppetAnimator2, transform.name).rotation;
                // find the absolute offset for the tpose
                while (!ReferenceEquals(transform, _rootJointTransform2))
                {
                    transform = transform.parent;
                    absOffset = GetSkeletonBone(PuppetAnimator2, transform.name).rotation * absOffset;
                }
                absoluteOffsetMap2[(JointId)i] = absOffset;
            }

            if (hbb != HumanBodyBones.LastBone)
            {
                Transform transform = PuppetAnimator3.GetBoneTransform(hbb);
                Quaternion absOffset = GetSkeletonBone(PuppetAnimator3, transform.name).rotation;
                // find the absolute offset for the tpose
                while (!ReferenceEquals(transform, _rootJointTransform3))
                {
                    transform = transform.parent;
                    absOffset = GetSkeletonBone(PuppetAnimator3, transform.name).rotation * absOffset;
                }
                absoluteOffsetMap3[(JointId)i] = absOffset;
            }
        }

    }

    private static SkeletonBone GetSkeletonBone(Animator animator, string boneName)
    {
        int count = 0;
        StringBuilder cloneName = new StringBuilder(boneName);
        cloneName.Append("(Clone)");
        foreach (SkeletonBone sb in animator.avatar.humanDescription.skeleton)
        {
            if (sb.name == boneName || sb.name == cloneName.ToString())
            {
                return animator.avatar.humanDescription.skeleton[count];
            }
            count++;
        }
        return new SkeletonBone();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        for (int j = 0; j < (int)JointId.Count; j++)
        {
            if (MapKinectJoint((JointId)j) != HumanBodyBones.LastBone && absoluteOffsetMap1.ContainsKey((JointId)j))
            {

                // Vector3 RootPosition = new Vector3(0.0f, 0.5f, 0.0f);
                Vector3 RootPosition = KinectDevice.pelvis1;
                // get the absolute offset
                Quaternion absOffset = absoluteOffsetMap1[(JointId)j];
                Transform finalJoint = PuppetAnimator1.GetBoneTransform(MapKinectJoint((JointId)j));

                finalJoint.rotation = absOffset * Quaternion.Inverse(absOffset) * KinectDevice.absoluteJointRotations1[j] * absOffset;
                
                if (j == 0)
                {
                    UnityEngine.Debug.Log("root1: " + RootPosition.x + RootPosition.y);
                    // character root plus translation reading from the kinect, plus the offset from the script public variables
                    finalJoint.position = CharacterRootTransform1.position + new Vector3(RootPosition.x, RootPosition.y + OffsetY1, RootPosition.z - OffsetZ1);
                    // finalJoint.position = CharacterRootTransform1.position + new Vector3(transform.GetChild(0).position.x, transform.GetChild(0).position.y + OffsetY1, transform.GetChild(0).position.z - OffsetZ1);
                }
            }
            if (MapKinectJoint((JointId)j) != HumanBodyBones.LastBone && absoluteOffsetMap2.ContainsKey((JointId)j))
            {
                // Vector3 RootPosition = new Vector3(-2.0f, 0.5f, 0.0f);
                Vector3 RootPosition = KinectDevice.pelvis2;
                // get the absolute offset
                Quaternion absOffset = absoluteOffsetMap2[(JointId)j];
                Transform finalJoint = PuppetAnimator2.GetBoneTransform(MapKinectJoint((JointId)j));

                finalJoint.rotation = absOffset * Quaternion.Inverse(absOffset) * KinectDevice.absoluteJointRotations2[j] * absOffset;
                
                if (j == 0)
                {
                    UnityEngine.Debug.Log("root2: " + RootPosition.x + RootPosition.y);

                    // character root plus translation reading from the kinect, plus the offset from the script public variables
                    finalJoint.position = CharacterRootTransform2.position + new Vector3(RootPosition.x, RootPosition.y + OffsetY2, RootPosition.z - OffsetZ2);                    
                    // finalJoint.position = CharacterRootTransform1.position + new Vector3(transform.GetChild(1).position.x, transform.GetChild(1).position.y + OffsetY2, transform.GetChild(1).position.z - OffsetZ2);
                }
            }
            if (MapKinectJoint((JointId)j) != HumanBodyBones.LastBone && absoluteOffsetMap3.ContainsKey((JointId)j))
            {
                // Vector3 RootPosition = new Vector3(-4.0f, 0.5f, 0.0f);
                Vector3 RootPosition = KinectDevice.pelvis3;
                // get the absolute offset
                Quaternion absOffset = absoluteOffsetMap3[(JointId)j];
                Transform finalJoint = PuppetAnimator3.GetBoneTransform(MapKinectJoint((JointId)j));

                finalJoint.rotation = absOffset * Quaternion.Inverse(absOffset) * KinectDevice.absoluteJointRotations3[j] * absOffset;
                
                if (j == 0)
                {
                    UnityEngine.Debug.Log("root3: " + RootPosition.x + RootPosition.y);

                    // character root plus translation reading from the kinect, plus the offset from the script public variables
                    finalJoint.position = CharacterRootTransform3.position + new Vector3(RootPosition.x, RootPosition.y + OffsetY3, RootPosition.z - OffsetZ3);                    
                    // finalJoint.position = CharacterRootTransform1.position + new Vector3(transform.GetChild(1).position.x, transform.GetChild(1).position.y + OffsetY2, transform.GetChild(1).position.z - OffsetZ2);
                }
            }
        }
    }
}
