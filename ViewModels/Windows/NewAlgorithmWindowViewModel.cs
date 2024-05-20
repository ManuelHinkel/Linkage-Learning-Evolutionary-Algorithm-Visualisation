using LLEAV.Models;
using LLEAV.Models.Algorithms;
using LLEAV.Models.Algorithms.GOM;
using LLEAV.Models.Algorithms.MIP;
using LLEAV.Models.Algorithms.ROM;
using LLEAV.Models.FitnessFunction;
using LLEAV.Models.FOSFunction;
using LLEAV.Models.GrowthFunction;
using LLEAV.Models.LocalSearchFunction;
using LLEAV.Models.TerminationCriteria;
using LLEAV.Views.Windows;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace LLEAV.ViewModels.Windows
{
    public class NewAlgorithmWindowViewModel: ViewModelBase
    {

        public static Type[] FitnessFunctions { get; private set; } = [
            typeof(LeadingOnes),
            typeof(TestFitness),
            ];
        public static Type[] LocalSearchFunctions { get; private set; } = [typeof(HillClimber)];
        public static Type[] TerminationCriterias { get; private set; } = [typeof(IterationTermination)];
        public static Type[] Algorithms { get; private set; } = [
            typeof(MIP),
            typeof(P3),
            typeof(ROMEA), 
            typeof(GOMEA),
            ];

        public static Type[] GrowthFunctions { get; private set; } = [
            typeof(ConstantGrowth),
            typeof(LinearGrowth)
            ];
        public static Type[] FOSFunctions { get; private set; } = [
            typeof(UnivariateFOS), 
            typeof(MarginalProductFOS),
            typeof(LinkageTreeFOS),
            ];


        public string NumberOfBits { get; set; } = "10";


        public int SelectedFitnessFuntion { get; set; }
        public int SelectedFOSFunction { get; set; }
        public int SelectedTerminationCriteria { get; set; }
        public string TerminationArgument { get; set; }

        private int _algorithm;
        public int SelectedAlgorithm
        {
            get => _algorithm;
            set
            {
                this.RaiseAndSetIfChanged(ref _algorithm, value);
                ShowGrowthFunction = value == 0;
                ShowLocalSearchFunction = value == 0 || value == 1;
                ShowPopulationSize = value == 2 || value == 3;
            }
        }

        public int SelectedLocalSearchFunction { get; set; }
        
        public int SelectedGrowthFunction { get; set; }

        public string PopulationSize { get; set; } = "2";

        [Reactive]
        public bool ShowLocalSearchFunction { get; set; } = true;
        [Reactive]
        public bool ShowGrowthFunction { get; set; } = true;

        [Reactive]
        public bool ShowPopulationSize { get; set; }

        [Reactive]
        public string ErrorMessage { get; set; }

        private NewAlgorithmWindow _window;

        public NewAlgorithmWindowViewModel(NewAlgorithmWindow window) 
        {
            _window = window;
        }

        public void Ok()
        {
            ErrorMessage = "";

            // Number of Bits validation
            int numberOfBits;
            bool isInt = int.TryParse(NumberOfBits, out numberOfBits);
            if (!isInt || numberOfBits < 1)
            {
                ErrorMessage = "Number Of Bits is not a valid integer.\n Must be greater than 0!";
                return;
            }

            // Population size validation
            int populationSize;
            isInt = int.TryParse(PopulationSize, out populationSize);
            if (ShowPopulationSize && (!isInt || populationSize < 2))
            {
                ErrorMessage = "Population Size is not valid.\n Must be greater than 1!";
                return;
            }


            // Termination Criteria validation
            AbstractTerminationCriteria terminationCriteria = Activator.CreateInstance(TerminationCriterias[SelectedTerminationCriteria], args: TerminationArgument)
                   as AbstractTerminationCriteria;

            if (!terminationCriteria.IsValid)
            {
                ErrorMessage = "Can't parse the termination argument.\n Must be of type: " + terminationCriteria.GetArgumentType().Name;
                return;
            }



            RunData newRunData = new RunData
            {
                Algorithm = Activator.CreateInstance(Algorithms[SelectedAlgorithm]) as ILinkageLearningAlgorithm,
                FOSFunction = Activator.CreateInstance(FOSFunctions[SelectedFOSFunction]) as IFOSFunction,
                FitnessFunction = Activator.CreateInstance(FitnessFunctions[SelectedFitnessFuntion]) as IFitnessFunction,
                TerminationCriteria = terminationCriteria,
                NumberOfBits = numberOfBits,
                NumberOfSolutions = populationSize,
            };

            if (ShowLocalSearchFunction)
            {
                newRunData.LocalSearchFunction = Activator.CreateInstance(LocalSearchFunctions[SelectedLocalSearchFunction]) as ILocalSearchFunction;
            }

            if (ShowGrowthFunction)
            {
                newRunData.GrowthFunction = Activator.CreateInstance(GrowthFunctions[SelectedGrowthFunction]) as IGrowthFunction;
            }


            GlobalManager.Instance.SetNewRunData(newRunData);
            CloseWindow();
        }

        public void CloseWindow()
        {
            _window.Close();
        }

    }
}
