using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Reminder - version with EventHandler, if needed one day
 * protected event EventHandler<MessageEventArgs> OnArriveToStation;


public void RegisterOnArriveToStation(Action<object,MessageEventArgs> a_action)
{
    EventHandler<MessageEventArgs> evenHandler = (a_object, a_message) => a_action(a_object, a_message);  
    OnArriveToStation += evenHandler;
}*/


//------------ LIST OF EVENTS -------------//
//NameOfEvent;Param1;Param2

//##BEGIN##

//Start;IntEventArgs
//DistanceUpdate;IntEventArgs
//CoinPicked
//ArmorPackPicked
//GlassDestroyed
//ArmorRushTrigerred
//DroneDestroyed
//ScoreUpdate;IntEventArgs
//Clean
//StartBurst
//ChangeSteps
//BulletTimeCooldown;Timer
//BurstTimerCooldown;Timer
//BulletTimeCharge;IntEventArgs
//BulletTimeMaxCharge;IntEventArgs
//RightButton;BoolEventArgs
//LeftButton;BoolEventArgs
//CenterButton;BoolEventArgs;Vector2EventArgs
//UpdateCenterButton;Vector2EventArgs
//Burst;Vector3EventArgs;NumberEventArgs
//SpawnableNewDistance;IntEventArgs
//ResolutionUpdated
//Burstable
//KeepScoreDisplay;float
//CharacteristicUpdated
//##END##

//------------ END - LIST OF EVENTS -------------//


public class EventManager : Singleton<EventManager>
{

// ----- AUTO GENERATED CODE ----- //




// --- EVENT --- Start --- //


	protected event Action<object, IntEventArgs> OnStart;
	public void RegisterOnStart(Action<object, IntEventArgs> a_action)
	{
		OnStart += a_action;
	}


	public void UnRegisterOnStart(Action<object, IntEventArgs> a_action)
	{
		OnStart -= a_action;
	}


	public void InvokeOnStart(object a_sender, IntEventArgs a_1intEventArgs)
	{
		if(OnStart != null)
		{
			OnStart.Invoke(a_sender, a_1intEventArgs);
		}
	}




// --- EVENT --- DistanceUpdate --- //


	protected event Action<object, IntEventArgs> OnDistanceUpdate;
	public void RegisterOnDistanceUpdate(Action<object, IntEventArgs> a_action)
	{
		OnDistanceUpdate += a_action;
	}


	public void UnRegisterOnDistanceUpdate(Action<object, IntEventArgs> a_action)
	{
		OnDistanceUpdate -= a_action;
	}


	public void InvokeOnDistanceUpdate(object a_sender, IntEventArgs a_1intEventArgs)
	{
		if(OnDistanceUpdate != null)
		{
			OnDistanceUpdate.Invoke(a_sender, a_1intEventArgs);
		}
	}




// --- EVENT --- CoinPicked --- //


	protected event Action<object> OnCoinPicked;
	public void RegisterOnCoinPicked(Action<object> a_action)
	{
		OnCoinPicked += a_action;
	}


	public void UnRegisterOnCoinPicked(Action<object> a_action)
	{
		OnCoinPicked -= a_action;
	}


	public void InvokeOnCoinPicked(object a_sender)
	{
		if(OnCoinPicked != null)
		{
			OnCoinPicked.Invoke(a_sender);
		}
	}




// --- EVENT --- ArmorPackPicked --- //


	protected event Action<object> OnArmorPackPicked;
	public void RegisterOnArmorPackPicked(Action<object> a_action)
	{
		OnArmorPackPicked += a_action;
	}


	public void UnRegisterOnArmorPackPicked(Action<object> a_action)
	{
		OnArmorPackPicked -= a_action;
	}


	public void InvokeOnArmorPackPicked(object a_sender)
	{
		if(OnArmorPackPicked != null)
		{
			OnArmorPackPicked.Invoke(a_sender);
		}
	}




// --- EVENT --- GlassDestroyed --- //


	protected event Action<object> OnGlassDestroyed;
	public void RegisterOnGlassDestroyed(Action<object> a_action)
	{
		OnGlassDestroyed += a_action;
	}


	public void UnRegisterOnGlassDestroyed(Action<object> a_action)
	{
		OnGlassDestroyed -= a_action;
	}


	public void InvokeOnGlassDestroyed(object a_sender)
	{
		if(OnGlassDestroyed != null)
		{
			OnGlassDestroyed.Invoke(a_sender);
		}
	}




// --- EVENT --- ArmorRushTrigerred --- //


	protected event Action<object> OnArmorRushTrigerred;
	public void RegisterOnArmorRushTrigerred(Action<object> a_action)
	{
		OnArmorRushTrigerred += a_action;
	}


