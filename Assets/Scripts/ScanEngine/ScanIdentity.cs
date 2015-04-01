
namespace Scanning
{
    /**
     * This class represents a unique Scan Identity, for example a QR code or a BLE MAC address
     * */
    public abstract class ScanIdentity
    {

        private ScanTechnology Tech;

        public ScanIdentity(ScanTechnology tech)
        {
            this.Tech = tech;
        }
    }
}