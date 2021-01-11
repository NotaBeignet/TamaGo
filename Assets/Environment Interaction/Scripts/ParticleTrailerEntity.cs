using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrailerEntity : MonoBehaviour {

    public ParticleTrailMaster particleTrailMaster;

    public float distanceSpawnSqr = 0.5f;
    private Vector3 previousPosition;
    private float distanceSincePrevious;

    public LayerMask groundLayer;
    public LayerMask waterLayer;

    public float distanceToGround = 0.1f;
    public float size = 3f;
    public float emissionRateOverTime = 1f;
    private float nextEmissionClock;
    private bool canInteractGround = false;
    private bool canInteractWater = false;

    private void Awake()
    {
        previousPosition = transform.position;
    }

    private void OnTriggerStay(Collider other)
    {

        int layer = 1 << other.gameObject.layer;


        if ((layer & groundLayer) != 0 && canInteractGround)
        {
            particleTrailMaster.EmitGroundTrail(transform.position, Vector3.up, transform.forward, size);
            canInteractGround = false;
        }
        if ((layer & waterLayer) != 0 && canInteractWater)
        {
            particleTrailMaster.EmitWaterRipple(transform.position, Vector3.up, transform.forward, size);
            canInteractWater = false;
        }

    }


    void Update()
    {
        if (particleTrailMaster != null)
        {

            distanceSincePrevious = Vector3.SqrMagnitude(transform.position - previousPosition);

            if (distanceSincePrevious >= distanceSpawnSqr)
            {
                previousPosition = transform.position;
                canInteractGround = canInteractWater = true;
            }


            if (emissionRateOverTime > 0f)
                nextEmissionClock -= Time.deltaTime;

            if (nextEmissionClock <= 0f && emissionRateOverTime > 0f)
            {
                nextEmissionClock = 1f / emissionRateOverTime;
                canInteractGround = canInteractWater = true;
            }
        }
    }

    // Or you could raycast down to check the ground... or use your characters isGrounded check perhaps.
    void CheckInteraction()
    {
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(transform.position, -Vector3.up, out hit, distanceToGround, groundLayer | waterLayer))
        {
            int layer = 1 << hit.collider.gameObject.layer;

            if ((layer & groundLayer) != 0)
                particleTrailMaster.EmitGroundTrail(hit.point, hit.normal, transform.forward, size);
            if ((layer & waterLayer) != 0)
                particleTrailMaster.EmitWaterRipple(hit.point, hit.normal, transform.forward, size);
        }
    }
}
