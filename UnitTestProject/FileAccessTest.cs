using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class FileAccessTest
    {
        [TestMethod]
        public async Task loadInfoTestMethod()
        {
            Random rand = new Random(24);

            var tempFileName = System.IO.Path.GetTempFileName();
            var tempFileUri = new Uri(tempFileName);

            Uri[] filenames = { new Uri(tempFileUri, "asdf"), new Uri(tempFileUri, "asfd"), new Uri(tempFileUri, "zxbq") };
            
            var expectedResult = new List<ObjectDetect.FileAccess.Pair>();

            using (var tempFile = new System.IO.StreamWriter(tempFileName))
            {
                foreach (var filename in filenames)
                {
                    using (System.IO.File.Create(filename.AbsolutePath)) { };

                    var numBoxes = rand.Next(30);
                    var boxes = new System.Collections.Generic.List<ObjectDetect.rectangle>();

                    tempFile.Write(filename.AbsolutePath + " " + numBoxes);

                    for (int i = numBoxes; i > 0; i--)
                    {
                        var box = new ObjectDetect.rectangle(rand.Next(2000), rand.Next(2000), rand.Next(2000), rand.Next(2000));

                        boxes.Add(box);

                        tempFile.Write((" " + box.Left + " " + box.Top + " " + box.Width + " " + box.Height).PadRight(20));
                    }

                    expectedResult.Add(new ObjectDetect.FileAccess.Pair(filename, boxes));

                    tempFile.WriteLine();
                }
            }

            var result = await ObjectDetect.FileAccess.loadInfo(tempFileName);

            foreach (var _ in result.Zip(expectedResult, (actual, expected) =>
            {
                Assert.AreEqual(actual.File, expected.File, actual + " " + expected);
                foreach (var _ in actual.Rectangles.Zip(expected.Rectangles, (a, e) =>
                {
                    Assert.AreEqual(a, e, a + " " + e);
                    return a;
                })) ;
                return actual;
            }));

            System.IO.File.Delete(tempFileName);
            foreach (var filename in filenames)
            {
                var tryDelete = true;
                while (tryDelete)
                {
                    try
                    {
                        System.IO.File.Delete(filename.AbsolutePath);
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
