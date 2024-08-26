using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using CommonM.logger;

namespace UpantClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker worker;
        private long sum = 1000;
        private Stopwatch clock = new Stopwatch();
        private int total = 1;

        private bool isRun = false;

        public MainWindow() {
            InitializeComponent();
            LogFactory.initLog();
        }

        /// <summary>
        /// 1, 备份， 2. 解压缩， 3. 复制， 4， 修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartUpdateClick(object sender, RoutedEventArgs e) {
            /*if (!isRun) {
                isRun = true;
                progressBar.Value = 0;
                progressBar.Maximum = sum;
                //进行启动BackgroundWorker操作
                worker = new BackgroundWorker();

                worker.WorkerReportsProgress = true;
                worker.DoWork += DoWork_Handler;
                worker.ProgressChanged += ProgressChanged_Handler;
                worker.RunWorkerCompleted += RunWorkerCompleted_Handler;
                worker.RunWorkerAsync();
            }
            else {
                StringBuilder si = new StringBuilder();
                errorMsg.Visibility = Visibility.Visible;
                for (int i = 0; i < 20; i++) {
                    si.Append($"num is '{i}',,,,\r\n");
                    errorMsg.Text = si.ToString();
                }
            }*/
        }

        //同步更新UI
        //1104  在后台线程进行发包
        private void DoWork_Handler(object sender, DoWorkEventArgs args) {
            //将需要进行计算的部分在这里进行
            clock.Start();
            StartToWork();
        }

        private void ProgressChanged_Handler(object sender, ProgressChangedEventArgs args) {
            //在事件里进行进度条控件操作
            progressBar.Value += args.ProgressPercentage;
        }

        private void RunWorkerCompleted_Handler(object sender, RunWorkerCompletedEventArgs args) {
            clock.Stop();
            errorMsg.Visibility = Visibility.Visible;
            errorMsg.Text = $"{clock.ElapsedMilliseconds} ms";
            Thread.Sleep(1000);
            progressBar.Value = 0;
            isRun = false;
        }
        
        public void StartToWork() {
            //进度条控件默认值是100，这里简单举例：间隔1s时进度条增加百分之十，可将延时1s换成你想进行的算法计算
            for (int i = 0; i < sum; i++) {
                worker.ReportProgress(1);
                Thread.Sleep(10);
            }
        }


    }
}