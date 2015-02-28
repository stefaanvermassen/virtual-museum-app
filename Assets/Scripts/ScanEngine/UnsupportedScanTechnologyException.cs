using UnityEngine;
using System.Collections;

public class UnsupportedScanTechnologyException : Exception {

	public UnsupportedScanTechnologyException(){
	}

	public UnsupportedScanTechnologyException(string message)
		: base(message)
	{
	}

}
