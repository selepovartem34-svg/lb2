using System.Text;
using System.Xml.Linq;

namespace MyXmlMAUIApp.Model;

// Одна програма з XML-файлу (елемент Program)
public class Software
{
    // Підписи атрибутів для інтерфейсу
    private static readonly Dictionary<string, string> Captions = new()
    {
        ["Name"] = "Назва",
        ["Type"] = "Вид",
        ["Version"] = "Версія",
        ["Author"] = "Автор",
        ["Faculty"] = "Факультет",
        ["Department"] = "Кафедра",
        ["License"] = "Ліцензія"
    };

    // У різних програм різна кількість атрибутів, тому зберігаю їх у словнику
    public Dictionary<string, string> Attributes { get; } = new();

    public string Annotation { get; set; } = "";
    public string Conditions { get; set; } = "";
    public string Distributive { get; set; } = "";

    public string Title => Attributes.TryGetValue("Name", out var name) ? name : "(без назви)";

    public string Details
    {
        get
        {
            var sb = new StringBuilder();
            foreach (var pair in Attributes)
            {
                if (pair.Key != "Name")
                    sb.AppendLine($"{GetCaption(pair.Key)}: {pair.Value}");
            }
            sb.AppendLine($"Анотація: {Annotation}");
            sb.AppendLine($"Умови використання: {Conditions}");
            sb.Append($"Дистрибутив: {Distributive}");
            return sb.ToString();
        }
    }

    public static string GetCaption(string attribute)
    {
        return Captions.TryGetValue(attribute, out var caption) ? caption : attribute;
    }

    // Заповнення вкладених елементів (однаково для SAX, DOM і LINQ)
    public void SetElement(string name, string value)
    {
        switch (name)
        {
            case "Annotation":
                Annotation = value.Trim();
                break;
            case "Conditions":
                Conditions = value.Trim();
                break;
            case "Distributive":
                Distributive = value.Trim();
                break;
        }
    }

    public bool ContainsKeyword(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return true;

        keyword = keyword.Trim();
        return Attributes.Values.Any(v => ContainsText(v, keyword))
               || ContainsText(Annotation, keyword)
               || ContainsText(Conditions, keyword)
               || ContainsText(Distributive, keyword);
    }

    public static bool ContainsText(string text, string keyword)
    {
        return text.Contains(keyword, StringComparison.CurrentCultureIgnoreCase);
    }

    // Назад у XML, щоб трансформувати в HTML тільки знайдені програми
    public XElement ToXElement()
    {
        var element = new XElement("Program");
        foreach (var pair in Attributes)
            element.Add(new XAttribute(pair.Key, pair.Value));

        element.Add(new XElement("Annotation", Annotation));
        element.Add(new XElement("Conditions", Conditions));
        element.Add(new XElement("Distributive", Distributive));
        return element;
    }
}
