using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

using System.Xml.Linq;

namespace MusicLibrary
{
    public static class DelegateTest
    {
        delegate void TestDelegate(string s);

        static void oldskool (String s)
        {
            Debug.WriteLine("oldskool delegate method");
        }

        static void TestMethod()
        {

            // C#1.0 
            TestDelegate testA = new TestDelegate(oldskool);


            // C#2.0
            TestDelegate testB = delegate(string s) { Debug.WriteLine(s); };


            // C#3.0
            TestDelegate testC = (x) => { Debug.WriteLine(x); };

            testA("Old skool");
            testB("A lot terser");
            testC("Lamdas bitch !!");

        }

    }
}