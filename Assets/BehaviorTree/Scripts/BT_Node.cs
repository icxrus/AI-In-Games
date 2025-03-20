using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System;
using static UnityEditor.Progress;



public class BT_Node
{
    public enum Status { Success, Failure, Running }

    public string name;
    public int priority;

    public  List<BT_Node> children = new();
    protected int currentchild;

    public BT_Node(string name = "Node", int priority = 0)
    {
        this.name = name;
        this.priority = priority;
     }

    public void AddChild(BT_Node child) 
    {
        children.Add(child);
    }

    public virtual Status Process() 
    {
       return children[currentchild].Process();
    } 

    public virtual void Reset() 
    {
        currentchild = 0;
        foreach (BT_Node child in children) 
        {
            child.Reset();
        }
    }
}
public class Selector : BT_Node
{
    public Selector(string name, int priority = 0) : base(name, priority) { }

    public override Status Process()
    {
        if (currentchild < children.Count)
        {
            BT_Node.Status isSuccess = children[currentchild].Process();
            switch (isSuccess)
            {

                case Status.Success:
                    Reset();
                    return Status.Success;
                case Status.Running:
                    return Status.Running;
                default:
                    currentchild++;
                    return Status.Running;

            }
        }

        Reset();
        return Status.Failure;
    }
}
public class Leaf : BT_Node 
{
     ITask task;

    public Leaf(string name, ITask task, int priority = 0) : base(name,priority)
    {
        this.task = task;
    }

    public override Status Process() 
    {
        return task.Process();
    } 

    public override void Reset()
    {
        task.Reset();
    }
}
public class BehaviorTree : BT_Node 
{
    public BehaviorTree(string name) : base(name) { }

    public override Status Process()
    {
        while (currentchild<children.Count) 
        {
            var status = children[currentchild].Process();
            if (status !=Status.Success)
            {
                return status;
            }
            currentchild++;
        }
        return Status.Success;
    }
}

public class Sequence : BT_Node 
{
    public Sequence(string name, int priority = 0) : base(name,priority) { }

    public override Status Process()
    {
        if (currentchild<children.Count)
        {
            switch (children[currentchild].Process())
            {
                case Status.Running:
                    return Status.Running;

                case Status.Failure:
                    Reset();
                    return Status.Failure;

                default:

                    currentchild++;
                    if (currentchild==children.Count)
                    {
                        return Status.Success;
                    }
                    else
                    {
                        return Status.Running;
                    }
            }
        }

        Reset();
        return Status.Success;
    }
}

public class PrioritySelector : Selector 
{
    List<BT_Node> SortedChildren;

    List<BT_Node> sortedChildrenReadOnly => SortedChildren ??= SortChildren();

    protected virtual List<BT_Node> SortChildren() 
    {
        return children.OrderByDescending(x=>x.priority).ToList();
    }

    public PrioritySelector(string name,int priority=0) : base(name,priority) { }

    public override void Reset() 
    {
        base.Reset();
        SortedChildren = null;
    }

    public override Status Process()
    {
        foreach (var child in sortedChildrenReadOnly)
        {
            switch (child.Process())
            {
                case Status.Success:
                    return Status.Success;
                case Status.Running:
                    return Status.Running;
                default:
                    continue;
            }
        }
        return Status.Failure;
    }
}

public class RandomSelector : PrioritySelector
{
    System.Random rnd = new System.Random();
    protected override List<BT_Node> SortChildren()
    {
        return children.Select(x => new { value = x, order = rnd.Next() })
            .OrderBy(x => x.order).Select(x => x.value).ToList();
    }

    public RandomSelector(string name, int priority =0) : base(name,priority) { }

}

public class Inverter : BT_Node 
{
    public Inverter(string name) : base(name) { }

    public override Status Process()
    {
        switch (children[0].Process())
        {
            

            case Status.Failure:
                return Status.Success;

            case Status.Running:
                return Status.Running;

            default:
                return Status.Failure;
        }
    }
}

public class UntilFail : BT_Node 
{
    public UntilFail(string name) : base(name) { }

    public override Status Process()
    {
        if (children[0].Process() == Status.Failure) 
        {
            Reset(); 
            return Status.Failure;
        }
        return Status.Running;
    }
}

//UntilSuccess

//Repeat




