using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Drawing;


namespace Scanning
{
    public class QRCode: ScanIdentity
    {

		public Bitmap Image { get; set; }

        public QRCode():base(ScanTechnology.QR)
        {

        }

    }
}
