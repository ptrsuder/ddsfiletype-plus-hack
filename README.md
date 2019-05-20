# ddsfiletype-plus
Quick and dirty solution to add DDS BC7 write/read support to .NET using paint.net plugin.

Uses [pdn-ddsfiletype-plus](https://github.com/0xC0000054/pdn-ddsfiletype-plus) plugin for Paint.NET and last open-sourced version of Paint.NET 3.36.7 stripped down to necessary elements.

## Use example
```c#
        void Main()
        {            
            progressBar1.Maximum = 100;

            Surface surface = DdsFile.Load(@"S:\testDDS\BC7.dds");

            System.IO.FileStream fileStream = new System.IO.FileStream(
                @"S:\testDDS\result.dds",
                System.IO.FileMode.Create);   
            
            DdsFile.Save(
                fileStream,
                DdsFileFormat.BC7,
                DdsErrorMetric.Perceptual,
                BC7CompressionMode.Fast,
                true,
                true,
                ResamplingAlgorithm.Bilinear,
                surface,
                ProgressChanged                );
            fileStream.Close();
        }

        private void ProgressChanged(object sender, ProgressEventArgs e)
        {
            progressBar1.Value = (int) Math.Round(e.Percent);
        }         
```
