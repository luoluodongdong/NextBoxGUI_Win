using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using NextBox.TestModel;
using NextBox.TestModel.SubClass;
using NextBox.SubForms;
using NextBox.SubViews;
using System.Threading;

namespace NextBox
{
    public partial class ContainerView : UserControl
    {
        public ContainerView()
        {
            InitializeComponent();
        }
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private List<UnitSettingView> unitSetViewArr = new List<UnitSettingView>();
        private List<DetailView> detailViewArr = new List<DetailView>();
        private Dashboard dashboard = new Dashboard();
        private UserControl currentView = new UserControl();
        //private DetailView detailView = new DetailView();
        private StationSettingView stationSetView = new StationSettingView();
        private LoadView loadView = new LoadView();
        //UnitSettingView unitSetView = new UnitSettingView();

        private TestCoreManager coreManager = TestCoreManager.Instance;
        private readonly object _syncShowDialog = new object();
        private void ContainerView_Load(object sender, EventArgs e)
        {
            logoLabel.Visible = false;
            loadView.eventFromLoadView += new EventFromSubViewDelegate(EventFromSubView);
            SwitchToLoadView();
            Thread loadTransaction = new Thread(new ThreadStart(loadFunction));
            loadTransaction.IsBackground = true;
            loadTransaction.Start();

            //SwitchToDashboard();
            
        }
        #region Load Function
        private void loadFunction()
        {
            //检查数字签名
            loadView.updateLoadMessage("Load safety check...", 5);
            Thread.Sleep(1000);
            CheckSecurity cs = new CheckSecurity();
            string securityFolder = Application.StartupPath + @"\NBArchive\Testplan";
            if (cs.verifySecurity(securityFolder) == false)
            {
                string errorMsg = "Station safety check failure!";
                Invoke((EventHandler)(delegate {
                    SwitchToFatalErrorView(errorMsg);
                }));
                return;
            }
            loadView.updateLoadMessage("Load safety check...OK", 15);
            Thread.Sleep(500);
            //启动测试管理主单元
            loadView.updateLoadMessage("Load test core message...", 20);
            Thread.Sleep(200);
            if(coreManager.initTestCoreManager() == false)
            {
                string errorMsg = "Load test core manager failure!";
                Invoke((EventHandler)(delegate {
                    SwitchToFatalErrorView(errorMsg);
                }));
                return;
            }
            coreManager.eventFromTestCoreManager += new EventFromTestCoreManagerDelegate(eventFromTestCoreManager);
            coreManager.stationUITaskEvent += new StationUITaskDelegate(eventOfStationUITask);
            loadView.updateLoadMessage("Load test core message...OK", 30);
            Thread.Sleep(500);
            //初始化dashboard
            loadView.updateLoadMessage("Load dashboard view...", 35);
            Thread.Sleep(200);
            dashboard.initDashboard();
            dashboard.eventFromDashboard += new EventFromSubViewDelegate(EventFromSubView);
            loadView.updateLoadMessage("Load dashboard view...OK", 40);
            Thread.Sleep(500);
            //初始化station setting view
            loadView.updateLoadMessage("Load station setting view...", 45);
            Thread.Sleep(200);
            stationSetView.initView();
            stationSetView.eventFromStationSetView += new EventFromSubViewDelegate(EventFromSubView);
            loadView.updateLoadMessage("Load station setting view...OK", 50);
            Thread.Sleep(500);
            //初始化Unit & Detailviews
            loadView.updateLoadMessage("Load unit setting view...", 60);
            Thread.Sleep(200);
            initUintSettingViews();
            initDetailViews();
            loadView.updateLoadMessage("Load unit setting view...OK", 80);
            Thread.Sleep(500);
            //加载完成
            loadView.updateLoadMessage("Load progress finished.", 100);
            Thread.Sleep(500);
        }

        public void closeApp()
        {
            stationSetView.closeView();
            foreach (UnitSettingView item in unitSetViewArr)
            {
                item.closeView();
            }
        }

        private void initUintSettingViews()
        {
            for(int i = 0; i < coreManager.unitCount; i++)
            {
                UnitSettingView unitSetView = new UnitSettingView();
                unitSetView.index = i;
                unitSetView.initView();
                unitSetView.eventFromUnitSetView += new EventFromSubViewDelegate(EventFromSubView);
                unitSetViewArr.Add(unitSetView);


            }
        }
        private void initDetailViews()
        {
            for (int i = 0; i < coreManager.unitCount; i++)
            {
                DetailView detailView = new DetailView();
                detailView.index = i;
                detailView.initView();
                detailView.eventFromDetailView += new EventFromSubViewDelegate(EventFromSubView);
                detailViewArr.Add(detailView);

                TestSequencer sequencer = coreManager.sequencersList[i];
                sequencer.insterDataRow += new EventInsertDataRow(detailView.insertDataRow);
            }
        }

        #endregion

        #region Switch To Sub Views
        private void SwitchToLoadView()
        {
            if (Controls.Contains(currentView))
            {
                Controls.Remove(currentView);
            }
            loadView.Location = new Point(0, 0);
            loadView.Dock = DockStyle.Fill;
            Controls.Add(loadView);
            currentView = loadView;
        }

        private void SwitchToDashboard()
        {
            if (Controls.Contains(currentView))
            {
                Controls.Remove(currentView);
            }
            dashboard.Location = new Point(0, 0);
            dashboard.Dock = DockStyle.Fill;
            dashboard.refreshDashboard();
            Controls.Add(dashboard);
            currentView = dashboard;
        }

