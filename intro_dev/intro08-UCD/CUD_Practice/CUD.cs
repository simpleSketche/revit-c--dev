using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUD_Practice
{
    [Transaction(TransactionMode.Manual)]
    public class CUD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var transaction = new Transaction(doc);
            transaction.Start("Create a wall.");

            // create wall

            transaction.Commit();

            return Result.Succeeded;
        }
    }
}
