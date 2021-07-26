using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace getGeo
{
    public static class getGeo
    {
        /// <summary>
        /// extract element geometrical objets
        /// </summary>
        /// <param name="element">selected elements</param>
        /// <param name="options">selection options</param>
        /// <returns></returns>
        public static List<GeometryObject> getGeometryobjs(this Element element, Options options = default(Options))
        {
            var results = new List<GeometryObject>();
            /*?? means if it is empty it is the latter one, it is the former one if it is not empty*/
            options = options ?? new Options();
            /*Find the solid of the element.*/
            var geometry = element.get_Geometry(options);
            recurseObj(geometry, ref results);
            return results;

        }

        private static void recurseObj(this GeometryElement geometry, ref List<GeometryObject> geometryObjects)
        {
            if (geometry == null)
            {
                return;
            }
            var enumerator = geometry.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                switch (current)
                {
                    /*Only add solid or all other but instance or element will be added to the geometroObjects list
                     Kinda unnessary to use recursive, isn't it? IF statement will just do perhaps. Take this as a recursive practice.*/
                    case GeometryInstance instance:
                        instance.SymbolGeometry.recurseObj(ref geometryObjects);
                        break;
                    case GeometryElement element:
                        element.recurseObj(ref geometryObjects);
                        break;
                    case Solid solid:
                        if (solid.Faces.Size == 0 || solid.Edges.Size == 0)
                        {
                            continue;
                        }
                        geometryObjects.Add(solid);
                        break;
                    default:
                        geometryObjects.Add(current);
                        break;
                }

            }
        }
    }
}

