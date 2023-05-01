using UnityEngine;

namespace Tutorial.Scripts
{
	public abstract class TimeStoppableObject : MonoBehaviour
	{
		protected virtual void Awake()
		{
			TimeStopController.TimeStopToggled += OnTimeStopToggled;
		}
		
		protected virtual void OnDestroy()
		{
			TimeStopController.TimeStopToggled -= OnTimeStopToggled;
		}

		protected abstract void OnTimeStopToggled(bool value);
	}
}