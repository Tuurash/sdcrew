using sdcrew.Views.ViewHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace sdcrew.Services
{
    public interface INativeServices
    {
        void OnThemeChanged(ThemeManager.Theme theme);
    }
}
