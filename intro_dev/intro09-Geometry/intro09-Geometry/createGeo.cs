using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace intro09_Geometry
{
    [Transaction(TransactionMode.Manual)]
    public class createGeo : IExternalCommand
    {
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var tol = commandData.Application.Application.ShortCurveTolerance;
            var doc = commandData.Application.ActiveUIDocument.Document;

            var pt1 = new XYZ(0, 0, 0);
            var pt2 = new XYZ(5, 0, 0);
            var pt3 = new XYZ(5, 8, 0);
            var pt4 = new XYZ(0, 8, 0);

            var l1 = Line.CreateBound(pt1, pt2);
            var l2 = Line.CreateBound(pt2, pt3);
            var l3 = Line.CreateBound(pt3, pt4);
            var l4 = Line.CreateBound(pt4, pt1);

            var crvs = new CurveLoop();

            crvs.Append(l1);
            crvs.Append(l2);
            crvs.Append(l3);
            crvs.Append(l4);
            
            var trans = Transform.CreateTranslation(new XYZ(5, 5, 0));
            crvs.Transform(trans);

            var solid = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop> { crvs }, XYZ.BasisZ, 10);
            var transaction = new Transaction(doc, "createGeo");
            transaction.Start();

            var shape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
            shape.SetShape(new GeometryObject[] { solid });

            transaction.Commit();

            return Result.Succeeded;
        }
    }
}
