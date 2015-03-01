using UnityEngine;
using System.Collections;

/**
 * Stub for testing, just saves the id;
 * */
public class IDScanner : Scanner {

	public IDScanner(){
	}

	public ScanIdentity MakeScannable(ScanIdentity scanId, Scannable s){
        return new ScanID(s.GetUniqueString());
	}
	

	public Scannable Scan(ScanIdentity scan){
        Scannable scannable = new ArtFilter();

        return scannable;
	}
}
