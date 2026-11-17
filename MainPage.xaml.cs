using MyXmlMAUIApp.Model;
using MyXmlMAUIApp.Services;
using MyXmlMAUIApp.Strategies;

namespace MyXmlMAUIApp;

public partial class MainPage : ContentPage
{
    private static readonly FilePickerFileType XmlFileType = new(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.WinUI, new[] { ".xml" } },
        { DevicePlatform.Android, new[] { "text/xml", "application/xml" } },
        { DevicePlatform.iOS, new[] { "public.xml" } },
        { DevicePlatform.MacCatalyst, new[] { "public.xml" } }
    });

    private static readonly FilePickerFileType XslFileType = new(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.WinUI, new[] { ".xsl", ".xslt" } },
        { DevicePlatform.Android, new[] { "text/xml", "application/xml", "application/xslt+xml" } },
        { DevicePlatform.iOS, new[] { "public.xml" } },
        { DevicePlatform.MacCatalyst, new[] { "public.xml" } }
    });

    private readonly XmlSearcher searcher = new(new SaxSearchStrategy());
    private readonly Dictionary<string, (CheckBox Check, Picker Picker)> attributeControls = new();
    private string? xmlPath;
    private List<Software>? lastResults;   // null - пошук ще не виконувався

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OpenButton_Clicked(object sender, EventArgs e)
    {
        var file = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Виберіть XML-файл",
            FileTypes = XmlFileType
        });
        if (file == null)
            return;

        try
        {
            var values = AttributeValuesReader.Read(file.FullPath);
            xmlPath = file.FullPath;
            fileLabel.Text = $"Файл: {xmlPath}";
            CreateAttributeControls(values);
            ClearSearch();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Не вдалося прочитати файл: {ex.Message}", "OK");
        }
    }

    private async void SearchButton_Clicked(object sender, EventArgs e)
    {
        if (xmlPath == null)
        {
            await DisplayAlert("Увага", "Спочатку відкрийте XML-файл.", "OK");
            return;
        }

        searcher.SetStrategy(GetSelectedStrategy());
        try
        {
            lastResults = searcher.Search(xmlPath, GetCriteria());
            resultsView.ItemsSource = lastResults;
            resultLabel.Text = $"Знайдено: {lastResults.Count}   (спосіб: {searcher.StrategyName})";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Помилка під час аналізу файлу: {ex.Message}", "OK");
        }
    }

    private async void TransformButton_Clicked(object sender, EventArgs e)
    {
        if (xmlPath == null)
        {
            await DisplayAlert("Увага", "Спочатку відкрийте XML-файл.", "OK");
            return;
        }

        string? xslPath = await GetXslPathAsync(xmlPath);
        if (xslPath == null)
            return;

        string htmlPath = Path.ChangeExtension(xmlPath, ".html");
        try
        {
            var transformer = new XslTransformer(xslPath);
            if (lastResults == null)
                transformer.Transform(xmlPath, htmlPath);
            else
                transformer.Transform(lastResults, htmlPath);

            string what = lastResults == null ? "весь файл" : "результати пошуку";
            bool open = await DisplayAlert("Готово",
                $"У HTML перетворено {what}.\nФайл: {htmlPath}\n\nВідкрити його?", "Так", "Ні");
            if (open)
                await Launcher.Default.OpenAsync(new OpenFileRequest("HTML", new ReadOnlyFile(htmlPath)));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Не вдалося виконати трансформацію: {ex.Message}", "OK");
        }
    }

    private void ClearButton_Clicked(object sender, EventArgs e)
    {
        ClearSearch();
    }

    private async void ExitButton_Clicked(object sender, EventArgs e)
    {
        await App.ConfirmExitAsync(this);
    }

    // XSL-файл з такою ж назвою, як XML, інакше користувач вибирає його сам
    private static async Task<string?> GetXslPathAsync(string xmlFile)
    {
        string xslPath = Path.ChangeExtension(xmlFile, ".xsl");
        if (File.Exists(xslPath))
            return xslPath;

        var file = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Виберіть XSL-файл",
            FileTypes = XslFileType
        });
        return file?.FullPath;
    }

    private IXmlSearchStrategy GetSelectedStrategy()
    {
        if (domRadio.IsChecked)
            return new DomSearchStrategy();
        if (linqRadio.IsChecked)
            return new LinqSearchStrategy();
        return new SaxSearchStrategy();
    }

    private SearchCriteria GetCriteria()
    {
        var criteria = new SearchCriteria { Keyword = keywordEntry.Text?.Trim() ?? "" };
        foreach (var pair in attributeControls)
        {
            if (pair.Value.Check.IsChecked && pair.Value.Picker.SelectedItem is string value)
                criteria.Attributes[pair.Key] = value;
        }
        return criteria;
    }

    // Для кожного атрибута з файлу: прапорець + список його значень
    private void CreateAttributeControls(Dictionary<string, List<string>> values)
    {
        attributesLayout.Children.Clear();
        attributeControls.Clear();

        foreach (var pair in values)
        {
            var check = new CheckBox { VerticalOptions = LayoutOptions.Center };
            var label = new Label { Text = Software.GetCaption(pair.Key), VerticalOptions = LayoutOptions.Center };
            var picker = new Picker { ItemsSource = pair.Value };
            picker.SelectedIndexChanged += (s, e) =>
            {
                if (picker.SelectedIndex >= 0)
                    check.IsChecked = true;
            };

            var row = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(new GridLength(90)),
                    new ColumnDefinition(GridLength.Star)
                },
                ColumnSpacing = 4
            };
            row.Add(check, 0, 0);
            row.Add(label, 1, 0);
            row.Add(picker, 2, 0);

            attributesLayout.Children.Add(row);
            attributeControls[pair.Key] = (check, picker);
        }
    }

    private void ClearSearch()
    {
        keywordEntry.Text = "";
        foreach (var (check, picker) in attributeControls.Values)
        {
            picker.SelectedIndex = -1;
            check.IsChecked = false;
        }
        lastResults = null;
        resultsView.ItemsSource = null;
        resultLabel.Text = "";
    }
}
