using MyXmlMAUIApp.Model;
using MyXmlMAUIApp.Strategies;

namespace MyXmlMAUIApp.Services;

// Контекст шаблону "Стратегія": не знає, яким способом аналізується файл
public class XmlSearcher
{
    private IXmlSearchStrategy strategy;

    public XmlSearcher(IXmlSearchStrategy strategy)
    {
        this.strategy = strategy;
    }

    public string StrategyName => strategy.Name;

    public void SetStrategy(IXmlSearchStrategy newStrategy)
    {
        strategy = newStrategy;
    }

    public List<Software> Search(string xmlPath, SearchCriteria criteria)
    {
        return strategy.Search(xmlPath, criteria);
    }
}
