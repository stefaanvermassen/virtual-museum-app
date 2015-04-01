
namespace Scanning
{
    public interface Scanner
    {

        ScanIdentity MakeScannable(ScanIdentity scanId, Scannable scannable);
        void Scan();
        Scannable GetScanResult();

    }
}