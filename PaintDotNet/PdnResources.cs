/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet.SystemLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;

namespace PaintDotNet
{
    public static class PdnResources
    {
        private static ResourceManager resourceManager;
        private const string ourNamespace = "PaintDotNet";
        private static Assembly ourAssembly;
        private static string[] localeDirs;
        private static CultureInfo pdnCulture;
        private static string resourcesDir;

        public static string ResourcesDir
        {
            get
            {
                if (resourcesDir == null)
                {
                    resourcesDir = Path.GetDirectoryName(typeof(PdnResources).Assembly.Location);
                }

                return resourcesDir;
            }

            set
            {
                resourcesDir = value;
                Initialize();
            }
        }

        public static CultureInfo Culture
        {
            get
            {
                return pdnCulture;
            }

            set
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = value;
                Initialize();
            }
        }

        private static void Initialize()
        {
            resourceManager = CreateResourceManager();
            ourAssembly = Assembly.GetExecutingAssembly();
            pdnCulture = CultureInfo.CurrentUICulture;
            localeDirs = GetLocaleDirs();
        }

        static PdnResources()
        {
            Initialize();
        }



        public static string[] GetInstalledLocales()
        {
            const string left = "PaintDotNet.Strings.3";
            const string right = ".resources";
            string ourDir = ResourcesDir;
            string fileSpec = left + "*" + right;
            string[] pathNames = Directory.GetFiles(ourDir, fileSpec);
            List<String> locales = new List<string>();

            for (int i = 0; i < pathNames.Length; ++i)
            {
                string pathName = pathNames[i];
                string dirName = Path.GetDirectoryName(pathName);
                string fileName = Path.GetFileName(pathName);
                string sansRight = fileName.Substring(0, fileName.Length - right.Length);
                string sansLeft = sansRight.Substring(left.Length);

                string locale;

                if (sansLeft.Length > 0 && sansLeft[0] == '.')
                {
                    locale = sansLeft.Substring(1);
                }
                else if (sansLeft.Length == 0)
                {
                    locale = "en-US";
                }
                else
                {
                    locale = sansLeft;
                }

                try
                {
                    // Ensure this locale can create a valid CultureInfo object.
                    CultureInfo ci = new CultureInfo(locale);
                }

                catch (Exception)
                {
                    // Skip past invalid locales -- don't let them crash us
                    continue;
                }

                locales.Add(locale);
            }

            return locales.ToArray();
        }

        public static string[] GetLocaleNameChain()
        {
            List<string> names = new List<string>();
            CultureInfo ci = pdnCulture;

            while (ci.Name != string.Empty)
            {
                names.Add(ci.Name);
                ci = ci.Parent;
            }

            return names.ToArray();
        }

        private static string[] GetLocaleDirs()
        {
            const string rootDirName = "Resources";
            string appDir = ResourcesDir;
            string rootDir = Path.Combine(appDir, rootDirName);
            List<string> dirs = new List<string>();

            CultureInfo ci = pdnCulture;

            while (ci.Name != string.Empty)
            {
                string localeDir = Path.Combine(rootDir, ci.Name);

                if (Directory.Exists(localeDir))
                {
                    dirs.Add(localeDir);
                }

                ci = ci.Parent;
            }

            return dirs.ToArray();
        }

        private static ResourceManager CreateResourceManager()
        {
            const string stringsFileName = "PaintDotNet.Strings.3";
            ResourceManager rm = ResourceManager.CreateFileBasedResourceManager(stringsFileName, ResourcesDir, null);
            return rm;
        }

        public static ResourceManager Strings
        {
            get
            {
                return resourceManager;
            }
        }

        public static string GetString(string stringName)
        {
            string theString = resourceManager.GetString(stringName, pdnCulture);

            if (theString == null)
            {
                Debug.WriteLine(stringName + " not found");
            }

            return theString;
        }

        public static Stream GetResourceStream(string fileName)
        {
            Stream stream = null;

            for (int i = 0; i < localeDirs.Length; ++i)
            {
                string filePath = Path.Combine(localeDirs[i], fileName);

                if (File.Exists(filePath))
                {
                    stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    break;
                }
            }

            if (stream == null)
            {
                string fullName = ourNamespace + "." + fileName;
                stream = ourAssembly.GetManifestResourceStream(fullName);
            }

            return stream;
        }

        private static bool CheckForSignature(Stream input, byte[] signature)
        {
            long oldPos = input.Position;
            byte[] inputSig = new byte[signature.Length];
            int amountRead = input.Read(inputSig, 0, inputSig.Length);

            bool foundSig = false;
            if (amountRead == signature.Length)
            {
                foundSig = true;

                for (int i = 0; i < signature.Length; ++i)
                {
                    foundSig &= (signature[i] == inputSig[i]);
                }
            }

            input.Position = oldPos;
            return foundSig;
        }

        public static bool IsGdiPlusImageAllowed(Stream input)
        {
            byte[] wmfSig = new byte[] { 0xd7, 0xcd, 0xc6, 0x9a };
            byte[] emfSig = new byte[] { 0x01, 0x00, 0x00, 0x00 };

            // Check for and explicitely block WMF and EMF images
            return !(CheckForSignature(input, emfSig) || CheckForSignature(input, wmfSig));
        }

    }
}