namespace MyXmlMAUIApp;

public partial class App : Application
{
    // true після підтвердження, щоб повторно не питати при закритті вікна
    public static bool IsExiting { get; private set; }

    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell()) { Title = "Лабораторна робота 2 - XML" };
    }

    public static async Task ConfirmExitAsync(Page page)
    {
        bool answer = await page.DisplayAlert("Вихід",
            "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
        if (!answer)
            return;

        IsExiting = true;
        Current?.Quit();
    }
}
