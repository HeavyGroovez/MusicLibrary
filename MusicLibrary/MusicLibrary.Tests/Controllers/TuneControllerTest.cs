﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicLibrary;
using MusicLibrary.Controllers;
using MusicLibrary.Models;


namespace MusicLibrary.Tests.Controllers
{
    [TestClass]
    public class TunesControllerTest
    {
        [TestMethod]
        public void TestMethod1()
        {

            // Arrange
            TunesController controller = new TunesController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
