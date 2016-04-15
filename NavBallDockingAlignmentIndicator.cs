using System;
using UnityEngine;
using KSP.UI.Screens.Flight;
using KSP.IO;

[KSPAddon(KSPAddon.Startup.Flight, false)]
public class NavBallDockingAlignmentIndicator : MonoBehaviour
{

	private NavBall navBall;

	private GameObject indicator;
	PluginConfiguration cfg;

	void Start()
	{
		Debug.Log (" ======== AWAKE  ======== ");
		this.cfg = KSP.IO.PluginConfiguration.CreateForType<NavBallDockingAlignmentIndicator>();
		cfg.load();
	}

	void LateUpdate()
	{
		if (this.navBall == null)
		{
			this.navBall = FindObjectOfType<NavBall>();
		}
		if (FlightGlobals.fetch != null 
		    && FlightGlobals.ready 
		    && FlightGlobals.fetch.activeVessel != null 
		    && FlightGlobals.fetch.VesselTarget != null 
		    && FlightGlobals.fetch.VesselTarget.GetTargetingMode() == VesselTargetModes.DirectionVelocityAndOrientation) {
			/// Targeted a Port if I am not mistaken o__o

			if (this.indicator == null)
			{
				SetupIndicator();
			}



			ITargetable targetPort = FlightGlobals.fetch.VesselTarget;
			Transform targetTransform = targetPort.GetTransform();
			Transform selfTransform = FlightGlobals.ActiveVessel.ReferenceTransform;

			// Position
			Vector3 targetPortOutVector = targetTransform.forward.normalized;
			Vector3 targetPortInVector = -targetPortOutVector;
			Vector3 rotatedTargetPortInVector = navBall.attitudeGymbal * targetPortInVector;
			indicator.transform.localPosition = rotatedTargetPortInVector * navBall.progradeVector.localPosition.magnitude;

			// Rotation
			Vector3 v1 = Vector3.Cross(selfTransform.up, -targetTransform.up);
			Vector3 v2 = Vector3.Cross(selfTransform.up, selfTransform.forward);
			float ang = Vector3.Angle(v1, v2);
			if (Vector3.Dot(selfTransform.up, Vector3.Cross(v1, v2)) < 0)
			{
				ang = -ang;
			}
			indicator.transform.rotation = Quaternion.Euler(90 + ang, 90, 270);

			// Set opacity
			float value = Vector3.Dot(indicator.transform.localPosition.normalized, Vector3.forward);
			value = Mathf.Clamp01(value);
			indicator.GetComponent<MeshRenderer>().materials[0].SetFloat("_Opacity", value);

			if (Input.GetKeyDown(KeyCode.K))
			{
				indicator.SetActive(!indicator.activeSelf);
			}
		}
	}

	void SetupIndicator()
	{
		this.indicator = GameObject.Instantiate(navBall.progradeVector.gameObject);
		this.indicator.transform.parent = navBall.progradeVector.parent;
		this.indicator.transform.position = navBall.progradeVector.position;
	}
}