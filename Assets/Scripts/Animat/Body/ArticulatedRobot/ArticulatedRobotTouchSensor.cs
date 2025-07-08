using System.Collections.Generic;
using UnityEngine;

public class ArticulatedRobotTouchSensor : MonoBehaviour
{
    // Enum to represent the six faces of the box
    public enum BoxFace { PositiveX, NegativeX, PositiveY, NegativeY, PositiveZ, NegativeZ }

    // Dictionary to track contact state of each face
    public Dictionary<BoxFace, int> faceContactStates = new Dictionary<BoxFace, int>
    {
        { BoxFace.PositiveX, 0 },
        { BoxFace.NegativeX, 0 },
        { BoxFace.PositiveY, 0 },
        { BoxFace.NegativeY, 0 },
        { BoxFace.PositiveZ, 0 },
        { BoxFace.NegativeZ, 0 }
    };

    public bool topTouching;
    public bool leftTouching;
    public bool rightTouching;
    public bool bottomTouching;
    public bool frontTouching;
    public bool backTouching;

    private void Awake()
    {
        //mr = this.transform.GetComponent<Renderer>();
    }

    public void CollisionEnter(Collision collision)
    {
        UpdateFaceContacts(collision, 1);
    }


    public void CollisionExit(Collision collision)
    {
        UpdateFaceContacts(collision,0);
    }

    private void UpdateFaceContacts(Collision collision, int count)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            //Vector3 normal = contact.normal;
            Vector3 localNormal = transform.InverseTransformDirection(contact.normal);

            // Determine which face is being contacted and update the state
            if (Vector3.Dot(localNormal, Vector3.right) > 0.9f)
                faceContactStates[BoxFace.PositiveX] = count;
            else if (Vector3.Dot(localNormal, Vector3.left) > 0.9f)
                faceContactStates[BoxFace.NegativeX] = count;
            else if (Vector3.Dot(localNormal, Vector3.up) > 0.9f)
                faceContactStates[BoxFace.PositiveY] = count;
            else if (Vector3.Dot(localNormal, Vector3.down) > 0.9f)
                faceContactStates[BoxFace.NegativeY] = count;
            else if (Vector3.Dot(localNormal, Vector3.forward) > 0.9f)
                faceContactStates[BoxFace.PositiveZ] = count;
            else if (Vector3.Dot(localNormal, Vector3.back) > 0.9f)
                faceContactStates[BoxFace.NegativeZ] = count;
        }

        rightTouching = faceContactStates[BoxFace.PositiveX] > 0;
        leftTouching = faceContactStates[BoxFace.NegativeX] > 0;
        topTouching = faceContactStates[BoxFace.PositiveY] > 0;
        bottomTouching = faceContactStates[BoxFace.NegativeY] > 0;
        frontTouching = faceContactStates[BoxFace.PositiveZ] > 0;
        backTouching = faceContactStates[BoxFace.NegativeZ] > 0;
        
    }


}
