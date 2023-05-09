using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;

public class TrackerHandler : MonoBehaviour
{
    public Dictionary<JointId, JointId> parentJointMap;
    Dictionary<JointId, Quaternion> basisJointMap;
    public Quaternion[] absoluteJointRotations1 = new Quaternion[(int)JointId.Count];
    public Quaternion[] absoluteJointRotations2 = new Quaternion[(int)JointId.Count];
    public Quaternion[] absoluteJointRotations3 = new Quaternion[(int)JointId.Count];
    public Vector3 pelvis1;
    public Vector3 pelvis2;
    public Vector3 pelvis3;
    public Dictionary<int, Quaternion[]> skeletonsRotations;
    public bool drawSkeletons = true;
    Quaternion Y_180_FLIP = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

    // Start is called before the first frame update
    void Awake()
    {
        parentJointMap = new Dictionary<JointId, JointId>();

        // pelvis has no parent so set to count
        parentJointMap[JointId.Pelvis] = JointId.Count;
        parentJointMap[JointId.SpineNavel] = JointId.Pelvis;
        parentJointMap[JointId.SpineChest] = JointId.SpineNavel;
        parentJointMap[JointId.Neck] = JointId.SpineChest;
        parentJointMap[JointId.ClavicleLeft] = JointId.SpineChest;
        parentJointMap[JointId.ShoulderLeft] = JointId.ClavicleLeft;
        parentJointMap[JointId.ElbowLeft] = JointId.ShoulderLeft;
        parentJointMap[JointId.WristLeft] = JointId.ElbowLeft;
        parentJointMap[JointId.HandLeft] = JointId.WristLeft;
        parentJointMap[JointId.HandTipLeft] = JointId.HandLeft;
        parentJointMap[JointId.ThumbLeft] = JointId.HandLeft;
        parentJointMap[JointId.ClavicleRight] = JointId.SpineChest;
        parentJointMap[JointId.ShoulderRight] = JointId.ClavicleRight;
        parentJointMap[JointId.ElbowRight] = JointId.ShoulderRight;
        parentJointMap[JointId.WristRight] = JointId.ElbowRight;
        parentJointMap[JointId.HandRight] = JointId.WristRight;
        parentJointMap[JointId.HandTipRight] = JointId.HandRight;
        parentJointMap[JointId.ThumbRight] = JointId.HandRight;
        parentJointMap[JointId.HipLeft] = JointId.SpineNavel;
        parentJointMap[JointId.KneeLeft] = JointId.HipLeft;
        parentJointMap[JointId.AnkleLeft] = JointId.KneeLeft;
        parentJointMap[JointId.FootLeft] = JointId.AnkleLeft;
        parentJointMap[JointId.HipRight] = JointId.SpineNavel;
        parentJointMap[JointId.KneeRight] = JointId.HipRight;
        parentJointMap[JointId.AnkleRight] = JointId.KneeRight;
        parentJointMap[JointId.FootRight] = JointId.AnkleRight;
        parentJointMap[JointId.Head] = JointId.Pelvis;
        parentJointMap[JointId.Nose] = JointId.Head;
        parentJointMap[JointId.EyeLeft] = JointId.Head;
        parentJointMap[JointId.EarLeft] = JointId.Head;
        parentJointMap[JointId.EyeRight] = JointId.Head;
        parentJointMap[JointId.EarRight] = JointId.Head;

        Vector3 zpositive = Vector3.forward;
        Vector3 xpositive = Vector3.right;
        Vector3 ypositive = Vector3.up;
        // spine and left hip are the same
        Quaternion leftHipBasis = Quaternion.LookRotation(xpositive, -zpositive);
        Quaternion spineHipBasis = Quaternion.LookRotation(xpositive, -zpositive);
        Quaternion rightHipBasis = Quaternion.LookRotation(xpositive, zpositive);
        // arms and thumbs share the same basis
        Quaternion leftArmBasis = Quaternion.LookRotation(ypositive, -zpositive);
        Quaternion rightArmBasis = Quaternion.LookRotation(-ypositive, zpositive);
        Quaternion leftHandBasis = Quaternion.LookRotation(-zpositive, -ypositive);
        Quaternion rightHandBasis = Quaternion.identity;
        Quaternion leftFootBasis = Quaternion.LookRotation(xpositive, ypositive);
        Quaternion rightFootBasis = Quaternion.LookRotation(xpositive, -ypositive);

        basisJointMap = new Dictionary<JointId, Quaternion>();

        // pelvis has no parent so set to count
        basisJointMap[JointId.Pelvis] = spineHipBasis;
        basisJointMap[JointId.SpineNavel] = spineHipBasis;
        basisJointMap[JointId.SpineChest] = spineHipBasis;
        basisJointMap[JointId.Neck] = spineHipBasis;
        basisJointMap[JointId.ClavicleLeft] = leftArmBasis;
        basisJointMap[JointId.ShoulderLeft] = leftArmBasis;
        basisJointMap[JointId.ElbowLeft] = leftArmBasis;
        basisJointMap[JointId.WristLeft] = leftHandBasis;
        basisJointMap[JointId.HandLeft] = leftHandBasis;
        basisJointMap[JointId.HandTipLeft] = leftHandBasis;
        basisJointMap[JointId.ThumbLeft] = leftArmBasis;
        basisJointMap[JointId.ClavicleRight] = rightArmBasis;
        basisJointMap[JointId.ShoulderRight] = rightArmBasis;
        basisJointMap[JointId.ElbowRight] = rightArmBasis;
        basisJointMap[JointId.WristRight] = rightHandBasis;
        basisJointMap[JointId.HandRight] = rightHandBasis;
        basisJointMap[JointId.HandTipRight] = rightHandBasis;
        basisJointMap[JointId.ThumbRight] = rightArmBasis;
        basisJointMap[JointId.HipLeft] = leftHipBasis;
        basisJointMap[JointId.KneeLeft] = leftHipBasis;
        basisJointMap[JointId.AnkleLeft] = leftHipBasis;
        basisJointMap[JointId.FootLeft] = leftFootBasis;
        basisJointMap[JointId.HipRight] = rightHipBasis;
        basisJointMap[JointId.KneeRight] = rightHipBasis;
        basisJointMap[JointId.AnkleRight] = rightHipBasis;
        basisJointMap[JointId.FootRight] = rightFootBasis;
        basisJointMap[JointId.Head] = spineHipBasis;
        basisJointMap[JointId.Nose] = spineHipBasis;
        basisJointMap[JointId.EyeLeft] = spineHipBasis;
        basisJointMap[JointId.EarLeft] = spineHipBasis;
        basisJointMap[JointId.EyeRight] = spineHipBasis;
        basisJointMap[JointId.EarRight] = spineHipBasis;

        skeletonsRotations = new Dictionary<int, Quaternion[]>();
        for (int i = 0; i < 3; i++) {
            skeletonsRotations[i] = new Quaternion[(int)JointId.Count];
        }
    }

