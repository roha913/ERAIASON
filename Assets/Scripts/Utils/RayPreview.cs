using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR;

public class RayPreview : MonoBehaviour
{
    [Header("Line Renderer Settings")]
    public Material lineMaterial;
    public float lineWidth = 0.05f;
    public Color lineColor = Color.black;

    private List<LineRenderer> lineRenderers = new List<LineRenderer>();

    AnimatBody body;
    GameObject eye1;
    GameObject pupil1;

    GameObject eye2;
    GameObject pupil2;

    VisionSensor sensor;

 

    [SerializeField] private float eyeRadius = 0.5f;
    [SerializeField] private float pupilRadius = 1.0f;
    [SerializeField] private Vector3 pupilOffset = new Vector3(0, 0, 0.5f);

    bool scaled = false;



    public bool overwrite = false;
    public void OverwriteRaycastPosDir(Vector3 pos, Vector3 dir, Vector3 up)
    {
        this.overwrite = true;
        this.pos = pos;
        this.forward_dir = dir;
        this.up = up;
    }

    Vector3 pos;
    Vector3 forward_dir;
    Vector3 up;
    private void Update()
    {
  
        if (!overwrite)
        {
            up = body.GetVisionSensorUpDirection();
            (pos, forward_dir) = body.GetVisionSensorPositionAndDirection();
        }
           
      
        Vector3 right = Vector3.Cross(forward_dir, up).normalized;
     
        float eyeWorldRadius = eye1.transform.lossyScale.x * 0.5f;
        eye1.transform.position = pos + eyeWorldRadius * right;
        eye2.transform.position = pos - eyeWorldRadius * right;
        PositionPupil(eye1.transform.position, forward_dir, pupil1);
        PositionPupil(eye2.transform.position, forward_dir, pupil2);
        
    }

    public void CreateEyes()
    {
        if (eye1 == null) (eye1, pupil1) = CreateEye();
        if (eye2 == null) (eye2, pupil2) = CreateEye();
    }


    public void ToggleLines()
    {
        if (GlobalConfig.show_lines)
        {
            SetHideLines();
        }
        else
        {
            SetShowLines();
        }
    }

    public void SetShowLines()
    {
        foreach (var lineRenderer in lineRenderers)
        {
            lineRenderer.enabled = true;
        }
    
    }

    public void SetHideLines()
    {
        foreach (var lineRenderer in lineRenderers)
        {
            lineRenderer.enabled = false;
        }
    }

    void ScaleEye(GameObject eye)
    {
        eye.transform.localScale /= this.body.scale;
        if(this.body is SoftVoxelRobot)
        {
            eye.transform.localScale *= (this.body.scale / 40.0f) * 2.0f; // change this when body scaling is removed
        }
        if (this.body is WheeledRobot)
        {
            eye.transform.localScale *= 3.0f;
        }
        if (this.body is ArticulatedRobot)
        {
            eye.transform.localScale *= 3.0f;
        }
    }
    /// <summary>
    /// Positions the pupil so it looks along worldDir,
    /// sitting on the surface of the eye sphere (in world space).
    /// </summary>
    /// <param name="worldDir">The direction in world space the eye should look.</param>
    public void PositionPupil(Vector3 worldPos, Vector3 worldDir, GameObject pupil)
    {
        // 2. compute each sphere's world-space radius
        float eyeWorldRadius = eye1.transform.lossyScale.x * 0.5f;
        float pupilWorldRadius = pupil1.transform.lossyScale.x * 0.5f;

        // 3. choose how much of the pupil’s radius to stick out (e.g. 20%)
        float protrusionFraction = 0.2f;
        float extraOffset = pupilWorldRadius * protrusionFraction;

        // 4. place the pupil so it sits "eyeRadius - pupilRadius + extra" along dir

        float surfaceOffset = (eyeWorldRadius - pupilWorldRadius) + extraOffset;
        pupil.transform.position = worldPos + worldDir * surfaceOffset;
    }




