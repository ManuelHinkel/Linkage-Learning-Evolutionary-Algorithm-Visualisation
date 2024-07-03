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
        /// <summary>
        /// Gets the list of all selecteable fitness functions.
        /// </summary>
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

        /// <summary>
        /// Gets the list of all selecteable local search functions.
        /// </summary>
        public static Type[] LocalSearchFunctions { get; private set; } = [typeof(HillClimber)];

        /// <summary>
        /// Gets the list of all selecteable termination criteria.
        /// </summary>
        public static Type[] TerminationCriterias { get; private set; } = [
            typeof(IterationTermination),
            typeof(FitnessTermination),
        ];

        /// <summary>
        /// Gets the list of all selecteable algorithms.
        /// </summary>
        public static Type[] Algorithms { get; private set; } = [
            typeof(MIP),
            typeof(P3),
            typeof(ROMEA), 
            typeof(GOMEA),
            typeof(LocalSearchROMEA),
            typeof(LocalSearchGOMEA),
        ];

        /// <summary>
        /// Gets the list of all selecteable growth functions.
        /// </summary>
        public static Type[] GrowthFunctions { get; private set; } = [
            typeof(ConstantGrowth),
            typeof(LinearGrowth)
            ];

        /// <summary>
        /// Gets the list of all selecteable FOS functions.
        /// </summary>
        public static Type[] FOSFunctions { get; private set; } = [
            typeof(UnivariateFOS), 
            typeof(MarginalProductFOS),
            typeof(MPVariantFOS),
            typeof(LinkageTreeFOS),
            ];

        /// <summary>
        /// Gets or sets the number of bits of a solution.
        /// </summary>
        public string NumberOfBits { get; set; } = "10";

        private int _fitnessFuntion;
        /// <summary>
        /// Gets or sets the index of the selected fitness function.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the index of the selected fos function.
        /// </summary>
        public int SelectedFOSFunction { get; set; }

        /// <summary>
        /// Gets or sets the index of the selected termination criteria.
        /// </summary>
        public int SelectedTerminationCriteria { get; set; }

        /// <summary>
        /// Gets or sets the termination argument.
        /// </summary>
        public string TerminationArgument { get; set; }

        /// <summary>
        /// Gets or sets the fitness argument.
        /// </summary>
        public string FitnessArgument { get; set; }

        private int _algorithm;
        /// <summary>
        /// Gets or sets the index of the selected algorithm.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the index of the selected local search function.
        /// </summary>
        public int SelectedLocalSearchFunction { get; set; }

        /// <summary>
        /// Gets or sets the index of the selected growth function.
        /// </summary>
        public int SelectedGrowthFunction { get; set; }

        /// <summary>
        /// Gets or sets the population size.
        /// </summary>
        public string PopulationSize { get; set; } = "2";

        /// <summary>
        /// Gets or sets if the local search function selection should be shown.
        /// </summary>
        [Reactive]
        public bool ShowLocalSearchFunction { get; set; } = true;

        /// <summary>
        /// Gets or sets if the growth function selection should be shown.
        /// </summary>
        [Reactive]
        public bool ShowGrowthFunction { get; set; } = true;

        /// <summary>
        /// Gets or sets if the population size input should be shown.
        /// </summary>
        [Reactive]
        public bool ShowPopulationSize { get; set; }

        /// <summary>
        /// Gets or sets if the fitness argument input should be shown.
        /// </summary>
        [Reactive]
        public bool EnableFitnessArg { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [Reactive]
        public string ErrorMessage { get; set; }

        private NewAlgorithmWindow _window;

        /// <summary>
        /// Creates a new instance of the view model with a window.
        /// </summary>
        /// <param name="window">The window assigned to the view model.</param>
        public NewAlgorithmWindowViewModel(NewAlgorithmWindow window) 
        {
            _window = window;
        }

        /// <summary>
        /// Creates a new simulation run.
        /// </summary>
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
        
        /// <summary>
        /// Forcefully closes the window.
        /// </summary>
        public void CloseWindow()
        {
            _window.Close();
        }

    }
}
