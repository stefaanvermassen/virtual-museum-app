using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace ScanEngine
{
    class QRCode: ScanIdentity
    {

        public Bitmap Image { get; set; }

        public QRCode():base(ScanTechnology.QR)
        {

        }

    }
}
