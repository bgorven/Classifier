using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class FileAccessTest
    {
        [TestMethod]
        public void loadInfoTestMethod()
        {
            Random rand = new Random(24);

            var tempFileName = System.IO.Path.GetTempFileName();
            var tempFileUri = new Uri(tempFileName);

            Uri[] filenames = { new Uri(tempFileUri, "asdf"), new Uri(tempFileUri, "asfd"), new Uri(tempFileUri, "zxbq") };
            
            var expectedResult = new System.Collections.Generic.List<Tuple<Uri, ObjectDetect.ImageSample[]>>();

            using (var tempFile = new System.IO.StreamWriter(tempFileName))
            {
                foreach (var filename in filenames)
                {
                    using (System.IO.File.Create(filename.AbsolutePath)) { };

                    var numBoxes = rand.Next(30);
                    var boxes = new System.Collections.Generic.List<ObjectDetect.ImageSample>();

                    tempFile.Write(filename.AbsolutePath + " " + numBoxes);

                    for (int i = numBoxes; i > 0; i--)
                    {
                        var box = new ObjectDetect.ImageSample(filename, rand.Next(2000), rand.Next(2000), rand.Next(2000), rand.Next(2000));

                        boxes.Add(box);

                        tempFile.Write((" " + box.Left + " " + box.Top + " " + box.Width + " " + box.Height).PadRight(20));
                    }

                    expectedResult.Add(Tuple.Create(filename, boxes.ToArray()));

                    tempFile.WriteLine();
                }
            }

            var result = ObjectDetect.FileAccess.loadInfo(tempFileName);

            foreach (var _ in result.Zip(expectedResult, (actual, expected) =>
            {
                Assert.AreEqual(actual.Item1, expected.Item1, actual + " " + expected);
                foreach (var _ in actual.Item2.Zip(expected.Item2, (a, e) =>
                {
                    Assert.AreEqual(a, e, a + " " + e);
                    Assert.IsTrue(a.fileProbablyEquals(e));
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
