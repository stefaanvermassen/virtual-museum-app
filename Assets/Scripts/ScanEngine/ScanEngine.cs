using UnityEngine;
using System.Collections;
using System;

/**
 * Mediater implemented as Singleton: ScanEngine, used for scanning technologies
 * 
 * Example use: 
 * > Scannable scanned;
 * > ScanEngine engine = ScanEngine.get(ScanTechnology.QR);
 * > scanned = engine.scan();
 * 
 * */
public class ScanEngine {
	
	private static ScanEngine instance;
	
	public ScanTechnology tech;
	private Scanner scanner;

	private ScanEngine () {
		//empty constructor
	}

	public static ScanEngine get(ScanTechnology tech){

		if (instance == null) {
			instance = new ScanEngine();
		}
		instance.setTech (tech);
		//create Scanner
		switch (tech) {
			case ScanTechnology.ID
				this.scanner = new IDScanner();
			break;
			case ScanTechnology.QR
				this.scanner = new QRScanner();
				break;
			default:
				//should not happen
			throw UnsupportedScanTechnologyException("Technology not supported: "+tech);
		}
		return instance;
	}

	public void setTech(ScanTechnology tech) {
		this.tech = tech;
	}
	public ScanTechnology getTech(){
		return tech;
	}

	/**
	 * Method used to scan artpiece, group of art or oeuvre
	 * */
	public Scannable scan(ScanIdentity scanId){
		scanner.scan(scanId);
	}

	/**
	 * Methode used to link a ScanIdentity to a Scannable (eg for BLE tags)
	 * */
	public ScanIdentity makeScannable(ScanIdentity scanId, Scannable scannable){
		scanner.makeScannable(scanId, scannable);
	}

	/**
	 * Methode used to generate a ScanIdentity for a SCannable (eg for QR codes)
	 * */
	public ScanIdentity makeScannable(Scannable scannable){
		return makeScannable(null,scannable);
	}


}
