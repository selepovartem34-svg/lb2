namespace MyXmlMAUIApp.Model;

// Параметри пошуку: вибрані значення атрибутів + ключове слово
public class SearchCriteria
{
    // ключ - назва атрибута, значення - вибране користувачем значення
    public Dictionary<string, string> Attributes { get; } = new();

    public string Keyword { get; set; } = "";

    public bool IsMatch(Software software)
    {
        foreach (var pair in Attributes)
        {
            if (!software.Attributes.TryGetValue(pair.Key, out var value) || value != pair.Value)
                return false;
        }
        return software.ContainsKeyword(Keyword);
    }
}
