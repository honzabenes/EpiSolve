# EpiSolve: Optimizing epidemic management strategies using the evolutionary algorithm

## Project Description

EpiSolve is a C# application that combines an agent-based epidemic simulation with an Evolutionary Algorithm (EA) to find optimal non-pharmaceutical intervention strategies (like lockdowns) to mitigate the spread and impact of a simulated disease.

The simulation models agents moving on a grid and transitioning between Susceptible (S), Infected (I), Recovered (R), and Dead (D) states based on interactions, time, and configurable parameters.

The Evolutionary Algorithm searches for the best set of strategy parameters (e.g., lockdown start/end thresholds, effectiveness) by evaluating candidate strategies through multiple simulation runs. The fitness function aims to minimize negative outcomes (total infections, peak infections, deaths) while considering the cost of interventions (lockdown duration). A lower fitness score is better.

## Features

*   Agent-based epidemic simulation on a grid.
*   SIR (Susceptible, Infected, Recovered) model extended with Death and Immunity Loss.
*   Configurable simulation parameters (grid size, agent count, infection rates, recovery, mortality, etc.).
*   Evolutionary Algorithm implementation (Tournament Selection, Averaging Crossover, Bounded Mutation).
*   Optimization of lockdown strategy parameters.
*   Fitness function balances health outcomes and intervention costs.
*   Automatic generation of a fitness evolution graph (`fitness_graph.png`).
*   Simulation of the best found strategy with optional console visualization.

## Prerequisites

*   .NET 6.0 SDK or later.

## Setup and Running

1.  **Clone the repository:**
    ```bash
    git clone <repository_url>
    cd <repository_name>
    ```
2.  **Navigate to the project directory:**
    ```bash
    cd EpiSolve # Assuming the project is in an EpiSolve directory
    ```
3.  **Run the application:**
    ```bash
    dotnet run
    ```

The application will:
*   Load or create the `config.json` file.
*   Run the Evolutionary Algorithm for the specified number of generations.
*   Output progress and the final best strategy/results to the console.
*   Save a graph showing the fitness evolution (`fitness_graph.png`) in the output directory.
*   Optionally visualize the simulation of the best found strategy.

## Configuration

Parameters for both the simulation and the Evolutionary Algorithm are loaded from the `config.json` file.

*   If `config.json` is not found, a default one will be created.
*   You can edit `config.json` to adjust population size, generations, mutation rates, simulation duration, infection parameters, fitness function weights, etc.

**`config.json` structure:**

```json
{
  "EAParameters": {
    // Parameters for the Evolutionary Algorithm
  },
  "SimulationParameters": {
    // Parameters for the epidemic simulation and fitness function weights
  }
}
```
## Output

*   Console: Displays generation progress, the best strategy found, and its simulation results.
*   fitness_graph.png: A PNG image showing the Best, Average, and Worst fitness scores over generations. Lower fitness indicates a better strategy according to the defined fitness function.

## Code Structure

* Program.cs: Entry point, loads config, starts EA.
* ConfigLoader.cs: Handles loading/saving config.json.
* EAParameters.cs, SimulationParameters.cs: Data classes for config parameters.
* EvolutionaryAlgorithm.cs: Core EA logic (population, selection, crossover, mutation, evaluation, stats). Contains the Individual class.
* Simulation.cs: Runs a single epidemic simulation.
* Agent.cs: Defines agent behavior (movement, infection, recovery, death).
* MeasuresStrategy.cs: Defines the structure of a mitigation strategy.
* FitnessCalculator.cs: Calculates the fitness score of a strategy based on simulation results.
* GridMap.cs, GridPosition.cs, TypeTileState.cs: Grid representation.
* SimulationResult.cs: Stores metrics from a simulation run.
* TypeAgentAge.cs, TypeSIR.cs: Enumerations.
* GraphPlotter.cs: Handles plotting the fitness graph using ScottPlot.
