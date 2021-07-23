using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace informationRetriever
{
    [Transaction(TransactionMode.ReadOnly)]
    public class Inforetriever : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            /*Open the current running revit file*/
            var unidoc = commandData.Application.ActiveUIDocument;
            /*Find its document variable*/
            var doc = unidoc.Document;
            try
            {

                /*Option1: Select element by element id*/
                var wallFilter = new WallSelectionFilter();
                unidoc.Selection.SetElementIds(new List<ElementId> { new ElementId(157139) });



                /*Option2: Select element by manually selection in revit*/
                /*Pick the object by element*//*
                var reference = unidoc.Selection.PickObject(ObjectType.Element, wallFilter);
                *//*Get the element by its reference ID*//*
                var element = doc.GetElement(reference);
                *//*Select the wall element.*//*
                var wall = element as Wall;
                TaskDialog.Show("Wall", element.Name);*/
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }

            
            return Result.Succeeded;
        }
    }
    public class WallSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if(elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