    /// <summary>
    /// Initializes the specified number of LineRenderers.
    /// </summary>
    /// <param name="rayCount">Number of rays to visualize.</param>
    public void Init(AnimatBody body, VisionSensor sensor)
    {
        this.body = body;

        if (sensor != null)
        {
            this.sensor = sensor;
            this.sensor.ray_preview = this;
        }


        int rayCount = VisionSensor.NUM_OF_RAYCASTS;

        // Clear existing LineRenderers
        foreach (var lr in lineRenderers)
        {
            if (lr != null)
                Destroy(lr.gameObject);
        }
        lineRenderers.Clear();

        // Create new LineRenderers
        for (int i = 0; i < rayCount; i++)
        {
            GameObject lrObj = new GameObject($"Ray_{i}");
            lrObj.transform.parent = this.transform;

            LineRenderer lr = lrObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
            lr.startColor = lineColor;
            lr.endColor = lineColor;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.positionCount = 2;
            lr.useWorldSpace = true;

            lineRenderers.Add(lr);
        }

        CreateEyes();
        if (GlobalConfig.show_lines)
        {
            SetShowLines();
        }
        else
        {
            SetHideLines();
        }
    }

    public struct RayPreviewCast
    {
        public Vector3 origin;
        public Vector3 direction;
        public float distance;
        public Color color;
        public RayPreviewCast(Vector3 origin, Vector3 direction, float distance, Color color)
        {
            this.origin = origin;
            this.direction = direction.normalized;
            this.distance = distance;
            this.color = color;
        }
    }

    public void UpdateRays(RaycastCommand[] commands)
    {
        int count = Mathf.Min(commands.Length, lineRenderers.Count);

        for (int i = 0; i < count; i++)
        {
            Vector3 origin = commands[i].from;
            Vector3 direction = commands[i].direction.normalized;
            float length = commands[i].distance;

            Vector3 endPoint = origin + direction * length;
            LineRenderer lr = lineRenderers[i];
            lr.SetPosition(0, origin);
            lr.SetPosition(1, endPoint);

            lineRenderers[i].startColor = Color.black;
            lineRenderers[i].endColor = Color.black;
        }
    }

    /// <summary>
    /// Updates the LineRenderers to match the provided RaycastCommands.
    /// </summary>
    /// <param name="commands">Array of RaycastCommand objects.</param>
    public void UpdateRays(List<RayPreviewCast> commands)
    {
        int count = Mathf.Min(commands.Count, lineRenderers.Count);

        for (int i = 0; i < count; i++)
        {
            Vector3 origin = commands[i].origin;
            Vector3 direction = commands[i].direction.normalized;
            float length = commands[i].distance;

            Vector3 endPoint = origin + direction * length;

            LineRenderer lr = lineRenderers[i];
            lr.SetPosition(0, origin);
            lr.SetPosition(1, endPoint);

            lineRenderers[i].startColor = commands[i].color;
            lineRenderers[i].endColor = commands[i].color;
        }

    }



    private (GameObject, GameObject) CreateEye()
    {
        // Create the white sphere (eye)
        var eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(eye.transform.GetComponent<SphereCollider>());
        eye.transform.parent = transform;
        eye.transform.localPosition = Vector3.zero;
        eye.transform.localScale = Vector3.one * eyeRadius * 0.25f;
        eye.GetComponent<Renderer>().material.color = Color.white;

        // Create the black sphere (pupil)
        var pupil = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(pupil.transform.GetComponent<SphereCollider>());
        pupil.transform.parent = eye.transform;
        pupil.transform.localPosition = pupilOffset;
        pupil.transform.localScale = Vector3.one * pupilRadius * 0.25f;
        pupil.GetComponent<Renderer>().material.color = Color.black;

        ScaleEye(eye);

        return (eye, pupil);    
    }
}