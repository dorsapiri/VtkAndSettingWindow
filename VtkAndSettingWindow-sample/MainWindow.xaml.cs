using Kitware.VTK;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VtkAndSettingWindow_sample.ViewModel;

namespace VtkAndSettingWindow_sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DrrTransformVM transformVM;
        public MainWindow()
        {
            InitializeComponent();
            //transformVM = new DrrTransformVM();
        }

        public void WindowsFormsHost_Loaded(object sender, RoutedEventArgs e)
        {
            renderWindowControl.RenderWindow.Render();
            renderWindowControl.RenderWindow.GetInteractor().SetInteractorStyle(vtkInteractorStyleImage.New());
            string directoryPath = @"D:\Study_1 _ CompleteS";
            var imageData = ReadDicomDirectory(directoryPath);
            var transformedImageData = transformeImageDataWithReslice(imageData);
            var drrRotation= DRRrotation(transformedImageData,transformVM.txtGantAngel,transformVM.txtTableAngle);
            RenderDicom(drrRotation);
        }
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
        private vtkImageData DRRrotation(vtkImageData imageData , int GantAngel, int TableAngel)
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
            //-----this rotation made it like Linatech
            transformAxis.RotateZ(GantAngel);
            transformAxis.RotateY(TableAngel);
            //reslice.SetResliceAxes(transformAxis.GetMatrix());
            //------------------------------------
            reslice.Update();
            return reslice.GetOutput();
        }
        private void RenderDicom(vtkImageData imageData)
        {
            vtkImageActor actor = new();
            actor.SetInputData(imageData);
            renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().AddActor(actor);
            renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().Render();
            renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().ResetCamera();
        }
    }
}