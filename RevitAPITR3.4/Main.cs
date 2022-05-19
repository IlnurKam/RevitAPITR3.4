using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITR3._4
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var categorySet = new CategorySet();
            categorySet.Insert(categorySet.GetCategory(doc, BuiltInCategory.OST_Pipes));
            

            using (Transaction ts = new Transaction(doc, "Add parameter"))
            {
                ts.Start();
                CreateSharedParameter(uiapp.Application, doc, "Труба НАРУЖНЫЙ_ДИАМЕТР / ВНУТРЕННИЙ_ДИАМЕТР", categorySet, BuiltInParameterGroup.PG_DATA, true);
                ts.Commit();
            }

            return Result.Succeeded;
        }
        private void CreateSharedParameter(ApplicationException application,
            Document doc, string parameterName, CategorySet category,
            BuiltInParameterGroup builtInParameterGroup, bool isInstance)
        {
            DefinitionFile definitionFile = application.OpenSharedParameterFile();
            if (definitionFile == null)
            {
                TaskDialog.Show("Oshibka", "Ne naiden fail obchih parametrov");
                return;
            }

            Definition definition = definitionFile.Groups.SelectMany(group => group.Definition).FirstOrDefault(def => def.Name.Equals(parameterName));
            if (definition==null)
            {
                TaskDialog.Show("Oshibka", "Ne naiden parametr");
                return;
            }
            Binding binding = application.Create.NewTypeBinding(categorySet);
            if (isInstance)
                binding = application.Create.NewInstanceBinding(categorySet);

            BindingMap map = doc.ParameterBindings;
            map.Insert(definition, binding, builtInParameterGroup);

        }
    }
}



