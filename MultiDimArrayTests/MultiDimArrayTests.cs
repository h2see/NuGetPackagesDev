using MultiDimArray;
namespace MultiDimArrayTests
{
    public class Tests
    {
        [Test]
        public void AddTest() {
            var aData = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var bData = new double[] { 10, 11, 12, 13, 14, 15, 16, 17, 18 };
            using var a = new MultiDimArray<double>(3, 3);
            var idx = 0;
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    a[i, j] = aData[idx++];
                }
            }
            using var b = new MultiDimArray<double>(3, 3);
            idx = 0;
            for (int i = 0; i < b.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    b[i, j] = bData[idx++];
                }
            }
            using var c = a + b;
            for (int i = 0; i < c.GetLength(0); i++)
            {
                for (int j = 0; j < c.GetLength(1); j++)
                {
                    Console.WriteLine(c[i, j]);
                }
            }
        }

    }
}
