using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

public class GenerateFutureResultTest
{
    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    public void GenerateResultTest(int runCount)
    {
        ResultGenerator generator = new ResultGenerator(100);
        ResultObject[] resultObjects = generator.GenerateNewResults();
        ResultConfig config = generator.GetConfig();
        List<int[]> placementPoints = generator.GetPlacementPoints();
        foreach (ResultObject resultObject in config.ResultObjects)
        {
            int count = 0;
            int countInRange = 0;
            int[] points = placementPoints[resultObject.resultNumber];
            for (int index = 0; index < resultObjects.Length; index++)
            {
                if (index > 0 && points.Contains(index) || index == 100)
                {
                    Assert.AreEqual(1, countInRange);
                    countInRange = 0;
                }

                ResultObject obj = resultObjects[index];
                if (obj.resultNumber == resultObject.resultNumber)
                {
                    count++;
                    countInRange++;
                }
            }

            Assert.AreEqual(count, resultObject.percentage);
        }

        Assert.AreEqual(100, resultObjects.Length);
    }
}