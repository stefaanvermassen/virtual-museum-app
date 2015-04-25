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

        private static ScanEngine Instance;

        private static Scanner Scanner;

        public ScanTechnology Tech { get; set; }

        private ScanEngine()
        {
            //empty constructor
        }

        public static ScanEngine Get(ScanTechnology tech)
        {

            if (Instance == null)
            {
                Instance = new ScanEngine();
            }
            Instance.Tech = tech;
            //create Scanner
            switch (tech)
            {
                case ScanTechnology.QR:
                    Scanner = new QRScanner();
                    break;
                default:
                    //should not happen
                    throw new UnsupportedScanTechnologyException("Technology not supported: " + tech);
            }
            return Instance;
        }


        /**
         * Method used to scan artpiece, group of art or oeuvre
         * */
        public Scannable scan(ScanIdentity scanId)
        {
            Scanner.Scan();
            return Scanner.GetScanResult();
        }

        /**
         * Methode used to link a ScanIdentity to a Scannable (eg for BLE tags)
         * */
        public ScanIdentity MakeScannable(ScanIdentity scanId, Scannable scannable)
        {
            return Scanner.MakeScannable(scanId, scannable);
        }

        /**
         * Methode used to generate a ScanIdentity for a Scannable (eg for QR codes)
         * */
        public ScanIdentity MakeScannable(Scannable scannable)
        {
            return MakeScannable(null, scannable);
        }


    }
}