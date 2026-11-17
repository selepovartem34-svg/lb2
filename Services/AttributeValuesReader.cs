using System.Xml.Linq;

namespace MyXmlMAUIApp.Services;

// Збирає всі атрибути елементів Program і їхні значення, які є у файлі
public static class AttributeValuesReader
{
    public static Dictionary<string, List<string>> Read(string xmlPath)
    {
        XDocument document = XDocument.Load(xmlPath);
        var programs = document.Descendants("Program").ToList();
        if (programs.Count == 0)
            throw new InvalidDataException("У файлі немає елементів Program.");

        return programs
            .Attributes()
            .GroupBy(a => a.Name.LocalName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(a => a.Value).Distinct().OrderBy(v => v).ToList());
    }
}
