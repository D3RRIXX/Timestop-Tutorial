using UnityEngine;

namespace Tutorial.Scripts
{
	[RequireComponent(typeof(Rigidbody))]
	public class TimeStoppableRigidbody : TimeStoppableObject
	{
		private Rigidbody _rb;
		private Vector3 _cachedVelocity;

		protected override void Awake()
		{
			base.Awake();
			_rb = GetComponent<Rigidbody>();
		}

		protected override void OnTimeStopToggled(bool value)
		{
			if (value)
			{
				_rb.isKinematic = true;
				_cachedVelocity = _rb.velocity;
			}
			else
			{
				_rb.isKinematic = false;
				_rb.velocity = _cachedVelocity;
			}
		}
	}
}