using System;

namespace EpiSolve
{
    /// <summary>
    /// Static class for plotting graphs related to the Evolutionary Algorithm's progress.
    /// </summary>
    static class EAGraphPlotter
    {
        public static void PlotEvolutionGraph(List<double> bestFitnessPerGeneration, 
                                              List<double> worstFitnessPerGeneration, 
                                              List<double> averageFitnessPerGeneration)
        {
            if (!bestFitnessPerGeneration.Any())
            {
                Console.WriteLine("No data to plot for evolution graph.");
                return;
            }

            var plt = new ScottPlot.Plot();

            double[] generations = Enumerable.Range(0, bestFitnessPerGeneration.Count)
                                             .Select(i => (double)i)
                                             .ToArray();

            double[] bestFitnessData = bestFitnessPerGeneration.ToArray();
            double[] averageFitnessData = averageFitnessPerGeneration.ToArray();
            double[] worstFitnessData = worstFitnessPerGeneration.ToArray();


            // Přidání křivky nejlepší fitness
            var bestFitnessLine = plt.Add.Scatter(generations, bestFitnessData);
            bestFitnessLine.LegendText = "Best Fitness";
            bestFitnessLine.Color = ScottPlot.Colors.Green;
            bestFitnessLine.LineWidth = 2;

            // Přidání křivky average fitness
            var averageFitnessLine = plt.Add.Scatter(generations, averageFitnessData);
            averageFitnessLine.LegendText = "Average Fitness";
            averageFitnessLine.Color = ScottPlot.Colors.Orange;
            averageFitnessLine.LineWidth = 2;

            // Přidání křivky nejhorší fitness
            var worstFitnessLine = plt.Add.Scatter(generations, worstFitnessData);
            worstFitnessLine.LegendText = "Worst Fitness";
            worstFitnessLine.Color = ScottPlot.Colors.Red;
            worstFitnessLine.LineWidth = 2;

            // Nastavení popisků os a titulku
            plt.Title("Evolution of Fitness Over Generations");


            plt.Axes.Bottom.Label.Text = "Generation";
            plt.Axes.Left.Label.Text = "Fitness Score (Lower is Better)";

            // Povolení legendy
            plt.Legend.IsVisible = true;
            plt.Legend.Alignment = ScottPlot.Alignment.UpperRight;

            // Uložení grafu jako obrázek
            string filePath = "../../../../../plots/fitness_graph.png";
            try
            {
                plt.Save(filePath, 600, 400);
                //Console.WriteLine($"Evolution graph saved to {System.IO.Path.GetFullPath(filePath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving graph: {ex.Message}");
            }
        }
    }
}
