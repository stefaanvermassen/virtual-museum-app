using UnityEngine;
using System.Collections;

/**
 * Stub for testing, just saves the id;
 * */
public class IDScanner : Scanner {

	public IDScanner(){
	}

	public ScanIdentity makeScannable(ScanIdentity scanId, Scannable s){
		ScanIdentity id = new ScanID(s.getUniqueId(),s.getScannableType());
		return id;
	}
	

	public Scannable scan(ScanIdentity scan){
		
	}
}
