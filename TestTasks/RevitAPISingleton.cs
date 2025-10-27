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
        public UIApplication uiapp = null;
        public UIDocument uidoc = null;
        public Autodesk.Revit.ApplicationServices.Application app = null;
        public Document doc = null;
     
        private RevitAPISingleton(ExternalCommandData commandData) {
            this.uiapp = commandData.Application;
            this.uidoc = uiapp.ActiveUIDocument;
            this.app = uiapp.Application;
            this.doc = uidoc.Document;
        }
        public static RevitAPISingleton getInstance(ExternalCommandData commandData)
        {
            if (Instance == null)
                Instance = new RevitAPISingleton(commandData);
            return Instance;
        }
    }
    
}
