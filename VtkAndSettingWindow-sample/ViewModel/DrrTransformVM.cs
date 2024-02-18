using Kitware.VTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VtkAndSettingWindow_sample.helpers;

namespace VtkAndSettingWindow_sample.ViewModel
{
    class DrrTransformVM : INotifyPropertyChanged
    {
        #region Properties
        #region Property Changed
        public event PropertyChangedEventHandler? PropertyChanged;
		protected void onPropertyChanged(string propertyName)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
        #endregion
        #region Gant Angel prop
        private int _txtGantAngel =0;
		public int txtGantAngel
        {
			get { return _txtGantAngel; }
			set 
			{ 
				_txtGantAngel = value;
				onPropertyChanged(nameof(txtGantAngel));
			}
		}
        #endregion
        #region Table Angle Prop
        private int _txtTableAngle = 0;
        public int txtTableAngle
        {
			get { return _txtTableAngle; }
			set 
			{ 
				_txtTableAngle = value;
				onPropertyChanged(nameof(txtTableAngle));
			}
		}
        #endregion
        private VtkWindowVM _vm;
        //public ICommand btnCalculateDrr;
        #endregion
        public DrrTransformVM(VtkWindowVM vtkWindowVM)
        {
            _vm = vtkWindowVM;
            //btnCalculateDrr = new RelayCommand(doDrrCalculate);
        }
        private void test()
        {
            var gant = txtGantAngel;
            var table = txtTableAngle;
            
           
        }
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            var imagedata = _vm.transformedImageData;
            var drrRotation = _vm.DRRrotation(imagedata, txtGantAngel, txtTableAngle);
            var lastActor = _vm.renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().GetActors().GetLastActor();

            _vm.renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().RemoveActor(lastActor);
            vtkImageActor actor = new();
            actor.SetInputData(drrRotation);
            _vm.renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().AddActor(actor);
            //_vm.RenderDicom(drrRotation);
        }
        /*private void doDrrCalculate()
        {
            var imagedata = _vm.transformedImageData;
            var drrRotation = _vm.DRRrotation(imagedata, txtGantAngel, txtTableAngle);
            _vm.RenderDicom(drrRotation);
        }*/
    }
}
