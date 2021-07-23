using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterInfo
{
    [Transaction(TransactionMode.ReadOnly)]
    public class filterinfo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            /*Current running revit UI document*/
            var doc = commandData.Application.ActiveUIDocument.Document;
            var collector = new FilteredElementCollector(doc);

            /*option1, faster*/
/*            collector.OfCategory(BuiltInCategory.OST_Doors).OfClass(typeof(FamilyInstance));*/


            /*option2*/
            var familyInstanceFilter = new ElementClassFilter(typeof(FamilyInstance));
            var categoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            var doorInstanceFilter = new LogicalAndFilter(familyInstanceFilter, categoryFilter);
            collector.WherePasses(doorInstanceFilter);

/*            option1 - select all the elements*/
              var elementList = new List<Element>();
            foreach (Element item in collector)
            {
                elementList.Add(item);
            }

            /*option2-select all the elements*/
            /*var selectedDoors = from elem in collector where elem.Name == "Entrance door" select elem;*/

            TaskDialog.Show("result", $"The number doors in the door family called Entrance door is {elementList.Count}");
            return Result.Succeeded;
        }
    }
}