	public void UnRegisterOnArmorRushTrigerred(Action<object> a_action)
	{
		OnArmorRushTrigerred -= a_action;
	}


	public void InvokeOnArmorRushTrigerred(object a_sender)
	{
		if(OnArmorRushTrigerred != null)
		{
			OnArmorRushTrigerred.Invoke(a_sender);
		}
	}




// --- EVENT --- DroneDestroyed --- //


	protected event Action<object> OnDroneDestroyed;
	public void RegisterOnDroneDestroyed(Action<object> a_action)
	{
		OnDroneDestroyed += a_action;
	}


	public void UnRegisterOnDroneDestroyed(Action<object> a_action)
	{
		OnDroneDestroyed -= a_action;
	}


	public void InvokeOnDroneDestroyed(object a_sender)
	{
		if(OnDroneDestroyed != null)
		{
			OnDroneDestroyed.Invoke(a_sender);
		}
	}




// --- EVENT --- ScoreUpdate --- //


	protected event Action<object, IntEventArgs> OnScoreUpdate;
	public void RegisterOnScoreUpdate(Action<object, IntEventArgs> a_action)
	{
		OnScoreUpdate += a_action;
	}


	public void UnRegisterOnScoreUpdate(Action<object, IntEventArgs> a_action)
	{
		OnScoreUpdate -= a_action;
	}


	public void InvokeOnScoreUpdate(object a_sender, IntEventArgs a_1intEventArgs)
	{
		if(OnScoreUpdate != null)
		{
			OnScoreUpdate.Invoke(a_sender, a_1intEventArgs);
		}
	}




// --- EVENT --- Clean --- //


	protected event Action<object> OnClean;
	public void RegisterOnClean(Action<object> a_action)
	{
		OnClean += a_action;
	}


	public void UnRegisterOnClean(Action<object> a_action)
	{
		OnClean -= a_action;
	}


	public void InvokeOnClean(object a_sender)
	{
		if(OnClean != null)
		{
			OnClean.Invoke(a_sender);
		}
	}




// --- EVENT --- StartBurst --- //


	protected event Action<object> OnStartBurst;
	public void RegisterOnStartBurst(Action<object> a_action)
	{
		OnStartBurst += a_action;
	}


	public void UnRegisterOnStartBurst(Action<object> a_action)
	{
		OnStartBurst -= a_action;
	}


	public void InvokeOnStartBurst(object a_sender)
	{
		if(OnStartBurst != null)
		{
			OnStartBurst.Invoke(a_sender);
		}
	}




// --- EVENT --- ChangeSteps --- //


	protected event Action<object> OnChangeSteps;
	public void RegisterOnChangeSteps(Action<object> a_action)
	{
		OnChangeSteps += a_action;
	}


	public void UnRegisterOnChangeSteps(Action<object> a_action)
	{
		OnChangeSteps -= a_action;
	}


	public void InvokeOnChangeSteps(object a_sender)
	{
		if(OnChangeSteps != null)
		{
			OnChangeSteps.Invoke(a_sender);
		}
	}




// --- EVENT --- BulletTimeCooldown --- //


	protected event Action<object, Timer> OnBulletTimeCooldown;
	public void RegisterOnBulletTimeCooldown(Action<object, Timer> a_action)
	{
		OnBulletTimeCooldown += a_action;
	}


	public void UnRegisterOnBulletTimeCooldown(Action<object, Timer> a_action)
	{
		OnBulletTimeCooldown -= a_action;
	}


	public void InvokeOnBulletTimeCooldown(object a_sender, Timer a_1timer)
	{
		if(OnBulletTimeCooldown != null)
		{
			OnBulletTimeCooldown.Invoke(a_sender, a_1timer);
		}
	}




// --- EVENT --- BurstTimerCooldown --- //


	protected event Action<object, Timer> OnBurstTimerCooldown;
	public void RegisterOnBurstTimerCooldown(Action<object, Timer> a_action)
	{
		OnBurstTimerCooldown += a_action;
	}


	public void UnRegisterOnBurstTimerCooldown(Action<object, Timer> a_action)
	{
		OnBurstTimerCooldown -= a_action;
	}


	public void InvokeOnBurstTimerCooldown(object a_sender, Timer a_1timer)
	{
		if(OnBurstTimerCooldown != null)
		{
			OnBurstTimerCooldown.Invoke(a_sender, a_1timer);
		}
	}




// --- EVENT --- BulletTimeCharge --- //


