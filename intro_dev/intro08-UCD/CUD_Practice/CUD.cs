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
                var numWalls = 30;
                var r1 = 10;
                var r2 = 25;
                var rotation = 2 * Math.PI / numWalls;

                var transCreate = new Transaction(doc, "create a wall");
                transCreate.Start();

                for(int i = 0; i < numWalls; i++)
                {
                    var line = Line.CreateBound(new XYZ(r1 * Math.Cos(rotation * i), r1 * Math.Sin(rotation * i), 0),
                        new XYZ(r2*Math.Cos(rotation*i), r2*Math.Sin(rotation*i), 0));
                    var door_Loc = line.Evaluate(0.5, true);
                    var wall = Wall.Create(doc, line, first_level.Id, false);
                    // add the door instance into the new created wall using the method below.
                    doc.Create.NewFamilyInstance(door_Loc, doorSymbol, wall, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                    
                    // refresh revit to show each individual change/transaction made to the running document.
                    doc.Regenerate();
                    uidoc.RefreshActiveView();
                    Thread.Sleep(100);
                }
               
                transCreate.Commit();
            }
            catch (Exception e)
            {
                TaskDialog.Show("error", e.Message);
            }
            

            return Result.Succeeded;
        }
    }
}
