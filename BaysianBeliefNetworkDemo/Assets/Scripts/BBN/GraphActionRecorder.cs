using UnityEngine;
using System.Collections.Generic;

public enum ActionKind { AddQuery, RemoveQuery, AddEvidence, RemoveEvidence }

public struct ActionRecord
{
    public ActionKind Kind;
    public Node Node;
    public bool IsTrue; // for Add*/Remove*: which polarity (true/false) it affected

    public ActionRecord(ActionKind kind, Node node, bool isTrue)
    { Kind = kind; Node = node; IsTrue = isTrue; }
}

public class GraphActionRecorder : MonoBehaviour
{
    private Graph _graph;

    // History cursor points to the NEXT action to redo.
    private readonly List<ActionRecord> _history = new();
    private int _cursor = 0;

    public bool CanUndo => _cursor > 0;
    public bool CanRedo => _cursor < _history.Count;

    private void Awake() => _graph = GetComponent<Graph>();

    public void RecordAddQuery(Node node, bool isTrue)
        => Record(new ActionRecord(ActionKind.AddQuery, node, isTrue));

    public void RecordRemoveQuery(Node node)
    {
        bool wasTrue = _graph.GetPositiveQuery().Contains(node);
        Record(new ActionRecord(ActionKind.RemoveQuery, node, wasTrue));
    }

    public void RecordAddEvidence(Node node, bool isTrue)
        => Record(new ActionRecord(ActionKind.AddEvidence, node, isTrue));

    public void RecordRemoveEvidence(Node node)
    {
        bool wasTrue = _graph.GetPositiveEvidence().Contains(node);
        Record(new ActionRecord(ActionKind.RemoveEvidence, node, wasTrue));
    }

    public void Undo()
    {
        if (!CanUndo) return;
        var ar = _history[--_cursor];
        ApplyInverse(ar);
    }

    public void Redo()
    {
        if (!CanRedo) return;
        var ar = _history[_cursor++];
        Apply(ar);
    }

    public void ClearHistory()
    {
        _history.Clear();
        _cursor = 0;
    }

    private void Record(ActionRecord ar)
    {
        if (_cursor < _history.Count)
            _history.RemoveRange(_cursor, _history.Count - _cursor);

        _history.Add(ar);
        _cursor++;
        Debug.Log("?");
    }

    private void Apply(ActionRecord ar)
    {
        switch (ar.Kind)
        {
            case ActionKind.AddQuery:
                _graph.AddToQuery(ar.Node, ar.IsTrue, record:false);
                break;
            case ActionKind.RemoveQuery:
                _graph.RemoveFromQuery(ar.Node, record:false);
                break;
            case ActionKind.AddEvidence:
                _graph.AddToEvidence(ar.Node, ar.IsTrue, record:false);
                break;
            case ActionKind.RemoveEvidence:
                _graph.RemoveFromEvidence(ar.Node, record:false);
                break;
        }
    }

    private void ApplyInverse(ActionRecord ar)
    {
        switch (ar.Kind)
        {
            case ActionKind.AddQuery:
                _graph.RemoveFromQuery(ar.Node, record:false);
                break;
            case ActionKind.RemoveQuery:
                _graph.AddToQuery(ar.Node, ar.IsTrue, record:false);
                break;
            case ActionKind.AddEvidence:
                _graph.RemoveFromEvidence(ar.Node, record:false);
                break;
            case ActionKind.RemoveEvidence:
                _graph.AddToEvidence(ar.Node, ar.IsTrue, record:false);
                break;
        }
    }
}