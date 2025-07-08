using UnityEngine;

public class ObjectTouchingCounter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public int num_objects_touching = 0;

    private void OnCollisionEnter(Collision collision)
    {
        num_objects_touching++;
    }

    private void OnCollisionExit(Collision collision)
    {

        num_objects_touching--;

    }
}

