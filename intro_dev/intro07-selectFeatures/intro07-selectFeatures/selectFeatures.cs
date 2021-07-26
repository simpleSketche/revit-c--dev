using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using getGeo;

namespace intro07_selectFeatures
{
    [Transaction(TransactionMode.Manual)]
    public class selectFeatures : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var unidoc = commandData.Application.ActiveUIDocument;
            var doc = unidoc.Document;
            var collector = new FilteredElementCollector(doc).OfClass(typeof(SpatialElement)).ToElements();
            var collector2 = new FilteredElementCollector(doc);
            var roomDataInfo = new List<List<string>>();

            ///extract all the wall family instances.
            collector2.OfCategory(BuiltInCategory.OST_Walls).OfClass(typeof(FamilyInstance)).ToElements();
            var wallElements = new List<List<GeometryObject>>();
            MessageBox.Show("heloooooo!");
            MessageBox.Show($"number of elements {collector2.Count()}");
            foreach (Element elm in collector2)
            {
                var wall = elm as FamilyInstance;
                var geoObjs = wall.getGeometryobjs();
                var val = geoObjs[0];
                MessageBox.Show($"Wall data {val}");
                wallElements.Add(geoObjs);
            }


            foreach (Room item in collector)
            {
                var name = item.Name;
                var area = item.Area;
                var level = item.Level.Name;
                var param = item.get_Parameter(BuiltInParameter.ROOM_HEIGHT);
                var roomHeight = param.AsDouble().ToString();
                var roomInfo = new List<string>
                {
                    name, area.ToString(), level, roomHeight
                };
                roomDataInfo.Add(roomInfo);
            }



/*            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("Room Info");
            var headers = new string[] { "Room Info", "Room Area", "Room Height", "Room Level" };
            var row0 = sheet.CreateRow(0);
            for (int i = 0; i < headers.Count(); i++)
            {
                var cell = row0.CreateCell(i);
                cell.SetCellValue(headers[i]);

            }
            for (int j = 0; j < roomDataInfo.Count; j++)
            {
                var row = sheet.CreateRow(j + 1);
                for (int z = 0; z < roomDataInfo[j].Count(); z++)
                {
                    var cell = row.CreateCell(z);
                    cell.SetCellValue(roomDataInfo[j][z]);
                }
            }

            SaveFileDialog fileDia = new SaveFileDialog();
            fileDia.Filter = "(EXCElfile)|*.xls";
            fileDia.FileName = "Room Info";
            bool isFileSave = false;
            fileDia.FileOk += (s, e) => { isFileSave = true; };
            fileDia.ShowDialog();
            if (isFileSave)
            {
                var path = fileDia.FileName;
                using(var fs = File.OpenWrite(path))
                {
                    workbook.Write(fs);
                    MessageBox.Show($"File saved successfully to {fileDia.FileName}!");
                }
            }*/
            return Result.Succeeded;
        }
    }
}
