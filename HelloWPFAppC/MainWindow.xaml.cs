using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tesseract;

namespace HelloWPFAppC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string imgPath;

        public MainWindow()
        {
            InitializeComponent();
        }

        //调用tesseract实现OCR识别
        public string ImageToText(string imgPath)
        {
            using (var engine = new TesseractEngine(@"C:\Users\zheng\source\repos\HelloWPFAppC\HelloWPFAppC\tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(imgPath))
                {
                    using (var page = engine.Process(img))
                    {
                        return page.GetText();
                    }
                }
            }
        }

        // 工具栏 “识别” 按钮单击事件
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TesseractEngine ocr;
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string rootPath = path.Substring(0, path.LastIndexOf("bin"));
            string tessdataPath = string.Concat(rootPath, "tessdata"); // TODO: 修改为资源->打包后可用
            ocr = new TesseractEngine(tessdataPath, "eng", EngineMode.Default);//设置语言   英文

            var img = Pix.LoadFromFile(imgPath);
            //bit = PreprocesImage(bit);//进行图像处理,如果识别率低可试试
            Tesseract.Page page = ocr.Process(img);
            string str = "识别结果：\n\n";
            str += page.GetText();//识别后的内容  
            page.Dispose();

            //string str = ImageToText(imgPath);
            tessTextBlock.Text = str;  // 识别结果显示到界面
            // TODO: 识别失败的处理
        }

        // 菜单栏 “打开”单击事件
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "打开文件";
            dlg.Multiselect = true;
            dlg.DefaultExt = ".png";
            dlg.Filter = "图片|*.png;*.jpg;*.jpeg|PDF|*.pdf";

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Get file dir
                //imgPath = dlg.FileName;
                string[] imgPaths = dlg.FileNames;  // TODO: save?
                UpdateTreeviewData(imgPaths);
                imgPath = imgPaths[0]; // 默认显示第一张图片
                originIm.Source = new BitmapImage(new Uri(imgPath));  // 图片显示到界面
                RecognizeButton.IsEnabled = true; // 识别按钮使能
            }
        }

        // 菜单栏 “保存”单击事件
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {

        }

        // “打开”文件后更新左侧文件列表
        private void UpdateTreeviewData(string[] imgPaths)
        {
            fileTreeViewer.Items.Clear(); // TODO: 叠加而非清除
            TreeViewItem defaultList = new TreeViewItem() { 
                TabIndex = 1,
                Header = "列表1",
                IsExpanded = true
            };

            for (int ii = 0; ii < imgPaths.Length; ii++)
            {
                TreeViewItem item = new TreeViewItem() { TabIndex = 2, Header = imgPaths[ii] };
                defaultList.Items.Add(item);
            }
            fileTreeViewer.Items.Add(defaultList);
        }

        // 选择列表中的文件后，更新当前图片
        private void fileTreeViewer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {   if (((TreeViewItem)e.NewValue).TabIndex == 1)
                return;

            imgPath = ((TreeViewItem)e.NewValue).Header.ToString();
            originIm.Source = new BitmapImage(new Uri(imgPath));  // 图片显示到界面
        }
    }
}
