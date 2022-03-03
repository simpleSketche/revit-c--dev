import clr
clr.AddReference("System.windows.Forms")
clr.AddReference("IronPython.wpf")
import wpf
from Autodesk.Revit.DB import *
from Autodesk.Revit.UI import *
from System.Collections.Generic import List
from pyrevit import UI
from pyrevit.compat import safe_strtype
from pyrevit import script
from System import Windows
import json
import subprocess
import os
import sys
import System


# Instantiate Revit Window Document
doc = __revit__.ActiveUIDocument.Document
uidoc = __revit__.ActiveUIDocument
xaml_file = script.get_bundle_file("ui.xaml")

# Addin Interface Design
class Popup_window(Windows.Window):
    def __init__(self):
        wpf.LoadComponent(self, xaml_file)
    
    def Option_Checked(self,sender, args):
        return True
    
    def Option_Unchecked(self, sender, args):
        return False

    def produce(self, items):
        return "The Data: {} are successfully exported to Google Sheet!".format([item.SelectedItem.Content.Text for item in items if item.SelectedValue is not None])

    @staticmethod
    def exterior_wall():
        """ Select Walls """
        walls = FilteredElementCollector(doc)\
        .OfCategory(BuiltInCategory.OST_Walls)\
        .WhereElementIsNotElementType()\
        .ToElements()

        wall_ids = []
        json_data = {}
        json_data["Exterior Walls"] = []
        existed_element = {}
        wall_linear_length = 0
        for idx in range(len(walls)):
            wall = walls[idx]
            wall_name = wall.Name
            if wall_name:
                ### action ###
                if "Exterior Wall" in wall_name:
                    wall_ids.append(wall.Id)
                    wall_length = wall.LookupParameter("Length").AsDouble()
                    if wall_name in existed_element.keys():
                        cur_idx = existed_element[wall_name]
                        json_data["Exterior Walls"][cur_idx]["Linear Length"] += wall_length
                    else:
                        json_data["Exterior Walls"].append(
                            {
                                "Name": wall_name,
                                "Linear Length": wall_length
                            }
                        )
                        existed_element[wall_name] = idx
                    wall_linear_length += wall_length
                ### action ###
        json_data["Exterior Walls"].append(
            {
                "Name": "Total Walls",
                "Linear Length": wall_linear_length
            }
        )
        return wall_ids, wall_linear_length, json_data

    @staticmethod
    def floor():
        """ Select Floors """
        floors = FilteredElementCollector(doc)\
        .OfCategory(BuiltInCategory.OST_Floors)\
        .WhereElementIsNotElementType()\
        .ToElements()

        floor_ids = []
        json_data = {}
        json_data["Floor"] = []
        existed_element = {}
        floor_sqft_area = 0
        for idx in range(len(floors)):
            floor = floors[idx]
            floor_name = floor.Name
            if floor_name:
                ### action ###
                floor_ids.append(floor.Id)
                floor_area_para = floor.LookupParameter("Area").AsDouble()
                if floor_name in existed_element.keys():
                    cur_idx = existed_element[floor_name]
                    json_data["Floor"][cur_idx]["Area"] += floor_area_para
                else:
                    json_data["Floor"].append(
                        {
                            "Name": floor_name,
                            "Area": floor_area_para
                        }
                    )
                    existed_element[floor_name] = idx
                floor_sqft_area += floor_area_para
                ### action ###
        json_data["Floor"].append(
            {
                "Name": "Total Floors",
                "Area": floor_sqft_area
            }
        )
        return floor_ids, floor_sqft_area, json_data

    @staticmethod
    def plumbing_fixtures():
        """ Select Plumbing Fixtures """
        plumbings = FilteredElementCollector(doc)\
        .OfCategory(BuiltInCategory.OST_GenericModel)\
        .WhereElementIsNotElementType()\
        .ToElements()

        plumbing_ids = []
        json_data = {}
        json_data["Plumbing Fixtures"] = []
        existed_element = {}
        num_plumbing = 0
        for idx in range(len(plumbings)):
            plumbing = plumbings[idx]
            plumbing_name = plumbing.Name
            if plumbing_name:
                ### action ###
                plumbing_ids.append(plumbing.Id)
                if "Plumbing Fixtures" in plumbing.Symbol.LookupParameter("Category").AsString():
                    if plumbing_name in existed_element.keys():
                        cur_idx = existed_element[plumbing_name]
                        json_data["Plumbing Fixtures"][cur_idx]["Count"] += 1
                    else:
                        json_data["Plumbing Fixtures"].append(
                            {
                                "Name": plumbing_name,
                                "Count": 1
                            }
                        )
                        existed_element[plumbing_name] = idx
                    num_plumbing += 1
                ### action ###
        json_data["Plumbing Fixtures"].append(
            {
                "Name": "Total Plumbing Fixtures",
                "Count": num_plumbing
            }
        )
        return plumbing_ids, num_plumbing, json_data
    
    @staticmethod
    def export_to_json(json_data):
        with open("E:\\Dropbox\\Work\\Revit Dev\\revit-c--dev\\adv_dev\ADU_sample\\Cottage.extension\\Cottage.tab\\Data Export.panel\\Export by selection.pushbutton\\test.json", "w") as out:
            json.dump(json_data, out)

    def result(self, sender, args):
        all_ids = []
        all_json_data = []
        output_data = {
            "title":None,
            "data":None
        }
        all_elements = [
            {
                "obj": self.ex_walls,
                "action": self.exterior_wall
            },
            {
                "obj": self.floors,
                "action": self.floor
            },
            {
                "obj": self.plumb_fixs,
                "action": self.plumbing_fixtures
            }
        ]
        UI.TaskDialog.Show(
            "Elements to Google Sheet",
            self.produce([element["obj"] for element in all_elements])
        )
        output_data["title"] = self.textbox.Text
        for elm in all_elements:
            if elm["obj"].SelectedValue is not None: 
                ids, data, json_data = elm["action"]()
                all_json_data.append(json_data)   
                all_ids.extend(ids)
        output_data["data"] = all_json_data
        self.export_to_json(output_data)
        path =  "E:\\Dropbox\Work\Revit Dev\\revit-c--dev\\adv_dev\ADU_sample\\Cottage.extension\\Cottage.tab\Data Export.panel\\Export by selection.pushbutton\\google_sheets_api.py"
        subprocess.call(["Python", path])
        uidoc.Selection.SetElementIds(List[ElementId](all_ids))

# Run!
if __name__ == "__main__":
    Popup_window().ShowDialog()
