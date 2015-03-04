using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;


namespace Scanning
{
    public class QRCode: ScanIdentity
    {

		public Texture2D Image { get; set; }

        public QRCode():base(ScanTechnology.QR)
        {

        }

    }
}
