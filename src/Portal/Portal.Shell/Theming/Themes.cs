using MudBlazor;

namespace YourBrand.Portal.Theming;

public static class Themes
{
    public static MudTheme AppTheme { get; } = new MudTheme()
    {
        Typography = new Typography()
        {
            Default = new DefaultTypography()
            {
                FontFamily = new[] { "Roboto", "sans-serif" }
            }
        },
        PaletteLight = new PaletteLight
        {
            Background = "rgb(248, 249, 250)",
            AppbarBackground = "#137cdf",
            Primary = "#4892d7"
            //Secondary = "#00000000"
        }
    };

    public static MudTheme AppTheme2 { get; } = new MudTheme()
    {
        Typography = new Typography()
        {
            Default = new DefaultTypography()
            {
                FontFamily = new[] { "Roboto", "sans-serif" }
            }
        },
        PaletteLight = new PaletteLight
        {
            Background = "#22FFDD",
            AppbarBackground = "#FF44BB",
            Primary = "#4892d7"
            //Secondary = "#00000000"
        }
    };
}