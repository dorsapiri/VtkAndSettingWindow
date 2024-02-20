using Kitware.VTK;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using VtkAndSettingWindow_sample.helpers;

namespace VtkAndSettingWindow_sample.ViewModel
{
    class VtkWindowVM : INotifyPropertyChanged
    {
        #region Properties
        #region Property Changed
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void onPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        public DrrTransformVM drrTransformVM { get; set; }
        public vtkImageData transformedImageData;
        vtkImageActor actor;
        #region RenderWindowControl Property
        private RenderWindowControl _renderWindowControl;
        public RenderWindowControl renderWindowControl
        {
            get { return _renderWindowControl; }
            set 
            { 
                _renderWindowControl = value;
                onPropertyChanged(nameof(renderWindowControl));
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Constroctor
        public VtkWindowVM()
        {
            drrTransformVM = new DrrTransformVM( this );
            actor = new();
        }
        #endregion
        #region WindowsFormHost Loaded
        public void WindowsFormsHost_Loaded(object sender, RoutedEventArgs e)
        {
            if (renderWindowControl == null)
            {
                WindowsFormsHost? windowsFormHost = sender as WindowsFormsHost;
                renderWindowControl = new();
                windowsFormHost.Child = renderWindowControl;
            }
            renderWindowControl.RenderWindow.Render();
            renderWindowControl.RenderWindow.GetInteractor().SetInteractorStyle(vtkInteractorStyleImage.New());
            string directoryPath = @"C:\Users\k703528\Documents\Study_1 _ CompleteS";
            var imageData = ReadDicomDirectory(directoryPath);
            transformedImageData = transformeImageDataWithReslice(imageData);
            var drrRotation = DRRrotation(transformedImageData, drrTransformVM.txtGantAngel, drrTransformVM.txtTableAngle);
            RenderDicom(drrRotation);
        }
        #endregion
        #region Read Dicoms
        private vtkImageData ReadDicomDirectory(string path)
        {
            vtkDICOMImageReader reader = new vtkDICOMImageReader();
            reader.SetDirectoryName(path);
            reader.Update();
            vtkImageData imageData = reader.GetOutput();
            var center = imageData.GetCenter();
            imageData.SetOrigin(-center[0], -center[1], -center[2]);//Move Images center into (0,0,0)
            return imageData;
        }
        #endregion
        #region Initial Gant
        private vtkImageData transformeImageDataWithReslice(vtkImageData imageData)
        {
            //Rotate ImageData
            vtkTransform transform = new();
            transform.RotateX(-90);

            vtkImageReslice reslice = new();
            reslice.SetInputData(imageData);
            reslice.SetResliceTransform(transform);
            reslice.SetResliceAxesOrigin(0, 0, 0);
            reslice.Update();


            vtkImageData transformedImageData = reslice.GetOutput();
            return transformedImageData;
        }
        #endregion
        #region DRR, Rotate Gant and Table
        public vtkImageData DRRrotation(vtkImageData imageData, int GantAngel, int TableAngel)
        {
            int slabNumberOfSlices = imageData.GetDimensions()[0] * (int)imageData.GetSpacing()[2];

            // Convert the DICOM images to a 3D volume
            vtkImageReslice reslice = vtkImageReslice.New();
            reslice.SetInputData(imageData);
            reslice.SetOutputDimensionality(2);
            reslice.SlabTrapezoidIntegrationOn();
            reslice.SetSlabSliceSpacingFraction(2);
            reslice.InterpolateOn();
            reslice.SetSlabModeToMax();
            reslice.SetSlabNumberOfSlices(slabNumberOfSlices);
            //Axis Rotation
            vtkTransform transformAxis = new();
            transformAxis.RotateZ(-TableAngel);
            transformAxis.RotateY(GantAngel);
            reslice.SetResliceAxes(transformAxis.GetMatrix());
            
            reslice.Update();
            return reslice.GetOutput();
        }
        #endregion
        #region Render Rotated DRR
        public void RenderDicom(vtkImageData imageData)
        {
            actor.SetInputData(imageData);
            actor.Update();
            
            renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().AddActor(actor);
            renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().Render();
            renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().ResetCamera();
        }
        #endregion
        #endregion
    }
}
