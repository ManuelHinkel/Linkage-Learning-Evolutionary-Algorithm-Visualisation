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
            typeof(OneMax),
            typeof(LeadingOnes),
            typeof(DeceptiveTrap),
            typeof(HIFF),
            typeof(NKLandscape),
            typeof(NKLandscapeRandom),
            typeof(IsingRing),
            typeof(IsingLattice),
            typeof(MaxSat),
            ];
        public static Type[] LocalSearchFunctions { get; private set; } = [typeof(HillClimber)];
        public static Type[] TerminationCriterias { get; private set; } = [
            typeof(IterationTermination),
            typeof(FitnessTermination),
        ];
        public static Type[] Algorithms { get; private set; } = [
            typeof(MIP),
            typeof(P3),
            typeof(ROMEA), 
            typeof(GOMEA),
            typeof(LocalSearchROMEA),
            typeof(LocalSearchGOMEA),
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

        private int _fitnessFuntion;
        public int SelectedFitnessFuntion
        {
            get => _fitnessFuntion;
            set
            {
                this.RaiseAndSetIfChanged(ref _fitnessFuntion, value);
                FitnessArgument = "";
                EnableFitnessArg = ((AFitnessFunction)Activator.CreateInstance(FitnessFunctions[value])).EnableArg;
            }
        }
        public int SelectedFOSFunction { get; set; }
        public int SelectedTerminationCriteria { get; set; }
        public string TerminationArgument { get; set; }
        public string FitnessArgument { get; set; }

        private int _algorithm;
        public int SelectedAlgorithm
        {
            get => _algorithm;
            set
            {
                this.RaiseAndSetIfChanged(ref _algorithm, value);
                var a = Activator.CreateInstance(Algorithms[value]) as ALinkageLearningAlgorithm;
                ShowGrowthFunction = a.ShowGrowthFunction;
                ShowLocalSearchFunction = a.ShowLocalSearchFunction;
                ShowPopulationSize = a.ShowPopulationSize;
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
        public bool EnableFitnessArg { get; set; }

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
                ErrorMessage = "Number Of Bits is not a valid integer.\nMust be greater than 0!";
                return;
            }

            ALinkageLearningAlgorithm algorithm = Activator.CreateInstance(Algorithms[SelectedAlgorithm]) as ALinkageLearningAlgorithm;
            
            // Population size validation
            int populationSize;
            isInt = int.TryParse(PopulationSize, out populationSize);
            if (algorithm.ShowPopulationSize && (!isInt || populationSize < 2))
            {
                ErrorMessage = "Population Size is not valid.\nMust be greater than 1!";
                return;
            }

            // Termination Criteria validation
            ATerminationCriteria terminationCriteria = Activator.CreateInstance(TerminationCriterias[SelectedTerminationCriteria])
                   as ATerminationCriteria;

            if (!terminationCriteria.CreateArgumentFromString(TerminationArgument))
            {
                ErrorMessage = "Can't parse the termination argument.\n Must be of type: " + terminationCriteria.ArgumentType.Name;
                return;
            }


            // Fitness function validation
            var f = Activator.CreateInstance(FitnessFunctions[SelectedFitnessFuntion]) as AFitnessFunction;
            if (!f.CreateArgumentFromString(FitnessArgument))
            {
                ErrorMessage = f.GetArgValidationErrorMessage(FitnessArgument);
                return;
            }
            if (!f.ValidateSolutionLength(numberOfBits))
            {
                ErrorMessage = f.GetSolutionLengthValidationErrorMessage(numberOfBits);
                return;
            }


            // Creating run data
            RunData newRunData = new RunData
            {
                Algorithm = algorithm,
                FOSFunction = Activator.CreateInstance(FOSFunctions[SelectedFOSFunction]) as AFOSFunction,
                FitnessFunction = f,
                TerminationCriteria = terminationCriteria,
                NumberOfBits = numberOfBits,
                NumberOfSolutions = populationSize,
            };

            if (algorithm.ShowLocalSearchFunction)
            {
                newRunData.LocalSearchFunction = Activator.CreateInstance(LocalSearchFunctions[SelectedLocalSearchFunction]) as ALocalSearchFunction;
            }

            if (algorithm.ShowGrowthFunction)
            {
                newRunData.GrowthFunction = Activator.CreateInstance(GrowthFunctions[SelectedGrowthFunction]) as AGrowthFunction;
            }

            Random r = new Random();
            newRunData.RNGSeed = r.Next();

            GlobalManager.Instance.SetNewRunData(newRunData);
            CloseWindow();
        }

        public void CloseWindow()
        {
            _window.Close();
        }

    }
}
