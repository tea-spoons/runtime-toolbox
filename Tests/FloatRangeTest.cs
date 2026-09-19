
namespace TeaSpoons.RuntimeToolbox.Editor.Tests
{
    using NUnit.Framework;

    public class FloatRangeTest
    {
        [Test]
        public void TestOperands()
        {
            var floatRange = new FloatRange(-1, 2);
            var dividedRange = floatRange / 2;
            var multipliedRange = floatRange * 2;

            Assert.AreEqual(new FloatRange(-1, 2), floatRange);
            Assert.AreEqual(new FloatRange(-0.5f, 1), dividedRange);
            Assert.AreEqual(new FloatRange(-2, 4), multipliedRange);
        }
    }
}
