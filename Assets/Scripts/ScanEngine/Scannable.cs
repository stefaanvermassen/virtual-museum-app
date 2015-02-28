using UnityEngine;
using System.Collections;

/**
 * This inteface should be implemented by everything that is scannable. (e.g. Artpieces, Groups of art and entire oeuvres)
 **/
public interface Scannable {

	public enum ScannableType {ART,GROUP,OEUVRE};

	/**
	 * This is used to link the scannable to the ScanTech identifier (e.g. QR code, BLE MAC adress)
	 * */
	int getUniqueId();
	ScannableType getScannableType();
}
