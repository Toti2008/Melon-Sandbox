using UnityEngine;


//created by tosaaa_3


public class WeldTool : MonoBehaviour
{
    
    private GameObject firstSelectedObject;
    private GameObject secondSelectedObject;
    private GameObject ropeInstance;


    void Update()
    {
        // Check for left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }

        
    }

    void HandleLeftClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;

            if (clickedObject.GetComponent<Rigidbody>() != null)
            {
                if (firstSelectedObject == null)
                {
                    // First object selected
                    firstSelectedObject = clickedObject;
                }
                else
                {
                    // Second object selected
                    secondSelectedObject = clickedObject;
                    ConnectObjects(firstSelectedObject, secondSelectedObject);
                }
            }
        }
    }

    void ConnectObjects(GameObject obj1, GameObject obj2)
    {
        // Create the visual representation of the rope
        ropeInstance = new GameObject("RopeInstance");

        // Add rigidbodies to both ends
        Rigidbody rb1 = obj1.GetComponent<Rigidbody>();
        Rigidbody rb2 = obj2.GetComponent<Rigidbody>();

        // Add HingeJoint components
        HingeJoint joint1 = obj1.AddComponent<HingeJoint>();
        joint1.connectedBody = rb2;
        JointSpring spring1 = new JointSpring { spring = 500f, damper = 50f };
        joint1.spring = spring1;
        joint1.useLimits = true;
        joint1.limits = new JointLimits { min = 0f, max = 0f };

        HingeJoint joint2 = obj2.AddComponent<HingeJoint>();
        joint2.connectedBody = rb1;
        JointSpring spring2 = new JointSpring { spring = 500f, damper = 50f };
        joint2.spring = spring2;
        joint2.useLimits = true;
        joint2.limits = new JointLimits { min = 0f, max = 0f };

        

        // Reset the selected objects
        firstSelectedObject = null;
        secondSelectedObject = null;
    }
}
