using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;

namespace CSharpSkills.Common
{
    /// <summary>
    /// html이나 xml 자료를 받는다 가정할 때, XmlDocument, HtmlAgilityPack.HtmlDocument 를 이용해서 파싱할 수 있다.
    /// </summary>
    public class XpathExample
    {
        private XmlDocument xmlDoc;
        private HtmlWeb web;
        private HtmlDocument htmlDoc;
        public HtmlNode node;

        public XpathExample()
        {
            string url = "https://potatocompletion.tistory.com/";

            web = new HtmlWeb();
            htmlDoc = web.Load(url);
            node = searchSingleNode(htmlDoc, "//body");
        }

        public HtmlNode searchSingleNode(HtmlDocument htmlDoc, string searchKeyword)
        {
            return htmlDoc.DocumentNode.SelectSingleNode(searchKeyword);
        }
    }
}