        private void SwitchToDetailView(int index)
        {
            if (Controls.Contains(currentView))
            {
                Controls.Remove(currentView);
            }
            DetailView detailView = detailViewArr[index];
            detailView.Location = new Point(0, 0);
            detailView.Dock = DockStyle.Fill;
            //detailView.index = index;
            //detailView.initView();
            detailView.refreshUI();
            Controls.Add(detailView);
            currentView = detailView;
        }
        private void SwitchToStationSetView()
        {
            if (Controls.Contains(currentView))
            {
                Controls.Remove(currentView);
            }
            stationSetView.Location = new Point(0, 0);
            stationSetView.Dock = DockStyle.Fill;
            //stationSetView.initView();
            Controls.Add(stationSetView);
            currentView = stationSetView;
        }
        private void SwitchToUnitSetView(int index)
        {
            if (Controls.Contains(currentView))
            {
                Controls.Remove(currentView);
            }
            UnitSettingView unitSetView = unitSetViewArr[index];
            unitSetView.Location = new Point(0, 0);
            unitSetView.Dock = DockStyle.Fill;
            unitSetView.reloadView();
            Controls.Add(unitSetView);
            currentView = unitSetView;
        }

        private void SwitchToFatalErrorView(string msg)
        {
            printLog("Fatal Error:" + msg);
            if (Controls.Contains(currentView))
            {
                Controls.Remove(currentView);
            }
            FatalErrorView errorView = new FatalErrorView();
            errorView.showErrorMessage(msg);
            errorView.Location = new Point(0, 0);
            errorView.Dock = DockStyle.Fill;
            Controls.Add(errorView);
            currentView = errorView;
        }
        #endregion

        #region Event From TestCoreManager
        private void eventFromTestCoreManager(Dictionary<string, object> content)
        {
            //string type = (string)content["type"];
            //printLog("event from test core manager:" + type);

        }
        #endregion

        #region Event Of StationUI Task
        private TMRecord eventOfStationUITask(TMRequest request)
        {
            TMTestItem item = request.testItem;
            TMRecord record = new TMRecord();
            if (item.function.Equals(Function.StationUIFuncSync))
            {
                DialogWithOK dialog = new DialogWithOK();
                dialog.title = "Station Dialog";
                dialog.message = item.param1;
                lock (_syncShowDialog)
                {
                    Invoke((EventHandler)(delegate {
                        dialog.ShowDialog();
                    }));
                    
                }
                
                record.measurement = "stationUISyncValue";
            }
            else if (item.function.Equals(Function.StationUIFuncAsync))
            {
                DialogWithOK dialog = new DialogWithOK();
                dialog.title = string.Format("{0}-{1}", request.name, request.index+1);
                dialog.message = item.param1;
                Invoke((EventHandler)(delegate {
                    dialog.ShowDialog();
                }));
                record.measurement = "OK";
            }
            //----------------dummy testplan function--------------
            else if (item.function.Equals(Function.AsyncDialog))
            {
                DialogWithOK dialog = new DialogWithOK();
                dialog.title = string.Format("{0}-{1}", request.name, request.index + 1);
                dialog.message = item.param1;
                Invoke((EventHandler)(delegate {
                    dialog.ShowDialog();
                }));
                record.measurement = "OK";
            }
            else if (item.function.Equals(Function.SyncDialog))
            {
                DialogWithOK dialog = new DialogWithOK();
                dialog.title = "Station Dialog";
                dialog.message = item.param1;
                lock (_syncShowDialog)
                {
                    Invoke((EventHandler)(delegate {
                        dialog.ShowDialog();
                    }));

                }

                record.measurement = "OK";
            }

            return record;
        }
        #endregion

        #region SubView Event Delegate
        private void EventFromSubView(Dictionary<string, string> content)
        {
            string name = content["name"];
            string type = content["type"];
            printLog(string.Format("[Container] - event type:{0} from:{1}", type, name));
            if (name.Equals("Dashboard"))
            {
                if (type.Equals("UnitClick"))
                {
                    int index = int.Parse(content["data"]);
                    SwitchToDetailView(index);

                }
                else if (type.Equals("SetBtnClick"))
                {
                    SwitchToStationSetView();
                }
                else if (type.Equals("UnitSetClick"))
                {
                    int index = int.Parse(content["data"]);
                    SwitchToUnitSetView(index);
                }
                else if (type.Equals("ModeChanged"))
                {
                    int index = int.Parse(content["index"]);
                    if(index == 0)
                    {
                        BackColor = Color.White;
                    }
                    else if(index == 1)
                    {
                        BackColor = Color.HotPink;
                    }
                    else if(index == 2)
                    {
                        BackColor = Color.DarkTurquoise;
                    }
                }
            }
            else if (name.Equals("Load"))
            {
                Invoke((EventHandler)(delegate {
                    logoLabel.Visible = true;
                    SwitchToDashboard();
                }));
                
            }
            else if (name.Equals("Detail"))
            {
                if (type.Equals("back"))
                {
                    SwitchToDashboard();
                }

            }
            else if(name.Equals("StationSet"))
            {
                if (type.Equals("back"))
                {
                    SwitchToDashboard();
                }

            }
            else if (name.Equals("UnitSet"))
            {
                int index = int.Parse(content["data"]);
                if (type.Equals("back"))
                {
                    SwitchToDashboard();
                }

            }
        }
        #endregion

        private void printLog(string txt)
        {
            log.Info(string.Format("[Container] ->log:{0}", txt));
            //Console.WriteLine(string.Format("[Container] ->log:{0}", txt));
        }
    }
}