	protected event Action<object, IntEventArgs> OnBulletTimeCharge;
	public void RegisterOnBulletTimeCharge(Action<object, IntEventArgs> a_action)
	{
		OnBulletTimeCharge += a_action;
	}


	public void UnRegisterOnBulletTimeCharge(Action<object, IntEventArgs> a_action)
	{
		OnBulletTimeCharge -= a_action;
	}


	public void InvokeOnBulletTimeCharge(object a_sender, IntEventArgs a_1intEventArgs)
	{
		if(OnBulletTimeCharge != null)
		{
			OnBulletTimeCharge.Invoke(a_sender, a_1intEventArgs);
		}
	}




// --- EVENT --- BulletTimeMaxCharge --- //


	protected event Action<object, IntEventArgs> OnBulletTimeMaxCharge;
	public void RegisterOnBulletTimeMaxCharge(Action<object, IntEventArgs> a_action)
	{
		OnBulletTimeMaxCharge += a_action;
	}


	public void UnRegisterOnBulletTimeMaxCharge(Action<object, IntEventArgs> a_action)
	{
		OnBulletTimeMaxCharge -= a_action;
	}


	public void InvokeOnBulletTimeMaxCharge(object a_sender, IntEventArgs a_1intEventArgs)
	{
		if(OnBulletTimeMaxCharge != null)
		{
			OnBulletTimeMaxCharge.Invoke(a_sender, a_1intEventArgs);
		}
	}




// --- EVENT --- RightButton --- //


	protected event Action<object, BoolEventArgs> OnRightButton;
	public void RegisterOnRightButton(Action<object, BoolEventArgs> a_action)
	{
		OnRightButton += a_action;
	}


	public void UnRegisterOnRightButton(Action<object, BoolEventArgs> a_action)
	{
		OnRightButton -= a_action;
	}


	public void InvokeOnRightButton(object a_sender, BoolEventArgs a_1boolEventArgs)
	{
		if(OnRightButton != null)
		{
			OnRightButton.Invoke(a_sender, a_1boolEventArgs);
		}
	}




// --- EVENT --- LeftButton --- //


	protected event Action<object, BoolEventArgs> OnLeftButton;
	public void RegisterOnLeftButton(Action<object, BoolEventArgs> a_action)
	{
		OnLeftButton += a_action;
	}


	public void UnRegisterOnLeftButton(Action<object, BoolEventArgs> a_action)
	{
		OnLeftButton -= a_action;
	}


	public void InvokeOnLeftButton(object a_sender, BoolEventArgs a_1boolEventArgs)
	{
		if(OnLeftButton != null)
		{
			OnLeftButton.Invoke(a_sender, a_1boolEventArgs);
		}
	}




// --- EVENT --- CenterButton --- //


	protected event Action<object, BoolEventArgs, Vector2EventArgs> OnCenterButton;
	public void RegisterOnCenterButton(Action<object, BoolEventArgs, Vector2EventArgs> a_action)
	{
		OnCenterButton += a_action;
	}


	public void UnRegisterOnCenterButton(Action<object, BoolEventArgs, Vector2EventArgs> a_action)
	{
		OnCenterButton -= a_action;
	}


	public void InvokeOnCenterButton(object a_sender, BoolEventArgs a_1boolEventArgs, Vector2EventArgs a_2vector2EventArgs)
	{
		if(OnCenterButton != null)
		{
			OnCenterButton.Invoke(a_sender, a_1boolEventArgs, a_2vector2EventArgs);
		}
	}




// --- EVENT --- UpdateCenterButton --- //


	protected event Action<object, Vector2EventArgs> OnUpdateCenterButton;
	public void RegisterOnUpdateCenterButton(Action<object, Vector2EventArgs> a_action)
	{
		OnUpdateCenterButton += a_action;
	}


	public void UnRegisterOnUpdateCenterButton(Action<object, Vector2EventArgs> a_action)
	{
		OnUpdateCenterButton -= a_action;
	}


	public void InvokeOnUpdateCenterButton(object a_sender, Vector2EventArgs a_1vector2EventArgs)
	{
		if(OnUpdateCenterButton != null)
		{
			OnUpdateCenterButton.Invoke(a_sender, a_1vector2EventArgs);
		}
	}




// --- EVENT --- Burst --- //


	protected event Action<object, Vector3EventArgs, NumberEventArgs> OnBurst;
	public void RegisterOnBurst(Action<object, Vector3EventArgs, NumberEventArgs> a_action)
	{
		OnBurst += a_action;
	}


	public void UnRegisterOnBurst(Action<object, Vector3EventArgs, NumberEventArgs> a_action)
	{
		OnBurst -= a_action;
	}