    public void updateTracker(BackgroundData trackerFrameData)
    {
        //this is an array in case you want to get the n closest bodies
        int max = 3;
        int[] closestBodies = findClosestTrackedBodies(trackerFrameData, max);

        // render the bodies
        for (int i = 0; i < closestBodies.Length; i++) {
            //UnityEngine.Debug.Log(i);
            Body skeleton = trackerFrameData.Bodies[closestBodies[i]];
            int index = findIndexFromId(trackerFrameData, (int)skeleton.Id);
            // UnityEngine.Debug.Log(index + ": " + closestBodies[i].dist);
            renderSkeleton(skeleton, i);
        }
        // for (int i = 0; i < max; i++) {
        //     Body body = trackerFrameData.Bodies[i];
        //     int index = findIndexFromId(trackerFrameData, (int)body.Id);
        //     renderSkeleton(body, index);
        // }
    }

    int findIndexFromId(BackgroundData frameData, int id)
    {
        int retIndex = -1;
        for (int i = 0; i < (int)frameData.NumOfBodies; i++)
        {
            if ((int)frameData.Bodies[i].Id == id)
            {
                retIndex = i;
                break;
            }
        }
        return retIndex;
    }

    public int GetLargestElement((int body, float dist)[] array) {
        float max = array[0].dist;
        int ret = 0;
        for (int i = 1; i < array.Length; i++) {
            if (array[i].dist > max) {
                max = array[i].dist;
                ret = i;
            }
        }
        return ret;

    }

    private int[] findClosestTrackedBodies(BackgroundData trackerFrameData, int max)
    {
        int numBodies = (int)trackerFrameData.NumOfBodies;
        const float MAX_DISTANCE = 500000000.0f;
        float minDistanceFromKinect = MAX_DISTANCE;

        if (numBodies >= max) {
            numBodies = max;
        }

        (int body, float dist)[] closestBodies = new (int, float)[numBodies];

        for (int i = 0; i < numBodies; i++) {
            closestBodies[i] = (-1, MAX_DISTANCE);
        }
        float[] magnitudes = new float[(int)trackerFrameData.NumOfBodies];
        Dictionary<float, int> bodyDist = new Dictionary<float, int>();
        
        
        for (int i = 0; i < (int)trackerFrameData.NumOfBodies; i++)
        {
            var pelvisPosition = trackerFrameData.Bodies[i].JointPositions3D[(int)JointId.Pelvis];
            Vector3 pelvisPos = new Vector3((float)pelvisPosition.X, (float)pelvisPosition.Y, (float)pelvisPosition.Z);
            magnitudes[i] = pelvisPos.magnitude;
            
            bodyDist[pelvisPos.magnitude] = i;

        }
        Array.Sort(magnitudes);
        int[] closest = new int[(int)trackerFrameData.NumOfBodies];
        for (int i = 0; i < (int)trackerFrameData.NumOfBodies; i++) {
            closest[i] = bodyDist[magnitudes[i]];
        }
        return closest;
    }

