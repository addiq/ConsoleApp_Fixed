namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataReader
    {
        IEnumerable<ImportedObject> ImportedObjects;

        public void ImportAndPrintData(string fileToImport, bool printData = true)
        {
            ImportedObjects = new List<ImportedObject>() { new ImportedObject() };

            var streamReader = new StreamReader(fileToImport);

            var importedLines = new List<string>();
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                if (!String.IsNullOrEmpty(line)) //check if line is null or empty, IF yes, than exit if
                { 
                    importedLines.Add(line);
                }
            }
            for (int i = 0; i <= importedLines.Count-1; i++) // -1 added
            {
                var importedLine = importedLines[i];
                var values = importedLine.Split(';');
                var importedObject = new ImportedObject();
                importedObject.Type = values[0];
                importedObject.Name = values[1];
                importedObject.Schema = values[2];
                importedObject.ParentName = values[3];
                importedObject.ParentType = values[4];
                importedObject.DataType = values[5];
                importedObject.IsNullable = values[6];
                ((List<ImportedObject>)ImportedObjects).Add(importedObject);
            }
                
            

            // clear and correct imported data
            foreach (var importedObject in ImportedObjects)
            {
                if (importedObject.Type != null) //check if line of imported objects is NOT empty
                {
                    importedObject.Type = importedObject.Type.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                    importedObject.Name = importedObject.Name.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    importedObject.Schema = importedObject.Schema.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    importedObject.ParentName = importedObject.ParentName.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    importedObject.ParentType = importedObject.ParentType.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                }

            }

            // assign number of children
            for (int i = 0; i < ImportedObjects.Count(); i++)
            {
                var importedObject = ImportedObjects.ToArray()[i];
                foreach (var impObj in ImportedObjects)
                {
                    if (impObj.ParentType == importedObject.Type)
                    {
                        if (impObj.ParentName == importedObject.Name)
                        {
                            importedObject.NumberOfChildren = 1 + importedObject.NumberOfChildren;
                        }
                    }
                }
            }

            foreach (var database in ImportedObjects)
            {
                if (database.Type == "DATABASE")
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                    // print all database's tables
                    foreach (var table in ImportedObjects)
                    {
                        if (!string.IsNullOrEmpty(table.ParentType))
                        {
                            if (table.ParentType.ToUpper() == database.Type)
                            {
                                if (table.ParentName == database.Name)
                                {
                                    Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                                    // print all table's columns
                                    foreach (var column in ImportedObjects)
                                    {
                                        if (!string.IsNullOrEmpty(column.ParentType))
                                        {
                                            if (column.ParentType.ToUpper() == table.Type)
                                            {
                                                if (column.ParentName == table.Name)
                                                {
                                                    Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Console.ReadLine();

        }
    }

    class ImportedObject : ImportedObjectBaseClass
    {
        public string _Name
        {
            get; // { return name; }
            set; // { name = Name; }
        }
        public string Schema
        {
            get; // { return schema; }
            set; // { schema = Schema; }
        }
        public string ParentName
        {
            get; // { return parentName; }
            set; // { parentName = ParentName; }
        }

        public string ParentType
        {
            get; // { return parentType; }
            set; // { parentType = ParentType; }
        }

        public string DataType 
        {
            get; // { return dataType; }
            set; // { dataType = DataType; }
        }
        public string IsNullable
        {
            get;// { return isNullable; }  
            set;// { isNullable = IsNullable; }
        }


        public double NumberOfChildren;
        
        //private string name;
        //private string schema;
        //private string parentName;
        //private string parentType;
        //private string dataType;
        //private string isNullable;
    }

    class ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type;
    }
}
