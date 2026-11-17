using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using MyXmlMAUIApp.Model;

namespace MyXmlMAUIApp.Services;

// Трансформація XML у HTML за XSL-документом
public class XslTransformer
{
    private readonly XslCompiledTransform xslt = new();

    public XslTransformer(string xslPath)
    {
        xslt.Load(xslPath);
    }

    // Весь XML-файл
    public void Transform(string xmlPath, string htmlPath)
    {
        using var writer = XmlWriter.Create(htmlPath, xslt.OutputSettings);
        xslt.Transform(xmlPath, writer);
    }

    // Тільки результати пошуку
    public void Transform(IEnumerable<Software> items, string htmlPath)
    {
        var document = new XDocument(
            new XElement("Software",
                new XAttribute("Title", "Результати пошуку"),
                items.Select(s => s.ToXElement())));

        using var reader = document.CreateReader();
        using var writer = XmlWriter.Create(htmlPath, xslt.OutputSettings);
        xslt.Transform(reader, writer);
    }
}
