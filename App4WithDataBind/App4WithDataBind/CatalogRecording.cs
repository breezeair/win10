using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App4WithDataBind
{
public class CatalogRecording
    {
        public string ArtistName { get; set; }
        public string CompositionName { get; set; }
        public DateTime ReleaseDateTime { get; set; }
        public CatalogRecording()
        {
            this.ArtistName = "W A M";
            this.CompositionName = "Book name 1";
            this.ReleaseDateTime = new DateTime(2000, 1, 1);
        }
        public string OneLineSummary
        {
            get
            {
                return $"{this.CompositionName} by {this.ArtistName}, released: "
                    + this.ReleaseDateTime.ToString("d");
            }
        }
    }

}
