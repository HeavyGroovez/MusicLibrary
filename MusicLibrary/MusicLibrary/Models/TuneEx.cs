using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace MusicLibrary.Models
{
    [MetadataType(typeof(TuneMetadata))]
    public partial class Tune
    {

        internal sealed class TuneMetadata 
        {

            [Required]
            [Display(Name="Title")]
            [MinLength(1)]
            [MaxLength(30)]
            public string Title { get; set; }

            [Required]
            [Display(Name = "Album")]
            [MinLength(1)]
            [MaxLength(30)]
            public string Album { get; set; }

            [Required]
            [Display(Name = "Artist")]
            [MinLength(1)]
            [MaxLength(30)]
            public string Artist { get; set; }

            [Required]
            [Display(Name = "Year")]
            [Year(ErrorMessage="Invalid Year")]
            public Nullable<int> Year { get; set; }


            [Display(Name = "Comment")]
            public string Comment { get; set; }


            [Display(Name = "Genre")]
            public string Genre { get; set; }

            [Required]
            [Display(Name = "Virtual Path")]
            public string VirtualPath { get; set; }
        }
    }
}