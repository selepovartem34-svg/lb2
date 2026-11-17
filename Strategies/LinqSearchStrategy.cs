using System.Xml.Linq;
using MyXmlMAUIApp.Model;

namespace MyXmlMAUIApp.Strategies;

// LINQ to XML: запит складається з умов Where, які додаються залежно від вибору користувача
public class LinqSearchStrategy : IXmlSearchStrategy
{
    public string Name => "LINQ to XML";

    public List<Software> Search(string xmlPath, SearchCriteria criteria)
    {
        XDocument document = XDocument.Load(xmlPath);
        IEnumerable<XElement> query = document.Descendants("Program");

        foreach (var pair in criteria.Attributes)
        {
            string name = pair.Key;
            string value = pair.Value;
            query = query.Where(e => (string?)e.Attribute(name) == value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Keyword))
        {
            string keyword = criteria.Keyword.Trim();
            query = query.Where(e =>
                e.Attributes().Any(a => Software.ContainsText(a.Value, keyword)) ||
                e.Elements().Any(c => Software.ContainsText(c.Value, keyword)));
        }

        return query.Select(ToSoftware).ToList();
    }

    private static Software ToSoftware(XElement element)
    {
        var software = new Software();
        foreach (var attribute in element.Attributes())
            software.Attributes[attribute.Name.LocalName] = attribute.Value;

        foreach (var child in element.Elements())
            software.SetElement(child.Name.LocalName, child.Value);

        return software;
    }
}
