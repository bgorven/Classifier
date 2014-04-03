using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Util.Collections;
using ObjectDetect;
using FileAccess = ObjectDetect.FileAccess;

namespace ObjectDetectTests
{
    [TestClass]
    public class FileAccessTest
    {
        [TestMethod]
        public async Task LoadInfoTestMethod()
        {
            var rand = new Random(24);

            var tempFileName = Path.GetTempFileName();

            string[] filenames = { Path.GetTempFileName(), Path.GetTempFileName(), Path.GetTempFileName() };
            
            var expectedResult = new List<FileAccess.FileEntry>();

            using (var tempFile = new StreamWriter(tempFileName))
            {
                foreach (var filename in filenames)
                {
                    using (File.Create(filename)) { }

                    var numBoxes = rand.Next(30);
                    var boxes = new List<Rectangle>();

                    tempFile.Write(filename + " " + numBoxes);

                    for (var i = numBoxes; i > 0; i--)
                    {
                        var box = new Rectangle(rand.Next(2000), rand.Next(2000), rand.Next(2000), rand.Next(2000));

                        boxes.Add(box);

                        tempFile.Write((" " + box.Left + " " + box.Top + " " + box.Width + " " + box.Height).PadRight(20));
                    }

                    expectedResult.Add(new FileAccess.FileEntry(filename, boxes, 5184, 3456, 128, 512, 4, 6));

                    tempFile.WriteLine();
                }
            }

            var result = await FileAccess.LoadInfo(tempFileName);

            result.Zip(expectedResult, (actual, expected) =>
            {
                Assert.AreEqual(actual.FileName, expected.FileName, actual + " " + expected);
                actual.Rectangles.Zip(expected.Rectangles, (a, e) => Assert.AreEqual(a, e, a + " " + e));
            });

            File.Delete(tempFileName);
            foreach (var filename in filenames)
            {
                var tryDelete = true;
                while (tryDelete)
                {
                    try
                    {
                        File.Delete(filename);
                        tryDelete = false;
                    }
                    catch (IOException)
                    {
                        tryDelete = true;
                    }
                }
            }
        }
    }
}
