using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MusicLibrary.Models;
using System.IO;
using HundredMilesSoftware.UltraID3Lib;
using System.Diagnostics;
using System.Data.Entity.Validation;
using MusicLibrary.Helpers;

namespace MusicLibrary.Controllers
{
    public class TunesController : Controller
    {
        private MusicLibraryContext db = new MusicLibraryContext();

        // GET: Tunes/Load
        // Return selected tune to partial view for AJAX rendering
        [HttpGet]
        public ActionResult Load(int tuneID)
        {
            Tune tune = db.Tunes.Find(tuneID);
            return PartialView("_Load",tune);
        
        }

        // GET: Tunes/LoadPlaylistTune
        // AJAX call to load the next tune in the playlist
        [HttpGet]
        public ActionResult LoadPlaylistTune(int? position) 
        {
            Playlist plItem = new Playlist();

            int i=0;
            foreach (Playlist pl in db.Playlists)
            {
                if (i == (position ?? 0))
                {
                    plItem = pl;
                    break;
                }
                i++;   
            }


            Tune tune = db.Tunes.Find(plItem.TuneID);
            return PartialView("_Load", tune);
        }


        // GET: Tunes/Upload
        // Action to create Tune records from mp3 files in specified directory path
        public void Upload()
        {

            //  directory to enumerate
            string directory = @"I:\MP3\HOUSE\";
            string vpath;

            var files = Directory.EnumerateFiles(directory, "*.mp3");

            UltraID3 mp3 = new UltraID3();

            foreach (string filename in files)
            {
                try
                {
                    mp3.Read(filename);

                    
                    int startStrip = directory.Length;

                    vpath = @"~/Library/HOUSE/" + filename.Substring(startStrip);


                    Tune tune = new Tune
                    {
                        Album = StringHelper.Truncate(mp3.Album,30),
                        Artist = StringHelper.Truncate(mp3.Artist,30),
                        Comment = StringHelper.Truncate(mp3.Comments,30),
                        Genre = "House",
                        Title = StringHelper.Truncate(mp3.Title,30),
                        Year = mp3.Year,
                        VirtualPath = vpath
                    };

                    db.Tunes.Add(tune);
                    db.SaveChanges();
                    Debug.WriteLine("Saved " + filename);
                }
                catch (DbEntityValidationException ex)
                {

                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }

                    Debug.WriteLine("Failed to save " + filename +  " : " + ex.Message);
                }
            } // end for


        } // end method

        // AJAX method to insert the specified new tune into the playlist and return the partial view
        // Of the updated playlist
        [HttpGet]
        public ActionResult AddTuneToPlaylist(int? tuneID)
        {

            // Create the new tune in the playlist and save it
            Playlist newtune = new Playlist { TuneID = tuneID ?? 0};
            db.Playlists.Add(newtune);
            db.SaveChanges();

            // Return the updated playlist project it into an IEnumerable<PlaylistViewModel>
            var tunes = from t in db.Tunes
                        join p in db.Playlists
                        on t.TuneID equals p.TuneID
                        orderby p.PlaylistID
                        select new PlaylistViewModel
                        {
                            PlaylistID = p.PlaylistID,
                            TuneID = t.TuneID,
                            Album = t.Album,
                            Artist = t.Artist,
                            Title = t.Title
                        };

            ViewBag.PlaylistViewModel = tunes;

            return PartialView("_Playlist");
        }

        // AJAX method to remove the specified tune from the playlist and return the partial view
        // Of the updated playlist
        [HttpGet]
        public ActionResult RemoveTuneFromPlaylist(int? playlistID)
        {
            
            // Remove the tune from the playlist 
            Playlist tune = db.Playlists.Find(playlistID);
            db.Playlists.Remove(tune);
            db.SaveChanges();

            // Return the updated playlist project it into an IEnumerable<PlaylistViewModel>
            var tunes = from t in db.Tunes
                        join p in db.Playlists
                        on t.TuneID equals p.TuneID
                        orderby p.PlaylistID
                        select new PlaylistViewModel
                        {
                            PlaylistID = p.PlaylistID,
                            TuneID = t.TuneID,
                            Album = t.Album,
                            Artist = t.Artist,
                            Title = t.Title
                        };

            ViewBag.PlaylistViewModel = tunes;

            return PartialView("_Playlist");
        }


