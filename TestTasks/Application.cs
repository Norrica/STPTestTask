using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestTasks
{
    public class Application : IExternalApplication
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

            var button = AddPushButton(ribbonPanel, "Трубы", $"TestTasks.PipeCommand", "Задание 1",PipeIcon);
            var button3 = AddPushButton(ribbonPanel, "Краны",  $"TestTasks.ValveCommand", "Задание 2", ValveIcon);
            var button2 = AddPushButton(ribbonPanel, "Тесты", $"TestTasks.PipeFind", "Всплывающая подсказка", PipeIcon);



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
