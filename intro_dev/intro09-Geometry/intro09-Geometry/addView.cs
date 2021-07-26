using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getGeo;

namespace intro09_Geometry
{
    [Transaction(TransactionMode.Manual)]
    class addView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            try
            {
                // add floor plan view
                var transaction = new Transaction(doc, "Create View");
                transaction.Start();

                var level = Level.Create(doc, 1);
                var viewType = from elem in new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType))
                               let type = elem as ViewFamilyType
                               where type.ViewFamily == ViewFamily.FloorPlan
                               select type;
                var viewId = viewType?.FirstOrDefault()?.Id;
                if (viewId == null)
                {
                    throw new Exception("No floor plan is found.");
                }
                var viewPlan = ViewPlan.Create(doc, viewId, level.Id);
                

                // add text type
                TextNoteType newTextType;
                var typeName = "1/4\" Arial";
                var textTypeList = from elem in new FilteredElementCollector(doc).OfClass(typeof(TextNoteType))
                                   let type = elem as TextNoteType
                                   where type.Name == typeName && type.FamilyName == "TextNote"
                                   select type;
                if (textTypeList.Count() > 0)
                {
                    newTextType = textTypeList.FirstOrDefault();
                }
                else
                {
                    textTypeList = from elem in new FilteredElementCollector(doc).OfClass(typeof(TextNoteType))
                                       let type = elem as TextNoteType
                                       where type.FamilyName == "TextNote"
                                       select type;
                    var textType = textTypeList.FirstOrDefault();
                    newTextType = textType.Duplicate(typeName) as TextNoteType;
                    newTextType.get_Parameter(BuiltInParameter.TEXT_SIZE).Set(1 / 4);
                    newTextType.get_Parameter(BuiltInParameter.TEXT_FONT).Set("Arial");
                    newTextType.get_Parameter(BuiltInParameter.TEXT_BACKGROUND).Set(1);
                }

                // create text
                var option = new TextNoteOptions();
                option.HorizontalAlignment = HorizontalTextAlignment.Center;
                option.TypeId = newTextType.Id;
                var textNote = TextNote.Create(doc, viewPlan.Id, new XYZ(0, 0, 0), viewPlan.Name, option);

                // applied to view template
                var viewTemplateList = from elm in new FilteredElementCollector(doc).OfClass(typeof(ViewPlan))
                                   let view = elm as ViewPlan
                                   where view.IsTemplate && view.Name == "Architectural Plan"
                                   select view;
                var viewTemplate = viewTemplateList?.FirstOrDefault();
                if(viewTemplate == null)
                {
                    throw new Exception("No template found.");
                }
                viewPlan.ViewTemplateId = viewTemplate.Id;

                // annotations
                var dimTypeList = from elem in new FilteredElementCollector(doc).OfClass(typeof(DimensionType))
                                  let type = elem as DimensionType
                                  where type.Name == "Feet & Inches"
                                  select type;
                var targetDimType = dimTypeList?.FirstOrDefault();
                var wall = doc.GetElement(new ElementId(196219)) as Wall;
                var wallLoc = (wall.Location as LocationCurve).Curve;
                var wallDir = (wallLoc as Line).Direction;
                var opt = new Options();
                opt.ComputeReferences = true;
                var wallSolid = wall.getGeometryobjs(opt).FirstOrDefault() as Solid;
                var references = new ReferenceArray();
                foreach(Face face in wallSolid.Faces)
                {
                    if(face is PlanarFace pface && pface.FaceNormal.CrossProduct(wallDir).IsAlmostEqualTo(XYZ.Zero))
                    {
                        references.Append(face.Reference);
                    }
                }
                var offset = 5;
                var line = Line.CreateBound(wallLoc.GetEndPoint(0) + XYZ.BasisY * offset, wallLoc.GetEndPoint(1) + XYZ.BasisY * offset);
                var dim = doc.Create.NewDimension(viewPlan, line, references);
                dim.DimensionType = targetDimType;

                //create sheet
                var sheet = ViewSheet.Create(doc, new ElementId(266823));

                // add view to sheet
                if(Viewport.CanAddViewToSheet(doc, sheet.Id, viewPlan.Id))
                {
                    var sheetSize = new XYZ(36 / 2, 24 / 2, 0);
                    var viewport = Viewport.Create(doc, sheet.Id, viewPlan.Id, sheetSize);
                }
                transaction.Commit();

                return Result.Succeeded;

            }
            catch(Exception e)
            {
                TaskDialog.Show("Failed!", "Failed!");
                return Result.Failed;
            }
        }
    }
}
