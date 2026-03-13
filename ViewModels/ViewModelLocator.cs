namespace AutomotiveBuilder.ViewModels
{
    public class ViewModelLocator
    {
        private static MainViewModel? _main;
        private static MakeModelVM? _makeMod;
        private static VenderVM? _venderViewModel;

        public MainViewModel Main => _main ??= new MainViewModel();
        public MakeModelVM MakeMod => _makeMod ??= new MakeModelVM();
        public VenderVM VenderViewModel => _venderViewModel ??= new VenderVM();
    }
}
