using AutomotiveBuilder.DAL;
using AutomotiveBuilder.Static;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace AutomotiveBuilder.Statics
{
    public static class StaticMethods
    {
        /// TODO: Implement parametric deleting to remove all references in relational data models when a model item is deleted.
        /// TODO: Add a function to clean orphaned model items.
        /// 
        /// TODO: In each model in the DAL where we save data to a file, add a backup function that creates a backup of the file with a timestamp in the name before saving. 
        /// If an error occurs during saving, catch the exception and restore the backup file to prevent data loss.       

        #region Private Members
        private static PartList _ProjPartList;
        private static bool _PartsChanged = true;
        private static BindingList<Part> _parts;
        private static VenderList _venders;
        private static bool _vendersChanged = false;
        private static BindingList<PartCatagory> _partCatagories;
        #endregion

        #region Properties
        public static string ProjectFolder = @"C:\Temp\AutoProjects\";
        public static string MakeModelFolder = @"C:\Temp\AutoProjects\MakeModels\";
        public static string PartsFolder = @"C:\Temp\AutoProjects\Parts\";
        public static string BOMfolder = @"C:\Temp\AutoProjects\BOM\";
        public static BindingList<Part> Parts
        {
            get { return _parts; }
        }
        public static VenderList Venders
        {
            get
            {
                if ((_venders == null) || _vendersChanged)
                {
                    _venders = new VenderList();
                    _vendersChanged = false;
                }
                return _venders;
            }
        }
        public static bool VendersChanged
        {
            get { return _vendersChanged; }
            set { _vendersChanged = value; }
        }
        public static PartList ProjPartList
        {
            get
            {
                if ((_ProjPartList == null) || (_PartsChanged)) _ProjPartList = new PartList("");
                return _ProjPartList;
            }
        }
        public static bool PartsChanged
        {
            get { return _PartsChanged; }
            set
            {
                _PartsChanged = value; 
                if (_PartsChanged) LoadParts();
            }
        }
        public static BindingList<PartCatagory> PartCatagories
        {
            get
            {
                if ((_partCatagories == null) || (_partCatagories.Count == 0))
                {
                    _partCatagories = LoadPartCatagories();
                }
                _partCatagories = new BindingList<PartCatagory>(_partCatagories.OrderBy(x => x.Name).ToList());
                return _partCatagories;
            }
        }
        #endregion

        #region Methods        
        public static void CreateFolders()
        {
            string strFileName = @"C:\Temp";
            if (!Directory.Exists(strFileName)) Directory.CreateDirectory(strFileName);
            strFileName = strFileName + @"\AutoProjects";
            if (!Directory.Exists(strFileName)) Directory.CreateDirectory(strFileName);
            strFileName = strFileName + @"\MakeModels";
            if (!Directory.Exists(strFileName)) Directory.CreateDirectory(strFileName);
            strFileName = @"C:\Temp";
            strFileName = strFileName + @"\AutoProjects";
            strFileName = strFileName + @"\Parts";
            if (!Directory.Exists(strFileName)) Directory.CreateDirectory(strFileName);
            strFileName = @"C:\Temp";
            strFileName = strFileName + @"\AutoProjects";
            strFileName = strFileName + @"\BOM";
            if (!Directory.Exists(strFileName)) Directory.CreateDirectory(strFileName);
            LoadParts();
        }
        public static void LoadParts()
        {
            _parts = new BindingList<Part>();
            foreach(var pt in ProjPartList)
            {
                _parts.Add(pt);
            }
        }

        public static BindingList<PartCatagory> LoadPartCatagories(string FileName = null)
        {
            BindingList<PartCatagory> partCatagories = new BindingList<PartCatagory>();
            BindingList<PartSubCatagory> partSubCatagories = new BindingList<PartSubCatagory>();
            string subcatagoryFileName;

            try
            {
                if ((FileName != null) && (File.Exists(FileName)))
                {
                    using (Stream reader = new FileStream(FileName, FileMode.Open))
                    {
                        var serializer = new XmlSerializer(typeof(BindingList<PartCatagory>));
                        // Call the Deserialize method to restore the object's state.
                        partCatagories = (BindingList<PartCatagory>)serializer.Deserialize(reader);
                    }
                }
                else
                {
                    FileName = PartsFolder + "PartCatagories.xml";
                    if(File.Exists(FileName))
                    {
                        using (Stream reader = new FileStream(FileName, FileMode.Open))
                        {
                            var serializer = new XmlSerializer(typeof(BindingList<PartCatagory>));
                            // Call the Deserialize method to restore the object's state.
                            partCatagories = (BindingList<PartCatagory>)serializer.Deserialize(reader);
                        }
                    }
                    else
                    {
                        PartCatagory np = new PartCatagory();  
                        np.Name = "Fuel System";
                        np.Description = "Fuel System components";
                        partCatagories.Add(np);
                        SavePartCatagories();
                    }
                }
               
            }
            catch (Exception ex)
            {
                string message = "Loading part catagory data (file " + FileName + "): | " + ex.Message + " | The part catagory data file cannot be opened.";
                MessageBox.Show(message, "Part Catagory Data File Error!");
            }
            return partCatagories;
        }
        public static string GetPartCatagoryName(string ID)
        {
            string rtn = "Uncategorized";
            foreach(PartCatagory pc in PartCatagories)
            {
                if(pc.ID == ID)
                {
                    rtn = pc.Name;
                    break;
                }
            }
            return rtn;
        }
        public static string GetPartSubCatagoryName(string ID)
        {
            string rtn = "Uncategorized";
            BindingList<PartCatagory> partCatagories = LoadPartCatagories();
            foreach (PartCatagory pc in partCatagories)
            {
                foreach(PartSubCatagory psc in pc.SubCategories)
                {
                    if(psc.ID == ID)
                    {
                        rtn = psc.Name;
                        return rtn;
                    }
                }
           }
            return rtn;
        }
        public static void SavePartCatagories()
        {
            string subcatagoryFileName, FileName = "";

            /// TODO: Remove orphaned subcatagories when saving catagories.

            if ((FileName == null) || (!File.Exists(FileName)))
            {
                FileName = PartsFolder + "PartCatagories.xml";
            }           
            var serializer = new XmlSerializer(typeof(BindingList<PartCatagory>));
            using (var writer = new StreamWriter(FileName))
            {
                serializer.Serialize(writer, PartCatagories);
            }            
           MessengerService.LogMessage("Part Catagories", MessengerService.MessageType.DataChanged);
        }
        public static string GetPartsFileName()
        {
            string rtn = PartsFolder + "Parts.xml";
            return rtn;
        }
        public static string GetPartImgHex(string PartID)
        {
            string rtn = "";
            if (Parts == null) LoadParts();
            foreach (Part p in Parts)
            {
                if (p.ID == PartID)
                {
                    rtn = p.ImgHEX;
                    break;
                }
            }         
            return rtn;
        }
        public static int GetPartInventory(string PartID)
        {
            int rtn = 0;
            if (Parts == null) LoadParts();
            foreach (Part p in Parts)
            {
                if (p.ID == PartID)
                {
                    rtn = p.QuantityOnHand;
                    break;
                }
            }

            return rtn;
        }
        public static string GetPartName(string PartID)
        {
            string rtn = "Unknown Part";
            if (Parts == null) LoadParts();
            foreach (Part p in Parts)
            {
                if (p.ID == PartID)
                {
                    rtn = p.Name;
                    break;
                }
            }
            return rtn;
        }
        public static string GetPartNumber(string PartID)
        {
            string rtn = "Unknown Part";
            if (Parts == null) LoadParts();
            foreach (Part p in Parts)
            {
                if (p.ID == PartID)
                {
                    rtn = p.PartNumber;
                    break;
                }
            }
            return rtn;
        }

        public static MakesList LoadMakes()
        {
            MakesList SavedMakes = new MakesList();

            try
            {
                string FileName = MakeModelFolder + "Makes.xml";
                if (File.Exists(FileName))
                {
                    using (Stream reader = new FileStream(FileName, FileMode.Open))
                    {
                        var serializer = new XmlSerializer(typeof(MakesList));
                        // Call the Deserialize method to restore the object's state.
                        SavedMakes = (MakesList)serializer.Deserialize(reader);
                    }
                }
                else
                {
                    Make NewMake = new Make();
                    NewMake.ID = Guid.NewGuid().ToString();
                    NewMake.Name = "Ford";
                    SavedMakes.Add(NewMake);
                    NewMake = new Make();
                    NewMake.ID = Guid.NewGuid().ToString();
                    NewMake.Name = "Chevrolet";
                    SavedMakes.Add(NewMake);
                    NewMake = new Make();
                    NewMake.ID = Guid.NewGuid().ToString();
                    NewMake.Name = "Chrysler";
                    SavedMakes.Add(NewMake);
                    NewMake = new Make();
                    NewMake.ID = Guid.NewGuid().ToString();
                    NewMake.Name = "Toyota";
                    SavedMakes.Add(NewMake);
                    NewMake = new Make();
                    NewMake.ID = Guid.NewGuid().ToString();
                    NewMake.Name = "Honda";
                    SavedMakes.Add(NewMake);
                    NewMake = new Make();
                    NewMake.ID = Guid.NewGuid().ToString();
                    NewMake.Name = "Nissan";
                    SavedMakes.Add(NewMake);
                    var serializer = new XmlSerializer(typeof(MakesList));
                    using (var writer = new StreamWriter(FileName))
                    {
                        serializer.Serialize(writer, SavedMakes);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Loading Make data (file Makes.xml): | " + ex.Message + " | The Makes data file will be backed up and a new one created.";
                MessageBox.Show(message, "Make Data File Error!");
                RemakeMakesFile();
                MessageBox.Show("A new make File has been created, please restart application.", "Make Data File Backup/Repair");
                App.Current.Shutdown();
            }
            return SavedMakes;
        }
        public static MakesList LoadMakesFile(string FileName)
        {
            MakesList SavedMakes = new MakesList();

            try
            {
                if (File.Exists(FileName))
                {
                    using (Stream reader = new FileStream(FileName, FileMode.Open))
                    {
                        var serializer = new XmlSerializer(typeof(MakesList));
                        // Call the Deserialize method to restore the object's state.
                        SavedMakes = (MakesList)serializer.Deserialize(reader);
                    }
                }
                else
                {
                    string message = "Cannot locate file:" + FileName + "): | Loading default file.";
                    MessageBox.Show(message, "Make Data File missing!");
                    SavedMakes = LoadMakes();
                }
            }
            catch (Exception ex)
            {
                string message = "Loading Make data (file " + FileName + "): | " + ex.Message + " | The Makes data file cannot be opened.";
                MessageBox.Show(message, "Make Data File Error!");
            }
            return SavedMakes;
        }
        public static void RemakeMakesFile()
        {
            string strTime = DateTime.Now.ToString();
            strTime = strTime.Replace(@"\","").Replace(":","").Replace(" ","").Replace("/","");

            string FileName = MakeModelFolder + "Makes.xml";
            string BakFileName = MakeModelFolder + strTime + "Makes.bak";

            if (File.Exists(FileName))
            {
                File.Move(FileName, BakFileName);
            }
        }
        public static void SortMake<Make>(this ObservableCollection<Make> collection, IComparer<Make> comparer)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                int min = i;
                for (int j = i + 1; j < collection.Count; j++)
                    if (comparer.Compare(collection[j], collection[min]) < 0) min = j;
                if (min == i) continue;
                Make temp = collection[i];
                collection[i] = collection[min];
                collection[min] = temp;
            }
        }
        

        public static string GetVenderName(string ID)
        {
            string rtn = "Unknown Vendor";
            VenderList venders = StaticMethods.Venders;
            foreach (Vender v in venders)
            {
                if (v.ID == ID)
                {
                    rtn = v.Name;
                    break;
                }
            }
            return rtn;
        }
        public static void OpenExternalFile(string TargetFileName)
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = TargetFileName, UseShellExecute = true });
            }
            catch
            {
                MessageBox.Show("Cannot launch File.", "File Launch Fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable
        {
            List<T> sorted = collection.OrderBy(x => x).ToList();
            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }



        #region Tools
        public static float CalculateInjectorFlowPressureCorrection(float RatedFlow, float RatedPressure, float ActualPressure)
        {
            float rtn = 0;

            rtn = RatedFlow * (float) Math.Pow((ActualPressure / RatedPressure), 0.5);

            return rtn;
        }
        public static int CalculateInjectorRequiredFlow(float CrankHP, int NumberOfInjectors, float BSFC, float DutyCycle)
        {
            int rtn = 0;

            rtn = (int) Math.Ceiling((CrankHP * BSFC)/(DutyCycle * NumberOfInjectors));

            return rtn;
        }
        #endregion

        #endregion
    }


}
