using UnityEngine;

public class ArticulatedRobotBodySegment : MonoBehaviour
{
    public ArticulatedRobotTouchSensor touch_sensor;
    public bool is_foot;
    public Animat animat;
    BoxCollider collider;
    // Start is called before the first frame update
    void Awake()
    {
        Transform segment = this.transform.Find("Segment");
        touch_sensor = segment.Find("Cube").GetComponent<ArticulatedRobotTouchSensor>();
        collider = this.GetComponent<BoxCollider>();
        gameObject.layer = AnimatArena.ANIMAT_GAMEOBJECT_LAYER;
    }


    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        touch_sensor.CollisionEnter(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        touch_sensor.CollisionEnter(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        touch_sensor.CollisionExit(collision);
    }
}
