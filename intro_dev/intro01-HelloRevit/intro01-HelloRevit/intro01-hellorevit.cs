﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intro01_HelloRevit
{
    [Transaction(TransactionMode.Manual)]
    public class hellorevit : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskDialog.Show("01-intro", "Hello Revit!");
            return Result.Succeeded;
        }
    }
}
