using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ObjectDetectTests
{
    [TestClass]
    public class FileAccessTest
    {
        [TestMethod]
        public async Task loadInfoTestMethod()
        {
            Random rand = new Random(24);

            var tempFileName = System.IO.Path.GetTempFileName();

            string[] filenames = { System.IO.Path.GetTempFileName(), System.IO.Path.GetTempFileName(), System.IO.Path.GetTempFileName() };
            
            var expectedResult = new List<ObjectDetect.FileAccess.FileEntry>();

            using (var tempFile = new System.IO.StreamWriter(tempFileName))
            {
                foreach (var filename in filenames)
                {
                    using (System.IO.File.Create(filename)) { }

                    var numBoxes = rand.Next(30);
                    var boxes = new System.Collections.Generic.List<ObjectDetect.rectangle>();

                    tempFile.Write(filename + " " + numBoxes);

                    for (int i = numBoxes; i > 0; i--)
                    {
                        var box = new ObjectDetect.rectangle(rand.Next(2000), rand.Next(2000), rand.Next(2000), rand.Next(2000));

                        boxes.Add(box);

                        tempFile.Write((" " + box.Left + " " + box.Top + " " + box.Width + " " + box.Height).PadRight(20));
                    }

                    expectedResult.Add(new ObjectDetect.FileAccess.FileEntry(filename, boxes, 5184, 3456));

                    tempFile.WriteLine();
                }
            }

            var result = await ObjectDetect.FileAccess.loadInfo(tempFileName);

            foreach (var _ in result.Zip(expectedResult, (actual, expected) =>
            {
                Assert.AreEqual(actual.FileName, expected.FileName, actual + " " + expected);
                foreach (var _ in actual.Rectangles.Zip(expected.Rectangles, (a, e) =>
                {
                    Assert.AreEqual(a, e, a + " " + e);
                    return a;
                })) { }
                return actual;
            })) { }

            System.IO.File.Delete(tempFileName);
            foreach (var filename in filenames)
            {
                var tryDelete = true;
                while (tryDelete)
                {
                    try
                    {
                        System.IO.File.Delete(filename);
                        tryDelete = false;
                    }
                    catch (System.IO.IOException)
                    {
                        tryDelete = true;
                    }
                }
            }
        }
    }
}
