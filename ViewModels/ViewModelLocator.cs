using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using MVVMtools;
using AutomotiveBuilder.PartUtils;
using AutomotiveBuilder.ViewModels.DriveTrain;

namespace AutomotiveBuilder.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    internal class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            //if (ViewModelBase.IsInDesignModeStatic)
            //{
            //    // Create design time view services and models
            //    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            //}
            //else
            //{
            //    // Create run time view services and models
            //    SimpleIoc.Default.Register<IDataService, DataService>();
            //}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MakeModelViewModel>();
            SimpleIoc.Default.Register<VenderVM>();
            SimpleIoc.Default.Register<PartSelectVM>();
            SimpleIoc.Default.Register<DriveTrainVM>();

        }

        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(Guid.NewGuid().ToString()); }
        }

        public MakeModelViewModel MakeMod
        {
            get { return ServiceLocator.Current.GetInstance<MakeModelViewModel>(Guid.NewGuid().ToString()); }
        }

        public VenderVM VenderViewModel
        {
            get { return ServiceLocator.Current.GetInstance<VenderVM>(Guid.NewGuid().ToString()); }
        }

        public PartSelectVM PartSelectViewModel
        {
            get { return ServiceLocator.Current.GetInstance<PartSelectVM>(Guid.NewGuid().ToString()); }
        }
        
        public DriveTrainVM DriveTrainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<DriveTrainVM>(Guid.NewGuid().ToString()); }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels

        }

    }
}
