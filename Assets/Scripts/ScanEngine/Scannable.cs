

namespace Scanning
{
    /**
     * This inteface should be implemented by everything that is scannable. (e.g. Artpieces, Groups of art and entire oeuvres)
     **/
    public interface Scannable
    {

        /**
         * This is used to link the scannable to the ScanTech identifier (e.g. QR code, BLE MAC adress)
         * */
        string GetUniqueString();
    }
}