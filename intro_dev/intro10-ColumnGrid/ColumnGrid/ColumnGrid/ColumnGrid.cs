using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace ColumnGrid
{
    [Transaction(TransactionMode.Manual)]
    public class ColumnGrid : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the user input values (number of columns, distance between columns)
            UserControl1 userinput = new UserControl1();
            userinput.ShowDialog();
            var inputNumCol = Convert.ToInt32(userinput.userInput.Text);
            var inputSpan = float.Parse(userinput.userInput2.Text);

            // initialize the current active revit window, start
            var doc = commandData.Application.ActiveUIDocument.Document;
            var transaction = new Transaction(doc, "grid");
            var collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_Columns);
            var columnSymbol = collector.FirstOrDefault() as FamilySymbol;
            var level = from element in new FilteredElementCollector(doc).OfClass(typeof(Level))
                        where element.Name == "Level 1"
                        select element;
            var first_level = level.FirstOrDefault() as Level;

            Guid schemaGuid = new Guid("3B4C586C-CC3A-47A7-8333-3E02547F95F2");


            transaction.Start();


            if (!columnSymbol.IsActive)
            {
                columnSymbol.Activate();
            }

            // Establish the grid first
            // initialize the h and v span distance
            float hSpan = inputSpan;
            float vSpan = inputSpan;
            int hCount = inputNumCol;
            int vCount = inputNumCol;


            var crvs = new CurveLoop();
            List<string> letters = countLetters(vCount);
            List<Grid> grids1 = new List<Grid>();
            List<Curve> crvs1 = new List<Curve>();
            List<FamilyInstance> columns = new List<FamilyInstance>();

            // horizontal grid lines
            for (int i = 1; i < hCount; i++)
            {
                var hPt1 = new XYZ(i * hSpan, 0, 0);
                var hPt2 = new XYZ(i * hSpan, vCount * hSpan, 0);
                var curLine = Line.CreateBound(hPt1, hPt2);
                var curGrid = Grid.Create(doc, curLine);
                var gridName = curGrid.get_Parameter(BuiltInParameter.DATUM_TEXT);
                gridName.Set(i.ToString());
                grids1.Add(curGrid);
                crvs1.Add(curLine);
            }

            // vertical grid lines
            for (int j = 1; j < vCount; j++)
            {
                var vPt1 = new XYZ(0, j * vSpan, 0);
                var vPt2 = new XYZ(hCount * vSpan, j * vSpan, 0);
                var curLine2 = Line.CreateBound(vPt1, vPt2);
                var curGrid2 = Grid.Create(doc, curLine2);
                var gridName2 = curGrid2.get_Parameter(BuiltInParameter.DATUM_TEXT);
                gridName2.Set(letters[j - 1]);
                
                for(int z=0; z < grids1.Count; z++)
                {
                    var intersect = curLine2.Intersect(crvs1[z]);
                    if (intersect == SetComparisonResult.Overlap)
                    {
                        XYZ intpt = intersectPt(curLine2, crvs1[z]);
                        var col = doc.Create.NewFamilyInstance(intpt, columnSymbol, first_level, Autodesk.Revit.DB.Structure.StructuralType.Column);
                        var schema = Schema.Lookup(schemaGuid);
                        if (schema == null)
                        {
                            schema = schemaBuilder(schemaGuid);
                        }
                        Entity entity = new Entity(schema);
                        var name = schema.GetField("Column_Name");
                        entity.Set(name, grids1[z].Name + curGrid2.Name);
                        col.SetEntity(entity);
                        
                        var dataStorageLst = from element in new FilteredElementCollector(doc).OfClass(typeof(DataStorage))
                                             let storage = element as DataStorage
                                             where storage.GetEntitySchemaGuids().Contains(schemaGuid)
                                             select storage;
                        var datastorage = dataStorageLst.FirstOrDefault();
                        if(datastorage == null)
                        {
                            datastorage = DataStorage.Create(doc);
                        }
                        datastorage.SetEntity(entity);

                        columns.Add(col);
                    }
                }
            }
            transaction.Commit();

            return Result.Succeeded;
        }
        // Get the all required letters
        public List<string> countLetters(int count)
        {
            int goalCount = count;
            int letterCount = 26;

            List<string> allLetters = new List<string> { "A", "B", "C", "D", "E", "F",
            "G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X",
            "Y","Z"};

            string header = "";
            List<string> headers = new List<string>();
            float loopCount = count / 26;
            int outerLoopCount = Convert.ToInt32(Floor(loopCount) + 1);

            for (int i = 0; i < outerLoopCount; i++)
            {
                header = defaultLetters(header);

                for(int j = 0; j < letterCount; j++)
                {
                    var headerTx = header.ToCharArray();
                    headerTx[i] = allLetters[j].ToCharArray()[0];
                    Console.WriteLine(headerTx);
                    string newHeader = new string(headerTx);
                    headers.Add(newHeader);
                    if (goalCount == 0) break;
                    goalCount -= 1;
                }
            }
            return headers;
        }
        // dynamically increase one slot for every 26 letters consumed.
        // eg. A -> AA -> AAA...
        public string defaultLetters(string header)
        {
            var newheader = "";

            for(int i = 0; i < header.Count()+1; i++)
            {
                newheader += "A";
            }
            return newheader;
        }

        public XYZ intersectPt(Curve crv1, Curve crv2)
        {
            XYZ crv1p1 = crv1.GetEndPoint(0);
            XYZ crv1p2 = crv1.GetEndPoint(1);
            XYZ crv2p1 = crv2.GetEndPoint(0);
            XYZ crv2p2 = crv2.GetEndPoint(1);

            XYZ v1 = crv1p2 - crv1p1;
            XYZ v2 = crv2p2 - crv2p1;
            XYZ w = crv2p1 - crv1p1;

            double c = (v2.X * w.Y - v2.Y * w.X)
              / (v2.X * v1.Y - v2.Y * v1.X);

            double x = crv1p1.X + c * v1.X;
            double y = crv1p1.Y + c * v1.Y;

            XYZ result = new XYZ(x, y, 0);

            return result;
        }

        public Schema schemaBuilder(Guid schemaGuid)
        {
            
            var schemaBlder = new SchemaBuilder(schemaGuid);
            schemaBlder.SetReadAccessLevel(AccessLevel.Public);
            schemaBlder.SetWriteAccessLevel(AccessLevel.Public);
            schemaBlder.SetSchemaName("Column_Name");
            schemaBlder.SetDocumentation("A unique name of the column which represents its location in plan.");
            var fieldBlder = schemaBlder.AddSimpleField("Column_Name", typeof(string));
            var schema = schemaBlder.Finish();

            return schema;
        }

    }
}