using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LLEAV.Models;
using LLEAV.ViewModels.Controls.PopulationDepictions;
using Xunit.Abstractions;

namespace LLEAVTest.Unit.ViewModels
{
    public class PopulationViewModelTest
    {
        private readonly ITestOutputHelper _out;
        public PopulationViewModelTest(ITestOutputHelper testOutputHelper)
        {
            _out = testOutputHelper;
        }

        [Fact]
        public void TestPopulationBlock()
        {
            PopulationBlocksViewModel blocks = new PopulationBlocksViewModel();

            blocks.Update(ExampleData());

            Assert.Equal(2, blocks.Containers.Count);

            Assert.Equal(10, ((PopulationBlock)blocks.Containers[0]).Population.MaximumFitness);
            Assert.Equal(24, ((PopulationBlock)blocks.Containers[1]).Population.MaximumFitness);

        }

        [Fact]
        public void TestPopulationBar()
        {
            PopulationBarsViewModel bars = new PopulationBarsViewModel();

            bars.Update(ExampleData());

            Assert.Equal(2, bars.Containers.Count);


            Assert.Equal("10", ((PopulationBar)bars.Containers[0]).XAxis[0].Labels.Last());
            Assert.Equal(2, ((ColumnSeries<int>)((PopulationBar)bars.Containers[0]).Series.GetValue(0)).Values.ElementAt(3));


        }

        [Fact]
        public void TestPopulationGraph()
        {
            RunData exampleData = ExampleRunData(20);

            PopulationGraphsViewModel graphs = new PopulationGraphsViewModel(exampleData);

            graphs.Update(exampleData.Iterations.Last());

            Assert.Equal(10, ((LineSeries<double>)((PopulationGraph)graphs.Containers[0]).Series.GetValue(0)).Values.Count());

            Assert.Equal("10", ((PopulationGraph)graphs.Containers[0]).XAxis[0].Labels[0]);

            Assert.Equal("19", ((PopulationGraph)graphs.Containers[0]).XAxis[0].Labels.Last());

            Assert.Equal(10.0, ((LineSeries<double>)((PopulationGraph)graphs.Containers[0]).Series.GetValue(2)).Values.ElementAt(0));

            Assert.Equal(19.0, ((LineSeries<double>)((PopulationGraph)graphs.Containers[0]).Series.GetValue(2)).Values.Last());
        }

        [Fact]
        public void TestPopulationBoxPlot()
        {
            RunData exampleData = ExampleRunData(30);

            PopulationBoxPlotViewModel box = new PopulationBoxPlotViewModel(exampleData);

            box.WindowSize = 13;

            box.Update(exampleData.Iterations.Last());

            Assert.Equal(13, ((BoxSeries<BoxValue>)((PopulationBoxPlot)box.Containers[0]).Series.GetValue(0)).Values.Count());

            Assert.Equal("17", ((PopulationBoxPlot)box.Containers[0]).XAxis[0].Labels[0]);

            Assert.Equal("29", ((PopulationBoxPlot)box.Containers[0]).XAxis[0].Labels.Last());

            Assert.Equal(17.0, ((BoxSeries<BoxValue>)((PopulationBoxPlot)box.Containers[0]).Series.GetValue(0)).Values.ElementAt(0).Max);

            Assert.Equal(29.0, ((BoxSeries<BoxValue>)((PopulationBoxPlot)box.Containers[0]).Series.GetValue(0)).Values.Last().Max);

            box.WindowSize = 40;

            box.Update(exampleData.Iterations.Last());

            Assert.Equal(30, ((BoxSeries<BoxValue>)((PopulationBoxPlot)box.Containers[0]).Series.GetValue(0)).Values.Count());

            Assert.Equal("0", ((PopulationBoxPlot)box.Containers[0]).XAxis[0].Labels[0]);

            Assert.Equal("29", ((PopulationBoxPlot)box.Containers[0]).XAxis[0].Labels.Last());

            Assert.Equal(0.0, ((BoxSeries<BoxValue>)((PopulationBoxPlot)box.Containers[0]).Series.GetValue(0)).Values.ElementAt(0).Max);

            Assert.Equal(29.0, ((BoxSeries<BoxValue>)((PopulationBoxPlot)box.Containers[0]).Series.GetValue(0)).Values.Last().Max);
        }

        private RunData ExampleRunData(int iterations)
        {
            RunData r = new RunData();

            List<IterationData> iterationDatas = new List<IterationData>();


            for (int i = 0; i < iterations; i++)
            {
                iterationDatas.Add(new IterationData(null, 0));

                Population p = new Population(0);

                for (int j = 0; j <= i; j++)
                {
                    p.Add(new Solution() { Fitness = j });
                }

                iterationDatas[i].Populations = [p];
                iterationDatas[i].Iteration = i;
            }
            r.Iterations = iterationDatas;
            return r;
        }

        private IterationData ExampleData()
        {
            Population p1 = new Population(0);

            Population p2 = new Population(1);

            for (int i = 0; i < 20; i++)
            {
                p1.Add(new Solution() { Fitness = (i + 1) / 2 });
            }

            for (int i = 0; i < 5; i++)
            {
                p2.Add(new Solution() { Fitness = i + 20 });
            }

            IterationData iteration = new IterationData(null, 0);
            iteration.Populations = [p1, p2];
            return iteration;
        }
    }
}
