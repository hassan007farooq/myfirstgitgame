using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMyVision : MonoBehaviour
{
    //How sensitivity we are about vision/line of sight? 
    public enum Sensitivity { HIGH, LOW };

    // To check sensitivity
    public Sensitivity sensitivity = Sensitivity.HIGH;

    // To see the target right now?
    public bool TargetInSight = false;

    // Feild of Vision
    public float FieldOfVision = 45f;

    //We need a refer. to our traget value 
    private Transform Target = null;

    // Refer. to our Eye. Yet we are
    public Transform MyEyes = null;

    //Transform Componet
    public Transform npcTransform = null;

    // my sphere collider
    public SphereCollider sphereCollider = null;

    // Last sight of object
    public Vector3 lastKnownSighting = Vector3.zero;

    private void Awake()
    {
        npcTransform = GetComponent <Transform> ();
        sphereCollider = GetComponent <SphereCollider> ();
        lastKnownSighting = npcTransform.position;
        Target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    bool InMyFieldOfVision()
    {
        Vector3 dirToTraget = Target.position - MyEyes.position;

        // Get angle btw forward and view direction
        float angle = Vector3.Angle(MyEyes.forward, dirToTraget);

        //Let us check the if within view of feild
        if(angle <= FieldOfVision)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Fucntion to check the line of sight
    bool ClearLineOfSight()
    {
        RaycastHit hit;

        if (Physics.Raycast(MyEyes.position, (Target.position - MyEyes.position).normalized,
            out hit, sphereCollider.radius))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }               
        }
        return false;
    }
    
    void UpdateSight()
    {
        switch (sensitivity)
        {
            case Sensitivity.HIGH:
                TargetInSight = InMyFieldOfVision() && ClearLineOfSight();
                break;
            case Sensitivity.LOW:
                TargetInSight = InMyFieldOfVision() || ClearLineOfSight();
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        UpdateSight();

        if (TargetInSight)
           lastKnownSighting = Target.position;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        TargetInSight = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
