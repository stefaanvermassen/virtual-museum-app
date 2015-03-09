using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using ZXing;
using ZXing.QrCode;


namespace Scanning
{
    public class QRCode: ScanIdentity
    {


        public Color32[] Image { get; set; }
        public QRCode():base(ScanTechnology.QR)
        {

        }

    }
}
