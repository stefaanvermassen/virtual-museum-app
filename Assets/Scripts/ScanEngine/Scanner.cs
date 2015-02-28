using UnityEngine;
using System.Collections;

public interface Scanner {

	public ScanIdentity makeScannable(ScanIdentity scanId, Scannable scannable);

	public Scannable scan(ScanIdentity scan);

}
