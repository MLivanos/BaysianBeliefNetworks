using UnityEngine;
using TMPro;

public class HistoryHook : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI historyText;

	private void Awake()
	{
		if (SamplingHistory.instance == null) return;
		historyText.text = "P(Q|E) ≈ Prob | Samples | Algo\n"+SamplingHistory.instance.historyRecord;
	}
}