        // GET: Tunes
        public ActionResult Index()
        {
            var tunes = from t in db.Tunes
                        orderby t.Artist, t.Album, t.Title
                        select t;


            // Build the filter select lists and pass them back in the view bag
            var genres =  (from t in db.Tunes
                           orderby t.Genre
                           select t.Genre).Distinct();

            var artists = (from t in db.Tunes
                           orderby t.Artist
                           select t.Artist).Distinct();

            // The playlist
            var playlist = from t in db.Tunes
                           join p in db.Playlists
                           on t.TuneID equals p.TuneID
                           orderby p.PlaylistID
                           select new PlaylistViewModel
                           {
                               PlaylistID = p.PlaylistID,
                               TuneID = t.TuneID,
                               Album = t.Album,
                               Artist = t.Artist,
                               Title = t.Title
                           };

            ViewBag.PlaylistViewModel = playlist;
            ViewBag.ArtistDropDown = new SelectList(artists);
            ViewBag.GenreDropDown = new SelectList(genres);



            return View(tunes.ToList());
        }

        [HttpGet]
        public ActionResult FilterArtists(string genre)
        {
            var artists = (from t in db.Tunes
                           where t.Genre.Equals(genre)
                           orderby t.Artist
                           select t.Artist).Distinct();

            ViewData["ArtistDropDown"] = new SelectList(artists);

            return PartialView("_Artists");
        }

        [HttpGet]
        public ActionResult Filter(string artist, string genre)
        {

            List<Tune> tunes = new List<Tune>();

            if (genre != "")
            {
                tunes = (from t in db.Tunes
                         where t.Genre.Equals(genre)
                         orderby t.Artist, t.Album, t.Title
                         select t).ToList();
            }
            else
            {
                tunes = (from t in db.Tunes
                        where t.Artist.Equals(artist)
                        orderby t.Artist, t.Album, t.Title
                        select t).ToList();

            }  

            return PartialView("_TuneList", tunes.ToList());
                        
        }

        [HttpGet]
        public ActionResult NoFilter()
        {

            var tunes = from t in db.Tunes
                        orderby t.Artist, t.Album, t.Title
                        select t;


            return PartialView("_TuneList", tunes.ToList());

        }

        [HttpGet]
        public ActionResult Search(string criteria)
        {

            List<Tune> tunes = new List<Tune>();

            tunes = (from t in db.Tunes
                        where t.Genre.Contains(criteria) || t.Album.Contains(criteria) || t.Title.Contains(criteria) || t.Artist.Contains(criteria)
                        orderby t.Artist, t.Album, t.Title
                        select t).ToList();

            return PartialView("_TuneList", tunes.ToList());

        }


        // GET: Tunes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tune tune = db.Tunes.Find(id);
            if (tune == null)
            {
                return HttpNotFound();
            }
            return View(tune);
        }

        // GET: Tunes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tunes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tune tune)
        {
            if (ModelState.IsValid)
            {
                db.Tunes.Add(tune);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var e in errors)
            {
                Debug.WriteLine(e.ErrorMessage);
            }

            return View(tune);
        }

        // GET: Tunes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tune tune = db.Tunes.Find(id);
            if (tune == null)
            {
                return HttpNotFound();
            }
            return View(tune);
        }

        // POST: Tunes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tune tune)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tune).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tune);
        }

        // GET: Tunes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tune tune = db.Tunes.Find(id);
            if (tune == null)
            {
                return HttpNotFound();
            }
            return View(tune);
        }

        // POST: Tunes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tune tune = db.Tunes.Find(id);
            db.Tunes.Remove(tune);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
