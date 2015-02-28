using UnityEngine;
using System.Collections;

/**
 * This class represents a unique Scan Identity, for example a QR code or a BLE MAC address
 * */
public abstract class ScanIdentity {

	public ScanTechnology tech;

	public ScanIdentity(ScanTechnology tech){
		this.tech = tech;
	}
}
