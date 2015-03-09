using System;

/**
 * Mediater implemented as Singleton: ScanEngine, used for scanning technologies
 * 
 * Example use: 
 * > Scannable scanned;
 * > ScanEngine engine = ScanEngine.get(ScanTechnology.QR);
 * > scanned = engine.scan(qrCode);
 * 
 * */
namespace Scanning
{
    public class ScanEngine
    {

        private static ScanEngine instance;

        private static Scanner scanner;

        public ScanTechnology Tech { get; set; }

        private ScanEngine()
        {
            //empty constructor
        }

        public static ScanEngine get(ScanTechnology tech)
        {

            if (instance == null)
            {
                instance = new ScanEngine();
            }
            instance.Tech = tech;
            //create Scanner
            switch (tech)
            {
                case ScanTechnology.QR:
                    scanner = new QRScanner();
                    break;
                default:
                    //should not happen
                    throw new UnsupportedScanTechnologyException("Technology not supported: " + tech);
            }
            return instance;
        }


        /**
         * Method used to scan artpiece, group of art or oeuvre
         * */
        public Scannable scan(ScanIdentity scanId)
        {
            scanner.Scan();
            return scanner.getScanResult();
        }

        /**
         * Methode used to link a ScanIdentity to a Scannable (eg for BLE tags)
         * */
        public ScanIdentity makeScannable(ScanIdentity scanId, Scannable scannable)
        {
            return scanner.MakeScannable(scanId, scannable);
        }

        /**
         * Methode used to generate a ScanIdentity for a Scannable (eg for QR codes)
         * */
        public ScanIdentity makeScannable(Scannable scannable)
        {
            return makeScannable(null, scannable);
        }


    }
}