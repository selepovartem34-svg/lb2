using System.Xml;
using MyXmlMAUIApp.Model;

namespace MyXmlMAUIApp.Strategies;

// DOM API: документ завантажується в XmlDocument, запит будується як XPath
public class DomSearchStrategy : IXmlSearchStrategy
{
    public string Name => "DOM API";

    public List<Software> Search(string xmlPath, SearchCriteria criteria)
    {
        var document = new XmlDocument();
        document.Load(xmlPath);

        var result = new List<Software>();
        XmlNodeList? nodes = document.SelectNodes(BuildXPath(criteria));
        if (nodes == null)
            return result;

        foreach (XmlNode node in nodes)
        {
            var software = ToSoftware(node);
            if (software.ContainsKeyword(criteria.Keyword))
                result.Add(software);
        }
        return result;
    }

    // Динамічна генерація запиту, наприклад: //Program[@Type='СУБД' and @License='Вільна']
    public static string BuildXPath(SearchCriteria criteria)
    {
        if (criteria.Attributes.Count == 0)
            return "//Program";

        var conditions = criteria.Attributes.Select(p => $"@{p.Key}={ToXPathLiteral(p.Value)}");
        return $"//Program[{string.Join(" and ", conditions)}]";
    }

    // Значення може містити апостроф (комп'ютерних), тоді беру інші лапки або concat
    private static string ToXPathLiteral(string value)
    {
        if (!value.Contains('\''))
            return $"'{value}'";
        if (!value.Contains('"'))
            return $"\"{value}\"";

        var parts = value.Split('\'');
        return "concat('" + string.Join("', \"'\", '", parts) + "')";
    }

    private static Software ToSoftware(XmlNode node)
    {
        var software = new Software();
        if (node.Attributes != null)
        {
            foreach (XmlAttribute attribute in node.Attributes)
                software.Attributes[attribute.Name] = attribute.Value;
        }

        foreach (XmlNode child in node.ChildNodes)
        {
            if (child.NodeType == XmlNodeType.Element)
                software.SetElement(child.Name, child.InnerText);
        }
        return software;
    }
}
