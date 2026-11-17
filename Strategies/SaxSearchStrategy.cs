using System.Xml;
using MyXmlMAUIApp.Model;

namespace MyXmlMAUIApp.Strategies;

// SAX API: послідовне читання документа через XmlReader, без завантаження в пам'ять
public class SaxSearchStrategy : IXmlSearchStrategy
{
    public string Name => "SAX API";

    public List<Software> Search(string xmlPath, SearchCriteria criteria)
    {
        var result = new List<Software>();
        var settings = new XmlReaderSettings { IgnoreWhitespace = true, IgnoreComments = true };

        using var reader = XmlReader.Create(xmlPath, settings);
        Software? current = null;
        string element = "";

        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    element = reader.Name;
                    if (element == "Program")
                    {
                        current = new Software();
                        ReadAttributes(reader, current);
                        if (reader.IsEmptyElement)
                        {
                            AddIfMatch(current, criteria, result);
                            current = null;
                        }
                    }
                    break;

                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                    current?.SetElement(element, reader.Value);
                    break;

                case XmlNodeType.EndElement:
                    if (reader.Name == "Program" && current != null)
                    {
                        AddIfMatch(current, criteria, result);
                        current = null;
                    }
                    element = "";
                    break;
            }
        }

        return result;
    }

    private static void ReadAttributes(XmlReader reader, Software software)
    {
        if (!reader.MoveToFirstAttribute())
            return;

        do
        {
            software.Attributes[reader.Name] = reader.Value;
        } while (reader.MoveToNextAttribute());

        reader.MoveToElement();
    }

    private static void AddIfMatch(Software software, SearchCriteria criteria, List<Software> result)
    {
        if (criteria.IsMatch(software))
            result.Add(software);
    }
}
