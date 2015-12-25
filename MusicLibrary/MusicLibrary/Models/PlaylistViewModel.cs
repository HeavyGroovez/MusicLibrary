using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicLibrary.Models
{
    public class PlaylistViewModel
    {
        public int PlaylistID { get; set; }
        public int TuneID { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
    }
}