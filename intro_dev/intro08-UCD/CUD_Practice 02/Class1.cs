using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CUD_Practice
{
    [Transaction(TransactionMode.Manual)]
    public class CUD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // define the current running revit document.
            var doc = commandData.Application.ActiveUIDocument.Document;

            // ui document
            var uidoc = commandData.Application.ActiveUIDocument;
            /*            var line = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 10, 0));*/

            // Find out the level where the wall will be placed at.
            var levelList = from element in new FilteredElementCollector(doc).OfClass(typeof(Level)) where element.Name == "Level 1" select element;
            var first_level = levelList.FirstOrDefault() as Level;

            // find out the door family and door location to be placed at.
            /*var door_Loc = line.Evaluate(0.5, true);*/
            var doorID = new ElementId(59772);
            var doorSymbol = doc.GetElement(doorID) as FamilySymbol;

            // if the door is not currently activated/used in revit, activate it.
            if (!doorSymbol.IsActive)
            {
                doorSymbol.Activate();
            }

            try
            {
                var line = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, 10, 0));
                var beamID = new ElementId(116116);
                var beamSymbol = doc.GetElement(beamID) as FamilySymbol;
                var transCreate = new Transaction(doc, "create a beam");

                transCreate.Start();
                if (!beamSymbol.IsActive)
                {
                    beamSymbol.Activate();
                }
                doc.Create.NewFamilyInstance(line, beamSymbol, first_level, Autodesk.Revit.DB.Structure.StructuralType.Beam);

                transCreate.Commit();
            }
            catch (Exception e)
            {
                TaskDialog.Show("error", e.Message);
                return Result.Failed;
            }


            return Result.Succeeded;
        }
    }
}