    public void turnOnOffSkeletons()
    {
        drawSkeletons = !drawSkeletons;
        const int bodyRenderedNum = 0;
        for (int jointNum = 0; jointNum < (int)JointId.Count; jointNum++)
        {
            transform.GetChild(bodyRenderedNum).GetChild(jointNum).gameObject.GetComponent<MeshRenderer>().enabled = drawSkeletons;
            transform.GetChild(bodyRenderedNum).GetChild(jointNum).GetChild(0).GetComponent<MeshRenderer>().enabled = drawSkeletons;
        }
    }

    public void renderSkeleton(Body skeleton, int skeletonNumber)
    {
        if (skeletonNumber == 0) {
            for (int jointNum = 0; jointNum < (int)JointId.Count; jointNum++)
            {
                Vector3 jointPos = new Vector3(skeleton.JointPositions3D[jointNum].X, -skeleton.JointPositions3D[jointNum].Y, skeleton.JointPositions3D[jointNum].Z);
                Vector3 offsetPosition = transform.rotation * jointPos;
                Vector3 positionInTrackerRootSpace = transform.position + offsetPosition;
                Quaternion jointRot = Y_180_FLIP * new Quaternion(skeleton.JointRotations[jointNum].X, skeleton.JointRotations[jointNum].Y,
                    skeleton.JointRotations[jointNum].Z, skeleton.JointRotations[jointNum].W) * Quaternion.Inverse(basisJointMap[(JointId)jointNum]);
                absoluteJointRotations1[jointNum] = jointRot;
                // these are absolute body space because each joint has the body root for a parent in the scene graph
                transform.GetChild(skeletonNumber).GetChild(jointNum).localPosition = jointPos;
                transform.GetChild(skeletonNumber).GetChild(jointNum).localRotation = jointRot;

                const int boneChildNum = 0;
                if (parentJointMap[(JointId)jointNum] != JointId.Head && parentJointMap[(JointId)jointNum] != JointId.Count)
                {
                    Vector3 parentTrackerSpacePosition = new Vector3(skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].X,
                        -skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Y, skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Z);
                    Vector3 boneDirectionTrackerSpace = jointPos - parentTrackerSpacePosition;
                    Vector3 boneDirectionWorldSpace = transform.rotation * boneDirectionTrackerSpace;
                    Vector3 boneDirectionLocalSpace = Quaternion.Inverse(transform.GetChild(skeletonNumber).GetChild(jointNum).rotation) * Vector3.Normalize(boneDirectionWorldSpace);
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).localScale = new Vector3(1, 20.0f * 0.5f * boneDirectionWorldSpace.magnitude, 1);
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).localRotation = Quaternion.FromToRotation(Vector3.up, boneDirectionLocalSpace);
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).position = transform.GetChild(skeletonNumber).GetChild(jointNum).position - 0.5f * boneDirectionWorldSpace;
                    if (parentJointMap[(JointId)jointNum] == JointId.Pelvis) {
                        pelvis1 = transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).position;
                    }
                }
                else
                {
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).gameObject.SetActive(false);
                }

                // UnityEngine.Debug.Log(skeletonNumber + " : " + pelvis1);
                //skeletonsRotations[skeletonNumber] = absoluteJointRotations;
            }
        } else if (skeletonNumber == 1){
            for (int jointNum = 0; jointNum < (int)JointId.Count; jointNum++)
            {
                Vector3 jointPos = new Vector3(skeleton.JointPositions3D[jointNum].X, -skeleton.JointPositions3D[jointNum].Y, skeleton.JointPositions3D[jointNum].Z);
                Vector3 offsetPosition = transform.rotation * jointPos;
                Vector3 positionInTrackerRootSpace = transform.position + offsetPosition;
                Quaternion jointRot = Y_180_FLIP * new Quaternion(skeleton.JointRotations[jointNum].X, skeleton.JointRotations[jointNum].Y,
                    skeleton.JointRotations[jointNum].Z, skeleton.JointRotations[jointNum].W) * Quaternion.Inverse(basisJointMap[(JointId)jointNum]);
                absoluteJointRotations2[jointNum] = jointRot;
                // these are absolute body space because each joint has the body root for a parent in the scene graph
                transform.GetChild(skeletonNumber).GetChild(jointNum).localPosition = jointPos;
                transform.GetChild(skeletonNumber).GetChild(jointNum).localRotation = jointRot;

                const int boneChildNum = 0;
                if (parentJointMap[(JointId)jointNum] != JointId.Head && parentJointMap[(JointId)jointNum] != JointId.Count)
                {
                    Vector3 parentTrackerSpacePosition = new Vector3(skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].X,
                        -skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Y, skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Z);
                    Vector3 boneDirectionTrackerSpace = jointPos - parentTrackerSpacePosition;
                    Vector3 boneDirectionWorldSpace = transform.rotation * boneDirectionTrackerSpace;
                    Vector3 boneDirectionLocalSpace = Quaternion.Inverse(transform.GetChild(skeletonNumber).GetChild(jointNum).rotation) * Vector3.Normalize(boneDirectionWorldSpace);
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).localScale = new Vector3(1, 20.0f * 0.5f * boneDirectionWorldSpace.magnitude, 1);
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).localRotation = Quaternion.FromToRotation(Vector3.up, boneDirectionLocalSpace);
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).position = transform.GetChild(skeletonNumber).GetChild(jointNum).position - 0.5f * boneDirectionWorldSpace;
                    if (parentJointMap[(JointId)jointNum] == JointId.Pelvis) {
                        pelvis2 = transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).position;
                    }
                }
                else
                {
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).gameObject.SetActive(false);
                }

                // UnityEngine.Debug.Log(skeletonNumber + " : " + pelvis2);
                //skeletonsRotations[skeletonNumber] = absoluteJointRotations;
                }
        } else if (skeletonNumber == 2){
            for (int jointNum = 0; jointNum < (int)JointId.Count; jointNum++)
            {
                Vector3 jointPos = new Vector3(skeleton.JointPositions3D[jointNum].X, -skeleton.JointPositions3D[jointNum].Y, skeleton.JointPositions3D[jointNum].Z);
                Vector3 offsetPosition = transform.rotation * jointPos;
                Vector3 positionInTrackerRootSpace = transform.position + offsetPosition;
                Quaternion jointRot = Y_180_FLIP * new Quaternion(skeleton.JointRotations[jointNum].X, skeleton.JointRotations[jointNum].Y,
                    skeleton.JointRotations[jointNum].Z, skeleton.JointRotations[jointNum].W) * Quaternion.Inverse(basisJointMap[(JointId)jointNum]);
                absoluteJointRotations3[jointNum] = jointRot;
                // these are absolute body space because each joint has the body root for a parent in the scene graph
                transform.GetChild(skeletonNumber).GetChild(jointNum).localPosition = jointPos;
                transform.GetChild(skeletonNumber).GetChild(jointNum).localRotation = jointRot;

                const int boneChildNum = 0;
                if (parentJointMap[(JointId)jointNum] != JointId.Head && parentJointMap[(JointId)jointNum] != JointId.Count)
                {
                    Vector3 parentTrackerSpacePosition = new Vector3(skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].X,
                        -skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Y, skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Z);
                    Vector3 boneDirectionTrackerSpace = jointPos - parentTrackerSpacePosition;
                    Vector3 boneDirectionWorldSpace = transform.rotation * boneDirectionTrackerSpace;
                    Vector3 boneDirectionLocalSpace = Quaternion.Inverse(transform.GetChild(skeletonNumber).GetChild(jointNum).rotation) * Vector3.Normalize(boneDirectionWorldSpace);
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).localScale = new Vector3(1, 20.0f * 0.5f * boneDirectionWorldSpace.magnitude, 1);
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).localRotation = Quaternion.FromToRotation(Vector3.up, boneDirectionLocalSpace);
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).position = transform.GetChild(skeletonNumber).GetChild(jointNum).position - 0.5f * boneDirectionWorldSpace;
                    if (parentJointMap[(JointId)jointNum] == JointId.Pelvis) {
                        pelvis3 = transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).position;
                    }
                }
                else
                {
                    transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).gameObject.SetActive(false);
                }

                // UnityEngine.Debug.Log(skeletonNumber + " : " + pelvis3);
                //skeletonsRotations[skeletonNumber] = absoluteJointRotations;
            }
        }
    }

    public Quaternion GetRelativeJointRotation(JointId jointId)
    {
        JointId parent = parentJointMap[jointId];
        Quaternion parentJointRotationBodySpace = Quaternion.identity;
        if (parent == JointId.Count)
        {
            parentJointRotationBodySpace = Y_180_FLIP;
        }
        else
        {
            parentJointRotationBodySpace = absoluteJointRotations1[(int)parent];
        }
        Quaternion jointRotationBodySpace = absoluteJointRotations1[(int)jointId];
        Quaternion relativeRotation =  Quaternion.Inverse(parentJointRotationBodySpace) * jointRotationBodySpace;

        return relativeRotation;
    }

}
