using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class TimeStopController : MonoBehaviour
{
	[SerializeField] private int _timeStopDuration = 5;

	private VolumeProfile _volume;
	private ColorAdjustments _colorAdjustments;
	private LensDistortion _lensDistortion;

	private void Awake()
	{
		_volume = GetComponent<Volume>().profile;
		
		_volume.TryGet(out _colorAdjustments);
		_volume.TryGet(out _lensDistortion);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
			StopTime();
	}

	private void StopTime()
	{

	}
}
