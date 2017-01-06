using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App4WithDataBind
{
    public class CatalogRecordingViewModel
    {
        private CatalogRecording defaultRecording = new CatalogRecording();
        public CatalogRecording DefaultRecording { get { return this.defaultRecording; } }

        private ObservableCollection<CatalogRecording> recordings = new ObservableCollection<CatalogRecording>();
        public ObservableCollection<CatalogRecording> Recordings { get { return this.recordings; } }

        public CatalogRecordingViewModel()
        {
            this.recordings.Add(new CatalogRecording()
            {
                ArtistName = "J S B",
                CompositionName = "Book name 2",
                ReleaseDateTime = new DateTime(2001, 1, 2)
            });
            this.recordings.Add(new CatalogRecording()
            {
                ArtistName = "L V B",
                CompositionName = "Book name 3",
                ReleaseDateTime = new DateTime(2002, 1, 3)
            });
            this.recordings.Add(new CatalogRecording()
            {
                ArtistName = "G F H",
                CompositionName = "Book name 4",
                ReleaseDateTime = new DateTime(2003, 1, 4)
            });
        }
    }
}
