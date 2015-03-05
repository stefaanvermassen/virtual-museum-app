using UnityEngine;
using System.Collections;
using QRCoder;
using System;


namespace Scanning
{
    public class QRScanner : Scanner
    {

        //empty constructor
        public QRScanner()
        {
        }

        public ScanIdentity MakeScannable(ScanIdentity scanId, Scannable scannable)
        {
            //for QR codes scanId will be null
            //TODO: implement
            QRCode id = new QRCode();

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(scannable.GetUniqueString(), QRCodeGenerator.ECCLevel.Q);
            id.Image = qrCode.GetGraphic(20);
            return id;
        }

        public Scannable Scan(ScanIdentity scan)
        {
            //TODO: implement
            throw new NotImplementedException();
        }
    }
}