using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/**
 * This class contains collected art, not art made by the user himself
 * */
    class ArtCollection
    {

        public ArtCollection()
        {

        }

        private List<ArtFilter> Collected { get; set; }

        public void AddCollected(ArtFilter filter)
        {
            Collected.Add(filter);
        }



    }
