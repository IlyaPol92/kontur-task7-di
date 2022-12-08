using System;
using System.Drawing;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TagCloud;
using TagCloud.PointGenerators;
using TagCloud.Tags;
using TagCloud.TagCloudVisualizations;

namespace TagCloudTests
{
    public class CircularCloudLayouterTests
    {
        private readonly string failedTestsPictureFolder = "FailedTestsPicture";
        private CircularCloudLayouter cloudLayouter;

        [SetUp]
        public void PrepareCircularCloudLayouter()
        {
            var center = new Point();
            cloudLayouter = new CircularCloudLayouter(new SpiralPointGenerator(center));
        }

        [TearDown]
        public void SavePicture_WhenTestFailed()
        {
            var context = TestContext.CurrentContext;
            if (context.Result.Outcome.Status != TestStatus.Failed)
                return;

            Directory.CreateDirectory(failedTestsPictureFolder);
            var fileName = $"{context.Test.MethodName}{new Random().Next()}";
            var filePath = Path.Combine(failedTestsPictureFolder, fileName);

            File.WriteAllText(filePath + ".txt", $"The test {context.Test.FullName} failed with an error: {context.Result.Message}" + 
                                                 Environment.NewLine + "StackTrace:" + context.Result.StackTrace);
            var visualization = new TagCloudBitmapVisualization(cloudLayouter.GetTagCloudOfLayout());
            visualization.Save(filePath + ".bmp");

            TestContext.WriteLine($"Tag cloud visualization saved to file {filePath}");
        }



        [TestCase(0, 0, TestName = "in zero point")]
        [TestCase(5, 10, TestName = "in point(5, 10)")]
        public void Ctor_SetCenterPoint(int x, int y)
        {
            var planningCenter = new Point(x, y);

            var tagCloud = new CircularCloudLayouter(new SpiralPointGenerator(planningCenter)).GetTagCloudOfLayout();

            tagCloud.GetWidth().Should().Be(0);
            tagCloud.GetHeight().Should().Be(0);
        }

        [TestCase(0, 0, TestName = "width and height are equal to zero")]
        [TestCase(0, 10, TestName = "width is zero")]
        [TestCase(10, 0, TestName = "height is zero")]
        public void PutNextRectangle_ThrowArgumentException_When(int width, int height)
        {
            Action act = () => cloudLayouter.PutNextRectangle(new Size(width, height));

            act.Should().Throw<ArgumentException>();
        }

        [TestCase(0, 0, 350, 750)]
        [TestCase(3, 3, 500, 500)]
        public void PutNextRectangle_ReturnedNotIntersectedRectangle(int centerX, int centerY, int firstRectWidth, int firstRectHeight)
        {
            var center = new Point(centerX, centerY);
            cloudLayouter = new CircularCloudLayouter(new SpiralPointGenerator(center));

            do
            {
                var rectSize = new Size(firstRectWidth, firstRectHeight);
                var newRect = cloudLayouter.PutNextRectangle(rectSize);
                cloudLayouter.GetTagCloudOfLayout().Rectangles.Where(rect => rect.Frame != newRect).
                    All(rect => rect?.Frame.IntersectsWith(newRect) == false).Should().BeTrue();

                firstRectHeight /= 2;
                firstRectWidth /= 2;
            } while (firstRectHeight > 1 && firstRectWidth > 1);
        }
    }
}