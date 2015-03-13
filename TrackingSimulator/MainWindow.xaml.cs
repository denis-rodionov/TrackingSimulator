using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using LogProvider;
using LocalizationCore.BuildingModel;
using LocalizationCore.Localization;
using LocalizationCore.PersonModel;
using LocalizationCore.PersonModel.Actions;
using LocalizationCore.Primitives;
using LocalizationCore.SensorModel.Sensors;
using LocalizationCore;
using LogProvider.ProcessInfo;
using LocalizationCore.Localization.Filtering;
using LocalizationCore.Localization.FingerprintingAlgorithms;
using LocalizationCore.Localization.Map;

namespace TrackingSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Options

        const FloorInstance FLOOR = FloorInstance.Hospital;

        const int FILTER_WINDOW = 100;
        const int ITERATION_COUNT = 1;

        const bool DRAW_PDF = false;
        const bool DRAW_PARTICLES = false;
        const bool DRAW_REAL_MOVEMENT = false;
        const bool DRAW_EST_MOVEMENT = false;
        const bool DRAW_EST_PERSON = false;
        const bool DRAW_ACCURACY_MAP = false;  
        const bool REDRAW = false;

        // error map shpowing and other visulalization
        string CUR_ALG = PF;
                      
        AccuracyAlgorithm CUR_AC_ALG = AccuracyAlgorithm.DistBased;

        const double PARAMETER_START = 100;
        const double PARAMETER_INCREMENT = 1;
        //const string TESTING = LEAVE_OUT;

        int BIN_COUNTS = 10;
        int PDF_COLOR_LIMIT = 3;

        const string GPS = GpsLocalization.NAME;
        const string WLAN = "WLAN";
        const string RFID = "RFID";
        const string MIXED = "Mixed";
        const string LLS = "Hybrid";
        const string LEAVE_OUT = "LeaveOut";
        const string DIST_BASED = "Distance";
        const string CLUSTERING = "Clustering";
        const string PF = "Final result";
        const string PF_W = "Particle Filter with Walls";

        const AccuracyAlgorithm PF_ACCURACY = AccuracyAlgorithm.DistBased;

        #endregion

        #region Data

        public int IterationCounter { get; set; }
        WorkThread worker;
        ModelPainter painter;
        AutoResetEvent syncEvent = new AutoResetEvent(false);   // for threads synchronization

        List<LocalizationAlgorithm> algs;

        Dictionary<string, Dictionary<int, double>> globalError = new Dictionary<string, Dictionary<int, double>>();
        Dictionary<string, Dictionary<int, double>> globalVariance = new Dictionary<string, Dictionary<int, double>>();

        Dictionary<double, double> ErrorDependency = new Dictionary<double, double>();  //   for dependency estimation

        Dictionary<double, double> TimeDependency = new Dictionary<double, double>();   // for performance estimations
        
        Timing timing;
        ProcessInfo proc;

        #endregion

        /// <summary>
        /// Testing parameter. For error dependency chart.
        /// </summary>
        public double Parameter {
            get { return ParticleFilter.PARTICLES_NUMBER; }
            set { ParticleFilter.PARTICLES_NUMBER = (int)value; }
        }

        public MainWindow()
        {
            try
            {
                Building.CreateBuilding(FLOOR);
                Parameter = PARAMETER_START;  // parameter to estimate dependency
                //test();
                timing = Timing.Start("Simulation");
                IterationCounter = 1;
                InitializeComponent();
                
                MainCanvas.Background = new SolidColorBrush(Colors.White);
                                
                painter = new ModelPainter(Model.Instance, Building.Instance.Floor, MainCanvas);                
                                
                RestartSimulation();
                
                if (DRAW_ACCURACY_MAP)
                    ShowAccuracyMap();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                Close();
            }
        }

        private void AlgorithmsInit()
        {
            algs = new List<LocalizationAlgorithm>();

            //algs.Add(new GpsLocalization(Building.Instance.Floor.ThisFloor));
            //algs.Add(new KnnFingerprinting(WLAN, new WifiRssiSensor()));
            //algs.Add(new KnnFingerprinting(RFID, new RfidSensor()));

            //algs.Add(new HistogramFingerprinting(WLAN, new WifiRssiSensor(), BIN_COUNTS));
            //algs.Add(new HistogramFingerprinting(RFID, new RfidSensor(), BIN_COUNTS));

            //algs.Add(new DataFusion(CLUSTERING, AccuracyAlgorithm.Clustering, FusionAlgs()));
            //algs.Add(new DataFusion(LEAVE_OUT, AccuracyAlgorithm.LeaveOut, FusionAlgs()));
            //algs.Add(new DataFusion(LLS, AccuracyAlgorithm.None));
            //algs.Add(new HistogramFingerprinting(MIXED, new HybridSensor(), BIN_COUNTS));
            algs.Add(new DataFusion(DIST_BASED, AccuracyAlgorithm.DistBased, FusionAlgs()));

            var area = new SRect(new Coord(0, 0), new Coord(26, 10));
            algs.Add(new WallParticleFilter(area, PF, new DataFusion(PF, PF_ACCURACY, FusionAlgs())));
            //algs.Add(new WallParticleFilter(area, PF, new DataFusion(PF, PF_ACCURACY, FusionAlgs())));
            //algs.Add(new WallParticleFilter(area, PF, new Fingerprinting(MIXED, new WifiRssiSensor())));

            if (IterationCounter == 1)  // if first iteration
                GlobalErrorInit();
        }

        private LocalizationAlgorithm[] FusionAlgs()
        {
            LocalizationAlgorithm[] res = new LocalizationAlgorithm[3];
            res[0] = new HistogramFingerprinting("WLAN (" + Name + ")", new WifiRssiSensor(), BIN_COUNTS);
            res[1] = new HistogramFingerprinting("RFID (" + Name + ")", new RfidSensor(), BIN_COUNTS);
            res[2] = new GpsLocalization(Building.Instance.Floor.ThisFloor);

            return res;
        }

        private Dictionary<string, LineSeries> ChartMapping()
        {
            Dictionary<string, LineSeries> mapping = new Dictionary<string, LineSeries>();
            mapping.Add(WLAN, FinalWlan);
            mapping.Add(RFID, FinalRfid);
            mapping.Add(GPS, FinalGPS);
            //mapping.Add(LLS, FinalClustering);
            mapping.Add(LEAVE_OUT, FinalWllsLeaveOut);
            mapping.Add(MIXED, FinalHybridFing);
            mapping.Add(PF, FinalPf);
            mapping.Add(CLUSTERING, FinalClustering);
            mapping.Add(PF_W, FinalPfExt);
            mapping.Add(DIST_BASED, FinalDistance);

            return mapping;
        }

        private void EventHandlerInit()
        {
            Model.Instance.ModelEventOccurred += Instance_ModelEventOccurred;
            foreach (Person p in Model.Instance.Patients)
            {
                p.PersonMoved += p_PersonMoved;
                p.ActionFinished += OnActionFinished;
            }
        }

        private void ShowAccuracyMap()
        {
            if (GetAlg(CUR_ALG) != null)
            {
                var alg = GetAlg(CUR_ALG);
                if (alg is Fingerprinting)
                    ((Fingerprinting)alg).BuildAccuracyMap(CUR_AC_ALG);
                DrawErrorMap(alg, CUR_AC_ALG);
            }
        }

        private void GlobalErrorInit()
        {
            foreach (var alg in algs)
            {
                globalError.Add(alg.Name, new Dictionary<int, double>());
                globalVariance.Add(alg.Name, new Dictionary<int, double>());
            }
        }

        private LocalizationAlgorithm GetAlg(string name)
        {
            return algs.Where(a => a.Name == name).SingleOrDefault();
        }

        private void FinalProcedure()
        {
            proc.StartNewStage("Column chart");
            BuildColumnChart(); ;
            proc.FinishStage();

            proc.StartNewStage("Comparison chart");
            BuildFinalChart();
            BuildErrorChart();
            proc.FinishStage();

            //((Fingerprinting)GetAlg(WLAN)).PrintStatistics();

            proc.StartNewStage("Accuracy map drawing");
            //ShowAccuracyMap();
            painter.DrawFloor();
            proc.FinishStage();

            proc.FinishProcess(true);
            ProcessStatistics.GetStatistics(proc.ProcessName).Log();
        }

        private void RestartSimulation()
        {
            if (proc != null && !proc.IsFinished)
                proc.FinishProcess();

            proc = new ProcessInfo("Simulation", IterationCounter.ToString(), true);
            lblIteration.Content = IterationCounter.ToString();

            EstimationRoute.Clear();

            if (worker != null)
                worker.Finish();

            proc.StartNewStage("Creating model");
            Model.Instance.Restart();
            Model.Instance.CreatePatients();
            EventHandlerInit();
            proc.FinishStage();

            proc.StartNewStage("Algorithm init");
            AlgorithmsInit();
            proc.FinishStage();

            proc.StartNewStage("Drawing");
            painter.DrawFilledRect(new Coord(0, 0), new Coord(26, 10), Colors.White);
            painter.DrawFloor();
            proc.FinishStage();

            proc.StartNewStage("Simulation");
            worker = new WorkThread();
        }

        private void SaveToGlobalStatistics()
        {
            foreach (var alg in algs)
            {
                globalError[alg.Name].Add(IterationCounter, alg.MeanError());
                globalVariance[alg.Name].Add(IterationCounter, alg.ErrorVariance());
            }

            if (GetAlg(CUR_ALG) != null)
            {
                ErrorDependency.Add(Parameter, GetAlg(CUR_ALG).MeanError());
                TimeDependency.Add(Parameter, lastEstimationTime.TotalMilliseconds);
            }
        }

        #region Events handlers

        /// <summary>
        /// Person moved callback
        /// </summary>
        /// <param name="p"></param>
        /// <param name="oldCoord"></param>
        /// <param name="newCoord"></param>
        void p_PersonMoved(Person p, Coord oldCoord, Coord newCoord)
        {            
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    personMovedHandler(p, oldCoord, newCoord);                    
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                syncEvent.Set();
            }), null);
            syncEvent.WaitOne();
        }

        private void OnActionFinished(Person person, PersonAction action)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    proc.FinishStage();

                    proc.StartNewStage("Adding to global statistic");
                    SaveToGlobalStatistics();
                    proc.FinishStage();

                    if (IterationCounter == ITERATION_COUNT)
                    {
                        FinalProcedure();
                    }
                    else
                    {
                        IterationCounter++;

                        // place for change parameters
                        Parameter = Parameter + PARAMETER_INCREMENT;

                        RestartSimulation();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }

            }), null);
        }

        int handler = 0;
        TimeSpan lastEstimationTime;
        void personMovedHandler(Person p, Coord oldCoord, Coord newCoord)
        {
            ++handler;
            Timing timing = Timing.Start("Handler #" + handler);
                        

            if (REDRAW)
                painter.Clear();

            if (DRAW_PDF)
                DrawPdf();

            if (REDRAW)
                painter.DrawFloor();

            if (DRAW_PARTICLES && GetAlg(CUR_ALG) is ParticleFilter)
                painter.DrawParticles(((ParticleFilter)GetAlg(CUR_ALG)).Particles);

            if (DRAW_REAL_MOVEMENT)
                DrawRealMovement(oldCoord, newCoord);

            // Estimation
            foreach (var alg in algs)
            {
                Timing time = Timing.Start();

                alg.EstimateLocation(newCoord);

                lastEstimationTime = time.Finish();
            }

            if (DRAW_EST_MOVEMENT)
                DrawEstimationMovement();

            if (DRAW_EST_PERSON)
                DrawEstPerson(GetAlg(CUR_ALG).CurrentEstimation);

            if (handler % 10 == 0)
                timing.Finish(LoggingLevel.Trace);
        }

        void Instance_ModelEventOccurred(string text)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    lbxEvents.Items.Insert(0, text);
                    lbxEvents.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }), null);
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            painter.DrawFloor();
        }

        #endregion

        #region Painting

        Coord lastCoord = null;
        private void DrawEstPerson(Coord estCoord)
        {
            if (lastCoord == null)
                lastCoord = new Coord(0, 0);

            painter.DrawEstPerson(estCoord, new SVector(lastCoord, estCoord), Colors.Black);
            lastCoord = estCoord;
        }

        private void DrawErrorMap(LocalizationAlgorithm alg, AccuracyAlgorithm acc)
        {
            var size = LocationMap.Instance.LocationSize;
            var floor = Building.Instance.Floor;
            for (double i = 0; i < floor.Height; i += size)
                for (double j = 0; j < floor.Width; j += size)
                {
                    var error = alg.GetPriorError(new Coord(j + size / 2, i + size / 2), acc);
                    painter.DrawFilledRect(new Coord(j, i), new Coord(j + size, i + size), Painter.GetGradient(error.X, 0, 4));
                }
        }

        private void Redraw()
        {
            painter.Clear();
            painter.DrawFloor();
        }

        List<Coord> realMovement = new List<Coord>();
        private void DrawRealMovement(Coord oldCoord, Coord newCoord)
        {
            //realMovement.Add(newCoord);
            //painter.DrawRoute(realMovement, Colors.Green);
            painter.DrawPerson(newCoord, new SVector(oldCoord, newCoord), Colors.Green);
        }

        LimitList<Coord> EstimationRoute = new LimitList<Coord>(100);
        private void DrawEstimationMovement()
        {
            EstimationRoute.Add(GetAlg(CUR_ALG).CurrentEstimation);
            //painter.DrawLine(GetAlg(CUR_ALG).LastEstimation, GetAlg(CUR_ALG).CurrentEstimation, Colors.Black);
            painter.DrawRoute(EstimationRoute.ToList(), Colors.Black);
        }

        private void DrawPdf()
        {
            var alg = GetAlg(CUR_ALG);
            painter.DrawPdf(alg.LastLikelihood, PDF_COLOR_LIMIT);
        }

        #endregion

        #region Charts

        private void BuildErrorChart()
        {
            //Current.DataContext = ErrorDependency.Smooth(FILTER_WINDOW);
            Current.DataContext = TimeDependency.Smooth(FILTER_WINDOW);
            lblVariance.Content = ErrorDependency.Variance();
        }

        private void BuildFinalChart()
        {
            SetYRange(LineChart, 0.5, 5.5);
            var mapping = ChartMapping();

            foreach (var item in mapping)
            {
                var alg = GetAlg(item.Key);
                if (alg != null)
                    item.Value.DataContext = alg.SmoothError(FILTER_WINDOW);
                else
                    item.Value.LegendItems.RemoveAt(0);
            }
        }

        private void SetYRange(Chart chart, double min, double max)
        {
            ((NumericAxis)chart.ActualAxes[0]).Minimum = min;
            ((NumericAxis)chart.ActualAxes[0]).Maximum = max;            
        }

        private void BuildColumnChart()
        {
            Dictionary<string, double> resultError = new Dictionary<string, double>();
            Dictionary<string, double> resultErrorVar = new Dictionary<string, double>();

            foreach (var alg in algs)
            {
                resultError[alg.Name] = globalError[alg.Name].Mean();
                resultErrorVar[alg.Name] = globalVariance[alg.Name].Mean();
            }

            ResultSeries.DataContext = resultError;
            ResultSeriesVar.DataContext = resultErrorVar;
        }

        private void CDF_click(object sender, RoutedEventArgs e)
        {
            SetYRange(LineChart, 0, 1);
            ((DisplayAxis)LineChart.Axes[0]).Title = "Probability";
            ((DisplayAxis)LineChart.Axes[1]).Title = "Error (m)";

            var mapping = ChartMapping();

            foreach (var item in mapping)
            {
                var alg = GetAlg(item.Key);
                if (alg != null)
                    item.Value.DataContext = alg.CDF(6);
            }
        }

        #endregion
    }
}
