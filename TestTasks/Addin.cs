using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestTasks
{
    public class Addin : IExternalApplication
    {
        private readonly string assemblyPath = Assembly.GetExecutingAssembly().Location;
        private readonly Uri PipeIcon = new Uri(@"/TestTasks;component/Resources/pipe.ico", UriKind.RelativeOrAbsolute);
        private readonly Uri ValveIcon = new Uri(@"/TestTasks;component/Resources/valve.ico", UriKind.RelativeOrAbsolute);

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "Тестовое задание";
            application.CreateRibbonTab(tabName);

            RibbonPanel ribbonPanel = application.CreateRibbonPanel(tabName, "Решения");
            
            AddPushButton(ribbonPanel, "1)Трубы", $"TestTasks.PipeCommand", "Задание 1", PipeIcon);
            AddPushButton(ribbonPanel, "2)Краны",  $"TestTasks.ValveCommand", "Задание 2", ValveIcon);
            AddPushButton(ribbonPanel, "Тест",  $"TestTasks.IdPicker", "Тест", ValveIcon);



            return Result.Succeeded;
        }
        private PushButton AddPushButton(RibbonPanel ribbonPanel, string buttonName, string linkToCommand, string toolTip,Uri icon)
        {
            var buttonData = new PushButtonData(buttonName, buttonName, assemblyPath, linkToCommand);
            var button = ribbonPanel.AddItem(buttonData) as PushButton;
            button.ToolTip = toolTip;
            button.LargeImage = (ImageSource)new BitmapImage(icon);

            return button;
        }
    }
}