	public void InvokeOnBurst(object a_sender, Vector3EventArgs a_1vector3EventArgs, NumberEventArgs a_2numberEventArgs)
	{
		if(OnBurst != null)
		{
			OnBurst.Invoke(a_sender, a_1vector3EventArgs, a_2numberEventArgs);
		}
	}




// --- EVENT --- SpawnableNewDistance --- //


	protected event Action<object, IntEventArgs> OnSpawnableNewDistance;
	public void RegisterOnSpawnableNewDistance(Action<object, IntEventArgs> a_action)
	{
		OnSpawnableNewDistance += a_action;
	}


	public void UnRegisterOnSpawnableNewDistance(Action<object, IntEventArgs> a_action)
	{
		OnSpawnableNewDistance -= a_action;
	}


	public void InvokeOnSpawnableNewDistance(object a_sender, IntEventArgs a_1intEventArgs)
	{
		if(OnSpawnableNewDistance != null)
		{
			OnSpawnableNewDistance.Invoke(a_sender, a_1intEventArgs);
		}
	}




// --- EVENT --- ResolutionUpdated --- //


	protected event Action<object> OnResolutionUpdated;
	public void RegisterOnResolutionUpdated(Action<object> a_action)
	{
		OnResolutionUpdated += a_action;
	}


	public void UnRegisterOnResolutionUpdated(Action<object> a_action)
	{
		OnResolutionUpdated -= a_action;
	}


	public void InvokeOnResolutionUpdated(object a_sender)
	{
		if(OnResolutionUpdated != null)
		{
			OnResolutionUpdated.Invoke(a_sender);
		}
	}




// --- EVENT --- Burstable --- //


	protected event Action<object> OnBurstable;
	public void RegisterOnBurstable(Action<object> a_action)
	{
		OnBurstable += a_action;
	}


	public void UnRegisterOnBurstable(Action<object> a_action)
	{
		OnBurstable -= a_action;
	}


	public void InvokeOnBurstable(object a_sender)
	{
		if(OnBurstable != null)
		{
			OnBurstable.Invoke(a_sender);
		}
	}




// --- EVENT --- KeepScoreDisplay --- //


	protected event Action<object, float> OnKeepScoreDisplay;
	public void RegisterOnKeepScoreDisplay(Action<object, float> a_action)
	{
		OnKeepScoreDisplay += a_action;
	}


	public void UnRegisterOnKeepScoreDisplay(Action<object, float> a_action)
	{
		OnKeepScoreDisplay -= a_action;
	}


	public void InvokeOnKeepScoreDisplay(object a_sender, float a_1float)
	{
		if(OnKeepScoreDisplay != null)
		{
			OnKeepScoreDisplay.Invoke(a_sender, a_1float);
		}
	}

	// --- EVENT --- CharacteristicUpdated --- //


	protected event Action<object> OnCharacteristicUpdated;
	public void RegisterOnCharacteristicUpdated(Action<object> a_action)
	{
		OnCharacteristicUpdated += a_action;
	}


	public void UnRegisterOnCharacteristicUpdated(Action<object> a_action)
	{
		OnCharacteristicUpdated -= a_action;
	}


	public void InvokeOnCharacteristicUpdated(object a_sender)
	{
		if (OnCharacteristicUpdated != null)
		{
			OnCharacteristicUpdated.Invoke(a_sender);
		}
	}


	// ----- END AUTO GENERATED CODE ----- //

}


public class MessageEventArgs : EventArgs
{
	public string m_message;
	public MessageEventArgs(string a_message)
	{
		m_message = a_message;
	}
}

public class NumberEventArgs : EventArgs
{
	public float m_number;
	public NumberEventArgs(float a_number)
	{
		m_number = a_number;
	}
}

public class IntEventArgs : EventArgs
{
	public int m_int;
	public IntEventArgs(int a_int)
	{
		m_int = a_int;
	}
}


public class BoolEventArgs : EventArgs
{
	public bool m_bool;
	public BoolEventArgs(bool a_bool)
	{
		m_bool = a_bool;
	}
}


public class Vector3EventArgs : EventArgs
{
	public Vector3 m_vector;
	public Vector3EventArgs(Vector3 a_vector)
	{
		m_vector = a_vector;
	}

	public Vector3EventArgs(float a_x, float a_y, float a_z)
	{
		m_vector = new Vector3(a_x, a_y, a_z);
	}

}


public class Vector2EventArgs : EventArgs
{
	public Vector2 m_vector;
	public Vector2EventArgs(Vector2 a_vector)
	{
		m_vector = a_vector;
	}

	public Vector2EventArgs(float a_x, float a_y)
	{
		m_vector = new Vector2(a_x, a_y);
	}

}
