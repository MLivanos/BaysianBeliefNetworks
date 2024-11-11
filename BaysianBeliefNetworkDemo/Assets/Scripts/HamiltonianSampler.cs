using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamiltonianSampler : Sampler
{
    public float stepSize = 0.1f;
    public int leapfrogSteps = 10;
    
    private Dictionary<Node, float> currentPositions;
    private Dictionary<Node, float> currentMomentum;
    
    public override float Sample()
    {
        for (int i = 0; i < numberOfSamples; i++)
        {
            if (i%100 == 0)
            {
                currentPositions = InitializePositions();
                currentMomentum = InitializeMomentum();
            }
            Dictionary<Node, float> proposedPosition = new Dictionary<Node, float>(currentPositions);
            Dictionary<Node, float> proposedMomentum = new Dictionary<Node, float>(currentMomentum);

            LeapFrogIntegration(proposedPosition, proposedMomentum);
            MetropolisAcceptanceTest(proposedPosition, proposedMomentum);
        }

        sampleCount += numberOfSamples;
        return CalculateProbability();
    }

    private void LeapFrogIntegration(Dictionary<Node, float> proposedPosition, Dictionary<Node, float> proposedMomentum)
    {
        for (int j = 0; j < leapfrogSteps; j++)
        {
            UpdateMomentum(proposedMomentum, proposedPosition, stepSize / 2);
            UpdatePosition(proposedPosition, proposedMomentum, stepSize);
            UpdateMomentum(proposedMomentum, proposedPosition, stepSize / 2);
        }
    }

    private void MetropolisAcceptanceTest(Dictionary<Node, float> proposedPosition, Dictionary<Node, float> proposedMomentum)
    {
        if (AcceptMove(proposedPosition, proposedMomentum))
        {
            currentPositions = proposedPosition;
            samples.Add(currentNodes.Select(n => n.IsTrue()).ToArray());
        }
        else
        {
            currentMomentum = InitializeMomentum();
        }
    }

    private Dictionary<Node, float> InitializePositions()
    {
        Dictionary<Node, float> positions = new Dictionary<Node, float>();
        foreach (Node node in graph.GetAllNodes())
        {
            positions[node] = Random.value;
        }
        return positions;
    }

    private Dictionary<Node, float> InitializeMomentum()
    {
        Dictionary<Node, float> momenta = new Dictionary<Node, float>();
        foreach (Node node in graph.GetAllNodes())
        {
            momenta[node] = Random.value - 0.5f;
        }
        return momenta;
    }

    private void UpdatePosition(Dictionary<Node, float> position, Dictionary<Node, float> momentum, float step)
    {
        foreach (Node node in currentNodes)
        {
            position[node] += step * momentum[node];
            position[node] = Mathf.Clamp(position[node], 0f, 1f);
        }
    }

    private void UpdateMomentum(Dictionary<Node, float> momentum, Dictionary<Node, float> position, float step)
    {
        foreach (Node node in currentNodes)
        {
            float grad = -CalculateGradient(node, position);
            momentum[node] += step * grad;
        }
    }

    private bool AcceptMove(Dictionary<Node, float> newPosition, Dictionary<Node, float> newMomentum)
    {
        float newEnergy = CalculateHamiltonian(newPosition, newMomentum);
        float currentEnergy = CalculateHamiltonian(currentPositions, currentMomentum);
        return Random.value < Mathf.Exp(currentEnergy - newEnergy);
    }

    private float CalculateHamiltonian(Dictionary<Node, float> position, Dictionary<Node, float> momentum)
    {
        float potential = CalculatePotentialEnergy(position);
        float kinetic = 0.0f;
        foreach (float p in momentum.Values)
        {
            kinetic += p * p / 2;
        }
        return potential + kinetic;
    }

    public float CalculatePotentialEnergy(Dictionary<Node, float> position)
    {
        return CalculatePotentialEnergy(currentNodes.Select(n => position[n]).ToArray());
    }

    public float CalculatePotentialEnergy(float[] position)
    {
        float potentialEnergy = 0f;
        
        for (int i = 0; i < numberOfNodes; i++)
        {
            Node node = currentNodes[i];
            float nodeProbability = node.Query();
            if (IsInEvidence(node))
            {
                bool isPositionConsistentWithEvidence = position[i] > (1-nodeProbability) == evidence[node];
                if (!isPositionConsistentWithEvidence)
                {
                    potentialEnergy += 1000f;
                    continue;
                }
            }
            
            node.IsTrue(position[i] < nodeProbability);
            if (node.IsTrue())
            {
                potentialEnergy -= Mathf.Log(nodeProbability + 1e-7f);
            }
            else
            {
                potentialEnergy -= Mathf.Log(1f - nodeProbability + 1e-7f);
            }
        }
        return potentialEnergy;
    }

    public float CalculateGradient(Node node, Dictionary<Node, float> position)
    {
        float epsilon = 1e-5f;
        Dictionary<Node, float> positionPlusEpsilon = new Dictionary<Node, float>(position);
        Dictionary<Node, float> positionMinusEpsilon = new Dictionary<Node, float>(position);

        positionPlusEpsilon[node] += epsilon;
        positionMinusEpsilon[node] -= epsilon;
        float potentialEnergyPlus = CalculatePotentialEnergy(positionPlusEpsilon);
        float potentialEnergyMinus = CalculatePotentialEnergy(positionMinusEpsilon);

        float gradient = (potentialEnergyPlus - potentialEnergyMinus) / (2 * epsilon);
        return gradient;
    }

    public override float CalculateProbability()
    {
        List<bool[]> filteredSamplesInQuery = FilterSamples(samples, graph.GetPositiveQuery(), graph.GetNegativeQuery());
        int numberOfAcceptedSamples = samples.Count;

        if (samples.Count == 0)
        {
            Debug.Log("N/A (no samples available)");
            return -1.0f;
        }

        return (float)filteredSamplesInQuery.Count / samples.Count;
    }

    public override int GetNumberOfAcceptedSamples()
    {
        return samples.Count;
    }
}
