using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class BrainCreatorGUILink : MonoBehaviour
{
    public UILineRenderer LR;
   // EdgeCollider2D edge_collider;
    ScrollRect SR;
    bool up;

    public Vector3 targetFrom;
    public Vector3 targetTo;
    public Vector3 midpoint;


    public RectTransform input_field;
    public RectTransform gui_fields;
    public RectTransform directional_arrow;
    public Brain.Synapse synapse;
    float offset_multiplier;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Initialize()
    { 
        this.LR = GetComponent<UILineRenderer>();
       // this.edge_collider = GetComponent<EdgeCollider2D>();
        this.up = false;

        this.SR = this.transform.GetComponentInParent<ScrollRect>();
        this.gui_fields = (RectTransform)this.transform.Find("GUI").transform;

    }
    public void SetTargetsAndPosition(Vector3 targetFrom, Vector3 targetTo)
    {
        this.targetFrom = targetFrom;
        this.targetTo = targetTo;
        this.midpoint = (this.targetFrom + this.targetTo) / 2f + Vector3.up * 100f; 
        this.offset_multiplier = 4*(targetTo.x - targetFrom.x) / 500f;
        this.SetLinePositionsToTargets();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLinePositionsToTargets()
    {
        Vector3 p1 = this.targetFrom;
        Vector3 p2 = this.targetTo;
        
        SetLinePositions(p1, p2);
    }

    public void SetLinePositions(Vector3 p1, Vector3 p2)
    {
        Vector2 lr_p1 = new Vector2 (p1.x, p1.y);
        Vector2 lr_p2 = new Vector2 (p2.x, p2.y);

        Vector2 lr_midpoint = (lr_p1 + lr_p2)/ 2f + Vector2.up * 100f * offset_multiplier;

        Vector2 lr_curvepoint1;
        Vector2 lr_curvepoint2;
        lr_curvepoint1 = (lr_p1 + lr_midpoint) / 2f + Vector2.up * 15f * offset_multiplier;
        lr_curvepoint2 = (lr_midpoint + lr_p2) / 2f + Vector2.up * 15f * offset_multiplier;
        //if (p1.x != p2.x)
        //{
        //    lr_curvepoint1 = (lr_p1 + lr_midpoint) / 2f + Vector2.up * 15f * offset_multiplier;
        //    lr_curvepoint2 = (lr_midpoint + lr_p2) / 2f + Vector2.up * 15f * offset_multiplier;
        //}
        //else
        //{
        //    lr_curvepoint1 = lr_midpoint + Vector2.left * 15f;
        //    lr_curvepoint2 = lr_midpoint + Vector2.right * 15f;
        //}
        
     
        this.LR.Points = new Vector2[] { lr_p1, lr_curvepoint1, lr_midpoint, lr_curvepoint2, lr_p2 };
       // this.edge_collider.points = new Vector2[] { lr_p1, lr_p2 };
        SetLinkDecoratorPositions();
    }

    public void SetLinkDecoratorPositions()
    {


        //if (this.input_field != null) this.input_field.localPosition = midpoint + Vector3.up * 10f;
        if (this.gui_fields != null){
            this.gui_fields.localPosition = midpoint + Vector3.up * 100f * offset_multiplier;
            if(offset_multiplier < 0.01f)
            {
                this.gui_fields.localPosition += Vector3.up * 100f;
            }
        }
        if (this.directional_arrow != null)
        {
            this.directional_arrow.sizeDelta = new Vector2(100, 100);
            this.directional_arrow.localPosition = Vector3.Lerp(midpoint, this.targetTo, 0.75f);

            // Calculate tangent vector at the end
            Vector3 tangent = (this.targetTo - this.midpoint).normalized;

            // Determine angle in degrees
            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;

            this.directional_arrow.rotation = Quaternion.Euler(new Vector3(0,0, -90 + angle)); 
        }

    }

    public void SnapTo(Vector2 target)
    {
        Canvas.ForceUpdateCanvases();

        this.SR.content.anchoredPosition = (Vector2)this.SR.transform.InverseTransformPoint(this.SR.content.position) - (Vector2)this.SR.transform.InverseTransformPoint(target);
    }

    Color colorA = Color.yellow;
    Color colorB = Color.black;
    void OnMouseOver()
    {
        if (up)
        {
            this.LR.color = colorA;
            /*this.LR.startColor = colorA;
            this.LR.endColor = colorA;*/
        }
        else
        {
            this.LR.color = colorA;
            /*     this.LR.startColor = colorA;
                 this.LR.endColor = colorA;*/
        }

        if(Input.GetMouseButtonDown(0)){
            // left click
            Canvas.ForceUpdateCanvases();

            if (this.up)
            {
                SnapTo(this.targetFrom);
            }
            else
            {
                SnapTo(this.targetTo);
            }
            this.up = !this.up;
        }

    }

    void OnMouseExit()
    {
        this.LR.color = Color.black;
    }
}
