using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using ZXing;
using ZXing.QrCode;


namespace Scanning
{
    public class QRScanner : Scanner
    {

        //CamTexture size of image to be scanned
        public int Height { get; set; }
        public int Width { get; set; }
        public Color32[] Color { get; set; }

        public bool IsQuit { get; set; }
        public string LastResult { get; set; }

        public ArtFilter Filter { get; set; }
        //empty constructor
        public QRScanner()
        {
            LastResult = "default";
        }

        public ScanIdentity MakeScannable(ScanIdentity scanId, Scannable scannable)
        {
            //for QR codes scanId will be null
            QRCode id = new QRCode();

            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    //size of code writer
                    Height = Height,
                    Width = Width
                }
            };

            id.Image = writer.Write(scannable.GetUniqueString());
            return id;
        }

        public void Scan()
        {
            // create a reader with a custom luminance source
            var barcodeReader = new BarcodeReader { AutoRotate = false, TryHarder = false };

            while (true)
            {
                if (IsQuit)
                    break;

                try
                {
                    // decode the current frame
                    var result = barcodeReader.Decode(Color, Width, Height);
                    if (result != null)
                    {
                        Filter = new ArtFilter();
                        LastResult = result.Text;
                        Filter.configure(result.Text);
                        return;
                    }

                    // Sleep a little bit and set the signal to get the next frame
                    Thread.Sleep(200);
                    Color = null; //if null, Update() of GUI will set Color to a new frame of the webcam
                }
                catch
                {
                }
            }
            throw new TimeoutException("Scanning of QR code was interrupted");
        }


        public Scannable getScanResult()
        {
            if(Filter != null){
                return Filter;
            }
            throw new MissingFieldException("The value of the result was not set, scan was not competed or unsuccesful");
        }
    }
}