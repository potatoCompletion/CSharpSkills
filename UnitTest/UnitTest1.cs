using CSharpSkills.Common;

namespace UnitTest
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            XpathExample ex = new XpathExample();
            Console.WriteLine(ex.node.InnerHtml);
            Assert.That(ex.node != null);
        }
    }
}