using CSharpSkills.Common;

namespace UnitTest
{
    public class Tests
    {
        [Test]
        public void ����׽�Ʈ()
        {
            XpathExample ex = new XpathExample();
            Console.WriteLine(ex.node.InnerHtml);
            Assert.That(ex.node != null);
        }
    }
}