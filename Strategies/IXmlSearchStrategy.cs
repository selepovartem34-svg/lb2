using MyXmlMAUIApp.Model;

namespace MyXmlMAUIApp.Strategies;

// Стратегія аналізу XML-документа (шаблон "Стратегія")
public interface IXmlSearchStrategy
{
    string Name { get; }

    List<Software> Search(string xmlPath, SearchCriteria criteria);
}
