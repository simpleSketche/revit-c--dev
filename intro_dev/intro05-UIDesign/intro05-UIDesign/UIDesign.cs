using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intro05_UIDesign
{
    [Transaction(TransactionMode.Manual)]
    public class UIDesign : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Welcome login = new Welcome();
            login.ShowDialog();
            
            return Result.Succeeded;
        }
    }
}
