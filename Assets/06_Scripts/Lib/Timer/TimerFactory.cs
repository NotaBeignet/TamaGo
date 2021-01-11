using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerFactory : Singleton<TimerFactory> {

	public Timer GetTimer()
    {
        return gameObject.AddComponent<Timer>();
	}

    public LerpFloat GetLerpFloat()
    {
        return gameObject.AddComponent<LerpFloat>();
    }

    public LerpFloatCurve GetLerpFloatCurve()
    {
        return gameObject.AddComponent<LerpFloatCurve>();
    }

}