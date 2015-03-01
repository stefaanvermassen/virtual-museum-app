public interface Scanner {

	ScanIdentity MakeScannable(ScanIdentity scanId, Scannable scannable);

	Scannable Scan(ScanIdentity scan);

}
