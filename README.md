# ddsfiletype-plus
Quick and dirty solution to add DDS BC7 write/read support to .NET using paint.net plugin.

This project is [pdn-ddsfiletype-plus](https://github.com/0xC0000054/pdn-ddsfiletype-plus) plugin for Paint.NET and last open-sourced version of Paint.NET 3.36.7 stripped down to necessary elements.

## Usage example
```c#
        using PaintDotNet;
        using DdsFileTypePlus;

        void Main()
        {     
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;

            Surface surface = DdsFile.Load(@"S:\testDDS\BC7.dds"); //get Paint.NET Surface
            
            System.Drawing.Bitmap bitmap = surface.CreateAliasedBitmap(); // convert to Bitmap
            MagickImage magickImage = new MagickImage(bitmap); // convert to Magick.NET MagickImage
            NetVips.Image vipsImage;
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                vipsImage = Image.NewFromBuffer(stream.ToArray()); // convert to NetVips Image
            }
            
            // do something with images
            
            System.Drawing.Bitmap processedBitmap = magickImage.ToBitmap();
            Surface processedSurface = Surface.CopyFromBitmap(processedBitmap);

            System.IO.FileStream fileStream = new System.IO.FileStream(
                @"S:\testDDS\result.dds",
                System.IO.FileMode.Create);   
            
             DdsFile.Save(
                fileStream,
                DdsFileFormat.BC7,
                DdsErrorMetric.Perceptual,
                BC7CompressionMode.Fast,
                cubeMap: true,
                generateMipMaps: true,
                ResamplingAlgorithm.Bilinear,
                processedSurface,
                ProgressChanged);
                
            fileStream.Close();
        }

        private void ProgressChanged(object sender, ProgressEventArgs e)
        {
            progressBar1.Value = (int) Math.Round(e.Percent);
        }         
```
