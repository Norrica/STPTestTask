using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace TestTasks
{
    internal class RevitAPISingleton
    {
        private static RevitAPISingleton Instance { get; set; } = null;
        public Application App { get => App1; private set => App1 = value; }
        public UIApplication Uiapp { get => uiapp; private set => uiapp = value; }
        public UIDocument Uidoc { get => uidoc; private set => uidoc = value; }
        public Application App1 { get => app; private set => app = value; }
        public Document Doc { get => doc; private set => doc = value; }

        private UIApplication uiapp = null;
        private UIDocument uidoc = null;
        private Application app = null;
        private Document doc = null;

        private RevitAPISingleton(ExternalCommandData commandData) {
            Uiapp = commandData.Application;
            Uidoc = Uiapp.ActiveUIDocument;
            App = Uiapp.Application;
            Doc = Uidoc.Document;
        }
        public static RevitAPISingleton getInstance(ExternalCommandData commandData = null)
        {
            if (Instance == null)
                Instance = new RevitAPISingleton(commandData);
            return Instance;
        }
    }
    
}
