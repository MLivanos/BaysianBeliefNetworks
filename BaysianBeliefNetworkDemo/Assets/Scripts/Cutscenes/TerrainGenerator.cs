using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
	[SerializeField] private GameObject[] meshGeneratorPrefab;
	[SerializeField] private GameObject[] effects;
	private Queue<MeshGenerator> chunks = new Queue<MeshGenerator>();

	private int numberOfChunks;
	private GameObject currentEffect;

	public void GenerateChunk()
	{
		if (numberOfChunks >= meshGeneratorPrefab.Length)
		{
		    // End Game
		    #if UNITY_EDITOR
		    	UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in the Unity Editor
		    #else
		    	Application.Quit(); // Quits the application in a built version
		    #endif
		}
		GameObject meshGeneratorObject = Instantiate(meshGeneratorPrefab[numberOfChunks]);
		MeshGenerator meshGenerator = meshGeneratorObject.GetComponent<MeshGenerator>();
		chunks.Enqueue(meshGenerator);
		// TODO: Change 100 to size of chunk
		//meshGenerator.zOffset = 100*numberOfChunks;
		//meshGenerator.transform.Translate(new Vector3(0,0,100*numberOfChunks));
		meshGenerator.GenerateMesh();
		numberOfChunks++;
		CycleEffects();

	}

	public void DeleteChunk()
	{
		MeshGenerator meshGenerator = chunks.Dequeue();
		meshGenerator.DeleteChunk();
	}

	public void CycleEffects()
	{
		int index = numberOfChunks - 2;
		if (index < 0)
		{
			return;
		}
		if (currentEffect != null)
		{
			Destroy(currentEffect);
		}
		// TODO: Figure out where to instantiate effects
		//currentEffect = Instantiate(effects[index], AnTransform);
	}
}