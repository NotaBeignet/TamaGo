using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class ParticleTrailMaster : MonoBehaviour {



    public ParticleSystem _groundTrailPS;
    ParticleSystem.MainModule groundTrailPSMain;
    private float groundTrailBaseStartSize = 1f;

    public ParticleSystem _waterRipplePS;
    ParticleSystem.MainModule waterRipplePSMain;
    private float waterRippleBaseStartSize = 1f;

    public ParticleSystem _waterForwardWakePS;
    ParticleSystem.MainModule waterForwardWakePSMain;
    private float waterForwardWakeBaseStartSize = 1f;


    void Awake() {
        if (_groundTrailPS != null)
        {
            groundTrailPSMain = _groundTrailPS.main;
            groundTrailBaseStartSize = (groundTrailPSMain.startSize.constantMax + groundTrailPSMain.startSize.constantMin) / 2f; ;
        }

        if (_waterRipplePS != null)
        {
            waterRipplePSMain = _waterRipplePS.main;
            waterRippleBaseStartSize = (waterRipplePSMain.startSize.constantMax + waterRipplePSMain.startSize.constantMin) / 2f; ;
        }

        if (_waterForwardWakePS != null)
        {
            waterForwardWakePSMain = _waterForwardWakePS.main;
            waterForwardWakeBaseStartSize = (waterForwardWakePSMain.startSize.constantMax + waterForwardWakePSMain.startSize.constantMin) / 2f; ;
        }
    }

    // TO DO - align particles with forward direction

    public void EmitGroundTrail(Vector3 emitHere, Vector3 upDirection, Vector3 forwardDirection, float size = 1f)
    {

        _groundTrailPS.transform.position = emitHere;
        _groundTrailPS.transform.rotation = Quaternion.LookRotation(upDirection);
        groundTrailPSMain.startSize = groundTrailBaseStartSize * size;
        _groundTrailPS.Emit(1);
    }
    
    public void EmitWaterRipple(Vector3 emitHere, Vector3 upDirection, Vector3 forwardDirection, float size = 1f)
    {
        _waterRipplePS.transform.position = emitHere;
        _waterRipplePS.transform.rotation = Quaternion.LookRotation(upDirection);
        waterRipplePSMain.startSize = waterRippleBaseStartSize * size;
        _waterRipplePS.Emit(1);
    }


    public void EmitWaterForwardWake(Vector3 emitHere, Vector3 upDirection, Vector3 forwardDirection, float size = 1f)
    {
        _waterForwardWakePS.transform.position = emitHere;
        _waterForwardWakePS.transform.rotation = Quaternion.LookRotation(upDirection);
        waterForwardWakePSMain.startSize = waterForwardWakeBaseStartSize * size;
        _waterForwardWakePS.Emit(1);
    }
}
