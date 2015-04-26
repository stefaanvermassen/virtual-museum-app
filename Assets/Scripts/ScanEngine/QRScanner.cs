using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using ZXing;
using ZXing.QrCode;
using System.IO;
using System.Runtime.InteropServices;


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
            Height = 256;
            Width = 256;
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

            Debug.Log("Generate QR");
            id.Image = writer.Write(scannable.GetUniqueString());
            //Color32[] image = writer.Write("pleasegivemeQR");
            foreach (Color32 c in id.Image)
            {
                if (c.r != 255 || c.g != 255 || c.b != 255)
                    Debug.Log("non white pixel: " + c);
            }

            return id;
        }

        public void Scan()
        {
            // create a reader with a custom luminance source
            var barcodeReader = new BarcodeReader { AutoRotate = false, TryHarder = false };

            while (true)
            {
				Debug.Log("Loop");
                if (IsQuit)
                    break;


                // decode the current frame
				Result result = null;
				if (Color != null){
					 result = barcodeReader.Decode(Color, Width, Height);
				}
                if (result != null)
                {
                    Filter = new ArtFilter();
					Debug.Log("QR found: " + result.Text);
                    /*LastResult = result.Text;
                    Filter.Configure(result.Text);*/
                    return;
                }
                Debug.Log("Decoding failed: set Color to null");
                // Sleep a little bit and set the signal to get the next frame
                Thread.Sleep(200);
                Color = null; //if null, Update() of GUI will set Color to a new frame of the webcam
  
            }
            throw new TimeoutException("Scanning of QR code was interrupted");
        }


        public Scannable GetScanResult()
        {
            if(Filter != null){
                return Filter;
            }
            throw new MissingFieldException("The value of the result was not set, scan was not competed or unsuccesful");
        }
    }
}