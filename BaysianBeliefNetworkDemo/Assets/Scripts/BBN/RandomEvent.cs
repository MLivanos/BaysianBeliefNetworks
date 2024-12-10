using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RandomEventOperation
{
	private enum Operation{
		SetAToB,
		SetAToScalar,
		MultiplyAByScalar,
		AddScalarToA,
		InvertA,
		ReturnToOriginal
	}

	[SerializeField] private List<ProbabilityValue> ProbabilityListA;
	[SerializeField] private List<ProbabilityValue> ProbabilityListB;
	[SerializeField] private List<int> indices;
	[SerializeField] private float scalar;
	[SerializeField] private Operation operation;

	private Dictionary<Operation, System.Action> operations;

	private RandomEventOperation(List<ProbabilityValue> probabilityList, List<int> indicesList)
	{
	    ProbabilityListA = probabilityList;
	    ProbabilityListB = new List<ProbabilityValue>();
	    indices = indicesList;
	    scalar = 0f;
	    operation = Operation.ReturnToOriginal;
	}

	public void Initialize()
	{
	    operations = new Dictionary<Operation, System.Action>
	    {
	        { Operation.SetAToB, SetAToB },
	        { Operation.SetAToScalar, SetAToScalar },
	        { Operation.MultiplyAByScalar, MultiplyAByScalar },
	        { Operation.AddScalarToA, AddScalarToA },
	        { Operation.InvertA, InvertA },
	        { Operation.ReturnToOriginal, ReturnToOriginal }
	    };
	}

	public void ApplyOperation()
	{
	    operations[operation]();
	}

	public void ReverseOperation()
	{
		ReturnToOriginal();
	}

	private void ApplyToA(System.Func<int, float, float> operation)
	{
	    for (int i = 0; i < ProbabilityListA.Count; i++)
	    {
	        float currentProbability = ProbabilityListA[i].GetProbability(indices[i]);
	        float updatedProbability = operation(i, currentProbability);
	        ProbabilityListA[i].UpdateProbability(indices[i], updatedProbability);
	    }
	}

	private void SetAToB() =>
	    ApplyToA((i, _) => ProbabilityListB[i].GetProbability(i));

	private void SetAToScalar() =>
	    ApplyToA((i, _) => scalar);

	private void MultiplyAByScalar() =>
	    ApplyToA((i, current) => current * scalar);

	private void AddScalarToA() =>
	    ApplyToA((i, current) => current + scalar);

	private void InvertA() =>
	    ApplyToA((i, current) => 1.0f - current);

	private void ReturnToOriginal() =>
	    ApplyToA((i, _) => ProbabilityListA[i].GetOriginalValue(indices[i]));

	public RandomEventOperation GetOperationInverse()
	{
		return new RandomEventOperation(ProbabilityListA, indices);
	}
}

public class RandomEvent : MonoBehaviour
{
	[SerializeField] private List<RandomEventOperation> operations;
	[SerializeField] private string message;
	[SerializeField] private string eventDescription;
	[SerializeField] private string inverse;
	[SerializeField] private string inverseEventDescription;
	private bool isInverse;

	public void Initialize()
	{
		foreach(RandomEventOperation operation in operations)
		{
			operation.Initialize();
		}
	}

	public void ApplyOperations()
	{
		foreach(RandomEventOperation operation in operations)
		{
			if (!isInverse) operation.ApplyOperation();
			else operation.ReverseOperation();
		}
		isInverse = !isInverse;
	}

	public string GetMessage()
	{
		return isInverse ? inverse : message;
	}

	public string GetDescription()
	{
		return isInverse ? inverseEventDescription : eventDescription;
	}
}