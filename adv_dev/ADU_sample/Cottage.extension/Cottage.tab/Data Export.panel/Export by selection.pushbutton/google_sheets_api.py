import json
import os
from google_create_service import Create_Service

SCOPES = ['https://www.googleapis.com/auth/spreadsheets']

client_secret = "E:\\Dropbox\\Work\\Revit Dev\\revit-c--dev\\adv_dev\\ADU_sample\\Cottage.extension\\Cottage.tab\\Data Export.panel\\Export by selection.pushbutton\\client_secret.json"
api_name = "sheets"
api_version = "v4"

def create_sub_sheet(elements):
    result = []
    for element in elements:
        cur_sheet = {
            "properties":{
                "title": element
            }
        }
        result.append(cur_sheet)
    return result


def write_data(data, service, spreadsheetid):
    for element in data:
        cell_range_start = "D8"
        work_sheet_name = list(element.keys())[0]
        sub_keys = list(element[work_sheet_name][0].keys())

        all_rows = [sub_keys]
        for item in element[work_sheet_name]:
            cur_row = []
            for key in sub_keys:
                cur_row.append(item[key])
            all_rows.append(cur_row)
        values = (
            all_rows
        )
        value_range_body = {
            "majorDimension": "ROWS",
            "values": values
        }
        service.spreadsheets().values().update(
            spreadsheetId = spreadsheetid,
            valueInputOption = "USER_ENTERED",
            range = work_sheet_name+"!" + cell_range_start,
            body = value_range_body 
        ).execute()



def create_sheet_body():

    with open("E:\\Dropbox\\Work\\Revit Dev\\revit-c--dev\\adv_dev\ADU_sample\\Cottage.extension\\Cottage.tab\\Data Export.panel\\Export by selection.pushbutton\\test.json") as json_file:
        data_file = json.load(json_file)
    with open("E:\\Dropbox\\Work\\Revit Dev\\revit-c--dev\\adv_dev\ADU_sample\\Cottage.extension\\Cottage.tab\Data Export.panel\\Export by selection.pushbutton\\gsheet_management.json") as management_json_file:
        management = json.load(management_json_file)

    gsheet_files = management["data"]
    title = data_file["title"]
    titles = list(gsheet_files.keys())

    data = data_file["data"]
    elements = [list(elem.keys())[0] for elem in data]
    service = Create_Service(client_secret, api_name, api_version, SCOPES)

    
    if title not in titles:  
        
        sheet_body = {
            "properties":{
                "title": title,
                "locale": "en_US",
                "timeZone": "America/New_York",
                "autoRecalc": "HOUR"
            },
            "sheets":create_sub_sheet(elements)
        }

        sheets_body = service.spreadsheets().create(
        body = sheet_body
        ).execute()
        gsheet_files[title] = {}
        gsheet_files[title]["sheetId"] = sheets_body["spreadsheetId"]
        spreadsheetid = sheets_body["spreadsheetId"]

        with open("E:\\Dropbox\\Work\\Revit Dev\\revit-c--dev\\adv_dev\\ADU_sample\\Cottage.extension\\Cottage.tab\\Data Export.panel\\Export by selection.pushbutton\\gsheet_management.json", "w") as management_output_file:
            json.dump(management, management_output_file)
    else:
        spreadsheetid = gsheet_files[title]["sheetId"]
    
    write_data(data, service, spreadsheetid)


if __name__ == "__main__":
    create_sheet_body()


